using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Model;
using BodyArchitect.Service;
using BodyArchitect.Service.Model;
using BodyArchitect.Service.Model.TrainingPlans;
using BodyArchitect.Shared;
using NUnit.Framework;
using ExerciseDifficult = BodyArchitect.Model.ExerciseDifficult;
using ExerciseForceType = BodyArchitect.Model.ExerciseForceType;
using MechanicsType = BodyArchitect.Model.MechanicsType;
using Profile = BodyArchitect.Model.Profile;
using PublishStatus = BodyArchitect.Model.PublishStatus;
using TrainingEnd = BodyArchitect.Service.Model.TrainingEnd;
using TrainingPlan = BodyArchitect.Model.TrainingPlan;
using TrainingPlanDifficult = BodyArchitect.Model.TrainingPlanDifficult;
using TrainingType = BodyArchitect.Model.TrainingType;

namespace BodyArchitect.UnitTests
{
    [TestFixture]
    public class TestService_WorkoutFavoriteOperation : TestServiceBase
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
                workoutPlan = createPlan(profile1, "test1-2",PublishStatus.Private, TrainingPlanDifficult.Advanced, TrainingType.HST,  exercises["t11"], exercises["t12"], exercises["t13"], exercises["t14"], exercises["t01"]);
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
            var plan = new BodyArchitect.Service.Model.TrainingPlans.TrainingPlan();
            
            var workoutPlan = new TrainingPlan();
            workoutPlan.GlobalId = Guid.NewGuid();
            workoutPlan.Language = "en";
            workoutPlan.Profile = profile1;
            workoutPlan.Name=plan.Name = name;
            plan.TrainingType = (Service.Model.TrainingPlans.TrainingType)(workoutPlan.TrainingType = type);
            plan.Difficult = (Service.Model.TrainingPlans.TrainingPlanDifficult)(workoutPlan.Difficult = difficult);
            workoutPlan.Author=plan.Author = "test";

            workoutPlan.Status = status;
            if (status==PublishStatus.Published)
            {
                workoutPlan.PublishDate = DateTime.UtcNow;
            }
            TrainingPlanDay day = new TrainingPlanDay();
            day.Name = "day";
            plan.AddDay(day);
            plan.Language = workoutPlan.Language;
            foreach (var exercise in exercises)
            {
                TrainingPlanEntry entry = new TrainingPlanEntry();
                entry.ExerciseId = exercise.GlobalId;
                day.AddEntry(entry);
            }

            XmlSerializationTrainingPlanFormatter formatter = new XmlSerializationTrainingPlanFormatter();
            workoutPlan.PlanContent = formatter.ToXml(plan).ToString();

            Session.Save(workoutPlan);
            workoutPlan.Tag = Mapper.Map<TrainingPlan, WorkoutPlanDTO>(workoutPlan);
            return workoutPlan;
        }

        [Test]
        [ExpectedException(typeof(PublishedObjectOperationException))]
        public void TestPublishWorkoutPlan_NotPublishedExercises()
        {
            var criteria = ExerciseSearchCriteria.CreateAllCriteria();
            criteria.UserId = profiles[1].Id;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            var plan = (WorkoutPlanDTO) workoutPlans["test1-2"].Tag;
            Assert.AreEqual(BodyArchitect.Service.Model.PublishStatus.Private,plan.Status);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.PublishWorkoutPlan(data.Token, plan);
            });

        }

        [Test]
        [ExpectedException(typeof(PublishedObjectOperationException))]
        public void TestPublishWorkoutPlan_PublishedPlan()
        {
            var criteria = ExerciseSearchCriteria.CreateAllCriteria();
            criteria.UserId = profiles[1].Id;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            var plan = (WorkoutPlanDTO)workoutPlans["test1-3"].Tag;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.PublishWorkoutPlan(data.Token, plan);
            });

        }

        [Test]
        public void TestPublishWorkoutPlan()
        {
            var criteria = ExerciseSearchCriteria.CreateAllCriteria();
            criteria.UserId = profiles[1].Id;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            var plan = (WorkoutPlanDTO)workoutPlans["test1-4"].Tag;
            Assert.AreEqual(BodyArchitect.Service.Model.PublishStatus.Private, plan.Status);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.PublishWorkoutPlan(data.Token, plan);
            });
            var dbPlan = Session.Get<TrainingPlan>(plan.GlobalId);
            Assert.AreEqual(PublishStatus.Published,dbPlan.Status);
            Assert.IsNotNull(dbPlan.PublishDate);
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void TestPublishWorkoutPlan_CrossProfile()
        {
            var criteria = ExerciseSearchCriteria.CreateAllCriteria();
            criteria.UserId = profiles[1].Id;

            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            var plan = (WorkoutPlanDTO)workoutPlans["test1-2"].Tag;
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.PublishWorkoutPlan(data.Token, plan);
            });
        }

    }
}
