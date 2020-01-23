using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.NHibernate.Mappings
{
    class ReminderItemMapping: ClassMapping<ReminderItem>
    {
        public ReminderItemMapping()
        {
            Id(x => x.GlobalId, map =>
                      {
                          map.Generator(Generators.GuidComb);
                      });

            ManyToOne(x => x.Profile, map =>
                      {
                          map.Cascade(Cascade.None);
                          map.NotNullable(false);
                          map.Column("Profile_id");
                          map.Lazy(LazyRelation.Proxy);
                      });

            Property(b => b.Name, y =>
                     {
                         y.NotNullable(true);
                         y.Length(Constants.NameColumnLength);
                     });
            Property(b => b.Type, y =>
                    {
                        y.NotNullable(true);
                    });
            Property(b => b.Repetitions, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.DateTime, y =>
                    {
                        y.NotNullable(true);
                        
                    });
            Property(b => b.LastShown, y =>
            {
                y.NotNullable(false);
            });
            Property(b => b.RemindBefore, y =>
                    {
                        y.NotNullable(false);
                    });

            Property(b => b.ConnectedObject, y =>
            {
                y.NotNullable(false);
                y.Length(100);
            });
            Version(x => x.Version, map => map.Generated(VersionGeneration.Never));
        }
    }
}
