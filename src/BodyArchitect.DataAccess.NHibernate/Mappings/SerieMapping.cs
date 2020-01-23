using BodyArchitect.Model;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;


namespace BodyArchitect.DataAccess.NHibernate.Mappings
{
    public class SerieMapping : ClassMapping<Serie>
    {
        public SerieMapping()
        {
            Id(x => x.GlobalId, map => map.Generator(Generators.GuidComb));
            Property(b => b.IsCiezarBezSztangi, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.CalculatedValue, y =>
            {
                y.NotNullable(false);
            });
            Property(b => b.SetType, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.RepetitionNumber, y =>
            {
                y.NotNullable(false);
            });
            Property(b => b.Position, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.IsIncorrect, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.IsSuperSlow, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.Weight, y =>
            {
                y.NotNullable(false);
            });
            Property(b => b.DropSet, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.IsRestPause, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.TrainingPlanItemId, y =>
            {
                y.NotNullable(false);
            });
            //TODO: Change UTC
            Property(b => b.StartTime, y =>
            {
                y.NotNullable(false);
            });
            Property(b => b.EndTime, y =>
            {
                y.NotNullable(false);
            });
            Property(b => b.Comment, y =>
            {
                y.Type(NHibernateUtil.StringClob);
                y.NotNullable(false);
            });
            ManyToOne(x => x.StrengthTrainingItem, map =>
            {
                map.Column("StrengthTrainingItem_id");
            });
            OneToOne(x => x.ExerciseProfileData, map =>
            {
                map.Cascade(Cascade.Remove | Cascade.DeleteOrphans);
                map.Lazy(LazyRelation.Proxy);
                map.Constrained(false);
                map.PropertyReference(typeof(ExerciseProfileData).GetProperty("Serie"));
                map.ForeignKey("none");
            });
        }

    }
}
