using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using NHibernate;

namespace BodyArchitect.DataAccess.Converter.V4_V5.SupplementsDefinitions
{
    class SupplementsDefinitionAnabolic6
    {
        private Suplement steryd;
        private const string Nandrolon = "Nandrolon Decanoat";
        private const string Testosteron = "Testosteron Enantat";
        private const string Metandienon = "Metandienon";
        private const string Hormon = "Hormon wzrostu";

        

        public SupplementCycleDefinition Create(IStatelessSession newSession, Profile profile)
        {
            steryd = newSession.Get<Suplement>(new Guid("F099FF98-BB78-4E99-AFA3-EE0355974CD9"));

            var cycleDefinition = new SupplementCycleDefinition();
            cycleDefinition.Profile = profile;
            cycleDefinition.Name = string.Format("Mix dla zaawansowanych ({0},{1},{2},{3})", Nandrolon, Testosteron, Metandienon, Hormon);
            cycleDefinition.Language = "pl";
            cycleDefinition.Profile = profile;
            cycleDefinition.Difficult = TrainingPlanDifficult.Professional;
            cycleDefinition.CanBeIllegal = true;
            cycleDefinition.Status = PublishStatus.Published;
            cycleDefinition.PublishDate = DateTime.UtcNow;
            cycleDefinition.Purpose = WorkoutPlanPurpose.Mass;
            cycleDefinition.CreationDate = DateTime.UtcNow;
            cycleDefinition.Comment = "Ten mix należy stosować przez 6 miesięcy. Dawkowanie 2 jednostek hormonu wzrostu znajduje się stosunkowo na niskim poziomie, jednak działa długo i równomiernie i tworzy przez 6 miesięcy dobrą solidną tkankę mięśniową. Nandrolon funkcjonuje jako dobrze tolerowane sterydy anaboliczne, podczas gdy testosteron, razem z hormonem wzrostu pobudza produkcję IFG-1 w komórkach wątroby i mięśni. Zarówno Nandrolon, Testosteron jak również hormon wzrostu nadają się ze względu na ich charakter do stosowania przez bardzo długi okres. Żaden z tych trzech hormonów nie posiada działania toksycznego. Ekstremalnie ważna dla osiągnięcia sukcesu jest wysoka dawka protein, których idealna dawka wynosi 4 g na kg masy ciała dziennie. Zadowalające rezultaty w bardzo solidnym przyroście masy osiągnie się przy dawkowaniu węglowodanów na umiarkowanym poziomie i codziennym przyjmowaniu tłuszczów na poziomie ok. 120 - 150 g. Jedynie w pierwszych 2 - 3 tygodniach stosowania dochodzi do znacznego gromadzenia się wody, które z czasem normalizuje się, pod warunkiem prawidłowego odżywiania. Dodawanie Metandienonu pozostaje w gestii stosującego. Użycie jego nie jest konieczne, ale możliwe. Metandienon współpracuje dobrze z testosteronem i hormonem wzrostu. Należy przestrzegać jednak, żeby Metandienon ze względu na jego potencjalne szkodliwe działanie na wątrobę stosować zawsze w takcie 4 tygodnie zażywania - 4 tygodnie przerwy";
            cycleDefinition.Url = "http://nabierz-masy.com/przyklady-zastosowania?start=4";
            newSession.Insert(cycleDefinition);

            SupplementCycleWeek week = new SupplementCycleWeek();
            week.CycleWeekStart = 1;
            week.CycleWeekEnd = 4;
            cycleDefinition.Weeks.Add(week);
            week.Definition = cycleDefinition;
            newSession.Insert(week);

            SupplementCycleDosage dosage = Helper.CreateDosage(400, Nandrolon,steryd);
            week.Dosages.Add(dosage);
            dosage = Helper.CreateDosage(750, Testosteron, steryd);
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);
            dosage = Helper.CreateDosage(30, Metandienon, steryd, SupplementCycleDayRepetitions.EveryDay);
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);
            dosage = Helper.CreateDosage(2, Hormon, steryd, SupplementCycleDayRepetitions.EveryDay, DosageType.Servings);
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);

            week = new SupplementCycleWeek();
            week.CycleWeekStart = 5;
            week.CycleWeekEnd = 8;
            cycleDefinition.Weeks.Add(week);
            week.Definition = cycleDefinition;
            newSession.Insert(week);
            dosage = Helper.CreateDosage(400, Nandrolon, steryd);
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);
            dosage = Helper.CreateDosage(750, Testosteron, steryd);
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);
            dosage = Helper.CreateDosage(2, Hormon, steryd, SupplementCycleDayRepetitions.EveryDay, DosageType.Servings);
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);

            week = new SupplementCycleWeek();
            week.CycleWeekStart = 9;
            week.CycleWeekEnd = 12;
            cycleDefinition.Weeks.Add(week);
            week.Definition = cycleDefinition;
            newSession.Insert(week);
            dosage = Helper.CreateDosage(400, Nandrolon, steryd);
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);
            dosage = Helper.CreateDosage(750, Testosteron, steryd);
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);
            dosage = Helper.CreateDosage(30, Metandienon, steryd, SupplementCycleDayRepetitions.EveryDay);
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);
            dosage = Helper.CreateDosage(2, Hormon, steryd, SupplementCycleDayRepetitions.EveryDay, DosageType.Servings);
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);


            week = new SupplementCycleWeek();
            week.CycleWeekStart = 13;
            week.CycleWeekEnd = 16;
            cycleDefinition.Weeks.Add(week);
            week.Definition = cycleDefinition;
            newSession.Insert(week);
            dosage = Helper.CreateDosage(400, Nandrolon, steryd);
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);
            dosage = Helper.CreateDosage(750, Testosteron, steryd);
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);
            dosage = Helper.CreateDosage(2, Hormon, steryd, SupplementCycleDayRepetitions.EveryDay, DosageType.Servings);
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);

            week = new SupplementCycleWeek();
            week.CycleWeekStart = 17;
            week.CycleWeekEnd = 20;
            cycleDefinition.Weeks.Add(week);
            week.Definition = cycleDefinition;
            newSession.Insert(week);
            dosage = Helper.CreateDosage(400, Nandrolon, steryd);
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);
            dosage = Helper.CreateDosage(750, Testosteron, steryd);
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);
            dosage = Helper.CreateDosage(30, Metandienon, steryd, SupplementCycleDayRepetitions.EveryDay);
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);
            dosage = Helper.CreateDosage(2, Hormon, steryd, SupplementCycleDayRepetitions.EveryDay, DosageType.Servings);
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);

            week = new SupplementCycleWeek();
            week.CycleWeekStart = 21;
            week.CycleWeekEnd = 24;
            cycleDefinition.Weeks.Add(week);
            week.Definition = cycleDefinition;
            newSession.Insert(week);
            dosage = Helper.CreateDosage(400, Nandrolon, steryd);
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);
            dosage = Helper.CreateDosage(750, Testosteron, steryd);
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);
            dosage = Helper.CreateDosage(2, Hormon, steryd, SupplementCycleDayRepetitions.EveryDay, DosageType.Servings);
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);

            return cycleDefinition;
        }
    }
}
