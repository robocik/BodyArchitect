using BodyArchitect.Model;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.NHibernate.Mappings
{
    public class CustomerGroupMapping: ClassMapping<CustomerGroup>
    {
        public CustomerGroupMapping()
        {
            Id(x => x.GlobalId, map =>
            {
                map.Generator(Generators.GuidComb);
            });

            Property(x => x.Name, map =>
                                           {
                                               map.Length(Constants.NameColumnLength);
                                               map.NotNullable(true);
                                           });
            Property(x => x.Color, map =>
            {
                map.Length(Constants.ColorFieldLength);
                map.NotNullable(true);
            });
            Property(x => x.MaxPersons, map => map.NotNullable(true));
            Property(x => x.RestrictedType, map => map.NotNullable(true));
            Property(x => x.CreationDate, map =>
                                              {
                                                  map.NotNullable(true);
                                                  
                                              });

            ManyToOne(x => x.DefaultActivity, map =>
            {
                map.Column("Activity_id");
                map.NotNullable(false);
                map.Lazy(LazyRelation.Proxy);
            });

            ManyToOne(x => x.Profile, map =>
            {
                map.Column("Profile_id");
                map.NotNullable(true);
                map.Lazy(LazyRelation.Proxy);
            });

            Set(x => x.Customers, v =>
            {
                v.Table("Customer_Group");
                v.Lazy(CollectionLazy.Lazy);
                v.Cascade(Cascade.None);
                v.Key(g => g.Column("group_id"));
            }, h => h.ManyToMany(x=>x.Column("customer_id")));

            Version(x => x.Version, map => map.Generated(VersionGeneration.Never));

            
        }
    }
}
