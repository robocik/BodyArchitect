using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Shared;
using NHibernate;
using NUnit.Framework;
using ReminderType = BodyArchitect.Model.ReminderType;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_SaveReminders : TestServiceBase
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

        #region Save/Update
        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void UpdateReminder_OtherProfile()
        {
            var reminder = CreateReminder("test", profiles[0],DateTime.UtcNow.AddDays(1));
            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var dto = reminder.Map<ReminderItemDTO>();
            dto.Name = "name";
            dto.ProfileId = profile.GlobalId;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveReminder(data.Token, dto);
            });

        }

        [Test]
        [ExpectedException(typeof(StaleObjectStateException))]
        public void UpdateReminder_OldVersion()
        {
            var reminder = CreateReminder("test", profiles[0], DateTime.UtcNow.AddDays(1));

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var dto = reminder.Map<ReminderItemDTO>();
            dto.Version = 0;//old version 
            dto.Name = "name";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveReminder(data.Token, dto);
            });

        }

        [Test]
        public void UpdateReminder()
        {
            var reminder = CreateReminder("test", profiles[0], DateTime.UtcNow.AddDays(1));
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var dto = reminder.Map<ReminderItemDTO>();
            dto.Name = "name";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var res = service.SaveReminder(data.Token, dto);
                UnitTestHelper.CompareObjects(res, dto, true);
            });
            var dbItem = Session.QueryOver<ReminderItem>().SingleOrDefault();
            Assert.AreEqual(ReminderType.Custom, dbItem.Type);

        }

        [Test]
        [ExpectedException(typeof(UniqueException))]
        public void SaveReminder_Validation_UniqueName()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var reminder = new ReminderItemDTO();
            reminder.Name = "name";
            reminder.DateTime = DateTime.UtcNow.AddDays(2);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveReminder(data.Token, reminder);
            });

            reminder = new ReminderItemDTO();
            reminder.Name = "name";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveReminder(data.Token, reminder);
            });
        }

        [Test]
        public void SaveReminder_Validation_UniqueName_DifferentUser()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;

            SessionData data = CreateNewSession(profile, ClientInformation);
            var reminder = new ReminderItemDTO();
            reminder.Name = "name";
            reminder.DateTime = DateTime.UtcNow;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveReminder(data.Token, reminder);
            });
            data = CreateNewSession(profile1, ClientInformation);
            reminder = new ReminderItemDTO();
            reminder.Name = "name";
            reminder.DateTime = DateTime.UtcNow;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveReminder(data.Token, reminder);
            });
            Assert.AreEqual(2, Session.QueryOver<ReminderItem>().RowCount());
        }

        [Test]
        public void SaveNewReminder()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var reminder = new ReminderItemDTO();
            reminder.Name = "name";
            reminder.DateTime = DateTime.UtcNow;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var res = service.SaveReminder(data.Token, reminder);
                Assert.AreNotEqual(Guid.Empty, res.GlobalId);
                reminder.ProfileId = res.ProfileId;
                UnitTestHelper.CompareObjects(res,reminder,true);
            });
            var dbItem=Session.QueryOver<ReminderItem>().SingleOrDefault();
            Assert.AreEqual(ReminderType.Custom, dbItem.Type);

        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SaveNewReminder_RemindBeforeMoreThanOneWeek()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var reminder = new ReminderItemDTO();
            reminder.Name = "name";
            reminder.RemindBefore = TimeSpan.FromDays(8);
            reminder.DateTime = DateTime.UtcNow;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var res = service.SaveReminder(data.Token, reminder);
                Assert.AreNotEqual(Guid.Empty, res.GlobalId);
                reminder.ProfileId = res.ProfileId;
                UnitTestHelper.CompareObjects(res, reminder, true);
            });
            var dbItem = Session.QueryOver<ReminderItem>().SingleOrDefault();
            Assert.AreEqual(ReminderType.Custom, dbItem.Type);

        }

        #endregion


        [Test]
        public void SaveNewReminder_DataInfo()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var oldHash = profiles[0].DataInfo.ReminderHash;

            var reminder = new ReminderItemDTO();
            reminder.Name = "name";
            reminder.DateTime = DateTime.UtcNow;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var res = service.SaveReminder(data.Token, reminder);
                Assert.AreNotEqual(Guid.Empty, res.GlobalId);
                reminder.ProfileId = res.ProfileId;
                UnitTestHelper.CompareObjects(res, reminder, true);
            });
            var dbItem = Session.Get<Profile>(profile.GlobalId);
            Assert.AreNotEqual(dbItem, dbItem.DataInfo.ReminderHash);

        }
    }
}
