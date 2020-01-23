using System;
using BodyArchitect.Model;
using NHibernate;

namespace BodyArchitect.DataAccess.Converter.V4_V5.SupplementsDefinitions
{
    class SupplementsDefinitionBCAA2
    {
        private const string BCAA = "BCAA";

        public SupplementCycleDefinition Create(IStatelessSession newSession, Profile profile)
        {
            var bcaa = newSession.Get<Suplement>(new Guid("34a141f4-3448-4552-9ab7-1b2537f7da1a"));

            var cycleDefinition = new SupplementCycleDefinition();
            cycleDefinition.Profile = profile;
            cycleDefinition.Name = string.Format("BCAA - 3 porcje w dni treningowe kardio i jedną porcję w DNT");
            cycleDefinition.Language = "pl";
            cycleDefinition.Profile = profile;
            cycleDefinition.Difficult = TrainingPlanDifficult.Beginner;
            cycleDefinition.CanBeIllegal = false;
            cycleDefinition.Status = PublishStatus.Published;
            cycleDefinition.PublishDate = DateTime.UtcNow;
            cycleDefinition.Purpose = WorkoutPlanPurpose.Mass;
            cycleDefinition.CreationDate = DateTime.UtcNow;
            cycleDefinition.Comment = "1g na 10kg masy ciała w porcji (mamy 3 porcje w dni treningowe kardio i jedną porcje w DNT)";
            newSession.Insert(cycleDefinition);

            SupplementCycleWeek week = new SupplementCycleWeek();
            week.CycleWeekStart = 1;
            week.CycleWeekEnd = 4;
            cycleDefinition.Weeks.Add(week);
            week.Definition = cycleDefinition;
            newSession.Insert(week);

            SupplementCycleDosage dosage = Helper.CreateDosage(1,BCAA, bcaa, SupplementCycleDayRepetitions.CardioTrainingDay, DosageType.Grams, TimeType.OnEmptyStomach);
            dosage.DosageUnit = DosageUnit.ON10KgWight;
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);
            dosage = Helper.CreateDosage(1, BCAA, bcaa, SupplementCycleDayRepetitions.CardioTrainingDay, DosageType.Grams, TimeType.BeforeWorkout);
            dosage.DosageUnit = DosageUnit.ON10KgWight;
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);

            dosage = Helper.CreateDosage(1, BCAA, bcaa, SupplementCycleDayRepetitions.CardioTrainingDay, DosageType.Grams, TimeType.AfterWorkout);
            dosage.DosageUnit = DosageUnit.ON10KgWight;
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);

            dosage = Helper.CreateDosage(1, BCAA, bcaa, SupplementCycleDayRepetitions.NonTrainingDay, DosageType.Grams, TimeType.OnEmptyStomach);
            dosage.DosageUnit = DosageUnit.ON10KgWight;
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);

            return cycleDefinition;
        }
    }
}
