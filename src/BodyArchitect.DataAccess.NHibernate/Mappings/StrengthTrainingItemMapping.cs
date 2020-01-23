using BodyArchitect.Model;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;


namespace BodyArchitect.DataAccess.NHibernate.Mappings
{
    public class StrengthTrainingItemMapping : ClassMapping<StrengthTrainingItem>
    {
        public StrengthTrainingItemMapping()
        {
            Id(x => x.GlobalId, map => map.Generator(Generators.GuidComb));
            ManyToOne(b => b.Exercise, y =>
            {
                y.Lazy(LazyRelation.NoLazy);
                y.NotNullable(true);
                y.Cascade(Cascade.None);
                y.Column("ExerciseId");
            });
            Property(b => b.Position, y =>
            {
                y.NotNullable(true);
            });

            Property(b => b.DoneWay, y =>
            {
                y.NotNullable(true);
            });
            //Map(x => x.ExerciseId).Not.Nullable();
            //Map(x => x.Position).Not.Nullable();
            ManyToOne(x => x.StrengthTrainingEntry, map =>
            {
                map.Column("StrengthTrainingEntry_id");
            });
            //References(x => x.StrengthTrainingEntry);
            Property(b => b.Comment, y =>
            {
                y.Type(NHibernateUtil.StringClob);
                y.NotNullable(false);
            });
            Property(b => b.TrainingPlanItemId, y =>
            {
                y.NotNullable(false);
            });
            Property(b => b.SuperSetGroup, y =>
            {
                y.NotNullable(false);
                y.Length(30);
            });
            //Map(x => x.Comment).CustomType("StringClob").Nullable();
            //Map(x => x.TrainingPlanItemId).Nullable();
            //Map(x => x.SuperSetGroup).Length(30).Nullable();
            Set(x => x.Series, v =>
            {
                //v.Table("UslugaPrice");
                v.Cascade(Cascade.All | Cascade.DeleteOrphans);
                v.Inverse(true);
                v.Key(g => g.Column("StrengthTrainingItem_id"));
                v.Fetch(CollectionFetchMode.Subselect);
                v.OrderBy(x=>x.Position);
                v.Lazy(CollectionLazy.NoLazy);
                v.BatchSize(Constants.DefaultBatchSize);
            }, h => h.OneToMany());
            //HasMany(x => x.Series).Cascade.AllDeleteOrphan().Inverse().Not.LazyLoad().Fetch.Subselect().BatchSize(20);
        }
    }
}
