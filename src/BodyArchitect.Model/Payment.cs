using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model
{
    public class Payment:PaymentBase
    {
        public virtual int Count { get; set; }

        public virtual decimal Price { get; set; }

        public virtual PaymentBasket PaymentBasket { get; set; }
    }
}
