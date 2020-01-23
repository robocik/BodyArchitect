using BodyArchitect.Model;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.NHibernate.Mappings
{
    public class SuplementMapping : ClassMapping<Suplement>
    {
        public SuplementMapping()
        {
            Id(x => x.GlobalId, map =>
            {
                map.Generator(Generators.Assigned);
            });
            
            Property(b => b.Name, y =>
            {
                y.NotNullable(true);
                y.Length(Constants.NameColumnLength);
            });
            Property(b => b.CreationDate, y =>
            {
                y.NotNullable(true);
            });

            Property(b => b.IsProduct, y =>
            {
                y.NotNullable(true);
            });

            Property(b => b.CanBeIllegal, y =>
            {
                y.NotNullable(true);
            });

            ManyToOne(x => x.Profile, map =>
            {
                map.Cascade(Cascade.None);
                map.NotNullable(false);
                map.Column("Profile_id");
            });
            Property(b => b.Comment, y =>
            {
                y.NotNullable(false);
                y.Type(NHibernateUtil.StringClob);
            });
            Property(b => b.Url, y =>
            {
                y.NotNullable(false);
                y.Length(Constants.UrlLength);
            });

            Property(b => b.Rating, y =>
            {
                y.NotNullable(true);
            });
        }
    }
}
