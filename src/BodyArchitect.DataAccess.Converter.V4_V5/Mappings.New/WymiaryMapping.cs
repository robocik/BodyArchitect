using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings
{
    public class WymiaryMapping : ClassMapping<Wymiary>
    {
        public WymiaryMapping()
        {
            Id(x => x.GlobalId, map => map.Generator(Generators.GuidComb));
            Component(x => x.Time);

            Property(b => b.Weight, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.Klatka, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.RightBiceps, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.LeftBiceps, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.Pas, y =>
            {
                y.NotNullable(true);
            }); Property(b => b.RightForearm, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.LeftForearm, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.RightUdo, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.LeftUdo, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.Height, y =>
            {
                y.NotNullable(true);
            });
            //Map(x => x.DateTime).Not.Nullable();
            //Map(x => x.IsNaCzczo);
            //Map(x => x.Weight).Not.Nullable();
            //Map(x => x.Klatka).Not.Nullable();
            //Map(x => x.RightBiceps).Not.Nullable();
            //Map(x => x.LeftBiceps).Not.Nullable();
            //Map(x => x.Pas).Not.Nullable();
            //Map(x => x.RightForearm).Not.Nullable();
            //Map(x => x.LeftForearm).Not.Nullable();
            //Map(x => x.RightUdo).Not.Nullable();
            //Map(x => x.LeftUdo).Not.Nullable();
            //Map(x => x.Height).Not.Nullable();

        }
    }
}
