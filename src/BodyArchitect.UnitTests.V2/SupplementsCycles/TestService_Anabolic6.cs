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
    /// <summary>
    /// http://nabierz-masy.com/przyklady-zastosowania?start=4 (second)
    /// </summary>
    [TestFixture]
    public class TestService_Anabolic6 : TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        private SuplementDTO nandrolon;
        private SuplementDTO testosteron;
        private SuplementDTO metandienon;
        private SuplementDTO hormon;

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                profiles.Add(CreateProfile(Session, "test1"));

                nandrolon = CreateSupplement("Nandrolon Decanoat").Map<SuplementDTO>();
                testosteron = CreateSupplement("Testosteron Enantat").Map<SuplementDTO>();
                metandienon = CreateSupplement("Metandienon").Map<SuplementDTO>();
                hormon = CreateSupplement("Hormon wzrostu").Map<SuplementDTO>();
                tx.Commit();
            }
        }

        

        private SupplementCycleDefinition createCycle()
        {
            var cycleDefinition = new SupplementCycleDefinitionDTO();
            cycleDefinition.Name = "sterydy";
            cycleDefinition.Language = "en";
            SupplementCycleWeekDTO week = new SupplementCycleWeekDTO();
            week.Name = "Training week";
            week.CycleWeekStart = 1;
            week.CycleWeekEnd = 4;
            cycleDefinition.Weeks.Add(week);
            SupplementCycleDosageDTO dosageDto = CreateDosageDTO(400,nandrolon);
            week.Dosages.Add(dosageDto);
            dosageDto = CreateDosageDTO(750, testosteron);
            week.Dosages.Add(dosageDto);
            dosageDto = CreateDosageDTO(30, metandienon, SupplementCycleDayRepetitions.EveryDay);
            week.Dosages.Add(dosageDto);
            dosageDto = CreateDosageDTO(2, hormon, SupplementCycleDayRepetitions.EveryDay, DosageType.Servings);
            week.Dosages.Add(dosageDto);

            week = new SupplementCycleWeekDTO();
            week.Name = "Training week";
            week.CycleWeekStart = 5;
            week.CycleWeekEnd = 8;
            cycleDefinition.Weeks.Add(week);
            dosageDto = CreateDosageDTO(400, nandrolon);
            week.Dosages.Add(dosageDto);
            dosageDto = CreateDosageDTO(750, testosteron);
            week.Dosages.Add(dosageDto);
            dosageDto = CreateDosageDTO(2, hormon, SupplementCycleDayRepetitions.EveryDay, DosageType.Servings);
            week.Dosages.Add(dosageDto);

            week = new SupplementCycleWeekDTO();
            week.Name = "Training week";
            week.CycleWeekStart = 9;
            week.CycleWeekEnd = 12;
            cycleDefinition.Weeks.Add(week);
            dosageDto = CreateDosageDTO(400, nandrolon);
            week.Dosages.Add(dosageDto);
            dosageDto = CreateDosageDTO(750, testosteron);
            week.Dosages.Add(dosageDto);
            dosageDto = CreateDosageDTO(30, metandienon, SupplementCycleDayRepetitions.EveryDay);
            week.Dosages.Add(dosageDto);
            dosageDto = CreateDosageDTO(2, hormon, SupplementCycleDayRepetitions.EveryDay, DosageType.Servings);
            week.Dosages.Add(dosageDto);

            week = new SupplementCycleWeekDTO();
            week.Name = "Training week";
            week.CycleWeekStart =13;
            week.CycleWeekEnd = 16;
            cycleDefinition.Weeks.Add(week);
            dosageDto = CreateDosageDTO(400, nandrolon);
            week.Dosages.Add(dosageDto);
            dosageDto = CreateDosageDTO(750, testosteron);
            week.Dosages.Add(dosageDto);
            dosageDto = CreateDosageDTO(2, hormon, SupplementCycleDayRepetitions.EveryDay, DosageType.Servings);
            week.Dosages.Add(dosageDto);

            week = new SupplementCycleWeekDTO();
            week.Name = "Training week";
            week.CycleWeekStart = 17;
            week.CycleWeekEnd = 20;
            cycleDefinition.Weeks.Add(week);
            dosageDto = CreateDosageDTO(400, nandrolon);
            week.Dosages.Add(dosageDto);
            dosageDto = CreateDosageDTO(750, testosteron);
            week.Dosages.Add(dosageDto);
            dosageDto = CreateDosageDTO(30, metandienon, SupplementCycleDayRepetitions.EveryDay);
            week.Dosages.Add(dosageDto);
            dosageDto = CreateDosageDTO(2, hormon, SupplementCycleDayRepetitions.EveryDay, DosageType.Servings);
            week.Dosages.Add(dosageDto);

            week = new SupplementCycleWeekDTO();
            week.Name = "Training week";
            week.CycleWeekStart = 21;
            week.CycleWeekEnd = 24;
            cycleDefinition.Weeks.Add(week);
            dosageDto = CreateDosageDTO(400, nandrolon);
            week.Dosages.Add(dosageDto);
            dosageDto = CreateDosageDTO(750, testosteron);
            week.Dosages.Add(dosageDto);
            dosageDto = CreateDosageDTO(2, hormon, SupplementCycleDayRepetitions.EveryDay, DosageType.Servings);
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
            Assert.AreEqual(168, dbCycle.EntryObjects.Count);
            //Assert.AreEqual(SuplementsEntryDTO.EntryTypeId, dbCycle.TypeId);
            var entries = dbCycle.EntryObjects.OrderBy(x => x.TrainingDay.TrainingDate).Cast<SuplementsEntry>().ToList();
            foreach (var entry in entries)
            {
                Assert.IsNotNull(entry.LoginData);
                Assert.IsNull(entry.Reminder);

                var week = (int)((entry.TrainingDay.TrainingDate - dbCycle.StartDate).TotalDays / 7) + 1;

                bool isMonday = entry.TrainingDay.TrainingDate.DayOfWeek == dbCycle.StartDate.DayOfWeek;
                bool isBigWeek = week >= 1 && week <= 4 || week >= 9 && week <= 12 || week >= 17 && week <= 20;

                if (isMonday)
                {
                    var item = entry.Items.Where(x => x.Suplement.GlobalId == nandrolon.GlobalId).Single();
                    Assert.AreEqual(400, item.Dosage);
                    Assert.AreEqual(Model.DosageType.MiliGrams, item.DosageType);
                    Assert.AreEqual(Model.TimeType.NotSet, item.Time.TimeType);

                    item = entry.Items.Where(x => x.Suplement.GlobalId == testosteron.GlobalId).Single();
                    Assert.AreEqual(750, item.Dosage);
                    Assert.AreEqual(Model.DosageType.MiliGrams, item.DosageType);
                    Assert.AreEqual(Model.TimeType.NotSet, item.Time.TimeType);
                    
                }

                if (isBigWeek)
                {

                    if (isMonday)
                    {
                        Assert.AreEqual(4, entry.Items.Count);
                        var item = entry.Items.Where(x => x.Suplement.GlobalId == metandienon.GlobalId).Single();
                        Assert.AreEqual(30, item.Dosage);
                        Assert.AreEqual(Model.DosageType.MiliGrams, item.DosageType);
                        Assert.AreEqual(Model.TimeType.NotSet, item.Time.TimeType);

                        item = entry.Items.Where(x => x.Suplement.GlobalId == hormon.GlobalId).Single();
                        Assert.AreEqual(2, item.Dosage);
                        Assert.AreEqual(Model.DosageType.Servings, item.DosageType);
                        Assert.AreEqual(Model.TimeType.NotSet, item.Time.TimeType);
                    }
                    else
                    {
                        Assert.AreEqual(2, entry.Items.Count);
                        var item = entry.Items.Where(x => x.Suplement.GlobalId == metandienon.GlobalId).Single();
                        Assert.AreEqual(30, item.Dosage);
                        Assert.AreEqual(Model.DosageType.MiliGrams, item.DosageType);
                        Assert.AreEqual(Model.TimeType.NotSet, item.Time.TimeType);

                        item = entry.Items.Where(x => x.Suplement.GlobalId == hormon.GlobalId).Single();
                        Assert.AreEqual(2, item.Dosage);
                        Assert.AreEqual(Model.DosageType.Servings, item.DosageType);
                        Assert.AreEqual(hormon.GlobalId, item.Suplement.GlobalId);
                        Assert.AreEqual(Model.TimeType.NotSet, item.Time.TimeType);
                    }

                }
                else
                {
                    if (isMonday)
                    {
                        Assert.AreEqual(3, entry.Items.Count);
                        var item = entry.Items.Where(x => x.Suplement.GlobalId == hormon.GlobalId).Single();
                        Assert.AreEqual(2, item.Dosage);
                        Assert.AreEqual(Model.DosageType.Servings, item.DosageType);
                        Assert.AreEqual(Model.TimeType.NotSet, item.Time.TimeType);
                    }
                    else
                    {
                        Assert.AreEqual(1, entry.Items.Count);
                        var item = entry.Items.Where(x => x.Suplement.GlobalId == hormon.GlobalId).Single();
                        Assert.AreEqual(2, item.Dosage);
                        Assert.AreEqual(Model.DosageType.Servings, item.DosageType);
                        Assert.AreEqual(Model.TimeType.NotSet, item.Time.TimeType);
                    }
                }
            }
        }
    }
}
