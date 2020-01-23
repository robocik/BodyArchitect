using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model
{
    public class ChampionshipTry
    {
        public virtual decimal Weight { get; set; }

        public virtual ChampionshipTryResult Result { get; set; }
    }

    public class ChampionshipEntry : FMGlobalObject
    {
        public ChampionshipEntry()
        {
            Try1 = new ChampionshipTry();
            Try2 = new ChampionshipTry();
            Try3 = new ChampionshipTry();
        }

        public virtual Exercise Exercise { get; set; }

        public virtual ChampionshipCustomer Customer { get; set; }

        public virtual ChampionshipTry Try1 { get; set; }

        public virtual ChampionshipTry Try2 { get; set; }

        public virtual ChampionshipTry Try3 { get; set; }
        //can be skipped because we can determine max from Try1..3
        public virtual decimal Max { get; set; }

        public virtual decimal Wilks { get; set; }
    }
}
