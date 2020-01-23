using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using NHibernate;

namespace BodyArchitect.DataAccess.Converter.V4_V5.SupplementsDefinitions
{
    class SupplementsDefinitionAnabolic8
    {
        private Suplement steryd;
        private Suplement kreatyna;
        private Suplement witaminy;
        private const string Efedryna = "Efedryna";
        private const string Clomifen = "Clomifen";
        private const string WitaminaC = "Witamina C";
        private const string HCG = "HCG";


        public SupplementCycleDefinition Create(IStatelessSession newSession, Profile profile)
        {
            steryd = newSession.Get<Suplement>(new Guid("F099FF98-BB78-4E99-AFA3-EE0355974CD9"));
            kreatyna = newSession.Get<Suplement>(new Guid("296004CF-4F0D-4FA9-8897-2349F3F411D3"));
            witaminy = newSession.Get<Suplement>(new Guid("9800EB48-DECF-4F1B-A9B2-0EC1BEAEF70C"));
            
            var cycleDefinition = new SupplementCycleDefinition();
            cycleDefinition.Profile = profile;
            cycleDefinition.Name = "Terapia po cyklu stosowania kuracji testosteronem enantat";
            cycleDefinition.Language = "pl";
            cycleDefinition.Profile = profile;
            cycleDefinition.Difficult = TrainingPlanDifficult.Professional;
            cycleDefinition.CanBeIllegal = true;
            cycleDefinition.Status = PublishStatus.Published;
            cycleDefinition.PublishDate = DateTime.UtcNow;
            cycleDefinition.Purpose = WorkoutPlanPurpose.Other;
            cycleDefinition.CreationDate = DateTime.UtcNow;
            cycleDefinition.Url = "http://nabierz-masy.com/przyklady-zastosowania?start=12";
            newSession.Insert(cycleDefinition);

            SupplementCycleWeek week = new SupplementCycleWeek();
            week.Name = "Week";
            week.CycleWeekStart = 1;
            week.CycleWeekEnd = 1;
            cycleDefinition.Weeks.Add(week);
            week.Definition = cycleDefinition;
            newSession.Insert(week);
            var dosageDto = Helper.CreateDosage(500, HCG,steryd, SupplementCycleDayRepetitions.EveryDay);
            week.Dosages.Add(dosageDto);

            week = new SupplementCycleWeek();
            week.Name = "Week";
            week.CycleWeekStart = 2;
            week.CycleWeekEnd = 2;
            cycleDefinition.Weeks.Add(week);
            week.Definition = cycleDefinition;
            newSession.Insert(week);
            dosageDto = Helper.CreateDosage(500, HCG,steryd, SupplementCycleDayRepetitions.EveryDay);
            week.Dosages.Add(dosageDto);
            dosageDto = Helper.CreateDosage(50, Clomifen,steryd, SupplementCycleDayRepetitions.EveryDay);
            dosageDto.Comment = "Około 6 dni po ostatniej iniekcji 300 mg Clomifenu, następnie 50 mg dziennie, po tym 40 mg dziennie";
            week.Dosages.Add(dosageDto);

            week = new SupplementCycleWeek();
            week.Name = "Week";
            week.CycleWeekStart = 3;
            week.CycleWeekEnd = 3;
            cycleDefinition.Weeks.Add(week);
            week.Definition = cycleDefinition;
            newSession.Insert(week);
            dosageDto = Helper.CreateDosage(50, Clomifen,steryd, SupplementCycleDayRepetitions.EveryDay);
            week.Dosages.Add(dosageDto);
            dosageDto.Week = week;
            newSession.Insert(dosageDto);
            dosageDto = Helper.CreateDosage(25,"", kreatyna, SupplementCycleDayRepetitions.EveryDay, DosageType.Grams);
            week.Dosages.Add(dosageDto);
            dosageDto.Week = week;
            newSession.Insert(dosageDto);
            dosageDto = Helper.CreateDosage(40, Efedryna,steryd, SupplementCycleDayRepetitions.StrengthTrainingDay, timeType: TimeType.BeforeWorkout);
            week.Dosages.Add(dosageDto);
            dosageDto.Week = week;
            newSession.Insert(dosageDto);
            dosageDto = Helper.CreateDosage(2, WitaminaC,witaminy, SupplementCycleDayRepetitions.StrengthTrainingDay, DosageType.Grams, TimeType.BeforeWorkout);
            week.Dosages.Add(dosageDto);
            dosageDto.Week = week;
            newSession.Insert(dosageDto);
            dosageDto = Helper.CreateDosage(2, WitaminaC,witaminy, SupplementCycleDayRepetitions.StrengthTrainingDay, DosageType.Grams, TimeType.AfterWorkout);
            week.Dosages.Add(dosageDto);
            dosageDto.Week = week;
            newSession.Insert(dosageDto);

            week = new SupplementCycleWeek();
            week.Name = "Week";
            week.CycleWeekStart = 4;
            week.CycleWeekEnd = 8;
            cycleDefinition.Weeks.Add(week);
            week.Definition = cycleDefinition;
            newSession.Insert(week);
            dosageDto = Helper.CreateDosage(50, Clomifen, steryd, SupplementCycleDayRepetitions.EveryDay);
            week.Dosages.Add(dosageDto);
            dosageDto.Week = week;
            newSession.Insert(dosageDto);
            dosageDto = Helper.CreateDosage(10,null, kreatyna, SupplementCycleDayRepetitions.EveryDay, DosageType.Grams);
            week.Dosages.Add(dosageDto);
            dosageDto.Week = week;
            newSession.Insert(dosageDto);
            dosageDto = Helper.CreateDosage(40, Efedryna,steryd, SupplementCycleDayRepetitions.StrengthTrainingDay, timeType: TimeType.BeforeWorkout);
            week.Dosages.Add(dosageDto);
            dosageDto.Week = week;
            newSession.Insert(dosageDto);
            dosageDto = Helper.CreateDosage(2, WitaminaC,witaminy, SupplementCycleDayRepetitions.StrengthTrainingDay, DosageType.Grams, TimeType.BeforeWorkout);
            week.Dosages.Add(dosageDto);
            dosageDto.Week = week;
            newSession.Insert(dosageDto);
            dosageDto = Helper.CreateDosage(2, WitaminaC,witaminy, SupplementCycleDayRepetitions.StrengthTrainingDay, DosageType.Grams, TimeType.AfterWorkout);
            week.Dosages.Add(dosageDto);
            dosageDto.Week = week;
            newSession.Insert(dosageDto);

            week = new SupplementCycleWeek();
            week.Name = "Week";
            week.CycleWeekStart = 9;
            week.CycleWeekEnd = 10;
            cycleDefinition.Weeks.Add(week);
            week.Definition = cycleDefinition;
            newSession.Insert(week);
            dosageDto = Helper.CreateDosage(10,null, kreatyna, SupplementCycleDayRepetitions.EveryDay, DosageType.Grams);
            week.Dosages.Add(dosageDto);
            dosageDto.Week = week;
            newSession.Insert(dosageDto);

            return cycleDefinition;
        }
    }
}
