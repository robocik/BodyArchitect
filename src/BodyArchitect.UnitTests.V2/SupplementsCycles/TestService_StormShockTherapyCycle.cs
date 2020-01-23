using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using NUnit.Framework;
using DosageType = BodyArchitect.Service.V2.Model.DosageType;
using SupplementCycleDayRepetitions = BodyArchitect.Service.V2.Model.SupplementCycleDayRepetitions;

namespace BodyArchitect.UnitTests.V2.SupplementsCycles
{
    [TestFixture]
    public class TestService_StormShockTherapyCycle:TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        private SuplementDTO storm;
        private SuplementDTO shock;
        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                profiles.Add(CreateProfile(Session, "test1"));

                storm = CreateSupplement("Storm").Map<SuplementDTO>();
                shock = CreateSupplement("ShockTherapy").Map<SuplementDTO>();
                tx.Commit();
            }


        }

        private SupplementCycleDefinition createCreatingStormAndShockTherapyDefinition()
        {
            var cycleDefinition = new SupplementCycleDefinitionDTO();
            cycleDefinition.Name = "storm and shock therapy";
            cycleDefinition.Language = "en";
            SupplementCycleWeekDTO trainingWeek = new SupplementCycleWeekDTO();
            trainingWeek.Name = "Training week";
            trainingWeek.CycleWeekStart = 1;
            trainingWeek.CycleWeekEnd = 6;
            cycleDefinition.Weeks.Add(trainingWeek);
            SupplementCycleDosageDTO dosageDto = new SupplementCycleDosageDTO();
            dosageDto.Dosage = 1;
            dosageDto.DosageType = DosageType.Servings;
            dosageDto.Supplement = storm;
            dosageDto.Repetitions = SupplementCycleDayRepetitions.StrengthTrainingDay;
            dosageDto.TimeType = Service.V2.Model.TimeType.OnEmptyStomach;
            trainingWeek.Dosages.Add(dosageDto);
            dosageDto = new SupplementCycleDosageDTO();
            dosageDto.Dosage = 1;
            dosageDto.DosageType = DosageType.Servings;
            dosageDto.Supplement = shock;
            dosageDto.Repetitions = SupplementCycleDayRepetitions.StrengthTrainingDay;
            dosageDto.TimeType = Service.V2.Model.TimeType.BeforeWorkout;
            trainingWeek.Dosages.Add(dosageDto);
            dosageDto = new SupplementCycleDosageDTO();
            dosageDto.Dosage = 1;
            dosageDto.DosageType = DosageType.Servings;
            dosageDto.Supplement = storm;
            dosageDto.Repetitions = SupplementCycleDayRepetitions.StrengthTrainingDay;
            dosageDto.TimeType = Service.V2.Model.TimeType.AfterWorkout;
            trainingWeek.Dosages.Add(dosageDto);

            dosageDto = new SupplementCycleDosageDTO();
            dosageDto.Dosage = 1;
            dosageDto.DosageType = DosageType.Servings;
            dosageDto.Supplement = storm;
            dosageDto.Repetitions = SupplementCycleDayRepetitions.NonTrainingDay;
            dosageDto.TimeType = Service.V2.Model.TimeType.OnEmptyStomach;
            trainingWeek.Dosages.Add(dosageDto);
            dosageDto = new SupplementCycleDosageDTO();
            dosageDto.Dosage = 1;
            dosageDto.DosageType = DosageType.Servings;
            dosageDto.Repetitions = SupplementCycleDayRepetitions.NonTrainingDay;
            dosageDto.Supplement = storm;
            dosageDto.TimeType = Service.V2.Model.TimeType.NotSet;
            trainingWeek.Dosages.Add(dosageDto);

            var definition = cycleDefinition.Map<SupplementCycleDefinition>();
            definition.Profile = profiles[0];
            Session.Save(definition);
            return definition;
        }

        [Test]
        public void StormShockTherapyCycle()
        {
            var cycleDef = createCreatingStormAndShockTherapyDefinition();
            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new SupplementsCycleDTO();
            cycle.TrainingDays = string.Format("{0}S,{1}S,{2}S", (int)DayOfWeek.Monday, (int)DayOfWeek.Wednesday, (int)DayOfWeek.Friday);
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
            Assert.AreEqual(42, dbCycle.EntryObjects.Count);
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
                    Assert.AreEqual(3, entry.Items.Count);
                    var item = entry.Items.Where(x => x.Time.TimeType == Model.TimeType.OnEmptyStomach).Single();
                    Assert.AreEqual(1, item.Dosage);
                    Assert.AreEqual(Model.DosageType.Servings, item.DosageType);
                    Assert.AreEqual(storm.GlobalId, item.Suplement.GlobalId);

                    item = entry.Items.Where(x => x.Time.TimeType == Model.TimeType.BeforeWorkout).Single();
                    Assert.AreEqual(1, item.Dosage);
                    Assert.AreEqual(Model.DosageType.Servings, item.DosageType);
                    Assert.AreEqual(shock.GlobalId, item.Suplement.GlobalId);

                    item = entry.Items.Where(x => x.Time.TimeType == Model.TimeType.AfterWorkout).Single();
                    Assert.AreEqual(1, item.Dosage);
                    Assert.AreEqual(Model.DosageType.Servings, item.DosageType);
                    Assert.AreEqual(storm.GlobalId, item.Suplement.GlobalId);
                }
                else
                {
                    Assert.AreEqual(2, entry.Items.Count);
                    var item = entry.Items.Where(x => x.Time.TimeType == Model.TimeType.OnEmptyStomach).Single();
                    Assert.AreEqual(1, item.Dosage);
                    Assert.AreEqual(Model.DosageType.Servings, item.DosageType);
                    Assert.AreEqual(storm.GlobalId, item.Suplement.GlobalId);

                    item = entry.Items.Where(x => x.Time.TimeType == Model.TimeType.NotSet).Single();
                    Assert.AreEqual(1, item.Dosage);
                    Assert.AreEqual(Model.DosageType.Servings, item.DosageType);
                    Assert.AreEqual(storm.GlobalId, item.Suplement.GlobalId);
                }
            }

        }
    }
}
