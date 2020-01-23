using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Shared;
using NUnit.Framework;
using ChampionshipType = BodyArchitect.Service.V2.Model.ChampionshipType;
using ReminderRepetitions = BodyArchitect.Model.ReminderRepetitions;
using ReminderType = BodyArchitect.Model.ReminderType;
using ScheduleEntryState = BodyArchitect.Model.ScheduleEntryState;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_SaveDeleteScheduleEntry : TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        List<Activity> activities = new List<Activity>();
        private Guid key = Guid.NewGuid();
        private APIKey apiKey;

        public override void BuildDatabase()
        {
            profiles.Clear();
            activities.Clear();
            using (var tx = Session.BeginTransaction())
            {
                var profile = CreateProfile(Session, "Profile1");
                profiles.Add(profile);
                profile = CreateProfile(Session, "Profile2");
                profiles.Add(profile);
                Activity activity = CreateActivity("fitness", profiles[0], 0,TimeSpan.FromHours(1));
                activities.Add(activity);
                activity = CreateActivity("swimming", profiles[0],0, TimeSpan.FromMinutes(90));
                activities.Add(activity);
                activity = CreateActivity("aqua", profiles[0],0, TimeSpan.FromMinutes(60));
                activities.Add(activity);

                apiKey = new APIKey();
                apiKey.ApiKey = key;
                apiKey.ApplicationName = "UnitTest";
                apiKey.EMail = "mail@mail.com";
                apiKey.RegisterDateTime = DateTime.UtcNow;
                insertToDatabase(apiKey);
                tx.Commit();
            }
        }

        #region Save

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void SaveScheduleEntry_WithCustomerGroup_AnotherProfile()
        {
            var group = CreateCustomerGroup("custGroup", profiles[1]);

            var entry = new ScheduleEntryDTO();
            entry.StartTime = DateTime.UtcNow.Date.AddHours(7);
            entry.EndTime = entry.StartTime.AddMinutes(90);
            entry.ActivityId = activities[0].GlobalId;
            entry.CustomerGroupId = group.GlobalId;
            entry.MaxPersons = 3;

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                entry = service.SaveScheduleEntry(data.Token, entry);
                Session.Clear();
                Assert.AreEqual(group.GlobalId, entry.CustomerGroupId);
            });
            Assert.AreEqual(group.GlobalId, Session.Get<ScheduleEntry>(entry.GlobalId).CustomerGroup.GlobalId);
        }

        [Test]
        public void SaveScheduleEntry_WithCustomerGroup()
        {
            var group=CreateCustomerGroup("custGroup", profiles[0]);

            var entry = new ScheduleEntryDTO();
            entry.StartTime = DateTime.UtcNow.Date.AddHours(7);
            entry.EndTime = entry.StartTime.AddMinutes(90);
            entry.ActivityId = activities[0].GlobalId;
            entry.CustomerGroupId = group.GlobalId;
            entry.MaxPersons = 3;

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                entry = service.SaveScheduleEntry(data.Token, entry);
                Session.Clear();
                Assert.AreEqual(group.GlobalId, entry.CustomerGroupId);
            });
            Assert.AreEqual(group.GlobalId, Session.Get<ScheduleEntry>(entry.GlobalId).CustomerGroup.GlobalId);
        }

        [Test]
        public void UpdateScheduleEntry_WithCustomerGroup()
        {
            var group = CreateCustomerGroup("custGroup", profiles[0]);

            var entry = new ScheduleEntryDTO();
            entry.StartTime = DateTime.UtcNow.Date.AddHours(7);
            entry.EndTime = entry.StartTime.AddMinutes(90);
            entry.ActivityId = activities[0].GlobalId;
            
            entry.MaxPersons = 3;

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                entry = service.SaveScheduleEntry(data.Token, entry);
            });
            entry.EndTime = entry.StartTime.AddMinutes(100);
            entry.CustomerGroupId = group.GlobalId;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                entry = service.SaveScheduleEntry(data.Token, entry);
                Assert.AreEqual(group.GlobalId, entry.CustomerGroupId);
            });

            Assert.AreEqual(group.GlobalId, Session.Get<ScheduleEntry>(entry.GlobalId).CustomerGroup.GlobalId);
        }

        [Test]
        public void SaveScheduleEntry()
        {
            var entry = new ScheduleEntryDTO();
            entry.StartTime = DateTime.UtcNow.Date.AddHours(7);
            entry.EndTime = entry.StartTime.AddMinutes(90);
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 3;

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                entry = service.SaveScheduleEntry(data.Token,entry);
                Session.Clear();
                Assert.AreEqual(1, Session.QueryOver<ScheduleEntry>().RowCount());
                Assert.AreEqual(profiles[0].GlobalId, Session.Get<ScheduleEntry>(entry.GlobalId).Profile.GlobalId);
            });
        }

        [Test]
        public void SaveScheduleEntry_DefaultMyPlace()
        {
            var entry = new ScheduleEntryDTO();
            entry.StartTime = DateTime.UtcNow.Date.AddHours(7);
            entry.EndTime = entry.StartTime.AddMinutes(90);
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 3;

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                entry = service.SaveScheduleEntry(data.Token, entry);
                Session.Clear();
                Assert.AreEqual(1, Session.QueryOver<ScheduleEntry>().RowCount());
                Assert.AreEqual(GetDefaultMyPlace(profiles[0]).GlobalId, Session.Get<ScheduleEntry>(entry.GlobalId).MyPlace.GlobalId);
            });
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void SaveScheduleEntry_MyPlaceFromAnotherProfile()
        {
            var entry = new ScheduleEntryDTO();
            entry.StartTime = DateTime.UtcNow.Date.AddHours(7);
            entry.EndTime = entry.StartTime.AddMinutes(90);
            entry.ActivityId = activities[0].GlobalId;
            entry.MyPlaceId = GetDefaultMyPlace(profiles[1]).GlobalId;
            entry.MaxPersons = 3;

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveScheduleEntry(data.Token, entry);
            });
        }

        [Test]
        public void UpdateScheduleEntry()
        {
            var entry = new ScheduleEntryDTO();
            entry.StartTime = DateTime.UtcNow.Date.AddHours(7);
            entry.EndTime = entry.StartTime.AddMinutes(90);
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 3;

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                entry = service.SaveScheduleEntry(data.Token, entry);
            });
            entry.EndTime = entry.StartTime.AddMinutes(100);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                entry = service.SaveScheduleEntry(data.Token, entry);
            });

            Assert.AreEqual(1, Session.QueryOver<ScheduleEntry>().RowCount());
            var db = Session.Get<ScheduleEntry>(entry.GlobalId);
            Assert.AreEqual(entry.StartTime.AddMinutes(100), db.EndTime);
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void UpdateScheduleEntry_ActivityOfAnotherUser()
        {
            var anotherUserActivity=CreateActivity("fitness", profiles[1]);

            var entry = new ScheduleEntryDTO();
            entry.StartTime = DateTime.UtcNow.Date.AddHours(7);
            entry.EndTime = entry.StartTime.AddMinutes(90);
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 3;

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                entry = service.SaveScheduleEntry(data.Token, entry);
            });
            entry.ActivityId = anotherUserActivity.GlobalId;
            entry.EndTime = entry.StartTime.AddMinutes(100);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                entry = service.SaveScheduleEntry(data.Token, entry);
            });

            Assert.AreEqual(1, Session.QueryOver<ScheduleEntry>().RowCount());
            var db = Session.Get<ScheduleEntry>(entry.GlobalId);
            Assert.AreEqual(entry.StartTime.AddMinutes(100), db.EndTime);
        }

        [Test]
        [ExpectedException(typeof(AlreadyOccupiedException))]
        public void SaveTwoScheduleEntry_SameProfileSameTime()
        {
            var entry = new ScheduleEntryDTO();
            entry.StartTime = DateTime.UtcNow.Date.AddHours(7);
            entry.EndTime = entry.StartTime.AddMinutes(90);
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 3;

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveScheduleEntry(data.Token,entry);

            });

            entry = new ScheduleEntryDTO();
            entry.StartTime = DateTime.UtcNow.Date.AddHours(7);
            entry.EndTime = entry.StartTime.AddMinutes(90);
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 3;

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveScheduleEntry(data.Token,entry);
            });
        }

        [Test]
        public void SaveTwoScheduleEntry_DifferentProfileSameTime()
        {
            var anotherUserActivity = CreateActivity("fitness", profiles[1]);

            var entry = new ScheduleEntryDTO();
            entry.StartTime = DateTime.UtcNow.Date.AddHours(7);
            entry.EndTime = entry.StartTime.AddMinutes(90);
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 3;

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveScheduleEntry(data.Token, entry);

            });

            var profile2 = (ProfileDTO)profiles[1].Tag;
            data = CreateNewSession(profile2, ClientInformation);
            entry = new ScheduleEntryDTO();
            entry.StartTime = DateTime.UtcNow.Date.AddHours(7);
            entry.EndTime = entry.StartTime.AddMinutes(90);
            entry.ActivityId = anotherUserActivity.GlobalId;
            entry.MaxPersons = 3;

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveScheduleEntry(data.Token, entry);
            });
            Assert.AreEqual(2,Session.QueryOver<ScheduleEntry>().RowCount());
        }

        [Test]
        [ExpectedException(typeof(AlreadyOccupiedException))]
        public void SaveTwoScheduleEntry_SameTime1()
        {
            var entry = new ScheduleEntryDTO();
            entry.StartTime = DateTime.UtcNow.Date.AddHours(7);
            entry.EndTime = entry.StartTime.AddMinutes(90);
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 3;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveScheduleEntry(data.Token, entry);
            });

            entry = new ScheduleEntryDTO();
            entry.StartTime = DateTime.UtcNow.Date.AddHours(6);
            entry.EndTime = entry.StartTime.AddMinutes(90);
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 3;

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveScheduleEntry(data.Token, entry);
            });
        }

        [Test]
        public void SaveTwoScheduleEntry_SameProfileTimeOneAfterOne()
        {
            var entry = new ScheduleEntryDTO();
            entry.StartTime = DateTime.UtcNow.Date.AddHours(7);
            entry.EndTime = entry.StartTime.AddMinutes(60);
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 3;

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveScheduleEntry(data.Token,entry);

            });

            entry = new ScheduleEntryDTO();
            entry.StartTime = DateTime.UtcNow.Date.AddHours(8);
            entry.EndTime = entry.StartTime.AddMinutes(90);
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 3;

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveScheduleEntry(data.Token,entry);
            });
        }

        #endregion

        #region Delete

        [Test]
        public void DeleteScheduleEntry()
        {
            var entry = new ScheduleEntry();
            entry.StartTime = DateTime.UtcNow.Date.AddHours(7);
            entry.EndTime = entry.StartTime.AddMinutes(90);
            entry.MyPlace = GetDefaultMyPlace(profiles[0]);
            entry.Activity = activities[0];
            entry.Profile = profiles[0];
            entry.MaxPersons = 3;
            insertToDatabase(entry);

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteScheduleEntry(data.Token, entry.Map<ScheduleEntryDTO>());
            });
            Assert.AreEqual(0, Session.QueryOver<ScheduleEntry>().RowCount());
            Assert.AreEqual(3, Session.QueryOver<Activity>().RowCount());
            Assert.AreEqual(3, Session.QueryOver<Profile>().RowCount());
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void DeleteScheduleEntry_OtherProfile()
        {
            var entry = new ScheduleEntry();
            entry.StartTime = DateTime.UtcNow.Date.AddHours(7);
            entry.EndTime = entry.StartTime.AddMinutes(90);
            entry.Activity = activities[0];
            entry.MyPlace = GetDefaultMyPlace(profiles[0]);
            entry.Profile = profiles[0];
            entry.MaxPersons = 3;
            insertToDatabase(entry);

            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteScheduleEntry(data.Token, entry.Map<ScheduleEntryDTO>());
            });
        }
        #endregion

        #region Save range

        [Test]
        public void SaveScheduleEntryRange()
        {
            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.Date.AddDays(7);
            var entry = new ScheduleEntryDTO();
            entry.StartTime = param.StartDay.AddHours(7);
            entry.EndTime = entry.StartTime.AddMinutes(90);
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 3;
            param.Entries.Add(entry);
            entry = new ScheduleEntryDTO();
            entry.StartTime = param.StartDay.AddHours(10);
            entry.EndTime = entry.StartTime.AddMinutes(60);
            entry.ActivityId = activities[1].GlobalId;
            entry.MaxPersons = 13;
            param.Entries.Add(entry);
            entry = new ScheduleEntryDTO();
            entry.StartTime = param.StartDay.AddDays(2).AddHours(8);
            entry.EndTime = entry.StartTime.AddMinutes(60);
            entry.ActivityId = activities[1].GlobalId;
            entry.MaxPersons = 13;
            param.Entries.Add(entry);
            entry = new ScheduleEntryDTO();
            entry.StartTime = param.StartDay.AddDays(6).AddHours(8);
            entry.EndTime = entry.StartTime.AddMinutes(60);
            entry.ActivityId = activities[2].GlobalId;
            entry.MaxPersons = 13;
            param.Entries.Add(entry);

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            IList<ScheduleEntryBaseDTO> list=null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                list = service.SaveScheduleEntriesRange(data.Token, param);
                Assert.AreNotEqual(Guid.Empty,list[0].GlobalId);
            });
            var dbResult=Session.QueryOver<ScheduleEntry>().List();
            Assert.AreEqual(4, dbResult.Count);
            UnitTestHelper.CompareObjects(list, dbResult.Map<IList<ScheduleEntryBaseDTO>>(),true);
        }

        [Test]
        public void SaveScheduleEntryRange_WithReminder()
        {
            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.Date.AddDays(7);
            var entry = new ScheduleEntryDTO();
            entry.StartTime = param.StartDay.AddHours(7);
            entry.EndTime = entry.StartTime.AddMinutes(90);
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 3;
            entry.RemindBefore = TimeSpan.FromMinutes(30);
            param.Entries.Add(entry);
            entry = new ScheduleEntryDTO();
            entry.StartTime = param.StartDay.AddHours(10);
            entry.EndTime = entry.StartTime.AddMinutes(60);
            entry.ActivityId = activities[1].GlobalId;
            entry.MaxPersons = 13;
            param.Entries.Add(entry);

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            IList<ScheduleEntryBaseDTO> list = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                list = service.SaveScheduleEntriesRange(data.Token, param);
                Assert.AreEqual(TimeSpan.FromMinutes(30),list[0].RemindBefore);
                Assert.AreEqual(null, list[1].RemindBefore);
            });
            var dbResult = Session.QueryOver<ScheduleEntry>().List();
            Assert.AreEqual(2, dbResult.Count);
            Assert.AreEqual(dbResult[0].StartTime, dbResult[0].Reminder.DateTime);
            Assert.AreEqual(TimeSpan.FromMinutes(30), dbResult[0].Reminder.RemindBefore);
            Assert.AreEqual("ScheduleEntryDTO:" + dbResult[0].GlobalId, dbResult[0].Reminder.ConnectedObject);
            Assert.AreEqual(ReminderRepetitions.Once, dbResult[0].Reminder.Repetitions);
            Assert.AreEqual(profiles[0].GlobalId, dbResult[0].Reminder.Profile.GlobalId);
            Assert.AreEqual(ReminderType.ScheduleEntry, dbResult[0].Reminder.Type);
            Assert.AreEqual(1, Session.QueryOver<ReminderItem>().RowCount());

            Assert.IsNull(dbResult[1].Reminder);
            
        }

        [Test]
        public void SaveScheduleEntriesRange_Update_RemoveReminder()
        {
            var se1 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(4));
            var se2 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(6));
            var se3 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddDays(2).AddHours(4));
            se3.Reminder = new ReminderItem();
            se3.Reminder.DateTime = DateTime.UtcNow;
            se3.Reminder.Name = "test";
            insertToDatabase(se3);


            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.Date.AddDays(7);
            param.Entries.Add(se1.Map<ScheduleEntryDTO>());
            param.Entries.Add(se2.Map<ScheduleEntryDTO>());
            var dto = se3.Map<ScheduleEntryDTO>();
            dto.RemindBefore = null;
            param.Entries.Add(dto);

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            IList<ScheduleEntryBaseDTO> list = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                list = service.SaveScheduleEntriesRange(data.Token, param);
            });
            var dto3 = list.Where(x => x.GlobalId == se3.GlobalId).SingleOrDefault();
            Assert.IsNull( dto3.RemindBefore);

            var dbResult = Session.QueryOver<ScheduleEntry>().List();
            var db = dbResult.Where(x => x.GlobalId == dto3.GlobalId).Single();
            Assert.IsNull(db.Reminder);
            Assert.AreEqual(0, Session.QueryOver<ReminderItem>().RowCount());
        }

        [Test]
        public void SaveScheduleEntriesRange_Update_ChangeReminder()
        {
            var se1 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(4));
            var se2 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(6));
            var se3 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddDays(2).AddHours(4));
            se3.Reminder = new ReminderItem();
            se3.Reminder.DateTime = DateTime.UtcNow;
            se3.Reminder.Name = "test";
            insertToDatabase(se3);


            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.Date.AddDays(7);
            param.Entries.Add(se1.Map<ScheduleEntryDTO>());
            param.Entries.Add(se2.Map<ScheduleEntryDTO>());
            var dto = se3.Map<ScheduleEntryDTO>();
            dto.RemindBefore = TimeSpan.FromMinutes(30);
            param.Entries.Add(dto);

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            IList<ScheduleEntryBaseDTO> list = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                list = service.SaveScheduleEntriesRange(data.Token, param);
            });
            var dto3 = list.Where(x => x.GlobalId == se3.GlobalId).SingleOrDefault();
            var dbResult = Session.Get<ScheduleEntry>(dto3.GlobalId);
            Assert.AreEqual(dbResult.StartTime, dbResult.Reminder.DateTime);
            Assert.AreEqual(TimeSpan.FromMinutes(30), dbResult.Reminder.RemindBefore);
            Assert.AreEqual("ScheduleEntryDTO:" + dbResult.GlobalId, dbResult.Reminder.ConnectedObject);
            Assert.AreEqual(ReminderRepetitions.Once, dbResult.Reminder.Repetitions);
            Assert.AreEqual(profiles[0].GlobalId, dbResult.Reminder.Profile.GlobalId);
            Assert.AreEqual(ReminderType.ScheduleEntry, dbResult.Reminder.Type);
            Assert.AreEqual(1, Session.QueryOver<ReminderItem>().RowCount());
        }

        [Test]
        [ExpectedException(typeof(AlreadyOccupiedException))]
        public void SaveScheduleEntriesRange_TwoAddedWithTheSameTime()
        {
            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.Date.AddDays(7);
            var entry = new ScheduleEntryDTO();
            entry.StartTime = param.StartDay.AddHours(7);
            entry.EndTime = entry.StartTime.AddMinutes(90);
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 3;
            param.Entries.Add(entry);
            entry = new ScheduleEntryDTO();
            entry.StartTime = param.StartDay.AddHours(8);
            entry.EndTime = entry.StartTime.AddMinutes(60);
            entry.ActivityId = activities[1].GlobalId;
            entry.MaxPersons = 13;
            param.Entries.Add(entry);
            entry = new ScheduleEntryDTO();
            entry.StartTime = param.StartDay.AddDays(2).AddHours(8);
            entry.EndTime = entry.StartTime.AddMinutes(60);
            entry.ActivityId = activities[1].GlobalId;
            entry.MaxPersons = 13;
            param.Entries.Add(entry);
            entry = new ScheduleEntryDTO();
            entry.StartTime = param.StartDay.AddDays(6).AddHours(8);
            entry.EndTime = entry.StartTime.AddMinutes(60);
            entry.ActivityId = activities[2].GlobalId;
            entry.MaxPersons = 13;
            param.Entries.Add(entry);

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveScheduleEntriesRange(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(AlreadyOccupiedException))]
        public void SaveScheduleEntriesRange_UpdateWithTheSameTime()
        {
            var se1 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(4));
            var se2 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(6));
            var se3 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddDays(2).AddHours(4));

            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.Date.AddDays(7);
            param.Entries.Add(se1.Map<ScheduleEntryDTO>());
            param.Entries.Add(se2.Map<ScheduleEntryDTO>());
            var dto = se3.Map<ScheduleEntryDTO>();
            dto.StartTime = DateTime.UtcNow.Date.AddHours(4);
            dto.EndTime = dto.StartTime.AddMinutes(30);
            param.Entries.Add(dto);

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveScheduleEntriesRange(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(AlreadyOccupiedException))]
        public void SaveScheduleEntriesRange_AddWithTheSameTime()
        {
            var se1 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(4));
            var se2 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(6));
            var se3 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddDays(2).AddHours(4));

            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.Date.AddDays(7);
            param.Entries.Add(se1.Map<ScheduleEntryDTO>());
            param.Entries.Add(se2.Map<ScheduleEntryDTO>());
            param.Entries.Add(se3.Map<ScheduleEntryDTO>());
            var entry = new ScheduleEntryDTO();
            entry.StartTime = param.StartDay.AddHours(6);
            entry.EndTime = entry.StartTime.AddMinutes(45);
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 3;
            param.Entries.Add(entry);

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveScheduleEntriesRange(data.Token, param);
            });
        }

        ScheduleEntry CreateScheduleEntry(Profile profile,DateTime startTime,Activity activity=null,TimeSpan? duration=null)
        {
            var scheduleEntry = new ScheduleEntry();
            scheduleEntry.MyPlace = GetDefaultMyPlace(profile);
            scheduleEntry.Profile = profile;
            scheduleEntry.StartTime = startTime;
            if(duration==null)
            {
                duration = TimeSpan.FromMinutes(60);
            }
            scheduleEntry.EndTime =scheduleEntry.StartTime+ duration.Value;
            if(activity==null)
            {
                activity = activities[0];
            }
            scheduleEntry.Activity = activity;
            insertToDatabase(scheduleEntry);
            return scheduleEntry;
        }

        [Test]
        public void SaveScheduleEntriesRange_Delete()
        {
            var se1 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(4));
            var se2 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(6));
            var se3 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddDays(2).AddHours(4));

            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.Date.AddDays(7);
            param.Entries.Add(se1.Map<ScheduleEntryDTO>());
            param.Entries.Add(se3.Map<ScheduleEntryDTO>());

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            IList<ScheduleEntryBaseDTO> list = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                list = service.SaveScheduleEntriesRange(data.Token, param);
            });
            var dbResult = Session.QueryOver<ScheduleEntry>().List();
            Assert.AreEqual(2, dbResult.Count);
            UnitTestHelper.CompareObjects(list, dbResult.Map<IList<ScheduleEntryBaseDTO>>());
            var dto1=list.Where(x => x.GlobalId == se1.GlobalId).SingleOrDefault();
            var dto2 = list.Where(x => x.GlobalId == se2.GlobalId).SingleOrDefault();
            var dto3 = list.Where(x => x.GlobalId == se3.GlobalId).SingleOrDefault();
            Assert.IsNotNull(dto1);
            Assert.IsNull(dto2);
            Assert.IsNotNull(dto3);
        }

        [Test]
        [ExpectedException(typeof(DeleteConstraintException))]
        public void SaveScheduleEntriesRange_DeleteWithReservations()
        {
            var customer = CreateCustomer("cust1", profiles[0]);
            var se1 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(4));
            var se2 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(6));
            var se3 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddDays(2).AddHours(4));
            var reservation = new ScheduleEntryReservation();
            reservation.ScheduleEntry = se2;
            reservation.Name = "name";
            reservation.Customer = customer;
            reservation.DateTime = DateTime.Now;
            reservation.Profile = profiles[0];
            se2.Reservations.Add(reservation);
            insertToDatabase(reservation);

            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.Date.AddDays(7);
            param.Entries.Add(se1.Map<ScheduleEntryDTO>());
            param.Entries.Add(se3.Map<ScheduleEntryDTO>());

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveScheduleEntriesRange(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(DeleteConstraintException))]
        public void SaveScheduleEntriesRange_DeleteWithDoneStatus()
        {
            var se1 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(4));
            var se2 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(6));
            var se3 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddDays(2).AddHours(4));
            se2.State = ScheduleEntryState.Done;
            insertToDatabase(se2);
            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.Date.AddDays(7);
            param.Entries.Add(se1.Map<ScheduleEntryDTO>());
            param.Entries.Add(se3.Map<ScheduleEntryDTO>());

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveScheduleEntriesRange(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(DeleteConstraintException))]
        public void SaveScheduleEntriesRange_DeleteWithCancelledStatus()
        {
            var se1 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(4));
            var se2 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(6));
            var se3 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddDays(2).AddHours(4));
            se2.State = ScheduleEntryState.Cancelled;
            insertToDatabase(se2);
            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.Date.AddDays(7);
            param.Entries.Add(se1.Map<ScheduleEntryDTO>());
            param.Entries.Add(se3.Map<ScheduleEntryDTO>());

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveScheduleEntriesRange(data.Token, param);
            });
        }

        [Test]
        public void SaveScheduleEntriesRange_Add()
        {
            var se1 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(4));
            var se2 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(6));
            var se3 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddDays(2).AddHours(4));

            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.Date.AddDays(7);
            param.Entries.Add(se1.Map<ScheduleEntryDTO>());
            param.Entries.Add(se2.Map<ScheduleEntryDTO>());
            param.Entries.Add(se3.Map<ScheduleEntryDTO>());
            var entry = new ScheduleEntryDTO();
            entry.StartTime = param.StartDay.AddHours(7);
            entry.EndTime = entry.StartTime.AddMinutes(90);
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 3;
            param.Entries.Add(entry);

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            IList<ScheduleEntryBaseDTO> list = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                list = service.SaveScheduleEntriesRange(data.Token, param);
            });
            var dbResult = Session.QueryOver<ScheduleEntry>().List();
            Assert.AreEqual(4, dbResult.Count);
            UnitTestHelper.CompareObjects(list, dbResult.Map<IList<ScheduleEntryBaseDTO>>());
            var dto1 = list.Where(x => x.GlobalId == se1.GlobalId).SingleOrDefault();
            var dto2 = list.Where(x => x.GlobalId == se2.GlobalId).SingleOrDefault();
            var dto3 = list.Where(x => x.GlobalId == se3.GlobalId).SingleOrDefault();
            var dto4 = list.Where(x => x.GlobalId == se3.GlobalId).SingleOrDefault();
            Assert.IsNotNull(dto1);
            Assert.IsNotNull(dto2);
            Assert.IsNotNull(dto3);
            Assert.IsNotNull(dto4);
        }

        [Test]
        public void SaveScheduleEntriesRange_SetDefaultMyPlace()
        {
            var se1 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(4));
            var se2 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(6));
            var se3 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddDays(2).AddHours(4));

            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.Date.AddDays(7);
            param.Entries.Add(se1.Map<ScheduleEntryDTO>());
            param.Entries.Add(se2.Map<ScheduleEntryDTO>());
            param.Entries.Add(se3.Map<ScheduleEntryDTO>());
            var entry = new ScheduleEntryDTO();
            entry.StartTime = param.StartDay.AddHours(7);
            entry.EndTime = entry.StartTime.AddMinutes(90);
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 3;
            param.Entries.Add(entry);

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            IList<ScheduleEntryBaseDTO> list = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                list = service.SaveScheduleEntriesRange(data.Token, param);
            });
            var dbResult = Session.QueryOver<ScheduleEntry>().List();
            var defaultMyPlace=GetDefaultMyPlace(profiles[0]);
            Assert.AreEqual(4, dbResult.Count);
            Assert.AreEqual(defaultMyPlace, dbResult[0].MyPlace);
            Assert.AreEqual(defaultMyPlace, dbResult[1].MyPlace);
            Assert.AreEqual(defaultMyPlace, dbResult[2].MyPlace);
            Assert.AreEqual(defaultMyPlace, dbResult[3].MyPlace);

        }

        [Test]
        public void SaveScheduleEntriesRange_WithCustomer()
        {
            var group = CreateCustomerGroup("custGroup", profiles[0]);
            var se1 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(4));
            var se2 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(6));
            var se3 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddDays(2).AddHours(4));

            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.Date.AddDays(7);
            var dto = se1.Map<ScheduleEntryDTO>();
            dto.CustomerGroupId = group.GlobalId;
            param.Entries.Add(dto);
            param.Entries.Add(se2.Map<ScheduleEntryDTO>());
            param.Entries.Add(se3.Map<ScheduleEntryDTO>());
            var entry = new ScheduleEntryDTO();
            entry.StartTime = param.StartDay.AddHours(7);
            entry.EndTime = entry.StartTime.AddMinutes(90);
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 3;
            param.Entries.Add(entry);

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var list = service.SaveScheduleEntriesRange(data.Token, param);
                entry = (ScheduleEntryDTO) list.Where(x => x.StartTime == DateTime.UtcNow.Date.AddHours(4)).SingleOrDefault();
                Assert.AreEqual(group.GlobalId, entry.CustomerGroupId);
            });
            var dbResult = Session.Get<ScheduleEntry>(entry.GlobalId);
            Assert.AreEqual(group.GlobalId, dbResult.CustomerGroup.GlobalId);
            
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void SaveScheduleEntriesRange_WithCustomer_AnotherProfile()
        {
            var group = CreateCustomerGroup("custGroup", profiles[1]);
            var se1 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(4));
            var se2 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(6));
            var se3 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddDays(2).AddHours(4));

            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.Date.AddDays(7);
            var dto = se1.Map<ScheduleEntryDTO>();
            dto.CustomerGroupId = group.GlobalId;
            param.Entries.Add(dto);
            param.Entries.Add(se2.Map<ScheduleEntryDTO>());
            param.Entries.Add(se3.Map<ScheduleEntryDTO>());
            var entry = new ScheduleEntryDTO();
            entry.StartTime = param.StartDay.AddHours(7);
            entry.EndTime = entry.StartTime.AddMinutes(90);
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 3;
            param.Entries.Add(entry);

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveScheduleEntriesRange(data.Token, param);
            });

        }

        [Test]
        public void SaveScheduleEntriesRange_Update()
        {
            var se1 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(4));
            var se2 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(6));
            var se3 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddDays(2).AddHours(4));

            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.Date.AddDays(7);
            param.Entries.Add(se1.Map<ScheduleEntryDTO>());
            param.Entries.Add(se2.Map<ScheduleEntryDTO>());
            var dto=se3.Map<ScheduleEntryDTO>();
            dto.ActivityId = activities[1].GlobalId;
            param.Entries.Add(dto);

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            IList<ScheduleEntryBaseDTO> list = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                list = service.SaveScheduleEntriesRange(data.Token, param);
            });
            var dbResult = Session.QueryOver<ScheduleEntry>().List();
            Assert.AreEqual(3, dbResult.Count);
            var dto3 = list.Where(x => x.GlobalId == se3.GlobalId).SingleOrDefault();
            Assert.AreEqual(activities[1].GlobalId,( (ScheduleEntryDTO)dto3).ActivityId);
        }

        [Test]
        public void SaveScheduleEntriesRange_UpdateLockedEntry()
        {
            var se1 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(4));
            var se2 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(6));
            var se3 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddDays(2).AddHours(4));
            se3.State = ScheduleEntryState.Done;
            insertToDatabase(se3);

            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.Date.AddDays(7);
            param.Entries.Add(se1.Map<ScheduleEntryDTO>());
            param.Entries.Add(se2.Map<ScheduleEntryDTO>());
            var dto = se3.Map<ScheduleEntryDTO>();
            dto.ActivityId = activities[1].GlobalId;
            param.Entries.Add(dto);

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            IList<ScheduleEntryBaseDTO> list = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                list = service.SaveScheduleEntriesRange(data.Token, param);
            });
            var dbResult = Session.QueryOver<ScheduleEntry>().List();
            Assert.AreEqual(3, dbResult.Count);
            var db3 = dbResult.Where(x => x.GlobalId == se3.GlobalId).SingleOrDefault();
            Assert.AreEqual(activities[0].GlobalId, db3.Activity.GlobalId, "Locked entry shouldn't be updated");
            var dto3 = list.Where(x => x.GlobalId == se3.GlobalId).SingleOrDefault();

            Assert.AreEqual(activities[0].GlobalId, ( (ScheduleEntryDTO)dto3).ActivityId,"Locked entry shouldn't be updated");
        }

        [Test]
        public void SaveScheduleEntriesRange_UpdateWithReservations()
        {
            var customer=CreateCustomer("cust1", profiles[0]);
            var se1 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(4));
            ScheduleEntryReservation reservation = new ScheduleEntryReservation();
            reservation.Customer = customer;
            reservation.ScheduleEntry = se1;
            reservation.Name = "test";
            reservation.Profile = profiles[0];
            insertToDatabase(reservation);
            var se2 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(6));
            var se3 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddDays(2).AddHours(4));

            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.Date.AddDays(7);
            param.Entries.Add(se1.Map<ScheduleEntryDTO>());
            param.Entries.Add(se2.Map<ScheduleEntryDTO>());
            var dto = se3.Map<ScheduleEntryDTO>();
            dto.ActivityId = activities[1].GlobalId;
            param.Entries.Add(dto);

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            IList<ScheduleEntryBaseDTO> list = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                list = service.SaveScheduleEntriesRange(data.Token, param);
                Assert.AreEqual(1, list[0].Reservations.Count);
            });
            var dbResult = Session.QueryOver<ScheduleEntry>().List();
            Assert.AreEqual(1, dbResult[0].Reservations.Count);
            
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void SaveScheduleEntriesRange_Add_MyPlace_AnotherProfile()
        {
            var se1 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(4));
            var se2 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(6));
            var se3 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddDays(2).AddHours(4));

            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.Date.AddDays(7);
            param.Entries.Add(se1.Map<ScheduleEntryDTO>());
            param.Entries.Add(se2.Map<ScheduleEntryDTO>());
            param.Entries.Add(se3.Map<ScheduleEntryDTO>());
            var entry = new ScheduleEntryDTO();
            entry.StartTime = param.StartDay.AddHours(7);
            entry.EndTime = entry.StartTime.AddMinutes(90);
            entry.MyPlaceId = GetDefaultMyPlace(profiles[1]).GlobalId;
            entry.ActivityId = activities[1].GlobalId;
            entry.MaxPersons = 3;
            param.Entries.Add(entry);

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveScheduleEntriesRange(data.Token, param);
            });
        }
 
        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void SaveScheduleEntriesRange_Add_Activity_AnotherProfile()
        {
            var se1 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(4));
            var se2 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(6));
            var se3 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddDays(2).AddHours(4));
            var anotherUserActivity = CreateActivity("fitness", profiles[1]);

            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.Date.AddDays(7);
            param.Entries.Add(se1.Map<ScheduleEntryDTO>());
            param.Entries.Add(se2.Map<ScheduleEntryDTO>());
            param.Entries.Add(se3.Map<ScheduleEntryDTO>());
            var entry = new ScheduleEntryDTO();
            entry.StartTime = param.StartDay.AddHours(7);
            entry.EndTime = entry.StartTime.AddMinutes(90);
            entry.ActivityId = anotherUserActivity.GlobalId;
            entry.MaxPersons = 3;
            param.Entries.Add(entry);

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveScheduleEntriesRange(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void SaveScheduleEntriesRange_Update_Activity_AnotherProfile()
        {
            var se1 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(4));
            var se2 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(6));
            var se3 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddDays(2).AddHours(4));
            var anotherUserActivity = CreateActivity("fitness", profiles[1]);
            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.Date.AddDays(7);
            param.Entries.Add(se1.Map<ScheduleEntryDTO>());
            param.Entries.Add(se2.Map<ScheduleEntryDTO>());
            var dto = se3.Map<ScheduleEntryDTO>();
            dto.ActivityId = anotherUserActivity.GlobalId;
            param.Entries.Add(dto);


            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveScheduleEntriesRange(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void SaveScheduleEntriesRange_Update_ScheduleEntry_AnotherProfile()
        {
            var se1 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(4));
            var se2 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(6));
            var se3 = CreateScheduleEntry(profiles[1], DateTime.UtcNow.Date.AddDays(2).AddHours(4));

            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.Date.AddDays(7);
            param.Entries.Add(se1.Map<ScheduleEntryDTO>());
            param.Entries.Add(se2.Map<ScheduleEntryDTO>());
            param.Entries.Add(se3.Map<ScheduleEntryDTO>());

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveScheduleEntriesRange(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SaveScheduleEntriesRange_EntriesBeforeStartDate()
        {
            var se1 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(4));
            var se2 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(6));
            var se3 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddDays(2).AddHours(4));

            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow.Date.AddDays(1);
            param.EndDay = DateTime.UtcNow.Date.AddDays(7);
            param.Entries.Add(se1.Map<ScheduleEntryDTO>());
            param.Entries.Add(se2.Map<ScheduleEntryDTO>());
            param.Entries.Add(se3.Map<ScheduleEntryDTO>());

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveScheduleEntriesRange(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SaveScheduleEntriesRange_EntriesAfterEndDate()
        {
            var se1 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(4));
            var se2 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(6));
            var se3 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddDays(8).AddHours(4));

            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow.Date.AddDays(1);
            param.EndDay = DateTime.UtcNow.Date.AddDays(7);
            param.Entries.Add(se1.Map<ScheduleEntryDTO>());
            param.Entries.Add(se2.Map<ScheduleEntryDTO>());
            param.Entries.Add(se3.Map<ScheduleEntryDTO>());

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveScheduleEntriesRange(data.Token, param);
            });
        }

        [Test]
        public void SaveScheduleEntriesRange_AddTheSameTimeButAnotherUser()
        {
            var act=CreateActivity("newActivity", profiles[1]);
            var se=CreateScheduleEntry(profiles[1], DateTime.UtcNow.Date.AddHours(7), act);

            var se1 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(4));
            var se2 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(6));
            var se3 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddDays(2).AddHours(4));

            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.Date.AddDays(7);
            param.Entries.Add(se1.Map<ScheduleEntryDTO>());
            param.Entries.Add(se2.Map<ScheduleEntryDTO>());
            param.Entries.Add(se3.Map<ScheduleEntryDTO>());
            var entry = new ScheduleEntryDTO();
            entry.StartTime = param.StartDay.AddHours(7);
            entry.EndTime = entry.StartTime.AddMinutes(90);
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 3;
            param.Entries.Add(entry);

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            IList<ScheduleEntryBaseDTO> list = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                list = service.SaveScheduleEntriesRange(data.Token, param);
            });
            var dbResult = Session.QueryOver<ScheduleEntry>().List();
            Assert.AreEqual(5, dbResult.Count);
        }
        #endregion

        #region Save and copy

        [Test]
        public void Copy_SourceTwoWeeksWithOneDay_DestTwoWeeks()
        {
            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            var entry = new ScheduleEntryDTO();
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 1;
            entry.StartTime = DateTime.UtcNow.Date;
            entry.EndTime = entry.StartTime.AddMinutes(30);
            param.Entries.Add(entry);

            var entry1 = new ScheduleEntryDTO();
            entry1.ActivityId = activities[1].GlobalId;
            entry1.MaxPersons = 2;
            entry1.StartTime = DateTime.UtcNow.Date.AddDays(7);
            entry1.EndTime = entry1.StartTime.AddMinutes(30);
            param.Entries.Add(entry1);


            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.AddDays(13).Date;
            param.CopyStart = DateTime.UtcNow.AddDays(7);
            param.CopyEnd = param.CopyStart.Value.AddDays(14).Date;

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            IList<ScheduleEntryBaseDTO> list = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                list = service.SaveScheduleEntriesRange(data.Token, param);
            });


            var result = Session.QueryOver<ScheduleEntry>().Where(x => x.StartTime >= param.CopyStart.Value.Date).OrderBy(x => x.StartTime).Asc.List<ScheduleEntry>();
            Assert.AreEqual(2, result.Count);

            var item = list.Where(x => x.StartTime == entry.StartTime.AddDays(7)).SingleOrDefault();
            var res = result.Where(x => x.StartTime == entry.StartTime.AddDays(7)).SingleOrDefault();
            assertEntries(item, res);

            item = list.Where(x => x.StartTime == entry1.StartTime.AddDays(7)).SingleOrDefault();
            res = result.Where(x => x.StartTime == entry1.StartTime.AddDays(7)).SingleOrDefault();
            assertEntries(item, res);
        }

        [Test]
        public void CopyWithReminder()
        {
            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            var entry = new ScheduleEntryDTO();
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 1;
            entry.StartTime = DateTime.UtcNow.Date;
            entry.EndTime = entry.StartTime.AddMinutes(30);
            entry.RemindBefore = TimeSpan.FromMinutes(30);
            param.Entries.Add(entry);


            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.AddDays(6).Date;
            param.CopyStart = DateTime.UtcNow.AddDays(7);
            param.CopyEnd = param.CopyStart.Value.AddDays(7).Date;

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            IList<ScheduleEntryBaseDTO> list = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                list = service.SaveScheduleEntriesRange(data.Token, param);
            });


            var result = Session.QueryOver<ScheduleEntry>().Where(x => x.StartTime >= param.CopyStart.Value.Date).OrderBy(x => x.StartTime).Asc.List<ScheduleEntry>();
            Assert.AreEqual(1, result.Count);

            Assert.AreEqual(result[0].StartTime, result[0].Reminder.DateTime);
            Assert.AreEqual(TimeSpan.FromMinutes(30), result[0].Reminder.RemindBefore);
            Assert.AreEqual("ScheduleEntryDTO:" + result[0].GlobalId, result[0].Reminder.ConnectedObject);
            Assert.AreEqual(ReminderRepetitions.Once, result[0].Reminder.Repetitions);
            Assert.AreEqual(profiles[0].GlobalId, result[0].Reminder.Profile.GlobalId);
            Assert.AreEqual(1, Session.QueryOver<ReminderItem>().RowCount());

        }

        [Test]
        public void CopyWithoutReservations()
        {
            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            var entry = new ScheduleEntryDTO();
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 1;
            entry.StartTime = DateTime.UtcNow.Date;
            entry.EndTime = entry.StartTime.AddMinutes(30);
            var reservation = new ScheduleEntryReservationDTO();
            reservation.CustomerId = Guid.NewGuid();
            reservation.ScheduleEntryId = Guid.NewGuid();
            entry.Reservations.Add(reservation);
            param.Entries.Add(entry);


            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.AddDays(6).Date;
            param.CopyStart = DateTime.UtcNow.AddDays(7);
            param.CopyEnd = param.CopyStart.Value.AddDays(7).Date;

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            IList<ScheduleEntryBaseDTO> list = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                list = service.SaveScheduleEntriesRange(data.Token, param);
            });


            var result = Session.QueryOver<ScheduleEntry>().Where(x => x.StartTime >= param.CopyStart.Value.Date).OrderBy(x => x.StartTime).Asc.List<ScheduleEntry>();
            Assert.AreEqual(1, result.Count);

            var item = list.Where(x => x.StartTime == entry.StartTime.AddDays(7)).SingleOrDefault();
            assertEntries(item, result[0]);

            var count = Session.QueryOver<ScheduleEntryReservation>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void Copy_SourceOneDayInOneWeek_DestTwoWeeks()
        {
            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            var entry = new ScheduleEntryDTO();
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 1;
            entry.StartTime = DateTime.UtcNow.Date.AddDays(2);
            entry.EndTime = entry.StartTime.AddMinutes(30);
            param.Entries.Add(entry);


            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.AddDays(6).Date;
            param.CopyStart = DateTime.UtcNow.AddDays(7);
            param.CopyEnd = param.CopyStart.Value.AddDays(14).Date;

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            IList<ScheduleEntryBaseDTO> list = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                list = service.SaveScheduleEntriesRange(data.Token, param);
            });


            var result = Session.QueryOver<ScheduleEntry>().Where(x => x.StartTime >= param.CopyStart.Value.Date).OrderBy(x => x.StartTime).Asc.List<ScheduleEntry>();
            Assert.AreEqual(2, result.Count);

            var item = list.Where(x => x.StartTime == entry.StartTime.AddDays(7)).SingleOrDefault();
            var res = result.Where(x => x.StartTime == entry.StartTime.AddDays(7)).SingleOrDefault();
            assertEntries(item, res);

            item = list.Where(x => x.StartTime == entry.StartTime.AddDays(14)).SingleOrDefault();
            res = result.Where(x => x.StartTime == entry.StartTime.AddDays(14)).SingleOrDefault();
            assertEntries(item, res);
        }

        [Test]
        public void CopyOneDay_Bug1()
        {
            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            var entry = new ScheduleEntryDTO();
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 1;
            entry.StartTime = DateTime.Parse("2012-10-16 06:00:00");
            entry.EndTime = entry.StartTime.AddMinutes(60);
            param.Entries.Add(entry);


            param.StartDay = new DateTime(2012, 10, 14, 22, 0, 0);
            param.EndDay = new DateTime(2012, 10, 21, 22, 0, 0);
            param.CopyStart = new DateTime(2012, 10, 21, 22, 0, 0);
            param.CopyEnd = new DateTime(2012, 11, 4, 23, 0, 0);

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            IList<ScheduleEntryBaseDTO> list = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                list = service.SaveScheduleEntriesRange(data.Token, param);
            });


            var result = Session.QueryOver<ScheduleEntry>().Where(x => x.StartTime >= param.CopyStart.Value.Date).OrderBy(x => x.StartTime).Asc.List<ScheduleEntry>();
            Assert.AreEqual(2, result.Count);

            var item = list.Where(x => x.StartTime == entry.StartTime.AddDays(7)).SingleOrDefault();
            assertEntries(item, result[0]);
        }

        [Test]
        public void CopyOneDayOnTheBeginningWeekToNextWeek()
        {
            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            var entry = new ScheduleEntryDTO();
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 1;
            entry.StartTime = DateTime.UtcNow.Date;
            entry.EndTime = entry.StartTime.AddMinutes(30);
            param.Entries.Add(entry);


            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.AddDays(6).Date;
            param.CopyStart = DateTime.UtcNow.AddDays(7);
            param.CopyEnd = param.CopyStart.Value.AddDays(7).Date;

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            IList<ScheduleEntryBaseDTO> list = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                list = service.SaveScheduleEntriesRange(data.Token, param);
            });

            
            var result = Session.QueryOver<ScheduleEntry>().Where(x => x.StartTime >= param.CopyStart.Value.Date).OrderBy(x => x.StartTime).Asc.List<ScheduleEntry>();
            Assert.AreEqual(1, result.Count);

            var item = list.Where(x => x.StartTime == entry.StartTime.AddDays(7)).SingleOrDefault();
            assertEntries(item, result[0]);
        }

        [Test]
        public void CopyOneDayOnTheEndingWeekToNextWeek()
        {
            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            var entry = new ScheduleEntryDTO();
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 1;
            entry.StartTime = DateTime.UtcNow.AddDays(6).Date.AddHours(23);
            entry.EndTime = entry.StartTime.AddMinutes(30);
            param.Entries.Add(entry);


            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.AddDays(6).Date;
            param.CopyStart = DateTime.UtcNow.AddDays(7);
            param.CopyEnd = param.CopyStart.Value.AddDays(7).Date;

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            IList<ScheduleEntryBaseDTO> list = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                list = service.SaveScheduleEntriesRange(data.Token, param);
            });


            var result = Session.QueryOver<ScheduleEntry>().Where(x => x.StartTime >= param.CopyStart.Value.Date).OrderBy(x => x.StartTime).Asc.List<ScheduleEntry>();
            Assert.AreEqual(1, result.Count);

            var item = list.Where(x => x.StartTime == entry.StartTime.AddDays(7)).SingleOrDefault();
            assertEntries(item, result[0]);
        }

        [Test]
        public void CopyThreeDaysInOneWeekToNextWeek()
        {
            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            var entry = new ScheduleEntryDTO();
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 1;
            entry.StartTime = DateTime.UtcNow.AddDays(6).Date.AddHours(23);
            entry.EndTime = entry.StartTime.AddMinutes(30);
            param.Entries.Add(entry);

            var entry1 = new ScheduleEntryDTO();
            entry1.ActivityId = activities[0].GlobalId;
            entry1.MaxPersons = 2;
            entry1.StartTime = DateTime.UtcNow.Date;
            entry1.EndTime = entry1.StartTime.AddMinutes(30);
            param.Entries.Add(entry1);

            var entry2 = new ScheduleEntryDTO();
            entry2.ActivityId = activities[0].GlobalId;
            entry2.MaxPersons = 3;
            entry2.StartTime = DateTime.UtcNow.AddDays(3).Date.AddHours(3);
            entry2.EndTime = entry2.StartTime.AddMinutes(60);
            param.Entries.Add(entry2);


            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.AddDays(6).Date;
            param.CopyStart = DateTime.UtcNow.AddDays(7);
            param.CopyEnd = param.CopyStart.Value.AddDays(7).Date;

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            IList<ScheduleEntryBaseDTO> list = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                list = service.SaveScheduleEntriesRange(data.Token, param);
            });


            var result = Session.QueryOver<ScheduleEntry>().Where(x => x.StartTime >= param.CopyStart.Value.Date).OrderBy(x => x.StartTime).Asc.List<ScheduleEntry>();
            Assert.AreEqual(3, result.Count);

            var item = list.Where(x => x.StartTime == entry.StartTime.AddDays(7)).SingleOrDefault();
            var res = result.Where(x => x.StartTime == entry.StartTime.AddDays(7)).SingleOrDefault();
            assertEntries(item, res);

            item = list.Where(x => x.StartTime == entry1.StartTime.AddDays(7)).SingleOrDefault();
            res = result.Where(x => x.StartTime == entry1.StartTime.AddDays(7)).SingleOrDefault();
            assertEntries(item, res);

            item = list.Where(x => x.StartTime == entry2.StartTime.AddDays(7)).SingleOrDefault();
            res = result.Where(x => x.StartTime == entry2.StartTime.AddDays(7)).SingleOrDefault();
            assertEntries(item, res);
        }

        [Test]
        public void Copy_SourceTwoDays_Dest10Days_OneShouldBeTakenSecondTime()
        {
            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            var entry = new ScheduleEntryDTO();
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 1;
            entry.StartTime = DateTime.UtcNow.AddDays(6).Date.AddHours(23);
            entry.EndTime = entry.StartTime.AddMinutes(30);
            param.Entries.Add(entry);

            var entry1 = new ScheduleEntryDTO();
            entry1.ActivityId = activities[0].GlobalId;
            entry1.MaxPersons = 2;
            entry1.StartTime = DateTime.UtcNow.Date;
            entry1.EndTime = entry1.StartTime.AddMinutes(30);
            param.Entries.Add(entry1);



            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.AddDays(6).Date;
            param.CopyStart = DateTime.UtcNow.AddDays(7);
            param.CopyEnd = param.CopyStart.Value.AddDays(11).Date;

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            IList<ScheduleEntryBaseDTO> list = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                list = service.SaveScheduleEntriesRange(data.Token, param);
            });


            var result = Session.QueryOver<ScheduleEntry>().Where(x => x.StartTime >= param.CopyStart.Value.Date).OrderBy(x => x.StartTime).Asc.List<ScheduleEntry>();
            Assert.AreEqual(3, result.Count);

            var item = list.Where(x => x.StartTime == entry.StartTime.AddDays(7)).SingleOrDefault();
            var res = result.Where(x => x.StartTime == entry.StartTime.AddDays(7)).SingleOrDefault();
            assertEntries(item, res);

            item = list.Where(x => x.StartTime == entry1.StartTime.AddDays(7)).SingleOrDefault();
            res = result.Where(x => x.StartTime == entry1.StartTime.AddDays(7)).SingleOrDefault();
            assertEntries(item, res);

            item = list.Where(x => x.StartTime == entry1.StartTime.AddDays(14)).SingleOrDefault();
            res = result.Where(x => x.StartTime == entry1.StartTime.AddDays(14)).SingleOrDefault();
            assertEntries(item, res);
        }

        [Test]
        public void Copy_SourceTwoDays_Dest10Days_BothDaysShouldBeTakenSecondTime()
        {
            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            var entry = new ScheduleEntryDTO();
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 1;
            entry.StartTime = DateTime.UtcNow.AddDays(1).Date.AddHours(23);
            entry.EndTime = entry.StartTime.AddMinutes(30);
            param.Entries.Add(entry);

            var entry1 = new ScheduleEntryDTO();
            entry1.ActivityId = activities[0].GlobalId;
            entry1.MaxPersons = 2;
            entry1.StartTime = DateTime.UtcNow.Date;
            entry1.EndTime = entry1.StartTime.AddMinutes(30);
            param.Entries.Add(entry1);



            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.AddDays(6).Date;
            param.CopyStart = DateTime.UtcNow.AddDays(7);
            param.CopyEnd = param.CopyStart.Value.AddDays(11).Date;

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            IList<ScheduleEntryBaseDTO> list = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                list = service.SaveScheduleEntriesRange(data.Token, param);
            });


            var result = Session.QueryOver<ScheduleEntry>().Where(x => x.StartTime >= param.CopyStart.Value.Date).OrderBy(x => x.StartTime).Asc.List<ScheduleEntry>();
            Assert.AreEqual(4, result.Count);

            var item = list.Where(x => x.StartTime == entry.StartTime.AddDays(7)).SingleOrDefault();
            var res = result.Where(x => x.StartTime == entry.StartTime.AddDays(7)).SingleOrDefault();
            assertEntries(item, res);

            item = list.Where(x => x.StartTime == entry1.StartTime.AddDays(7)).SingleOrDefault();
            res = result.Where(x => x.StartTime == entry1.StartTime.AddDays(7)).SingleOrDefault();
            assertEntries(item, res);

            item = list.Where(x => x.StartTime == entry.StartTime.AddDays(14)).SingleOrDefault();
            res = result.Where(x => x.StartTime == entry.StartTime.AddDays(14)).SingleOrDefault();
            assertEntries(item, res);

            item = list.Where(x => x.StartTime == entry1.StartTime.AddDays(14)).SingleOrDefault();
            res = result.Where(x => x.StartTime == entry1.StartTime.AddDays(14)).SingleOrDefault();
            assertEntries(item, res);
        }

        [Test]
        [ExpectedException(typeof(AlreadyOccupiedException))]
        public void CopyToOccupiedWeek()
        {
            CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddDays(8).AddHours(4));

            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            var entry = new ScheduleEntryDTO();
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 1;
            entry.StartTime = DateTime.UtcNow.Date;
            entry.EndTime = entry.StartTime.AddMinutes(30);
            param.Entries.Add(entry);


            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.AddDays(6).Date;
            param.CopyStart = DateTime.UtcNow.AddDays(7);
            param.CopyEnd = param.CopyStart.Value.AddDays(14).Date;

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveScheduleEntriesRange(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Copy_Bug_OccupiedEntries()
        {
            ScheduleEntry entry1 = new ScheduleEntry();
            entry1.StartTime = new DateTime(2012, 9, 18, 5, 30, 0);
            entry1.Activity = activities[0];
            entry1.MyPlace = GetDefaultMyPlace(profiles[0]);
            entry1.MaxPersons = 1;
            entry1.Profile = profiles[0];
            entry1.EndTime = entry1.StartTime.AddMinutes(30);
            insertToDatabase(entry1);

            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            var entry = entry1.Map<ScheduleEntryDTO>();
            param.Entries.Add(entry);


            param.StartDay = new DateTime(2012, 9, 16, 22, 0, 0);
            param.EndDay = new DateTime(2012, 9, 22, 22, 0, 0);
            param.CopyStart = new DateTime(2012, 9, 18, 22, 0, 0);
            param.CopyEnd = new DateTime(2012, 9, 19, 22, 0, 0);

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveScheduleEntriesRange(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(AlreadyOccupiedException))]
        public void Copy_OccupiedEntries()
        {
            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            var entry = new ScheduleEntryDTO();
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 1;
            entry.StartTime = DateTime.UtcNow.Date;
            entry.EndTime = entry.StartTime.AddMinutes(30);
            param.Entries.Add(entry);
            entry = new ScheduleEntryDTO();
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 1;
            entry.StartTime = DateTime.UtcNow.Date;
            entry.EndTime = entry.StartTime.AddMinutes(60);
            param.Entries.Add(entry);


            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.AddDays(6).Date;
            param.CopyStart = DateTime.UtcNow.AddDays(7);
            param.CopyEnd = param.CopyStart.Value.AddDays(14).Date;

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveScheduleEntriesRange(data.Token, param);
            });
        }



        [Test]
        public void CopyOneDayOnTheEndingWeekToPreviousWeek()
        {
            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            var entry = new ScheduleEntryDTO();
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 1;
            entry.StartTime = DateTime.UtcNow.AddDays(6).Date.AddHours(23);
            entry.EndTime = entry.StartTime.AddMinutes(30);
            param.Entries.Add(entry);


            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.AddDays(6).Date;
            param.CopyStart = DateTime.UtcNow.AddDays(-7);
            param.CopyEnd = param.CopyStart.Value.AddDays(7).Date;

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            IList<ScheduleEntryBaseDTO> list = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                list = service.SaveScheduleEntriesRange(data.Token, param);
            });


            var result = Session.QueryOver<ScheduleEntry>().Where(x => x.StartTime >= param.CopyStart.Value.Date).OrderBy(x => x.StartTime).Asc.List<ScheduleEntry>();
            Assert.AreEqual(1, result.Count);

            var item = list.Where(x => x.StartTime == entry.StartTime.AddDays(-7)).SingleOrDefault();
            var res = result.Where(x => x.StartTime == entry.StartTime.AddDays(-7)).SingleOrDefault();
            assertEntries(item, res);
        }

        [Test]
        public void CopyOneDayOnTheBeginningWeekToPreviousWeek()
        {
            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            var entry = new ScheduleEntryDTO();
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 1;
            entry.StartTime = DateTime.UtcNow.Date;
            entry.EndTime = entry.StartTime.AddMinutes(30);
            param.Entries.Add(entry);


            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.AddDays(6).Date;
            param.CopyStart = DateTime.UtcNow.AddDays(-7);
            param.CopyEnd = param.CopyStart.Value.AddDays(7).Date;

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            IList<ScheduleEntryBaseDTO> list = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                list = service.SaveScheduleEntriesRange(data.Token, param);
            });


            var result = Session.QueryOver<ScheduleEntry>().Where(x => x.StartTime >= param.CopyStart.Value.Date).OrderBy(x => x.StartTime).Asc.List<ScheduleEntry>();
            Assert.AreEqual(1, result.Count);

            var item = list.Where(x => x.StartTime == entry.StartTime.AddDays(-7)).SingleOrDefault();
            var res = result.Where(x => x.StartTime == entry.StartTime.AddDays(-7)).SingleOrDefault();
            assertEntries(item, res);
        }

        [Test]
        public void CopyExistingOneDayOnTheBeginningWeekToPreviousWeek()
        {
            var se=CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date);
            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            var entry = se.Map<ScheduleEntryDTO>();
            param.Entries.Add(entry);


            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.AddDays(6).Date;
            param.CopyStart = DateTime.UtcNow.AddDays(-7);
            param.CopyEnd = param.CopyStart.Value.AddDays(7).Date;

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            IList<ScheduleEntryBaseDTO> list = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                list = service.SaveScheduleEntriesRange(data.Token, param);
            });


            var result = Session.QueryOver<ScheduleEntry>().Where(x => x.StartTime >= param.CopyStart.Value.Date).OrderBy(x => x.StartTime).Asc.List<ScheduleEntry>();
            Assert.AreEqual(2, result.Count);

            var item = list.Where(x => x.StartTime == entry.StartTime.AddDays(-7)).SingleOrDefault();
            var res = result.Where(x => x.StartTime == entry.StartTime.AddDays(-7)).SingleOrDefault();
            assertEntries(item, res);
        }
        private void assertEntries(ScheduleEntryBaseDTO entryDto, ScheduleEntryBase copy)
        {
            var entry=Session.Get<ScheduleEntryBase>(entryDto.GlobalId);
            if (entry is ScheduleEntry)
            {
                Assert.AreEqual(((ScheduleEntry)entry).MaxPersons, ((ScheduleEntry)copy).MaxPersons);
            }
            if (entry is Championship)
            {
                Assert.AreEqual(((Championship)entry).Name, ((Championship)copy).Name);
            }
            Assert.AreEqual(entry.StartTime.TimeOfDay, copy.StartTime.TimeOfDay);
            Assert.AreEqual(entry.EndTime.TimeOfDay, copy.EndTime.TimeOfDay);
            Assert.AreEqual(entry.EndTime.DayOfWeek, copy.EndTime.DayOfWeek);
            Assert.AreEqual(entry.EndTime, copy.EndTime);
        }
        #endregion

        #region DataInfo

        [Test]
        public void SaveScheduleEntriesRange_DataInfo()
        {
            var se1 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(4));
            var se2 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddHours(6));
            var se3 = CreateScheduleEntry(profiles[0], DateTime.UtcNow.Date.AddDays(2).AddHours(4));

            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.Date.AddDays(7);
            param.Entries.Add(se1.Map<ScheduleEntryDTO>());
            param.Entries.Add(se2.Map<ScheduleEntryDTO>());
            var dto = se3.Map<ScheduleEntryDTO>();
            dto.ActivityId = activities[1].GlobalId;
            param.Entries.Add(dto);

            var oldScheduleEntryHash = profiles[0].DataInfo.ScheduleEntryHash;
            var oldReminderEntryHash = profiles[0].DataInfo.ReminderHash;

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveScheduleEntriesRange(data.Token, param);
            });
            var dbProfile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.AreNotEqual(oldScheduleEntryHash, dbProfile.DataInfo.ScheduleEntryHash);
            Assert.AreEqual(oldReminderEntryHash, dbProfile.DataInfo.ReminderHash);
        }

        [Test]
        public void CopyWithReminder_DataInfo()
        {
            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            var entry = new ScheduleEntryDTO();
            entry.ActivityId = activities[0].GlobalId;
            entry.MaxPersons = 1;
            entry.StartTime = DateTime.UtcNow.Date;
            entry.EndTime = entry.StartTime.AddMinutes(30);
            entry.RemindBefore = TimeSpan.FromMinutes(30);
            param.Entries.Add(entry);


            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.AddDays(6).Date;
            param.CopyStart = DateTime.UtcNow.AddDays(7);
            param.CopyEnd = param.CopyStart.Value.AddDays(7).Date;

            var oldScheduleEntryHash = profiles[0].DataInfo.ScheduleEntryHash;
            var oldReminderEntryHash = profiles[0].DataInfo.ReminderHash;

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveScheduleEntriesRange(data.Token, param);
            });

            var dbProfile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.AreNotEqual(oldScheduleEntryHash, dbProfile.DataInfo.ScheduleEntryHash);
            Assert.AreNotEqual(oldReminderEntryHash, dbProfile.DataInfo.ReminderHash);
        }
        #endregion

        #region Championships

        [Test]
        public void Championship_SaveScheduleEntryRange()
        {
            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.Date.AddDays(7);
            
            var championship = new ScheduleChampionshipDTO();
            championship.StartTime = param.StartDay.AddHours(7);
            championship.Name = "Championship";
            championship.EndTime = championship.StartTime.AddMinutes(90);
            championship.Price = 22m;
            param.Entries.Add(championship);
            var entry = new ScheduleEntryDTO();
            entry.StartTime = param.StartDay.AddHours(10);
            entry.EndTime = entry.StartTime.AddMinutes(60);
            entry.ActivityId = activities[1].GlobalId;
            entry.MaxPersons = 13;
            param.Entries.Add(entry);
            entry = new ScheduleEntryDTO();
            entry.StartTime = param.StartDay.AddDays(2).AddHours(8);
            entry.EndTime = entry.StartTime.AddMinutes(60);
            entry.ActivityId = activities[1].GlobalId;
            entry.MaxPersons = 13;
            param.Entries.Add(entry);
            entry = new ScheduleEntryDTO();
            entry.StartTime = param.StartDay.AddDays(6).AddHours(8);
            entry.EndTime = entry.StartTime.AddMinutes(60);
            entry.ActivityId = activities[2].GlobalId;
            entry.MaxPersons = 13;
            param.Entries.Add(entry);

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            IList<ScheduleEntryBaseDTO> list = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                list = service.SaveScheduleEntriesRange(data.Token, param);
                Assert.AreNotEqual(Guid.Empty, list[0].GlobalId);
            });
            var dbResult = Session.QueryOver<ScheduleEntryBase>().List();
            Assert.AreEqual(4, dbResult.Count);
            UnitTestHelper.CompareObjects(list, dbResult.Map<IList<ScheduleEntryBaseDTO>>(), true);
        }

        [Test]
        public void Championship_Copy_SourceOneDayInOneWeek_DestTwoWeeks()
        {
            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            param.Mode = SaveScheduleEntryRangeCopyMode.All;
            var entry = new ScheduleChampionshipDTO();
            entry.Name = "test name";
            entry.ChampionshipType = ChampionshipType.Trojboj;
            entry.StartTime = DateTime.UtcNow.Date.AddDays(2);
            entry.EndTime = entry.StartTime.AddMinutes(30);
            param.Entries.Add(entry);


            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.AddDays(6).Date;
            param.CopyStart = DateTime.UtcNow.AddDays(7);
            param.CopyEnd = param.CopyStart.Value.AddDays(14).Date;

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            IList<ScheduleEntryBaseDTO> list = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                list = service.SaveScheduleEntriesRange(data.Token, param);
            });


            var result = Session.QueryOver<Championship>().Where(x => x.StartTime >= param.CopyStart.Value.Date).OrderBy(x => x.StartTime).Asc.List();
            Assert.AreEqual(2, result.Count);

            var item = list.Where(x => x.StartTime == entry.StartTime.AddDays(7)).SingleOrDefault();
            var res = result.Where(x => x.StartTime == entry.StartTime.AddDays(7)).SingleOrDefault();
            assertEntries(item, res);

            item = list.Where(x => x.StartTime == entry.StartTime.AddDays(14)).SingleOrDefault();
            res = result.Where(x => x.StartTime == entry.StartTime.AddDays(14)).SingleOrDefault();
            assertEntries(item, res);
        }

        //[Test]
        //public void Championship_Copy__SkipEntriesGroupsAndCustomers()
        //{
        //    var customer = CreateCustomer("cust",profiles[0]);
        //    var exercise = CreateExercise(Session, null, "prof","pr");
        //    SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
        //    param.Mode = SaveScheduleEntryRangeCopyMode.All;
        //    var entry = new ScheduleChampionshipDTO();
        //    entry.Name = "test name";
        //    entry.ChampionshipType = ChampionshipType.Trojboj;
        //    entry.StartTime = DateTime.UtcNow.Date.AddDays(2);
        //    entry.EndTime = entry.StartTime.AddMinutes(30);
        //    param.Entries.Add(entry);

        //    var championshipCustomerDto = new ChampionshipCustomerDTO() {CustomerId = customer.GlobalId};
        //    var championshipGroup = new ChampionshipGroupDTO();
        //    championshipGroup.Members.Add(championshipCustomerDto);
        //    entry.Customers.Add(championshipCustomerDto);
        //    entry.Groups.Add(championshipGroup);
        //    entry.Entries.Add(new ChampionshipEntryDTO() { Customer = championshipCustomerDto, Exercise = exercise .Map<ExerciseLightDTO>()});
        //    param.StartDay = DateTime.UtcNow.Date;
        //    param.EndDay = DateTime.UtcNow.AddDays(6).Date;
        //    param.CopyStart = DateTime.UtcNow.AddDays(7);
        //    param.CopyEnd = param.CopyStart.Value.AddDays(14).Date;

        //    var profile1 = (ProfileDTO)profiles[0].Tag;
        //    SessionData data = CreateNewSession(profile1, ClientInformation);
        //    IList<ScheduleEntryBaseDTO> list = null;
        //    RunServiceMethod(delegate(InternalBodyArchitectService service)
        //    {
        //        list = service.SaveScheduleEntriesRange(data.Token, param);
        //    });


        //    var result = Session.QueryOver<Championship>().Where(x => x.StartTime >= param.CopyStart.Value.Date).OrderBy(x => x.StartTime).Asc.List();
        //    Assert.AreEqual(2, result.Count);

        //    var item = (ScheduleChampionshipDTO)list.Where(x => x.StartTime == entry.StartTime.AddDays(7)).SingleOrDefault();
        //    var res = (Championship)result.Where(x => x.StartTime == entry.StartTime.AddDays(7)).SingleOrDefault();
        //    Assert.AreEqual(0, item.Groups.Count);
        //    Assert.AreEqual(0, item.Entries.Count);
        //    Assert.AreEqual(0, item.Customers.Count);
        //    Assert.AreEqual(0, res.Groups.Count);
        //    Assert.AreEqual(0, res.Entries.Count);
        //    Assert.AreEqual(0, res.Customers.Count);

        //    item = (ScheduleChampionshipDTO)list.Where(x => x.StartTime == entry.StartTime.AddDays(14)).SingleOrDefault();
        //    res = result.Where(x => x.StartTime == entry.StartTime.AddDays(14)).SingleOrDefault();
        //    Assert.AreEqual(0, item.Groups.Count);
        //    Assert.AreEqual(0, item.Entries.Count);
        //    Assert.AreEqual(0, item.Customers.Count);
        //    Assert.AreEqual(0, res.Groups.Count);
        //    Assert.AreEqual(0, res.Entries.Count);
        //    Assert.AreEqual(0, res.Customers.Count);
        //}

        [Test]
        public void Championship_Copy_SkipChampionship()
        {
            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            param.Mode = SaveScheduleEntryRangeCopyMode.OnlyScheduleEntries;
            var entry = new ScheduleChampionshipDTO();
            entry.Name = "test name";
            entry.ChampionshipType = ChampionshipType.Trojboj;
            entry.StartTime = DateTime.UtcNow.Date.AddDays(2);
            entry.EndTime = entry.StartTime.AddMinutes(30);
            param.Entries.Add(entry);


            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.AddDays(6).Date;
            param.CopyStart = DateTime.UtcNow.AddDays(7);
            param.CopyEnd = param.CopyStart.Value.AddDays(14).Date;

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            IList<ScheduleEntryBaseDTO> list = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                list = service.SaveScheduleEntriesRange(data.Token, param);
            });


            var result = Session.QueryOver<Championship>().Where(x => x.StartTime >= param.CopyStart.Value.Date).OrderBy(x => x.StartTime).Asc.List();
            Assert.AreEqual(0, result.Count);
        }
        #endregion
    }
}
