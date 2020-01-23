using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NUnit.Framework;
using EntryObjectStatus = BodyArchitect.Service.V2.Model.EntryObjectStatus;
using ReminderRepetitions = BodyArchitect.Model.ReminderRepetitions;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_A6WTraining:TestServiceBase
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
        public void StartA6WTraining()
        {
            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new A6WTrainingDTO();
            cycle.Name = "My A6W";
            cycle.StartDate = date;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            MyTrainingDTO result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Start;
                param.MyTraining = cycle;
                result = service.MyTrainingOperation(data.Token, param);
            });
            var dbCycle = Session.Get<MyTraining>(result.GlobalId);
            Assert.AreEqual(42, dbCycle.EntryObjects.Count);
            //Assert.AreEqual(SuplementsEntryDTO.EntryTypeId, dbCycle.TypeId);
            var entries = dbCycle.EntryObjects.OrderBy(x => x.TrainingDay.TrainingDate).Cast<A6WEntry>().ToList();
            Assert.AreEqual(date, entries.ElementAt(0).TrainingDay.TrainingDate);
            Assert.AreEqual(date.AddDays(41), entries.ElementAt(41).TrainingDay.TrainingDate);
            foreach (var entry in entries)
            {
                Assert.AreEqual(Model.EntryObjectStatus.Planned, entry.Status);
            }
        }

        [Test]
        public void StartA6WTraining_ForCustomer_Statistics()
        {
            var cust=CreateCustomer("test",profiles[0]);
            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new A6WTrainingDTO();
            cycle.Name = "My A6W";
            cycle.StartDate = date;
            cycle.CustomerId = cust.GlobalId;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            MyTrainingDTO result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Start;
                param.MyTraining = cycle;
                result = service.MyTrainingOperation(data.Token, param);
            });
            var dbProfile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.AreEqual(0, dbProfile.Statistics.A6WEntriesCount);
        }

        [Test]
        public void StartA6WTraining_ForCustomer()
        {
            var cust = CreateCustomer("test", profiles[0]);
            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new A6WTrainingDTO();
            cycle.Name = "My A6W";
            cycle.StartDate = date;
            cycle.CustomerId = cust.GlobalId;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            MyTrainingDTO result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Start;
                param.MyTraining = cycle;
                result = service.MyTrainingOperation(data.Token, param);
            });
            Assert.AreEqual(cust.GlobalId,result.CustomerId.Value);
            var a6w=Session.Get<A6WTraining>(result.GlobalId);
            Assert.AreEqual(cust.GlobalId, a6w.Customer.GlobalId);
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void StartA6WTraining_ForCustomerFromAnotherProfile()
        {
            var cust = CreateCustomer("test", profiles[1]);
            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new A6WTrainingDTO();
            cycle.Name = "My A6W";
            cycle.StartDate = date;
            cycle.CustomerId = cust.GlobalId;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Start;
                param.MyTraining = cycle;
                service.MyTrainingOperation(data.Token, param);
            });
        }

        [Test]
        public void StartA6WTraining_ForCustomer_EntriesBelongsToCustomer()
        {
            var cust = CreateCustomer("test", profiles[0]);
            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new A6WTrainingDTO();
            cycle.Name = "My A6W";
            cycle.StartDate = date;
            cycle.CustomerId = cust.GlobalId;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            MyTrainingDTO result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Start;
                param.MyTraining = cycle;
                result = service.MyTrainingOperation(data.Token, param);
            });
            var a6w = Session.Get<A6WTraining>(result.GlobalId);
            foreach (var entryObject in a6w.EntryObjects)
            {
                Assert.AreEqual(cust.GlobalId,entryObject.TrainingDay.Customer.GlobalId);
                Assert.AreEqual(profiles[0].GlobalId, entryObject.TrainingDay.Profile.GlobalId);
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void StartAlreadyStartedA6WTraining()
        {
            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new A6WTrainingDTO();
            cycle.Name = "My A6W";
            cycle.StartDate = date;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            MyTrainingDTO result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Start;
                param.MyTraining = cycle;
                result = service.MyTrainingOperation(data.Token, param);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Start;
                param.MyTraining = result;
                service.MyTrainingOperation(data.Token, param);
            });
        }

        [Test]
        public void StartA6WTraining_Entries()
        {
            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new A6WTrainingDTO();
            cycle.Name = "My A6W";
            cycle.StartDate = date;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            MyTrainingDTO result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Start;
                param.MyTraining = cycle;
                result = service.MyTrainingOperation(data.Token, param);
            });
            var dbCycle = Session.Get<MyTraining>(result.GlobalId);
            var entries = dbCycle.EntryObjects.OrderBy(x => x.TrainingDay.TrainingDate).Cast<A6WEntry>().ToList();

            for (int index = 0; index < entries.Count; index++)
            {
                var entry = entries[index];
                Assert.AreEqual(index+1,entry.DayNumber);
                Assert.IsNull(entry.Reminder);
                Assert.AreEqual(Model.EntryObjectStatus.Planned, entry.Status);
            }
        }

        [Test]
        public void SimulateA6WTraining_ResultEntries()
        {
            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new A6WTrainingDTO();
            cycle.Name = "My A6W";
            cycle.StartDate = date;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            MyTrainingDTO result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Simulate;
                param.MyTraining = cycle;
                result = service.MyTrainingOperation(data.Token, param);
            });
            var entries = result.EntryObjects.OrderBy(x => x.TrainingDay.TrainingDate).ToList();

            for (int index = 0; index < entries.Count; index++)
            {
                var entry = (A6WEntryDTO)entries[index];
                Assert.AreEqual(index + 1, entry.Day.DayNumber);
                Assert.IsNull(entry.RemindBefore);
                Assert.AreEqual(EntryObjectStatus.Planned, entry.Status);
            }
        }

        [Test]
        public void SimulateA6WTraining_EntriesInDb()
        {
            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new A6WTrainingDTO();
            cycle.Name = "My A6W";
            cycle.StartDate = date;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            MyTrainingDTO result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Simulate;
                param.MyTraining = cycle;
                result = service.MyTrainingOperation(data.Token, param);
            });
            Assert.AreEqual(0, Session.QueryOver<A6WEntry>().RowCount());
            Assert.AreEqual(0, Session.QueryOver<TrainingDay>().RowCount());
            Assert.AreEqual(0, Session.QueryOver<SupplementCycle>().RowCount());
        }

        [Test]
        public void StartA6WTraining_ResultEntries()
        {
            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new A6WTrainingDTO();
            cycle.Name = "My A6W";
            cycle.StartDate = date;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            MyTrainingDTO result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Start;
                param.MyTraining = cycle;
                result = service.MyTrainingOperation(data.Token, param);
            });
            var entries = result.EntryObjects.OrderBy(x => x.TrainingDay.TrainingDate).ToList();

            for (int index = 0; index < entries.Count; index++)
            {
                var entry = (A6WEntryDTO)entries[index];
                Assert.AreEqual(index + 1, entry.Day.DayNumber);
                Assert.IsNull(entry.RemindBefore);
                Assert.AreEqual(EntryObjectStatus.Planned, entry.Status);
            }
        }

        [Test]
        public void StartA6WTraining_TrainingDayStatistics()
        {
            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new A6WTrainingDTO();
            cycle.Name = "My A6W";
            cycle.StartDate = date;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            MyTrainingDTO result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Start;
                param.MyTraining = cycle;
                result = service.MyTrainingOperation(data.Token, param);
            });
            var dbProfile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.AreEqual(42, dbProfile.Statistics.TrainingDaysCount);
            Assert.IsNotNull(dbProfile.Statistics.LastEntryDate);
            Assert.AreEqual(0, dbProfile.Statistics.A6WFullCyclesCount);
            Assert.AreEqual(42, dbProfile.Statistics.A6WEntriesCount);
        }

        [Test]
        public void FinishA6WTraining_Statistics_FullyCompleted()
        {
            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new A6WTrainingDTO();
            cycle.Name = "My A6W";
            cycle.StartDate = date;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            MyTrainingDTO result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Start;
                param.MyTraining = cycle;
                result = service.MyTrainingOperation(data.Token, param);
            });
            Run(s=>
                    {
                        for (int index = 0; index < result.EntryObjects.Count-1; index++)
                        {
                            var entryObjectDto = result.EntryObjects[index];
                            var entry = s.Get<EntryObject>(entryObjectDto.GlobalId);
                            entry.Status = Model.EntryObjectStatus.Done;
                            s.Update(entry);
                        }
                    });

            TrainingDayDTO dayDTO = Session.Get<TrainingDay>(result.EntryObjects[41].TrainingDay.GlobalId).Map<TrainingDayDTO>();
            RunServiceMethod(delegate(InternalBodyArchitectService service)
                                 {
                                     dayDTO.Objects[0].Status = EntryObjectStatus.Done;
                service.SaveTrainingDay(data.Token, dayDTO);
            });
            var dbProfile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.AreEqual(42, dbProfile.Statistics.TrainingDaysCount);
            Assert.AreEqual(1, dbProfile.Statistics.A6WFullCyclesCount);
            Assert.AreEqual(42, dbProfile.Statistics.A6WEntriesCount);
        }

        [Test]
        public void FinishA6WTraining_Statistics_PartiallyCompleted()
        {
            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new A6WTrainingDTO();
            cycle.Name = "My A6W";
            cycle.StartDate = date;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            MyTrainingDTO result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Start;
                param.MyTraining = cycle;
                result = service.MyTrainingOperation(data.Token, param);
            });
            Run(s =>
            {
                for (int index = 0; index < 12; index++)
                {
                    var entryObjectDto = result.EntryObjects[index];
                    var entry = s.Get<EntryObject>(entryObjectDto.GlobalId);
                    entry.Status = Model.EntryObjectStatus.Done;
                    s.Update(entry);
                }
            });

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Stop;
                param.MyTraining = result;
                result = service.MyTrainingOperation(data.Token, param);
            });
            var dbProfile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.AreEqual(12, dbProfile.Statistics.TrainingDaysCount);
            Assert.AreEqual(0, dbProfile.Statistics.A6WFullCyclesCount);
            Assert.AreEqual(12, dbProfile.Statistics.A6WEntriesCount);
        }

        [Test]
        [Ignore]
        public void SaveOnceEntryAsDoneAndStopTraining_TrainingDayStatistics()
        {
            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new A6WTrainingDTO();
            cycle.Name = "My A6W";
            cycle.StartDate = date;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            MyTrainingDTO result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Start;
                param.MyTraining = cycle;
                result = service.MyTrainingOperation(data.Token, param);
            });

            var dbTrainingDay = Session.QueryOver<TrainingDay>().Where(x => x.TrainingDate == date).Fetch(x => x.Objects).Eager.Fetch(x => x.Objects.First().MyTraining).Eager.SingleOrDefault();
            var dtoTrainingDay = dbTrainingDay.Map<TrainingDayDTO>();

            dtoTrainingDay.Objects[0].Status = Service.V2.Model.EntryObjectStatus.Done;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                dtoTrainingDay = service.SaveTrainingDay(data.Token, dtoTrainingDay).TrainingDay;
            });

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Stop;
                param.MyTraining = result;
                result = service.MyTrainingOperation(data.Token, param);
            });

            var dbProfile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.AreEqual(1, dbProfile.Statistics.TrainingDaysCount);
            Assert.IsNotNull(dbProfile.Statistics.LastEntryDate);
            Assert.AreEqual(0, dbProfile.Statistics.A6WFullCyclesCount);
            Assert.AreEqual(1, dbProfile.Statistics.A6WEntriesCount);
        }

        
    }
}
