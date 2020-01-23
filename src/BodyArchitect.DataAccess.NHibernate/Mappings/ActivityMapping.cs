using BodyArchitect.Model;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.NHibernate.Mappings
{
    public class ActivityMapping: ClassMapping<Activity>
    {
        public ActivityMapping()
        {
            Id(x => x.GlobalId, map => map.Generator(Generators.GuidComb));
            Property(x => x.Name, map =>
            {
                map.NotNullable(true);
                map.Length(Constants.NameColumnLength);
            } );
            ManyToOne(x => x.Profile, map =>
            {
                map.Column("Profile_id");
                map.NotNullable(true);
                map.Lazy(LazyRelation.Proxy);
            });

            Property(x => x.Duration, map => map.NotNullable(true));
            Property(x => x.MaxPersons, map => map.NotNullable(true));
            Property(x => x.Color, map =>
                                       {
                                           map.Length(Constants.ColorFieldLength);
                                           map.NotNullable(true);
                                       });
            Property(x => x.CreationDate, map =>
                                              {
                                                  map.NotNullable(true);
                                                  
                                              });
            Property(x => x.Price, map =>
            {
                map.NotNullable(true);
            });
            Version(x => x.Version, map => map.Generated(VersionGeneration.Never));
        }
    }
}
