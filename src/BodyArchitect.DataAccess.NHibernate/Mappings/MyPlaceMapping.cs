using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Shared;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.NHibernate.Mappings
{
    public class MyPlaceMapping:ClassMapping<MyPlace>
    {
        public MyPlaceMapping()
        {
            Id(x => x.GlobalId, map =>
            {
                map.Generator(Generators.GuidComb);
            });

            Property(b => b.IsSystem, y =>
            {
                y.NotNullable(true);
            });

            Property(b => b.IsDefault, y =>
            {
                y.NotNullable(true);
            });

            Property(b => b.NotForRecords, y =>
            {
                y.NotNullable(true);
            });

            ManyToOne(x => x.Address, g =>
            {
                g.NotNullable(false);
                g.Column("Address_id");
                g.Lazy(LazyRelation.Proxy);
                g.Cascade(Cascade.All);
            });

            Property(b => b.CreationDate, y =>
            {
                y.NotNullable(true);
            });

            Version(x => x.Version, map => map.Generated(VersionGeneration.Never));

            Property(b => b.Name, y =>
            {
                y.NotNullable(true);
                y.Length(Constants.NameColumnLength);
            });

            ManyToOne(x => x.Profile, g =>
            {
                g.NotNullable(true);
                g.Column("Profile_id");
                g.Lazy(LazyRelation.Proxy);
                g.Cascade(Cascade.None);
            });

            Set(x => x.Entries, v =>
            {
                //v.Table("UslugaPrice");
                v.Cascade(Cascade.None);
                v.Inverse(true);
                v.Key(c => c.Column("MyPlace_id"));
                v.Fetch(CollectionFetchMode.Subselect);
                v.Lazy(CollectionLazy.Extra);
                v.BatchSize(Constants.DefaultBatchSize);

            }, h => h.OneToMany());

            Property(x => x.Color, map =>
            {
                map.Length(Constants.ColorFieldLength);
                map.NotNullable(true);
            });
        }
    }
}
