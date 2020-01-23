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
    /// http://nabierz-masy.com/przyklady-zastosowania?start=8   (first)
    /// </summary>
    [TestFixture]
    public class TestService_Anabolic1:TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        private SuplementDTO metenolon;
        private SuplementDTO testosteron;
        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                profiles.Add(CreateProfile(Session, "test1"));

                metenolon = CreateSupplement("Metenolon Enantat").Map < SuplementDTO>();
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
            week.Name = "Training week";
            week.CycleWeekStart = 1;
            week.CycleWeekEnd = 1;
            cycleDefinition.Weeks.Add(week);
            SupplementCycleDosageDTO dosageDto = new SupplementCycleDosageDTO();
            dosageDto.Dosage = 600;
            dosageDto.DosageType = Service.V2.Model.DosageType.MiliGrams;
            dosageDto.Supplement = metenolon;
            dosageDto.Repetitions = SupplementCycleDayRepetitions.OnceAWeek;
            dosageDto.TimeType = Service.V2.Model.TimeType.NotSet;
            week.Dosages.Add(dosageDto);
            dosageDto = new SupplementCycleDosageDTO();
            dosageDto.Dosage = 50;
            dosageDto.DosageType = Service.V2.Model.DosageType.MiliGrams;
            dosageDto.Repetitions = SupplementCycleDayRepetitions.EveryDay;
            dosageDto.Supplement = testosteron;
            dosageDto.TimeType = Service.V2.Model.TimeType.NotSet;
            week.Dosages.Add(dosageDto);

            week = new SupplementCycleWeekDTO();
            week.Name = "Training week";
            week.CycleWeekStart = 2;
            week.CycleWeekEnd = 16;
            cycleDefinition.Weeks.Add(week);
            dosageDto = new SupplementCycleDosageDTO();
            dosageDto.Dosage = 300;
            dosageDto.DosageType = Service.V2.Model.DosageType.MiliGrams;
            dosageDto.Repetitions = SupplementCycleDayRepetitions.OnceAWeek;
            dosageDto.Supplement = metenolon;
            dosageDto.TimeType = Service.V2.Model.TimeType.NotSet;
            week.Dosages.Add(dosageDto);
            dosageDto = new SupplementCycleDosageDTO();
            dosageDto.Dosage = 50;
            dosageDto.DosageType = Service.V2.Model.DosageType.MiliGrams;
            dosageDto.Repetitions = SupplementCycleDayRepetitions.EveryDay;
            dosageDto.Supplement = testosteron;
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
            Assert.AreEqual(112, dbCycle.EntryObjects.Count);
            //Assert.AreEqual(SuplementsEntryDTO.EntryTypeId, dbCycle.TypeId);
            var entries = dbCycle.EntryObjects.OrderBy(x => x.TrainingDay.TrainingDate).Cast<SuplementsEntry>().ToList();
            foreach (var entry in entries)
            {
                Assert.IsNotNull(entry.LoginData);
                Assert.IsNull(entry.Reminder);
                
                var week=(int)((entry.TrainingDay.TrainingDate - dbCycle.StartDate).TotalDays / 7)+ 1;
                bool isMonday = entry.TrainingDay.TrainingDate.DayOfWeek == DayOfWeek.Monday;
                Assert.AreEqual(isMonday?2:1, entry.Items.Count);
                if (isMonday)
                {
                    Assert.AreEqual(week == 1 ? 600 : 300, entry.Items.Where(x => x.Suplement.GlobalId == metenolon.GlobalId).Single().Dosage);

                    Assert.AreEqual(Model.DosageType.MiliGrams, entry.Items.Where(x => x.Suplement.GlobalId == metenolon.GlobalId).Single().DosageType);
                    Assert.AreEqual(Model.TimeType.NotSet, entry.Items.Where(x => x.Suplement.GlobalId == metenolon.GlobalId).Single().Time.TimeType);

                    Assert.AreEqual(50, entry.Items.Where(x => x.Suplement.GlobalId == testosteron.GlobalId).Single().Dosage);
                    Assert.AreEqual(Model.DosageType.MiliGrams, entry.Items.Where(x => x.Suplement.GlobalId == testosteron.GlobalId).Single().DosageType);
                    Assert.AreEqual(Model.TimeType.NotSet, entry.Items.Where(x => x.Suplement.GlobalId == testosteron.GlobalId).Single().Time.TimeType);
                }
                else
                {
                    Assert.AreEqual(50, entry.Items.Where(x => x.Suplement.GlobalId == testosteron.GlobalId).Single().Dosage);
                    Assert.AreEqual(Model.DosageType.MiliGrams, entry.Items.Where(x => x.Suplement.GlobalId == testosteron.GlobalId).Single().DosageType);
                    Assert.AreEqual(Model.TimeType.NotSet, entry.Items.Where(x => x.Suplement.GlobalId == testosteron.GlobalId).Single().Time.TimeType);
                }
                

            }
        }
    }
}
