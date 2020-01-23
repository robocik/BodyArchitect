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
    public class AddressMapping : ClassMapping<Address>
    {
        public AddressMapping()
        {
            Id(x => x.GlobalId, map => map.Generator(Generators.GuidComb));


            Property(x => x.Country, map =>
                {
                    map.Length(Constants.NameColumnLength);
                    map.NotNullable(false);
                });

            Property(x => x.Country, map =>
            {
                map.Length(Constants.NameColumnLength);
                map.NotNullable(false);
            });

            Property(x => x.Address1, map =>
            {
                map.Length(200);
                map.NotNullable(false);
            });

            Property(x => x.Address2, map =>
            {
                map.Length(200);
                map.NotNullable(false);
            });

            Property(x => x.City, map =>
            {
                map.Length(Constants.NameColumnLength);
                map.NotNullable(false);
            });

            Property(x => x.PostalCode, map =>
            {
                map.Length(10);
                map.NotNullable(false);
            });
        }
    }
}
