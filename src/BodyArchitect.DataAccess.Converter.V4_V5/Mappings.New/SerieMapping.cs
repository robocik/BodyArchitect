using BodyArchitect.Model;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;


namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings
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
            Property(b => b.SetType, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.RepetitionNumber, y =>
            {
                y.NotNullable(false);
            });
            Property(b => b.Weight, y =>
            {
                y.NotNullable(false);
            });
            Property(b => b.IsIncorrect, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.DropSet, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.Position, y =>
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
            //Map(x => x.IsCiezarBezSztangi).Not.Nullable();
            //Map(x => x.SetType).CustomType<SetType>().Not.Nullable();
            //Map(x => x.RepetitionNumber).Nullable();
            //Map(x => x.Weight).Nullable();
            //Map(x => x.DropSet).CustomType<DropSetType>().Not.Nullable();
            //Map(x => x.TrainingPlanItemId).Nullable();
            //Map(x => x.StartTime).Nullable();
            //Map(x => x.EndTime).Nullable();
            //Map(x => x.Comment).CustomType("StringClob").Nullable();
            //References(x => x.StrengthTrainingItem);
            ManyToOne(x => x.StrengthTrainingItem, map =>
            {
                map.Column("StrengthTrainingItem_id");
            });

        }

    }
}
