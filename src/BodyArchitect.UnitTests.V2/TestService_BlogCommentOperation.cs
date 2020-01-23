using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NUnit.Framework;
using MessagePriority = BodyArchitect.Model.MessagePriority;
using ProfileNotification = BodyArchitect.Model.ProfileNotification;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_TrainingDayCommentOperation : TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        private APIKey apiKey;
        private Guid key = Guid.NewGuid();

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                profiles.Add(CreateProfile(Session, "test1"));
                profiles.Add(CreateProfile(Session, "test2"));

                apiKey = new APIKey();
                apiKey.ApiKey = key;
                apiKey.ApplicationName = "UnitTest";
                apiKey.EMail = "mail@mail.com";
                apiKey.RegisterDateTime = DateTime.UtcNow;
                insertToDatabase(apiKey);
                tx.Commit();
            }
        }


        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void AddComment_AnotherProfile()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            day.AllowComments = true;
            day.AddEntry(blogEntry);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            data = CreateNewSession(profile1, ClientInformation);
            TrainingDayCommentDTO comment = new TrainingDayCommentDTO();
            comment.Profile = profile;
            comment.Comment = "msg";
            DateTime time = DateTime.UtcNow;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                TimerService.UtcNow = time;
                TrainingDayCommentOperationParam arg = new TrainingDayCommentOperationParam();
                arg.TrainingDayId = day.GlobalId;
                arg.Comment = comment;
                arg.OperationType = TrainingDayOperationType.Add;
                Service.TrainingDayCommentOperation(data.Token, arg);
            });
        }

        [Test]
        public void AddComment_SendMessage()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            day.AllowComments = true;
            day.AddEntry(blogEntry);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            data = CreateNewSession(profile1, ClientInformation);
            TrainingDayCommentDTO comment = new TrainingDayCommentDTO();
            comment.Profile = profile1;
            comment.Comment = "msg";
            DateTime time = DateTime.UtcNow;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                TimerService.UtcNow = time;
                TrainingDayCommentOperationParam arg = new TrainingDayCommentOperationParam();
                arg.TrainingDayId = day.GlobalId;
                arg.Comment = comment;
                arg.OperationType = TrainingDayOperationType.Add;
                Service.TrainingDayCommentOperation(data.Token, arg);
                Assert.IsFalse(((MockEmailService)Service.EMailService).EMailSent);
            });

            var messages = Session.QueryOver<Message>().Where(x => x.Receiver == profiles[0] ).SingleOrDefault();
            Assert.IsNotNull(messages);
            Assert.IsNotNull(messages.Content);
            Assert.IsFalse(messages.Content.Contains("BodyArchitect.Service.V2.Model.TrainingDayCommentDTO"));
            Assert.IsFalse(messages.Content.Contains("00:00:00"));
            Assert.AreEqual(MessagePriority.System, messages.Priority);
        }

        [Test]
        public void AddComment_SendEMail()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;

            profiles[0].Settings.NotificationBlogCommentAdded = ProfileNotification.Email;
            insertToDatabase(profiles[0]);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            day.AllowComments = true;
            day.AddEntry(blogEntry);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            data = CreateNewSession(profile1, ClientInformation);
            TrainingDayCommentDTO comment = new TrainingDayCommentDTO();
            comment.Profile = profile1;
            comment.Comment = "msg";
            DateTime time = DateTime.UtcNow;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                TimerService.UtcNow = time;
                TrainingDayCommentOperationParam arg = new TrainingDayCommentOperationParam();
                arg.TrainingDayId = day.GlobalId;
                arg.Comment = comment;
                arg.OperationType = TrainingDayOperationType.Add;
                Service.TrainingDayCommentOperation(data.Token, arg);
                Assert.True(((MockEmailService)Service.EMailService).EMailSent);
            });

            var count =Session.QueryOver<Message>().Where(x => x.Receiver == profiles[0] ).RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void AddComment_SendEMailAndMessage()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;

            profiles[0].Settings.NotificationBlogCommentAdded =ProfileNotification.Message| ProfileNotification.Email;
            insertToDatabase(profiles[0]);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            day.AllowComments = true;
            day.AddEntry(blogEntry);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            data = CreateNewSession(profile1, ClientInformation);
            TrainingDayCommentDTO comment = new TrainingDayCommentDTO();
            comment.Profile = profile1;
            comment.Comment = "msg";
            DateTime time = DateTime.UtcNow;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                TimerService.UtcNow = time;
                TrainingDayCommentOperationParam arg = new TrainingDayCommentOperationParam();
                arg.TrainingDayId = day.GlobalId;
                arg.Comment = comment;
                arg.OperationType = TrainingDayOperationType.Add;
                Service.TrainingDayCommentOperation(data.Token, arg);
                Assert.True(((MockEmailService)Service.EMailService).EMailSent);
            });

            var messages = Session.QueryOver<Message>().Where(x => x.Receiver == profiles[0]).SingleOrDefault();
            Assert.IsNotNull(messages);
            Assert.IsNotNull(messages.Content);
            Assert.AreEqual(MessagePriority.System, messages.Priority);
        }

        [Test]
        public void AddComment_DisableSendMessage()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;

            profiles[0].Settings.NotificationBlogCommentAdded = ProfileNotification.None;
            insertToDatabase(profiles[0]);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            day.AllowComments = true;
            day.AddEntry(blogEntry);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            data = CreateNewSession(profile1, ClientInformation);
            TrainingDayCommentDTO comment = new TrainingDayCommentDTO();
            comment.Profile = profile1;
            comment.Comment = "msg";
            DateTime time = DateTime.UtcNow;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                TimerService.UtcNow = time;
                TrainingDayCommentOperationParam arg = new TrainingDayCommentOperationParam();
                arg.TrainingDayId = day.GlobalId;
                arg.Comment = comment;
                arg.OperationType = TrainingDayOperationType.Add;
                Service.TrainingDayCommentOperation(data.Token, arg);
                Assert.IsFalse(((MockEmailService)Service.EMailService).EMailSent);
            });

            var count = Session.QueryOver<Message>().Where(x => x.Receiver == profiles[0]).RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void AddComment_SetLoginData()
        {

            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            day.AllowComments = true;
            day.AddEntry(blogEntry);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            LoginData loginData = new LoginData();
            loginData.ApiKey = apiKey;
            loginData.ApplicationVersion = "1.0.0";
            loginData.LoginDateTime = DateTime.UtcNow;
            loginData.ApplicationLanguage = "en";
            loginData.PlatformVersion = "NUnit";
            insertToDatabase(loginData);

            data = CreateNewSession(profile1, ClientInformation,loginData);
            TrainingDayCommentDTO comment = new TrainingDayCommentDTO();
            comment.Profile = profile1;
            comment.Comment = "msg";
            DateTime time = DateTime.UtcNow;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                TimerService.UtcNow = time;
                TrainingDayCommentOperationParam arg = new TrainingDayCommentOperationParam();
                arg.TrainingDayId = day.GlobalId;
                arg.Comment = comment;
                arg.OperationType = TrainingDayOperationType.Add;
                Service.TrainingDayCommentOperation(data.Token, arg);
            });

            var dbDay = Session.QueryOver<TrainingDay>().SingleOrDefault();
            Assert.AreEqual(loginData.ApiKey.ApiKey, dbDay.Comments.First().LoginData.ApiKey.ApiKey);
        }

        [Test]
        public void AddComment()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            day.AllowComments = true;
            day.AddEntry(blogEntry);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            data = CreateNewSession(profile1, ClientInformation);
            TrainingDayCommentDTO comment = new TrainingDayCommentDTO();
            comment.Profile = profile1;
            comment.Comment = "msg";
            DateTime time = DateTime.UtcNow;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
                                 {
                                     TimerService.UtcNow = time;
                TrainingDayCommentOperationParam arg = new TrainingDayCommentOperationParam();
                arg.TrainingDayId = day.GlobalId;
                arg.Comment = comment;
                arg.OperationType = TrainingDayOperationType.Add;
                Service.TrainingDayCommentOperation(data.Token, arg);
            });

           var dbDay = Session.QueryOver<TrainingDay>().SingleOrDefault();
           Assert.IsNotNull(dbDay);
           Assert.AreEqual(1, dbDay.CommentsCount);
           Assert.IsTrue(dbDay.LastCommentDate.Value.CompareDateTime(time));
        }

        [Test]
        public void AddTwoComments()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            day.AllowComments = true;
            day.AddEntry(blogEntry);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            
            TrainingDayCommentDTO comment = new TrainingDayCommentDTO();
            comment.Profile = profile;
            comment.Comment = "msg";
            DateTime time = DateTime.UtcNow;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
                                 {
                                     TimerService.UtcNow = time;
                TrainingDayCommentOperationParam arg = new TrainingDayCommentOperationParam();
                arg.TrainingDayId = day.GlobalId;
                arg.Comment = comment;
                arg.OperationType = TrainingDayOperationType.Add;
                Service.TrainingDayCommentOperation(data.Token, arg);
            });
            data = CreateNewSession(profile1, ClientInformation);
            comment = new TrainingDayCommentDTO();
            comment.Profile = profile1;
            comment.Comment = "msg1";
            time = DateTime.UtcNow.AddHours(2);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
                                 {
                                     TimerService.UtcNow = time;
                TrainingDayCommentOperationParam arg = new TrainingDayCommentOperationParam();
                arg.TrainingDayId = day.GlobalId;
                arg.Comment = comment;
                arg.OperationType = TrainingDayOperationType.Add;
                Service.TrainingDayCommentOperation(data.Token, arg);
            });

            var dbDay = Session.QueryOver<TrainingDay>().SingleOrDefault();
            Assert.IsNotNull(dbDay);
            Assert.AreEqual(2, dbDay.CommentsCount);
            Assert.IsTrue(dbDay.LastCommentDate.Value.CompareDateTime(time));
        }
    }
}
