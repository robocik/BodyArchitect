using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using NHibernate;

namespace BodyArchitect.DataAccess.Converter.V4_V5.SupplementsDefinitions
{
    class SupplementsDefinitionAnabolic5
    {
        private const string Nandrolon = "Nandrolon Decanoat";
        private const string Testosteron = "Testosteron Enantat";
        private const string Metandienon = "Metandienon";

        public SupplementCycleDefinition Create(IStatelessSession newSession, Profile profile)
        {
            var steryd = newSession.Get<Suplement>(new Guid("F099FF98-BB78-4E99-AFA3-EE0355974CD9"));

            var cycleDefinition = new SupplementCycleDefinition();
            cycleDefinition.Profile = profile;
            cycleDefinition.Name = string.Format("Mix: {0},{1} i {2}", Nandrolon, Testosteron,Metandienon);
            cycleDefinition.Language = "pl";
            cycleDefinition.Profile = profile;
            cycleDefinition.Difficult = TrainingPlanDifficult.Professional;
            cycleDefinition.CanBeIllegal = true;
            cycleDefinition.Status = PublishStatus.Published;
            cycleDefinition.PublishDate = DateTime.UtcNow;
            cycleDefinition.Purpose = WorkoutPlanPurpose.Mass;
            cycleDefinition.CreationDate = DateTime.UtcNow;
            cycleDefinition.Comment = "W tym klasycznym mixie Nadrolon Decanoat jest anabolicznym sterydem bazowym, Testosteron Enantat sterydem masy o długim działaniu oraz Metandienon jako starter o szybkim działaniu. Przy dużym spożywaniu protein 3-4 g na kg masy ciała dziennie., przy umiarkowanym przyjmowaniu węglowodanów jak również dziennej dawce tłuszczu wynoszącej ok. 120 g dojdzie do nadzwyczaj szybkiego wzrostu masy mięśniowej, które przy tym sposobie odżywiania jest przeważnie solidnej natury. Na początku kuracji należy liczyć się ze względu na szybkie działanie Metandienonu z silnym gromadzeniem się wody, jednak spada ono przy dalszym zażywaniu, szczególnie wtedy, jeżeli dawka węglowodanów nie będzie zbyt wysoka, a większa będzie konsumpcja protein. Długość kuracji powinna trwać przynajmniej 16 tygodni, aby dać organizmowi czas na przyzwyczajenie się do nowej masy mięśniowej. Jednakże należy uważać na to, żeby nie stosować Metandienonu dłużej niżz 4 tygodnie w jednej kuracji.";
            cycleDefinition.Url = "http://nabierz-masy.com/przyklady-zastosowania?start=2";
            newSession.Insert(cycleDefinition);

            SupplementCycleWeek week = new SupplementCycleWeek();
            week.CycleWeekStart = 1;
            week.CycleWeekEnd = 6;
            week.Definition = cycleDefinition;
            cycleDefinition.Weeks.Add(week);
            newSession.Insert(week);

            SupplementCycleDosage dosage = new SupplementCycleDosage();
            dosage.Dosage = 400;
            dosage.DosageType = DosageType.MiliGrams;
            dosage.Repetitions = SupplementCycleDayRepetitions.OnceAWeek;
            dosage.Supplement = steryd;
            dosage.Name = Nandrolon;
            dosage.TimeType = TimeType.NotSet;
            week.Dosages.Add(dosage);
            dosage = new SupplementCycleDosage();
            dosage.Dosage = 500;
            dosage.DosageType = DosageType.MiliGrams;
            dosage.Repetitions = SupplementCycleDayRepetitions.OnceAWeek;
            dosage.Supplement = steryd;
            dosage.Name = Testosteron;
            dosage.TimeType = TimeType.NotSet;
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);
            dosage = new SupplementCycleDosage();
            dosage.Dosage = 30;
            dosage.DosageType = DosageType.MiliGrams;
            dosage.Repetitions = SupplementCycleDayRepetitions.EveryDay;
            dosage.Supplement = steryd;
            dosage.Name = Metandienon;
            dosage.TimeType = TimeType.NotSet;
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);

            week = new SupplementCycleWeek();
            week.CycleWeekStart = 7;
            week.CycleWeekEnd = 8;
            week.Definition = cycleDefinition;
            cycleDefinition.Weeks.Add(week);
            newSession.Insert(week);

            dosage = new SupplementCycleDosage();
            dosage.Dosage = 400;
            dosage.DosageType = DosageType.MiliGrams;
            dosage.Repetitions = SupplementCycleDayRepetitions.OnceAWeek;
            dosage.Supplement = steryd;
            dosage.Name = Nandrolon;
            dosage.TimeType = TimeType.NotSet;
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);
            dosage = new SupplementCycleDosage();
            dosage.Dosage = 500;
            dosage.DosageType = DosageType.MiliGrams;
            dosage.Repetitions = SupplementCycleDayRepetitions.OnceAWeek;
            dosage.Supplement = steryd;
            dosage.Name = Testosteron;
            dosage.TimeType = TimeType.NotSet;
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);

            week = new SupplementCycleWeek();
            week.CycleWeekStart = 9;
            week.CycleWeekEnd = 12;
            week.Definition = cycleDefinition;
            cycleDefinition.Weeks.Add(week);
            newSession.Insert(week);

            dosage = new SupplementCycleDosage();
            dosage.Dosage = 400;
            dosage.DosageType = DosageType.MiliGrams;
            dosage.Repetitions = SupplementCycleDayRepetitions.OnceAWeek;
            dosage.Supplement = steryd;
            dosage.Name = Nandrolon;
            dosage.TimeType = TimeType.NotSet;
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);
            dosage = new SupplementCycleDosage();
            dosage.Dosage = 500;
            dosage.DosageType = DosageType.MiliGrams;
            dosage.Repetitions = SupplementCycleDayRepetitions.OnceAWeek;
            dosage.Supplement = steryd;
            dosage.Name = Testosteron;
            dosage.TimeType = TimeType.NotSet;
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);
            dosage = new SupplementCycleDosage();
            dosage.Dosage = 30;
            dosage.DosageType = DosageType.MiliGrams;
            dosage.Repetitions = SupplementCycleDayRepetitions.EveryDay;
            dosage.Supplement = steryd;
            dosage.Name = Metandienon;
            dosage.TimeType = TimeType.NotSet;
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);

            week = new SupplementCycleWeek();
            week.CycleWeekStart = 13;
            week.CycleWeekEnd = 16;
            week.Definition = cycleDefinition;
            cycleDefinition.Weeks.Add(week);
            newSession.Insert(week);

            dosage = new SupplementCycleDosage();
            dosage.Dosage = 400;
            dosage.DosageType = DosageType.MiliGrams;
            dosage.Repetitions = SupplementCycleDayRepetitions.OnceAWeek;
            dosage.Supplement = steryd;
            dosage.Name = Nandrolon;
            dosage.TimeType = TimeType.NotSet;
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);
            dosage = new SupplementCycleDosage();
            dosage.Dosage = 500;
            dosage.DosageType = DosageType.MiliGrams;
            dosage.Repetitions = SupplementCycleDayRepetitions.OnceAWeek;
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
