using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model
{
    /*
     nk - niesklasyfikowany (brak limitu III klasy)
     pk - poza konkursem
     */
    public enum ChampionshipCustomerType
    {
        Normal,
        OutsideCompetition,//pk
        NotClassified,//nk
        Disqualified
    }

    [DebuggerDisplay("{Customer.LastName}:{TotalWilks}")]
    public class ChampionshipCustomer : FMGlobalObject
    {
        public virtual ChampionshipCustomerType Type { get; set; }

        public virtual decimal Total { get; set; }

        public virtual decimal TotalWilks { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual decimal Weight { get; set; }

        public virtual DateTime WeightDateTime { get; set; }

        public virtual ChampionshipGroup Group { get; set; }

        public virtual string Comment { get; set; }

        public virtual StrengthTrainingEntry StrengthTraining { get; set; }
    }
}
