using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Model;
using BodyArchitect.Service;
using BodyArchitect.Service.Model;
using BodyArchitect.Shared;
using NHibernate;
using NUnit.Framework;
using NHibernate.Linq;
using PublishStatus = BodyArchitect.Model.PublishStatus;
using TrainingEnd = BodyArchitect.Model.TrainingEnd;
using WorkoutPlanPurpose = BodyArchitect.Model.WorkoutPlanPurpose;

namespace BodyArchitect.UnitTests
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
                dto.Id = profile1.Id;
                dto.UserName = profile1.UserName;
                profile1.Tag = dto;
                profiles.Add(profile1);

                Profile profile2 = new Profile();
                profile2.UserName = "test2";
                profile2.Email = "mai13l@wfg.pl";
                Session.SaveOrUpdate(profile2);

                dto = new ProfileDTO();
                dto.Id = profile2.Id;
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

        

        [Test]
        public void TestGetWorkoutPlans_UserName()
        {
            WorkoutPlanSearchCriteria criteria = WorkoutPlanSearchCriteria.CreateFindAllCriteria();
            criteria.UserName = profiles[0].UserName;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;
            PagedResult<WorkoutPlanDTO> result = null;

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 7, new[] { "test1-1", "test1-2", "test1-3","test1-4","test1-5", "test1-6", "test1-7" }, null);

            criteria.UserName = profiles[1].UserName;
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 4, new[] { "test2-2", "test2-3", "test2-4", "test2-7" }, null);
        }

        [Test]
        [Ignore]
        public void TestGetWorkoutPlans_LoadPlanContent()
        {

            WorkoutPlanSearchCriteria criteria = WorkoutPlanSearchCriteria.CreateFindAllCriteria();
            criteria.UserName = profiles[0].UserName;
            PagedResult<WorkoutPlanDTO> result = null;
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.LongTexts = false;
            pageInfo.PageSize = 50;

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            foreach (var plan in result.Items)
            {
                Assert.IsNull(plan.PlanContent);
            }

            pageInfo.LongTexts = true;
            criteria.UserName = profiles[1].UserName;
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            foreach (var plan in result.Items)
            {
                Assert.AreEqual("plan content",plan.PlanContent);
            }
        }

        [Test]
        public void TestGetWorkoutPlans_OnlyOneDayCriteria()
        {
            WorkoutPlanSearchCriteria criteria = WorkoutPlanSearchCriteria.CreateFindAllCriteria();
            criteria.Days.Add(3);
            PagedResult<WorkoutPlanDTO> result = null;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 5, new[] { "test1-1", "test1-2", "test1-6", "test2-2", "test2-7" }, null);

            profile = (ProfileDTO)profiles[1].Tag;
            data = SecurityManager.CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 6, new[] { "test1-1", "test1-6", "test2-2", "test2-7", "test2-1", "test2-6" }, null);
        }

        [Test]
        public void TestGetWorkoutPlans_OnlyTwoDaysCriteria()
        {
            WorkoutPlanSearchCriteria criteria = WorkoutPlanSearchCriteria.CreateFindAllCriteria();
            criteria.Days.Add(3);
            criteria.Days.Add(4);
            PagedResult<WorkoutPlanDTO> result = null;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 8, new[] { "test1-1", "test1-2","test1-3", "test1-6","test1-7", "test2-2","test2-3", "test2-7" }, null);

            profile = (ProfileDTO)profiles[1].Tag;
            data = SecurityManager.CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 10, new[] { "test1-1", "test1-3", "test1-6", "test1-7", "test2-2", "test2-3", "test2-7", "test2-1", "test2-6", "test2-8" }, null);
        }

        [Test]
        public void TestGetWorkoutPlans_OnlyStrengthPurpose()
        {
            WorkoutPlanSearchCriteria criteria = WorkoutPlanSearchCriteria.CreateFindAllCriteria();
            criteria.Purposes.Add(Service.Model.WorkoutPlanPurpose.Strength);
            PagedResult<WorkoutPlanDTO> result = null;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 2, new[] { "test1-4", "test1-7" }, null);

            profile = (ProfileDTO)profiles[1].Tag;
            data = SecurityManager.CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result,4, new[] { "test1-7", "test2-1", "test2-5", "test2-6" }, null);
        }

        [Test]
        public void TestGetWorkoutPlans_OnlyOneLanguage()
        {
            WorkoutPlanSearchCriteria criteria = WorkoutPlanSearchCriteria.CreateFindAllCriteria();
            criteria.Languages.Add(Language.Languages[0].Shortcut);
            PagedResult<WorkoutPlanDTO> result = null;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 5, new[] { "test1-1", "test1-2", "test1-7", "test2-2", "test2-7" }, null);

            profile = (ProfileDTO)profiles[1].Tag;
            data = SecurityManager.CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 5, new[] { "test1-1", "test1-7", "test2-1", "test2-2", "test2-7" }, null);
        }

        [Test]
        public void TestGetWorkoutPlans_OnlyTwoLanguages()
        {
            WorkoutPlanSearchCriteria criteria = WorkoutPlanSearchCriteria.CreateFindAllCriteria();
            criteria.Languages.Add(Language.Languages[0].Shortcut);
            criteria.Languages.Add(Language.Languages[1].Shortcut);
            PagedResult<WorkoutPlanDTO> result = null;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 8, new[] { "test1-1", "test1-2", "test1-3", "test1-5", "test1-7", "test2-2", "test2-4", "test2-7" }, null);

            profile = (ProfileDTO)profiles[1].Tag;
            data = SecurityManager.CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 8, new[] { "test1-1", "test1-3", "test1-7", "test2-1", "test2-2", "test2-4","test2-5", "test2-7" }, null);
        }

        [Test]
        public void TestGetWorkoutPlans_OnlySplit()
        {
            WorkoutPlanSearchCriteria criteria = WorkoutPlanSearchCriteria.CreateFindAllCriteria();
            criteria.WorkoutPlanType.Add(BodyArchitect.Service.Model.TrainingPlans.TrainingType.Split);
            PagedResult<WorkoutPlanDTO> result = null;
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 2, new[] { "test1-4", "test1-6" }, null);

            profile = (ProfileDTO)profiles[1].Tag;
            data = SecurityManager.CreateNewSession(profile, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 3, new[] { "test1-6", "test2-5", "test2-8" }, null);
        }

        [Test]
        public void TestGetWorkoutPlans_OnlyFavorites()
        {
            WorkoutPlanSearchCriteria criteria = WorkoutPlanSearchCriteria.CreateFindAllCriteria();
            criteria.SearchGroups.Remove(WorkoutPlanSearchCriteriaGroup.Other);
            criteria.SearchGroups.Remove(WorkoutPlanSearchCriteriaGroup.Mine);
            PagedResult<WorkoutPlanDTO> result = null;
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 1, new [] { "test2-2" },null);

            profile = (ProfileDTO)profiles[1].Tag;
            data = SecurityManager.CreateNewSession(profile, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 2, new[] { "test1-1", "test1-6" },null);
        }

        [Test]
        public void TestGetWorkoutPlans_OnlyFavoritesAndOther_bugWhenNoFavorites()
        {
            WorkoutPlanSearchCriteria criteria = WorkoutPlanSearchCriteria.CreateFindAllCriteria();
            criteria.SearchGroups.Remove(WorkoutPlanSearchCriteriaGroup.Mine);
            //clear favorites plans
            profiles[0].FavoriteWorkoutPlans.Clear();
            Session.SaveOrUpdate(profiles[0]);
            PagedResult<WorkoutPlanDTO> result = null;
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
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
            WorkoutPlanSearchCriteria criteria = WorkoutPlanSearchCriteria.CreateFindAllCriteria();
            criteria.SearchGroups.Remove(WorkoutPlanSearchCriteriaGroup.Other);
            PagedResult<WorkoutPlanDTO> result = null;
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 8, new[] { "test2-2", "test1-1", "test1-2", "test1-3", "test1-4", "test1-5", "test1-6", "test1-7" }, null);

            profile = (ProfileDTO)profiles[1].Tag;
            data = SecurityManager.CreateNewSession(profile, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 10, new[] { "test1-1", "test1-6", "test2-1", "test2-2", "test2-3", "test2-4", "test2-5", "test2-6", "test2-7", "test2-8" }, null);
        }

        [Test]
        public void TestGetWorkoutPlans_OnlyBeginners()
        {
            PagedResult<WorkoutPlanDTO> result = null;
            WorkoutPlanSearchCriteria criteria = WorkoutPlanSearchCriteria.CreateFindAllCriteria();
            criteria.Difficults.Add(Service.Model.TrainingPlans.TrainingPlanDifficult.Beginner);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 5, new[] { "test1-1", "test1-3", "test1-5", "test1-7", "test2-3" }, null);

            profile = (ProfileDTO)profiles[1].Tag;
            data = SecurityManager.CreateNewSession(profile, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 6, new[] { "test1-1", "test1-3", "test1-7", "test2-3", "test2-5", "test2-8" }, null);
        }

        [Test]
        public void TestGetWorkoutPlans_OnlyMine()
        {
            PagedResult<WorkoutPlanDTO> result = null;
            WorkoutPlanSearchCriteria criteria = WorkoutPlanSearchCriteria.CreateFindAllCriteria();
            criteria.SearchGroups.Remove(WorkoutPlanSearchCriteriaGroup.Other);
            criteria.SearchGroups.Remove(WorkoutPlanSearchCriteriaGroup.Favorites);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 7, new[] { "test1-1", "test1-2", "test1-3", "test1-4", "test1-5", "test1-6", "test1-7" }, null);

            profile = (ProfileDTO)profiles[1].Tag;
            data = SecurityManager.CreateNewSession(profile, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result,8, new[] { "test2-1", "test2-2", "test2-3", "test2-4", "test2-5", "test2-6", "test2-7", "test2-8" }, null);
        }

        [Test]
        public void TestGetWorkoutPlans_OnlyOther()
        {
            WorkoutPlanSearchCriteria criteria = WorkoutPlanSearchCriteria.CreateFindAllCriteria();
            criteria.SearchGroups.Remove(WorkoutPlanSearchCriteriaGroup.Mine);
            criteria.SearchGroups.Remove(WorkoutPlanSearchCriteriaGroup.Favorites);
            PagedResult<WorkoutPlanDTO> result = null;
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 3, new[] { "test2-3", "test2-4", "test2-7" }, null);

            profile = (ProfileDTO)profiles[1].Tag;
            data = SecurityManager.CreateNewSession(profile, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 2, new[] { "test1-3", "test1-7" }, null);
        }

        [Test]
        public void TestGetWorkoutPlans_None()
        {
            PagedResult<WorkoutPlanDTO> result = null;
            WorkoutPlanSearchCriteria criteria =new WorkoutPlanSearchCriteria();
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 0, null,null);

            profile = (ProfileDTO)profiles[1].Tag;
            data = SecurityManager.CreateNewSession(profile, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 0, null, null);
        }

        [Test]
        public void TestGetWorkoutPlans_All()
        {
            PagedResult<WorkoutPlanDTO> result = null;
            WorkoutPlanSearchCriteria criteria = WorkoutPlanSearchCriteria.CreateFindAllCriteria();
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 11, null, new[] { "test2-1", "test2-5", "test2-6", "test2-8" });

            profile = (ProfileDTO)profiles[1].Tag;
            data = SecurityManager.CreateNewSession(profile, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetWorkoutPlans(data.Token, criteria, pageInfo);
            });
            assertWorkoutPlansListPack(result, 12, null, new[] { "test1-2", "test1-4", "test1-5" });
        }

        void assertWorkoutPlansListPack(PagedResult<WorkoutPlanDTO> pack, int allItems, string[] availablePlanNames, string[] notAvailablePlans)
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

        WorkoutPlanDTO findPlan(string name,IList<WorkoutPlanDTO> plans)
        {
            return (from plan in plans where plan.Name == name select plan).SingleOrDefault();
        }
    }
}
