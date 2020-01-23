using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using NHibernate;

namespace BodyArchitect.DataAccess.Converter.V4_V5.SupplementsDefinitions
{
    /// <summary>
    /// http://nabierz-masy.com/przyklady-zastosowania?start=2 (first)
    /// </summary>
    class SupplementsDefinitionAnabolic4
    {
        private const string Nandrolon = "Nandrolon Decanoat";
        private const string Testosteron = "Testosteron Enantat";
        

        public SupplementCycleDefinition Create(IStatelessSession newSession, Profile profile)
        {
            var steryd = newSession.Get<Suplement>(new Guid("F099FF98-BB78-4E99-AFA3-EE0355974CD9"));

            var cycleDefinition = new SupplementCycleDefinition();
            cycleDefinition.Profile = profile;
            cycleDefinition.Name = string.Format("{0} i {1}", Nandrolon, Testosteron);
            cycleDefinition.Language = "pl";
            cycleDefinition.Profile = profile;
            cycleDefinition.Difficult = TrainingPlanDifficult.Professional;
            cycleDefinition.CanBeIllegal = true;
            cycleDefinition.Status = PublishStatus.Published;
            cycleDefinition.PublishDate = DateTime.UtcNow;
            cycleDefinition.Purpose = WorkoutPlanPurpose.Mass;
            cycleDefinition.CreationDate = DateTime.UtcNow;
            cycleDefinition.Comment = "Stosując dawki do 400 mg nandrolonu tygodniowo stosujący doświadczy dobrego solidnego przyrostu mięśni bez szczególnych działań ubocznych. Jednak, jeżeli przekroczy się tę dawkę, to należy liczyć się ze zwiększonym gromadzeniem się wody, co powoduje nabrzmiały wygląd. Kombinacja z 250 mg Testosteronu Enantat na tydzień gwarantuje nie tylko zdrowe pożycie seksualne, lecz także wspiera dalej przyrost mięśni i siły. Efekt wizualny jest nadzwyczajny a masa ciała przy wystarczająco wysokiej dawce proteiny w ilości 3 g na kg masy ciała dziennie szybko wzrośnie. Długość kuracji powinna wynosić przynajmniej 16 tygodni. W tym okresie należy oczekiwać przyrostu od 12 do 15 kg, przy czym większość składa się z solidnej masy mięśniowej, szczególnie, jeżeli odżywianie pozostanie w rozsądnych ramach. Ponieważ Nandrolon ze względu na działanie progesteronu tłumi szybko własną produkcję testosteronu, szczególną uwagę należy przywiązać do prawidłowego postępowania podczas odstawiania.";
            cycleDefinition.Url = "http://nabierz-masy.com/przyklady-zastosowania?start=2";
            newSession.Insert(cycleDefinition);

            SupplementCycleWeek week = new SupplementCycleWeek();
            week.CycleWeekStart = 1;
            week.CycleWeekEnd = 1;
            cycleDefinition.Weeks.Add(week);
            week.Definition = cycleDefinition;
            newSession.Insert(week);

            SupplementCycleDosage dosage = new SupplementCycleDosage();
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
            week.CycleWeekStart = 2;
            week.CycleWeekEnd = 16;
            cycleDefinition.Weeks.Add(week);
            week.Definition = cycleDefinition;
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
            dosage.Dosage = 250;
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
