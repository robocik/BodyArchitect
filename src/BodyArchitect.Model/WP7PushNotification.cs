using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model
{
    public class WP7PushNotification
    {
        public virtual string DeviceID { get; set; }

        public virtual string URI { get; set; }

        public virtual bool PushNotifications { get; set; }

        public virtual DateTime Added { get; set; }

        public virtual DateTime Modified { get; set; }

        public virtual bool LiveTile { get; set; }

        public virtual int LiveTileFrequency { get; set; }

        public virtual DateTime? LiveTileLastUpdate { get; set; }

        public virtual Guid ProfileId { get; set; }

        public virtual int Counter { get; set; }
    }
}
