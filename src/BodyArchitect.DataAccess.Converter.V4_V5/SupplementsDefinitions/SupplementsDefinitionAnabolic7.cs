using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using NHibernate;

namespace BodyArchitect.DataAccess.Converter.V4_V5.SupplementsDefinitions
{
    class SupplementsDefinitionAnabolic7
    {
        private Suplement steryd;
        private const string Trenbolon = "Trenbolon Acetat";

        public SupplementCycleDefinition Create(IStatelessSession newSession, Profile profile)
        {
            steryd = newSession.Get<Suplement>(new Guid("F099FF98-BB78-4E99-AFA3-EE0355974CD9"));

            var cycleDefinition = new SupplementCycleDefinition();
            cycleDefinition.Profile = profile;
            cycleDefinition.Name = "Pojedyńczy Trenbolon";
            cycleDefinition.Language = "pl";
            cycleDefinition.Profile = profile;
            cycleDefinition.Difficult = TrainingPlanDifficult.Professional;
            cycleDefinition.CanBeIllegal = true;
            cycleDefinition.Status = PublishStatus.Published;
            cycleDefinition.PublishDate = DateTime.UtcNow;
            cycleDefinition.Purpose = WorkoutPlanPurpose.Mass;
            cycleDefinition.CreationDate = DateTime.UtcNow;
            cycleDefinition.Comment = "W USA trenbolon należy od pewnego czasu do hitów, ponieważ jest on silnie anaboliczny i ekstremalnie androgenny. Buduje on szybko i skutecznie czystą, twardą i suchą pod względem jakościowym muskulaturę, jak żaden inny steryd. Osiągany przyrost siły jest ogromny i to pomimo brakującego gromadzenia się wody. Poza tym uzyskana muskulatura nie znika nagle po odstawieniu, jak to ma miejsce często w przypadku Oxymetholonu, Metandienonu i Testosteronu w wysokich dawkach. Trenbolon potęguje również działanie innych hormonów i nadaje się bardzo dobrze jako stymulant IGF-1. Trenbolon jest jedyną substancją (pomijając hormon wzrostu), który powoduje u stosujących zwiększenie wagi ciała przy równoczesnej utracie tłuszczu, pod warunkiem, że odżywianie odbywa się w rozsądnych granicach a dawka proteiny wynosi 3 g na kg ciała dziennie. \r\n\r\nTrenbolonu ze względu na jego możliwą toksyczność nie należy stosować dłużej niż 8-10 tygodni. W każdym razie zaleca się intensywną terapię po cyklu stosowania, ponieważ substancja ta silnie tłumi własną produkcję hormonalną. Dużym problemem przy stosowaniu Trenbolonu jako substancji działającej pojedynczo jest jego wpływ na libido oraz zdolność erekcyjną penisa. Stosujący nie będzie w stanie prowadzić normalne współżycie seksualne. ";
            cycleDefinition.Url = "http://nabierz-masy.com/przyklady-zastosowania?start=12";
            newSession.Insert(cycleDefinition);

            SupplementCycleWeek week = new SupplementCycleWeek();
            week.CycleWeekStart = 1;
            week.CycleWeekEnd = 10;
            cycleDefinition.Weeks.Add(week);
            week.Definition = cycleDefinition;
            newSession.Insert(week);

            SupplementCycleDosage dosage = Helper.CreateDosage(100, Trenbolon, steryd,SupplementCycleDayRepetitions.EveryDay);
            week.Dosages.Add(dosage);
            dosage.Week = week;
            newSession.Insert(dosage);
            
            return cycleDefinition;
        }
    }
}
