using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.NHibernate.Mappings
{
    public class PaymentBaseMapping: ClassMapping<PaymentBase>
    {
        public PaymentBaseMapping()
        {
            
            Id(x => x.GlobalId, map =>
            {
                map.Generator(Generators.GuidComb);
            });

            OneToOne(x => x.Product, map =>
            {
                map.Constrained(true);
                map.Cascade(Cascade.All);
                map.PropertyReference(typeof(Product).GetProperty("Payment"));
            });

            Property(x => x.DateTime, map =>
            {
                map.NotNullable(true);
                
            });
        }
    }
}
