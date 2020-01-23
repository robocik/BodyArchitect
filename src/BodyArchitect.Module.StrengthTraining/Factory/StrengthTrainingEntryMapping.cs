using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using FM.Model;
using FM.StrengthTraining.Model;

namespace FM.StrengthTraining.Factory
{
    public class StrengthTrainingEntryMapping : SubclassMap<StrengthTrainingEntry>
    {
        public StrengthTrainingEntryMapping()
        {
            Map(x => x.EndTime).Nullable();
            Map(x => x.StartTime).Nullable();
            Map(x => x.TrainingPlanItemId).Nullable();
            Map(x => x.Intensity).CustomType<Intensity>().Not.Nullable();
            HasMany(x => x.Entries).Cascade.AllDeleteOrphan().Inverse().Not.LazyLoad().Fetch.Subselect().BatchSize(Constants.DefaultBatchSize).OrderBy("Position ASC");
        }
    }
}
