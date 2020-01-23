using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Reports;
using BodyArchitect.Shared;
using NUnit.Framework;
using EntryObjectStatus = BodyArchitect.Model.EntryObjectStatus;
using Privacy = BodyArchitect.Model.Privacy;
using ReportStatus = BodyArchitect.Model.ReportStatus;

namespace BodyArchitect.UnitTests.V2.Reports
{
    [TestFixture]
    public class TestReport_MeasurementsTime : TestServiceBase
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

        SizeEntry AddTrainingDaySize(Profile profile, Customer customer, DateTime date, Wymiary wymiary, ReportStatus reportStatus = ReportStatus.ShowInReport)
        {
            var trainingDay = new TrainingDay(date);
            trainingDay.Customer = customer;
            trainingDay.Profile = profile;
            var entry = new SizeEntry();
            entry.Wymiary = wymiary;
            entry.ReportStatus = reportStatus;
            trainingDay.AddEntry(entry);

            insertToDatabase(trainingDay);
            return entry;
        }

        [Test]
        public void TwoMeasurements()
        {
            AddTrainingDaySize(profiles[0],null,DateTime.UtcNow.Date.AddDays(-10),new Wymiary(){Height = 100,Klatka = 50});
            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-11), new Wymiary() { Height = 40, Klatka = 30 });

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportMeasurementsTimeParams();
                var list = service.ReportMeasurementsTime(data.Token, param);
                assert(list, new Tuple<DateTime, float, float>(DateTime.UtcNow.Date.AddDays(-10), 100, 50), new Tuple<DateTime, float, float>(DateTime.UtcNow.Date.AddDays(-11), 40, 30));
            });
        }

        [Test]
        public void OneMeasurementFromOneType_Max()
        {
            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 100});
            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 401});

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportMeasurementsTimeParams();
                var list = service.ReportMeasurementsTime(data.Token, param);
                assert(list, new Tuple<DateTime, float, float>(DateTime.UtcNow.Date.AddDays(-10), 401, 0));
            });
        }

        [Test]
        public void TwoMeasurementsFromOneType_Max()
        {
            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 100,Klatka=55 });
            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 401,Klatka = 10});

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportMeasurementsTimeParams();
                var list = service.ReportMeasurementsTime(data.Token, param);
                assert(list, new Tuple<DateTime, float, float>(DateTime.UtcNow.Date.AddDays(-10), 401, 55));
            });
        }

        [Test]
        public void EntriesFromManyCustomersInDb_ReturnDataForSpecificCustomer()
        {
            var customer = CreateCustomer("cust", profiles[0]);
            AddTrainingDaySize(profiles[0], customer, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 100, Klatka = 55 });
            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 401, Klatka = 10 });
            AddTrainingDaySize(profiles[1], null, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 402, Klatka = 11 });

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportMeasurementsTimeParams();
                param.CustomerId = customer.GlobalId;
                var list = service.ReportMeasurementsTime(data.Token, param);
                assert(list, new Tuple<DateTime, float, float>(DateTime.UtcNow.Date.AddDays(-10), 100, 55));
            });
        }

        [Test]
        public void EntriesFromManyProfileInDb_ReturnDataForSpecificUser()
        {
            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 100, Klatka = 55 });
            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 401, Klatka = 10 });
            AddTrainingDaySize(profiles[1], null, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 402, Klatka = 11 });

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportMeasurementsTimeParams();
                var list = service.ReportMeasurementsTime(data.Token, param);
                assert(list, new Tuple<DateTime, float, float>(DateTime.UtcNow.Date.AddDays(-10), 401, 55));
            });
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void ForCustomer_AnotherProfile()
        {
            var customer = CreateCustomer("cust", profiles[0]);
            AddTrainingDaySize(profiles[0], customer, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 100, Klatka = 55 });
            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 401, Klatka = 10 });
            AddTrainingDaySize(profiles[1], null, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 402, Klatka = 11 });

            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportMeasurementsTimeParams();
                param.CustomerId = customer.GlobalId;
                var list = service.ReportMeasurementsTime(data.Token, param);
                assert(list, new Tuple<DateTime, float, float>(DateTime.UtcNow.Date.AddDays(-10), 100, 55));
            });
        }

        [Test]
        public void AnotherProfile_Private()
        {
            setPrivacy(Privacy.Private);
            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 100, Klatka = 55 });
            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 401, Klatka = 10 });
            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 402, Klatka = 11 });

            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportMeasurementsTimeParams();
                param.UserId = profiles[0].GlobalId;
                var list = service.ReportMeasurementsTime(data.Token, param);
                Assert.AreEqual(0,list.Count);
            });
        }

        [Test]
        public void AnotherProfile_Friends_ForUnknownUser()
        {
            setPrivacy(Privacy.FriendsOnly);
            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 100, Klatka = 55 });
            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 401, Klatka = 10 });
            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 402, Klatka = 11 });

            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportMeasurementsTimeParams();
                param.UserId = profiles[0].GlobalId;
                var list = service.ReportMeasurementsTime(data.Token, param);
                Assert.AreEqual(0, list.Count);
            });
        }

        [Test]
        public void AnotherProfile_Friends_ForMyFriend()
        {
            setPrivacy(Privacy.FriendsOnly);
            profiles[0].Friends.Add(profiles[1]);
            profiles[1].Friends.Add(profiles[0]);
            insertToDatabase(profiles[0]);
            insertToDatabase(profiles[1]);

            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 100, Klatka = 55 });
            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 401, Klatka = 10 });
            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-11), new Wymiary() { Height = 402, Klatka = 11 });

            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportMeasurementsTimeParams();
                param.UserId = profiles[0].GlobalId;
                var list = service.ReportMeasurementsTime(data.Token, param);
                Assert.AreEqual(2, list.Count);
            });
        }

        [Test]
        public void UseForReportOnly()
        {
            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 100, Klatka = 55 });
            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 401, Klatka = 10 }, ReportStatus.SkipInReport);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportMeasurementsTimeParams();
                var list = service.ReportMeasurementsTime(data.Token, param);
                assert(list, new Tuple<DateTime, float, float>(DateTime.UtcNow.Date.AddDays(-10), 100, 55));
            });
        }

        [Test]
        public void UseAllEntries()
        {
            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 100, Klatka = 55 });
            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 401, Klatka = 10 }, ReportStatus.SkipInReport);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportMeasurementsTimeParams();
                param.UseAllEntries = true;
                var list = service.ReportMeasurementsTime(data.Token, param);
                assert(list, new Tuple<DateTime, float, float>(DateTime.UtcNow.Date.AddDays(-10), 401, 55));
            });
        }

        [Test]
        public void SkipPlanned()
        {
            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 100, Klatka = 55 });
            
            var entry=AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 401, Klatka = 10 });
            entry.Status = EntryObjectStatus.Planned;
            insertToDatabase(entry);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportMeasurementsTimeParams();
                param.UseAllEntries = true;
                var list = service.ReportMeasurementsTime(data.Token, param);
                assert(list, new Tuple<DateTime, float, float>(DateTime.UtcNow.Date.AddDays(-10), 100, 55));
            });
        }

        [Test]
        public void FromDate()
        {
            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-3), new Wymiary() { Height = 100, Klatka = 55 });
            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 401, Klatka = 10 });
            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-21), new Wymiary() { Height = 402, Klatka = 11 });

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportMeasurementsTimeParams();
                param.StartDate = DateTime.UtcNow.AddDays(-11);
                var list = service.ReportMeasurementsTime(data.Token, param);
                assert(list, new Tuple<DateTime, float, float>(DateTime.UtcNow.Date.AddDays(-3), 100, 55), new Tuple<DateTime, float, float>(DateTime.UtcNow.Date.AddDays(-10),401,10));
            });
            
        }

        [Test]
        public void ToDate()
        {
            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-3), new Wymiary() { Height = 100, Klatka = 55 });
            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 401, Klatka = 10 });
            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-21), new Wymiary() { Height = 402, Klatka = 11 });

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportMeasurementsTimeParams();
                param.EndDate = DateTime.UtcNow.AddDays(-8);
                var list = service.ReportMeasurementsTime(data.Token, param);
                assert(list, new Tuple<DateTime, float, float>(DateTime.UtcNow.Date.AddDays(-21), 402, 11), new Tuple<DateTime, float, float>(DateTime.UtcNow.Date.AddDays(-10), 401, 10));
            });
        }

        void assert(IList<MeasurementsTimeReportResultItem> results, params Tuple<DateTime, float, float>[] expected)
        {
            Assert.AreEqual(expected.Length,results.Count);
            foreach (var tuple in expected)
            {
                var item=results.Where(x => x.DateTime == tuple.Item1).Single();
                Assert.AreEqual(tuple.Item2,item.Wymiary.Height);
                Assert.AreEqual(tuple.Item3, item.Wymiary.Klatka);
            }
        }

        private void setPrivacy(Privacy newPrivacy)
        {
            profiles[0].Privacy.CalendarView = newPrivacy;
            Session.Update(profiles[0]);
            Session.Flush();
            Session.Clear();
        }
    }
}
