using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using BodyArchitect.Shared;
using NUnit.Framework;
using PublishStatus = BodyArchitect.Service.V2.Model.PublishStatus;
using TrainingPlan = BodyArchitect.Model.TrainingPlan;
using TrainingPlanDay = BodyArchitect.Service.V2.Model.TrainingPlans.TrainingPlanDay;
using TrainingPlanEntry = BodyArchitect.Service.V2.Model.TrainingPlans.TrainingPlanEntry;
using TrainingType = BodyArchitect.Service.V2.Model.TrainingPlans.TrainingType;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_SaveWorkoutPlan : TestServiceBase
    {
        private List<Profile> profiles = new List<Profile>();
        private Exercise exercise;

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                profiles.Add(CreateProfile(Session, "test1"));
                profiles.Add(CreateProfile(Session, "test2"));

                exercise = CreateExercise(Session, null, "fg", "gf");
                tx.Commit();
            }
        }

        Service.V2.Model.TrainingPlans.TrainingPlan createWorkoutPlanObject(ProfileDTO profile)
        {
            var contentPlan = new Service.V2.Model.TrainingPlans.TrainingPlan();
            contentPlan.Author = "rtyt";
            //contentPlan.CreationDate = DateTime.Parse("06/28/2011 19:18:26");
            contentPlan.Name = "rtyre";
            contentPlan.Profile = profile;
            contentPlan.Language = "en";
            TrainingPlanDay day = new TrainingPlanDay();
            day.Name = "Day 1";
            contentPlan.AddDay(day);
            TrainingPlanEntry entry = new TrainingPlanEntry();
            entry.Exercise = exercise.Map<ExerciseLightDTO>();
            day.AddEntry(entry);
            return contentPlan;
        }

        [Test]
        public void SimpleSaveNewPlan()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            
            SessionData data = CreateNewSession(profile, ClientInformation);

            var plan = createWorkoutPlanObject(profile);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var res=Service.SaveWorkoutPlan(data.Token, plan);
                Assert.IsNotNull(res);
                UnitTestHelper.CompareObjects(plan, res,true);
            });
        }

        [Test]
        public void SimpleSaveNewPlan_CreatedDate()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            SessionData data = CreateNewSession(profile, ClientInformation);

            var plan = createWorkoutPlanObject(profile);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var res = Service.SaveWorkoutPlan(data.Token, plan);
                Assert.AreNotEqual(DateTime.MinValue, res.CreationDate);
                var dbPlan=Session.Get<TrainingPlan>(res.GlobalId);
                Assert.AreNotEqual(DateTime.MinValue, dbPlan.CreationDate);
            });
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SimpleSaveNewPlan_PublishedStatus()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            SessionData data = CreateNewSession(profile, ClientInformation);

            var plan = createWorkoutPlanObject(profile);
            plan.Status = PublishStatus.Published;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveWorkoutPlan(data.Token, plan);
            });
        }

        [Test]
        public void UpdatePlan_PreventUpdateProfile()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            SessionData data = CreateNewSession(profile, ClientInformation);

            var plan = createWorkoutPlanObject(profile);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                plan = Service.SaveWorkoutPlan(data.Token, plan);
            });
            plan.Profile.UserName = "NewName";
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveWorkoutPlan(data.Token, plan);
            });
            var dbProfile=Session.Get<Profile>(plan.Profile.GlobalId);
            Assert.AreEqual(profile.UserName,dbProfile.UserName);
        }

        [Test]
        public void UpdatePlan()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            SessionData data = CreateNewSession(profile, ClientInformation);

            var plan = createWorkoutPlanObject(profile);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                plan = Service.SaveWorkoutPlan(data.Token, plan);
            });
            plan.Comment = "fgdfgd";
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var res = Service.SaveWorkoutPlan(data.Token, plan);
                Assert.IsNotNull(res);
                plan.Version = res.Version;
                UnitTestHelper.CompareObjects(plan, res);
            });
        }

        [Test]
        [ExpectedException(typeof(PublishedObjectOperationException))]
        public void UpdatePlan_StatusPublished()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            SessionData data = CreateNewSession(profile, ClientInformation);

            var plan = createWorkoutPlanObject(profile);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                plan = Service.SaveWorkoutPlan(data.Token, plan);
            });
            var db=Session.Get<TrainingPlan>(plan.GlobalId);
            db.Status = Model.PublishStatus.Published;
            insertToDatabase(db);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var res = Service.SaveWorkoutPlan(data.Token, plan);
            });
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UpdatePlan_ChangeStatus()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            SessionData data = CreateNewSession(profile, ClientInformation);

            var plan = createWorkoutPlanObject(profile);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                plan = Service.SaveWorkoutPlan(data.Token, plan);
            });
            plan.Status = PublishStatus.Published;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var res = Service.SaveWorkoutPlan(data.Token, plan);
                Assert.IsNotNull(res);
                UnitTestHelper.CompareObjects(plan, res);
            });
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void UpdatePlan_AnotherUser_SecurityBug()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;

            SessionData data = CreateNewSession(profile1, ClientInformation);

            var plan = createWorkoutPlanObject(profile1);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                plan = Service.SaveWorkoutPlan(data.Token, plan);
            });
            data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
                                 {
                                     plan.Profile = profile;
                Service.SaveWorkoutPlan(data.Token, plan);
            });
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void UpdatePlan_AnotherUser()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;

            SessionData data = CreateNewSession(profile1, ClientInformation);

            var plan = createWorkoutPlanObject(profile1);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                plan = Service.SaveWorkoutPlan(data.Token, plan);
            });
            data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveWorkoutPlan(data.Token, plan);
            });
        }

        [Test]
        public void SavePlan_RatingShouldNotChange()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            SessionData data = CreateNewSession(profile, ClientInformation);

            var plan = createWorkoutPlanObject(profile);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                plan = Service.SaveWorkoutPlan(data.Token, plan);
            });
            var dbPlan=Session.Get<TrainingPlan>(plan.GlobalId);
            dbPlan.Rating = 4;
            insertToDatabase(dbPlan);

            plan.Version = dbPlan.Version;
            plan.Rating = 3;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveWorkoutPlan(data.Token, plan);
            });
            dbPlan = Session.Get<TrainingPlan>(plan.GlobalId);
            Assert.AreEqual(4, dbPlan.Rating);
        }

        [Test]
        public void SavePlan_UserRatingInResult()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            SessionData data = CreateNewSession(profile, ClientInformation);

            var plan = createWorkoutPlanObject(profile);
            plan.Rating = 2;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                plan= Service.SaveWorkoutPlan(data.Token, plan);
            });
            RatingUserValue rating = new RatingUserValue();
            rating.RatedObjectId = plan.GlobalId;
            rating.Rating = 2;
            rating.ProfileId = profile.GlobalId;
            rating.VotedDate = DateTime.UtcNow;
            insertToDatabase(rating);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                plan = Service.SaveWorkoutPlan(data.Token, plan);
            });
            Assert.AreEqual(rating.Rating,plan.UserRating);
            Assert.AreEqual(rating.ShortComment, plan.UserShortComment);

        }

        [Test]
        public void UpdatePlan_DataInfo_Refresh()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            SessionData data = CreateNewSession(profile, ClientInformation);

            var plan = createWorkoutPlanObject(profile);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                plan = Service.SaveWorkoutPlan(data.Token, plan);
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            var oldHash = dbProfile.DataInfo.WorkoutPlanHash;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                TimerService.UtcNow = DateTime.UtcNow.AddHours(1);
                plan = Service.SaveWorkoutPlan(data.Token, plan);
            });
            dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreNotEqual(oldHash, dbProfile.DataInfo.WorkoutPlanHash);
        }

        [Test]
        public void DeletePlan_DataInfo_Refresh()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            SessionData data = CreateNewSession(profile, ClientInformation);

            var plan = createWorkoutPlanObject(profile);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
                                 {
                                     plan = Service.SaveWorkoutPlan(data.Token, plan);
            });
            

            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            var oldHash = dbProfile.DataInfo.WorkoutPlanHash;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                TimerService.UtcNow = DateTime.UtcNow.AddHours(1);
                WorkoutPlanOperationParam param = new WorkoutPlanOperationParam();
                param.WorkoutPlanId = plan.GlobalId;
                param.Operation = SupplementsCycleDefinitionOperation.Delete;
                Service.WorkoutPlanOperation(data.Token, param);
            });
            dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreNotEqual(oldHash, dbProfile.DataInfo.WorkoutPlanHash);
        }
    }
}
