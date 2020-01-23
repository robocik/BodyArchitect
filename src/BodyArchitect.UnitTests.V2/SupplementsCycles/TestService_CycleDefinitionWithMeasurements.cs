using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using NUnit.Framework;
using TimeType = BodyArchitect.Model.TimeType;

namespace BodyArchitect.UnitTests.V2.SupplementsCycles
{
    [TestFixture]
    public class TestService_CycleDefinitionWithMeasurements:TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        private SuplementDTO bcaa;
        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                profiles.Add(CreateProfile(Session, "test1"));

                bcaa = CreateSupplement("BCAA").Map<SuplementDTO>();
                tx.Commit();
            }
        }

        private SupplementCycleDefinition createCycle()
        {
            var cycleDefinition = new SupplementCycleDefinitionDTO();
            cycleDefinition.Name = "bcaa";
            cycleDefinition.Language = "en";
            SupplementCycleWeekDTO week = new SupplementCycleWeekDTO();
            week.Name = "Training week";
            week.CycleWeekStart = 1;
            week.CycleWeekEnd = 4;
            cycleDefinition.Weeks.Add(week);
            
            SupplementCycleDosageDTO dosageDto = CreateDosageDTO(5, bcaa, Service.V2.Model.SupplementCycleDayRepetitions.EveryDay,Service.V2.Model.DosageType.Tablets, Service.V2.Model.TimeType.OnEmptyStomach);
            week.Dosages.Add(dosageDto);
            dosageDto = CreateDosageDTO(5, bcaa, Service.V2.Model.SupplementCycleDayRepetitions.EveryDay, Service.V2.Model.DosageType.Tablets, Service.V2.Model.TimeType.BeforeWorkout);
            week.Dosages.Add(dosageDto);
            dosageDto = CreateDosageDTO(5, bcaa, Service.V2.Model.SupplementCycleDayRepetitions.EveryDay, Service.V2.Model.DosageType.Tablets, Service.V2.Model.TimeType.AfterWorkout);
            week.Dosages.Add(dosageDto);

            week = new SupplementCycleWeekDTO();
            week.Name = "Training week";
            week.CycleWeekStart = 1;
            week.CycleWeekEnd = 1;
            cycleDefinition.Weeks.Add(week);

            var measurement = new SupplementCycleMeasurementDTO();
            measurement.Repetitions = Service.V2.Model.SupplementCycleDayRepetitions.OnceAWeek;
            week.Dosages.Add(measurement);

            week = new SupplementCycleWeekDTO();
            week.Name = "Training week";
            week.CycleWeekStart = 5;
            week.CycleWeekEnd = 5;
            cycleDefinition.Weeks.Add(week);
            measurement = new SupplementCycleMeasurementDTO();
            measurement.Repetitions = Service.V2.Model.SupplementCycleDayRepetitions.OnceAWeek;
            week.Dosages.Add(measurement);

            var definition = cycleDefinition.Map<SupplementCycleDefinition>();
            definition.Profile = profiles[0];
            Session.Save(definition);
            return definition;
        }

        [Test]
        public void Test()
        {
            var cycleDef = createCycle();
            DateTime date = new DateTime(2012, 03, 26); //monday
            var cycle = new SupplementsCycleDTO();
            cycle.Name = "Sterydy";
            cycle.StartDate = date;
            cycle.SupplementsCycleDefinitionId = cycleDef.GlobalId;
            var profile1 = (ProfileDTO) profiles[0].Tag;
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
            Assert.AreEqual(30, dbCycle.EntryObjects.Count);
            //Assert.AreEqual(SuplementsEntryDTO.EntryTypeId, dbCycle.TypeId);
            var entries = dbCycle.EntryObjects.OrderBy(x => x.TrainingDay.TrainingDate).ToList();
            Assert.AreEqual(2,entries.OfType<SizeEntry>().Count());
            Assert.AreEqual(28, entries.OfType<SuplementsEntry>().Count());
            foreach (var entry in entries)
            {
                SuplementsEntry suplementEntry = entry as SuplementsEntry;
                if(suplementEntry==null)
                {
                    continue;
                }
                Assert.IsNotNull(entry.LoginData);
                Assert.IsNull(entry.Reminder);

                Assert.AreEqual(5, suplementEntry.Items.ElementAt(0).Dosage);
                Assert.AreEqual(Model.DosageType.Tablets, suplementEntry.Items.ElementAt(0).DosageType);
                Assert.AreEqual(bcaa.GlobalId, suplementEntry.Items.ElementAt(0).Suplement.GlobalId);
                Assert.AreEqual(Model.TimeType.OnEmptyStomach, suplementEntry.Items.ElementAt(0).Time.TimeType);

                Assert.AreEqual(5, suplementEntry.Items.ElementAt(1).Dosage);
                Assert.AreEqual(Model.DosageType.Tablets, suplementEntry.Items.ElementAt(1).DosageType);
                Assert.AreEqual(bcaa.GlobalId, suplementEntry.Items.ElementAt(1).Suplement.GlobalId);
                Assert.AreEqual(Model.TimeType.BeforeWorkout, suplementEntry.Items.ElementAt(1).Time.TimeType);

                Assert.AreEqual(5, suplementEntry.Items.ElementAt(2).Dosage);
                Assert.AreEqual(Model.DosageType.Tablets, suplementEntry.Items.ElementAt(2).DosageType);
                Assert.AreEqual(bcaa.GlobalId, suplementEntry.Items.ElementAt(2).Suplement.GlobalId);
                Assert.AreEqual(Model.TimeType.AfterWorkout, suplementEntry.Items.ElementAt(2).Time.TimeType);

            }
        }
    }
}
