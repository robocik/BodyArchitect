using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NUnit.Framework;
using DosageType = BodyArchitect.Service.V2.Model.DosageType;
using EntryObjectStatus = BodyArchitect.Service.V2.Model.EntryObjectStatus;
using ReminderRepetitions = BodyArchitect.Model.ReminderRepetitions;
using SupplementCycleDayRepetitions = BodyArchitect.Service.V2.Model.SupplementCycleDayRepetitions;
using TimeType = BodyArchitect.Model.TimeType;
using TrainingEnd = BodyArchitect.Service.V2.Model.TrainingEnd;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_SupplementsCycle:TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        private SupplementCycleDefinition definition;
        private Suplement supplement;
        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                profiles.Add(CreateProfile(Session, "test1"));
                profiles.Add(CreateProfile(Session, "test2"));

                supplement=CreateSupplement("creatine");
                
                tx.Commit();
            }
        }

        #region Start/Stop

        [Test]
        public void StartNewCycle_WithoutTrainingDays()
        {
            definition = createCreating2DT1DNTDefinition();

            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new SupplementsCycleDTO();
            cycle.Name = "Creatine";
            cycle.StartDate = date;
            cycle.SupplementsCycleDefinitionId = definition.GlobalId;
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
            var dbCycle = Session.Get<SupplementCycle>(result.GlobalId);
            Assert.AreEqual(28, dbCycle.EntryObjects.Count);
        }

        [Test]
        public void StartNewCycle_WithEmptyTrainingDays()
        {
            definition = createCreating2DT1DNTDefinition();

            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new SupplementsCycleDTO();
            cycle.Name = "Creatine";
            cycle.StartDate = date;
            cycle.TrainingDays = "";
            cycle.SupplementsCycleDefinitionId = definition.GlobalId;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            MyTrainingDTO result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                MyTrainingOperationParam param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Start;
                
                param.MyTraining = cycle;
                result = service.MyTrainingOperation(data.Token, param);
            });
            var dbCycle = Session.Get<SupplementCycle>(result.GlobalId);
            Assert.AreEqual(28, dbCycle.EntryObjects.Count);
        }
        [Test]
        public void StartNewCycle()
        {
            definition = createCreating2DT1DNTDefinition();

            DateTime date=new DateTime(2012,03,26);//monday
            var cycle = new SupplementsCycleDTO();
            cycle.TrainingDays = string.Format("{0}S,{1}S,{2}S", (int)DayOfWeek.Monday, (int)DayOfWeek.Wednesday, (int)DayOfWeek.Friday);
            cycle.Name = "Creatine";
            cycle.StartDate = date;
            cycle.SupplementsCycleDefinitionId = definition.GlobalId;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            MyTrainingDTO result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                MyTrainingOperationParam param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Start;
                
                param.MyTraining = cycle;
                result=service.MyTrainingOperation(data.Token, param);
            });
            var dbCycle = Session.Get<SupplementCycle>(result.GlobalId);
            Assert.AreEqual(28, dbCycle.EntryObjects.Count);
            //Assert.AreEqual(SuplementsEntryDTO.EntryTypeId, dbCycle.TypeId);
            var entries=dbCycle.EntryObjects.OrderBy(x => x.TrainingDay.TrainingDate).Cast<SuplementsEntry>().ToList();
            Assert.AreEqual(date, entries.ElementAt(0).TrainingDay.TrainingDate);
            Assert.AreEqual("dosageName", entries[0].Items.ElementAt(0).Name);
            Assert.IsNull(entries[0].Items.ElementAt(1).Name);
            Assert.AreEqual(date.AddDays(27), entries.ElementAt(27).TrainingDay.TrainingDate);
            foreach (var entry in entries)
            {
                Assert.AreEqual(Model.EntryObjectStatus.Planned,entry.Status);
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void StartNewCycle_WithoutStartDate()
        {
            definition = createCreating2DT1DNTDefinition();

            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new SupplementsCycleDTO();
            cycle.TrainingDays = string.Format("{0},{1},{2}", (int)DayOfWeek.Monday, (int)DayOfWeek.Wednesday, (int)DayOfWeek.Friday);
            cycle.Name = "Creatine";
            cycle.SupplementsCycleDefinitionId = definition.GlobalId;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            MyTrainingDTO result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                MyTrainingOperationParam param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Start;
                
                param.MyTraining = cycle;
                result = service.MyTrainingOperation(data.Token, param);
            });
        }

        [Test]
        public void StartNewCycle_Entries()
        {
            definition = createCreating2DT1DNTDefinition();

            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new SupplementsCycleDTO();
            cycle.TrainingDays = string.Format("{0}S,{1}S,{2}S", (int)DayOfWeek.Monday, (int)DayOfWeek.Wednesday, (int)DayOfWeek.Friday);
            cycle.Name = "Creatine";
            cycle.StartDate = date;
            cycle.SupplementsCycleDefinitionId = definition.GlobalId;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            MyTrainingDTO result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                MyTrainingOperationParam param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Start;
                
                param.MyTraining = cycle;
                result = service.MyTrainingOperation(data.Token, param);
            });
            Assert.AreEqual(28, result.EntryObjects.Count);
            //Assert.AreEqual(SuplementsEntryDTO.EntryTypeId, dbCycle.TypeId);
            var entries = result.EntryObjects.OrderBy(x => x.TrainingDay.TrainingDate).ToList();
            Assert.AreEqual(date, entries.ElementAt(0).TrainingDay.TrainingDate);
            Assert.AreEqual(date.AddDays(27), entries.ElementAt(27).TrainingDay.TrainingDate);
            foreach (var entry in entries)
            {
                Assert.AreEqual(EntryObjectStatus.Planned, entry.Status);
            }
        }

        [Test]
        public void StartNewCycle_AllowCommentsSetting_False()
        {
            profiles[0].Settings.AllowTrainingDayComments = false;
            insertToDatabase(profiles[0].Settings);

            definition = createCreating2DT1DNTDefinition();

            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new SupplementsCycleDTO();
            cycle.TrainingDays = string.Format("{0}S,{1}S,{2}S", (int)DayOfWeek.Monday, (int)DayOfWeek.Wednesday, (int)DayOfWeek.Friday);
            cycle.Name = "Creatine";
            cycle.StartDate = date;
            cycle.SupplementsCycleDefinitionId = definition.GlobalId;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            MyTrainingDTO result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                MyTrainingOperationParam param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Start;

                param.MyTraining = cycle;
                result = service.MyTrainingOperation(data.Token, param);
            });
            //Assert.AreEqual(SuplementsEntryDTO.EntryTypeId, dbCycle.TypeId);
            var entries = result.EntryObjects.OrderBy(x => x.TrainingDay.TrainingDate).ToList();
            foreach (var entry in entries)
            {
                Assert.IsFalse(entry.TrainingDay.AllowComments);
            }
        }

        [Test]
        public void StartNewCycle_AllowCommentsSetting_True()
        {
            profiles[0].Settings.AllowTrainingDayComments = true;
            insertToDatabase(profiles[0].Settings);

            definition = createCreating2DT1DNTDefinition();

            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new SupplementsCycleDTO();
            cycle.TrainingDays = string.Format("{0}S,{1}S,{2}S", (int)DayOfWeek.Monday, (int)DayOfWeek.Wednesday, (int)DayOfWeek.Friday);
            cycle.Name = "Creatine";
            cycle.StartDate = date;
            cycle.SupplementsCycleDefinitionId = definition.GlobalId;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            MyTrainingDTO result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                MyTrainingOperationParam param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Start;

                param.MyTraining = cycle;
                result = service.MyTrainingOperation(data.Token, param);
            });
            //Assert.AreEqual(SuplementsEntryDTO.EntryTypeId, dbCycle.TypeId);
            var entries = result.EntryObjects.OrderBy(x => x.TrainingDay.TrainingDate).ToList();
            foreach (var entry in entries)
            {
                Assert.IsTrue(entry.TrainingDay.AllowComments);
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void StartNewCycle_WithEndDate()
        {
            definition = createCreating2DT1DNTDefinition();

            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new SupplementsCycleDTO();
            cycle.TrainingDays = string.Format("{0},{1},{2}", (int)DayOfWeek.Monday, (int)DayOfWeek.Wednesday, (int)DayOfWeek.Friday);
            cycle.Name = "Creatine";
            cycle.SetData(date,DateTime.UtcNow.AddDays(20),TrainingEnd.NotEnded);
            cycle.StartDate = date;
            cycle.SupplementsCycleDefinitionId = definition.GlobalId;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);


            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                MyTrainingOperationParam param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Start;
                
                param.MyTraining = cycle;
                service.MyTrainingOperation(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void StartNewCycle_WithTrainingEnd()
        {
            definition = createCreating2DT1DNTDefinition();

            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new SupplementsCycleDTO();
            cycle.TrainingDays = string.Format("{0},{1},{2}", (int)DayOfWeek.Monday, (int)DayOfWeek.Wednesday, (int)DayOfWeek.Friday);
            cycle.Name = "Creatine";
            cycle.SetData(date, null, TrainingEnd.Complete);
            cycle.StartDate = date;
            cycle.SupplementsCycleDefinitionId = definition.GlobalId;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);


            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                MyTrainingOperationParam param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Start;
                
                param.MyTraining = cycle;
                service.MyTrainingOperation(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void StartNewCycle_WithPercentage()
        {
            definition = createCreating2DT1DNTDefinition();

            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new SupplementsCycleDTO();
            cycle.TrainingDays = string.Format("{0},{1},{2}", (int)DayOfWeek.Monday, (int)DayOfWeek.Wednesday, (int)DayOfWeek.Friday);
            cycle.Name = "Creatine";
            cycle.PercentageCompleted = 30;
            cycle.StartDate = date;
            cycle.SupplementsCycleDefinitionId = definition.GlobalId;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);


            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                MyTrainingOperationParam param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Start;
                
                param.MyTraining = cycle;
                service.MyTrainingOperation(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void StartNewCycle_CustomerFromAnotherProfile()
        {
            definition = createCreating2DT1DNTDefinition();

            var customer = CreateCustomer("ccc", profiles[1]);
            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new SupplementsCycleDTO();
            cycle.TrainingDays = string.Format("{0},{1},{2}", (int)DayOfWeek.Monday, (int)DayOfWeek.Wednesday, (int)DayOfWeek.Friday);
            cycle.Name = "Creatine";
            cycle.CustomerId = customer.GlobalId;
            cycle.StartDate = date;
            cycle.SupplementsCycleDefinitionId = definition.GlobalId;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);


            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                MyTrainingOperationParam param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Start;
                
                param.MyTraining = cycle;
                service.MyTrainingOperation(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void StartAlreadyStartedCycle()
        {
            definition = createCreating2DT1DNTDefinition();

            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new SupplementsCycleDTO();
            cycle.TrainingDays = string.Format("{0}S,{1}S,{2}S", (int)DayOfWeek.Monday, (int)DayOfWeek.Wednesday, (int)DayOfWeek.Friday);
            cycle.Name = "Creatine";
            cycle.StartDate = date;
            cycle.SupplementsCycleDefinitionId = definition.GlobalId;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            MyTrainingDTO result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                MyTrainingOperationParam param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Start;
                
                param.MyTraining = cycle;
                result = service.MyTrainingOperation(data.Token, param);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                MyTrainingOperationParam param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Start;
                
                param.MyTraining = (SupplementsCycleDTO) result;
                service.MyTrainingOperation(data.Token, param);
            });
        }

        [Test]
        public void StartNewCycle_Statistics()
        {
            definition = createCreating2DT1DNTDefinition();

            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new SupplementsCycleDTO();
            cycle.TrainingDays = string.Format("{0}S,{1}S,{2}S", (int)DayOfWeek.Monday, (int)DayOfWeek.Wednesday, (int)DayOfWeek.Friday);
            cycle.Name = "Creatine";
            cycle.StartDate = date;
            cycle.SupplementsCycleDefinitionId = definition.GlobalId;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            MyTrainingDTO result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                MyTrainingOperationParam param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Start;
                
                param.MyTraining = cycle;
                result = service.MyTrainingOperation(data.Token, param);
            });
            var dbProfile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.AreEqual(28, dbProfile.Statistics.TrainingDaysCount);
            Assert.IsNotNull(dbProfile.Statistics.LastEntryDate);
            Assert.AreEqual(28, dbProfile.Statistics.SupplementEntriesCount);
        }

        [Test]
        public void StartNewCycle_WithReminders()
        {
            definition = createCreating2DT1DNTDefinition();

            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new SupplementsCycleDTO();
            cycle.TrainingDays = string.Format("{0}S,{1}S,{2}S", (int)DayOfWeek.Monday, (int)DayOfWeek.Wednesday, (int)DayOfWeek.Friday);
            cycle.Name = "Creatine";
            cycle.StartDate = date;
            cycle.RemindBefore = TimeSpan.FromMinutes(15);
            cycle.SupplementsCycleDefinitionId = definition.GlobalId;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            MyTrainingDTO result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                MyTrainingOperationParam param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Start;
                
                param.MyTraining = cycle;
                result = service.MyTrainingOperation(data.Token, param);
            });
            var dbCycle = Session.Get<SupplementCycle>(result.GlobalId);
            var entries = dbCycle.EntryObjects.OrderBy(x => x.TrainingDay.TrainingDate).Cast<SuplementsEntry>().ToList();
            foreach (var entry in entries)
            {
                Assert.AreEqual(entry.TrainingDay.TrainingDate,entry.Reminder.DateTime.Date);
                Assert.AreEqual(TimeSpan.FromMinutes(15), entry.Reminder.RemindBefore);
                Assert.AreEqual("EntryObjectDTO:" + entry.GlobalId, entry.Reminder.ConnectedObject);
                Assert.AreEqual(ReminderRepetitions.Once, entry.Reminder.Repetitions);
            }
        }


        [Test]
        public void SaveOnceEntryAsDoneAndStopCycle_Statistics()
        {
            definition = createCreating2DT1DNTDefinition();

            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new SupplementsCycleDTO();
            cycle.TrainingDays = string.Format("{0}S,{1}S,{2}S", (int)DayOfWeek.Monday, (int)DayOfWeek.Wednesday, (int)DayOfWeek.Friday);
            cycle.Name = "Creatine";
            cycle.StartDate = date;
            cycle.SupplementsCycleDefinitionId = definition.GlobalId;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            MyTrainingDTO result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                MyTrainingOperationParam param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Start;
                
                param.MyTraining = cycle;
                result = service.MyTrainingOperation(data.Token, param);
            });

            var dbTrainingDay = Session.QueryOver<TrainingDay>().Where(x => x.TrainingDate == date).Fetch(x => x.Objects).Eager.Fetch(x => x.Objects.First().MyTraining).Eager.SingleOrDefault();
            var dtoTrainingDay = dbTrainingDay.Map<TrainingDayDTO>();
            dtoTrainingDay.Objects[0].Status = EntryObjectStatus.Done;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                dtoTrainingDay = service.SaveTrainingDay(data.Token, dtoTrainingDay).TrainingDay;
            });

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                MyTrainingOperationParam param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Stop;
                param.MyTraining = (SupplementsCycleDTO) result;
                result = service.MyTrainingOperation(data.Token, param);
            });
            var dbProfile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.AreEqual(1, dbProfile.Statistics.TrainingDaysCount);
            Assert.IsNotNull(dbProfile.Statistics.LastEntryDate);
            Assert.AreEqual(1, dbProfile.Statistics.SupplementEntriesCount);
        }

        [Test]
        public void StartNewCycle_Longer_ShouldUseRepetition()
        {
            definition = createCycleForRepetitions();

            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new SupplementsCycleDTO();
            cycle.TrainingDays = string.Format("{0}S,{1}S,{2}S", (int)DayOfWeek.Monday, (int)DayOfWeek.Wednesday, (int)DayOfWeek.Friday);
            cycle.Name = "Creatine";
            cycle.StartDate = date;
            cycle.TotalWeeks = 14;
            cycle.SupplementsCycleDefinitionId = definition.GlobalId;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            MyTrainingDTO result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                MyTrainingOperationParam param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Start;

                param.MyTraining = cycle;
                result = service.MyTrainingOperation(data.Token, param);
            });
            var dbCycle = Session.Get<SupplementCycle>(result.GlobalId);
            var entries = dbCycle.EntryObjects.OrderBy(x => x.TrainingDay.TrainingDate).Cast<SuplementsEntry>().ToList();
            Assert.AreEqual(14,entries.Count);
            
            Assert.AreEqual(400, entries[7].Items.ElementAt(0).Dosage);
            Assert.AreEqual(400, entries[8].Items.ElementAt(0).Dosage);

            Assert.AreEqual(300, entries[9].Items.ElementAt(0).Dosage);
            Assert.AreEqual(300, entries[10].Items.ElementAt(0).Dosage);

            Assert.AreEqual(400, entries[11].Items.ElementAt(0).Dosage);
            Assert.AreEqual(400, entries[12].Items.ElementAt(0).Dosage);

            Assert.AreEqual(300, entries[13].Items.ElementAt(0).Dosage);
        }

        [Test]
        [ExpectedException(typeof(TrainingIntegrationException))]
        public void StartNewCycle_Longer_PlanDoesntSupportLongerCycles()
        {
            definition = createCycleForRepetitions1();

            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new SupplementsCycleDTO();
            cycle.TrainingDays = string.Format("{0}S,{1}S,{2}S", (int)DayOfWeek.Monday, (int)DayOfWeek.Wednesday, (int)DayOfWeek.Friday);
            cycle.Name = "Creatine";
            cycle.StartDate = date;
            cycle.TotalWeeks = 14;
            cycle.SupplementsCycleDefinitionId = definition.GlobalId;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                MyTrainingOperationParam param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Start;

                param.MyTraining = cycle;
                 service.MyTrainingOperation(data.Token, param);
            });
        }

        [Test]
        public void StartNewCycle_Longer_ShouldUseRepetition_DoNotChangeDefinitionInDb()
        {
            definition = createCycleForRepetitions();

            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new SupplementsCycleDTO();
            cycle.TrainingDays = string.Format("{0}S,{1}S,{2}S", (int)DayOfWeek.Monday, (int)DayOfWeek.Wednesday, (int)DayOfWeek.Friday);
            cycle.Name = "Creatine";
            cycle.StartDate = date;
            cycle.TotalWeeks = 14;
            cycle.SupplementsCycleDefinitionId = definition.GlobalId;
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            MyTrainingDTO result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                MyTrainingOperationParam param = new MyTrainingOperationParam();
                param.Operation = MyTrainingOperationType.Start;

                param.MyTraining = cycle;
                result = service.MyTrainingOperation(data.Token, param);
            });
            var dbCycle = Session.Get<SupplementCycleDefinition>(definition.GlobalId);
            Assert.AreEqual(4,dbCycle.Weeks.Count);
        }
        #endregion

        #region Simulate

        [Test]
        public void SimulateNewCycle_EntriesFromResult()
        {
            definition = createCreating2DT1DNTDefinition();

            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new SupplementsCycleDTO();
            cycle.TrainingDays = string.Format("{0}S,{1}S,{2}S", (int)DayOfWeek.Monday, (int)DayOfWeek.Wednesday, (int)DayOfWeek.Friday);
            cycle.Name = "Creatine";
            cycle.StartDate = date;
            cycle.SupplementsCycleDefinitionId = definition.GlobalId;
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
            Assert.AreEqual(28, result.EntryObjects.Count);
            //Assert.AreEqual(SuplementsEntryDTO.EntryTypeId, dbCycle.TypeId);
            var entries = result.EntryObjects.OrderBy(x => x.TrainingDay.TrainingDate).ToList();
            Assert.AreEqual(date, entries.ElementAt(0).TrainingDay.TrainingDate);
            Assert.AreEqual(date.AddDays(27), entries.ElementAt(27).TrainingDay.TrainingDate);
            foreach (var entry in entries)
            {
                Assert.AreEqual(EntryObjectStatus.Planned, entry.Status);
            }
        }

        [Test]
        public void SimulateNewCycle_EntriesInDb()
        {
            definition = createCreating2DT1DNTDefinition();

            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new SupplementsCycleDTO();
            cycle.TrainingDays = string.Format("{0}S,{1}S,{2}S", (int)DayOfWeek.Monday, (int)DayOfWeek.Wednesday, (int)DayOfWeek.Friday);
            cycle.Name = "Creatine";
            cycle.StartDate = date;
            cycle.SupplementsCycleDefinitionId = definition.GlobalId;
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
            Assert.AreEqual(0, Session.QueryOver<SupplementCycle>().RowCount());
            Assert.AreEqual(0, Session.QueryOver<TrainingDay>().RowCount());
            Assert.AreEqual(0, Session.QueryOver<SupplementCycle>().RowCount());
        }

        #endregion

        private SupplementCycleDefinition createCycleForRepetitions()
        {
            var suppleDTO = supplement.Map<SuplementDTO>();
            var cycleDefinition = new SupplementCycleDefinitionDTO();
            cycleDefinition.Name = "sterydy";
            cycleDefinition.Language = "en";
            SupplementCycleWeekDTO week = new SupplementCycleWeekDTO();
            week.Name = "Training week";
            week.CycleWeekStart = 1;
            week.CycleWeekEnd = 2;
            cycleDefinition.Weeks.Add(week);
            SupplementCycleDosageDTO dosageDto = CreateDosageDTO(400, suppleDTO);
            week.Dosages.Add(dosageDto);

            week = new SupplementCycleWeekDTO();
            week.Name = "Training week";
            week.CycleWeekStart = 3;
            week.CycleWeekEnd = 4;
            week.IsRepetitable = false;
            cycleDefinition.Weeks.Add(week);
            dosageDto = CreateDosageDTO(401, suppleDTO);
            week.Dosages.Add(dosageDto);

            week = new SupplementCycleWeekDTO();
            week.Name = "Training week";
            week.CycleWeekStart = 5;
            week.CycleWeekEnd = 6;
            cycleDefinition.Weeks.Add(week);
            dosageDto = CreateDosageDTO(300, suppleDTO);
            week.Dosages.Add(dosageDto);

            week = new SupplementCycleWeekDTO();
            week.Name = "Training week";
            week.CycleWeekStart = 7;
            week.CycleWeekEnd = 7;
            week.IsRepetitable = false;
            cycleDefinition.Weeks.Add(week);
            dosageDto = CreateDosageDTO(410, suppleDTO);
            week.Dosages.Add(dosageDto);


            var definition = cycleDefinition.Map<SupplementCycleDefinition>();
            definition.Profile = profiles[0];
            Session.Save(definition);
            return definition;
        }

        private SupplementCycleDefinition createCycleForRepetitions1()
        {
            var suppleDTO = supplement.Map<SuplementDTO>();
            var cycleDefinition = new SupplementCycleDefinitionDTO();
            cycleDefinition.Name = "sterydy";
            cycleDefinition.Language = "en";
            SupplementCycleWeekDTO week = new SupplementCycleWeekDTO();
            week.Name = "Training week";
            week.CycleWeekStart = 1;
            week.CycleWeekEnd = 2;
            week.IsRepetitable = false;
            cycleDefinition.Weeks.Add(week);
            SupplementCycleDosageDTO dosageDto = CreateDosageDTO(400, suppleDTO);
            week.Dosages.Add(dosageDto);

            week = new SupplementCycleWeekDTO();
            week.Name = "Training week";
            week.CycleWeekStart = 3;
            week.CycleWeekEnd = 4;
            week.IsRepetitable = false;
            cycleDefinition.Weeks.Add(week);
            dosageDto = CreateDosageDTO(401, suppleDTO);
            week.Dosages.Add(dosageDto);

            week = new SupplementCycleWeekDTO();
            week.Name = "Training week";
            week.CycleWeekStart = 5;
            week.CycleWeekEnd = 6;
            week.IsRepetitable = false;
            cycleDefinition.Weeks.Add(week);
            dosageDto = CreateDosageDTO(300, suppleDTO);
            week.Dosages.Add(dosageDto);

            week = new SupplementCycleWeekDTO();
            week.Name = "Training week";
            week.CycleWeekStart = 7;
            week.CycleWeekEnd = 7;
            week.IsRepetitable = false;
            cycleDefinition.Weeks.Add(week);
            dosageDto = CreateDosageDTO(410, suppleDTO);
            week.Dosages.Add(dosageDto);


            var definition = cycleDefinition.Map<SupplementCycleDefinition>();
            definition.Profile = profiles[0];
            Session.Save(definition);
            return definition;
        }

        private SupplementCycleDefinition createCreating2DT1DNTDefinition()
        {
            var supplementDTO = supplement.Map<SuplementDTO>();
            var cycleDefinition = new SupplementCycleDefinitionDTO();
            cycleDefinition.Name = "creatine cycle";
            cycleDefinition.Language = "en";
            SupplementCycleWeekDTO trainingWeek = new SupplementCycleWeekDTO();
            trainingWeek.Name = "Training week";
            trainingWeek.CycleWeekStart = 1;
            trainingWeek.CycleWeekEnd = 4;
            cycleDefinition.Weeks.Add(trainingWeek);
            SupplementCycleDosageDTO dosageDto = new SupplementCycleDosageDTO();
            dosageDto.Dosage = 5;
            dosageDto.Name = "dosageName";
            dosageDto.Repetitions = Service.V2.Model.SupplementCycleDayRepetitions.StrengthTrainingDay;
            dosageDto.DosageType = DosageType.Grams;
            dosageDto.Supplement = supplementDTO;
            dosageDto.TimeType = Service.V2.Model.TimeType.OnEmptyStomach;
            trainingWeek.Dosages.Add(dosageDto);
            dosageDto = new SupplementCycleDosageDTO();
            dosageDto.Dosage = 5;
            dosageDto.Repetitions = Service.V2.Model.SupplementCycleDayRepetitions.StrengthTrainingDay;
            dosageDto.DosageType = DosageType.Grams;
            dosageDto.Supplement = supplementDTO;
            dosageDto.TimeType = Service.V2.Model.TimeType.AfterWorkout;
            trainingWeek.Dosages.Add(dosageDto);

            dosageDto = new SupplementCycleDosageDTO();
            dosageDto.Dosage = 5;
            dosageDto.Repetitions = Service.V2.Model.SupplementCycleDayRepetitions.NonTrainingDay;
            dosageDto.DosageType = DosageType.Grams;
            dosageDto.Supplement = supplementDTO;
            dosageDto.TimeType = Service.V2.Model.TimeType.OnEmptyStomach;
            trainingWeek.Dosages.Add(dosageDto);

            var definition = cycleDefinition.Map<SupplementCycleDefinition>();
            definition.Profile = profiles[0];
            Session.Save(definition);
            return definition;
        }

        [Test]
        public void CreatineCycle_2DT_1DNT()
        {
            definition = createCreating2DT1DNTDefinition();

            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new SupplementsCycleDTO();
            cycle.TrainingDays = string.Format("{0}S,{1}S,{2}S", (int)DayOfWeek.Monday, (int)DayOfWeek.Wednesday, (int)DayOfWeek.Friday);
            cycle.Name = "Creatine";
            cycle.StartDate = date;
            cycle.SupplementsCycleDefinitionId = definition.GlobalId;
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
            var dbCycle = Session.Get<SupplementCycle>(result.GlobalId);
            Assert.AreEqual(28, dbCycle.EntryObjects.Count);
            //Assert.AreEqual(SuplementsEntryDTO.EntryTypeId, dbCycle.TypeId);
            var entries = dbCycle.EntryObjects.OrderBy(x => x.TrainingDay.TrainingDate).Cast<SuplementsEntry>().ToList();
            foreach (var entry in entries)
            {
                Assert.IsNotNull(entry.LoginData);
                Assert.IsNull(entry.Reminder);
                if (entry.TrainingDay.TrainingDate.DayOfWeek == DayOfWeek.Monday ||
                    entry.TrainingDay.TrainingDate.DayOfWeek == DayOfWeek.Wednesday ||
                    entry.TrainingDay.TrainingDate.DayOfWeek == DayOfWeek.Friday)
                {
                    Assert.AreEqual(2, entry.Items.Count);
                    var item = entry.Items.Where(x => x.Time.TimeType == Model.TimeType.OnEmptyStomach).Single();
                    Assert.AreEqual(5, item.Dosage);
                    Assert.AreEqual(Model.DosageType.Grams, item.DosageType);
                    Assert.AreEqual(supplement.GlobalId, item.Suplement.GlobalId);

                    item = entry.Items.Where(x => x.Time.TimeType == Model.TimeType.AfterWorkout).Single();
                    Assert.AreEqual(5, item.Dosage);
                    Assert.AreEqual(Model.DosageType.Grams, item.DosageType);
                    Assert.AreEqual(supplement.GlobalId, item.Suplement.GlobalId);
                }
                else
                {
                    Assert.AreEqual(1, entry.Items.Count);
                    var item = entry.Items.Where(x => x.Time.TimeType == Model.TimeType.OnEmptyStomach).Single();
                    Assert.AreEqual(5, item.Dosage);
                    Assert.AreEqual(Model.DosageType.Grams, item.DosageType);
                    Assert.AreEqual(supplement.GlobalId, item.Suplement.GlobalId);
                }
            }
        }
    }
}
