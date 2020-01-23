using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using NHibernate;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings
{
    public class BATimeMapping: ComponentMapping<BATime>
    {
        public BATimeMapping()
        {
            Property(b => b.DateTime, y =>
                                          {
                                              y.NotNullable(true);
                                              
                                          });
            Property(b => b.TimeType, y => y.NotNullable(true));
        }
    }
}
