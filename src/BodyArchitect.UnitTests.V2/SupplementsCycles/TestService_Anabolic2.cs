using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using NUnit.Framework;
using SupplementCycleDayRepetitions = BodyArchitect.Service.V2.Model.SupplementCycleDayRepetitions;

namespace BodyArchitect.UnitTests.V2.SupplementsCycles
{
    /// <summary>
    /// http://nabierz-masy.com/przyklady-zastosowania?start=8  (second 2)
    /// </summary>
    [TestFixture]
    public class TestService_Anabolic2 : TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        private SuplementDTO stanozolol;
        private SuplementDTO trenbolon;
        private SuplementDTO testosteron;
        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                profiles.Add(CreateProfile(Session, "test1"));

                stanozolol = CreateSupplement("Stanozolol").Map<SuplementDTO>();
                trenbolon = CreateSupplement("Trenbolon Acetat").Map<SuplementDTO>();
                testosteron = CreateSupplement("Testosteron Propionat").Map<SuplementDTO>();
                tx.Commit();
            }
        }

        private SupplementCycleDefinition createCycle()
        {
            var cycleDefinition = new SupplementCycleDefinitionDTO();
            cycleDefinition.Name = "creatine cycle";
            cycleDefinition.Language = "en";
            SupplementCycleWeekDTO week = new SupplementCycleWeekDTO();
            week.Name = "Training day";
            week.CycleWeekStart = 1;
            week.CycleWeekEnd = 4;
            cycleDefinition.Weeks.Add(week);
            SupplementCycleDosageDTO dosageDto = new SupplementCycleDosageDTO();
            dosageDto.Dosage = 100;
            dosageDto.DosageType = Service.V2.Model.DosageType.MiliGrams;
            dosageDto.Repetitions = SupplementCycleDayRepetitions.EveryDay;
            dosageDto.Supplement = testosteron;
            dosageDto.TimeType = Service.V2.Model.TimeType.NotSet;
            week.Dosages.Add(dosageDto);
            dosageDto = new SupplementCycleDosageDTO();
            dosageDto.Dosage = 75;
            dosageDto.DosageType = Service.V2.Model.DosageType.MiliGrams;
            dosageDto.Supplement = trenbolon;
            dosageDto.Repetitions = SupplementCycleDayRepetitions.EveryDay;
            dosageDto.TimeType = Service.V2.Model.TimeType.NotSet;
            week.Dosages.Add(dosageDto);

            week = new SupplementCycleWeekDTO();
            week.Name = "Training week";
            week.CycleWeekStart = 5;
            week.CycleWeekEnd = 12;
            cycleDefinition.Weeks.Add(week);
            dosageDto = new SupplementCycleDosageDTO();
            dosageDto.Dosage = 100;
            dosageDto.DosageType = Service.V2.Model.DosageType.MiliGrams;
            dosageDto.Repetitions = SupplementCycleDayRepetitions.EveryDay;
            dosageDto.Supplement = testosteron;
            dosageDto.TimeType = Service.V2.Model.TimeType.NotSet;
            week.Dosages.Add(dosageDto);
            dosageDto = new SupplementCycleDosageDTO();
            dosageDto.Dosage = 75;
            dosageDto.DosageType = Service.V2.Model.DosageType.MiliGrams;
            dosageDto.Supplement = trenbolon;
            dosageDto.Repetitions = SupplementCycleDayRepetitions.EveryDay;
            dosageDto.TimeType = Service.V2.Model.TimeType.NotSet;
            week.Dosages.Add(dosageDto);
            dosageDto = new SupplementCycleDosageDTO();
            dosageDto.Dosage = 50;
            dosageDto.DosageType = Service.V2.Model.DosageType.MiliGrams;
            dosageDto.Repetitions = SupplementCycleDayRepetitions.EveryDay;
            dosageDto.Supplement = stanozolol;
            dosageDto.TimeType = Service.V2.Model.TimeType.NotSet;
            week.Dosages.Add(dosageDto);

            var definition = cycleDefinition.Map<SupplementCycleDefinition>();
            definition.Profile = profiles[0];
            Session.Save(definition);
            return definition;
        }

        [Test]
        public void Test()
        {
            var cycleDef = createCycle();
            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new SupplementsCycleDTO();
            cycle.Name = "Sterydy";
            cycle.StartDate = date;
            cycle.SupplementsCycleDefinitionId = cycleDef.GlobalId;
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
            Assert.AreEqual(84, dbCycle.EntryObjects.Count);
            //Assert.AreEqual(SuplementsEntryDTO.EntryTypeId, dbCycle.TypeId);
            var entries = dbCycle.EntryObjects.OrderBy(x => x.TrainingDay.TrainingDate).Cast<SuplementsEntry>().ToList();
            foreach (var entry in entries)
            {
                Assert.IsNotNull(entry.LoginData);
                Assert.IsNull(entry.Reminder);

                var week = (int)((entry.TrainingDay.TrainingDate - dbCycle.StartDate).TotalDays / 7) + 1;
                Assert.AreEqual(week<5?2:3, entry.Items.Count);

                Assert.AreEqual(100, entry.Items.Where(x => x.Suplement.GlobalId == testosteron.GlobalId).Single().Dosage);
                Assert.AreEqual(Model.DosageType.MiliGrams, entry.Items.Where(x => x.Suplement.GlobalId == testosteron.GlobalId).Single().DosageType);
                Assert.AreEqual(Model.TimeType.NotSet, entry.Items.Where(x => x.Suplement.GlobalId == testosteron.GlobalId).Single().Time.TimeType);

                Assert.AreEqual(75, entry.Items.Where(x => x.Suplement.GlobalId == trenbolon.GlobalId).Single().Dosage);
                Assert.AreEqual(Model.DosageType.MiliGrams, entry.Items.Where(x => x.Suplement.GlobalId == trenbolon.GlobalId).Single().DosageType);
                Assert.AreEqual(Model.TimeType.NotSet, entry.Items.Where(x => x.Suplement.GlobalId == trenbolon.GlobalId).Single().Time.TimeType);

                if(week>4)
                {
                    Assert.AreEqual(50, entry.Items.Where(x => x.Suplement.GlobalId == stanozolol.GlobalId).Single().Dosage);
                    Assert.AreEqual(Model.DosageType.MiliGrams, entry.Items.Where(x => x.Suplement.GlobalId == stanozolol.GlobalId).Single().DosageType);
                    Assert.AreEqual(Model.TimeType.NotSet, entry.Items.Where(x => x.Suplement.GlobalId == stanozolol.GlobalId).Single().Time.TimeType);
                }

            }
        }
    }
}
