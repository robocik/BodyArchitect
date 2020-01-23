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
using ExerciseDifficult = BodyArchitect.Model.ExerciseDifficult;
using ExerciseForceType = BodyArchitect.Model.ExerciseForceType;
using ExerciseType = BodyArchitect.Model.ExerciseType;
using MechanicsType = BodyArchitect.Model.MechanicsType;
using PublishStatus = BodyArchitect.Model.PublishStatus;

namespace BodyArchitect.UnitTests
{
    [TestFixture]
    public class TestService_GetExercises : TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        Dictionary<string, Exercise> exercises = new Dictionary<string, Exercise>();

        public override  void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                exercises.Clear();
                BuildTwoProfilesDatabase(Session, profiles,exercises);
                tx.Commit(); 
            }

        }

        

        public static void BuildTwoProfilesDatabase(ISession session, List<Profile> profiles, Dictionary<string, Exercise> exercises)
        {
            Profile profile1 = CreateProfile(session,"test1");
            profiles.Add(profile1);

            Profile profile2 = CreateProfile(session, "test2");
            profiles.Add(profile2);

            //create global exercises
            var workoutPlan = CreateExercise(session, null, "test0-1", "t01", PublishStatus.Published, ExerciseType.Klatka, MechanicsType.Isolation, ExerciseForceType.Pull, ExerciseDifficult.One);
            workoutPlan.Rating = 2;
            exercises.Add(workoutPlan.Shortcut, workoutPlan);
            workoutPlan = CreateExercise(session, null, "test0-2", "t02", PublishStatus.Published, ExerciseType.Biceps, MechanicsType.Compound, ExerciseForceType.Push, ExerciseDifficult.Two);
            workoutPlan.Rating = 4.3f;
            exercises.Add(workoutPlan.Shortcut, workoutPlan);
            workoutPlan = CreateExercise(session, null, "test0-3", "t03", PublishStatus.Published, ExerciseType.Triceps, MechanicsType.Isolation, ExerciseForceType.Push, ExerciseDifficult.Three);
            workoutPlan.Rating = 1;
            exercises.Add(workoutPlan.Shortcut, workoutPlan);
            workoutPlan = CreateExercise(session, null, "test0-4", "t04", PublishStatus.Published, ExerciseType.Biceps, MechanicsType.Compound, ExerciseForceType.Push, ExerciseDifficult.Two);
            workoutPlan.Rating = 3.2f;
            exercises.Add(workoutPlan.Shortcut, workoutPlan);
            workoutPlan = CreateExercise(session, null, "test0-5", "t05", PublishStatus.Published, ExerciseType.Biceps, MechanicsType.Compound, ExerciseForceType.Push, ExerciseDifficult.One);
            workoutPlan.Rating = 6;
            exercises.Add(workoutPlan.Shortcut, workoutPlan);
            workoutPlan = CreateExercise(session, null, "test0-6", "t06", PublishStatus.Published, ExerciseType.Klatka, MechanicsType.Compound, ExerciseForceType.Push, ExerciseDifficult.One);
            exercises.Add(workoutPlan.Shortcut, workoutPlan);
            workoutPlan = CreateExercise(session, null, "test0-7", "t07", PublishStatus.Published, ExerciseType.Triceps, MechanicsType.NotSet, ExerciseForceType.Pull, ExerciseDifficult.Three);
            exercises.Add(workoutPlan.Shortcut, workoutPlan);
            workoutPlan = CreateExercise(session, null, "test0-8", "t08", PublishStatus.Published, ExerciseType.Triceps, MechanicsType.Isolation, ExerciseForceType.NotSet, ExerciseDifficult.Two);
            exercises.Add(workoutPlan.Shortcut, workoutPlan);


            //creates exercises for profile 1
            workoutPlan = CreateExercise(session, profile1, "test1-1", "t11", PublishStatus.Private, ExerciseType.Klatka, MechanicsType.Isolation, ExerciseForceType.Pull, ExerciseDifficult.One);
            exercises.Add(workoutPlan.Shortcut, workoutPlan);
            workoutPlan = CreateExercise(session, profile1, "test1-2", "t12", PublishStatus.Published, ExerciseType.Biceps, MechanicsType.Compound, ExerciseForceType.Push, ExerciseDifficult.Two);
            exercises.Add(workoutPlan.Shortcut, workoutPlan);
            workoutPlan = CreateExercise(session, profile1, "test1-3", "t13", PublishStatus.Published, ExerciseType.Triceps, MechanicsType.Isolation, ExerciseForceType.Push, ExerciseDifficult.Three);
            exercises.Add(workoutPlan.Shortcut, workoutPlan);
            workoutPlan = CreateExercise(session, profile1, "test1-4", "t14", PublishStatus.Private, ExerciseType.Biceps, MechanicsType.Compound, ExerciseForceType.Push, ExerciseDifficult.Two);
            exercises.Add(workoutPlan.Shortcut, workoutPlan);
            workoutPlan = CreateExercise(session, profile1, "test1-5", "t15", PublishStatus.Published, ExerciseType.Biceps, MechanicsType.Compound, ExerciseForceType.Push, ExerciseDifficult.One);
            exercises.Add(workoutPlan.Shortcut, workoutPlan);
            workoutPlan = CreateExercise(session, profile1, "test1-6", "t16", PublishStatus.Published, ExerciseType.Klatka, MechanicsType.Compound, ExerciseForceType.Push, ExerciseDifficult.One);
            exercises.Add(workoutPlan.Shortcut, workoutPlan);
            workoutPlan = CreateExercise(session, profile1, "test1-7", "t17", PublishStatus.Private, ExerciseType.Triceps, MechanicsType.NotSet, ExerciseForceType.Pull, ExerciseDifficult.Three);
            exercises.Add(workoutPlan.Shortcut, workoutPlan);
            workoutPlan = CreateExercise(session, profile1, "test1-8", "t18", PublishStatus.Private, ExerciseType.Triceps, MechanicsType.Isolation, ExerciseForceType.NotSet, ExerciseDifficult.Two);
            exercises.Add(workoutPlan.Shortcut, workoutPlan);
            workoutPlan = CreateExercise(session, profile1, "test1-9", "t19", PublishStatus.PendingPublish, ExerciseType.Klatka, MechanicsType.Isolation, ExerciseForceType.Pull, ExerciseDifficult.Two);
            exercises.Add(workoutPlan.Shortcut, workoutPlan);

            //now for profile 2
            workoutPlan = CreateExercise(session, profile2, "test2-1", "t21", PublishStatus.Private, ExerciseType.Biceps, MechanicsType.Compound, ExerciseForceType.Push, ExerciseDifficult.Two);
            exercises.Add(workoutPlan.Shortcut, workoutPlan);
            workoutPlan = CreateExercise(session, profile2, "test2-2", "t22", PublishStatus.Published, ExerciseType.Triceps, MechanicsType.NotSet, ExerciseForceType.Pull, ExerciseDifficult.Two);
            exercises.Add(workoutPlan.Shortcut, workoutPlan);
            workoutPlan = CreateExercise(session, profile2, "test2-3", "t23", PublishStatus.Published, ExerciseType.Triceps, MechanicsType.NotSet, ExerciseForceType.Push, ExerciseDifficult.One);
            exercises.Add(workoutPlan.Shortcut, workoutPlan);
            workoutPlan = CreateExercise(session, profile2, "test2-4", "t24", PublishStatus.Private, ExerciseType.Klatka, MechanicsType.Isolation, ExerciseForceType.Pull, ExerciseDifficult.Two);
            exercises.Add(workoutPlan.Shortcut, workoutPlan);
            workoutPlan = CreateExercise(session, profile2, "test2-5", "t25", PublishStatus.Published, ExerciseType.Biceps, MechanicsType.Isolation, ExerciseForceType.Pull, ExerciseDifficult.One);
            exercises.Add(workoutPlan.Shortcut, workoutPlan);
            workoutPlan = CreateExercise(session, profile2, "test2-6", "t26", PublishStatus.Published, ExerciseType.Klatka, MechanicsType.Compound, ExerciseForceType.Push, ExerciseDifficult.One);
            exercises.Add(workoutPlan.Shortcut, workoutPlan);
            workoutPlan = CreateExercise(session, profile2, "test2-7", "t27", PublishStatus.Private, ExerciseType.Triceps, MechanicsType.Compound, ExerciseForceType.Pull, ExerciseDifficult.Three);
            exercises.Add(workoutPlan.Shortcut, workoutPlan);
            workoutPlan = CreateExercise(session, profile2, "test2-8", "t28", PublishStatus.PendingPublish, ExerciseType.Klatka, MechanicsType.Isolation, ExerciseForceType.Pull, ExerciseDifficult.One);
            exercises.Add(workoutPlan.Shortcut, workoutPlan);
        }
        
        [Test]
        public void TestGetExercises_UserName()
        {
            var criteria = ExerciseSearchCriteria.CreateAllCriteria();
            criteria.UserId = profiles[1].Id;
 
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<ExerciseDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetExercises(data.Token, criteria, pageInfo);
            });

            assertExercisesListPack(result, 5, new[] { "t22", "t23", "t25", "t26", "t28" }, null);

            profile = (ProfileDTO)profiles[1].Tag;
            data = SecurityManager.CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetExercises(data.Token, criteria, pageInfo);
            });

            assertExercisesListPack(result, 8, new[] { "t21", "t22", "t23", "t24", "t25", "t26", "t27", "t28" }, null);
        }

        [Test]
        public void TestGetExercises_SortOrder_Rating()
        {
            RatingUserValue rating = new RatingUserValue();
            rating.RatedObjectId = exercises["t05"].GlobalId;
            rating.ProfileId = profiles[0].Id;
            rating.Rating = 5;
            insertToDatabase(rating);

            rating = new RatingUserValue();
            rating.RatedObjectId = exercises["t02"].GlobalId;
            rating.ProfileId = profiles[1].Id;
            rating.Rating = 5;
            insertToDatabase(rating);

            rating = new RatingUserValue();
            rating.RatedObjectId = exercises["t04"].GlobalId;
            rating.ProfileId = profiles[1].Id;
            rating.Rating = 3;
            insertToDatabase(rating);

            rating = new RatingUserValue();
            rating.RatedObjectId = exercises["t01"].GlobalId;
            rating.ProfileId = profiles[0].Id;
            rating.Rating = 1;
            insertToDatabase(rating);

            var criteria = ExerciseSearchCriteria.CreateAllCriteria();
            criteria.SortOrder = WorkoutPlanSearchOrder.HighestRating;

            var profile = (ProfileDTO) profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;
            PagedResult<ExerciseDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
                                 {
                                     result = Service.GetExercises(data.Token, criteria, pageInfo);
                                 });
            Assert.AreEqual("t05", result.Items[0].Shortcut);
            Assert.AreEqual("t02", result.Items[1].Shortcut);
            Assert.AreEqual("t04", result.Items[2].Shortcut);
            Assert.AreEqual("t01", result.Items[3].Shortcut);
        }


        [Test]
        public void TestGetExercises_PendingPublishOnly()
        {
            var criteria = new ExerciseSearchCriteria();
            criteria.SearchGroups.Add(ExerciseSearchCriteriaGroup.PendingPublish);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;
            PagedResult<ExerciseDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetExercises(data.Token, criteria, pageInfo);
            });
            assertExercisesListPack(result, 2, new[] { "t19","t28"}, null);

            profile = (ProfileDTO)profiles[1].Tag;
            data = SecurityManager.CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetExercises(data.Token, criteria, pageInfo);
            });
            assertExercisesListPack(result, 2, new[] { "t19", "t28" }, null);
        }

        [Test]
        public void TestGetExercises_AllAvailableForUser()
        {
            var criteria = ExerciseSearchCriteria.CreateAllCriteria();

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;
            PagedResult<ExerciseDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetExercises(data.Token, criteria, pageInfo);
            });
            assertExercisesListPack(result, 22, new[] { "t01", "t02", "t03", "t04", "t05", "t06", "t07", "t08" ,
                                "t11","t12","t13","t14","t15","t16","t17","t18","t19",
                                "t22","t23","t25","t26"}, null);

            profile = (ProfileDTO)profiles[1].Tag;
            data = SecurityManager.CreateNewSession(profile, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetExercises(data.Token, criteria, pageInfo);
            });
            assertExercisesListPack(result, 21, new[] { "t01", "t02", "t03", "t04", "t05", "t06", "t07", "t08",
            "t21","t22","t23","t24","t25","t26","t27","t28",
            "t12","t13","t15","t16","t19"}, null);
        }

        [Test]
        public void TestGetExercises_OnlyBicepsAvailableForUser()
        {
            var criteria = ExerciseSearchCriteria.CreateAllCriteria();
            criteria.ExerciseTypes.Add(Service.Model.ExerciseType.Biceps);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;
            PagedResult<ExerciseDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetExercises(data.Token, criteria, pageInfo);
            });
            assertExercisesListPack(result, 7, new[] {"t02", "t04", "t05", "t12","t14","t15","t25"}, null);

            profile = (ProfileDTO)profiles[1].Tag;
            data = SecurityManager.CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetExercises(data.Token, criteria, pageInfo);
            });
            assertExercisesListPack(result, 7, new[] { "t02", "t04", "t05", "t21","t25","t12","t15"}, null);
        }

        [Test]
        public void TestGetExercises_EmptyCriteria()
        {
            var criteria = new ExerciseSearchCriteria();
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;
            PagedResult<ExerciseDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetExercises(data.Token, criteria, pageInfo);
            });
            assertExercisesListPack(result, 0, null, null);

            profile = (ProfileDTO)profiles[1].Tag;
            data = SecurityManager.CreateNewSession(profile, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetExercises(data.Token, criteria, pageInfo);
            });
            assertExercisesListPack(result, 0, null, null);
        }

        //[Test]
        //public void TestGetExercises_OnlyOther()
        //{
        //    var criteria = ExerciseSearchCriteria.CreateAllCriteria();
        //    criteria.SearchGroups.Remove(ExerciseSearchCriteriaGroup.Favorites);
        //    criteria.SearchGroups.Remove(ExerciseSearchCriteriaGroup.Global);
        //    criteria.SearchGroups.Remove(ExerciseSearchCriteriaGroup.Mine);
        //    var profile = (ProfileDTO)profiles[0].Tag;
        //    SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
        //    PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
        //    pageInfo.PageSize = 50;
        //    PagedResult<ExerciseDTO> result = null;
            
        //    RunServiceMethod(delegate(InternalBodyArchitectService Service)
        //    {
        //        result = Service.GetExercises(data.Token, criteria, pageInfo);
        //    });
        //    assertExercisesListPack(result, 3, new[] {   "t23", "t25", "t26",  }, null);

        //    profile = (ProfileDTO)profiles[1].Tag;
        //    data = SecurityManager.CreateNewSession(profile, ClientInformation);
            
        //    RunServiceMethod(delegate(InternalBodyArchitectService Service)
        //    {
        //        result = Service.GetExercises(data.Token, criteria, pageInfo);
        //    });
        //    assertExercisesListPack(result, 2, new[] {   "t13",  "t15" }, null);
        //}

        [Test]
        public void TestGetExercises_OnlyGlobalAndMine()
        {
            var criteria = ExerciseSearchCriteria.CreatePersonalCriteria();
            
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;
            PagedResult<ExerciseDTO> result = null;

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetExercises(data.Token, criteria, pageInfo);
            });
            assertExercisesListPack(result, 21, new[] { "t01", "t02", "t03", "t04", "t05", "t06", "t07", "t08" ,
                                "t11","t12","t13","t14","t15","t16","t17","t18","t19","t22","t23","t25","t26"}, null);

            profile = (ProfileDTO)profiles[1].Tag;
            data = SecurityManager.CreateNewSession(profile, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetExercises(data.Token, criteria, pageInfo);
            });
            assertExercisesListPack(result, 20, new[] { "t01", "t02", "t03", "t04", "t05", "t06", "t07", "t08",
            "t21","t22","t23","t24","t25","t26","t27","t28","t12","t13","t15","t16"}, null);
        }

        void assertExercisesListPack(PagedResult<ExerciseDTO> pack, int allItems, string[] availableExercisesShortcuts, string[] notAvailablePlans)
        {
            Assert.AreEqual(allItems, pack.AllItemsCount);
            if (availableExercisesShortcuts != null)
            {
                foreach (string availablePlanName in availableExercisesShortcuts)
                {
                    Assert.IsNotNull(findExercise(availablePlanName, pack.Items),
                                     "Cannot find plan: " + availablePlanName);
                }
            }
            if (notAvailablePlans != null)
            {
                foreach (string notAvailablePlan in notAvailablePlans)
                {
                    Assert.IsNull(findExercise(notAvailablePlan, pack.Items),
                                  "Plan " + notAvailablePlan + " shouldn't be in a results");
                }
            }
        }

        ExerciseDTO findExercise(string shortcut, IList<ExerciseDTO> plans)
        {
            return (from plan in plans where plan.Shortcut == shortcut select plan).SingleOrDefault();
        }
    }
}
