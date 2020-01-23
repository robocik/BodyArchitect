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
using TimeType = BodyArchitect.Service.V2.Model.TimeType;

namespace BodyArchitect.UnitTests.V2.SupplementsCycles
{
    //http://nabierz-masy.com/przyklady-zastosowania?start=12
    public class TestService_Anabolic8 : TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        private SuplementDTO hcg;
        private SuplementDTO clomifen;
        private SuplementDTO kreatyna;
        private SuplementDTO efedryna;
        private SuplementDTO witaminaC;

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                profiles.Add(CreateProfile(Session, "test1"));

                hcg = CreateSupplement("HCG").Map<SuplementDTO>();
                clomifen = CreateSupplement("Clomifen").Map<SuplementDTO>();
                kreatyna = CreateSupplement("Kreatyna").Map<SuplementDTO>();
                efedryna = CreateSupplement("Efedryna").Map<SuplementDTO>();
                witaminaC = CreateSupplement("Witamina C").Map<SuplementDTO>();
                tx.Commit();
            }
        }

        private SupplementCycleDefinition createCycle()
        {
            var cycleDefinition = new SupplementCycleDefinitionDTO();
            cycleDefinition.Name = "Terapia po cyklu stosowania kuracji testosteronem enantat";
            cycleDefinition.Language = "pl";
            SupplementCycleWeekDTO week = new SupplementCycleWeekDTO();
            week.Name = "Week";
            week.CycleWeekStart = 1;
            week.CycleWeekEnd = 1;
            cycleDefinition.Weeks.Add(week);
            SupplementCycleDosageDTO dosageDto = CreateDosageDTO(500, hcg, SupplementCycleDayRepetitions.EveryDay);
            week.Dosages.Add(dosageDto);

            week = new SupplementCycleWeekDTO();
            week.Name = "Week";
            week.CycleWeekStart = 2;
            week.CycleWeekEnd = 2;
            cycleDefinition.Weeks.Add(week);
            dosageDto = CreateDosageDTO(500, hcg, SupplementCycleDayRepetitions.EveryDay);
            week.Dosages.Add(dosageDto);
            dosageDto = CreateDosageDTO(50, clomifen, SupplementCycleDayRepetitions.EveryDay);
            dosageDto.Comment = "Około 6 dni po ostatniej iniekcji 300 mg Clomifenu, następnie 50 mg dziennie, po tym 40 mg dziennie";
            week.Dosages.Add(dosageDto);

            week = new SupplementCycleWeekDTO();
            week.Name = "Week";
            week.CycleWeekStart = 3;
            week.CycleWeekEnd = 3;
            cycleDefinition.Weeks.Add(week);
            dosageDto = CreateDosageDTO(50, clomifen, SupplementCycleDayRepetitions.EveryDay);
            week.Dosages.Add(dosageDto);
            dosageDto = CreateDosageDTO(25, kreatyna, SupplementCycleDayRepetitions.EveryDay,DosageType.Grams);
            week.Dosages.Add(dosageDto);
            dosageDto = CreateDosageDTO(40, efedryna, SupplementCycleDayRepetitions.StrengthTrainingDay,timeType:TimeType.BeforeWorkout);
            week.Dosages.Add(dosageDto);
            dosageDto = CreateDosageDTO(2, witaminaC, SupplementCycleDayRepetitions.StrengthTrainingDay, DosageType.Grams, TimeType.BeforeWorkout);
            week.Dosages.Add(dosageDto);
            dosageDto = CreateDosageDTO(2, witaminaC, SupplementCycleDayRepetitions.StrengthTrainingDay, DosageType.Grams, TimeType.AfterWorkout);
            week.Dosages.Add(dosageDto);

            week = new SupplementCycleWeekDTO();
            week.Name = "Week";
            week.CycleWeekStart =4;
            week.CycleWeekEnd = 8;
            cycleDefinition.Weeks.Add(week);
            dosageDto = CreateDosageDTO(50, clomifen, SupplementCycleDayRepetitions.EveryDay);
            week.Dosages.Add(dosageDto);
            dosageDto = CreateDosageDTO(10, kreatyna, SupplementCycleDayRepetitions.EveryDay, DosageType.Grams);
            week.Dosages.Add(dosageDto);
            dosageDto = CreateDosageDTO(40, efedryna, SupplementCycleDayRepetitions.StrengthTrainingDay, timeType: TimeType.BeforeWorkout);
            week.Dosages.Add(dosageDto);
            dosageDto = CreateDosageDTO(2, witaminaC, SupplementCycleDayRepetitions.StrengthTrainingDay, DosageType.Grams, TimeType.BeforeWorkout);
            week.Dosages.Add(dosageDto);
            dosageDto = CreateDosageDTO(2, witaminaC, SupplementCycleDayRepetitions.StrengthTrainingDay, DosageType.Grams, TimeType.AfterWorkout);
            week.Dosages.Add(dosageDto);

            week = new SupplementCycleWeekDTO();
            week.Name = "Week";
            week.CycleWeekStart = 9;
            week.CycleWeekEnd = 10;
            cycleDefinition.Weeks.Add(week);
            dosageDto = CreateDosageDTO(10, kreatyna, SupplementCycleDayRepetitions.EveryDay, DosageType.Grams);
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
            cycle.TrainingDays = string.Format("{0}S,{1}S,{2}B", (int)DayOfWeek.Monday, (int)DayOfWeek.Wednesday, (int)DayOfWeek.Friday);
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
            Assert.AreEqual(70, dbCycle.EntryObjects.Count);
        }
    }
}
