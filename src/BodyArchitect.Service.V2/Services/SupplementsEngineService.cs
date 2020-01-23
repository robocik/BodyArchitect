using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Portable;
using BodyArchitect.Service.V2.Localization;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Criterion.Lambda;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using DosageType = BodyArchitect.Model.DosageType;
using DosageUnit = BodyArchitect.Service.V2.Model.DosageUnit;
using EntryObjectStatus = BodyArchitect.Model.EntryObjectStatus;
using ExerciseType = BodyArchitect.Model.ExerciseType;
using ReminderRepetitions = BodyArchitect.Model.ReminderRepetitions;
using ReminderType = BodyArchitect.Model.ReminderType;
using SupplementCycleDayRepetitions = BodyArchitect.Service.V2.Model.SupplementCycleDayRepetitions;
using TimeType = BodyArchitect.Model.TimeType;
using TrainingEnd = BodyArchitect.Model.TrainingEnd;
using NHibernate.Transform;
using System.Collections;
using Constants = BodyArchitect.Portable.Constants;
using PublishStatus = BodyArchitect.Model.PublishStatus;
using TrainingPlanDifficult = BodyArchitect.Service.V2.Model.TrainingPlans.TrainingPlanDifficult;
using WorkoutPlanPurpose = BodyArchitect.Model.WorkoutPlanPurpose;

namespace BodyArchitect.Service.V2.Services
{
    public enum SupplementsCycleTrainingType
    {
        Strength,
        Cardio,
        Both
    }

    public class TrainingDayItem
    {
        public SupplementsCycleTrainingType TrainingType { get; set; }

        public DayOfWeek Day { get; set; }

        public static TrainingDayItem Create(string dayString)
        {
            TrainingDayItem item = new TrainingDayItem();
            item.Day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), dayString.Substring(0, 1));
            switch (dayString.Substring(1, 1))
            {
                case "B":
                    item.TrainingType = SupplementsCycleTrainingType.Both;
                    break;
                case "C":
                    item.TrainingType = SupplementsCycleTrainingType.Cardio;
                    break;
                default:
                    item.TrainingType = SupplementsCycleTrainingType.Strength;
                    break;
            }
            return item;
        }
    }

    internal class SupplementsEngineService : ServiceBase
    {
        public SupplementsEngineService(ISession session, SecurityInfo securityInfo, ServiceConfiguration configuration)
            : base(session, securityInfo, configuration)
        {
        }

        private void ensureCanStartMyTraining(MyTraining myTraining, Profile dbProfile)
        {
            if (!myTraining.IsNew)
            {
                throw new InvalidOperationException("This cycle cannot be started");
            }
            if (myTraining.EndDate.HasValue || myTraining.TrainingEnd != TrainingEnd.NotEnded ||
                myTraining.PercentageCompleted != 0)
            {
                throw new ArgumentException("Wrong parameters");
            }
            if(myTraining.StartDate==DateTime.MinValue)
            {
                throw new ArgumentException("Wrong start date");
            }
            if (myTraining.Customer != null && myTraining.Customer.Profile != dbProfile)
            {
                throw new CrossProfileOperationException("Customer not belong to this profile");
            }
        }
        
        private void createEntryObject(MyTrainingDTO param, int dayIndex, Profile dbProfile, MyTraining dbCycle,Func<IEnumerable<EntryObject>> createMethod)
        {
            var entries = createMethod();
            foreach (var entry in entries)
            {
                var newDate = dbCycle.StartDate.Date.AddDays(dayIndex);
                var trainingDay = Session.QueryOver<TrainingDay>().Where(x => x.TrainingDate == newDate && x.Profile == dbCycle.Profile && x.Customer == dbCycle.Customer).SingleOrDefault();
                if (trainingDay == null)
                {
                    trainingDay = new TrainingDay(newDate);
                    trainingDay.Profile = dbProfile;
                    trainingDay.AllowComments = dbProfile.Settings.AllowTrainingDayComments;
                    trainingDay.Customer = dbCycle.Customer;
                }

                entry.Status = EntryObjectStatus.Planned;
                trainingDay.AddEntry(entry);
                dbCycle.EntryObjects.Add(entry);
                entry.LoginData = SecurityInfo.LoginData;
                entry.MyTraining = dbCycle;

                if (param.RemindBefore.HasValue)
                {
                    entry.Reminder = new ReminderItem();

                    entry.Reminder.DateTime = entry.TrainingDay.TrainingDate;
                    entry.Reminder.Profile = dbProfile;
                    entry.Reminder.Type = ReminderType.EntryObject;
                    entry.Reminder.RemindBefore = param.RemindBefore != TimeSpan.Zero
                                                      ? param.RemindBefore.Value
                                                      : (TimeSpan?)null;
                    entry.Reminder.Repetitions = ReminderRepetitions.Once;
                    entry.Reminder.Name = string.Format(LocalizedStrings.SupplementDefinitionEntry_ReminderName, dbCycle.Name);
                    Session.Save(entry.Reminder);
                }

                Session.SaveOrUpdate(trainingDay);
                Session.Flush();
                if (entry.Reminder != null)
                {
                    entry.Reminder.ConnectedObject = string.Format("EntryObjectDTO:{0}", entry.GlobalId);
                    Session.Update(entry.Reminder);
                }
            }
        }

        private List<SupplementCycleEntry> getDayDosages(SupplementCycleDefinition definition, int currentDay,SupplementCycle cycle)
        {
            var trainingDays = !string.IsNullOrEmpty(cycle.TrainingDays)
                                   ? cycle.TrainingDays.Split(',').Select(x => TrainingDayItem.Create(x)).ToList()
                                   : new List<TrainingDayItem>();
            var newDate = cycle.StartDate.AddDays(currentDay);
            int week = (int) ((newDate - cycle.StartDate).TotalDays/7) + 1;
            List<SupplementCycleEntry> dosages = new List<SupplementCycleEntry>();
            foreach (var day in definition.Weeks)
            {
                if (week >= day.CycleWeekStart && week <= day.CycleWeekEnd)
                {
                    foreach (var dosage in day.Dosages)
                    {
                        if (dosage.Repetitions == BodyArchitect.Model.SupplementCycleDayRepetitions.EveryDay)
                        {
                            dosages.Add(dosage);
                            continue;
                        }
                        if (dosage.Repetitions == BodyArchitect.Model.SupplementCycleDayRepetitions.OnceAWeek &&
                            newDate.DayOfWeek == cycle.StartDate.DayOfWeek)
                        {
                            dosages.Add(dosage);
                            continue;
                        }
                        if (dosage.Repetitions == BodyArchitect.Model.SupplementCycleDayRepetitions.StrengthTrainingDay &&
                            trainingDays.Where(x => x.Day == newDate.DayOfWeek && (x.TrainingType == SupplementsCycleTrainingType.Strength || x.TrainingType == SupplementsCycleTrainingType.Both)).Count() > 0)
                        {
                            dosages.Add(dosage);
                            continue;
                        }
                        if (dosage.Repetitions == BodyArchitect.Model.SupplementCycleDayRepetitions.CardioTrainingDay &&
                            trainingDays.Where(x => x.Day == newDate.DayOfWeek && (x.TrainingType == SupplementsCycleTrainingType.Cardio || x.TrainingType == SupplementsCycleTrainingType.Both)).Count() > 0)
                        {
                            dosages.Add(dosage);
                            continue;
                        }
                        if (dosage.Repetitions == BodyArchitect.Model.SupplementCycleDayRepetitions.NonTrainingDay &&
                            trainingDays.Where(x => x.Day == newDate.DayOfWeek).Count()==0)
                        {
                            dosages.Add(dosage);
                            continue;
                        }
                    }
                }
            }
            return dosages;
        }

        public MyTrainingDTO MyTrainingOperation(MyTrainingOperationParam param)
        {
            Log.WriteWarning("MyTrainingOperation:Username={0},operation={1}",SecurityInfo.SessionData.Profile.UserName, param.Operation);

            using (var trans = Session.BeginSaveTransaction())
            {
                var dbProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
                MyTraining dbCycle = null;
                if (param.Operation == MyTrainingOperationType.Start || param.Operation == MyTrainingOperationType.Simulate)
                {
                    dbCycle = param.MyTraining.Map<MyTraining>();
                    ensureCanStartMyTraining(dbCycle, dbProfile);
                    dbCycle.Profile = dbProfile;
                    if (dbCycle is A6WTraining)
                    {
                        startA6WTraining(param, (A6WTraining)dbCycle, dbProfile);
                    }
                    else if(dbCycle is SupplementCycle)
                    {
                        SupplementsCycleDTO cycleDto = (SupplementsCycleDTO) param.MyTraining;
                        startSupplementsCycle(param, (SupplementCycle)dbCycle, dbProfile, cycleDto.SupplementsCycleDefinitionId);
                    }
                    Session.Flush();
                    ProfileStatisticsUpdater.UpdateTrainindDay(Session, dbProfile);
                }
                else
                {
                    dbCycle = StopMyTraining(param.MyTraining.GlobalId, dbProfile);
                }
                Session.SaveOrUpdate(dbCycle);

                if (param.Operation != MyTrainingOperationType.Simulate)
                {//for simulate we must reject all changes in the db
                    trans.Commit();
                }

                return dbCycle.Map<MyTrainingDTO>();
            }
        }

        private void startSupplementsCycle(MyTrainingOperationParam param, SupplementCycle dbCycle, Profile dbProfile,Guid supplementsCycleDefinitionId)
        {
            dbCycle.Profile = dbProfile;

            var dbCycleDefinition =dbCycle.SupplementsCycleDefinition =Session.QueryOver<SupplementCycleDefinition>().Where(x => x.GlobalId == supplementsCycleDefinitionId).Fetch(x => x.Weeks).Eager.Fetch(x => x.Weeks.First().Dosages).Eager.SingleOrDefault();
            var definitionDTO=dbCycleDefinition.Map<SupplementCycleDefinitionDTO>();
            SupplementsCycleRepetiter repetiter = new SupplementsCycleRepetiter();
            var preparedCycleDefinition = repetiter.Preapre(definitionDTO, dbCycle.TotalWeeks);
            dbCycleDefinition = preparedCycleDefinition.Map<SupplementCycleDefinition>();

            for (int i = 0; i < dbCycleDefinition.GetTotalDays(dbCycle.TotalWeeks); i++)
            {
                createEntryObject(param.MyTraining, i, dbProfile, dbCycle, () =>
                {
                    List<EntryObject> entries = new List<EntryObject>();
                    var dosages = getDayDosages(dbCycleDefinition, i, dbCycle);
                    if (dosages.Count == 0)
                    {
                        return entries;
                    }
                    if(dosages.OfType<SupplementCycleMeasurement>().Count()>0)
                    {
                        SizeEntry sizeEntry = new SizeEntry();
                        entries.Add(sizeEntry);
                    }
                    var supplementEntries = dosages.OfType<SupplementCycleDosage>().ToList();
                    if(supplementEntries.Count==0)
                    {
                        return entries;
                    }
                    SuplementsEntry entry = new SuplementsEntry();
                    foreach (var dosage in supplementEntries)
                    {
                        SuplementItem item = new SuplementItem();
                        entry.AddItem(item);
                        item.Name = dosage.Name;
                        item.Time.TimeType = (TimeType)dosage.TimeType;
                        item.Suplement = Session.Load
                                <Suplement>(dosage.Supplement.GlobalId);
                        item.Dosage = dosage.Dosage;
                        if (dosage.DosageUnit ==BodyArchitect.Model.DosageUnit.ON10KgWight)
                        {
                            item.Dosage = (dosage.Dosage * (dbCycle.Weight / 10M)).RoundToNearestHalf();
                        }
                        item.DosageType = (DosageType)dosage.DosageType;
                    }
                    entries.Add(entry);
                    return entries;
                });
            }
        }
        private void startA6WTraining(MyTrainingOperationParam param, A6WTraining dbCycle, Profile dbProfile)
        {
            for (int i = 0; i < A6WManager.Days.Count; i++)
            {
                createEntryObject(param.MyTraining, i, dbProfile, dbCycle, () =>
                                                                               {
                                                                                   var entry = new A6WEntry();
                                                                                   entry.DayNumber = i + 1;
                                                                                   return new []{entry};
                                                                               });
            }
        }

        internal MyTraining StopMyTraining(Guid myTrainingId, Profile dbProfile)
        {
            var dbCycle=Session.QueryOver<MyTraining>()
                .Fetch(x => x.EntryObjects).Eager
                .Fetch(x=>x.EntryObjects.First().TrainingDay).Eager
                .Fetch(x => (((SuplementsEntry) x.EntryObjects.First()).Items).First().Suplement).Eager
                .Fetch(x => ((StrengthTrainingEntry) x.EntryObjects.First()).Entries).Eager
                .Fetch(x => ((StrengthTrainingEntry) x.EntryObjects.First()).MyPlace).Eager
                .Fetch(x => (((StrengthTrainingEntry) x.EntryObjects.First()).Entries).First().Exercise).Eager
                .Fetch(x => (((StrengthTrainingEntry) x.EntryObjects.First()).Entries.First().Series)).Eager
                .Where(
                    x => x.GlobalId == myTrainingId).SingleOrDefault();
            dbCycle = Session.Get<MyTraining>(myTrainingId);
            if (dbCycle.Profile != dbProfile)
            {
                throw new CrossProfileOperationException("MyTraining doesn't belong to this profile");
            }
            dbCycle.Complete(Configuration.TimerService);
            var plannedEntries = dbCycle.EntryObjects
                        .Where(x => x.Status == EntryObjectStatus.Planned).ToList();
            bool trainingDayDeleted = false;

            foreach (var entryObject in plannedEntries)
            {
                dbCycle.EntryObjects.Remove(entryObject);
                var td = entryObject.TrainingDay;
                td.RemoveEntry(entryObject);
                Session.Delete(entryObject);
                if (td.IsEmpty)
                {
                    Session.Delete(td);
                    trainingDayDeleted = true;
                }
            }
            Session.Flush();
            if (trainingDayDeleted)
            {
                ProfileStatisticsUpdater.UpdateTrainindDay(Session, dbProfile);
            }
            Session.Update(dbCycle);
            return dbCycle;
        }

        public PagedResult<SupplementCycleDefinitionDTO> GetSupplementsCycleDefinitions(GetSupplementsCycleDefinitionsParam param, PartialRetrievingInfo pageInfo)
        {
            Log.WriteWarning("GetSupplementsCycleDefinitions:Username={0}", SecurityInfo.SessionData.Profile.UserName);

            using (var transactionScope = Session.BeginGetTransaction())
            {

                var myProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
                var loggedProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
                if (param.UserId.HasValue)
                {
                    myProfile = Session.Load<Profile>(param.UserId.Value);
                }
                
                var ids = myProfile.FavoriteSupplementCycleDefinitions.Select(x => x.GlobalId).ToList();

                var idsQuery =Session.QueryOver<SupplementCycleDefinition>();
                idsQuery = getSupplementsCycleDefinitionsCriterias(param, loggedProfile, ids, myProfile, idsQuery);
                idsQuery = idsQuery.ApplySorting(param.SortOrder, param.SortAscending);

                var fetchQuery =
                    Session.QueryOver<SupplementCycleDefinition>()
                        .Fetch(x => x.Weeks).Eager
                        .Fetch(x => x.Weeks.First()).Eager
                        .Fetch(x => x.Weeks.First().Dosages).Eager
                        .Fetch(x => ((SupplementCycleDosage) x.Weeks.First().Dosages.First()).Supplement).Eager
                        .Fetch(x => x.Profile).Eager;
                    
                fetchQuery = fetchQuery.ApplySorting(param.SortOrder, param.SortAscending);


                var res1 = (from rv in Session.Query<RatingUserValue>()
                            from tp in Session.Query<SupplementCycleDefinition>()
                            where tp.GlobalId == rv.RatedObjectId &&
                                rv.ProfileId == SecurityInfo.SessionData.Profile.GlobalId
                            select rv).ToDictionary(t => t.RatedObjectId);

                var listPack = fetchQuery.ToExPagedResults
                    (pageInfo, idsQuery, delegate(IEnumerable<SupplementCycleDefinition> list)
                      {
                          var output = new List<SupplementCycleDefinitionDTO>();
                          foreach (var planDto in list)
                          {

                              var tmp = planDto.Map<SupplementCycleDefinitionDTO>();
                              if (res1.ContainsKey(planDto.GlobalId))
                              {
                                  tmp.UserRating = res1[planDto.GlobalId].Rating;
                                  tmp.UserShortComment = res1[planDto.GlobalId].ShortComment;
                              }
                              output.Add(tmp);
                          }
                          return output.ToArray();
                      });
                transactionScope.Commit();
                return listPack;
            }

        }

        private static IQueryOver<SupplementCycleDefinition, SupplementCycleDefinition> getSupplementsCycleDefinitionsCriterias(GetSupplementsCycleDefinitionsParam param,Profile loggedProfile, List<Guid> ids, Profile myProfile,
                                                                          IQueryOver<SupplementCycleDefinition, SupplementCycleDefinition> queryCustomer)
        {
            SupplementCycleDosage dosage = null;
            SupplementCycleWeek week = null;
            queryCustomer=queryCustomer.JoinAlias(x => x.Weeks, () => week)
                .JoinAlias(x => x.Weeks.First().Dosages, () => dosage);

            if (param.LegalCriteria == CanBeIllegalCriteria.OnlyLegal)
            {
                queryCustomer = queryCustomer.Where(x => !x.CanBeIllegal);
            }
            else if (param.LegalCriteria == CanBeIllegalCriteria.OnlyIllegal)
            {
                queryCustomer = queryCustomer.Where(x => x.CanBeIllegal);
            }

            if(param.PlanId.HasValue)
            {
                queryCustomer = queryCustomer.Where(x => x.GlobalId==param.PlanId.Value);
            }
            if (param.Languages.Count > 0)
            {
                var langOr = Restrictions.Disjunction();
                foreach (var lang in param.Languages)
                {
                    langOr.Add<SupplementCycleDefinition>(x => x.Language == lang);
                }
                queryCustomer = queryCustomer.And(langOr);
            }
            if (param.Purposes.Count > 0)
            {
                var purposeOr = Restrictions.Disjunction();
                foreach (var purpose in param.Purposes)
                {
                    purposeOr.Add<SupplementCycleDefinition>(x => x.Purpose == (WorkoutPlanPurpose) purpose);
                }
                queryCustomer = queryCustomer.And(purposeOr);
            }
            if (param.Difficults.Count > 0)
            {
                var mainOr = Restrictions.Disjunction();
                foreach (TrainingPlanDifficult diff in param.Difficults)
                {
                    var tt = (BodyArchitect.Model.TrainingPlanDifficult) diff;
                    mainOr.Add<SupplementCycleDefinition>(x => x.Difficult == tt);
                }
                queryCustomer = queryCustomer.And(mainOr);
            }

            if (param.Supplements.Count > 0)
            {
                Junction supplementsOperations = null;
                if (param.SupplementsListOperator == CriteriaOperator.Or)
                {
                    supplementsOperations = Restrictions.Disjunction();
                    foreach (var supplementId in param.Supplements)
                    {
                        supplementsOperations.Add<SupplementCycleDefinition>(x => dosage.Supplement.GlobalId == supplementId);
                    }
                }
                else
                {
                    supplementsOperations = Restrictions.Conjunction();
                    foreach (var supplementId in param.Supplements)
                    {
                        var orderIdsCriteria = DetachedCriteria.For<SupplementCycleDosage>();
                        orderIdsCriteria.SetProjection(Projections.CountDistinct("GlobalId"))
                            .Add(Restrictions.Where<SupplementCycleDosage>(x => x.Supplement.GlobalId == supplementId))
                            .Add(Restrictions.Where<SupplementCycleDosage>(x => x.Week.GlobalId == week.GlobalId));

                        supplementsOperations.Add(Subqueries.Lt(0, orderIdsCriteria));
                        //supplementsOperations.Add<SupplementCycleDosage>(x => dosage.Supplement.GlobalId == supplementId);
                    }
                }
                queryCustomer = queryCustomer.And(supplementsOperations);
            }

            queryCustomer =queryCustomer.Where(x => x.Profile == loggedProfile || (x.Profile != loggedProfile && x.Status == PublishStatus.Published));

            var groupOr = new Disjunction();
            if (param.SearchGroups.Count > 0)
            {
                if (param.SearchGroups.IndexOf(WorkoutPlanSearchCriteriaGroup.Mine) > -1)
                {
                    groupOr.Add<BodyArchitect.Model.SupplementCycleDefinition>(x => x.Profile == myProfile);
                }
                if (param.SearchGroups.IndexOf(WorkoutPlanSearchCriteriaGroup.Favorites) > -1)
                {
                    if (myProfile.FavoriteSupplementCycleDefinitions.Count > 0)
                    {
                        groupOr.Add<BodyArchitect.Model.SupplementCycleDefinition>(x => x.GlobalId.IsIn((ICollection) ids));
                    }
                }
                if (param.SearchGroups.IndexOf(WorkoutPlanSearchCriteriaGroup.Other) > -1)
                {
                    var tmpAnd = Restrictions.Conjunction();
                    tmpAnd.Add<BodyArchitect.Model.SupplementCycleDefinition>(
                        dto => dto.Profile != null && dto.Profile != myProfile);

                    if (ids.Count > 0)
                    {
                        tmpAnd.Add(Restrictions.On<BodyArchitect.Model.SupplementCycleDefinition>(x => x.GlobalId).Not.IsIn((ICollection) ids));
                    }

                    groupOr.Add(tmpAnd);
                }
                queryCustomer = queryCustomer.Where(groupOr);
            }
            return queryCustomer;
        }

        //public PagedResult<SupplementCycleDefinitionDTO> GetSupplementsCycleDefinitions(GetSupplementsCycleDefinitionsParam param, PartialRetrievingInfo pageInfo)
        //{
        //    Log.WriteWarning("GetSupplementsCycleDefinitions:Username={0}", SecurityInfo.SessionData.Profile.UserName);

        //    using (var transactionScope = Session.BeginTransaction())
        //    {
        //        //var dbProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.Id);
        //        SupplementCycleDosage dosage = null;
        //        SupplementCycleWeek week = null;

        //        var myProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.Id);
        //        var loggedProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.Id);
        //        if (param.UserId.HasValue)
        //        {
        //           myProfile = Session.Load<Profile>(param.UserId.Value);
        //        }

        //        var ids = myProfile.FavoriteSupplementCycleDefinitions.Select(x => x.GlobalId).ToList();
        //        //this commented query works where some definitions don't have weeks and/or dosages 
        //        //var queryCustomer = Session.QueryOver<SupplementCycleDefinition>().JoinAlias(x => x.Weeks, () => week, JoinType.LeftOuterJoin).JoinAlias(x => x.Weeks.First().Dosages, () => dosage, JoinType.LeftOuterJoin);
        //        var queryCustomer =
        //            Session.QueryOver<SupplementCycleDefinition>()
        //            .JoinAlias(x => x.Weeks, () => week)
        //            .JoinAlias(x => x.Weeks.First().Dosages, () => dosage)
        //            .Fetch(x => ((SupplementCycleDosage)x.Weeks.First().Dosages.First()).Supplement).Eager
        //            .Fetch(x => x.Profile).Eager;

        //        if(param.LegalCriteria==CanBeIllegalCriteria.OnlyLegal)
        //        {
        //            queryCustomer = queryCustomer.Where(x =>!x.CanBeIllegal);
        //        }
        //        else if (param.LegalCriteria == CanBeIllegalCriteria.OnlyIllegal)
        //        {
        //            queryCustomer = queryCustomer.Where(x => x.CanBeIllegal);
        //        }
        //        if (param.Languages.Count > 0)
        //        {
        //            var langOr = Restrictions.Disjunction();
        //            foreach (var lang in param.Languages)
        //            {
        //                langOr.Add<SupplementCycleDefinition>(x => x.Language == lang);
        //            }
        //            queryCustomer = queryCustomer.And(langOr);

        //        }
        //        if (param.Purposes.Count > 0)
        //        {
        //            var purposeOr = Restrictions.Disjunction();
        //            foreach (var purpose in param.Purposes)
        //            {
        //                purposeOr.Add<SupplementCycleDefinition>(x => x.Purpose == (WorkoutPlanPurpose)purpose);
        //            }
        //            queryCustomer = queryCustomer.And(purposeOr);
        //        }
        //        if (param.Difficults.Count > 0)
        //        {
        //            var mainOr = Restrictions.Disjunction();
        //            foreach (TrainingPlanDifficult diff in param.Difficults)
        //            {
        //                var tt = (BodyArchitect.Model.TrainingPlanDifficult)diff;
        //                mainOr.Add<SupplementCycleDefinition>(x => x.Difficult == tt);
        //            }
        //            queryCustomer=queryCustomer.And(mainOr);
        //        }
        //        //if (param.Supplements.Count > 0)
        //        //{
        //        //    if (param.SupplementsListOperator == CriteriaOperator.Or)
        //        //    {
        //        //        var supplementsOperations = Restrictions.Disjunction();
        //        //        foreach (var supplementId in param.Supplements)
        //        //        {
        //        //            supplementsOperations.Add<SupplementCycleDefinition>(x => dosage.Supplement.GlobalId == supplementId);
        //        //        }
        //        //        queryCustomer = queryCustomer.And(supplementsOperations);
        //        //    }
        //        //    else
        //        //    {
        //        //        var supplementsOperations = Restrictions.Conjunction();
        //        //        supplementsOperations.Add(Restrictions.On<Suplement>(x => x.GlobalId).IsIn((ICollection)param.Supplements));
        //        //        queryCustomer = queryCustomer.And(supplementsOperations);

        //        //    }

        //        //if (param.Supplements.Count > 0)
        //        //{
        //        //    if (param.SupplementsListOperator == CriteriaOperator.Or)
        //        //    {
        //        //        var supplementsOperations = Restrictions.Disjunction();
        //        //        foreach (var supplementId in param.Supplements)
        //        //        {
        //        //            supplementsOperations.Add<SupplementCycleDefinition>(
        //        //                x => dosage.Supplement.GlobalId == supplementId);
        //        //        }
        //        //        queryCustomer = queryCustomer.And(supplementsOperations);
        //        //    }
        //        //    else
        //        //    {
        //        //        var supplementsOperations = Restrictions.Conjunction();

        //        //        supplementsOperations.Add(
        //        //            Restrictions.On<BodyArchitect.Model.Suplement>(x => dosage.Supplement.GlobalId).IsIn(
        //        //                (ICollection) param.Supplements));
        //        //        queryCustomer = queryCustomer.And(supplementsOperations);

        //        //    }

        //            //}
        //        if (param.Supplements.Count > 0)
        //        {
        //            Junction supplementsOperations = null;
        //            if (param.SupplementsListOperator == CriteriaOperator.Or)
        //            {
        //                supplementsOperations = Restrictions.Disjunction();
        //                foreach (var supplementId in param.Supplements)
        //                {
        //                    supplementsOperations.Add<SupplementCycleDefinition>(x => dosage.Supplement.GlobalId == supplementId);
        //                }
                        
        //            }
        //            else
        //            {
        //                supplementsOperations = Restrictions.Conjunction();
        //                foreach (var supplementId in param.Supplements)
        //                {
        //                    var orderIdsCriteria = DetachedCriteria.For<SupplementCycleDosage>();
        //                    orderIdsCriteria.SetProjection(Projections.CountDistinct("GlobalId"))
        //                    .Add(Restrictions.Where<SupplementCycleDosage>(x => x.Supplement.GlobalId == supplementId))
        //                    .Add(Restrictions.Where<SupplementCycleDosage>(x => x.Week.GlobalId == week.GlobalId));

        //                    supplementsOperations.Add(Subqueries.Lt(0, orderIdsCriteria));
        //                    //supplementsOperations.Add<SupplementCycleDosage>(x => dosage.Supplement.GlobalId == supplementId);
        //                }

        //            }
        //            queryCustomer = queryCustomer.And(supplementsOperations);
        //        }

        //        queryCustomer = queryCustomer.Where(x =>x.Profile==loggedProfile ||  (x.Profile != loggedProfile && x.Status == PublishStatus.Published));

        //        var groupOr = new Disjunction();
        //        if (param.SearchGroups.Count > 0)
        //        {

        //            if (param.SearchGroups.IndexOf(WorkoutPlanSearchCriteriaGroup.Mine) > -1)
        //            {
        //                groupOr.Add<BodyArchitect.Model.SupplementCycleDefinition>(x => x.Profile == myProfile);
        //            }
        //            if (param.SearchGroups.IndexOf(WorkoutPlanSearchCriteriaGroup.Favorites) > -1)
        //            {
        //                if (myProfile.FavoriteSupplementCycleDefinitions.Count > 0)
        //                {
        //                    groupOr.Add<BodyArchitect.Model.SupplementCycleDefinition>(x => x.GlobalId.IsIn((ICollection)ids));
        //                }
        //            }
        //            if (param.SearchGroups.IndexOf(WorkoutPlanSearchCriteriaGroup.Other) > -1)
        //            {
        //                var tmpAnd = Restrictions.Conjunction();
        //                tmpAnd.Add<BodyArchitect.Model.SupplementCycleDefinition>(dto => dto.Profile != null && dto.Profile != myProfile);

        //                if (ids.Count > 0)
        //                {
        //                    tmpAnd.Add(Restrictions.On<BodyArchitect.Model.SupplementCycleDefinition>(x => x.GlobalId).Not.IsIn((ICollection)ids));
        //                }

        //                groupOr.Add(tmpAnd);
        //            }
        //            queryCustomer = queryCustomer.Where(groupOr);
        //        }
                
        //        queryCustomer=queryCustomer.ApplySorting(param.SortOrder, param.SortAscending);

        //        var res1 = (from rv in Session.Query<RatingUserValue>()
        //                    from tp in Session.Query<SupplementCycleDefinition>()
        //                    where tp.GlobalId == rv.RatedObjectId &&
        //                        rv.ProfileId == SecurityInfo.SessionData.Profile.Id
        //                    select rv).ToDictionary(t => t.RatedObjectId);
        //            queryCustomer = queryCustomer.TransformUsing(Transformers.DistinctRootEntity);
        //            var listPack =
        //                queryCustomer.ToPagedResults<SupplementCycleDefinitionDTO, SupplementCycleDefinition>(pageInfo, "GlobalId",
        //                  delegate(IEnumerable<SupplementCycleDefinition> list)
        //                  {
        //                      var output = new List<SupplementCycleDefinitionDTO>();
        //                      foreach (var planDto in list)
        //                      {

        //                          var tmp = planDto.Map<SupplementCycleDefinitionDTO>();
        //                          if (res1.ContainsKey(planDto.GlobalId))
        //                          {
        //                              tmp.UserRating = res1[planDto.GlobalId].Rating;
        //                              tmp.UserShortComment = res1[planDto.GlobalId].ShortComment;
        //                          }
        //                          output.Add(tmp);
        //                      }
        //                      return output.ToArray();
        //                  });
        //            transactionScope.Commit();
        //            return listPack;
        //        }

        //    }


        public SupplementCycleDefinitionDTO SaveSupplementsCycleDefinition(SupplementCycleDefinitionDTO definition)
        {
            Log.WriteWarning("SaveSupplementsCycleDefinition:Username={0},GlobalId={1}", SecurityInfo.SessionData.Profile.UserName,definition.GlobalId);

            if (!SecurityInfo.Licence.IsPremium)
            {
                throw new LicenceException("This feature is allowed for Premium account");
            }

            var dbDef = definition.Map<SupplementCycleDefinition>();

            using (var transactionScope = Session.BeginSaveTransaction())
            {
                Profile dbProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
                if (dbDef.IsNew)
                {
                    dbDef.CreationDate = Configuration.TimerService.UtcNow;
                    
                }
                else
                {
                    var dbGroup = Session.Get<SupplementCycleDefinition>(dbDef.GlobalId);
                    if (dbGroup != null)
                    {
                        if (dbProfile != dbGroup.Profile)
                        {
                            throw new CrossProfileOperationException("Cannot modify Cycle definition for another user");
                        }
                        if(dbGroup.Status==PublishStatus.Published)
                        {
                            throw new PublishedObjectOperationException("Cannot change published definition");
                        }
                    }
                }
                dbDef.Profile = dbProfile;

                dbDef.CanBeIllegal = calculateCanBeIllegal(dbDef);

                dbDef=Session.Merge(dbDef);
                dbProfile.DataInfo.SupplementsCycleDefinitionHash = Guid.NewGuid();
                transactionScope.Commit();
                return dbDef.Map<SupplementCycleDefinitionDTO>();
            }
        }

        bool calculateCanBeIllegal(SupplementCycleDefinition definition)
        {//todo:this sends many selects for each supplements. maybe egar loading all supplements needed?
            foreach (var week in definition.Weeks)
            {
                foreach (var dosage in week.Dosages.OfType<SupplementCycleDosage>())
                {
                    if(dosage.Supplement.CanBeIllegal)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public SupplementCycleDefinitionDTO SupplementsCycleDefinitionOperation(SupplementsCycleDefinitionOperationParam param)
        {
            Log.WriteWarning("SupplementsCycleDefinitionOperation:Username={0},Operation={1},GlobalId={2}", SecurityInfo.SessionData.Profile.UserName, param.Operation,param.SupplementsCycleDefinitionId);
            SupplementCycleDefinitionDTO result = null;

            if (!SecurityInfo.Licence.IsPremium)
            {
                throw new LicenceException("This feature is allowed for Premium account");
            }

            using (var transactionScope = Session.BeginSaveTransaction())
            {
                Profile dbProfile = Session.Get<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
                var db = Session.Get<SupplementCycleDefinition>(param.SupplementsCycleDefinitionId);
                if (param.Operation==Model.SupplementsCycleDefinitionOperation.Delete)
                {
                    if(db.Profile!=dbProfile)
                    {
                        throw new CrossProfileOperationException("This definition doesn't belong to this user");
                    }
                    if(db.Status==PublishStatus.Published)
                    {
                        throw new PublishedObjectOperationException("Cannot delete published definition");
                    }
                    Session.Delete(db);
                }
                else if(param.Operation==Model.SupplementsCycleDefinitionOperation.AddToFavorites)
                {
                    if (db.Status == PublishStatus.Private || db.Profile==dbProfile)
                    {
                        throw new InvalidOperationException("Cannot add private cycle definition to favorites list");
                    }
                    if (dbProfile.FavoriteSupplementCycleDefinitions.Contains(db))
                    {
                        throw new ObjectIsFavoriteException("Cycle definition is in favorites list already");
                    }
                    dbProfile.FavoriteSupplementCycleDefinitions.Add(db);
                }
                else if (param.Operation == Model.SupplementsCycleDefinitionOperation.RemoveFromFavorites)
                {
                    if (dbProfile.FavoriteSupplementCycleDefinitions.Contains(db))
                    {
                        dbProfile.FavoriteSupplementCycleDefinitions.Remove(db);
                    }
                    else
                    {
                        throw new ObjectIsNotFavoriteException("Cycle definition is not in favorites list");
                    }
                }
                else
                {
                    db=publishCycleDefinition(db,dbProfile);
                    result=db.Map<SupplementCycleDefinitionDTO>();
                }
                dbProfile.DataInfo.SupplementsCycleDefinitionHash = Guid.NewGuid();
                transactionScope.Commit();
                return result;
            }
        }

        private SupplementCycleDefinition publishCycleDefinition(SupplementCycleDefinition db, Profile dbProfile)
        {
            if (dbProfile != db.Profile)
            {
                throw new CrossProfileOperationException("Cannot publish cycle definition for another user");
            }

            if (db.Profile.Statistics.SupplementEntriesCount < Constants.StrengthTrainingEntriesCount)
            {
                throw new ProfileRankException("You must have at least " + Constants.StrengthTrainingEntriesCount + " supplements entries to publish supplements cycle definition");
            }

            //we cannot modify published definition
            if (db.Status == PublishStatus.Published)
            {
                throw new PublishedObjectOperationException("Cannot change published cycle definition");
            }
            db.PublishDate = Configuration.TimerService.UtcNow;
            db.Status = PublishStatus.Published;
            Session.Update(db);
            ProfileStatisticsUpdater.UpdateSupplementsDefinitions(Session, dbProfile);
            return db;
            //now update cache modification date
            //TODO:Maybe finish?
            //profileDb.DataInfo.LastPlanModification = Configuration.TimerService.UtcNow;
            //session.Update(profileDb);
            
        }
    }
}
