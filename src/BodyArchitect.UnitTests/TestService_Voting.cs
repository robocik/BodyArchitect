using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using BodyArchitect.Model;
using BodyArchitect.Service;
using BodyArchitect.Service.Model;
using NUnit.Framework;
using MessageType = BodyArchitect.Model.MessageType;
using Profile = BodyArchitect.Model.Profile;
using PublishStatus = BodyArchitect.Model.PublishStatus;
using WorkoutPlanPurpose = BodyArchitect.Model.WorkoutPlanPurpose;

namespace BodyArchitect.UnitTests
{
    [TestFixture]
    public class TestService_Voting:TestServiceBase
    {
        Dictionary<string, TrainingPlan> workoutPlans = new Dictionary<string, TrainingPlan>();
        Dictionary<string, Exercise> exercises = new Dictionary<string, Exercise>();
        List<Profile> profiles = new List<Profile>();

        public override void BuildDatabase()
        {
            using (var tx=Session.BeginTransaction())
            {
                profiles.Clear();
                workoutPlans.Clear();
                exercises.Clear();
                profiles.Add(CreateProfile(Session, "test1"));
                profiles.Add(CreateProfile(Session, "test2"));
                //creates workout plans for profile 1
                var workoutPlan = CreatePlan(Session, profiles[0], "test1-1", TrainingPlanDifficult.Beginner, TrainingType.HST, true, Language.Languages[0].Shortcut, WorkoutPlanPurpose.FatLost, 3);
                workoutPlans.Add(workoutPlan.Name, workoutPlan);
                workoutPlan = CreatePlan(Session, profiles[1], "test1-2", TrainingPlanDifficult.Advanced, TrainingType.HST, false, Language.Languages[0].Shortcut, WorkoutPlanPurpose.Mass, 3);
                workoutPlans.Add(workoutPlan.Name, workoutPlan);

                var ex=CreateExercise(Session, profiles[0], "Test", "t", PublishStatus.Published);
                exercises.Add(ex.Shortcut,ex);
                ex = CreateExercise(Session,null, "Global", "global", PublishStatus.Published);
                exercises.Add(ex.Shortcut, ex);
                tx.Commit();
            }
        }

        [Test]
        public void TestVotePlan_SendMessage()
        {
            //var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
                                 {
                                     WorkoutPlanDTO dto = (WorkoutPlanDTO)workoutPlans["test1-1"].Tag;
                                     dto.UserRating = 3;
                                     dto.UserShortComment = "test";
                                     service.VoteWorkoutPlan(data.Token, dto);
                                 });
            var messages = Session.QueryOver<Message>().Where(x => x.Receiver == profiles[0] && x.MessageType == MessageType.WorkoutPlanVoted).SingleOrDefault();
            Assert.IsNotNull(messages);
            Assert.AreEqual(4, messages.Content.Split(',').Length);
            Assert.AreEqual(workoutPlans["test1-1"].Name, messages.Content.Split(',')[0]);
            Assert.AreEqual(profile1.UserName, messages.Content.Split(',')[1]);
            Assert.AreEqual(3, int.Parse(messages.Content.Split(',')[3]));
        }

        [Test]
        public void TestVotePlan_DisabledSendMessage()
        {
            profiles[0].Settings.NotificationWorkoutPlanVoted = false;
            insertToDatabase(profiles[0]);

            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                WorkoutPlanDTO dto = (WorkoutPlanDTO)workoutPlans["test1-1"].Tag;
                dto.UserRating = 3;
                dto.UserShortComment = "test";
                service.VoteWorkoutPlan(data.Token, dto);
            });
            var messages = Session.QueryOver<Message>().Where(x => x.Receiver == profiles[0] && x.MessageType == MessageType.WorkoutPlanVoted).SingleOrDefault();
            Assert.IsNull(messages);
        }

        [Test]
        public void TestVoteExercise_Global_DontSendMessage()
        {
            //var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ExerciseDTO dto = Mapper.Map<Exercise, ExerciseDTO>(exercises["global"]);
                dto.UserRating = 3;
                dto.UserShortComment = "test";
                service.VoteExercise(data.Token, dto);
            });
            var count = Session.QueryOver<Message>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestVoteExercise_SendMessage()
        {
            //var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
                                 {
                                     ExerciseDTO dto = Mapper.Map<Exercise, ExerciseDTO>(exercises["t"]);
                                    dto.UserRating = 3;
                                    dto.UserShortComment = "test";
                                    service.VoteExercise(data.Token, dto);
            });
            var messages = Session.QueryOver<Message>().Where(x => x.Receiver == profiles[0] && x.MessageType == MessageType.ExerciseVoted).SingleOrDefault();
            Assert.IsNotNull(messages);
            Assert.AreEqual(4, messages.Content.Split(',').Length);
            Assert.AreEqual(exercises["t"].Name, messages.Content.Split(',')[0]);
            Assert.AreEqual(profile1.UserName, messages.Content.Split(',')[1]);
            Assert.AreEqual(3, int.Parse(messages.Content.Split(',')[3]));
        }

        [Test]
        public void TestVoteExercise_DisableSendMessage()
        {
            profiles[0].Settings.NotificationExerciseVoted = false;
            insertToDatabase(profiles[0]);

            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ExerciseDTO dto = Mapper.Map<Exercise, ExerciseDTO>(exercises["t"]);
                dto.UserRating = 3;
                dto.UserShortComment = "test";
                service.VoteExercise(data.Token, dto);
            });
            var messages = Session.QueryOver<Message>().Where(x => x.Receiver == profiles[0] && x.MessageType == MessageType.ExerciseVoted).SingleOrDefault();
            Assert.IsNull(messages);
        }
        #region Statistics

        [Test]
        public void TestVotePlan_Statistics()
        {
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                WorkoutPlanDTO dto = (WorkoutPlanDTO)workoutPlans["test1-1"].Tag;
                dto.UserRating = 3;
                dto.UserShortComment = "test";
                service.VoteWorkoutPlan(data.Token, dto);
            });
            var dbProfile = Session.Get<Profile>(profile1.Id);
            Assert.AreEqual(1, dbProfile.Statistics.VotingsCount);
        }

        [Test]
        public void TestVoteExercise_Statistics()
        {
            //var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ExerciseDTO dto = Mapper.Map<Exercise, ExerciseDTO>(exercises["t"]);
                dto.UserRating = 3;
                dto.UserShortComment = "test";
                service.VoteExercise(data.Token, dto);
            });
            var dbProfile = Session.Get<Profile>(profile1.Id);
            Assert.AreEqual(1, dbProfile.Statistics.VotingsCount);
        }
        #endregion
    }
}
