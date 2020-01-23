using BodyArchitect.Model;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings
{
    public class ScheduleEntryReservationMapping:UnionSubclassMapping<ScheduleEntryReservation>
    {
        public ScheduleEntryReservationMapping()
        {

            ManyToOne(x => x.ScheduleEntry, map =>
            {
                map.NotNullable(false);
                map.Cascade(Cascade.None);
            });

            Property(x => x.EnterDateTime, map =>
                                               {
                                                   map.NotNullable(false);
                                                   
                                               });
            Property(x => x.LeaveDateTime, map =>
                                               {
                                                   map.NotNullable(false);
                                                   
                                               });

            Set(x => x.EntryObjects, v =>
            {
                //v.Table("UslugaPrice");
                v.Cascade(Cascade.None);
                v.Inverse(true);
                v.Fetch(CollectionFetchMode.Select);
                v.Lazy(CollectionLazy.Lazy);
                v.Key(k => k.Column("Reservation_id"));
            }, h => h.OneToMany());
        }
    }
}
