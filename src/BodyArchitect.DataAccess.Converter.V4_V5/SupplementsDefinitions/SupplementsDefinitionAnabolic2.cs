using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using NHibernate;

namespace BodyArchitect.DataAccess.Converter.V4_V5.SupplementsDefinitions
{
    class SupplementsDefinitionAnabolic2
    {
        private const string Trenbolon = "Trenbolon Acetat";
        private const string Testosteron = "Testosteron Propionat";
        private const string Stanozolol = "Stanozolol";

        public SupplementCycleDefinition Create(IStatelessSession newSession, Profile profile)
        {
            var steryd = newSession.Get<Suplement>(new Guid("F099FF98-BB78-4E99-AFA3-EE0355974CD9"));

            var cycleDefinition = new SupplementCycleDefinition();
            cycleDefinition.Profile = profile;
            cycleDefinition.Name = string.Format("Kuracja wspierająca dietę ({0},{1},{2})", Trenbolon, Testosteron, Stanozolol);
            cycleDefinition.Language = "pl";
            cycleDefinition.Profile = profile;
            cycleDefinition.Difficult = TrainingPlanDifficult.Professional;
            cycleDefinition.CanBeIllegal = true;
            cycleDefinition.Status = PublishStatus.Published;
            cycleDefinition.PublishDate = DateTime.UtcNow;
            cycleDefinition.Purpose = WorkoutPlanPurpose.Mass;
            cycleDefinition.CreationDate = DateTime.UtcNow;
            cycleDefinition.Comment = "Za podstawę służy tutaj Testosteron Propionat, dodatkowo stosuje się dziennie 50 mg Stanozololu i 75 mg Trenbolonu Acetat. Zarówno Stanozolol jak i Trenbolon pod względem zachowania mięśni są bardziej efektywne niż sam testosteron. Trenbolon jest nie tylko wysoce anaboliczny, lecz równocześnie także ekstremalnie androgenny, co ma dobry wpływ na twardość mięśni. Ani Stanozolol ani Trenbolon nie podwyższają poziomu estrogenu, ponieważ obie substancje nie konwertują w estrogen. Jedynie Testosteron Propionat mógłby ewentualnie aromatyzować, co jednak można opanować stosując codzienną dawkę cynku w ilości około 75-150 mg . Kto zainteresowany jest jak najmniejszą ilością tłuszczu, ten łączy tą kurację z hormonem gruczołu tarczycy Lionthyrin T3 lub Metaboliten Tiratricol T3. W razie potrzeby można dodać także 2-6 jednostek hormonu wzrostu dziennie, jeśli np. sportowiec przygotowuje się do zawodów. Ponieważ Stanozolol ma silny negatywny wpływ na wartości krwi, zażywanie jego należy ograniczyć do 8 tygodni, to znaczy w pierwszych czterech tygodniach kuracji stanozololu nie stosuje się. Wszystkie substancje można łączyć w zależności od własnych celów i życzeń, np. Stanozolol z Trenbolonem Acetat lub Trenbolon Acetat z Testosteronem Propionat. ";
            cycleDefinition.Url = "http://nabierz-masy.com/przyklady-zastosowania?start=8";
            newSession.Insert(cycleDefinition);

            SupplementCycleWeek week = new SupplementCycleWeek();
            week.CycleWeekStart = 1;
            week.CycleWeekEnd = 4;
            cycleDefinition.Weeks.Add(week);
            week.Definition = cycleDefinition;
            newSession.Insert(week);

            SupplementCycleDosage dosage = new SupplementCycleDosage();
            dosage.Dosage = 100;
            dosage.DosageType = DosageType.MiliGrams;
            dosage.Repetitions = SupplementCycleDayRepetitions.EveryDay;
            dosage.Supplement = steryd;
            dosage.Name = Stanozolol;
            dosage.TimeType = TimeType.NotSet;
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);

            dosage = new SupplementCycleDosage();
            dosage.Dosage = 75;
            dosage.DosageType = DosageType.MiliGrams;
            dosage.Supplement = steryd;
            dosage.Name = Trenbolon;
            dosage.Repetitions = SupplementCycleDayRepetitions.EveryDay;
            dosage.TimeType = TimeType.NotSet;
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);

            week = new SupplementCycleWeek();
            week.CycleWeekStart = 5;
            week.CycleWeekEnd = 12;
            cycleDefinition.Weeks.Add(week);
            week.Definition = cycleDefinition;
            newSession.Insert(week);
            dosage = new SupplementCycleDosage();
            dosage.Dosage = 100;
            dosage.DosageType = DosageType.MiliGrams;
            dosage.Repetitions = SupplementCycleDayRepetitions.EveryDay;
            dosage.Supplement = steryd;
            dosage.Name = Testosteron;
            dosage.TimeType = TimeType.NotSet;
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);

            dosage = new SupplementCycleDosage();
            dosage.Dosage = 75;
            dosage.DosageType = DosageType.MiliGrams;
            dosage.Supplement = steryd;
            dosage.Name = Trenbolon;
            dosage.Repetitions = SupplementCycleDayRepetitions.EveryDay;
            dosage.TimeType = TimeType.NotSet;
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);

            dosage = new SupplementCycleDosage();
            dosage.Dosage = 50;
            dosage.DosageType = DosageType.MiliGrams;
            dosage.Repetitions = SupplementCycleDayRepetitions.EveryDay;
            dosage.Supplement = steryd;
            dosage.Name = Stanozolol;
            dosage.TimeType = TimeType.NotSet;
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);
            

            return cycleDefinition;

        }
    }
}
