using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using NUnit.Framework;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_GetScheduleEntries : TestServiceBase
    {
        List<Activity> activities = new List<Activity>();
        List<ScheduleEntryBase> entries = new List<ScheduleEntryBase>();
        List<Profile> profiles = new List<Profile>();

        public override void BuildDatabase()
        {
            profiles.Clear();
            activities.Clear();
            entries.Clear();

            using (var tx = Session.BeginTransaction())
            {
                var profile = CreateProfile(Session, "Profile1");
                profiles.Add(profile);
                profile = CreateProfile(Session, "Profile2");
                profiles.Add(profile);


                var activity = CreateActivity("Swimming", profiles[0]);
                activities.Add(activity);
                activity = CreateActivity("Strength training", profiles[0]);
                activities.Add(activity);

                ScheduleEntry entry = new ScheduleEntry();
                entry.Profile = profiles[0];
                entry.MyPlace = GetDefaultMyPlace(profiles[0]);
                entry.StartTime = DateTime.UtcNow.Date.AddDays(-10);
                entry.EndTime = DateTime.UtcNow.Date.AddDays(-10).AddHours(1);
                entry.Activity = activities[0];
                entries.Add(entry);
                insertToDatabase(entry);

                entry = new ScheduleEntry();
                entry.Profile = profiles[0];
                entry.MyPlace = GetDefaultMyPlace(profiles[0]);
                entry.StartTime = DateTime.UtcNow.Date.AddDays(-1).AddHours(1);
                entry.EndTime = DateTime.UtcNow.Date.AddDays(-1).AddHours(2);
                entry.Activity = activities[0];
                entries.Add(entry);
                insertToDatabase(entry);

                entry = new ScheduleEntry();
                entry.Profile = profiles[0];
                entry.MyPlace = GetDefaultMyPlace(profiles[0]);
                entry.StartTime = DateTime.UtcNow.Date.AddDays(-10).AddHours(1);
                entry.EndTime = DateTime.UtcNow.Date.AddDays(-10).AddHours(2);
                entry.Activity = activities[0];
                entries.Add(entry);
                insertToDatabase(entry);

                entry = new ScheduleEntry();
                entry.Profile = profiles[0];
                entry.MyPlace = GetDefaultMyPlace(profiles[0]);
                entry.StartTime = DateTime.UtcNow.Date.AddDays(-8).AddHours(1);
                entry.EndTime = DateTime.UtcNow.Date.AddDays(-8).AddHours(2);
                entry.Activity = activities[1];
                entries.Add(entry);
                insertToDatabase(entry);

                entry = new ScheduleEntry();
                entry.Profile = profiles[0];
                entry.MyPlace = GetDefaultMyPlace(profiles[0]);
                entry.StartTime = DateTime.UtcNow.Date.AddDays(-4).AddHours(1);
                entry.EndTime = DateTime.UtcNow.Date.AddDays(-4).AddHours(2);
                entry.Activity = activities[1];
                entries.Add(entry);
                insertToDatabase(entry);

                entry = new ScheduleEntry();
                entry.Profile = profiles[1];
                entry.MyPlace = GetDefaultMyPlace(profiles[1]);
                entry.StartTime = DateTime.UtcNow.Date.AddDays(-5).AddHours(1);
                entry.EndTime = DateTime.UtcNow.Date.AddDays(-5).AddHours(2);
                entry.Activity = activities[1];
                insertToDatabase(entry);

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
                var param = new GetScheduleEntriesParam();
                var retrievingInfo = new PartialRetrievingInfo();
                var list = service.GetScheduleEntries(data.Token,param, retrievingInfo);
                Assert.AreEqual(entries.Count, list.AllItemsCount);
                Assert.AreEqual(entries.Count, list.Items.Count);
            });
        }

        [Test]
        public void WithChampionship()
        {
            var champ = CreateChampionship(profiles[0], "test");
            entries.Add(champ);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new GetScheduleEntriesParam();
                var retrievingInfo = new PartialRetrievingInfo();
                var list = service.GetScheduleEntries(data.Token, param, retrievingInfo);
                Assert.AreEqual(entries.Count, list.AllItemsCount);
                Assert.AreEqual(entries.Count, list.Items.Count);
                Assert.IsFalse(list.Items[5] is ChampionshipDTO);
            });
        }

        [Test]
        public void TillEndTime()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new GetScheduleEntriesParam();
                param.EndTime = DateTime.UtcNow.Date.AddDays(-3);
                var retrievingInfo = new PartialRetrievingInfo();
                var list = service.GetScheduleEntries(data.Token,param, retrievingInfo);
                assert(list, 0, 2,3,4);
            });
        }

        [Test]
        public void SinceStartDateTillEndTime()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new GetScheduleEntriesParam();
                param.StartTime = DateTime.UtcNow.AddDays(-9);
                param.EndTime = DateTime.UtcNow.AddDays(-3);
                var retrievingInfo = new PartialRetrievingInfo();
                var list = service.GetScheduleEntries(data.Token,param, retrievingInfo);
                assert(list, 3,4);
            });
        }


        [Test]
        public void SinceStartDate()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new GetScheduleEntriesParam();
                param.StartTime = DateTime.UtcNow.AddDays(-9);
                var retrievingInfo = new PartialRetrievingInfo();
                var list = service.GetScheduleEntries(data.Token,param, retrievingInfo);
                assert(list,1,3,4);
            });
        }

        [Test]
        public void ForUsluga()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new GetScheduleEntriesParam();
                param.ActivityId = activities[0].GlobalId;
                var retrievingInfo = new PartialRetrievingInfo();
                var list = service.GetScheduleEntries(data.Token,param, retrievingInfo);
                assert(list,0, 1, 2);
            });
        }

        [Test]
        public void Mapper_ActivityId()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new GetScheduleEntriesParam();
                param.ActivityId = activities[0].GlobalId;
                var retrievingInfo = new PartialRetrievingInfo();
                var list = service.GetScheduleEntries(data.Token, param, retrievingInfo);
                Assert.AreEqual(activities[0].GlobalId, ((ScheduleEntryDTO)list.Items[0]).ActivityId);
            });
        }

        void assert(PagedResult<ScheduleEntryBaseDTO> result, params int[] scheduleIndexes)
        {
            Assert.AreEqual(scheduleIndexes.Length, result.AllItemsCount);
            foreach (int id in scheduleIndexes)
            {
                var res = result.Items.Where(x => x.GlobalId == entries[id].GlobalId).SingleOrDefault();
                Assert.IsNotNull(res);
            }
        }

    }
}
