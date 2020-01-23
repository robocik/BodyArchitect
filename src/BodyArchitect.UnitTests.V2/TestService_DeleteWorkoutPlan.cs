using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using BodyArchitect.Shared;
using NUnit.Framework;
using TrainingPlanDay = BodyArchitect.Service.V2.Model.TrainingPlans.TrainingPlanDay;
using TrainingPlanEntry = BodyArchitect.Service.V2.Model.TrainingPlans.TrainingPlanEntry;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_DeleteWorkoutPlan : TestServiceBase
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

                exercise = CreateExercise(Session, null, "ex", "ex");
                tx.Commit();
            }
        }

        Service.V2.Model.TrainingPlans.TrainingPlan createWorkoutPlanObject(ProfileDTO profile)
        {
            var contentPlan = new Service.V2.Model.TrainingPlans.TrainingPlan();
            contentPlan.Author = "rtyt";
            contentPlan.CreationDate = new DateTime(2011, 6, 28, 19, 18, 26);
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
        public void DeletePlan()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            SessionData data = CreateNewSession(profile, ClientInformation);

            var plan = createWorkoutPlanObject(profile);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                plan = Service.SaveWorkoutPlan(data.Token, plan);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
                {
                    WorkoutPlanOperationParam param = new WorkoutPlanOperationParam();
                    param.WorkoutPlanId = plan.GlobalId;
                    param.Operation = SupplementsCycleDefinitionOperation.Delete;
                    Service.WorkoutPlanOperation(data.Token, param);
            });
            var count = Session.QueryOver<BodyArchitect.Model.TrainingPlan>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void SecurityBug()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;

            SessionData data = CreateNewSession(profile, ClientInformation);

            var plan = createWorkoutPlanObject(profile);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                plan = Service.SaveWorkoutPlan(data.Token, plan);
            });
            data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
                                 {
                                     WorkoutPlanOperationParam param = new WorkoutPlanOperationParam();
                                     param.WorkoutPlanId = plan.GlobalId;
                                     param.Operation = SupplementsCycleDefinitionOperation.Delete;
                                     Service.WorkoutPlanOperation(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void DeleteForOtherUser()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;

            SessionData data = CreateNewSession(profile, ClientInformation);

            var plan = createWorkoutPlanObject(profile);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                plan = Service.SaveWorkoutPlan(data.Token, plan);
            });
            data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                WorkoutPlanOperationParam param = new WorkoutPlanOperationParam();
                param.WorkoutPlanId = plan.GlobalId;
                param.Operation = SupplementsCycleDefinitionOperation.Delete;
                Service.WorkoutPlanOperation(data.Token, param);
            });
        }
    }
}
