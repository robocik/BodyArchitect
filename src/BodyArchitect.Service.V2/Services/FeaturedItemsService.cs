using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Service.V2.Model.Reports;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Gender = BodyArchitect.Service.V2.Model.Gender;
using Privacy = BodyArchitect.Model.Privacy;
using PublishStatus = BodyArchitect.Model.PublishStatus;
using TrainingPlan = BodyArchitect.Model.TrainingPlan;

namespace BodyArchitect.Service.V2.Services
{
    class FeaturedItemsService: ServiceBase
    {
        public FeaturedItemsService(ISession session, SecurityInfo securityInfo, ServiceConfiguration configuration)
            : base(session, securityInfo, configuration)
        {
        }

        public FeaturedData GetFeaturedData(GetFeaturedDataParam param)
        {
            Log.WriteWarning("GetFeaturedData:Username={0}", SecurityInfo.SessionData.Profile.UserName);
            prepareParameter(param);
            using (var transactionScope = Session.BeginGetTransaction())
            {
                FeaturedData result = new FeaturedData();
                var dbProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
                prepareFeaturedTrainingPlans(dbProfile, result, param);

                prepareFeaturedSupplementsCycleDefinitions(result, dbProfile, param);

                prepareFeaturedEntries(dbProfile, result, param);

                prepareExercisesRecords(dbProfile, result, param);
                transactionScope.Commit();
                return result;
            }
        }

        void prepareParameter(GetFeaturedDataParam param)
        {
            if(!param.LatestBlogsCount.HasValue)
            {
                param.LatestBlogsCount = 3;
            }
            if (!param.LatestStrengthTrainingsCount.HasValue)
            {
                param.LatestStrengthTrainingsCount = 3;
            }
            if (!param.RandomSupplementsDefinitionsCount.HasValue)
            {
                param.RandomSupplementsDefinitionsCount = 2;
            }
            if (!param.RandomWorkoutPlansCount.HasValue)
            {
                param.RandomWorkoutPlansCount = 2;
            }
            if (!param.LatestSupplementsDefinitionsCount.HasValue)
            {
                param.LatestSupplementsDefinitionsCount = 2;
            }
            if (!param.LatestTrainingPlansCount.HasValue)
            {
                param.LatestTrainingPlansCount = 2;
            }
        }

        IList<FeaturedEntryObjectDTO> mapToFeaturedEntryObjectDTO(IEnumerable<object[]> blogs)
        {
            return blogs.Select(x => new FeaturedEntryObjectDTO()
            {
                GlobalId = (Guid)x[1],
                Comment = (string)x[2],
                DateTime = (DateTime)x[0],
                User = new UserDTO()
                {
                    GlobalId = (Guid)x[3],
                    Version = (int)x[4],
                    Privacy = new ProfilePrivacyDTO()
                    {
                        CalendarView = (Model.Privacy)(int)x[5],
                        Sizes = (Model.Privacy)(int)x[6],
                        Friends = (Model.Privacy)(int)x[7],
                        BirthdayDate = (Model.Privacy)(int)x[8],
                    },
                    Picture = x[9] != null ? new PictureInfoDTO((Guid)x[9], (string)x[10]) : null,
                    UserName = (string)x[11],
                    Gender = (Gender)(int)x[12],
                    CountryId = (int)x[13],
                    CreationDate = (DateTime)x[14]

                }
            }).ToList();
        }

        void prepareExercisesRecords(Profile dbProfile, FeaturedData result, GetFeaturedDataParam param)
        {
            if(param.SkipRecords)
            {
                return;
            }
            //TODO:Optimalization?
            Profile profile = null;
            Exercise exercise = null;
            //var query = Session.QueryOver<ExerciseProfileData>().JoinAlias(x => x.Profile, () => profile).JoinAlias(x => x.Exercise, () => exercise).Where(x => exercise.UseInRecords);
            //query=ReportService.PublicRecordsQueryCriteria(query, profile);
            //query = query.OrderBy(z => z.MaxWeight).Desc.ThenBy(x => x.Repetitions).Desc;
            var query = Session.QueryOver<ExerciseProfileData>().JoinAlias(x => x.Profile, () => profile).JoinAlias(x => x.Exercise, () => exercise).Where(x => exercise.UseInRecords);
            query=ReportService.PublicRecordsQueryCriteria(query, profile);
            query = query.SelectList(x=>x.SelectMax(h=>h.MaxWeight).SelectGroup(e => e.Exercise.GlobalId));
            query = query.OrderBy(z => z.MaxWeight).Desc.ThenBy(x => x.Repetitions).Desc;
            var res=query.List<object[]>();
            query = Session.QueryOver<ExerciseProfileData>().Fetch(x => x.Exercise).Eager.Fetch(x => x.Profile).Eager.Fetch(x => x.Serie).Eager.Fetch(x => x.Serie.ExerciseProfileData).Eager;
            var langOr = Restrictions.Disjunction();
            foreach (var item in res)
            {
                langOr.Add<ExerciseProfileData>(x=> x.MaxWeight == (decimal)item[0] && x.Exercise.GlobalId == (Guid)item[1]);
            }

            query = query.And(langOr);


            var records = query.List();
            //now we must filter by repetitions to get only one row for every exercise
            var groups=records.GroupBy(x => x.Exercise);
            List<ExerciseProfileData> finallRecords = new List<ExerciseProfileData>();
            foreach (var groupItem in groups)
            {
                finallRecords.Add(groupItem.OrderByDescending(x=>x.Repetitions).ThenByDescending(x=>x.TrainingDate).First());
            }
            result.Records = finallRecords.Map<List<ExerciseRecordsReportResultItem>>();
        }
        void prepareFeaturedEntries(Profile dbProfile, FeaturedData result, GetFeaturedDataParam param)
        {
            //TrainingDay day = null;
            //Profile profile = null;
            //ProfileStatistics stat = null;

            //var blogs = Session.QueryOver<BlogEntry>().JoinAlias(x => x.TrainingDay, () => day).JoinAlias(x => day.Profile, () => profile).JoinAlias(x => profile.Statistics, () => stat)
            //    .Where(x => day.Profile != dbProfile && profile.Privacy.CalendarView == Privacy.Public && stat.TrainingDaysCount >= Constants.PublicTrainingDaysCount && x.Comment != null && x.Comment.StrLength() > 20)
            //    .OrderBy(x => day.TrainingDate).Desc.Take(3).Future();

            if (param.LatestBlogsCount > 0)
            {
                var blogsQUery =
                    Session.CreateSQLQuery(
                        @"select d.TrainingDate,eo.GlobalId,eo.Comment,
d.Profile_id,profile2_.Version as Version21_1_, profile2_.CalendarView as Calendar8_21_1_, profile2_.Sizes as Sizes21_1_, profile2_.Friends as Friends21_1_, profile2_.BirthdayDate as Birthda11_21_1_, profile2_.PictureId as PictureId21_1_, profile2_.HashValue as HashValue21_1_, profile2_.UserName as UserName21_1_, profile2_.Gender as Gender21_1_,  profile2_.CountryId as CountryId21_1_,profile2_.CreationDate as Creatio24_21_1_
FROM entryobject eo
inner join BlogEntry this_ on this_.EntryObject_id=eo.GlobalId 
inner join TrainingDay d ON eo.TrainingDay_id=d.GlobalId 
inner join Profile profile2_ on d.Profile_id=profile2_.GlobalId 
inner join ProfileStatistics stat3_ on profile2_.Statistics_id=stat3_.GlobalId 
    inner join 
        (select d1.Profile_Id, max(d1.TrainingDate) as other_col 
         from TrainingDay d1,BlogEntry be1,EntryObject eo1 
		where be1.EntryObject_id=eo1.GlobalId and eo1.TrainingDay_id=d1.GlobalId 
		group by Profile_Id) as b 
	on  d.Profile_Id = b.Profile_Id and d.TrainingDate = b.other_col
where ((not (d.Profile_id = :ProfileId) and profile2_.CalendarView = :Privacy) and stat3_.TrainingDaysCount >= :TrainingDaysCount
and not (eo.Comment is null) and length(eo.Comment) > :blogLength) 
order by d.TrainingDate desc limit :limitRow");
                blogsQUery.SetGuid("ProfileId", dbProfile.GlobalId);
                blogsQUery.SetEnum("Privacy", Privacy.Public);
                blogsQUery.SetInt32("TrainingDaysCount", Portable.Constants.PublicTrainingDaysCount);
                blogsQUery.SetInt32("limitRow", param.LatestBlogsCount.Value);
                blogsQUery.SetInt32("blogLength", 20);
                var blogs = blogsQUery.List<object[]>();
                result.LatestBlogs = mapToFeaturedEntryObjectDTO(blogs);
            }
            //var strengthTrainings = Session.QueryOver<StrengthTrainingEntry>().JoinAlias(x => x.TrainingDay, () => day).JoinAlias(x => day.Profile, () => profile).JoinAlias(x => profile.Statistics, () => stat)
            //    .Where(x => day.Profile != dbProfile && profile.Privacy.CalendarView == Privacy.Public && stat.TrainingDaysCount >= Constants.PublicTrainingDaysCount )
            //    .OrderBy(x => day.TrainingDate).Desc.Take(3).Future();

            if (param.LatestStrengthTrainingsCount > 0)
            {
                var strengthTrainingQuery =
                    Session.CreateSQLQuery(
                        @"select d.TrainingDate,eo.GlobalId,eo.Comment,
d.Profile_id,profile2_.Version as Version21_1_, profile2_.CalendarView as Calendar8_21_1_, profile2_.Sizes as Sizes21_1_, profile2_.Friends as Friends21_1_, profile2_.BirthdayDate as Birthda11_21_1_, profile2_.PictureId as PictureId21_1_, profile2_.HashValue as HashValue21_1_, profile2_.UserName as UserName21_1_, profile2_.Gender as Gender21_1_,  profile2_.CountryId as CountryId21_1_,profile2_.CreationDate as Creatio24_21_1_
FROM entryobject eo
inner join StrengthTrainingEntry this_ on this_.EntryObject_id=eo.GlobalId 
inner join TrainingDay d ON eo.TrainingDay_id=d.GlobalId 
inner join Profile profile2_ on d.Profile_id=profile2_.GlobalId 
inner join ProfileStatistics stat3_ on profile2_.Statistics_id=stat3_.GlobalId 
    inner join 
        (select d1.Profile_Id, max(d1.TrainingDate) as other_col 
         from TrainingDay d1,StrengthTrainingEntry be1,EntryObject eo1 
		where be1.EntryObject_id=eo1.GlobalId and eo1.TrainingDay_id=d1.GlobalId 
		group by Profile_Id) as b 
	on  d.Profile_Id = b.Profile_Id and d.TrainingDate = b.other_col
where ((not (d.Profile_id = :ProfileId) and profile2_.CalendarView = :Privacy) and stat3_.TrainingDaysCount >= :TrainingDaysCount) 
order by d.TrainingDate desc limit :limitRow");
                strengthTrainingQuery.SetGuid("ProfileId", dbProfile.GlobalId);
                strengthTrainingQuery.SetEnum("Privacy", Privacy.Public);
                strengthTrainingQuery.SetInt32("TrainingDaysCount", Portable.Constants.PublicTrainingDaysCount);
                strengthTrainingQuery.SetInt32("limitRow", param.LatestStrengthTrainingsCount.Value);
                var strengthTrainings = strengthTrainingQuery.List<object[]>();

                result.LatestStrengthTrainings = mapToFeaturedEntryObjectDTO(strengthTrainings);
            }
            //result.LatestBlogs = blogs.Map<IList<FeaturedEntryObjectDTO>>();
            //result.LatestStrengthTrainings = strengthTrainings.Map<IList<FeaturedEntryObjectDTO>>();
            
        }

//        private void prepareFeaturedTrainingPlans(Profile dbProfile, FeaturedData result, GetFeaturedDataParam param)
//        {
//            var trainingPlansCount = Session.QueryOver<TrainingPlan>().Where(
//                        x => x.Profile != dbProfile && x.Status == PublishStatus.Published).
//                        ToRowCountQuery().FutureValue<int>();
//            if (param.LatestTrainingPlansCount > 0)
//            {
////first take a two latest training plans except current user's plans
//            var queryTrainingPlans =
//                Session.QueryOver<TrainingPlan>() .Where(
//                    x => x.Profile != dbProfile && x.Status == PublishStatus.Published).
//                    OrderBy(x => x.PublishDate).Desc.Take(param.LatestTrainingPlansCount.Value).List();
//            result.LatestTrainingPlans = queryTrainingPlans.Map<IList<BodyArchitect.Service.V2.Model.TrainingPlans.TrainingPlan>>();
//            }

//            if (param.RandomWorkoutPlansCount > 0)
//            {
//                //now take a random plan except current user's plans and previously selected
//                int count = trainingPlansCount.Value - result.LatestTrainingPlans.Count;
//                Random rnd = new Random();
//                var randomIndex = rnd.Next(count);
//                var randomTrainingPlans =Session.QueryOver<TrainingPlan>() .Where(
//                        x => x.Profile != dbProfile && x.Status == PublishStatus.Published).WhereRestrictionOn(x => x.GlobalId).Not.IsIn(
//                            result.LatestTrainingPlans.Select(b => b.GlobalId).ToList()).Skip(randomIndex).Take(param.RandomWorkoutPlansCount.Value).List();

//                result.RandomTrainingPlans = randomTrainingPlans.Map<IList<BodyArchitect.Service.V2.Model.TrainingPlans.TrainingPlan>>();
//            }
//        }
        private void prepareFeaturedTrainingPlans(Profile dbProfile, FeaturedData result, GetFeaturedDataParam param)
        {
            var trainingPlansCount = Session.QueryOver<TrainingPlan>().Where(
                        x => x.Profile != dbProfile && x.Status == PublishStatus.Published).
                        ToRowCountQuery().FutureValue<int>();
            var planIds =
                    Session.QueryOver<TrainingPlan>().Where(
                        x => x.Profile != dbProfile && x.Status == PublishStatus.Published).
                        OrderBy(x => x.PublishDate).Desc.Select(x => x.GlobalId).Take(param.LatestTrainingPlansCount.Value).List<Guid>();

            if (param.LatestTrainingPlansCount > 0)
            {
                //first take a two latest training plans except current user's plans
                var fetchQuery = Session.QueryOver<BodyArchitect.Model.TrainingPlan>()
                    .Fetch(t => t.Days).Eager
                .Fetch(t => t.Days.First().Entries).Eager
                .Fetch(t => t.Days.First().Entries.First().Exercise).Eager
                .Fetch(t => t.Days.First().Entries.First().Sets).Eager.WhereRestrictionOn(x => x.GlobalId).IsIn(planIds.ToArray()).OrderBy(x => x.PublishDate).Desc;
                fetchQuery.TransformUsing(Transformers.DistinctRootEntity);
                result.LatestTrainingPlans = fetchQuery.List().Map<IList<BodyArchitect.Service.V2.Model.TrainingPlans.TrainingPlan>>();
            }

            if (param.RandomWorkoutPlansCount > 0)
            {
                //now take a random plan except current user's plans and previously selected
                int count = trainingPlansCount.Value - result.LatestTrainingPlans.Count;
                Random rnd = new Random();
                var randomIndex = rnd.Next(count);
                var randomTrainingPlansIds = Session.QueryOver<TrainingPlan>().Where(
                        x => x.Profile != dbProfile && x.Status == PublishStatus.Published).WhereRestrictionOn(x => x.GlobalId).Not.IsIn(
                            result.LatestTrainingPlans.Select(b => b.GlobalId).ToList()).Select(x => x.GlobalId).Skip(randomIndex).Take(param.RandomWorkoutPlansCount.Value).List<Guid>();

                var fetchQuery = Session.QueryOver<BodyArchitect.Model.TrainingPlan>()
                    .Fetch(t => t.Days).Eager
                .Fetch(t => t.Days.First().Entries).Eager
                .Fetch(t => t.Days.First().Entries.First().Exercise).Eager
                .Fetch(t => t.Days.First().Entries.First().Sets).Eager.WhereRestrictionOn(x => x.GlobalId).IsIn(randomTrainingPlansIds.ToArray()).OrderBy(x => x.PublishDate).Desc;
                fetchQuery.TransformUsing(Transformers.DistinctRootEntity);

                result.RandomTrainingPlans = fetchQuery.List().Map<IList<BodyArchitect.Service.V2.Model.TrainingPlans.TrainingPlan>>();
            }
        }

        private void prepareFeaturedSupplementsCycleDefinitions(FeaturedData result, Profile dbProfile, GetFeaturedDataParam param)
        {
            Random rnd=new Random();
            int randomIndex;
            int count;
//TODO:Optimalization
            //take now two IsLegal! latest supplements definitions except current user's plans
            var supplementsDefinitionCount =
                Session.QueryOver<SupplementCycleDefinition>().Where(
                    x => x.Profile != dbProfile && !x.CanBeIllegal && x.Status == PublishStatus.Published).ToRowCountQuery().
                    FutureValue<int>();
            var supplementsDefinitionIds =
                    Session.QueryOver<SupplementCycleDefinition>().Where(
                        x => x.Profile != dbProfile && !x.CanBeIllegal && x.Status == PublishStatus.Published).OrderBy(
                            x => x.PublishDate).Desc.Select(x => x.GlobalId).Take(
                                param.LatestSupplementsDefinitionsCount.Value).List<Guid>();

            if (param.LatestSupplementsDefinitionsCount > 0)
            {
                var fetchQuery =Session.QueryOver<SupplementCycleDefinition>()
                        .Fetch(x => x.Weeks).Eager
                        .Fetch(x => x.Weeks.First()).Eager
                        .Fetch(x => x.Weeks.First().Dosages).Eager
                        .Fetch(x => ((SupplementCycleDosage) x.Weeks.First().Dosages.First()).Supplement).Eager
                        .Fetch(x => x.Profile).Eager
                        .WhereRestrictionOn(x => x.GlobalId).IsIn(supplementsDefinitionIds.ToArray()).OrderBy(
                            x => x.PublishDate).Desc;
                fetchQuery.TransformUsing(Transformers.DistinctRootEntity);
                result.SupplementsDefinitions = fetchQuery.List().Map<IList<SupplementCycleDefinitionDTO>>();
            }

            if (param.RandomSupplementsDefinitionsCount > 0)
            {
                //count = supplementsDefinitionCount.Value - result.SupplementsDefinitions.Count;
                //randomIndex = rnd.Next(count);
                count = supplementsDefinitionCount.Value - result.SupplementsDefinitions.Count;
                randomIndex = rnd.Next(count);

                var randomSupplementsDefinitionIds =
                    Session.QueryOver<SupplementCycleDefinition>().Where(
                        x => x.Profile != dbProfile && !x.CanBeIllegal && x.Status == PublishStatus.Published).
                        WhereRestrictionOn(
                            x => x.GlobalId).Not.IsIn(supplementsDefinitionIds.ToList()).Select(x => x.GlobalId)
                            .Skip(randomIndex).Take(param.RandomSupplementsDefinitionsCount.Value).List<Guid>();
                var fetchRandomQuery =
                    Session.QueryOver<SupplementCycleDefinition>()
                        .Fetch(x => x.Weeks).Eager
                        .Fetch(x => x.Weeks.First()).Eager
                        .Fetch(x => x.Weeks.First().Dosages).Eager
                        .Fetch(x => ((SupplementCycleDosage) x.Weeks.First().Dosages.First()).Supplement).Eager
                        .Fetch(x => x.Profile).Eager
                        .WhereRestrictionOn(x => x.GlobalId).IsIn(randomSupplementsDefinitionIds.ToList());
                fetchRandomQuery.TransformUsing(Transformers.DistinctRootEntity);
                result.RandomSupplementsDefinitions = fetchRandomQuery.List().Map<IList<SupplementCycleDefinitionDTO>>();
            }
        }
    }
}
