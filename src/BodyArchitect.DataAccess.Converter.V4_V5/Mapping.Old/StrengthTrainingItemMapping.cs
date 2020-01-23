using BodyArchitect.Model.Old;
using FluentNHibernate.Mapping;


namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings.Old
{
    public class StrengthTrainingItemMapping : ClassMap<StrengthTrainingItem>
    {
        public StrengthTrainingItemMapping()
        {
            this.Not.LazyLoad();
            Id(x => x.Id);
            Map(x => x.ExerciseId).Not.Nullable();
            Map(x => x.Position).Not.Nullable();
            References(x => x.StrengthTrainingEntry);
            Map(x => x.Comment).CustomType("StringClob").Nullable();
            Map(x => x.TrainingPlanItemId).Nullable();
            Map(x => x.SuperSetGroup).Length(30).Nullable();
            HasMany(x => x.Series).Cascade.AllDeleteOrphan().Inverse().LazyLoad();
        }
    }
}
