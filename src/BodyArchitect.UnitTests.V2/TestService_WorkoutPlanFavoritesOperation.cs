using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NUnit.Framework;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_WorkoutPlanFavoritesOperation : TestServiceBase
    {
        private List<Profile> profiles = new List<Profile>();

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                profiles.Add(CreateProfile(Session, "test1"));
                profiles.Add(CreateProfile(Session, "test2"));
                tx.Commit();
            }
        }


        [Test]
        public void AddToFavorites_DataInfo_Refresh()
        {
            var plan=CreatePlan(Session, profiles[0], "plan1");
            ProfileDTO profile1 = (ProfileDTO) profiles[1].Tag;
            var oldHash = profiles[1].DataInfo.WorkoutPlanHash;
            SessionData data = CreateNewSession(profile1, ClientInformation, new LoginData());
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
                                 {
                                     var param = new WorkoutPlanOperationParam();
                                     param.WorkoutPlanId = plan.GlobalId;
                                     param.Operation = SupplementsCycleDefinitionOperation.AddToFavorites;
                                     Service.WorkoutPlanOperation(data.Token, param);
                                 });
            var dbProfile = Session.Get<Profile>(profile1.GlobalId);
            Assert.AreNotEqual(dbProfile.DataInfo.WorkoutPlanHash, oldHash);
        }

        [Test]
        public void RemoveFromFavorites_DataInfo_Refresh()
        {
            var plan = CreatePlan(Session, profiles[0], "plan1");
            //now add plan to favorites
            profiles[1].FavoriteWorkoutPlans.Add(plan);
            insertToDatabase(profiles[1]);
            var oldHash = profiles[0].DataInfo.WorkoutPlanHash;
            ProfileDTO profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation, new LoginData());
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new WorkoutPlanOperationParam();
                param.WorkoutPlanId = plan.GlobalId;
                param.Operation = SupplementsCycleDefinitionOperation.RemoveFromFavorites;
                Service.WorkoutPlanOperation(data.Token, param);
            });
            var dbProfile = Session.Get<Profile>(profile1.GlobalId);
            Assert.AreNotEqual(dbProfile.DataInfo.WorkoutPlanHash, oldHash);
        }

        [Test]
        public void AddToFavorites_DataInfo_Refresh_ForPlanOwner()
        {
            var plan = CreatePlan(Session, profiles[0], "plan1");
            var oldHash = profiles[0].DataInfo.WorkoutPlanHash;
            ProfileDTO profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation, new LoginData());
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new WorkoutPlanOperationParam();
                param.WorkoutPlanId = plan.GlobalId;
                param.Operation = SupplementsCycleDefinitionOperation.AddToFavorites;
                Service.WorkoutPlanOperation(data.Token, param);
            });
            var dbProfile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.AreEqual(oldHash,dbProfile.DataInfo.WorkoutPlanHash);
        }

        [Test]
        public void RemoveFromFavorites_DataInfo_Refresh_ForPlanOwner()
        {
            var plan = CreatePlan(Session, profiles[0], "plan1");
            //now add plan to favorites
            profiles[1].FavoriteWorkoutPlans.Add(plan);
            insertToDatabase(profiles[1]);

            var oldHash = profiles[0].DataInfo.WorkoutPlanHash;
            ProfileDTO profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation, new LoginData());
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new WorkoutPlanOperationParam();
                param.WorkoutPlanId = plan.GlobalId;
                param.Operation = SupplementsCycleDefinitionOperation.RemoveFromFavorites;
                Service.WorkoutPlanOperation(data.Token, param);
            });
            var dbProfile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.AreEqual(oldHash,dbProfile.DataInfo.WorkoutPlanHash);
        }

        [Test]
        public void AddToFavorites()
        {
            var plan = CreatePlan(Session, profiles[1], "plan1");
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new WorkoutPlanOperationParam();
                param.WorkoutPlanId = plan.GlobalId;
                param.Operation = SupplementsCycleDefinitionOperation.AddToFavorites;
                service.WorkoutPlanOperation(data.Token, param);
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(1, dbProfile.FavoriteWorkoutPlans.Count);
            Assert.AreEqual(plan.GlobalId, dbProfile.FavoriteWorkoutPlans.ElementAt(0).GlobalId);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddToFavorites_PlanFromCurrentProfile()
        {
            var plan = CreatePlan(Session, profiles[0], "plan1");
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new WorkoutPlanOperationParam();
                param.WorkoutPlanId = plan.GlobalId;
                param.Operation = SupplementsCycleDefinitionOperation.AddToFavorites;
                service.WorkoutPlanOperation(data.Token, param);
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(1, dbProfile.FavoriteWorkoutPlans.Count);
            Assert.AreEqual(plan.GlobalId, dbProfile.FavoriteWorkoutPlans.ElementAt(0).GlobalId);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddToFavorites_PrivatePlan()
        {
            var plan = CreatePlan(Session, profiles[0], "plan1",isPublished:false);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new WorkoutPlanOperationParam();
                param.WorkoutPlanId = plan.GlobalId;
                param.Operation = SupplementsCycleDefinitionOperation.AddToFavorites;
                service.WorkoutPlanOperation(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(ObjectIsFavoriteException))]
        public void AddToFavorites_TwiceTheSameExercise()
        {
            var plan = CreatePlan(Session, profiles[1], "plan1");
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new WorkoutPlanOperationParam();
                param.WorkoutPlanId = plan.GlobalId;
                param.Operation = SupplementsCycleDefinitionOperation.AddToFavorites;
                service.WorkoutPlanOperation(data.Token, param);
            });
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new WorkoutPlanOperationParam();
                param.WorkoutPlanId = plan.GlobalId;
                param.Operation = SupplementsCycleDefinitionOperation.AddToFavorites;
                service.WorkoutPlanOperation(data.Token, param);
            });
        }

       
        [Test]
        public void RemoveFromFavorites()
        {
            var plan = CreatePlan(Session, profiles[1], "plan1");
            profiles[0].FavoriteWorkoutPlans.Add(plan);
            insertToDatabase(profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new WorkoutPlanOperationParam();
                param.WorkoutPlanId = plan.GlobalId;
                param.Operation = SupplementsCycleDefinitionOperation.RemoveFromFavorites;
                service.WorkoutPlanOperation(data.Token, param);
            });

            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(0, dbProfile.FavoriteWorkoutPlans.Count);
        }

        [Test]
        [ExpectedException(typeof(ObjectIsNotFavoriteException))]
        public void RemoveFromFavorites_Twice()
        {
            var plan = CreatePlan(Session, profiles[1], "plan1");
            profiles[0].FavoriteWorkoutPlans.Add(plan);
            insertToDatabase(profiles[0]);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new WorkoutPlanOperationParam();
                param.WorkoutPlanId = plan.GlobalId;
                param.Operation = SupplementsCycleDefinitionOperation.RemoveFromFavorites;
                service.WorkoutPlanOperation(data.Token, param);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new WorkoutPlanOperationParam();
                param.WorkoutPlanId = plan.GlobalId;
                param.Operation = SupplementsCycleDefinitionOperation.RemoveFromFavorites;
                service.WorkoutPlanOperation(data.Token, param);
            });
        }

    }
}
