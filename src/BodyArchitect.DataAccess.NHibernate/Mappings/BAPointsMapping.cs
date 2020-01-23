using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.NHibernate.Mappings
{
    public class BAPointsMapping: ClassMapping<BAPoints>
    {
        public BAPointsMapping()
        {
            Id(x => x.GlobalId, map =>
            {
                map.Generator(Generators.GuidComb);
            });

            Property(b => b.ImportedDate, y =>
            {
                y.NotNullable(true);
            });


            Property(b => b.Points, y =>
            {
                y.NotNullable(true);
            });

            Property(b => b.Identifier, y =>
            {
                y.Length(256);
                y.NotNullable(true);
            });

            Property(b => b.Type, y =>
            {
                y.NotNullable(true);
            });
            
            ManyToOne(x => x.Profile, map =>
            {
                map.NotNullable(true);
                map.Cascade(Cascade.None);
            });

            ManyToOne(x => x.LoginData, map =>
            {
                map.NotNullable(false);
                map.Cascade(Cascade.None);
            });
        }
    }
}
