using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Portable.Exceptions;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using Profile = BodyArchitect.Model.Profile;
using PublishStatus = BodyArchitect.Model.PublishStatus;
using TrainingPlan = BodyArchitect.Service.V2.Model.TrainingPlans.TrainingPlan;
using TrainingPlanDifficult = BodyArchitect.Service.V2.Model.TrainingPlans.TrainingPlanDifficult;
using TrainingType = BodyArchitect.Service.V2.Model.TrainingPlans.TrainingType;
using WorkoutPlanPurpose = BodyArchitect.Model.WorkoutPlanPurpose;

namespace BodyArchitect.Service.V2.Services
{
    public class WorkoutPlanService : ServiceBase
    {
        public WorkoutPlanService(ISession session, SecurityInfo securityInfo, ServiceConfiguration configuration) : base(session, securityInfo, configuration)
        {
        }

        //public TrainingPlan GetWorkoutPlan(Guid planId, RetrievingInfo retrievingInfo)
        //{
        //    Log.WriteWarning("GetWorkoutPlan:Username={0},planId={1}", SecurityInfo.SessionData.Profile.UserName, planId);

        //    var session = Session;

        //    var result = session.QueryOver<BodyArchitect.Model.TrainingPlan>()
        //        .Fetch(t => t.Days).Eager
        //        .Fetch(t => t.Days.First().Entries).Eager
        //        .Fetch(t => t.Days.First().Entries.First().Exercise).Eager
        //        .Fetch(t => t.Days.First().Entries.First().Sets).Eager
        //        .Where(t => t.GlobalId == planId && (t.Profile.GlobalId == SecurityInfo.SessionData.Profile.GlobalId || t.Status == PublishStatus.Published));
        //    var dbPlan = result.SingleOrDefault();

        //    var ratingDict = (from rv in session.Query<RatingUserValue>()
        //                      from tp in session.Query<BodyArchitect.Model.TrainingPlan>()
        //                      where
        //                          tp.GlobalId == rv.RatedObjectId &&
        //                          rv.ProfileId == SecurityInfo.SessionData.Profile.GlobalId && tp.GlobalId == planId
        //                      select rv).SingleOrDefault();


        //    var workoutPlan = Mapper.Map<BodyArchitect.Model.TrainingPlan, TrainingPlan>(dbPlan);
        //    if (ratingDict != null)
        //    {
        //        workoutPlan.UserShortComment = ratingDict.ShortComment;
        //        workoutPlan.UserRating = ratingDict.Rating;
        //    }
        //    return workoutPlan;
        //}

        public PagedResult<TrainingPlan> GetWorkoutPlans(WorkoutPlanSearchCriteria searchCriteria, PartialRetrievingInfo pagerInfo)
        {
            Log.WriteWarning("GetWorkoutPlans: Username={0}", SecurityInfo.SessionData.Profile.UserName);

            var session = Session;
            Dictionary<Guid, RatingUserValue> ratingDict = new Dictionary<Guid, RatingUserValue>();

            using (var tx = session.BeginGetTransaction())
            {
                Profile _profile = null;
                var myProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
                var loggedProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
                if (searchCriteria.UserId.HasValue)
                {
                    myProfile = Session.Load<Profile>(searchCriteria.UserId.Value);
                }
                var ids = myProfile.FavoriteWorkoutPlans.Select(x => x.GlobalId).ToList();
                BodyArchitect.Model.TrainingPlan _plan=null;
                var fetchQuery = session.QueryOver<BodyArchitect.Model.TrainingPlan>(() => _plan)
                    .Fetch(t => t.Days).Eager
                .Fetch(t => t.Days.First().Entries).Eager
                .Fetch(t => t.Days.First().Entries.First().Exercise).Eager
                .Fetch(t => t.Days.First().Entries.First().Sets).Eager;

                var planQuery = session.QueryOver<BodyArchitect.Model.TrainingPlan>(() => _plan);
                //if we want to get plan content then we need to inform nhibernate about this
                //if (pagerInfo.LongTexts)
                //{
                //    planQuery = planQuery.Fetch(x => x.PlanContent).Eager;
                //}

                var queryExercises = planQuery.JoinAlias(x => x.Profile, () => _profile);

                if (searchCriteria.Days.Count > 0)
                {
                    //var daysOr = Restrictions.Disjunction();
                    //foreach (var day in searchCriteria.Days)
                    //{
                    //    var orderIdsCriteria = DetachedCriteria.For<BodyArchitect.Model.TrainingPlanDay>();
                    //    orderIdsCriteria.SetProjection(Projections.CountDistinct("GlobalId"))
                    //    .Add(Restrictions.Where<BodyArchitect.Model.TrainingPlanDay>(x => x.TrainingPlan.GlobalId ==_plan.GlobalId));

                    //    daysOr.Add(Subqueries.Eq(day, orderIdsCriteria));
                    //}
                    //queryExercises = queryExercises.And(daysOr);
                    var orderIdsCriteria = DetachedCriteria.For<BodyArchitect.Model.TrainingPlanDay>();
                        orderIdsCriteria.SetProjection(Projections.CountDistinct("GlobalId"))
                        .Add(Restrictions.Where<BodyArchitect.Model.TrainingPlanDay>(x => x.TrainingPlan.GlobalId == _plan.GlobalId));
                    queryExercises =queryExercises.Where(Restrictions.In(Projections.SubQuery(orderIdsCriteria),(ICollection) searchCriteria.Days));

                }

                if(searchCriteria.PlanId.HasValue)
                {
                    queryExercises = queryExercises.Where(x=>x.GlobalId==searchCriteria.PlanId.Value);
                }

                if (searchCriteria.Purposes.Count > 0)
                {
                    var purposeOr = Restrictions.Conjunction();
                    foreach (var purpose in searchCriteria.Purposes)
                    {
                        purposeOr.Add<BodyArchitect.Model.TrainingPlan>(x => x.Purpose == (WorkoutPlanPurpose)purpose);
                    }
                    queryExercises = queryExercises.And(purposeOr);
                }

                if (searchCriteria.Languages.Count > 0)
                {
                    var langOr = Restrictions.Disjunction();
                    foreach (var lang in searchCriteria.Languages)
                    {
                        langOr.Add<BodyArchitect.Model.TrainingPlan>(x => x.Language == lang);
                    }
                    queryExercises = queryExercises.And(langOr);

                }

                queryExercises = queryExercises.Where(x => x.Profile == loggedProfile || (x.Profile != loggedProfile && x.Status == PublishStatus.Published));


                if (searchCriteria.WorkoutPlanType.Count > 0)
                {
                    var mainOr = Restrictions.Disjunction();
                    foreach (TrainingType trainingType in searchCriteria.WorkoutPlanType)
                    {
                        var tt = (BodyArchitect.Model.TrainingType)trainingType;
                        mainOr.Add<BodyArchitect.Model.TrainingPlan>(x => x.TrainingType == tt);
                    }
                    queryExercises=queryExercises.And(mainOr);
                }

                if (searchCriteria.Difficults.Count > 0)
                {
                    var mainOr = Restrictions.Disjunction();
                    foreach (TrainingPlanDifficult diff in searchCriteria.Difficults)
                    {
                        var tt = (BodyArchitect.Model.TrainingPlanDifficult)diff;
                        mainOr.Add<BodyArchitect.Model.TrainingPlan>(x => x.Difficult == tt);
                    }
                    queryExercises=queryExercises.And(mainOr);
                }

                var groupOr = new Disjunction();
                if (searchCriteria.SearchGroups.Count > 0)
                {

                    if (searchCriteria.SearchGroups.IndexOf(WorkoutPlanSearchCriteriaGroup.Mine) > -1)
                    {
                        groupOr.Add<BodyArchitect.Model.TrainingPlan>(x => x.Profile == myProfile);
                    }
                    if (searchCriteria.SearchGroups.IndexOf(WorkoutPlanSearchCriteriaGroup.Favorites) > -1)
                    {
                        if (myProfile.FavoriteWorkoutPlans.Count > 0)
                        {
                            groupOr.Add<BodyArchitect.Model.TrainingPlan>(x => x.GlobalId.IsIn((ICollection)ids));
                        }
                    }
                    if (searchCriteria.SearchGroups.IndexOf(WorkoutPlanSearchCriteriaGroup.Other) > -1)
                    {
                        var tmpAnd = Restrictions.Conjunction();
                        tmpAnd.Add<BodyArchitect.Model.TrainingPlan>(dto => dto.Profile != null && dto.Profile != myProfile && dto.Status == PublishStatus.Published);

                        if (ids.Count > 0)
                        {
                            tmpAnd.Add(Restrictions.On<BodyArchitect.Model.TrainingPlan>(x => x.GlobalId).Not.IsIn((ICollection)ids));
                        }

                        groupOr.Add(tmpAnd);
                    }
                    queryExercises = queryExercises.Where(groupOr);
                }
                

                queryExercises = queryExercises.ApplySorting(searchCriteria.SortOrder, searchCriteria.SortAscending);
                fetchQuery = fetchQuery.ApplySorting(searchCriteria.SortOrder, searchCriteria.SortAscending);
                //var rowCountQuery = queryExercises.ToRowCountQuery();
                ratingDict = (from rv in session.Query<RatingUserValue>()
                              from tp in session.Query<BodyArchitect.Model.TrainingPlan>()
                              where
                                  tp.GlobalId == rv.RatedObjectId &&
                                  rv.ProfileId == SecurityInfo.SessionData.Profile.GlobalId
                              select rv).ToDictionary(t => t.RatedObjectId);

                //var list = queryExercises.Take(pagerInfo.PageSize).Skip(pagerInfo.PageIndex * pagerInfo.PageSize).Future();

                //count = rowCountQuery.FutureValue<int>().Value;
                //dbList = list.ToList();
                var res = fetchQuery.ToExPagedResults<TrainingPlan, BodyArchitect.Model.TrainingPlan>(pagerInfo, queryExercises, delegate(IEnumerable<BodyArchitect.Model.TrainingPlan> list)
                {
                    var output = new List<TrainingPlan>();
                    foreach (var planDto in list)
                    {
                        var tmp = Mapper.Map<BodyArchitect.Model.TrainingPlan, TrainingPlan>(planDto);
                        //if(!pagerInfo.LongTexts)
                        //{
                        //    tmp.PlanContent = null;
                        //}
                        //set the user only when workout plan was not created by current user
                        if (planDto.Profile.GlobalId != SecurityInfo.SessionData.Profile.GlobalId)
                        {
                            tmp.Profile = Mapper.Map<Profile, UserDTO>(planDto.Profile);
                        }

                        if (ratingDict.ContainsKey(planDto.GlobalId))
                        {
                            tmp.UserRating = ratingDict[planDto.GlobalId].Rating;
                            tmp.UserShortComment = ratingDict[planDto.GlobalId].ShortComment;
                        }
                        output.Add(tmp);
                    }
                    return output.ToArray();
                });
                tx.Commit();
                return res;

            }
        }

        private void DeleteWorkoutPlan(Guid trainingPlanId)
        {
            Log.WriteWarning("DeleteWorkoutPlan: Username={0},planId={1}", SecurityInfo.SessionData.Profile.UserName, trainingPlanId);

            if (!SecurityInfo.Licence.IsPremium)
            {
                throw new LicenceException("This feature is allowed for Premium account");
            }
            var dbPlan = Session.Get<BodyArchitect.Model.TrainingPlan>(trainingPlanId);
            if (SecurityInfo.SessionData.Profile.GlobalId != dbPlan.Profile.GlobalId)
            {
                throw new CrossProfileOperationException("Cannot modify workout plan for another user");
            }
            if (dbPlan.Status == PublishStatus.Published)
            {
                throw new PublishedObjectOperationException("Cannot delete published workout plan");
            }
            Session.Delete("FROM RatingUserValue WHERE RatedObjectId=?", trainingPlanId, NHibernate.NHibernateUtil.Guid);
            Session.Delete(dbPlan);
        }

        public TrainingPlan SaveWorkoutPlan(TrainingPlan dto)
        {
             Log.WriteWarning("SaveWorkoutPlan: Username={0},planId={1}", SecurityInfo.SessionData.Profile.UserName, dto.GlobalId);

            if (!SecurityInfo.Licence.IsPremium)
            {
                throw new LicenceException("This feature is allowed for Premium account");
            }
            if(dto.Status!=Model.PublishStatus.Private)
            {
                throw new InvalidOperationException("Wrong status. Only Private can be saved");
            }
            var session = Session;
            var dbPlan = Mapper.Map<TrainingPlan, BodyArchitect.Model.TrainingPlan>(dto);

            using (var tx = session.BeginSaveTransaction())
            {
                var dbProfile = session.Get<Profile>(SecurityInfo.SessionData.Profile.GlobalId);

                dbPlan.Profile = dbProfile;
                //now we need to check if the user is publishing the workout plan
                //var planFromDb = (from p in session.Query<BodyArchitect.Model.TrainingPlan>()
                //                  where p.GlobalId == dbPlan.GlobalId
                //                  select p).SingleOrDefault();
                var planFromDb = session.Get<BodyArchitect.Model.TrainingPlan>(dbPlan.GlobalId);
                if (planFromDb!=null)
                {
                    if (SecurityInfo.SessionData.Profile.GlobalId != planFromDb.Profile.GlobalId)
                    {
                        throw new CrossProfileOperationException("Cannot modify training plan for another user");
                    }
                    //we cannot modify published workout plan)
                    if (planFromDb.Status == PublishStatus.Published)
                    {
                        throw new PublishedObjectOperationException("Cannot change published workout plan");
                    }
                    if (dto.Status == Model.PublishStatus.Published && planFromDb.Status != PublishStatus.Published)
                    {
                        throw new InvalidOperationException("Cannot publish workout using SaveWorkoutPlan method. Use PublishWorkoutPlan method instead.");
                    }
                    dbPlan.Rating = planFromDb.Rating;
                }
                else
                {
                    dbPlan.CreationDate = Configuration.TimerService.UtcNow;
                }
                
                //XmlSerializationTrainingPlanFormatter formatter = new XmlSerializationTrainingPlanFormatter();
                //TrainingPlan plan = formatter.FromXml(dto.PlanContent,null);
                var validator = new ObjectValidator(typeof(TrainingPlan));
                var result = validator.Validate(dto);
                if (!result.IsValid)
                {
                    throw new ValidationException(result.ToValidationResults());
                }

                dbPlan =session.Merge(dbPlan);

                //now update cache modification date
                dbProfile.DataInfo.WorkoutPlanHash = Guid.NewGuid();
                tx.Commit();
            }
            //return Mapper.Map<BodyArchitect.Model.TrainingPlan, WorkoutPlanDTO>(dbPlan);
            var param = new WorkoutPlanSearchCriteria();
            param.PlanId = dbPlan.GlobalId;
            var res= GetWorkoutPlans(param,new PartialRetrievingInfo());
            if (res.Items.Count > 0)
            {
                return res.Items[0];
            }
            return null;
        }

        public TrainingPlan WorkoutPlanOperation(WorkoutPlanOperationParam param)
        {
            Log.WriteWarning("WorkoutPlanFavoritesOperation: Username={0},userDto.GlobalId={1}", SecurityInfo.SessionData.Profile.UserName, param.Operation);

            if (!SecurityInfo.Licence.IsPremium)
            {
                throw new LicenceException("This feature is allowed for Premium account");
            }

            var session = Session;
            TrainingPlan result = null;
            using (var tx = session.BeginSaveTransaction())
            {
                var profileDb = session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);

                var workoutPlanDb = session.Get<BodyArchitect.Model.TrainingPlan>(param.WorkoutPlanId);

                
                if (param.Operation == SupplementsCycleDefinitionOperation.AddToFavorites)
                {
                    if (workoutPlanDb.Profile == profileDb || workoutPlanDb.Status == PublishStatus.Private)
                    {
                        throw new InvalidOperationException("Cannot add/remove your workout plan to the favorite list");
                    }
                    if (profileDb.FavoriteWorkoutPlans.Contains(workoutPlanDb))
                    {
                        throw new ObjectIsFavoriteException("Plan is in favorites list already");
                    }
                    profileDb.FavoriteWorkoutPlans.Add(workoutPlanDb);
                }
                else if (param.Operation == SupplementsCycleDefinitionOperation.RemoveFromFavorites)
                {
                    if (profileDb.FavoriteWorkoutPlans.Contains(workoutPlanDb))
                    {
                        profileDb.FavoriteWorkoutPlans.Remove(workoutPlanDb);
                    }
                    else
                    {
                        throw new ObjectIsNotFavoriteException("Plan is not in favorites list");
                    }
                }
                else if (param.Operation == SupplementsCycleDefinitionOperation.Publish)
                {
                    workoutPlanDb=publishWorkoutPlan(workoutPlanDb, profileDb);
                    result = workoutPlanDb.Map<TrainingPlan>();
                }
                else if (param.Operation == SupplementsCycleDefinitionOperation.Delete)
                {
                    DeleteWorkoutPlan(param.WorkoutPlanId);
                }
                profileDb.DataInfo.WorkoutPlanHash = Guid.NewGuid();
                session.SaveOrUpdate(profileDb);
                tx.Commit();
                return result;
            }

        }

        private BodyArchitect.Model.TrainingPlan publishWorkoutPlan(BodyArchitect.Model.TrainingPlan db, Profile dbProfile)
        {
            if (dbProfile != db.Profile)
            {
                throw new CrossProfileOperationException("Cannot publish cycle definition for another user");
            }

            //we cannot modify published definition
            if (db.Status == PublishStatus.Published)
            {
                throw new PublishedObjectOperationException("Cannot change published cycle definition");
            }
            if (db.Profile.Statistics.StrengthTrainingEntriesCount < Portable.Constants.StrengthTrainingEntriesCount)
            {
                throw new ProfileRankException("You must have at least " + Portable.Constants.StrengthTrainingEntriesCount + " strength training entries to publish workout plan");
            }

            db.PublishDate = Configuration.TimerService.UtcNow;
            db.Status = PublishStatus.Published;

            var missingExercises=db.Days.SelectMany(x => x.Entries).Count(x => x.Exercise.IsDeleted);
            if(missingExercises>0)
            {
                throw new ValidationException("Training plan cannot have missing exercises");
            }
            //workoutPlanExercisesOperation(Session, dbProfile, db, delegate(Exercise exercise)
            //{
            //    //if (exercise.Status != PublishStatus.Published)
            //    //{
            //    //    throw new PublishedObjectOperationException("Exercise: " + exercise.Name + " is not global so you cannot publish this workout plan");
            //    //}
            //});
            Session.Update(db);
            ProfileStatisticsUpdater.UpdateWorkoutPlans(Session, dbProfile);
            return db;
        }

        //void workoutPlanExercisesOperation(ISession session, Profile dbProfile, BodyArchitect.Model.TrainingPlan workoutPlanDb, Action<Exercise> operation)
        //{
        //    var list = new List<Exercise>();
        //    Dictionary<Guid, Guid> exercises = new Dictionary<Guid, Guid>();
        //    //XmlSerializationTrainingPlanFormatter formatter = new XmlSerializationTrainingPlanFormatter();
        //    //TrainingPlan newPlan = formatter.FromXml(workoutPlanDb.PlanContent, delegate(IEnumerable<Guid> exerciseId)
        //    //{
        //    //    var exercisesQuery = from exercise in session.Query<Exercise>() where exerciseId.Contains(exercise.GlobalId) select exercise;
        //    //    list = exercisesQuery.ToList();
        //    //    return exercisesQuery.Map<IList<ExerciseLightDTO>>().ToDictionary(x => x.GlobalId);
        //    //});
        //    foreach (var day in newPlan.Days)
        //    {
        //        foreach (var entry in day.Entries)
        //        {
        //            if (entry.Exercise == null)
        //            {
        //                throw new ArgumentException("One or more entry has no exercise set");
        //            }
        //            exercises[entry.Exercise.GlobalId] = entry.Exercise.GlobalId;
        //        }
        //    }

        //    var validator = new ObjectValidator(typeof(TrainingPlan));
        //    var result = validator.Validate(newPlan);
        //    if (!result.IsValid)
        //    {
        //        throw new BodyArchitect.Shared.ValidationException(result);
        //    }

        //    if (exercises.Count == 0)
        //    {
        //        throw new Exception("Workout plan must have exercises");
        //    }
        //    //var exercisesQuery = from exercise in session.Query<Exercise>() where list.Contains(exercise.GlobalId) select exercise;
        //    foreach (var exercise in list)
        //    {
        //        operation(exercise);

        //    }
        //}
        /*
        public WorkoutPlanDTO PublishWorkoutPlan(Token token, WorkoutPlanDTO planDto)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            Log.WriteWarning("PublishWorkoutPlan: Username={0},planId={1}", securityInfo.SessionData.Profile.UserName, planDto.GlobalId);

            var session = Session;

            using (var tx = session.BeginTransaction())
            {
                //now we need to check if the user is publishing the workout plan
                var planFromDb = (from p in session.Query<BodyArchitect.Model.TrainingPlan>()
                                  where p.GlobalId == planDto.GlobalId
                                  select p).SingleOrDefault();
                var profileDb = session.Get<Profile>(securityInfo.SessionData.Profile.Id);
                if (profileDb != planFromDb.Profile)
                {
                    throw new CrossProfileOperationException("Cannot publish workout plan for another user");
                }
                //we cannot modify published workout plan
                if (planFromDb.Status == PublishStatus.Published)
                {
                    throw new PublishedObjectOperationException("Cannot change published workout plan");
                }
                planFromDb.PublishDate = Configuration.TimerService.UtcNow;
                planFromDb.Status = PublishStatus.Published;

                //now we need to deserialize plan and check (publish) all private exercises
                //publishExercises(securityInfo,planFromDb);
                Log.WriteVerbose("Checking exercises...");
                workoutPlanExercisesOperation(session, profileDb, planFromDb, delegate(Exercise exercise)
                {
                    //if (exercise.Status != PublishStatus.Published)
                    //{
                    //    throw new PublishedObjectOperationException("Exercise: " + exercise.Name + " is not global so you cannot publish this workout plan");
                    //}
                });
                Log.WriteVerbose("Exercises ok");
                session.SaveOrUpdate(planFromDb);
                //now update cache modification date
                profileDb.DataInfo.LastPlanModification = Configuration.TimerService.UtcNow;
                session.Update(profileDb);
                ProfileStatisticsUpdater.UpdateWorkoutPlans(session, profileDb);
                tx.Commit();
                Log.WriteVerbose("Publishing ok");
                return Mapper.Map<BodyArchitect.Model.TrainingPlan, WorkoutPlanDTO>(planFromDb);
            }

        }*/
    }
}
