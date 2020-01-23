using BodyArchitect.Model;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.NHibernate.Mappings
{
    public class CustomerMapping : ClassMapping<Customer>
    {
        public CustomerMapping()
        {
            Id(x => x.GlobalId, map =>
            {
                map.Generator(Generators.GuidComb);
            });

            Component(x => x.Picture);

            Property(x => x.FirstName, map =>
                                           {
                                               map.Length(Constants.NameColumnLength);
                                               map.NotNullable(true);
                                           });
            Property(x => x.LastName, map => map.Length(Constants.NameColumnLength));

            Property(x => x.Birthday);
            Property(x => x.Email, map => map.Length(200));
            Property(x => x.CreationDate, map =>
                                              {
                                                  map.NotNullable(true);
                                                  
                                              });
            Property(x => x.IsVirtual, map => map.NotNullable(true));
            Property(x => x.PhoneNumber, map => map.Length(30));
            Property(x => x.Gender, map => map.NotNullable(true));

            ManyToOne(x => x.Address, map =>
            {
                map.Column("Address_id");
                map.NotNullable(false);
                map.Cascade(Cascade.All);
                map.Lazy(LazyRelation.Proxy);
            });

            ManyToOne(x => x.Profile, map =>
            {
                map.Column("Profile_id");
                map.NotNullable(true);
                map.Lazy(LazyRelation.Proxy);
            });

            ManyToOne(x => x.Wymiary, map =>
            {
                map.Column("Wymiary_id");
                map.NotNullable(false);
                map.Cascade(Cascade.All);
                map.Lazy(LazyRelation.Proxy);
            });

            ManyToOne(x => x.Reminder, map =>
            {
                map.Column("BirthdayReminder_id");
                map.NotNullable(false);
                map.Cascade(Cascade.All);
                map.Lazy(LazyRelation.Proxy);
            });

            ManyToOne(x => x.Settings, map =>
            {
                map.Column("Settings_id");
                map.NotNullable(false);
                map.Cascade(Cascade.All);
                map.Lazy(LazyRelation.Proxy);
            });

            ManyToOne(x => x.ConnectedAccount, map =>
            {
                map.Column("ConnectedAccount_id");
                map.NotNullable(false);
                map.Lazy(LazyRelation.Proxy);
            });

            Set(x => x.Groups, v =>
            {
                v.Table("Customer_Group");
                v.Lazy(CollectionLazy.Lazy);
                v.Cascade(Cascade.None);
                v.Key(g => g.Column("customer_id"));
                v.Inverse(true);
            }, h => h.ManyToMany(x => x.Column("group_id")));

            Version(x => x.Version, map => map.Generated(VersionGeneration.Never));

            
        }
    }
}
