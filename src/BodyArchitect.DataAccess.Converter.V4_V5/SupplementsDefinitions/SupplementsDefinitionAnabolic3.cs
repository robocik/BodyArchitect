using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using NHibernate;

namespace BodyArchitect.DataAccess.Converter.V4_V5.SupplementsDefinitions
{
    class SupplementsDefinitionAnabolic3
    {
        private const string Metenolon = "Metenolonu Enantat";

        public SupplementCycleDefinition Create(IStatelessSession newSession, Profile profile)
        {
            var steryd = newSession.Get<Suplement>(new Guid("F099FF98-BB78-4E99-AFA3-EE0355974CD9"));

            var cycleDefinition = new SupplementCycleDefinition();
            cycleDefinition.Profile = profile;
            cycleDefinition.Name = string.Format("Pojedyńcza substancja ({0})", Metenolon);
            cycleDefinition.Language = "pl";
            cycleDefinition.Profile = profile;
            cycleDefinition.Difficult = TrainingPlanDifficult.Professional;
            cycleDefinition.CanBeIllegal = true;
            cycleDefinition.Status = PublishStatus.Published;
            cycleDefinition.PublishDate = DateTime.UtcNow;
            cycleDefinition.Purpose = WorkoutPlanPurpose.Mass;
            cycleDefinition.CreationDate = DateTime.UtcNow;
            cycleDefinition.Comment = "Ponieważ metenolon enantat jest anabolikiem o bardzo łagodnym działaniu można zażywać go spokojnie przez 16 tygodni. W tym okresie nastąpi powolny, równomierny przyrost mięśni w dobrej jakości, bez znacznego gromadzenia się wody. Przyrost ten pozostaje z reguły po odstawieniu. Substancja ta posiada wprawdzie tylko niewielkie działania uboczne wynikające z androgenności, jednak z czasem stają się one odczuwalne. Są to tłusta cera, trądzik i włosy na ciele. Typowym głównym skutkiem ubocznym jest zwiększone wypadanie włosów na głowie. Nie trzeba stosować dodatkowo substancji blokujących receptory estrogenowe lub aromatazę. Ponieważ metenolon enantat wpływa na własną produkcję hormonów tylko w zwiększonych dawkach w terapii po cyklu stosowania wystarcza zupełnie stosowanie przez cztery tygodnie Clomifenu lub Tamoxifenu Citrat. Stosowanie należy rozpocząć po około 5 dniach po ostatniej iniekcji według znanego schematu. W celu utrzymania siły można zrezygnować z Efedryny lub Clenbuterolu. Zaleca się jedynie stosowanie witaminy C i kwasu acetylosalicylowego, aby stłumić wartości kortyzolu. W tej fazie można stosować również Kreatynę, ale przede wszystkim ze względu na efekt powiększający komórki. ";
            cycleDefinition.Url = "http://nabierz-masy.com/przyklady-zastosowania?start=8";
            newSession.Insert(cycleDefinition);

            SupplementCycleWeek week = new SupplementCycleWeek();
            week.CycleWeekStart = 1;
            week.CycleWeekEnd = 1;
            cycleDefinition.Weeks.Add(week);
            week.Definition = cycleDefinition;
            newSession.Insert(week);

            SupplementCycleDosage dosage = new SupplementCycleDosage();
            dosage.Dosage = 600;
            dosage.DosageType = DosageType.MiliGrams;
            dosage.Repetitions = SupplementCycleDayRepetitions.OnceAWeek;
            dosage.Name = Metenolon;
            dosage.Supplement = steryd;
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
            dosage.Dosage = 300;
            dosage.DosageType = DosageType.MiliGrams;
            dosage.Repetitions = SupplementCycleDayRepetitions.OnceAWeek;
            dosage.Name = Metenolon;
            dosage.Supplement = steryd;
            dosage.TimeType = TimeType.NotSet;
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);

            return cycleDefinition;
        }
    }
}
