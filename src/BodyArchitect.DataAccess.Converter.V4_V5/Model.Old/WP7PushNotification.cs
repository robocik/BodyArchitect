using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model.Old
{
    public class WP7PushNotification
    {
        public string DeviceID { get; set; }

        public string URI { get; set; }

        public bool PushNotifications { get; set; }

        public DateTime Added { get; set; }

        public DateTime Modified { get; set; }

        public bool LiveTile { get; set; }

        public int LiveTileFrequency { get; set; }

        public DateTime? LiveTileLastUpdate { get; set; }

        public int ProfileId { get; set; }

        public int Counter { get; set; }
    }
}
