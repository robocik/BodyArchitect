using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.NHibernate.Mappings
{
    public class CustomerSettingsMapping : ClassMapping<CustomerSettings>
    {
        public CustomerSettingsMapping()
        {
            Id(x => x.GlobalId, map =>
            {
                map.Generator(Generators.GuidComb);
            });
            Property(b => b.AutomaticUpdateMeasurements, y =>
            {
                y.NotNullable(true);
            });
        }
    }
}
