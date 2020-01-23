using BodyArchitect.Model;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings
{
    public class TrainingDayCommentMapping : ClassMapping<TrainingDayComment>
    {
        public TrainingDayCommentMapping()
        {
            Id(x => x.GlobalId, map => map.Generator(Generators.GuidComb));
            ManyToOne(x => x.Profile, g =>
            {
                g.NotNullable(true);
                g.Column("Profile_id");

            });
            ManyToOne(x => x.TrainingDay, g =>
            {
                g.NotNullable(true);
                g.Column("TrainingDay_id");

            });
            Property(b => b.Comment, y =>
            {
                y.NotNullable(true);
                y.Type(NHibernateUtil.StringClob);
            });
            Property(b => b.DateTime, y =>
            {
                y.NotNullable(true);
                
            });
            ManyToOne(x => x.LoginData, g =>
            {
                g.NotNullable(false);
                g.Column("LoginData_id");
                g.Cascade(Cascade.None);
            });
            //References(x => x.Profile).Not.Nullable();
            //References(x => x.BlogEntry).Not.Nullable();
            //Map(x => x.Comment).CustomType("StringClob").Not.Nullable();
            //Map(x => x.DateTime).Not.Nullable();
            //Map(x => x.CommentType).CustomType<ContentType>().Not.Nullable();
        }
    }
}
