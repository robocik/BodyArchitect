using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using NHibernate;

namespace BodyArchitect.DataAccess.Converter.V4_V5.SupplementsDefinitions
{
    /// <summary>
    /// http://nabierz-masy.com/przyklady-zastosowania?start=8   (first)
    /// </summary>
    public class SupplementsDefinitionAnabolic1
    {
        private const string Metonolon = "Metenolon Enantat";
        private const string Testosteron = "Testosteron Propionat";

        public SupplementCycleDefinition Create(IStatelessSession newSession,Profile profile)
        {
            var steryd = newSession.Get<Suplement>(new Guid("F099FF98-BB78-4E99-AFA3-EE0355974CD9"));
            var cycleDefinition = new SupplementCycleDefinition();
            cycleDefinition.Name = string.Format("Łagodna kuracja ({0},{1})",Metonolon,Testosteron);
            cycleDefinition.Language = "pl";
            cycleDefinition.Profile = profile;
            cycleDefinition.Difficult=TrainingPlanDifficult.Professional;
            cycleDefinition.CanBeIllegal = true;
            cycleDefinition.Status = PublishStatus.Published;
            cycleDefinition.PublishDate = DateTime.UtcNow;
            cycleDefinition.Purpose = WorkoutPlanPurpose.Mass;
            cycleDefinition.CreationDate = DateTime.UtcNow;
            cycleDefinition.Comment ="Pomijając pierwsze 2 tygodnie stosowania to w kuracji tej gromadzenie się wody jest prawie wykluczone. Jedynie u osób bardzo wrażliwych stwierdza się takie działanie uboczne jak np. wypadanie włosów, trądzik lub tłusta cera. Ze względu na łagodność tej kuracji powinna ona trwać przynajmniej 16 tygodni a najlepiej 6 miesięcy. Codzienna dawka protein nie musi być zbyt wysoka, ale powinna kształtować się na poziomie przynajmniej 2 g na kg wagi ciała. Efektem będzie wolny ale stały przyrost bardzo solidnej tkanki mięśniowej wraz z umiarkowanym i równomiernym wzrostem siły. Kuracja ta jest interesująca również dla osób rozpoczynających stosowanie sterydów.";
            cycleDefinition.Url = "http://nabierz-masy.com/przyklady-zastosowania?start=8";
            newSession.Insert(cycleDefinition);

            SupplementCycleWeek week = new SupplementCycleWeek();
            //week.Name = "Training week";
            week.CycleWeekStart = 1;
            week.CycleWeekEnd = 1;
            cycleDefinition.Weeks.Add(week);
            week.Definition = cycleDefinition;
            newSession.Insert(week);

            SupplementCycleDosage dosage = new SupplementCycleDosage();
            dosage.Dosage = 600;
            dosage.DosageType = DosageType.MiliGrams;
            dosage.Supplement = steryd;
            dosage.Name = Metonolon;
            dosage.Repetitions = SupplementCycleDayRepetitions.OnceAWeek;
            dosage.TimeType = TimeType.NotSet;
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);

            dosage = new SupplementCycleDosage();
            dosage.Dosage = 50;
            dosage.DosageType = DosageType.MiliGrams;
            dosage.Repetitions = SupplementCycleDayRepetitions.EveryDay;
            dosage.Supplement = steryd;
            dosage.Name = Testosteron;
            dosage.TimeType = TimeType.NotSet;
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);

            week = new SupplementCycleWeek();
            //week.Name = "Training week";
            week.CycleWeekStart = 2;
            week.CycleWeekEnd = 16;
            cycleDefinition.Weeks.Add(week);
            week.Definition = cycleDefinition;
            newSession.Insert(week);
            dosage = new SupplementCycleDosage();
            dosage.Dosage = 300;
            dosage.DosageType = DosageType.MiliGrams;
            dosage.Repetitions = SupplementCycleDayRepetitions.OnceAWeek;
            dosage.Supplement = steryd;
            dosage.Name = Metonolon;
            dosage.TimeType = TimeType.NotSet;
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);
            dosage = new SupplementCycleDosage();
            dosage.Dosage = 50;
            dosage.DosageType = DosageType.MiliGrams;
            dosage.Repetitions = SupplementCycleDayRepetitions.EveryDay;
            dosage.Supplement = steryd;
            dosage.Name = Testosteron;
            dosage.TimeType = TimeType.NotSet;
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);
            
            return cycleDefinition;
        }
    }
}
