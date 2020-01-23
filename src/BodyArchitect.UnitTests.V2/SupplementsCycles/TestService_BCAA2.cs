using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using NUnit.Framework;
using DosageType = BodyArchitect.Service.V2.Model.DosageType;
using DosageUnit = BodyArchitect.Service.V2.Model.DosageUnit;
using SupplementCycleDayRepetitions = BodyArchitect.Service.V2.Model.SupplementCycleDayRepetitions;
using TimeType = BodyArchitect.Service.V2.Model.TimeType;

namespace BodyArchitect.UnitTests.V2.SupplementsCycles
{
    /// <summary>
    /// 1g na 10kg masy ciała w porcji (mamy 3 porcje w dni treningowe i jedną porcje w DNT)
    /// </summary>
    public class TestService_BCAA2:TestServiceBase
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
            SupplementCycleDosageDTO dosageDto = CreateDosageDTO(1, bcaa, SupplementCycleDayRepetitions.CardioTrainingDay, DosageType.Grams, TimeType.OnEmptyStomach);
            dosageDto.DosageUnit = DosageUnit.ON10KgWight;
            week.Dosages.Add(dosageDto);
            dosageDto = CreateDosageDTO(1, bcaa, SupplementCycleDayRepetitions.CardioTrainingDay, DosageType.Grams, TimeType.BeforeWorkout);
            dosageDto.DosageUnit = DosageUnit.ON10KgWight;
            week.Dosages.Add(dosageDto);
            dosageDto = CreateDosageDTO(1, bcaa, SupplementCycleDayRepetitions.CardioTrainingDay, DosageType.Grams, TimeType.AfterWorkout);
            dosageDto.DosageUnit = DosageUnit.ON10KgWight;
            week.Dosages.Add(dosageDto);

            dosageDto = CreateDosageDTO(1, bcaa, SupplementCycleDayRepetitions.NonTrainingDay, DosageType.Grams, TimeType.OnEmptyStomach);
            dosageDto.DosageUnit = DosageUnit.ON10KgWight;
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
            cycle.Weight = 85;
            cycle.TrainingDays = string.Format("{0}C,{1}C,{2}C", (int)DayOfWeek.Monday, (int)DayOfWeek.Wednesday, (int)DayOfWeek.Friday);
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

                    Assert.AreEqual(8.5, entry.Items.ElementAt(0).Dosage);
                    Assert.AreEqual(Model.DosageType.Grams, entry.Items.ElementAt(0).DosageType);
                    Assert.AreEqual(bcaa.GlobalId, entry.Items.ElementAt(0).Suplement.GlobalId);
                    Assert.AreEqual(Model.TimeType.OnEmptyStomach, entry.Items.ElementAt(0).Time.TimeType);

                    Assert.AreEqual(8.5, entry.Items.ElementAt(1).Dosage);
                    Assert.AreEqual(Model.DosageType.Grams, entry.Items.ElementAt(1).DosageType);
                    Assert.AreEqual(bcaa.GlobalId, entry.Items.ElementAt(1).Suplement.GlobalId);
                    Assert.AreEqual(Model.TimeType.BeforeWorkout, entry.Items.ElementAt(1).Time.TimeType);

                    Assert.AreEqual(8.5, entry.Items.ElementAt(2).Dosage);
                    Assert.AreEqual(Model.DosageType.Grams, entry.Items.ElementAt(2).DosageType);
                    Assert.AreEqual(bcaa.GlobalId, entry.Items.ElementAt(2).Suplement.GlobalId);
                    Assert.AreEqual(Model.TimeType.AfterWorkout, entry.Items.ElementAt(2).Time.TimeType);
                }
                else
                {
                    Assert.AreEqual(8.5, entry.Items.ElementAt(0).Dosage);
                    Assert.AreEqual(Model.DosageType.Grams, entry.Items.ElementAt(0).DosageType);
                    Assert.AreEqual(bcaa.GlobalId, entry.Items.ElementAt(0).Suplement.GlobalId);
                    Assert.AreEqual(Model.TimeType.OnEmptyStomach, entry.Items.ElementAt(0).Time.TimeType);
                }
            }
        }
    }
}
