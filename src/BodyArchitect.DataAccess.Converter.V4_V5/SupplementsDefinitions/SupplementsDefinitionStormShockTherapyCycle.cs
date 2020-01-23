using System;
using BodyArchitect.Model;
using NHibernate;

namespace BodyArchitect.DataAccess.Converter.V4_V5.SupplementsDefinitions
{
    class SupplementsDefinitionStormShockTherapyCycle
    {
        private const string Storm = "UNIVERSAL Storm";
        private const string Shock = "UNIVERSAL Shock Therapy";

        public SupplementCycleDefinition Create(IStatelessSession newSession, Profile profile)
        {
            var kreatynaStack = newSession.Get<Suplement>(new Guid("598a10da-ee36-4706-8abc-aa218b0ac142"));
            var przedtreningowka = newSession.Get<Suplement>(new Guid("4e9df6c8-9652-4c15-9cc5-a09a33f19c97"));

            var cycleDefinition = new SupplementCycleDefinition();
            cycleDefinition.Profile = profile;
            cycleDefinition.Name = string.Format("Storm z Shock Therapy");
            cycleDefinition.Language = "pl";
            cycleDefinition.Profile = profile;
            cycleDefinition.Difficult = TrainingPlanDifficult.Advanced;
            cycleDefinition.CanBeIllegal = false;
            cycleDefinition.Status = PublishStatus.Published;
            cycleDefinition.Comment = "Standardowy cykl kreatynowy. Bierzemy dwie porcje kreatyny UNIVERSAL Storm w dni treningowe (najlepiej rano i po treningu - odstęp min 8 godź). A w dni nietreningowe jedna porcja." +
                                      "\r\nShock Therapy tylko przed treningiem." +
                                      "\r\nDługość cyklu 6-8 tyg.";
            cycleDefinition.PublishDate = DateTime.UtcNow;
            cycleDefinition.Purpose = WorkoutPlanPurpose.Mass;
            cycleDefinition.CreationDate = DateTime.UtcNow;
            newSession.Insert(cycleDefinition);

            SupplementCycleWeek trainingWeek = new SupplementCycleWeek();
            trainingWeek.CycleWeekStart = 1;
            trainingWeek.CycleWeekEnd = 8;
            cycleDefinition.Weeks.Add(trainingWeek);
            trainingWeek.Definition = cycleDefinition;
            newSession.Insert(trainingWeek);

            SupplementCycleDosage dosage = new SupplementCycleDosage();
            dosage.Dosage = 1;
            dosage.DosageType = DosageType.Servings;
            dosage.Supplement = kreatynaStack;
            dosage.Name = Storm;
            dosage.Repetitions = SupplementCycleDayRepetitions.StrengthTrainingDay;
            dosage.TimeType = TimeType.OnEmptyStomach;
            trainingWeek.Dosages.Add(dosage);
            dosage.Week = trainingWeek;
            newSession.Insert(dosage);
            dosage = new SupplementCycleDosage();
            dosage.Dosage = 1;
            dosage.DosageType = DosageType.Servings;
            dosage.Supplement = przedtreningowka;
            dosage.Name = Shock;
            dosage.Repetitions = SupplementCycleDayRepetitions.StrengthTrainingDay;
            dosage.TimeType = TimeType.BeforeWorkout;
            trainingWeek.Dosages.Add(dosage);
            dosage.Week = trainingWeek;
            newSession.Insert(dosage);
            dosage = new SupplementCycleDosage();
            dosage.Dosage = 1;
            dosage.DosageType = DosageType.Servings;
            dosage.Supplement = kreatynaStack;
            dosage.Name = Storm;
            dosage.Repetitions = SupplementCycleDayRepetitions.StrengthTrainingDay;
            dosage.TimeType = TimeType.AfterWorkout;
            trainingWeek.Dosages.Add(dosage);
            dosage.Week = trainingWeek;
            newSession.Insert(dosage);

            dosage = new SupplementCycleDosage();
            dosage.Dosage = 1;
            dosage.DosageType = DosageType.Servings;
            dosage.Supplement = kreatynaStack;
            dosage.Name = Storm;
            dosage.Repetitions = SupplementCycleDayRepetitions.NonTrainingDay;
            dosage.TimeType = TimeType.OnEmptyStomach;
            trainingWeek.Dosages.Add(dosage);
            dosage.Week = trainingWeek;
            newSession.Insert(dosage);
            dosage = new SupplementCycleDosage();
            dosage.Dosage = 1;
            dosage.DosageType = DosageType.Servings;
            dosage.Repetitions = SupplementCycleDayRepetitions.NonTrainingDay;
            dosage.Supplement = kreatynaStack;
            dosage.Name = Shock;
            dosage.TimeType = TimeType.NotSet;
            trainingWeek.Dosages.Add(dosage);
            dosage.Week = trainingWeek;
            newSession.Insert(dosage);

            return cycleDefinition;
        }
    }
}
