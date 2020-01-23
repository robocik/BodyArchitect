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
    public class APIKeyMapping: ClassMapping<APIKey>
    {
        public APIKeyMapping()
        {
            Id(x => x.ApiKey, map =>
                              {
                                  map.Generator(Generators.Assigned);
                              });
            Property(x => x.ApplicationName, map =>
                                                 {
                                                     map.NotNullable(true);
                                                     map.Length(30);
                                                 } );
            Property(x => x.EMail, map =>
            {
                map.NotNullable(true);
                map.Length(50);
            });
            Property(x => x.Platform, map => map.NotNullable(true));
            Property(x => x.RegisterDateTime, map =>
                                                  {
                                                      map.NotNullable(true);
                                                      
                                                  });
        }
    }
}
