using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model.Old;
using BodyArchitect.Shared;
using FluentNHibernate.Mapping;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings.Old
{
    public class TrainingDayMapping : ClassMap<TrainingDay>
    {
        public TrainingDayMapping()
        {
            this.Not.LazyLoad();
            Id(x => x.Id);
            Map(x => x.TrainingDate).Not.Nullable();
            Map(x => x.Comment).CustomType("StringClob").Nullable();
            References(x => x.Profile).Not.Nullable().LazyLoad();
            HasMany(x => x.Objects).Cascade.AllDeleteOrphan().Inverse().LazyLoad();
            Version(x => x.Version);
        }
    }
}
