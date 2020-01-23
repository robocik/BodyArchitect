using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Portable;

namespace BodyArchitect.Service.Client.WP7.ModelExtensions
{
    public class GPSPointsBag
    {
        public GPSPointsBag()
        {
            Points = new List<GPSPoint>();
        }

        public GPSPointsBag(IEnumerable<GPSPoint> points,bool isSaved=true)
        {
            Points = new List<GPSPoint>(points);
            IsSaved = isSaved;
        }

        public bool IsSaved { get; set; }

        public IList<GPSPoint> Points { get; set; }
    }
}
