using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Reports;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using ExerciseDoneWay = BodyArchitect.Service.V2.Model.ExerciseDoneWay;
using ReportStatus = BodyArchitect.Model.ReportStatus;
using SetType = BodyArchitect.Model.SetType;
using BodyArchitect.Service.V2;
using EntryObjectStatus = BodyArchitect.Model.EntryObjectStatus;

namespace BodyArchitect.Service.V2.Services
{
    class ReportService: ServiceBase
    {
        public ReportService(ISession session, SecurityInfo securityInfo, ServiceConfiguration configuration)
            : base(session, securityInfo, configuration)
        {
        }

        public IList<WeightExerciseReportResultItem> ReportExerciseWeight(ReportExerciseWeightParams param)
        {
            if (!SecurityInfo.Licence.IsPremium)
            {
                throw new LicenceException("Reports are available in Premium or Instructor account only");
            }
            StrengthTrainingEntry entry = null;
            TrainingDay day = null;
            Serie set = null;
            StrengthTrainingItem item = null;

            var query = Session.QueryOver<StrengthTrainingItem>(() => item).
                JoinAlias(x => item.StrengthTrainingEntry, () => entry).
                JoinAlias(x => entry.TrainingDay, () => day).
                JoinAlias(x => item.Series, () => set)
                .Where(x => entry.Status==EntryObjectStatus.Done);//reports are generated only for done entries

            //optimalization: for not retrieving exercises one by one
            var exercisesQuery=Session.QueryOver<Exercise>();

            var dbMyProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
            var dbProfile = dbMyProfile;
            if (param.UserId.HasValue)
            {
                dbProfile = Session.Get<Profile>(param.UserId.Value);
            }

            if (param.CustomerId != null && dbProfile.GlobalId != SecurityInfo.SessionData.Profile.GlobalId)
            {
                throw new CrossProfileOperationException("This customer doesn't belong to your profile");
            }
            if (ServiceHelper.IsPrivateCalendar(dbProfile,dbMyProfile))
            {
                return new List<WeightExerciseReportResultItem>();
            }

            query = query.Where(x => day.Profile == dbProfile);
            if (param.CustomerId.HasValue)
            {
                Customer customer = Session.Get<Customer>(param.CustomerId.Value);
                if(customer.Profile!=dbProfile)
                {
                    throw new CrossProfileOperationException("This customer doesn't belong to your profile");
                }
                query = query.Where(x => day.Customer == customer);
            }
            else
            {
                query = query.Where(x => day.Customer == null);
            }

            var purposeOr = Restrictions.Disjunction();
            var exerciseOr = Restrictions.Disjunction();
            foreach (var exerciseId in param.Exercises)
            {
                purposeOr.Add<StrengthTrainingItem>(x => item.Exercise.GlobalId == exerciseId);
                exerciseOr.Add<Exercise>(x => x.GlobalId == exerciseId);
            }
            query = query.And(purposeOr);
            exercisesQuery = exercisesQuery.And(exerciseOr);

            if(!param.UseAllEntries)
            {
                query = query.Where(x => entry.ReportStatus == ReportStatus.ShowInReport);
            }
            if (param.StartDate.HasValue)
            {
                query = query.Where(x => day.TrainingDate >= param.StartDate);
            }
            if (param.EndDate.HasValue)
            {
                query = query.Where(x => day.TrainingDate <= param.EndDate);
            }

            if (param.RepetitionsFrom.HasValue && param.RepetitionsFrom.Value>0)
            {
                query = query.Where(x => set.RepetitionNumber >= param.RepetitionsFrom.Value);
            }
            if (param.RepetitionsTo.HasValue && param.RepetitionsTo.Value > 0)
            {
                query = query.Where(x => set.RepetitionNumber <= param.RepetitionsTo.Value);
            }
            if(param.SetTypes.Count>0)
            {
                var setTypeOf = Restrictions.Disjunction();
                foreach (var setType in param.SetTypes)
                {
                    setTypeOf.Add<StrengthTrainingItem>(x => set.SetType == (SetType) setType);
                }
                query = query.And(setTypeOf);
            }

            if (param.MyPlaces.Count > 0)
            {
                var myPlacesOr = Restrictions.Disjunction();
                foreach (var myPlaceId in param.MyPlaces)
                {
                    myPlacesOr.Add<StrengthTrainingItem>(x => entry.MyPlace.GlobalId==myPlaceId);
                }
                query = query.And(myPlacesOr);
            }

            if (param.DoneWays.Count > 0)
            {
                var doneWaysOr = Restrictions.Disjunction();
                foreach (ExerciseDoneWay way in param.DoneWays)
                {
                    doneWaysOr.Add<StrengthTrainingItem>(x => x.DoneWay == (BodyArchitect.Model.ExerciseDoneWay)way);
                }
                query = query.And(doneWaysOr);
            }

            if(param.RestPause.HasValue)
            {
                query = query.Where(x => set.IsRestPause == param.RestPause.Value);
            }
            if (param.SuperSlow.HasValue)
            {
                query = query.Where(x => set.IsSuperSlow == param.SuperSlow);
            }

            exercisesQuery.Future();
            //query = query.Where(x=>set.Weight>0).OrderBy(x => day.TrainingDate).Asc.Select(x => new Tuple<double,DateTime,Guid>(set.Weight.Value, day.TrainingDate, x.Exercise.GlobalId ));
            query = query.Where(x => set.Weight > 0).OrderBy(x => day.TrainingDate).Asc
                .SelectList(l => l
                      .SelectMax(x => set.Weight.Value)
                      .SelectGroup(x =>  day.TrainingDate)
                      .SelectGroup(x => item.Exercise)
                      .Select(x => item.GlobalId));
            var future = query.Future<object[]>();
                //Select(x => set.Weight.Value,x=> day.TrainingDate,x=> x.Exercise.GlobalId);
            //var result = query.List<object[]>().Select(x => new Tuple<DateTime, float, Guid>((DateTime)x[1], (float)x[0], (Guid)x[2])).ToList();
            var result = future.Select(x => new WeightExerciseReportResultItem() { DateTime = (DateTime)x[1], Weight = (decimal)x[0], Exercise = (x[2]).Map<ExerciseLightDTO>(), StrengthTrainingItemId = (Guid)x[3] }).ToList();

            return result;
        }

        public IList<WeightReperitionReportResultItem> ReportWeightRepetitions(ReportWeightRepetitionsParams param)
        {
            if (!SecurityInfo.Licence.IsPremium)
            {
                throw new LicenceException("Reports are available in Premium or Instructor account only");
            }

            StrengthTrainingEntry entry = null;
            TrainingDay day = null;
            Serie set = null;
            StrengthTrainingItem item = null;
            //var query = Session.QueryOver<TrainingDay>(() => day).
            //    JoinAlias(x => day.Objects, () => entry).
            //    JoinAlias(x => entry.Entries, () => item).
            //    JoinAlias(x => item.Series, () => set);
            var query = Session.QueryOver<StrengthTrainingItem>(() => item).
                JoinAlias(x => item.StrengthTrainingEntry, () => entry).
                JoinAlias(x => entry.TrainingDay, () => day).
                JoinAlias(x => item.Series, () => set)
                .Where(x => entry.Status == EntryObjectStatus.Done);//reports are generated only for done entries

            //optimalization: for not retrieving exercises one by one
            var exercisesQuery = Session.QueryOver<Exercise>();

            var dbMyProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
            var dbProfile = dbMyProfile;
            if (param.UserId.HasValue)
            {
                dbProfile = Session.Get<Profile>(param.UserId.Value);
            }

            if (param.CustomerId != null && dbProfile.GlobalId != SecurityInfo.SessionData.Profile.GlobalId)
            {
                throw new CrossProfileOperationException("This customer doesn't belong to your profile");
            }
            if (ServiceHelper.IsPrivateCalendar(dbProfile, dbMyProfile))
            {
                return new List<WeightReperitionReportResultItem>();
            }

            query = query.Where(x => day.Profile == dbProfile);
            if (param.CustomerId.HasValue)
            {
                Customer customer = Session.Get<Customer>(param.CustomerId.Value);
                if (customer.Profile != dbProfile)
                {
                    throw new CrossProfileOperationException("This customer doesn't belong to your profile");
                }
                query = query.Where(x => day.Customer == customer);
            }
            else
            {
                query = query.Where(x => day.Customer == null);
            }

            var purposeOr = Restrictions.Disjunction();
            var exerciseOr = Restrictions.Disjunction();
            foreach (var exerciseId in param.Exercises)
            {
                purposeOr.Add<StrengthTrainingItem>(x => item.Exercise.GlobalId == exerciseId);
                exerciseOr.Add<Exercise>(x => x.GlobalId == exerciseId);
            }
            query = query.And(purposeOr);
            exercisesQuery = exercisesQuery.And(exerciseOr);

            if (!param.UseAllEntries)
            {
                query = query.Where(x => entry.ReportStatus == ReportStatus.ShowInReport);
            }
            if (param.StartDate.HasValue)
            {
                query = query.Where(x => day.TrainingDate >= param.StartDate);
            }
            if (param.EndDate.HasValue)
            {
                query = query.Where(x => day.TrainingDate <= param.EndDate);
            }

            if (param.MyPlaces.Count > 0)
            {

                var myPlacesOr = Restrictions.Disjunction();
                foreach (var myPlaceId in param.MyPlaces)
                {
                    myPlacesOr.Add<StrengthTrainingItem>(x => entry.MyPlace.GlobalId == myPlaceId);
                }
                query = query.And(myPlacesOr);
            }

            if (param.SetTypes.Count > 0)
            {
                var setTypeOf = Restrictions.Disjunction();
                foreach (var setType in param.SetTypes)
                {
                    setTypeOf.Add<StrengthTrainingItem>(x => set.SetType == (SetType)setType);
                }
                query = query.And(setTypeOf);
            }
            if(param.RestPause.HasValue)
            {
                query = query.Where(x => set.IsRestPause==param.RestPause);
            }
            if (param.SuperSlow.HasValue)
            {
                query = query.Where(x => set.IsSuperSlow == param.SuperSlow);
            }

            if (param.DoneWays.Count > 0)
            {
                var doneWaysOr = Restrictions.Disjunction();
                foreach (ExerciseDoneWay way in param.DoneWays)
                {
                    doneWaysOr.Add<StrengthTrainingItem>(x => x.DoneWay == (BodyArchitect.Model.ExerciseDoneWay)way);
                }
                query = query.And(doneWaysOr);
            }

            query = query.Where(x =>set.RepetitionNumber!=null && set.Weight!=null && set.Weight > 0).OrderBy(x => set.RepetitionNumber.Value).Asc
                .SelectList(l => l
                      .SelectMax(x => set.Weight.Value)
                      .SelectGroup(x => set.RepetitionNumber.Value)
                      .SelectGroup(x => item.Exercise)
                      .Select(x=>item.GlobalId)
                      //.SelectGroup(x => item.GlobalId)
                      );
            exercisesQuery.Future();
            var future = query.Future<object[]>();
            var result = future.Select(x => new WeightReperitionReportResultItem() { Repetitions = (decimal)x[1], Weight = (decimal)x[0], Exercise = x[2].Map<ExerciseLightDTO>(), StrengthTrainingItemId = (Guid)x[3] }).ToList();
            return result;
        }

        public IList<MeasurementsTimeReportResultItem> ReportMeasurementsTime(ReportMeasurementsTimeParams param)
        {
            if (!SecurityInfo.Licence.IsPremium)
            {
                throw new LicenceException("Reports are available in Premium or Instructor account only");
            }
            SizeEntry entry = null;
            TrainingDay day = null;
            Wymiary wymiary = null;
            //var query = Session.QueryOver<TrainingDay>(() => day).
            //    JoinAlias(x => day.Objects, () => entry).
            //    JoinAlias(x => entry.Entries, () => item).
            //    JoinAlias(x => item.Series, () => set);
            var query = Session.QueryOver<SizeEntry>(() => entry).
                JoinAlias(x => entry.TrainingDay, () => day).
                JoinAlias(x => entry.Wymiary, () => wymiary)
                .Where(x => entry.Status == EntryObjectStatus.Done);//reports are generated only for done entries;

            var dbMyProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
            var dbProfile = dbMyProfile;
            if (param.UserId.HasValue)
            {
                dbProfile = Session.Get<Profile>(param.UserId.Value);
            }

            if (param.CustomerId != null && dbProfile.GlobalId != SecurityInfo.SessionData.Profile.GlobalId)
            {
                throw new CrossProfileOperationException("This customer doesn't belong to your profile");
            }
            if (ServiceHelper.IsPrivateCalendar(dbProfile, dbMyProfile))
            {
                return new List<MeasurementsTimeReportResultItem>();
            }

            query = query.Where(x => day.Profile == dbProfile);
            if (param.CustomerId.HasValue)
            {
                Customer customer = Session.Get<Customer>(param.CustomerId.Value);
                if (customer.Profile != dbProfile)
                {
                    throw new CrossProfileOperationException("This customer doesn't belong to your profile");
                }
                query = query.Where(x => day.Customer == customer);
            }
            else
            {
                query = query.Where(x => day.Customer == null);
            }


            if (!param.UseAllEntries)
            {
                query = query.Where(x => entry.ReportStatus == ReportStatus.ShowInReport);
            }
            if (param.StartDate.HasValue)
            {
                query = query.Where(x => day.TrainingDate >= param.StartDate);
            }
            if (param.EndDate.HasValue)
            {
                query = query.Where(x => day.TrainingDate <= param.EndDate);
            }


            //var purposeOr = Restrictions.Disjunction();
            //foreach (var item in param.Items)
            //{
            //    if (item == MeasurementItem.Weight)
            //    {
            //        purposeOr.Add<SizeEntry>(x => item.Exercise.GlobalId == exerciseId);
            //    }
            //}
            //query = query.And(purposeOr);
            
            //query = query.Where(x=>set.Weight>0).OrderBy(x => day.TrainingDate).Asc.Select(x => new Tuple<double,DateTime,Guid>(set.Weight.Value, day.TrainingDate, x.Exercise.GlobalId ));
            //query = query.OrderBy(x => day.TrainingDate).Asc
            //    .SelectList(l =>
            //                    {
            //                        foreach (var item in param.Items)
            //                        {
            //                            l = l.Select(Projections.Max(item.ToString()));
            //                        }
            //                        return l.SelectGroup(x => day.TrainingDate);
            //                    });
            //query = query.OrderBy(x => day.TrainingDate).Asc
            //    .SelectList(l =>
            //                    {
            //                        l = l.SelectMax(x => x.Wymiary);
            //        return l.SelectGroup(x => day.TrainingDate);
            //    });
            query = query.OrderBy(x => day.TrainingDate).Asc
                .SelectList(l => 
                    l.SelectMax(x => wymiary.Height)
                    .SelectMax(x => wymiary.Klatka)
                    .SelectMax(x => wymiary.RightBiceps)
                    .SelectMax(x => wymiary.RightForearm)
                    .SelectMax(x => wymiary.RightUdo)
                    .SelectMax(x => wymiary.LeftBiceps)
                    .SelectMax(x => wymiary.LeftForearm)
                    .SelectMax(x => wymiary.LeftUdo)
                    .SelectMax(x => wymiary.Pas)
                    .SelectMax(x => wymiary.Weight)
                    .SelectGroup(x => day.TrainingDate)
                    .Select(x=>x.GlobalId));
            //Select(x => set.Weight.Value,x=> day.TrainingDate,x=> x.Exercise.GlobalId);
            var result1 = query.List<object[]>();
            //MeasurementsTimeReportResultItem
            //var result=result1.Select(x => new Tuple<DateTime, WymiaryDTO>((DateTime) x[10], new WymiaryDTO()
            //                                                                  {
            //                                                                      Height = (float) x[0],
            //                                                                      Klatka = (float) x[1],
            //                                                                      RightBiceps = (float) x[2],
            //                                                                      RightForearm = (float) x[3],
            //                                                                      RightUdo = (float) x[4],
            //                                                                      LeftBiceps = (float) x[5],
            //                                                                      LeftForearm = (float) x[6],
            //                                                                      LeftUdo = (float) x[7],
            //                                                                      Pas = (float) x[8],
            //                                                                      Weight = (float) x[9],
            //                                                                  })).ToList();
            var result = result1.Select(x => new MeasurementsTimeReportResultItem(){DateTime=(DateTime)x[10], 
                Wymiary=new WymiaryDTO()
            {
                Height = (decimal)x[0],
                Klatka = (decimal)x[1],
                RightBiceps = (decimal)x[2],
                RightForearm = (decimal)x[3],
                RightUdo = (decimal)x[4],
                LeftBiceps = (decimal)x[5],
                LeftForearm = (decimal)x[6],
                LeftUdo = (decimal)x[7],
                Pas = (decimal)x[8],
                Weight = (decimal)x[9],
            },SizeEntryId=(Guid)x[11]}).ToList();
            return result;
        }

        public PagedResult<ExerciseRecordsReportResultItem> ReportExerciseRecords(ExerciseRecordsParams param, PartialRetrievingInfo pageInfo)
        {
            Profile profile = null;
            var query = Session.QueryOver<ExerciseProfileData>().JoinAlias(x=>x.Profile,()=>profile).Fetch(x=>x.Exercise).Eager;

            var dbMyProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
           

            //query = query.Where(x => day.Profile == dbProfile);
            //if (param.CustomerId.HasValue)
            //{
            //    Customer customer = Session.Get<Customer>(param.CustomerId.Value);
            //    if (customer.Profile != dbProfile)
            //    {
            //        throw new CrossProfileOperationException("This customer doesn't belong to your profile");
            //    }
            //    query = query.Where(x => day.Customer == customer);
            //}
            //else
            if (param.Mode==RecordMode.Customer)
            {
                
                query = query.Where(x => x.Customer != null && x.Profile == dbMyProfile);
            }
            else if(param.Mode==RecordMode.Friends)
            {
                var friendsIds = dbMyProfile.Friends.Select(x => x.GlobalId).ToList();
                var bigOr = Restrictions.Disjunction();
                bigOr.Add(Restrictions.On<Profile>(x => profile.GlobalId).IsIn((ICollection)friendsIds));
                bigOr.Add(Restrictions.Where<Profile>(x => profile.GlobalId==dbMyProfile.GlobalId));
                query = query.Where(x => x.Customer == null).And(bigOr);
            }
            else
            {
                query = PublicRecordsQueryCriteria(query, profile);
            }
            query = query.Where(x => x.Exercise.GlobalId == param.ExerciseId);

            
            query=query.OrderBy(z=>z.MaxWeight).Desc.ThenBy(x=>x.Repetitions).Desc;

            var listPack = query.ToPagedResults<ExerciseRecordsReportResultItem, ExerciseProfileData>(pageInfo);
            return listPack;
            //IEnumerable<object[]> queryEnumerable = null;
            //int count = 0;
            //if (pageInfo.PageSize > PartialRetrievingInfo.AllElementsPageSize)
            //{
            //    IFutureValue<int> rowCountQuery = Session.QueryOver<Profile>(() => test1).WithSubquery.WhereExists(
            //    rowCount.Where(x => user.GlobalId == test1.GlobalId)).ToRowCountQuery().FutureValue<int>();
            //    //IQueryOver<object[]> pagedResults = query.ApplyPaging(pageInfo);
            //    if (pageInfo.PageSize > PartialRetrievingInfo.AllElementsPageSize)
            //    {
            //        query = (IQueryOver<StrengthTrainingItem, StrengthTrainingItem>) query.Take(pageInfo.PageSize).Skip(pageInfo.PageIndex * pageInfo.PageSize);
            //    }

            //    queryEnumerable = query.Future<object[]>();
            //    count = rowCountQuery.Value;
            //}
            //else
            //{
            //    queryEnumerable = query.Future<object[]>();
            //    count = queryEnumerable.Count();
            //}
            //var test = queryEnumerable.ToList();

            //Session.QueryOver<Profile>().WhereRestrictionOn(a => a.GlobalId).IsIn(test.Select(x => ((Profile) x[1]).GlobalId).ToArray()).List();

            //var result = queryEnumerable.Select(x => 
            //    new ExerciseRecordsReportResultItem()
            //        {
            //            Weight = (decimal)x[0], 
            //            StrengthTrainingItemId = (Guid)x[3],
            //            User = (x[1]).Map<UserDTO>(),
            //            CustomerId = x[2]!=null?((Customer)x[2]).GlobalId:(Guid?) null,
            //            DateTime = (DateTime)x[4],
            //            Repetitions = (int)(decimal)x[5]
            //        }).ToList();

            //return new PagedResult<ExerciseRecordsReportResultItem>(result,count, pageInfo.PageIndex);
        }

        internal static IQueryOver<ExerciseProfileData, ExerciseProfileData> PublicRecordsQueryCriteria(IQueryOver<ExerciseProfileData, ExerciseProfileData> query, Profile profile)
        {
            ProfileStatistics stat = null;
            query = query.JoinAlias(x => profile.Statistics, () => stat);
            query = query.Where(x => (stat.StrengthTrainingEntriesCount >= Portable.Constants.StrengthTrainingEntriesCount || stat.TrainingDaysCount >= Portable.Constants.PublicTrainingDaysCount) && x.Customer == null);
            return query;
        }
    }
}
