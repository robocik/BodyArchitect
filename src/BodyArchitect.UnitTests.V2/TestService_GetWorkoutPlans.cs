using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using BodyArchitect.Shared;
using NHibernate;
using NUnit.Framework;
using NHibernate.Linq;
using PublishStatus = BodyArchitect.Model.PublishStatus;
using TrainingEnd = BodyArchitect.Model.TrainingEnd;
using TrainingPlan = BodyArchitect.Model.TrainingPlan;
using TrainingPlanDifficult = BodyArchitect.Model.TrainingPlanDifficult;
using TrainingType = BodyArchitect.Model.TrainingType;
using WorkoutPlanPurpose = BodyArchitect.Model.WorkoutPlanPurpose;

namespace BodyArchitect.UnitTests.V2
{

    [TestFixture]
    public class TestService_GetWorkoutPlans : TestServiceBase
    {
        List<Profile > profiles  = new List<Profile>();
        Dictionary<string,TrainingPlan> workoutPlans = new Dictionary<string, TrainingPlan>();

        public override void BuildDatabase()
        {

            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                workoutPlans.Clear();
                Profile profile1 = new Profile();
                profile1.UserName = "test1";
                profile1.Email = "mail@wfg.pl";
                Session.SaveOrUpdate(profile1);
                ProfileDTO dto = new ProfileDTO();
                dto.GlobalId = profile1.GlobalId;
                dto.UserName = profile1.UserName;
                profile1.Tag = dto;
                profiles.Add(profile1);

                Profile profile2 = new Profile();
                profile2.UserName = "test2";
                profile2.Email = "mai13l@wfg.pl";
                Session.SaveOrUpdate(profile2);

                dto = new ProfileDTO();
                dto.GlobalId = profile2.GlobalId;
                dto.UserName = profile2.UserName;
                profile2.Tag = dto;
                profiles.Add(profile2);

                //creates workout plans for profile 1
                var workoutPlan = CreatePlan(Session,profile1, "test1-1", TrainingPlanDifficult.Beginner, TrainingType.HST, true,Language.Languages[0].Shortcut,WorkoutPlanPurpose.FatLost,3);
                workoutPlans.Add(workoutPlan.Name, workoutPlan);
                workoutPlan = CreatePlan(Session, profile1, "test1-2", TrainingPlanDifficult.Advanced, TrainingType.HST, false, Language.Languages[0].Shortcut, WorkoutPlanPurpose.Mass, 3);
                workoutPlans.Add(workoutPlan.Name, workoutPlan);
                workoutPlan = CreatePlan(Session, profile1, "test1-3", TrainingPlanDifficult.Beginner, TrainingType.PushPull, true, Language.Languages[1].Shortcut, WorkoutPlanPurpose.Definition, 4);
                workoutPlans.Add(workoutPlan.Name, workoutPlan);
                workoutPlan = CreatePlan(Session, profile1, "test1-4", TrainingPlanDifficult.NotSet, TrainingType.Split, false, Language.Languages[2].Shortcut, WorkoutPlanPurpose.Strength, 2);
                workoutPlans.Add(workoutPlan.Name, workoutPlan);
                workoutPlan = CreatePlan(Session, profile1, "test1-5", TrainingPlanDifficult.Beginner, TrainingType.FBW, false, Language.Languages[1].Shortcut, WorkoutPlanPurpose.Mass, 2);
                workoutPlans.Add(workoutPlan.Name, workoutPlan);
                workoutPlan = CreatePlan(Session, profile1, "test1-6", TrainingPlanDifficult.Professional, TrainingType.Split, true, Language.Languages[2].Shortcut, WorkoutPlanPurpose.Mass, 3);
                workoutPlans.Add(workoutPlan.Name, workoutPlan);
                workoutPlan = CreatePlan(Session, profile1, "test1-7", TrainingPlanDifficult.Beginner, TrainingType.ACT, true, Language.Languages[0].Shortcut, WorkoutPlanPurpose.Strength, 4);
                workoutPlans.Add(workoutPlan.Name, workoutPlan);
                
                //now for profile 2
                workoutPlan = CreatePlan(Session, profile2, "test2-1", TrainingPlanDifficult.Advanced, TrainingType.HST, false, Language.Languages[0].Shortcut, WorkoutPlanPurpose.Strength, 3);
                workoutPlans.Add(workoutPlan.Name, workoutPlan);
                workoutPlan = CreatePlan(Session, profile2, "test2-2", TrainingPlanDifficult.Professional, TrainingType.ACT, true, Language.Languages[0].Shortcut, WorkoutPlanPurpose.Mass, 3);
                workoutPlans.Add(workoutPlan.Name, workoutPlan);
                workoutPlan = CreatePlan(Session, profile2, "test2-3", TrainingPlanDifficult.Beginner, TrainingType.PushPull, true, Language.Languages[2].Shortcut, WorkoutPlanPurpose.Definition, 4);
                workoutPlans.Add(workoutPlan.Name, workoutPlan);
                workoutPlan = CreatePlan(Session, profile2, "test2-4", TrainingPlanDifficult.NotSet, TrainingType.HIT, true, Language.Languages[1].Shortcut, WorkoutPlanPurpose.Mass, 2);
                workoutPlans.Add(workoutPlan.Name, workoutPlan);
                workoutPlan = CreatePlan(Session, profile2, "test2-5", TrainingPlanDifficult.Beginner, TrainingType.Split, false, Language.Languages[1].Shortcut, WorkoutPlanPurpose.Strength, 2);
                workoutPlans.Add(workoutPlan.Name, workoutPlan);
                workoutPlan = CreatePlan(Session, profile2, "test2-6", TrainingPlanDifficult.Advanced, TrainingType.ACT, false, Language.Languages[2].Shortcut, WorkoutPlanPurpose.Strength, 3);
                workoutPlans.Add(workoutPlan.Name, workoutPlan);
                workoutPlan = CreatePlan(Session, profile2, "test2-7", TrainingPlanDifficult.NotSet, TrainingType.FBW, true, Language.Languages[0].Shortcut, WorkoutPlanPurpose.FatLost, 3);
                workoutPlans.Add(workoutPlan.Name, workoutPlan);
                workoutPlan = CreatePlan(Session, profile2, "test2-8", TrainingPlanDifficult.Beginner, TrainingType.Split, false, Language.Languages[2].Shortcut, WorkoutPlanPurpose.Mass, 4);
                workoutPlans.Add(workoutPlan.Name, workoutPlan);

                //add favorities for profile 1
                profile1.FavoriteWorkoutPlans.Add(workoutPlans["test2-2"]);
                Session.SaveOrUpdate(profile1);
                //and for profile 2
                profile2.FavoriteWorkoutPlans.Add(workoutPlans["test1-1"]);
                profile2.FavoriteWorkoutPlans.Add(workoutPlans["test1-6"]);
                Session.SaveOrUpdate(profile2);
                tx.Commit();
            }
            
        }

        #region GetWorkoutPlans

        [Test]
        public void TestGetWorkoutPlans_UserId_OnlyMine()
        {
            WorkoutPlanSearchCriteria criteria = new WorkoutPlanSearchCriteria();
            criteria.UserId = profiles[0].GlobalId;
            criteria.SearchGroups.Add(WorkoutPlanSearchCriteriaGroup.Mine);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;
            PagedResult<Service.V2.Model.TrainingPlans.TrainingPlan> result = null;

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                 result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 7, new[] { "test1-1", "test1-2", "test1-3","test1-4","test1-5", "test1-6", "test1-7" }, null);

            criteria.UserId = profiles[1].GlobalId;
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 4, new[] { "test2-2", "test2-3", "test2-4", "test2-7" }, null);
        }

        //[Test]
        //[Ignore]
        //public void TestGetWorkoutPlans_LoadPlanContent()
        //{

        //    WorkoutPlanSearchCriteria criteria = WorkoutPlanSearchCriteria.CreateFindAllCriteria();
        //    criteria.UserName = profiles[0].UserName;
        //    PagedResult<TrainingPlanInfo> result = null;
        //    var profile = (ProfileDTO)profiles[0].Tag;
        //    SessionData data = CreateNewSession(profile, ClientInformation);
        //    PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
        //    pageInfo.LongTexts = false;
        //    pageInfo.PageSize = 50;

        //    RunServiceMethod(delegate(InternalBodyArchitectService service)
        //    {
        //        result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
        //    });
        //    foreach (var plan in result.Items)
        //    {
        //        Assert.IsNull(plan.PlanContent);
        //    }

        //    pageInfo.LongTexts = true;
        //    criteria.UserName = profiles[1].UserName;
            
        //    RunServiceMethod(delegate(InternalBodyArchitectService service)
        //    {
        //        result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
        //    });
        //    foreach (var plan in result.Items)
        //    {
        //        Assert.AreEqual("plan content",plan.PlanContent);
        //    }
        //}

        [Test]
        public void TestGetWorkoutPlans_OnlyOneDayCriteria()
        {
            WorkoutPlanSearchCriteria criteria =new WorkoutPlanSearchCriteria();
            criteria.Days.Add(3);
            PagedResult<Service.V2.Model.TrainingPlans.TrainingPlan> result = null;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 5, new[] { "test1-1", "test1-2", "test1-6", "test2-2", "test2-7" }, null);

            profile = (ProfileDTO)profiles[1].Tag;
            data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 6, new[] { "test1-1", "test1-6", "test2-2", "test2-7", "test2-1", "test2-6" }, null);
        }

        [Test]
        public void TestGetWorkoutPlans_OnlyTwoDaysCriteria()
        {
            WorkoutPlanSearchCriteria criteria = new WorkoutPlanSearchCriteria();
            criteria.Days.Add(3);
            criteria.Days.Add(4);
            PagedResult<Service.V2.Model.TrainingPlans.TrainingPlan> result = null;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 8, new[] { "test1-1", "test1-2","test1-3", "test1-6","test1-7", "test2-2","test2-3", "test2-7" }, null);

            profile = (ProfileDTO)profiles[1].Tag;
            data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 10, new[] { "test1-1", "test1-3", "test1-6", "test1-7", "test2-2", "test2-3", "test2-7", "test2-1", "test2-6", "test2-8" }, null);
        }

        [Test]
        public void TestGetWorkoutPlans_OnlyStrengthPurpose()
        {
            WorkoutPlanSearchCriteria criteria = new WorkoutPlanSearchCriteria();
            criteria.Purposes.Add(Service.V2.Model.WorkoutPlanPurpose.Strength);
            PagedResult<Service.V2.Model.TrainingPlans.TrainingPlan> result = null;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 2, new[] { "test1-4", "test1-7" }, null);

            profile = (ProfileDTO)profiles[1].Tag;
            data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result,4, new[] { "test1-7", "test2-1", "test2-5", "test2-6" }, null);
        }

        [Test]
        public void TestGetWorkoutPlans_OnlyOneLanguage()
        {
            WorkoutPlanSearchCriteria criteria = new WorkoutPlanSearchCriteria();
            criteria.Languages.Add(Language.Languages[0].Shortcut);
            PagedResult<Service.V2.Model.TrainingPlans.TrainingPlan> result = null;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 5, new[] { "test1-1", "test1-2", "test1-7", "test2-2", "test2-7" }, null);

            profile = (ProfileDTO)profiles[1].Tag;
            data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 5, new[] { "test1-1", "test1-7", "test2-1", "test2-2", "test2-7" }, null);
        }

        [Test]
        public void TestGetWorkoutPlans_OnlyTwoLanguages()
        {
            WorkoutPlanSearchCriteria criteria = new WorkoutPlanSearchCriteria();
            criteria.Languages.Add(Language.Languages[0].Shortcut);
            criteria.Languages.Add(Language.Languages[1].Shortcut);
            PagedResult<Service.V2.Model.TrainingPlans.TrainingPlan> result = null;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 8, new[] { "test1-1", "test1-2", "test1-3", "test1-5", "test1-7", "test2-2", "test2-4", "test2-7" }, null);

            profile = (ProfileDTO)profiles[1].Tag;
            data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 8, new[] { "test1-1", "test1-3", "test1-7", "test2-1", "test2-2", "test2-4","test2-5", "test2-7" }, null);
        }

        [Test]
        public void TestGetWorkoutPlans_OnlySplit()
        {
            WorkoutPlanSearchCriteria criteria = new WorkoutPlanSearchCriteria();
            criteria.WorkoutPlanType.Add(BodyArchitect.Service.V2.Model.TrainingPlans.TrainingType.Split);
            PagedResult<Service.V2.Model.TrainingPlans.TrainingPlan> result = null;
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 2, new[] { "test1-4", "test1-6" }, null);

            profile = (ProfileDTO)profiles[1].Tag;
            data = CreateNewSession(profile, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 3, new[] { "test1-6", "test2-5", "test2-8" }, null);
        }

        [Test]
        public void TestGetWorkoutPlans_OnlyFavorites()
        {
            WorkoutPlanSearchCriteria criteria = new WorkoutPlanSearchCriteria();
            criteria.SearchGroups.Add(WorkoutPlanSearchCriteriaGroup.Favorites);
            PagedResult<Service.V2.Model.TrainingPlans.TrainingPlan> result = null;
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 1, new [] { "test2-2" },null);

            profile = (ProfileDTO)profiles[1].Tag;
            data = CreateNewSession(profile, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 2, new[] { "test1-1", "test1-6" },null);
        }

        [Test]
        public void TestGetWorkoutPlans_OnlyFavoritesAndOther_bugWhenNoFavorites()
        {
            WorkoutPlanSearchCriteria criteria = new WorkoutPlanSearchCriteria();
            criteria.SearchGroups.Add(WorkoutPlanSearchCriteriaGroup.Other);
            criteria.SearchGroups.Add(WorkoutPlanSearchCriteriaGroup.Favorites);
            //clear favorites plans
            profiles[0].FavoriteWorkoutPlans.Clear();
            Session.SaveOrUpdate(profiles[0]);
            PagedResult<Service.V2.Model.TrainingPlans.TrainingPlan> result = null;
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 4, new[] { "test2-2" }, null);

        }

        [Test]
        public void TestGetWorkoutPlans_OnlyFavoritesAndMine()
        {
            WorkoutPlanSearchCriteria criteria = new WorkoutPlanSearchCriteria();
            criteria.SearchGroups.Add(WorkoutPlanSearchCriteriaGroup.Mine);
            criteria.SearchGroups.Add(WorkoutPlanSearchCriteriaGroup.Favorites);
            PagedResult<Service.V2.Model.TrainingPlans.TrainingPlan> result = null;
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 8, new[] { "test2-2", "test1-1", "test1-2", "test1-3", "test1-4", "test1-5", "test1-6", "test1-7" }, null);

            profile = (ProfileDTO)profiles[1].Tag;
            data = CreateNewSession(profile, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 10, new[] { "test1-1", "test1-6", "test2-1", "test2-2", "test2-3", "test2-4", "test2-5", "test2-6", "test2-7", "test2-8" }, null);
        }

        [Test]
        public void TestGetWorkoutPlans_OnlyBeginners()
        {
            PagedResult<Service.V2.Model.TrainingPlans.TrainingPlan> result = null;
            WorkoutPlanSearchCriteria criteria =new WorkoutPlanSearchCriteria();
            criteria.Difficults.Add(Service.V2.Model.TrainingPlans.TrainingPlanDifficult.Beginner);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 5, new[] { "test1-1", "test1-3", "test1-5", "test1-7", "test2-3" }, null);

            profile = (ProfileDTO)profiles[1].Tag;
            data = CreateNewSession(profile, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 6, new[] { "test1-1", "test1-3", "test1-7", "test2-3", "test2-5", "test2-8" }, null);
        }

        [Test]
        public void TestGetWorkoutPlans_ById()
        {
            PagedResult<Service.V2.Model.TrainingPlans.TrainingPlan> result = null;
            WorkoutPlanSearchCriteria criteria = new WorkoutPlanSearchCriteria();
            criteria.PlanId = workoutPlans.Values.ElementAt(0).GlobalId;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 1, new[] { workoutPlans.Values.ElementAt(0).Name }, null);
        }

        [Test]
        public void TestGetWorkoutPlans_OnlyMine()
        {
            PagedResult<Service.V2.Model.TrainingPlans.TrainingPlan> result = null;
            WorkoutPlanSearchCriteria criteria = new WorkoutPlanSearchCriteria();
            criteria.SearchGroups.Add(WorkoutPlanSearchCriteriaGroup.Mine);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 7, new[] { "test1-1", "test1-2", "test1-3", "test1-4", "test1-5", "test1-6", "test1-7" }, null);

            profile = (ProfileDTO)profiles[1].Tag;
            data = CreateNewSession(profile, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result,8, new[] { "test2-1", "test2-2", "test2-3", "test2-4", "test2-5", "test2-6", "test2-7", "test2-8" }, null);
        }

        [Test]
        public void TestGetWorkoutPlans_OnlyOther()
        {
            WorkoutPlanSearchCriteria criteria = new WorkoutPlanSearchCriteria();
            criteria.SearchGroups.Add(WorkoutPlanSearchCriteriaGroup.Other);
            PagedResult<Service.V2.Model.TrainingPlans.TrainingPlan> result = null;
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 3, new[] { "test2-3", "test2-4", "test2-7" }, null);

            profile = (ProfileDTO)profiles[1].Tag;
            data = CreateNewSession(profile, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 2, new[] { "test1-3", "test1-7" }, null);
        }

        [Test]
        public void TestGetWorkoutPlans_None()
        {
            PagedResult<Service.V2.Model.TrainingPlans.TrainingPlan> result = null;
            WorkoutPlanSearchCriteria criteria =new WorkoutPlanSearchCriteria();
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 11, null, new[] { "test2-1", "test2-5", "test2-6", "test2-8" });

            profile = (ProfileDTO)profiles[1].Tag;
            data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 12, null, new[] { "test1-2", "test1-4", "test1-5" });
        }

        [Test]
        public void TestGetWorkoutPlans_All()
        {
            PagedResult<Service.V2.Model.TrainingPlans.TrainingPlan> result = null;
            WorkoutPlanSearchCriteria criteria = new WorkoutPlanSearchCriteria();
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 11, null, new[] { "test2-1", "test2-5", "test2-6", "test2-8" });

            profile = (ProfileDTO)profiles[1].Tag;
            data = CreateNewSession(profile, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 12, null, new[] { "test1-2", "test1-4", "test1-5" });
        }
        //plan.PlanContent =
//                @"<TrainingPlan>
//  <Purpose>NotSet</Purpose>
//  <Language>en</Language>
//  <Author>rtyt</Author>
//  <Comment><![CDATA[r]]></Comment>
//  <CreationDate>06/28/2011 19:18:26</CreationDate>
//  <Difficult>NotSet</Difficult>
//  <GlobalId>1acfc8e3-7430-4911-a221-89eeca149ee2</GlobalId>
//  <Name>rtyre</Name>
//  <RestSeconds>0</RestSeconds>
//  <TrainingType>Split</TrainingType>
//  <Url></Url>
//  <Weeks>
//    <Day>
//      <GlobalId>d6b0363b-3c1e-450a-934e-760532bf0b9f</GlobalId>
//      <Name>Day 1</Name>
//      <Entries>
//        <Entry>
//          <ExerciseId>02a3b488-1823-4703-aee2-ae0a87adf43d</ExerciseId>
//          <GlobalId>9c29319f-e85b-4762-b966-7fc2b1643f97</GlobalId>
//          <RestSeconds>0</RestSeconds>
//          <Sets />
//        </Entry>
//      </Entries>
//      <SuperSets />
//    </Day>
//  </Weeks>
//</TrainingPlan>";
        #endregion



        void assertWorkoutPlansListPack(PagedResult<Service.V2.Model.TrainingPlans.TrainingPlan> pack, int allItems, string[] availablePlanNames, string[] notAvailablePlans)
        {
            Assert.AreEqual(allItems, pack.AllItemsCount);
            if (availablePlanNames != null)
            {
                foreach (string availablePlanName in availablePlanNames)
                {
                    Assert.IsNotNull(findPlan(availablePlanName, pack.Items),
                                     "Cannot find plan: " + availablePlanName);
                }
            }
            if (notAvailablePlans != null)
            {
                foreach (string notAvailablePlan in notAvailablePlans)
                {
                    Assert.IsNull(findPlan(notAvailablePlan, pack.Items),
                                  "Plan " + notAvailablePlan + " shouldn't be in a results");
                }
            }
        }

        Service.V2.Model.TrainingPlans.TrainingPlan findPlan(string name, IList<Service.V2.Model.TrainingPlans.TrainingPlan> plans)
        {
            return (from plan in plans where plan.Name == name select plan).SingleOrDefault();
        }
    }
}
