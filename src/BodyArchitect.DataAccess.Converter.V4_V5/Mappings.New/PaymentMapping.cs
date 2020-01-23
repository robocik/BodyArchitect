using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings
{
    public class PaymentMapping: UnionSubclassMapping<Payment>
    {
        public PaymentMapping()
        {
            ManyToOne(x => x.PaymentBasket, map =>
            {
                map.NotNullable(true);
                map.Lazy(LazyRelation.Proxy);
            });

            Property(x => x.Count, map => map.NotNullable(true));

            Property(x => x.Price, map => map.NotNullable(true));
        }
    }
}
