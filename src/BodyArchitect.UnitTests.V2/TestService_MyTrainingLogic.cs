using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NUnit.Framework;
using EntryObjectStatus = BodyArchitect.Model.EntryObjectStatus;
using ReminderRepetitions = BodyArchitect.Model.ReminderRepetitions;
using TrainingEnd = BodyArchitect.Service.V2.Model.TrainingEnd;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_MyTrainingLogic : TestServiceBase
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
        public void FinishTraining_BySaveTrainingDay()
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

            var dbTrainingDay = Session.QueryOver<TrainingDay>().Fetch(x => x.Objects).Eager.Fetch(x => x.Objects.First().MyTraining).Eager.List();
            for (int i = 0; i < 41; i++)
            {
                dbTrainingDay[i].Objects.ElementAt(0).Status = Model.EntryObjectStatus.Done;
                insertToDatabase(dbTrainingDay[i].Objects.ElementAt(0));
            }
            //mark last entry as Done
            var dtoTrainingDay = dbTrainingDay[41].Map<TrainingDayDTO>();

            dtoTrainingDay.Objects[0].Status = Service.V2.Model.EntryObjectStatus.Done;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                TimerService.UtcNow = DateTime.UtcNow.AddDays(3).Date;
                var res = service.SaveTrainingDay(data.Token, dtoTrainingDay);
                dtoTrainingDay = res.TrainingDay;
            });
            Assert.AreEqual(DateTime.UtcNow.AddDays(3).Date, dtoTrainingDay.Objects[0].MyTraining.EndDate);
            Assert.AreEqual(100, dtoTrainingDay.Objects[0].MyTraining.PercentageCompleted);
            Assert.AreEqual(Service.V2.Model.TrainingEnd.Complete, dtoTrainingDay.Objects[0].MyTraining.TrainingEnd);

            var dbCycle = Session.Get<MyTraining>(result.GlobalId);
            Assert.AreEqual(DateTime.UtcNow.AddDays(3).Date, dbCycle.EndDate);
            Assert.AreEqual(100, dbCycle.PercentageCompleted);
            Assert.AreEqual(Model.TrainingEnd.Complete, dbCycle.TrainingEnd);
        }

        [Test]
        public void SaveTrainingDay_MyTrainingType()
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

            var dbTrainingDay = Session.QueryOver<TrainingDay>().Fetch(x => x.Objects).Eager.Fetch(x => x.Objects.First().MyTraining).Eager.List();
            for (int i = 0; i < 41; i++)
            {
                dbTrainingDay[i].Objects.ElementAt(0).Status = Model.EntryObjectStatus.Done;
                insertToDatabase(dbTrainingDay[i].Objects.ElementAt(0));
            }
            //mark last entry as Done
            var dtoTrainingDay = dbTrainingDay[41].Map<TrainingDayDTO>();

            dtoTrainingDay.Objects[0].Status = Service.V2.Model.EntryObjectStatus.Done;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                TimerService.UtcNow = DateTime.UtcNow.AddDays(3).Date;
                var res = service.SaveTrainingDay(data.Token, dtoTrainingDay);
                dtoTrainingDay = res.TrainingDay;
            });
            Assert.AreEqual(typeof(MyTrainingLightDTO), dtoTrainingDay.Objects[0].MyTraining.GetType());
        }

        [Test]
        public void BreakTraining_BySaveTrainingDay_50Percentage()
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

            var dbTrainingDay = Session.QueryOver<TrainingDay>().Fetch(x => x.Objects).Eager.Fetch(x => x.Objects.First().MyTraining).Eager.List();
            for (int i = 0; i < 20; i++)
            {
                dbTrainingDay[i].Objects.ElementAt(0).Status = Model.EntryObjectStatus.Done;
                insertToDatabase(dbTrainingDay[i].Objects.ElementAt(0));
            }
            //abort in the middle of training
            var dtoTrainingDay = dbTrainingDay[20].Map<TrainingDayDTO>();
            dtoTrainingDay.Objects[0].MyTraining.Complete();
            dtoTrainingDay.Objects[0].Status = Service.V2.Model.EntryObjectStatus.Done;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                TimerService.UtcNow = DateTime.UtcNow.AddDays(3).Date;
                var res = service.SaveTrainingDay(data.Token, dtoTrainingDay);
                dtoTrainingDay = res.TrainingDay;
            });
            var dbCycle = Session.Get<MyTraining>(result.GlobalId);
            Assert.AreEqual(21, dbCycle.EntryObjects.Count);
            Assert.AreEqual(50, dbCycle.PercentageCompleted);
            Assert.AreEqual(21, Session.QueryOver<TrainingDay>().RowCount());
            Assert.AreEqual(21, Session.QueryOver<EntryObject>().RowCount());
            Assert.AreEqual(DateTime.UtcNow.AddDays(3).Date, dbCycle.EndDate);
            Assert.AreEqual(Model.TrainingEnd.Complete, dbCycle.TrainingEnd);
        }

        [Test]
        public void FinishTraining_BySaveTrainingDay_50Percentage()
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

            var dbTrainingDay = Session.QueryOver<TrainingDay>().Fetch(x => x.Objects).Eager.Fetch(x => x.Objects.First().MyTraining).Eager.List();
            for (int i = 0; i < 20; i++)
            {
                dbTrainingDay[i].Objects.ElementAt(0).Status = Model.EntryObjectStatus.Done;
                insertToDatabase(dbTrainingDay[i].Objects.ElementAt(0));
            }
            //mark last entry as Done
            var dtoTrainingDay = dbTrainingDay[41].Map<TrainingDayDTO>();

            dtoTrainingDay.Objects[0].Status =Service.V2.Model.EntryObjectStatus.Done;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                TimerService.UtcNow = DateTime.UtcNow.AddDays(3).Date;
                var res = service.SaveTrainingDay(data.Token, dtoTrainingDay);
                dtoTrainingDay = res.TrainingDay;
            });
            Assert.AreEqual(DateTime.UtcNow.AddDays(3).Date, dtoTrainingDay.Objects[0].MyTraining.EndDate);
            Assert.AreEqual(50, dtoTrainingDay.Objects[0].MyTraining.PercentageCompleted);
            Assert.AreEqual(Service.V2.Model.TrainingEnd.Complete, dtoTrainingDay.Objects[0].MyTraining.TrainingEnd);

            var dbCycle = Session.Get<MyTraining>(result.GlobalId);
            Assert.AreEqual(DateTime.UtcNow.AddDays(3).Date, dbCycle.EndDate);
            Assert.AreEqual(50, dbCycle.PercentageCompleted);
            Assert.AreEqual(Model.TrainingEnd.Complete, dbCycle.TrainingEnd);
        }

        [Test]
        [ExpectedException(typeof(TrainingIntegrationException))]
        public void AddAnotherEntryToTraining()
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

            TrainingDayDTO day = new TrainingDayDTO(DateTime.UtcNow.AddDays(50));
            A6WEntryDTO entry = new A6WEntryDTO();
            entry.Day = A6WManager.Days[0];
            entry.MyTraining = result;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                TimerService.UtcNow = DateTime.UtcNow.AddDays(3).Date;
                service.SaveTrainingDay(data.Token, day);
            });
        }

        [Test]
        public void CreateNewTrainingInSaveTrainingDay()
        {
            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new A6WTrainingDTO();
            cycle.Name = "My A6W";
            cycle.StartDate = date;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);


            TrainingDayDTO day = new TrainingDayDTO(DateTime.UtcNow.AddDays(50));
            A6WEntryDTO entry = new A6WEntryDTO();
            entry.Day = A6WManager.Days[0];
            entry.MyTraining = cycle;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var result = service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            var count=Session.QueryOver<MyTrainingDTO>().RowCount();
            Assert.AreEqual(0,count);
            var dbDay=Session.Get<TrainingDay>(day.GlobalId);
            Assert.IsNull(dbDay.Objects.ElementAt(0).MyTraining);
        }

        [Test]
        [ExpectedException(typeof(TrainingIntegrationException))]
        public void DeleteEntryFromTraining_SaveTrainingDay()
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

            var dbTrainingDay = Session.QueryOver<TrainingDay>().Fetch(x => x.Objects).Eager.Fetch(x => x.Objects.First().MyTraining).Eager.List();
            for (int i = 0; i < 41; i++)
            {
                dbTrainingDay[i].Objects.ElementAt(0).Status = Model.EntryObjectStatus.Done;
                insertToDatabase(dbTrainingDay[i].Objects.ElementAt(0));
            }
            var dtoTrainingDay = dbTrainingDay[40].Map<TrainingDayDTO>();

            dtoTrainingDay.Objects.RemoveAt(0);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                TimerService.UtcNow = DateTime.UtcNow.AddDays(3).Date;
                service.SaveTrainingDay(data.Token, dtoTrainingDay);
            });
        }

        [Test]
        [ExpectedException(typeof(TrainingIntegrationException))]
        public void DeleteTrainingDayWithTrainingEntry()
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

            var dbTrainingDay = Session.QueryOver<TrainingDay>().Fetch(x => x.Objects).Eager.Fetch(x => x.Objects.First().MyTraining).Eager.List();
            for (int i = 0; i < 41; i++)
            {
                dbTrainingDay[i].Objects.ElementAt(0).Status = Model.EntryObjectStatus.Done;
                insertToDatabase(dbTrainingDay[i].Objects.ElementAt(0));
            }
            var dtoTrainingDay = dbTrainingDay[40].Map<TrainingDayDTO>();

            dtoTrainingDay.Objects.RemoveAt(0);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                DeleteTrainingDayParam param = new DeleteTrainingDayParam();
                param.TrainingDayId = dtoTrainingDay.GlobalId;
                param.Mode = DeleteTrainingDayMode.All;
                TimerService.UtcNow = DateTime.UtcNow.AddDays(3).Date;
                service.DeleteTrainingDay(data.Token, param);
            });
        }

        [Test]
        public void StopTraining()
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
                param.Operation = MyTrainingOperationType.Stop;
                param.MyTraining = result;
                result = service.MyTrainingOperation(data.Token, param);
            });

            var dbCycle = Session.Get<MyTraining>(result.GlobalId);
            Assert.AreEqual(0, dbCycle.EntryObjects.Count);
            Assert.AreEqual(0, dbCycle.PercentageCompleted);
            Assert.AreEqual(0, Session.QueryOver<TrainingDay>().RowCount());
            Assert.AreEqual(0, Session.QueryOver<EntryObject>().RowCount());
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void StopTraining_AnotherProfile()
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
            profile1 = (ProfileDTO)profiles[1].Tag;
            data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Stop;
                param.MyTraining = result;
                service.MyTrainingOperation(data.Token, param);
            });
        }

        [Test]
        public void StopTraining_WithReminders()
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
                param.Operation = MyTrainingOperationType.Stop;
                param.MyTraining = result;
                result = service.MyTrainingOperation(data.Token, param);
            });

            Assert.AreEqual(0, Session.QueryOver<ReminderItem>().RowCount());
        }

        
        [Test]
        public void SaveOnceEntryAsDoneAndStopTraining()
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

            var dbCycle = Session.Get<MyTraining>(result.GlobalId);
            Assert.AreEqual(1, dbCycle.EntryObjects.Count);
            Assert.AreEqual(2, dbCycle.PercentageCompleted);
            Assert.AreEqual(1, Session.QueryOver<TrainingDay>().RowCount());
            Assert.AreEqual(1, Session.QueryOver<EntryObject>().RowCount());
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

            dtoTrainingDay.Objects[0].Status =Service.V2.Model.EntryObjectStatus.Done;
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

        [Test]
        public void StartA6WTraining_WithReminders()
        {
            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new A6WTrainingDTO();
            cycle.Name = "My A6W";
            cycle.StartDate = date;
            cycle.RemindBefore = TimeSpan.FromMinutes(15);
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

            foreach (var entry in entries)
            {
                Assert.AreEqual(entry.TrainingDay.TrainingDate, entry.Reminder.DateTime.Date);
                Assert.AreEqual(TimeSpan.FromMinutes(15), entry.Reminder.RemindBefore);
                Assert.AreEqual("EntryObjectDTO:" + entry.GlobalId, entry.Reminder.ConnectedObject);
                Assert.AreEqual(ReminderRepetitions.Once, entry.Reminder.Repetitions);
            }
        }


        [Test]
        public void StartA6WTraining_WithReminders_ForCustomer()
        {
            var cust = CreateCustomer("cust",profiles[0]);
            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new A6WTrainingDTO();
            cycle.Name = "My A6W";
            cycle.StartDate = date;
            cycle.RemindBefore = TimeSpan.FromMinutes(15);
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
            var dbCycle = Session.Get<MyTraining>(result.GlobalId);
            var entries = dbCycle.EntryObjects.OrderBy(x => x.TrainingDay.TrainingDate).Cast<A6WEntry>().ToList();

            Assert.AreEqual(cust.GlobalId,dbCycle.Customer.GlobalId);
            foreach (var entry in entries)
            {
                Assert.AreEqual(entry.TrainingDay.TrainingDate, entry.Reminder.DateTime.Date);
                Assert.AreEqual(cust.GlobalId,entry.TrainingDay.Customer.GlobalId);
                Assert.AreEqual(TimeSpan.FromMinutes(15), entry.Reminder.RemindBefore);
                Assert.AreEqual("EntryObjectDTO:" + entry.GlobalId, entry.Reminder.ConnectedObject);
                Assert.AreEqual(ReminderRepetitions.Once, entry.Reminder.Repetitions);
            }
            //if we start training for customers then profile statistic shouldn't be changed
            var dbProfile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.AreEqual(0, dbProfile.Statistics.TrainingDaysCount);
            Assert.IsNull(dbProfile.Statistics.LastEntryDate);
            Assert.AreEqual(0, dbProfile.Statistics.A6WFullCyclesCount);
            Assert.AreEqual(0, dbProfile.Statistics.A6WEntriesCount);
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void StartA6WTraining_WithReminders_ForCustomer_OtherProfile()
        {
            var cust = CreateCustomer("cust", profiles[1]);
            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new A6WTrainingDTO();
            cycle.Name = "My A6W";
            cycle.StartDate = date;
            cycle.RemindBefore = TimeSpan.FromMinutes(15);
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
    }
}
