using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Shared;

namespace BodyArchitect.Model
{
    [Serializable]
    public abstract class Product:FMGlobalObject
    {
        public virtual Customer Customer { get; set; }
        
        public virtual string Name { get; set; }

        public virtual int Version { get; set; }

        public virtual Profile Profile { get; set; }

        public virtual decimal Price { get; set; }

        public virtual PaymentBase Payment { get; set; }

        //UTC
        public virtual DateTime DateTime { get; set; }
    }
}
