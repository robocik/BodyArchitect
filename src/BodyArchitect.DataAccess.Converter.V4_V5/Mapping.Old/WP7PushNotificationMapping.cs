using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model.Old;
using FluentNHibernate.Mapping;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings.Old
{
    public class WP7PushNotificationMapping:ClassMap<WP7PushNotification>
    {
        public WP7PushNotificationMapping()
        {
            this.Not.LazyLoad();
            Id(x => x.DeviceID).Length(200).GeneratedBy.Assigned();
            Map(x => x.URI).Length(400).Not.Nullable();
            Map(x => x.PushNotifications).Not.Nullable();
            Map(x => x.Modified).Not.Nullable();
            Map(x => x.LiveTileLastUpdate).Nullable();
            Map(x => x.LiveTileFrequency).Not.Nullable();
            Map(x => x.LiveTile).Not.Nullable();
            Map(x => x.Added).Not.Nullable();
            Map(x => x.ProfileId).Not.Nullable();
            Map(x => x.Counter).Not.Nullable();
        }
    }
}
