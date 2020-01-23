using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model
{
    public class CustomerSettings:FMGlobalObject
    {
        public virtual bool AutomaticUpdateMeasurements { get; set; }
    }
}
