using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model
{
    [Serializable]
    public class Address : FMGlobalObject
    {
        public virtual string Country { get; set; }

        public virtual string PostalCode { get; set; }

        public virtual string City { get; set; }

        public virtual string Address1 { get; set; }

        public virtual string Address2 { get; set; }

        public virtual bool IsEmpty
        {
            get { return string.IsNullOrEmpty(Country) &&
                string.IsNullOrEmpty(PostalCode) &&
                string.IsNullOrEmpty(City) &&
                string.IsNullOrEmpty(Address1) &&
                string.IsNullOrEmpty(Address2);
            }
        }
    }
}
