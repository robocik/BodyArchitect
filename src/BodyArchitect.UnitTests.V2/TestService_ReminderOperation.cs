using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NUnit.Framework;
using ReminderRepetitions = BodyArchitect.Model.ReminderRepetitions;
using ReminderType = BodyArchitect.Model.ReminderType;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_ReminderOperation : TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();

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
        public void Delete_ReminderAttachedTOEntryObject()
        {
            var reminder = CreateReminder("test", profiles[0], DateTime.UtcNow,type:ReminderType.EntryObject);

            TrainingDay day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];
            SizeEntry sizeEntry = new SizeEntry();
            sizeEntry.Wymiary = new Wymiary();
            sizeEntry.Reminder = reminder;
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);
            insertToDatabase(day);
            reminder.ConnectedObject = "EntryObjectDTO:" + sizeEntry.GlobalId;
            insertToDatabase(reminder);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ReminderOperationParam param = new ReminderOperationParam();
                param.Operation = ReminderOperationType.Delete;
                param.ReminderItemId = reminder.GlobalId;
                var result = service.ReminderOperation(data.Token, param);
                Assert.IsNull(result);
            });
            Assert.AreEqual(0, Session.QueryOver<ReminderItem>().RowCount());
        }

        [Test]
        public void Delete_OneTimeReminder()
        {
            var reminder = CreateReminder("test",profiles[0],DateTime.UtcNow,null);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ReminderOperationParam param = new ReminderOperationParam();
                param.Operation = ReminderOperationType.Delete;
                param.ReminderItemId = reminder.GlobalId;
                var result = service.ReminderOperation(data.Token, param);
                Assert.IsNull(result);
            });
            Assert.AreEqual(0,Session.QueryOver<ReminderItem>().RowCount());
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void Delete_OneTimeReminder_AnotherProfile()
        {
            var reminder = CreateReminder("test", profiles[1], DateTime.UtcNow, null);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ReminderOperationParam param = new ReminderOperationParam();
                param.Operation = ReminderOperationType.Delete;
                param.ReminderItemId = reminder.GlobalId;
                service.ReminderOperation(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void CloseAfterShow_OneTimeReminder_AnotherProfile()
        {
            var reminder = CreateReminder("test", profiles[1], DateTime.UtcNow, null);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ReminderOperationParam param = new ReminderOperationParam();
                param.Operation = ReminderOperationType.CloseAfterShow;
                param.ReminderItemId = reminder.GlobalId;
                service.ReminderOperation(data.Token, param);
            });
        }

        [Test]
        public void CloseAfterShow_OneTimeReminder()
        {
            var reminder = CreateReminder("test", profiles[0], DateTime.UtcNow);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ReminderOperationParam param = new ReminderOperationParam();
                param.Operation = ReminderOperationType.CloseAfterShow;
                param.ReminderItemId = reminder.GlobalId;
                var result = service.ReminderOperation(data.Token, param);
                Assert.IsNull(result);
            });
            Assert.AreEqual(0, Session.QueryOver<ReminderItem>().RowCount());
        }

        [Test]
        public void Delete_PatternReminder()
        {
            var reminder = CreateReminder("test", profiles[0], DateTime.UtcNow, null,ReminderRepetitions.EveryYear);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ReminderOperationParam param = new ReminderOperationParam();
                param.Operation = ReminderOperationType.Delete;
                param.ReminderItemId = reminder.GlobalId;
                var result = service.ReminderOperation(data.Token, param);
                Assert.IsNull(result);
            });
            Assert.AreEqual(0, Session.QueryOver<ReminderItem>().RowCount());
        }

        [Test]
        public void CloseAfterShow_PatternReminder()
        {
            var reminder = CreateReminder("test", profiles[0], DateTime.UtcNow, null, ReminderRepetitions.EveryYear);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
                                 {
                TimerService.UtcNow = DateTime.UtcNow.AddDays(1).Date;
                ReminderOperationParam param = new ReminderOperationParam();
                param.Operation = ReminderOperationType.CloseAfterShow;
                param.ReminderItemId = reminder.GlobalId;
                var result = service.ReminderOperation(data.Token, param);
                Assert.IsNotNull(result);
                Assert.AreEqual(reminder.GlobalId,result.GlobalId);
                Assert.AreEqual(DateTime.UtcNow.AddDays(1).Date, result.LastShown.Value.Date);
            });
            var db = Session.QueryOver<ReminderItem>().SingleOrDefault();
            Assert.AreEqual(DateTime.UtcNow.AddDays(1).Date,db.LastShown.Value);
        }

        [Test]
        public void Delete_DataInfo()
        {
            var reminder = CreateReminder("test", profiles[0], DateTime.UtcNow, null);
            var oldHash = profiles[0].DataInfo.ReminderHash;
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ReminderOperationParam param = new ReminderOperationParam();
                param.Operation = ReminderOperationType.Delete;
                param.ReminderItemId = reminder.GlobalId;
                service.ReminderOperation(data.Token, param);
            });
            var dbItem = Session.Get<Profile>(profile.GlobalId);
            Assert.AreNotEqual(oldHash, dbItem.DataInfo.ReminderHash);
        }

        [Test]
        public void CloseAfterShow_OneTimeReminder_DataInfo()
        {
            var reminder = CreateReminder("test", profiles[0], DateTime.UtcNow);
            var oldHash = profiles[0].DataInfo.ReminderHash;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ReminderOperationParam param = new ReminderOperationParam();
                param.Operation = ReminderOperationType.CloseAfterShow;
                param.ReminderItemId = reminder.GlobalId;
                service.ReminderOperation(data.Token, param);
            });
            var dbItem = Session.Get<Profile>(profile.GlobalId);
            Assert.AreNotEqual(oldHash, dbItem.DataInfo.ReminderHash);
        }

        [Test]
        public void CloseAfterShow_PatternReminder_DataInfo()
        {
            var reminder = CreateReminder("test", profiles[0], DateTime.UtcNow, null, ReminderRepetitions.EveryYear);
            var oldHash = profiles[0].DataInfo.ReminderHash;
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                TimerService.UtcNow = DateTime.UtcNow.AddDays(1).Date;
                ReminderOperationParam param = new ReminderOperationParam();
                param.Operation = ReminderOperationType.CloseAfterShow;
                param.ReminderItemId = reminder.GlobalId;
                service.ReminderOperation(data.Token, param);
                
            });
            var dbItem = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(oldHash, dbItem.DataInfo.ReminderHash);
        }
    }
}
