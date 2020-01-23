using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor.ViewModel
{
    public class ChampionshipViewModel:ViewModelBase
    {
        private ChampionshipDTO championship;

        public ChampionshipViewModel(ChampionshipDTO championship)
        {
            this.championship = championship;
        }

        public string Name
        {
            get { return Championship.Name; }
        }

        public DateTime StartTime
        {
            get { return Championship.StartTime; }
        }

        public string Type
        {
            get { return InstructorHelper.Translate(Championship.ChampionshipType); }
        }

        public bool IsDone
        {
            get { return Championship.State == ScheduleEntryState.Done; }
        }

        public bool IsCancelled
        {
            get { return Championship.State == ScheduleEntryState.Cancelled; }
        }

        public ChampionshipDTO Championship
        {
            get { return championship; }
        }
    }
}
