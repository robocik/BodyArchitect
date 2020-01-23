using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model.Old;
using FluentNHibernate.Mapping;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings.Old
{
    public class StrengthTrainingEntryMapping : SubclassMap<StrengthTrainingEntry>
    {
        public StrengthTrainingEntryMapping()
        {
            this.Not.LazyLoad();
            Map(x => x.EndTime).Nullable();
            Map(x => x.StartTime).Nullable();
            Map(x => x.TrainingPlanItemId).Nullable();
            Map(x => x.TrainingPlanId).Nullable();
            Map(x => x.Intensity).CustomType<Intensity>().Not.Nullable();
            HasMany(x => x.Entries).Cascade.AllDeleteOrphan().Inverse().LazyLoad();
        }
    }
}
