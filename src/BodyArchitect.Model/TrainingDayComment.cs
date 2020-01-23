using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model
{

    public class TrainingDayComment:FMGlobalObject
    {
        public virtual Profile Profile { get; set; }

        public virtual string Comment { get; set; }
        //UTC
        public virtual DateTime DateTime { get; set; }
        
        public virtual TrainingDay TrainingDay { get; set; }

        public virtual LoginData LoginData { get; set; }
    }
}
