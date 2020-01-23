using FluentNHibernate.Mapping;
using FM.Model;
using FM.StrengthTraining.Model;

namespace FM.StrengthTraining.Factory
{
    public class StrengthTrainingItemMapping : ClassMap<StrengthTrainingItem>
    {
        public StrengthTrainingItemMapping()
        {
            Id(x => x.Id);
            Map(x => x.ExerciseId).Not.Nullable();
            Map(x => x.Position).Not.Nullable();
            References(x => x.StrengthTrainingEntry);
            Map(x => x.Comment).CustomSqlType("ntext").Nullable();
            Map(x => x.TrainingPlanItemId).Nullable();
            Map(x => x.SuperSetGroup).Nullable();
            HasMany(x => x.Series).Cascade.AllDeleteOrphan().Inverse().LazyLoad().Fetch.Subselect().BatchSize(Constants.DefaultBatchSize);
        }
    }
}
