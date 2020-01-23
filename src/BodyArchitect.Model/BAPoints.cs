using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model
{
    public enum BAPointsType
    {
        PayPal,
        Serialkey,
        Transferuj,
        Przelewy24
    }

    public class BAPoints:FMGlobalObject
    {
        public virtual int Points { get; set; }
        
        public virtual DateTime ImportedDate { get; set; }

        public virtual Profile Profile { get; set; }

        public virtual LoginData LoginData { get; set; }

        public virtual string Identifier { get; set; }

        public virtual BAPointsType Type { get; set; }
    }
}
