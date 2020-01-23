using BodyArchitect.Model;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.NHibernate.Mappings
{
    public class WP7PushNotificationMapping : ClassMapping<WP7PushNotification>
    {
        public WP7PushNotificationMapping()
        {
            Id(x => x.DeviceID, map =>
            {
                map.Generator(Generators.Assigned);
                map.Length(200);
            });

            Property(b => b.URI, y =>
            {
                y.NotNullable(true);
                y.Length(400);
            });
            Property(b => b.PushNotifications, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.Modified, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.LiveTileLastUpdate, y =>
            {
                y.NotNullable(false);
                
            });
            Property(b => b.LiveTileFrequency, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.LiveTile, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.Added, y =>
            {
                y.NotNullable(true);
                
            });
            Property(b => b.ProfileId, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.Counter, y =>
            {
                y.NotNullable(true);
            });
        }
    }
}
