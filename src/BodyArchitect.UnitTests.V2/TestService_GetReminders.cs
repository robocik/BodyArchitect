using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using NUnit.Framework;
using ReminderRepetitions = BodyArchitect.Model.ReminderRepetitions;
using ReminderType = BodyArchitect.Model.ReminderType;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_GetReminders : TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        List<ReminderItem>  reminders = new List<ReminderItem>();

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                reminders.Clear();
                profiles.Add(CreateProfile(Session, "test1"));
                profiles.Add(CreateProfile(Session, "test2"));

                var reminder = CreateReminder("rem1", profiles[0], DateTime.UtcNow.Date.AddHours(3),null);
                reminders.Add(reminder);
                reminder = CreateReminder("rem2", profiles[0], DateTime.UtcNow.Date.AddDays(3), null,ReminderRepetitions.Once,ReminderType.Birthday);
                reminders.Add(reminder);
                reminder = CreateReminder("rem3", profiles[0], DateTime.UtcNow.Date.AddDays(8), null, ReminderRepetitions.Once, ReminderType.ScheduleEntry);
                reminders.Add(reminder);
                reminder = CreateReminder("rem4", profiles[0], DateTime.UtcNow.Date.AddDays(15), null);
                reminders.Add(reminder);
                reminder = CreateReminder("rem5", profiles[0], DateTime.UtcNow.Date.AddDays(30), null);
                reminders.Add(reminder);
                reminder = CreateReminder("rem6", profiles[0], DateTime.UtcNow.Date.AddMonths(2), null);
                reminders.Add(reminder);
                reminder = CreateReminder("rem7", profiles[0], DateTime.UtcNow.Date.AddMonths(4), null);
                reminders.Add(reminder);
                reminder = CreateReminder("rem8", profiles[0], DateTime.UtcNow.Date.AddMonths(6), null);
                reminders.Add(reminder);
                reminder = CreateReminder("rem9", profiles[0], DateTime.UtcNow.Date.AddMonths(1), TimeSpan.FromDays(3));
                reminders.Add(reminder);
                reminder = CreateReminder("rem10", profiles[0], DateTime.UtcNow.Date.AddMonths(3), TimeSpan.FromDays(7));
                reminders.Add(reminder);
                reminder = CreateReminder("rem11", profiles[0], DateTime.UtcNow.Date, null, ReminderRepetitions.EveryWeek);
                reminders.Add(reminder);
                reminder = CreateReminder("rem12", profiles[0], DateTime.UtcNow.Date.AddMonths(2), null, ReminderRepetitions.EveryWeek);
                reminders.Add(reminder);
                reminder = CreateReminder("rem13", profiles[0], DateTime.UtcNow.Date, null, ReminderRepetitions.EveryMonth);
                reminders.Add(reminder);
                reminder = CreateReminder("rem14", profiles[0], DateTime.UtcNow.Date, null, ReminderRepetitions.EveryYear);
                reminders.Add(reminder);
                tx.Commit();
            }
        }

        [Test]
        public void All()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                GetRemindersParam param = new GetRemindersParam();
                var list = service.GetReminders(data.Token, param,new PartialRetrievingInfo());

                Assert.AreEqual(reminders.Count, list.Items.Count);
            });
        }

        [Test]
        public void BirthdayOnly()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                GetRemindersParam param = new GetRemindersParam();
                param.Types.Add(Service.V2.Model.ReminderType.Birthday);
                var list = service.GetReminders(data.Token, param, new PartialRetrievingInfo());

                assert(list,1);
            });
        }

        [Test]
        public void BirthdayAndScheduleOnly()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                GetRemindersParam param = new GetRemindersParam();
                param.Types.Add(Service.V2.Model.ReminderType.Birthday);
                param.Types.Add(Service.V2.Model.ReminderType.ScheduleEntry);
                var list = service.GetReminders(data.Token, param, new PartialRetrievingInfo());

                assert(list, 1,2);
            });
        }

        void assert(PagedResult<ReminderItemDTO> pack, params int[] indexes)
        {
            Assert.AreEqual(indexes.Length, pack.Items.Count);
            foreach (var index in indexes)
            {
                Assert.IsNotNull(pack.Items.Where(x => x.GlobalId == reminders[index].GlobalId).SingleOrDefault(), "Index:" + index);
            }
        }
    }
}
