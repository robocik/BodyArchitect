using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using NHibernate;

namespace BodyArchitect.DataAccess.Converter.V4_V5.SupplementsDefinitions
{
    class SupplementsDefinitionCreatineCycle_2DT
    {
        public SupplementCycleDefinition Create(IStatelessSession newSession, Profile profile)
        {
            var kreatyna = newSession.Get<Suplement>(new Guid("296004cf-4f0d-4fa9-8897-2349f3f411d3"));

            var cycleDefinition = new SupplementCycleDefinition();
            cycleDefinition.Profile = profile;
            cycleDefinition.Name = string.Format("Kreatyna mono - 2 porcje w dni treningowe");
            cycleDefinition.Language = "pl";
            cycleDefinition.Profile = profile;
            cycleDefinition.Difficult = TrainingPlanDifficult.Advanced;
            cycleDefinition.CanBeIllegal = false;
            cycleDefinition.Status = PublishStatus.Published;
            cycleDefinition.PublishDate = DateTime.UtcNow;
            cycleDefinition.Purpose = WorkoutPlanPurpose.Mass;
            cycleDefinition.CreationDate = DateTime.UtcNow;
            cycleDefinition.Comment = "Kreatyna mono - 2 porcje w dni treningowe, 1 porcja w dni nietreningowe";
            newSession.Insert(cycleDefinition);

            SupplementCycleWeek trainingWeek = new SupplementCycleWeek();
            trainingWeek.CycleWeekStart = 1;
            trainingWeek.CycleWeekEnd = 6;
            cycleDefinition.Weeks.Add(trainingWeek);
            trainingWeek.Definition = cycleDefinition;
            newSession.Insert(trainingWeek);

            SupplementCycleDosage dosage = new SupplementCycleDosage();
            dosage.Dosage = 5;
            dosage.DosageType = DosageType.Grams;
            dosage.Supplement = kreatyna;
            dosage.Repetitions = SupplementCycleDayRepetitions.StrengthTrainingDay;
            dosage.TimeType = TimeType.OnEmptyStomach;
            trainingWeek.Dosages.Add(dosage);
            dosage.Week = trainingWeek;
            newSession.Insert(dosage);

            dosage = new SupplementCycleDosage();
            dosage.Dosage = 5;
            dosage.DosageType = DosageType.Grams;
            dosage.Repetitions = SupplementCycleDayRepetitions.StrengthTrainingDay;
            dosage.Supplement = kreatyna;
            dosage.TimeType = TimeType.BeforeSleep;
            trainingWeek.Dosages.Add(dosage);
            dosage.Week = trainingWeek;
            newSession.Insert(dosage);

            dosage = new SupplementCycleDosage();
            dosage.Dosage = 5;
            dosage.DosageType = DosageType.Grams;
            dosage.Repetitions = SupplementCycleDayRepetitions.NonTrainingDay;
            dosage.Supplement = kreatyna;
            dosage.TimeType = TimeType.OnEmptyStomach;
            trainingWeek.Dosages.Add(dosage);
            dosage.Week = trainingWeek;
            newSession.Insert(dosage);

            return cycleDefinition;
        }
    }
}
