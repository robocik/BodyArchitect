using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NHibernate;
using NUnit.Framework;
using ExerciseDifficult = BodyArchitect.Model.ExerciseDifficult;
using ExerciseForceType = BodyArchitect.Model.ExerciseForceType;
using ExerciseType = BodyArchitect.Model.ExerciseType;
using MechanicsType = BodyArchitect.Model.MechanicsType;

namespace BodyArchitect.UnitTests.V2
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
            var workoutPlan = CreateExercise(session, null, "test0-1", "t01", ExerciseType.Klatka, MechanicsType.Isolation, ExerciseForceType.Pull, ExerciseDifficult.One);
            workoutPlan.Rating = 2;
            exercises.Add(workoutPlan.Shortcut, workoutPlan);
            workoutPlan = CreateExercise(session, null, "test0-2", "t02",  ExerciseType.Biceps, MechanicsType.Compound, ExerciseForceType.Push, ExerciseDifficult.Two);
            workoutPlan.Rating = 4.3f;
            exercises.Add(workoutPlan.Shortcut, workoutPlan);
            workoutPlan = CreateExercise(session, null, "test0-3", "t03", ExerciseType.Triceps, MechanicsType.Isolation, ExerciseForceType.Push, ExerciseDifficult.Three);
            workoutPlan.Rating = 1;
            exercises.Add(workoutPlan.Shortcut, workoutPlan);



            //creates exercises for profile 1
            workoutPlan = CreateExercise(session, profile1, "test1-1", "t11",ExerciseType.Klatka, MechanicsType.Isolation, ExerciseForceType.Pull, ExerciseDifficult.One);
            exercises.Add(workoutPlan.Shortcut, workoutPlan);
            workoutPlan = CreateExercise(session, profile1, "test1-2", "t12",  ExerciseType.Biceps, MechanicsType.Compound, ExerciseForceType.Push, ExerciseDifficult.Two);
            exercises.Add(workoutPlan.Shortcut, workoutPlan);

            //now for profile 2
            workoutPlan = CreateExercise(session, profile2, "test2-1", "t21",  ExerciseType.Biceps, MechanicsType.Compound, ExerciseForceType.Push, ExerciseDifficult.Two);
            exercises.Add(workoutPlan.Shortcut, workoutPlan);
            workoutPlan = CreateExercise(session, profile2, "test2-2", "t22",  ExerciseType.Triceps, MechanicsType.NotSet, ExerciseForceType.Pull, ExerciseDifficult.Two);
            exercises.Add(workoutPlan.Shortcut, workoutPlan);

            //set favorites
            profile1.FavoriteExercises.Add(exercises["t22"]);
            session.Update(profile1);
            profile2.FavoriteExercises.Add(exercises["t12"]);
            session.Update(profile2);
            
        }

        [Test]
        public void WithoutIsDeleted()
        {
            var deletedExercise=CreateExercise(Session,null,"test","tt");
            deletedExercise.IsDeleted = true;
            insertToDatabase(deletedExercise);
            var criteria = ExerciseSearchCriteria.CreateAllCriteria();
            criteria.ExerciseTypes.Add(Service.V2.Model.ExerciseType.Klatka);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<ExerciseDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetExercises(data.Token, criteria, pageInfo);
            });

            var count=result.Items.Where(x => x.GlobalId == deletedExercise.GlobalId).Count();
            Assert.AreEqual(0,count);
        }

        [Test]
        public void OnlyChest()
        {
            var criteria = ExerciseSearchCriteria.CreateAllCriteria();
            criteria.ExerciseTypes.Add(Service.V2.Model.ExerciseType.Klatka);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<ExerciseDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetExercises(data.Token, criteria, pageInfo);
            });

            assertExercisesListPack(result, new[] { "t01",  "t11" });
        }

        [Test]
        public void ForUser()
        {
            var criteria = ExerciseSearchCriteria.CreateAllCriteria();
            criteria.UserId = profiles[1].GlobalId;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<ExerciseDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetExercises(data.Token, criteria, pageInfo);
            });

            assertExercisesListPack(result, new[] { "t21", "t22" });
        }

        [Test]
        public void MineAndFavorites_ForCurrentUser()
        {
            var criteria = ExerciseSearchCriteria.CreateAllCriteria();
            criteria.SearchGroups.Add(ExerciseSearchCriteriaGroup.Mine);
            criteria.SearchGroups.Add(ExerciseSearchCriteriaGroup.Favorites);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<ExerciseDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetExercises(data.Token, criteria, pageInfo);
            });

            assertExercisesListPack(result, new[] { "t11","t12", "t22" });
        }

        [Test]
        public void MineAndFavorites_ForOtherUser()
        {
            var criteria = ExerciseSearchCriteria.CreateAllCriteria();
            criteria.SearchGroups.Add(ExerciseSearchCriteriaGroup.Mine);
            criteria.SearchGroups.Add(ExerciseSearchCriteriaGroup.Favorites);
            criteria.UserId = profiles[1].GlobalId;
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<ExerciseDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetExercises(data.Token, criteria, pageInfo);
            });

            assertExercisesListPack(result, new[] {"t21","t22", "t12" });
        }

        [Test]
        public void All()
        {
            var criteria = ExerciseSearchCriteria.CreateAllCriteria();

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<ExerciseDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetExercises(data.Token, criteria, pageInfo);
            });

            assertExercisesListPack(result,  new[] {"t01","t02","t03","t11","t12", "t21", "t22"});
        }

        [Test]
        public void ByName()
        {
            var criteria = ExerciseSearchCriteria.CreateAllCriteria();
            criteria.Name = "test2-";
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<ExerciseDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetExercises(data.Token, criteria, pageInfo);
            });

            assertExercisesListPack(result, new[] {  "t21", "t22" });
        }

        [Test]
        public void OnlyGlobals()
        {
            var criteria = ExerciseSearchCriteria.CreateAllCriteria();
            criteria.SearchGroups.Add(ExerciseSearchCriteriaGroup.Global);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<ExerciseDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetExercises(data.Token, criteria, pageInfo);
            });

            assertExercisesListPack(result, new[] { "t01", "t02", "t03" });
        }

        [Test]
        public void OnlyMine()
        {
            var criteria = ExerciseSearchCriteria.CreateAllCriteria();
            criteria.SearchGroups.Add(ExerciseSearchCriteriaGroup.Mine);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<ExerciseDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetExercises(data.Token, criteria, pageInfo);
            });

            assertExercisesListPack(result, new[] { "t11", "t12" });
        }

        [Test]
        public void OnlyOthers()
        {
            var criteria = ExerciseSearchCriteria.CreateAllCriteria();
            criteria.SearchGroups.Add(ExerciseSearchCriteriaGroup.Other);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<ExerciseDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetExercises(data.Token, criteria, pageInfo);
            });

            assertExercisesListPack(result, new[] { "t21", "t22" });
        }

        [Test]
        public void OnlyMineAndOthers()
        {
            var criteria = ExerciseSearchCriteria.CreateAllCriteria();
            criteria.SearchGroups.Add(ExerciseSearchCriteriaGroup.Other);
            criteria.SearchGroups.Add(ExerciseSearchCriteriaGroup.Mine);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<ExerciseDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetExercises(data.Token, criteria, pageInfo);
            });

            assertExercisesListPack(result, new[] {"t11", "t12", "t21", "t22" });
        }

        /*
        [Test]
        public void TestGetExercises_UserName()
        {
            var criteria = ExerciseSearchCriteria.CreateAllCriteria();
            criteria.UserId = profiles[1].Id;
 
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<ExerciseDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetExercises(data.Token, criteria, pageInfo);
            });

            assertExercisesListPack(result, 5, new[] { "t22", "t23", "t25", "t26", "t28" }, null);

            profile = (ProfileDTO)profiles[1].Tag;
            data = CreateNewSession(profile, ClientInformation);
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
            rating.ProfileId = profiles[0].GlobalId;
            rating.Rating = 5;
            insertToDatabase(rating);

            rating = new RatingUserValue();
            rating.RatedObjectId = exercises["t02"].GlobalId;
            rating.ProfileId = profiles[1].Id;
            rating.Rating = 4;
            insertToDatabase(rating);

            rating = new RatingUserValue();
            rating.RatedObjectId = exercises["t04"].GlobalId;
            rating.ProfileId = profiles[1].Id;
            rating.Rating = 3;
            insertToDatabase(rating);

            rating = new RatingUserValue();
            rating.RatedObjectId = exercises["t01"].GlobalId;
            rating.ProfileId = profiles[0].GlobalId;
            rating.Rating = 1;
            insertToDatabase(rating);

            var criteria = ExerciseSearchCriteria.CreateAllCriteria();
            criteria.SortOrder = SearchSortOrder.HighestRating;

            var profile = (ProfileDTO) profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
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
        public void TestGetExercises_AllAvailableForUser()
        {
            var criteria = ExerciseSearchCriteria.CreateAllCriteria();

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
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
            data = CreateNewSession(profile, ClientInformation);
            
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
            criteria.ExerciseTypes.Add(Service.V2.Model.ExerciseType.Biceps);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;
            PagedResult<ExerciseDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetExercises(data.Token, criteria, pageInfo);
            });
            assertExercisesListPack(result, 7, new[] {"t02", "t04", "t05", "t12","t14","t15","t25"}, null);

            profile = (ProfileDTO)profiles[1].Tag;
            data = CreateNewSession(profile, ClientInformation);

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
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;
            PagedResult<ExerciseDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetExercises(data.Token, criteria, pageInfo);
            });
            assertExercisesListPack(result, 0, null, null);

            profile = (ProfileDTO)profiles[1].Tag;
            data = CreateNewSession(profile, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetExercises(data.Token, criteria, pageInfo);
            });
            assertExercisesListPack(result, 0, null, null);
        }

        [Test]
        public void TestGetExercises_OnlyGlobalAndMine()
        {
            var criteria = ExerciseSearchCriteria.CreatePersonalCriteria();
            
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
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
            data = CreateNewSession(profile, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetExercises(data.Token, criteria, pageInfo);
            });
            assertExercisesListPack(result, 20, new[] { "t01", "t02", "t03", "t04", "t05", "t06", "t07", "t08",
            "t21","t22","t23","t24","t25","t26","t27","t28","t12","t13","t15","t16"}, null);
        }
        */
        void assertExercisesListPack(PagedResult<ExerciseDTO> pack,  string[] availableExercisesShortcuts)
        {
            Assert.AreEqual(pack.Items.Count, pack.AllItemsCount);
            Assert.AreEqual(pack.Items.Count, pack.Items.Count);
            if (availableExercisesShortcuts != null)
            {
                foreach (string availablePlanName in availableExercisesShortcuts)
                {
                    Assert.IsNotNull(findExercise(availablePlanName, pack.Items),
                                     "Cannot find plan: " + availablePlanName);
                }
            }
        }

        ExerciseDTO findExercise(string shortcut, IList<ExerciseDTO> plans)
        {
            return (from plan in plans where plan.Shortcut == shortcut select plan).SingleOrDefault();
        }
    }
}
