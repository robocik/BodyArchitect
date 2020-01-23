using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model
{
    public abstract class PaymentBase : FMGlobalObject
    { 
        public virtual Product Product { get; set; }

        //UTC
        public virtual DateTime DateTime { get; set; }
    }
}
