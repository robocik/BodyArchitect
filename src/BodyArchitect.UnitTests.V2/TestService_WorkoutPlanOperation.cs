using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Model;
using BodyArchitect.Portable.Exceptions;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using BodyArchitect.Shared;
using NUnit.Framework;
using ExerciseDifficult = BodyArchitect.Model.ExerciseDifficult;
using ExerciseForceType = BodyArchitect.Model.ExerciseForceType;
using MechanicsType = BodyArchitect.Model.MechanicsType;
using Profile = BodyArchitect.Model.Profile;
using PublishStatus = BodyArchitect.Model.PublishStatus;
using TrainingEnd = BodyArchitect.Service.V2.Model.TrainingEnd;
using TrainingPlan = BodyArchitect.Model.TrainingPlan;
using TrainingPlanDay = BodyArchitect.Service.V2.Model.TrainingPlans.TrainingPlanDay;
using TrainingPlanDifficult = BodyArchitect.Model.TrainingPlanDifficult;
using TrainingPlanEntry = BodyArchitect.Service.V2.Model.TrainingPlans.TrainingPlanEntry;
using TrainingType = BodyArchitect.Model.TrainingType;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_WorkoutPlanOperation : TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        Dictionary<string, TrainingPlan> workoutPlans = new Dictionary<string, TrainingPlan>();
        Dictionary<string, Exercise> exercises = new Dictionary<string, Exercise>();
        
        public override void BuildDatabase()
        {
            ObjectsConverter.Configure();
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                workoutPlans.Clear();
                exercises.Clear();
                TestService_GetExercises.BuildTwoProfilesDatabase(Session,profiles,exercises);

                profiles[0].Statistics.StrengthTrainingEntriesCount = 40;

                //remove all favorites items
                profiles[0].FavoriteWorkoutPlans.Clear();
                Session.Update(profiles[0]);
                profiles[1].FavoriteWorkoutPlans.Clear();
                Session.Update(profiles[1]);

                Profile profile1 = profiles[0];
                Profile profile2 = profiles[1];

                

                //creates workout plans for profile 1
                var workoutPlan = createPlan(profile1, "test1-1",PublishStatus.Published, TrainingPlanDifficult.Beginner, TrainingType.HST);
                workoutPlans.Add(workoutPlan.Name, workoutPlan);
                workoutPlan = createPlan(profile1, "test1-2",PublishStatus.Private, TrainingPlanDifficult.Advanced, TrainingType.HST,  exercises["t11"], exercises["t12"],  exercises["t01"]);
                workoutPlans.Add(workoutPlan.Name, workoutPlan);

                workoutPlan = createPlan(profile1, "test1-3",PublishStatus.Published, TrainingPlanDifficult.Beginner, TrainingType.PushPull);
                workoutPlans.Add(workoutPlan.Name, workoutPlan);
                workoutPlan = createPlan(profile1, "test1-4", PublishStatus.Private, TrainingPlanDifficult.NotSet, TrainingType.Split, exercises["t01"], exercises["t12"], exercises["t22"]);
                workoutPlans.Add(workoutPlan.Name, workoutPlan);
                workoutPlan = createPlan(profile1, "test1-5",PublishStatus.Private, TrainingPlanDifficult.Beginner, TrainingType.FBW);
                workoutPlans.Add(workoutPlan.Name, workoutPlan);
                workoutPlan = createPlan(profile1, "test1-6", PublishStatus.Published,TrainingPlanDifficult.Professional, TrainingType.Split);
                workoutPlans.Add(workoutPlan.Name, workoutPlan);
                workoutPlan = createPlan(profile1, "test1-7",PublishStatus.Published, TrainingPlanDifficult.Beginner, TrainingType.ACT);
                workoutPlans.Add(workoutPlan.Name, workoutPlan);

                //now for profile 2
                workoutPlan = createPlan(profile2, "test2-1",PublishStatus.Private, TrainingPlanDifficult.Advanced, TrainingType.HST);
                workoutPlans.Add(workoutPlan.Name, workoutPlan);
                workoutPlan = createPlan(profile2, "test2-2",PublishStatus.Published, TrainingPlanDifficult.Professional, TrainingType.ACT);
                workoutPlans.Add(workoutPlan.Name, workoutPlan);
                workoutPlan = createPlan(profile2, "test2-3",PublishStatus.Published, TrainingPlanDifficult.Beginner, TrainingType.PushPull);
                workoutPlans.Add(workoutPlan.Name, workoutPlan);
                workoutPlan = createPlan(profile2, "test2-4",PublishStatus.Published, TrainingPlanDifficult.NotSet, TrainingType.HIT);
                workoutPlans.Add(workoutPlan.Name, workoutPlan);
                workoutPlan = createPlan(profile2, "test2-5",PublishStatus.Private, TrainingPlanDifficult.Beginner, TrainingType.Split);
                workoutPlans.Add(workoutPlan.Name, workoutPlan);
                workoutPlan = createPlan(profile2, "test2-6",PublishStatus.Private, TrainingPlanDifficult.Advanced, TrainingType.ACT);
                workoutPlans.Add(workoutPlan.Name, workoutPlan);
                workoutPlan = createPlan(profile2, "test2-7",PublishStatus.Published, TrainingPlanDifficult.NotSet, TrainingType.FBW);
                workoutPlans.Add(workoutPlan.Name, workoutPlan);
                workoutPlan = createPlan(profile2, "test2-8",PublishStatus.Private, TrainingPlanDifficult.Beginner, TrainingType.Split);
                workoutPlans.Add(workoutPlan.Name, workoutPlan);

                ////add favorities for profile 1
                //profile1.FavoriteWorkoutPlans.Add(workoutPlans["test2-2"]);
                //Session.SaveOrUpdate(profile1);
                ////and for profile 2
                //profile2.FavoriteWorkoutPlans.Add(workoutPlans["test1-1"]);
                //profile2.FavoriteWorkoutPlans.Add(workoutPlans["test1-6"]);
                //Session.SaveOrUpdate(profile2);
                tx.Commit();
            }

        }

        private TrainingPlan createPlan(Profile profile1, string name,PublishStatus status, TrainingPlanDifficult difficult, TrainingType type,params Exercise[] exercises)
        {

            var workoutPlan = new TrainingPlan();
            //workoutPlan.GlobalId = Guid.NewGuid();
            workoutPlan.Language = "en";
            workoutPlan.Profile = profile1;
            workoutPlan.Name= name;
            workoutPlan.TrainingType = type;
            workoutPlan.Difficult =  difficult;
            workoutPlan.Author= "test";

            workoutPlan.Status = status;
            if (status==PublishStatus.Published)
            {
                workoutPlan.PublishDate = DateTime.UtcNow;
            }
            BodyArchitect.Model.TrainingPlanDay day = new BodyArchitect.Model.TrainingPlanDay();
            day.Name = "day";
            workoutPlan.Days.Add(day);
            day.TrainingPlan = workoutPlan;
            workoutPlan.Language = workoutPlan.Language;
            foreach (var exercise in exercises)
            {
                BodyArchitect.Model.TrainingPlanEntry entry = new BodyArchitect.Model.TrainingPlanEntry();
                entry.Exercise = exercise;
                day.Entries.Add(entry);
                entry.Day = day;
            }

            insertToDatabase(workoutPlan);
            workoutPlan.Tag = Mapper.Map<TrainingPlan, Service.V2.Model.TrainingPlans.TrainingPlan>(workoutPlan);
            return workoutPlan;
        }


        [Test]
        [ExpectedException(typeof(PublishedObjectOperationException))]
        public void TestPublishWorkoutPlan_PublishedPlan()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var plan = (Service.V2.Model.TrainingPlans.TrainingPlan)workoutPlans["test1-3"].Tag;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new WorkoutPlanOperationParam();
                param.WorkoutPlanId = plan.GlobalId;
                param.Operation = SupplementsCycleDefinitionOperation.Publish;
                Service.WorkoutPlanOperation(data.Token, param);
            });

        }

        [Test]
        [ExpectedException(typeof(ProfileRankException))]
        public void TestPublishWorkoutPlan_NotEnoughStrengthTrainingEntries()
        {
            profiles[0].Statistics.StrengthTrainingEntriesCount = 20;
            insertToDatabase(profiles[0].Statistics);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var plan = (Service.V2.Model.TrainingPlans.TrainingPlan)workoutPlans["test1-4"].Tag;
            Assert.AreEqual(BodyArchitect.Service.V2.Model.PublishStatus.Private, plan.Status);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new WorkoutPlanOperationParam();
                param.WorkoutPlanId = plan.GlobalId;
                param.Operation = SupplementsCycleDefinitionOperation.Publish;
                Service.WorkoutPlanOperation(data.Token, param);
            });
        }

        [Test]
        public void TestPublishWorkoutPlan()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var plan = (Service.V2.Model.TrainingPlans.TrainingPlan)workoutPlans["test1-4"].Tag;
            Assert.AreEqual(BodyArchitect.Service.V2.Model.PublishStatus.Private, plan.Status);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new WorkoutPlanOperationParam();
                param.WorkoutPlanId = plan.GlobalId;
                param.Operation = SupplementsCycleDefinitionOperation.Publish;
                Service.WorkoutPlanOperation(data.Token, param);
            });
            var dbPlan = Session.Get<TrainingPlan>(plan.GlobalId);
            Assert.AreEqual(PublishStatus.Published,dbPlan.Status);
            Assert.IsNotNull(dbPlan.PublishDate);
        }

        [Test]
        public void TestPublishWorkoutPlan_Statistics()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var plan = (Service.V2.Model.TrainingPlans.TrainingPlan)workoutPlans["test1-4"].Tag;
            Assert.AreEqual(BodyArchitect.Service.V2.Model.PublishStatus.Private, plan.Status);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new WorkoutPlanOperationParam();
                param.WorkoutPlanId = plan.GlobalId;
                param.Operation = SupplementsCycleDefinitionOperation.Publish;
                Service.WorkoutPlanOperation(data.Token, param);
            });
            var dbProfile=Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(5,dbProfile.Statistics.WorkoutPlansCount);
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void TestPublishWorkoutPlan_WithMissingExercise()
        {
            var missingExercise=CreateExercise(Session, null, "Missing", "--");
            missingExercise.IsDeleted = true;
            insertToDatabase(missingExercise);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var plan = createPlan(profiles[0], "test1-3", PublishStatus.Private, TrainingPlanDifficult.Beginner, TrainingType.PushPull, missingExercise);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new WorkoutPlanOperationParam();
                param.WorkoutPlanId = plan.GlobalId;
                param.Operation = SupplementsCycleDefinitionOperation.Publish;
                Service.WorkoutPlanOperation(data.Token, param);
            });
        }

        [Test]
        public void PublishWorkoutPlan_DataInfo_Refresh()
        {

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var plan = (Service.V2.Model.TrainingPlans.TrainingPlan)workoutPlans["test1-4"].Tag;
            Assert.AreEqual(BodyArchitect.Service.V2.Model.PublishStatus.Private, plan.Status);
            var oldHash = profiles[0].DataInfo.WorkoutPlanHash;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new WorkoutPlanOperationParam();
                param.WorkoutPlanId = plan.GlobalId;
                param.Operation = SupplementsCycleDefinitionOperation.Publish;
                Service.WorkoutPlanOperation(data.Token, param);
            });
            var dbPlan = Session.Get<TrainingPlan>(plan.GlobalId);
            Assert.AreEqual(PublishStatus.Published, dbPlan.Status);
            Assert.IsNotNull(dbPlan.PublishDate);

            var dbProfile = Session.Get<Profile>(plan.Profile.GlobalId);
            Assert.AreNotEqual(oldHash,dbProfile.DataInfo.WorkoutPlanHash);
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void TestPublishWorkoutPlan_CrossProfile()
        {
            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var plan = (Service.V2.Model.TrainingPlans.TrainingPlan)workoutPlans["test1-2"].Tag;
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new WorkoutPlanOperationParam();
                param.WorkoutPlanId = plan.GlobalId;
                param.Operation = SupplementsCycleDefinitionOperation.Publish;
                Service.WorkoutPlanOperation(data.Token, param);
            });
        }

    }
}
