using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Mapping.ByCode.Conformist;
using BodyArchitect.Model;
using NHibernate.Mapping.ByCode;

namespace BodyArchitect.DataAccess.NHibernate.Mappings
{
    public class ScheduleEntryMapping : JoinedSubclassMapping<ScheduleEntry>
    {
        public ScheduleEntryMapping()
        {
            Key(x => x.Column("ScheduleEntryBase_id"));

            ManyToOne(x => x.Activity, map =>
                                           {
                                               map.NotNullable(true);
                                           });

            ManyToOne(x => x.CustomerGroup, map =>
                                                {
                                                    map.NotNullable(false);
                                                });
        }
    }

    public class ScheduleEntryBaseMapping : ClassMapping<ScheduleEntryBase>
    {
        public ScheduleEntryBaseMapping()
        {
            Id(x => x.GlobalId, map =>
                              {
                                  map.Generator(Generators.GuidComb);
                              });

            ManyToOne(x => x.Profile, map =>
            {
                map.NotNullable(true);
            });
            ManyToOne(x => x.Reminder, map =>
            {
                map.Cascade(Cascade.All);
                map.NotNullable(false);
            });
            ManyToOne(x => x.MyPlace, map =>
            {
                map.Cascade(Cascade.None);
                map.NotNullable(true);
            });
            Property(x => x.State, map => map.NotNullable(true));
            Property(x => x.StartTime, map =>
                                           {
                                               map.NotNullable(true);
                                               
                                           });
            Property(x => x.EndTime, map =>
            {
                map.NotNullable(true);
                
            });

            Property(x => x.Price, map => map.NotNullable(true));

            Version(x => x.Version, map => map.Generated(VersionGeneration.Never));

            Set(x => x.Reservations, v =>
            {
                v.Cascade(Cascade.All | Cascade.DeleteOrphans);
                v.Inverse(true);
                v.Fetch(CollectionFetchMode.Subselect);
                v.Lazy(CollectionLazy.Lazy);
            }, h => h.OneToMany());
        }
    }
}
