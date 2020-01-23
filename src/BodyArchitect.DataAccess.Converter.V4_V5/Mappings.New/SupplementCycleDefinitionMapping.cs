using BodyArchitect.Model;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings
{
    public class SupplementCycleDefinitionMapping: ClassMapping<SupplementCycleDefinition>
    {
        public SupplementCycleDefinitionMapping()
        {
            Id(x => x.GlobalId, map =>
                      {
                          map.Generator(Generators.GuidComb);
                      });

            Property(b => b.Language, y =>
            {
                y.NotNullable(true);
                y.Length(10);
            });

            Property(b => b.CanBeIllegal, y =>
            {
                y.NotNullable(true);
            });

            Property(b => b.Url, y =>
            {
                y.NotNullable(false);
                y.Length(Constants.UrlLength);
            });

            Property(b => b.PublishDate, y =>
            {
                y.NotNullable(false);

            });

            Property(b => b.Status, y =>
            {
                y.NotNullable(true);
            });

            Property(b => b.Purpose, y =>
            {
                y.NotNullable(true);
            });

            Property(b => b.Difficult, y =>
            {
                y.NotNullable(true);
            });

            ManyToOne(x => x.BasedOn, map =>
            {
                map.Cascade(Cascade.None);
                map.NotNullable(false);
                map.Column("SupplementCycleDefinition_id");
                map.Lazy(LazyRelation.Proxy);
            });

            ManyToOne(x => x.Profile, map =>
                      {
                          map.Cascade(Cascade.None);
                          map.NotNullable(true);
                          map.Column("Profile_id");
                          map.Lazy(LazyRelation.Proxy);
                      });

            Property(b => b.Comment, y =>
            {
                y.NotNullable(false);
                y.Type(NHibernateUtil.StringClob);
            });

            Property(b => b.Author, y =>
            {
                y.NotNullable(false);
                y.Length(Constants.NameColumnLength);
            });

            Property(b => b.Name, y =>
                     {
                         y.NotNullable(true);
                         y.Length(Constants.NameColumnLength);
                     });

            Property(b => b.Rating, y =>
            {
                y.NotNullable(true);
            });


            Property(b => b.CreationDate, y =>
            {
                y.NotNullable(true);
            });

            Set(x => x.Weeks, v =>
            {
                v.Cascade(Cascade.All | Cascade.DeleteOrphans);
                v.Inverse(true);
                v.Key(c => c.Column("SupplementCycleDefinition_id"));
                v.Fetch(CollectionFetchMode.Subselect);
                v.Lazy(CollectionLazy.Lazy);
            }, h => h.OneToMany());

            Version(x => x.Version, map => map.Generated(VersionGeneration.Never));
        }
    }
}
