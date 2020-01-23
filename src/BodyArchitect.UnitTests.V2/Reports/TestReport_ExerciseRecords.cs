using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Reports;
using NUnit.Framework;

namespace BodyArchitect.UnitTests.V2.Reports
{
    [TestFixture]
    public class TestReport_ExerciseRecords : TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        List<Exercise> exercises = new List<Exercise>();

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                exercises.Clear();
                profiles.Add(CreateProfile(Session, "test1"));
                profiles.Add(CreateProfile(Session, "test2"));
                profiles.Add(CreateProfile(Session, "test3"));
                profiles.Add(CreateProfile(Session, "test4"));
                profiles.Add(CreateProfile(Session, "test5"));
                profiles[0].Statistics.StrengthTrainingEntriesCount = 30;
                profiles[1].Statistics.StrengthTrainingEntriesCount = 50;
                profiles[2].Statistics.TrainingDaysCount = 50;
                profiles[3].Statistics.TrainingDaysCount = 130;
                profiles[4].Statistics.StrengthTrainingEntriesCount = 660;
                var ex = CreateExercise(Session, null, "ex1", "ex1");
                exercises.Add(ex);
                ex = CreateExercise(Session, null, "ex2", "ex2");
                exercises.Add(ex);
                tx.Commit();
            }
        }

    
        [Test]
        public void ForFriends()
        {
            CreateExerciseRecord(exercises[1], profiles[0], new Tuple<int, decimal>(5, 65), DateTime.UtcNow.AddDays(-15).Date);
            CreateExerciseRecord(exercises[0], profiles[0], new Tuple<int, decimal>(11, 40), DateTime.UtcNow.AddDays(-10).Date);
            CreateExerciseRecord(exercises[0], profiles[2], new Tuple<int, decimal>(3, 40), DateTime.UtcNow.AddDays(-11).Date);

            CreateExerciseRecord(exercises[0], profiles[1], new Tuple<int, decimal>(5, 67), DateTime.UtcNow.AddDays(-4).Date);

            CreateExerciseRecord(exercises[0], profiles[3], new Tuple<int, decimal>(3, 61), DateTime.UtcNow.AddDays(-5).Date);

            profiles[0].Friends.Add(profiles[2]);
            profiles[2].Friends.Add(profiles[0]);
            profiles[0].Friends.Add(profiles[3]);
            profiles[3].Friends.Add(profiles[0]);
            insertToDatabase(profiles[0]);
            insertToDatabase(profiles[2]);
            insertToDatabase(profiles[3]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ExerciseRecordsParams();
                param.ExerciseId = exercises[0].GlobalId;
                param.Mode = RecordMode.Friends;
                var list = service.ReportExerciseRecords(data.Token, param, new PartialRetrievingInfo());

                assert(list,new Tuple<Profile, decimal, DateTime>(profiles[3], 61, DateTime.UtcNow.AddDays(-5).Date),
                            new Tuple<Profile, decimal, DateTime>(profiles[0], 40, DateTime.UtcNow.AddDays(-10).Date),
                            new Tuple<Profile, decimal, DateTime>(profiles[2], 40, DateTime.UtcNow.AddDays(-11).Date));
            });
        }

        [Test]
        public void ForUsers()
        {
            CreateExerciseRecord(exercises[1], profiles[0], new Tuple<int, decimal>(5, 65), DateTime.UtcNow.AddDays(-15).Date);
            CreateExerciseRecord(exercises[0], profiles[0], new Tuple<int, decimal>(11, 40), DateTime.UtcNow.AddDays(-10).Date);
            CreateExerciseRecord(exercises[0], profiles[2], new Tuple<int, decimal>(3, 40), DateTime.UtcNow.AddDays(-11).Date);

            CreateExerciseRecord(exercises[0], profiles[1], new Tuple<int, decimal>(5, 67), DateTime.UtcNow.AddDays(-4).Date);

            CreateExerciseRecord(exercises[0], profiles[3], new Tuple<int, decimal>(3, 61), DateTime.UtcNow.AddDays(-5).Date);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ExerciseRecordsParams();
                param.ExerciseId = exercises[0].GlobalId;
                var list = service.ReportExerciseRecords(data.Token, param, new PartialRetrievingInfo());

                assert(list, new Tuple<Profile, decimal, DateTime>(profiles[1], 67, DateTime.UtcNow.AddDays(-4).Date),
                            new Tuple<Profile, decimal, DateTime>(profiles[3], 61, DateTime.UtcNow.AddDays(-5).Date),
                            new Tuple<Profile, decimal, DateTime>(profiles[0], 40, DateTime.UtcNow.AddDays(-10).Date),
                            new Tuple<Profile, decimal, DateTime>(profiles[2], 40, DateTime.UtcNow.AddDays(-11).Date));
            });
        }


        [Test]
        public void ForUsers_SkipUserWithSmallStatistics()
        {
            profiles[1].Statistics.StrengthTrainingEntriesCount = 0;
            profiles[1].Statistics.TrainingDaysCount = 0;
            insertToDatabase(profiles[1].Statistics);
            CreateExerciseRecord(exercises[1], profiles[0], new Tuple<int, decimal>(5, 65), DateTime.UtcNow.AddDays(-15).Date);
            CreateExerciseRecord(exercises[0], profiles[0], new Tuple<int, decimal>(11, 40), DateTime.UtcNow.AddDays(-10).Date);
            CreateExerciseRecord(exercises[0], profiles[2], new Tuple<int, decimal>(3, 40), DateTime.UtcNow.AddDays(-11).Date);

            CreateExerciseRecord(exercises[0], profiles[1], new Tuple<int, decimal>(5, 67), DateTime.UtcNow.AddDays(-4).Date);

            CreateExerciseRecord(exercises[0], profiles[3], new Tuple<int, decimal>(3, 61), DateTime.UtcNow.AddDays(-5).Date);


            var test=Session.QueryOver<Profile>().Fetch(x => x.Statistics).Eager.List();
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ExerciseRecordsParams();
                param.ExerciseId = exercises[0].GlobalId;
                var list = service.ReportExerciseRecords(data.Token, param, new PartialRetrievingInfo());

                assert(list,new Tuple<Profile, decimal, DateTime>(profiles[3], 61, DateTime.UtcNow.AddDays(-5).Date),
                            new Tuple<Profile, decimal, DateTime>(profiles[0], 40, DateTime.UtcNow.AddDays(-10).Date),
                            new Tuple<Profile, decimal, DateTime>(profiles[2], 40, DateTime.UtcNow.AddDays(-11).Date));
            });
        }

        [Test]
        public void ForUsers_SkipCustomers()
        {
            var customer = CreateCustomer("cust",profiles[0]);

            CreateExerciseRecord(exercises[0], profiles[0], new Tuple<int, decimal>(5, 65), DateTime.UtcNow.AddDays(-15).Date, customer);
            CreateExerciseRecord(exercises[0], profiles[0], new Tuple<int, decimal>(11, 40), DateTime.UtcNow.AddDays(-10).Date);
            CreateExerciseRecord(exercises[0], profiles[2], new Tuple<int, decimal>(3, 40), DateTime.UtcNow.AddDays(-11).Date);

            CreateExerciseRecord(exercises[0], profiles[1], new Tuple<int, decimal>(5, 67), DateTime.UtcNow.AddDays(-4).Date);

            CreateExerciseRecord(exercises[0], profiles[3], new Tuple<int, decimal>(3, 61), DateTime.UtcNow.AddDays(-5).Date);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ExerciseRecordsParams();
                param.ExerciseId = exercises[0].GlobalId;
                var list = service.ReportExerciseRecords(data.Token, param, new PartialRetrievingInfo());

                assert(list, new Tuple<Profile, decimal, DateTime>(profiles[1], 67, DateTime.UtcNow.AddDays(-4).Date),
                            new Tuple<Profile, decimal, DateTime>(profiles[3], 61, DateTime.UtcNow.AddDays(-5).Date),
                            new Tuple<Profile, decimal, DateTime>(profiles[0], 40, DateTime.UtcNow.AddDays(-10).Date),
                            new Tuple<Profile, decimal, DateTime>(profiles[2], 40, DateTime.UtcNow.AddDays(-11).Date));
            });
        }

        

        [Test]
        public void ForCustomers()
        {
            var customer = CreateCustomer("cust", profiles[0]);
            var customer1 = CreateCustomer("cust1", profiles[0]);
            var customer2 = CreateCustomer("cust1", profiles[1]);

            CreateExerciseRecord(exercises[1], profiles[0], new Tuple<int, decimal>(5, 65), DateTime.UtcNow.AddDays(-15).Date, customer);
            CreateExerciseRecord(exercises[0], profiles[0], new Tuple<int, decimal>(11, 40), DateTime.UtcNow.AddDays(-10).Date, customer);
            CreateExerciseRecord(exercises[0], profiles[0], new Tuple<int, decimal>(3, 40), DateTime.UtcNow.AddDays(-11).Date, customer1);

            CreateExerciseRecord(exercises[0], profiles[1], new Tuple<int, decimal>(5, 67), DateTime.UtcNow.AddDays(-4).Date, customer2);

            CreateExerciseRecord(exercises[0], profiles[1], new Tuple<int, decimal>(3, 61), DateTime.UtcNow.AddDays(-5).Date);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ExerciseRecordsParams();
                param.ExerciseId = exercises[0].GlobalId;
                param.Mode = RecordMode.Customer;
                var list = service.ReportExerciseRecords(data.Token, param, new PartialRetrievingInfo());

                assert(list, new Tuple<Customer, decimal, DateTime>(customer, 40, DateTime.UtcNow.AddDays(-10).Date),
                            new Tuple<Customer, decimal, DateTime>(customer1, 40, DateTime.UtcNow.AddDays(-11).Date));
            });
        }

        [Test]
        public void ForCustomers_UserWithSmallStatistics()
        {
            var customer = CreateCustomer("cust", profiles[0]);
            var customer1 = CreateCustomer("cust1", profiles[0]);
            var customer2 = CreateCustomer("cust1", profiles[1]);

            profiles[0].Statistics.StrengthTrainingEntriesCount = 0;
            profiles[0].Statistics.TrainingDaysCount = 0;
            insertToDatabase(profiles[0].Statistics);


            CreateExerciseRecord(exercises[1], profiles[0], new Tuple<int, decimal>(5, 65), DateTime.UtcNow.AddDays(-15).Date, customer);
            CreateExerciseRecord(exercises[0], profiles[0], new Tuple<int, decimal>(11, 40), DateTime.UtcNow.AddDays(-10).Date, customer);
            CreateExerciseRecord(exercises[0], profiles[0], new Tuple<int, decimal>(3, 40), DateTime.UtcNow.AddDays(-11).Date, customer1);

            CreateExerciseRecord(exercises[0], profiles[1], new Tuple<int, decimal>(5, 67), DateTime.UtcNow.AddDays(-4).Date, customer2);

            CreateExerciseRecord(exercises[0], profiles[1], new Tuple<int, decimal>(3, 61), DateTime.UtcNow.AddDays(-5).Date);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ExerciseRecordsParams();
                param.ExerciseId = exercises[0].GlobalId;
                param.Mode = RecordMode.Customer;
                var list = service.ReportExerciseRecords(data.Token, param, new PartialRetrievingInfo());

                assert(list, new Tuple<Customer, decimal, DateTime>(customer, 40, DateTime.UtcNow.AddDays(-10).Date),
                            new Tuple<Customer, decimal, DateTime>(customer1, 40, DateTime.UtcNow.AddDays(-11).Date));
            });
        }

        [Test]
        public void Pager()
        {
            CreateExerciseRecord(exercises[1], profiles[0], new Tuple<int, decimal>(5, 65), DateTime.UtcNow.AddDays(-15).Date);
            CreateExerciseRecord(exercises[0], profiles[0], new Tuple<int, decimal>(11, 40), DateTime.UtcNow.AddDays(-10).Date);
            CreateExerciseRecord(exercises[0], profiles[2], new Tuple<int, decimal>(3, 40), DateTime.UtcNow.AddDays(-11).Date);

            CreateExerciseRecord(exercises[0], profiles[1], new Tuple<int, decimal>(5, 67), DateTime.UtcNow.AddDays(-4).Date);

            CreateExerciseRecord(exercises[0], profiles[3], new Tuple<int, decimal>(3, 61), DateTime.UtcNow.AddDays(-5).Date);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ExerciseRecordsParams();
                param.ExerciseId = exercises[0].GlobalId;
                var list = service.ReportExerciseRecords(data.Token, param, new PartialRetrievingInfo(){PageSize = 2});

                Assert.AreEqual(2, list.Items.Count);
                Assert.AreEqual(4,list.AllItemsCount);

                assert(list, new Tuple<Profile, decimal, DateTime>(profiles[1], 67, DateTime.UtcNow.AddDays(-4).Date),
                            new Tuple<Profile, decimal, DateTime>(profiles[3], 61, DateTime.UtcNow.AddDays(-5).Date));
            });

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ExerciseRecordsParams();
                param.ExerciseId = exercises[0].GlobalId;
                var list = service.ReportExerciseRecords(data.Token, param, new PartialRetrievingInfo() { PageSize = 2,PageIndex = 1});

                Assert.AreEqual(2, list.Items.Count);
                Assert.AreEqual(4, list.AllItemsCount);

                assert(list, new Tuple<Profile, decimal, DateTime>(profiles[0], 40, DateTime.UtcNow.AddDays(-10).Date),
                            new Tuple<Profile, decimal, DateTime>(profiles[2], 40, DateTime.UtcNow.AddDays(-11).Date));
            });
        }

        void assert(PagedResult<ExerciseRecordsReportResultItem> results, params Tuple<Profile,decimal,DateTime >[] weights)
        {
            Assert.AreEqual(weights.Length,results.Items.Count);
            for (int i = 0; i < weights.Length; i++)
            {
                Assert.IsTrue(results.Items[i].MaxWeight == weights[i].Item2);
                Assert.IsTrue(results.Items[i].User.GlobalId == weights[i].Item1.GlobalId);
                Assert.IsTrue(results.Items[i].TrainingDate == weights[i].Item3);
            }
        }

        void assert(PagedResult<ExerciseRecordsReportResultItem> results, params Tuple<Customer, decimal,DateTime>[] weights)
        {
            Assert.AreEqual(weights.Length, results.Items.Count);
            for (int i = 0; i < weights.Length; i++)
            {
                Assert.IsTrue(results.Items[i].MaxWeight == weights[i].Item2);
                Assert.IsTrue(results.Items[i].CustomerId == weights[i].Item1.GlobalId);
                Assert.IsTrue(results.Items[i].TrainingDate == weights[i].Item3);
            }
        }
    }
}
