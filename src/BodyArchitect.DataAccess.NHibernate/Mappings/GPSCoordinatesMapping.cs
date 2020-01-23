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
    public class GPSCoordinatesMapping: ClassMapping<GPSCoordinates>
    {
        public GPSCoordinatesMapping()
        {
            Id(x => x.GlobalId, map =>
                                    {
                                        map.Generator(Generators.GuidComb);
                                    });
            Property(x=>x.Content,x=>
                                      {
                                          x.Type(NHibernateUtil.BinaryBlob);
                                          x.NotNullable(false);
                                      });
        }
    }
}
