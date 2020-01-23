using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using NUnit.Framework;
using DosageType = BodyArchitect.Model.DosageType;
using SupplementCycleDayRepetitions = BodyArchitect.Service.V2.Model.SupplementCycleDayRepetitions;

namespace BodyArchitect.UnitTests.V2.SupplementsCycles
{
    [TestFixture]
    public class TestService_CreatineCycle_2DT : TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        private Suplement supplement;

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                profiles.Add(CreateProfile(Session, "test1"));

                supplement = CreateSupplement("creatine");
                tx.Commit();
            }


        }

        private SupplementCycleDefinition createCreating2DTDefinition()
        {
            var supplementDTO = supplement.Map<SuplementDTO>();
            var cycleDefinition = new SupplementCycleDefinitionDTO();
            cycleDefinition.Language = "en";
            cycleDefinition.Name = "creatine cycle";
            SupplementCycleWeekDTO trainingWeek = new SupplementCycleWeekDTO();
            trainingWeek.Name = "Training week";
            trainingWeek.CycleWeekStart = 1;
            trainingWeek.CycleWeekEnd = 4;
            cycleDefinition.Weeks.Add(trainingWeek);
            SupplementCycleDosageDTO dosageDto = new SupplementCycleDosageDTO();
            dosageDto.Dosage = 5;
            dosageDto.DosageType = Service.V2.Model.DosageType.Grams;
            dosageDto.Supplement = supplementDTO;
            dosageDto.Repetitions = SupplementCycleDayRepetitions.StrengthTrainingDay;
            dosageDto.TimeType = Service.V2.Model.TimeType.OnEmptyStomach;
            trainingWeek.Dosages.Add(dosageDto);
            dosageDto = new SupplementCycleDosageDTO();
            dosageDto.Dosage = 5;
            dosageDto.DosageType = Service.V2.Model.DosageType.Grams;
            dosageDto.Repetitions = SupplementCycleDayRepetitions.StrengthTrainingDay;
            dosageDto.Supplement = supplementDTO;
            dosageDto.TimeType = Service.V2.Model.TimeType.BeforeSleep;
            trainingWeek.Dosages.Add(dosageDto);

            var definition = cycleDefinition.Map<SupplementCycleDefinition>();
            definition.Profile = profiles[0];
            Session.Save(definition);
            return definition;
        }

        [Test]
        public void CreatineCycle_2DT_0DNT()
        {
            var cycleDef = createCreating2DTDefinition();
            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new SupplementsCycleDTO();
            cycle.TrainingDays = string.Format("{0}S,{1}S,{2}B", (int)DayOfWeek.Monday, (int)DayOfWeek.Wednesday, (int)DayOfWeek.Friday);
            cycle.Name = "Creatine";
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
            Assert.AreEqual(12, dbCycle.EntryObjects.Count);
            //Assert.AreEqual(SuplementsEntryDTO.EntryTypeId, dbCycle.TypeId);
            var entries = dbCycle.EntryObjects.OrderBy(x => x.TrainingDay.TrainingDate).Cast<SuplementsEntry>().ToList();
            foreach (var entry in entries)
            {
                Assert.IsNotNull(entry.LoginData);
                Assert.IsNull(entry.Reminder);
                Assert.IsTrue(entry.TrainingDay.TrainingDate.DayOfWeek == DayOfWeek.Monday ||
                entry.TrainingDay.TrainingDate.DayOfWeek == DayOfWeek.Wednesday ||
                entry.TrainingDay.TrainingDate.DayOfWeek == DayOfWeek.Friday);

                Assert.AreEqual(2, entry.Items.Count);
                var item = entry.Items.Where(x => x.Time.TimeType == Model.TimeType.OnEmptyStomach).Single();
                Assert.AreEqual(5, item.Dosage);
                Assert.AreEqual(Model.DosageType.Grams, item.DosageType);
                Assert.AreEqual(supplement.GlobalId, item.Suplement.GlobalId);

                item = entry.Items.Where(x => x.Time.TimeType == Model.TimeType.BeforeSleep).Single();
                Assert.AreEqual(5, item.Dosage);
                Assert.AreEqual(Model.DosageType.Grams, item.DosageType);
                Assert.AreEqual(supplement.GlobalId, item.Suplement.GlobalId);
            }
        }
    }
}
