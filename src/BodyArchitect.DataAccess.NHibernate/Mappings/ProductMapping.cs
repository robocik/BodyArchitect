using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.NHibernate.Mappings
{
    public class ProductMapping: ClassMapping<Product>
    {
        public ProductMapping()
        {
            Id(x => x.GlobalId, map =>
            {
                map.Generator(Generators.GuidComb);
            });

            ManyToOne(x => x.Customer, map =>
            {
                map.NotNullable(false);
                map.Cascade(Cascade.None);
            });

            ManyToOne(x => x.Profile, map =>
            {
                map.NotNullable(true);
                map.Cascade(Cascade.None);
            });

            ManyToOne(x => x.Payment, map =>
            {
                map.NotNullable(false);
                map.Cascade(Cascade.All);
            });

            //Property(x => x.CompanyId, map => map.NotNullable(true));

            Property(x => x.Name, map =>
                                      {
                                          map.NotNullable(true);
                                          map.Length(Constants.NameColumnLength);
                                      });

            Property(x => x.Price, map =>
            {
                map.NotNullable(true);
            });

            Property(x => x.DateTime, map =>
            {
                map.NotNullable(true);
                
            });

            //Component<Price>(x => x.Price);

            Version(x => x.Version, map => map.Generated(VersionGeneration.Never));
        }
    }
}
