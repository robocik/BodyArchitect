using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings
{
    public class PaymentBasketMapping: ClassMapping<PaymentBasket>
    {
        public PaymentBasketMapping()
        {
            Id(x => x.GlobalId, map =>
            {
                map.Generator(Generators.GuidComb);
            });
            Property(x => x.TotalPrice, map =>
                                      {
                                          map.NotNullable(true);
                                      });
            ManyToOne(x => x.Profile, map =>
            {
                map.NotNullable(true);
                map.Cascade(Cascade.None);
            });

            ManyToOne(x => x.Customer, map =>
            {
                map.NotNullable(false);
                map.Cascade(Cascade.None);
            });

            //ManyToOne(x => x.Faktura, map =>
            //{
            //    map.NotNullable(false);
            //    map.Cascade(Cascade.None);
            //});

            Set(x => x.Payments, v =>
            {
                v.Cascade(Cascade.All | Cascade.DeleteOrphans);
                v.Inverse(true);
                v.Fetch(CollectionFetchMode.Join);
                v.Lazy(CollectionLazy.NoLazy);
            }, h => h.OneToMany());

            //Property(x => x.PlatnoscType, map => map.NotNullable(true));
            Property(x => x.DateTime, map =>
                                          {
                                              map.NotNullable(true);
                                              
                                          });

            }
    }
}
