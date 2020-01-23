using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model.Old;
using FluentNHibernate.Mapping;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings.Old
{
    public class WymiaryMapping : ClassMap<Wymiary>
    {
        public WymiaryMapping()
        {
            this.LazyLoad();
            Id(x => x.Id);
            Map(x => x.DateTime).Not.Nullable();
            Map(x => x.IsNaCzczo);
            Map(x => x.Weight).Not.Nullable();
            Map(x => x.Klatka).Not.Nullable();
            Map(x => x.RightBiceps).Not.Nullable();
            Map(x => x.LeftBiceps).Not.Nullable();
            Map(x => x.Pas).Not.Nullable();
            Map(x => x.RightForearm).Not.Nullable();
            Map(x => x.LeftForearm).Not.Nullable();
            Map(x => x.RightUdo).Not.Nullable();
            Map(x => x.LeftUdo).Not.Nullable();
            Map(x => x.Height).Not.Nullable();

        }
    }
}
