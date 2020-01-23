using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using NHibernate;

namespace BodyArchitect.DataAccess.Converter.V4_V5.SupplementsDefinitions
{
    class SupplementsDefinitionBCAA1
    {
        private const string BCAA = "BCAA";

        public SupplementCycleDefinition Create(IStatelessSession newSession, Profile profile)
        {
            var bcaa = newSession.Get<Suplement>(new Guid("34a141f4-3448-4552-9ab7-1b2537f7da1a"));

            var cycleDefinition = new SupplementCycleDefinition();
            cycleDefinition.Profile = profile;
            cycleDefinition.Name = string.Format("BCAA 3 razy dziennie");
            cycleDefinition.Language = "pl";
            cycleDefinition.Profile = profile;
            cycleDefinition.Difficult = TrainingPlanDifficult.Beginner;
            cycleDefinition.CanBeIllegal = false;
            cycleDefinition.Status = PublishStatus.Published;
            cycleDefinition.PublishDate = DateTime.UtcNow;
            cycleDefinition.Purpose = WorkoutPlanPurpose.Mass;
            cycleDefinition.CreationDate = DateTime.UtcNow;
            cycleDefinition.Comment = "3 razy dziennie po 5 kapsułek-przed śniadaniem, przed treningiem i po treningu. W dni nietreningowe bierzemy takie same dawki w porach, w których zwykle mamy trening.";
            newSession.Insert(cycleDefinition);

            SupplementCycleWeek week = new SupplementCycleWeek();
            week.CycleWeekStart = 1;
            week.CycleWeekEnd = 4;
            cycleDefinition.Weeks.Add(week);
            week.Definition = cycleDefinition;
            newSession.Insert(week);
            SupplementCycleDosage dosage = Helper.CreateDosage(5,BCAA, bcaa, SupplementCycleDayRepetitions.EveryDay, DosageType.Tablets, TimeType.OnEmptyStomach);
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);
            dosage = Helper.CreateDosage(5, BCAA, bcaa, SupplementCycleDayRepetitions.EveryDay, DosageType.Tablets, TimeType.BeforeWorkout);
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);
            dosage = Helper.CreateDosage(5, BCAA, bcaa, SupplementCycleDayRepetitions.EveryDay, DosageType.Tablets, TimeType.AfterWorkout);
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);

            return cycleDefinition;
        }
    }
}
