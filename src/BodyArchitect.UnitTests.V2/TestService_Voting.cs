using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using NUnit.Framework;
using MessagePriority = BodyArchitect.Model.MessagePriority;
using Profile = BodyArchitect.Model.Profile;
using ProfileNotification = BodyArchitect.Model.ProfileNotification;
using PublishStatus = BodyArchitect.Model.PublishStatus;
using WorkoutPlanPurpose = BodyArchitect.Model.WorkoutPlanPurpose;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_Voting:TestServiceBase
    {
        Dictionary<string, TrainingPlan> workoutPlans = new Dictionary<string, TrainingPlan>();
        Dictionary<string, Exercise> exercises = new Dictionary<string, Exercise>();
        List<Profile> profiles = new List<Profile>();
        private APIKey apiKey;
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

                var ex=CreateExercise(Session, profiles[0], "Test", "t");
                exercises.Add(ex.Shortcut,ex);
                ex = CreateExercise(Session,null, "Global", "global");
                exercises.Add(ex.Shortcut, ex);

                apiKey = new APIKey();
                apiKey.ApiKey = Guid.NewGuid();
                apiKey.ApplicationName = "UnitTest";
                apiKey.EMail = "mail@mail.com";
                apiKey.RegisterDateTime = DateTime.UtcNow;
                insertToDatabase(apiKey);
                tx.Commit();
            }
        }

        #region Supplements
        [Test]
        public void Supplement_RatingUpdate()
        {
            var sup=CreateSupplement("sup",false,false,profiles[0]);
            //var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var dto = sup.Map<SuplementDTO>();
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.Supplement;
                param.GlobalId = dto.GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                dto.Rating=service.Vote(data.Token, param).Rating;
            });
            Assert.AreEqual(3, dto.Rating);
        }

        [Test]
        public void Supplement_SetLoginData()
        {
            var sup = CreateSupplement("sup", false, false, profiles[0]);
            //var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation, null, apiKey);
            var loginData = Session.QueryOver<LoginData>().SingleOrDefault();

            var dto = sup.Map<SuplementDTO>();
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.Supplement;
                param.GlobalId = dto.GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                dto.Rating = service.Vote(data.Token, param).Rating;
            });
            var rating = Session.QueryOver<RatingUserValue>().SingleOrDefault();
            Assert.AreEqual(loginData.ApiKey.ApiKey, rating.LoginData.ApiKey.ApiKey);
        }

        [Test]
        public void SupplementUpdateRating_SetLoginData()
        {
            var sup = CreateSupplement("sup", false, false, profiles[0]);
            //var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation, null, apiKey);
            var loginData = Session.QueryOver<LoginData>().SingleOrDefault();

            var dto = sup.Map<SuplementDTO>();
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.Supplement;
                param.GlobalId = dto.GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                dto.Rating = service.Vote(data.Token, param).Rating;
            });

            var key = new APIKey();
            key.ApiKey = Guid.NewGuid();
            key.ApplicationName = "UnitTest1";
            key.EMail = "mail1@mail.com";
            key.RegisterDateTime = DateTime.UtcNow;
            insertToDatabase(key);

            data = CreateNewSession(profile1, ClientInformation, null, key);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.Supplement;
                param.GlobalId = dto.GlobalId;
                param.UserRating = 1;
                param.UserShortComment = "test";
                dto.Rating = service.Vote(data.Token, param).Rating;
            });
            var rating = Session.QueryOver<RatingUserValue>().SingleOrDefault();
            Assert.AreEqual(key.ApiKey, rating.LoginData.ApiKey.ApiKey);
        }

        [Test]
        public void Supplement_RatingUpdateTwice()
        {
            var sup = CreateSupplement("sup", false, false, profiles[0]);

            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var dto = sup.Map<SuplementDTO>();
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.Supplement;
                param.GlobalId = dto.GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                dto.Rating = service.Vote(data.Token, param).Rating;
            });

            data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.Supplement;
                param.GlobalId = dto.GlobalId;
                param.UserRating = 1;
                param.UserShortComment = "test";
                dto.Rating = service.Vote(data.Token, param).Rating;
            });
            Assert.AreEqual(2, dto.Rating);
        }
        #endregion

        #region Exercises
        [Test]
        public void Exercise_RatingUpdate()
        {
            //var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            ExerciseDTO dto = Mapper.Map<Exercise, ExerciseDTO>(exercises["t"]);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.Exercise;
                param.GlobalId = dto.GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                dto.Rating = service.Vote(data.Token, param).Rating;
            });
            Assert.AreEqual(3, dto.Rating);
        }

        [Test]
        public void Exercise_SetLoginData()
        {
            //var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation,null,apiKey);
            var loginData = Session.QueryOver<LoginData>().SingleOrDefault();

            ExerciseDTO dto = Mapper.Map<Exercise, ExerciseDTO>(exercises["t"]);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.Exercise;
                param.GlobalId = dto.GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                dto.Rating = service.Vote(data.Token, param).Rating;
            });
            var rating=Session.QueryOver<RatingUserValue>().SingleOrDefault();
            Assert.AreEqual(loginData.ApiKey.ApiKey, rating.LoginData.ApiKey.ApiKey);
        }

        [Test]
        public void ExerciseUpdateRating_SetLoginData()
        {
            //var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation, null, apiKey);
            var loginData = Session.QueryOver<LoginData>().SingleOrDefault();

            ExerciseDTO dto = Mapper.Map<Exercise, ExerciseDTO>(exercises["t"]);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.Exercise;
                param.GlobalId = dto.GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                dto.Rating = service.Vote(data.Token, param).Rating;
            });

            var key = new APIKey();
            key.ApiKey = Guid.NewGuid();
            key.ApplicationName = "UnitTest1";
            key.EMail = "mail1@mail.com";
            key.RegisterDateTime = DateTime.UtcNow;
            insertToDatabase(key);

            data = CreateNewSession(profile1, ClientInformation, null, key);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.Exercise;
                param.GlobalId = dto.GlobalId;
                param.UserRating = 1;
                param.UserShortComment = "test";
                dto.Rating = service.Vote(data.Token, param).Rating;
            });
            var rating = Session.QueryOver<RatingUserValue>().SingleOrDefault();
            Assert.AreEqual(key.ApiKey, rating.LoginData.ApiKey.ApiKey);
        }

        [Test]
        public void Exercise_RatingUpdateTwice()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            ExerciseDTO dto = Mapper.Map<Exercise, ExerciseDTO>(exercises["t"]);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.Exercise;
                param.GlobalId = dto.GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                dto.Rating = service.Vote(data.Token, param).Rating;
            });

            data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.Exercise;
                param.GlobalId = dto.GlobalId;
                param.UserRating =1;
                param.UserShortComment = "test";
                dto.Rating = service.Vote(data.Token, param).Rating;
            });
            Assert.AreEqual(2, dto.Rating);
        }

        [Test]
        public void TestVoteExercise_Global_DontSendMessage()
        {
            //var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {

                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.Exercise;
                param.GlobalId = exercises["global"].GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                service.Vote(data.Token, param);
            });
            var count = Session.QueryOver<Message>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestVoteExercise_SendMessage()
        {
            //var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.Exercise;
                param.GlobalId = exercises["t"].GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                service.Vote(data.Token, param);
                Assert.IsFalse(((MockEmailService)service.EMailService).EMailSent);
            });
            var messages = Session.QueryOver<Message>().Where(x => x.Receiver == profiles[0]).SingleOrDefault();
            Assert.IsNotNull(messages);
            Assert.IsNotNull(messages.Content);
            Assert.AreEqual(MessagePriority.System, messages.Priority);
        }

        [Test]
        public void TestVoteExercise_SendEMail()
        {
            profiles[0].Settings.NotificationVoted = ProfileNotification.Email;
            insertToDatabase(profiles[0]);

            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.Exercise;
                param.GlobalId = exercises["t"].GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                service.Vote(data.Token, param);
                Assert.IsTrue(((MockEmailService)service.EMailService).EMailSent);
            });
            var count = Session.QueryOver<Message>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestVoteExercise_SendMessageAndEMail()
        {
            profiles[0].Settings.NotificationVoted = ProfileNotification.Message | ProfileNotification.Email;
            insertToDatabase(profiles[0]);

            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.Exercise;
                param.GlobalId = exercises["t"].GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                service.Vote(data.Token, param);
                Assert.IsTrue(((MockEmailService)service.EMailService).EMailSent);
            });
            var messages = Session.QueryOver<Message>().Where(x => x.Receiver == profiles[0]).SingleOrDefault();
            Assert.IsNotNull(messages);
            Assert.IsNotNull(messages.Content);
            Assert.AreEqual(MessagePriority.System, messages.Priority);
        }

        [Test]
        public void TestVoteExercise_DisableSendMessage()
        {
            profiles[0].Settings.NotificationVoted = ProfileNotification.None;
            insertToDatabase(profiles[0]);

            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.Exercise;
                param.GlobalId = exercises["t"].GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                service.Vote(data.Token, param);
                Assert.IsFalse(((MockEmailService)service.EMailService).EMailSent);
            });
            var messages = Session.QueryOver<Message>().Where(x => x.Receiver == profiles[0] ).SingleOrDefault();
            Assert.IsNull(messages);
        }
        #endregion

        #region Workout plans

        [Test]
        public void WorkoutPlan_RatingUpdate()
        {
            //var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var dto = (Service.V2.Model.TrainingPlans.TrainingPlan)workoutPlans["test1-1"].Tag;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.WorkoutPlan;
                param.GlobalId = dto.GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                dto.Rating = service.Vote(data.Token, param).Rating;
            });
            Assert.AreEqual(3,dto.Rating);
        }

        [Test]
        public void WorkoutPlan_SetLoginData()
        {
            //var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation, null, apiKey);
            var loginData = Session.QueryOver<LoginData>().SingleOrDefault();

            var dto = (Service.V2.Model.TrainingPlans.TrainingPlan)workoutPlans["test1-1"].Tag;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.WorkoutPlan;
                param.GlobalId = dto.GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                dto.Rating=service.Vote(data.Token, param).Rating;
            });
            var rating = Session.QueryOver<RatingUserValue>().SingleOrDefault();
            Assert.AreEqual(loginData.ApiKey.ApiKey, rating.LoginData.ApiKey.ApiKey);
        }

        [Test]
        public void WorkoutPlanUpdateRating_SetLoginData()
        {
            //var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation, null, apiKey);
            var loginData = Session.QueryOver<LoginData>().SingleOrDefault();

            var dto = (Service.V2.Model.TrainingPlans.TrainingPlan)workoutPlans["test1-1"].Tag;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.WorkoutPlan;
                param.GlobalId = dto.GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                dto.Rating = service.Vote(data.Token, param).Rating;
            });

            var key = new APIKey();
            key.ApiKey = Guid.NewGuid();
            key.ApplicationName = "UnitTest1";
            key.EMail = "mail1@mail.com";
            key.RegisterDateTime = DateTime.UtcNow;
            insertToDatabase(key);

            data = CreateNewSession(profile1, ClientInformation, null, key);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.WorkoutPlan;
                param.GlobalId = dto.GlobalId;
                param.UserRating = 1;
                param.UserShortComment = "test";
                dto.Rating = service.Vote(data.Token, param).Rating;
            });
            var rating = Session.QueryOver<RatingUserValue>().SingleOrDefault();
            Assert.AreEqual(key.ApiKey, rating.LoginData.ApiKey.ApiKey);
        }

        [Test]
        public void WorkoutPlan_RatingUpdate_Twice()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var dto = (Service.V2.Model.TrainingPlans.TrainingPlan)workoutPlans["test1-1"].Tag;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {

                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.WorkoutPlan;
                param.GlobalId = dto.GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                dto.Rating = service.Vote(data.Token, param).Rating;
            });
            data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.WorkoutPlan;
                param.GlobalId = dto.GlobalId;
                param.UserRating = 1;
                param.UserShortComment = "test";
                dto.Rating = service.Vote(data.Token, param).Rating;
            });
            Assert.AreEqual(2, dto.Rating);
        }

        [Test]
        public void TestVotePlan_SendMessage()
        {
            //var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
                                 {
                                     VoteParams param = new VoteParams();
                                     param.ObjectType = VoteObject.WorkoutPlan;
                                     param.GlobalId = workoutPlans["test1-1"].GlobalId;
                                     param.UserRating = 3;
                                     param.UserShortComment = "test";
                                     service.Vote(data.Token, param);
                                     Assert.IsFalse(((MockEmailService)service.EMailService).EMailSent);
                                 });
            var messages = Session.QueryOver<Message>().Where(x => x.Receiver == profiles[0]).SingleOrDefault();
            Assert.IsNotNull(messages);
            Assert.IsNotNull(messages.Content);
            Assert.AreEqual(MessagePriority.System,messages.Priority);
        }

        [Test]
        public void TestVotePlan_SendEMail()
        {
            profiles[0].Settings.NotificationVoted = ProfileNotification.Email;
            insertToDatabase(profiles[0]);

            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.WorkoutPlan;
                param.GlobalId = workoutPlans["test1-1"].GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                service.Vote(data.Token, param);
                Assert.IsTrue(((MockEmailService)service.EMailService).EMailSent);
            });
            var count = Session.QueryOver<Message>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestVotePlan_SendMessageAndEMail()
        {
            profiles[0].Settings.NotificationVoted = ProfileNotification.Message | ProfileNotification.Email;
            insertToDatabase(profiles[0]);

            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.WorkoutPlan;
                param.GlobalId = workoutPlans["test1-1"].GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                service.Vote(data.Token, param);
                Assert.IsTrue(((MockEmailService)service.EMailService).EMailSent);
            });
            var messages = Session.QueryOver<Message>().Where(x => x.Receiver == profiles[0]).SingleOrDefault();
            Assert.IsNotNull(messages);
            Assert.IsNotNull(messages.Content);
            Assert.AreEqual(MessagePriority.System, messages.Priority);
        }

        [Test]
        public void TestVotePlan_DisabledSendMessage()
        {
            profiles[0].Settings.NotificationVoted = ProfileNotification.None;
            insertToDatabase(profiles[0]);

            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.WorkoutPlan;
                param.GlobalId = workoutPlans["test1-1"].GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                service.Vote(data.Token, param);
                Assert.IsFalse(((MockEmailService)service.EMailService).EMailSent);
            });
            var messages = Session.QueryOver<Message>().Where(x => x.Receiver == profiles[0] ).SingleOrDefault();
            Assert.IsNull(messages);
        }

        #endregion

        #region Supplements definition cycle

        [Test]
        public void SupplementCycleDefinition_RatingUpdate()
        {
            var sup=CreateSupplement("sup");
            var def=CreateSupplementsCycleDefinition("def",sup,profiles[0]);
            //var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var dto = def.Map<SupplementCycleDefinitionDTO>();
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.SupplementCycleDefinition;
                param.GlobalId = dto.GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                dto.Rating = service.Vote(data.Token, param).Rating;
            });
            Assert.AreEqual(3, dto.Rating);
        }

        [Test]
        public void SupplementCycleDefinition_SetLoginData()
        {
            var sup = CreateSupplement("sup");
            var def = CreateSupplementsCycleDefinition("def", sup, profiles[0]);
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation, null, apiKey);
            var loginData = Session.QueryOver<LoginData>().SingleOrDefault();

            var dto = def.Map<SupplementCycleDefinitionDTO>();
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.SupplementCycleDefinition;
                param.GlobalId = dto.GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                dto.Rating = service.Vote(data.Token, param).Rating;
            });
            var rating = Session.QueryOver<RatingUserValue>().SingleOrDefault();
            Assert.AreEqual(loginData.ApiKey.ApiKey, rating.LoginData.ApiKey.ApiKey);
        }

        [Test]
        public void SupplementCycleDefinitionUpdateRating_SetLoginData()
        {
            var sup = CreateSupplement("sup");
            var def = CreateSupplementsCycleDefinition("def", sup, profiles[0]);
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation, null, apiKey);
            var loginData = Session.QueryOver<LoginData>().SingleOrDefault();

            var dto = def.Map<SupplementCycleDefinitionDTO>();
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.SupplementCycleDefinition;
                param.GlobalId = dto.GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                dto.Rating = service.Vote(data.Token, param).Rating;
            });

            var key = new APIKey();
            key.ApiKey = Guid.NewGuid();
            key.ApplicationName = "UnitTest1";
            key.EMail = "mail1@mail.com";
            key.RegisterDateTime = DateTime.UtcNow;
            insertToDatabase(key);

            data = CreateNewSession(profile1, ClientInformation, null, key);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.SupplementCycleDefinition;
                param.GlobalId = dto.GlobalId;
                param.UserRating = 1;
                param.UserShortComment = "test";
                dto.Rating = service.Vote(data.Token, param).Rating;
            });
            var rating = Session.QueryOver<RatingUserValue>().SingleOrDefault();
            Assert.AreEqual(key.ApiKey, rating.LoginData.ApiKey.ApiKey);
        }

        [Test]
        public void SupplementCycleDefinition_RatingUpdate_Twice()
        {
            var sup = CreateSupplement("sup");
            var def = CreateSupplementsCycleDefinition("def", sup, profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var dto = def.Map<SupplementCycleDefinitionDTO>();
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.SupplementCycleDefinition;
                param.GlobalId = dto.GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                dto.Rating = service.Vote(data.Token, param).Rating;
            });
            data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.SupplementCycleDefinition;
                param.GlobalId = dto.GlobalId;
                param.UserRating = 1;
                param.UserShortComment = "test";
                dto.Rating = service.Vote(data.Token, param).Rating;
            });
            Assert.AreEqual(2, dto.Rating);
        }

        [Test]
        public void SupplementCycleDefinition_SendEMail()
        {
            var sup = CreateSupplement("sup");
            var def = CreateSupplementsCycleDefinition("def", sup, profiles[0]);
            var profile1 = (ProfileDTO)profiles[1].Tag;
            profiles[0].Settings.NotificationVoted = ProfileNotification.Email;
            insertToDatabase(profiles[0]);
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var dto = def.Map<SupplementCycleDefinitionDTO>();
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.SupplementCycleDefinition;
                param.GlobalId = dto.GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                service.Vote(data.Token, param);
                Assert.IsTrue(((MockEmailService)service.EMailService).EMailSent);
            });
            var count = Session.QueryOver<Message>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void SupplementCycleDefinition_SendEMailAndMessage()
        {
            var sup = CreateSupplement("sup");
            var def = CreateSupplementsCycleDefinition("def", sup, profiles[0]);
            var profile1 = (ProfileDTO)profiles[1].Tag;
            profiles[0].Settings.NotificationVoted = ProfileNotification.Email | ProfileNotification.Message;
            insertToDatabase(profiles[0]);
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var dto = def.Map<SupplementCycleDefinitionDTO>();
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.SupplementCycleDefinition;
                param.GlobalId = dto.GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                service.Vote(data.Token, param);
                Assert.IsTrue(((MockEmailService)service.EMailService).EMailSent);
            });
            var messages = Session.QueryOver<Message>().Where(x => x.Receiver == profiles[0]).SingleOrDefault();
            Assert.IsNotNull(messages);
            Assert.IsNotNull(messages.Content);
            Assert.AreEqual(MessagePriority.System, messages.Priority);
        }

        [Test]
        public void SupplementCycleDefinition_SendMessage()
        {
            var sup = CreateSupplement("sup");
            var def = CreateSupplementsCycleDefinition("def", sup, profiles[0]);
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var dto = def.Map<SupplementCycleDefinitionDTO>();
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.SupplementCycleDefinition;
                param.GlobalId = dto.GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                service.Vote(data.Token, param);
                Assert.IsFalse(((MockEmailService)service.EMailService).EMailSent);
            });
            var messages = Session.QueryOver<Message>().Where(x => x.Receiver == profiles[0]).SingleOrDefault();
            Assert.IsNotNull(messages);
            Assert.IsNotNull(messages.Content);
            Assert.AreEqual(MessagePriority.System, messages.Priority);
        }

        [Test]
        public void SupplementCycleDefinition_DisabledSendMessage()
        {

            var sup = CreateSupplement("sup");
            var def = CreateSupplementsCycleDefinition("def", sup, profiles[0]);
            profiles[0].Settings.NotificationVoted = ProfileNotification.None;
            insertToDatabase(profiles[0]);

            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var dto = def.Map<SupplementCycleDefinitionDTO>();
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.SupplementCycleDefinition;
                param.GlobalId = dto.GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                service.Vote(data.Token, param);
                Assert.IsFalse(((MockEmailService)service.EMailService).EMailSent);
            });
            var messages = Session.QueryOver<Message>().Where(x => x.Receiver == profiles[0]).SingleOrDefault();
            Assert.IsNull(messages);
        }

        #endregion

        #region Statistics

        [Test]
        public void TestVotePlan_Statistics()
        {
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.WorkoutPlan;
                param.GlobalId = workoutPlans["test1-1"].GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                service.Vote(data.Token, param);
            });
            var dbProfile = Session.Get<Profile>(profile1.GlobalId);
            Assert.AreEqual(1, dbProfile.Statistics.VotingsCount);
        }

        [Test]
        public void TestVoteExercise_Statistics()
        {
            //var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.Exercise;
                param.GlobalId = exercises["t"].GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                service.Vote(data.Token, param);
            });
            var dbProfile = Session.Get<Profile>(profile1.GlobalId);
            Assert.AreEqual(1, dbProfile.Statistics.VotingsCount);
        }

        [Test]
        public void TestVoteSupplementsCycleDefinition_Statistics()
        {
            var sup = CreateSupplement("sup");
            var def = CreateSupplementsCycleDefinition("def", sup, profiles[1]);
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.SupplementCycleDefinition;
                param.GlobalId = def.GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                service.Vote(data.Token, param);
            });
            var dbProfile = Session.Get<Profile>(profile1.GlobalId);
            Assert.AreEqual(1, dbProfile.Statistics.VotingsCount);
        }
        #endregion

        #region Send messages

        [Test]
        public void SendMessage_ActiveAccount()
        {
            profiles[0].Statistics.LastLoginDate = DateTime.UtcNow;
            insertToDatabase(profiles[0].Statistics);
            
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var prevCount = Session.QueryOver<Message>().RowCount();
            ExerciseDTO dto = Mapper.Map<Exercise, ExerciseDTO>(exercises["t"]);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.Exercise;
                param.GlobalId = dto.GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                dto.Rating = service.Vote(data.Token, param).Rating;
            });
            var newCount = Session.QueryOver<Message>().RowCount();
            Assert.AreEqual(prevCount+1,newCount);
        }

        [Test]
        public void SendMessage_NonActiveAccount()
        {
            profiles[0].Statistics.LastLoginDate = DateTime.UtcNow.AddMonths(-2);
            insertToDatabase(profiles[0].Statistics);
            
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var prevCount = Session.QueryOver<Message>().RowCount();
            ExerciseDTO dto = Mapper.Map<Exercise, ExerciseDTO>(exercises["t"]);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                VoteParams param = new VoteParams();
                param.ObjectType = VoteObject.Exercise;
                param.GlobalId = dto.GlobalId;
                param.UserRating = 3;
                param.UserShortComment = "test";
                dto.Rating = service.Vote(data.Token, param).Rating;
            });
            var newCount = Session.QueryOver<Message>().RowCount();
            Assert.AreEqual(prevCount, newCount);
        }

        #endregion
    }
}
