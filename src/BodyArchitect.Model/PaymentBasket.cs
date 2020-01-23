using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model
{
    public class PaymentBasket:FMGlobalObject
    {
        public PaymentBasket()
        {
            Payments = new HashSet<Payment>();
        }


        public virtual int Id { get; set; }

        //public virtual PlatnoscType PlatnoscType { get; set; }

        public virtual decimal TotalPrice { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual Profile Profile { get; set; }

        //UTC
        public virtual DateTime DateTime { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }
    }
}
