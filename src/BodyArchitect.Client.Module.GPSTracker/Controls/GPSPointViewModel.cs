using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Portable;
using Microsoft.Maps.MapControl.WPF;

namespace BodyArchitect.Client.Module.GPSTracker.Controls
{
    public  class GPSPointViewModel:ViewModelBase
    {
        private GPSPoint point;
        private decimal _distance;

        public GPSPointViewModel(GPSPoint point)
        {
            this.point = point;
        }

        public decimal Distance
        {
            get { return _distance; }
            set
            {
                _distance = value;
                NotifyOfPropertyChange(()=>Distance);
            }
        }

        public GPSPoint Point
        {
            get { return point; }
        }

        public string DisplayAltitude
        {
            get
            {
                if (!float.IsNaN(Point.Altitude))
                {
                    return ((decimal) Point.Altitude).ToDisplayAltitude().ToString("0.#");
                }
                return "-1";
            }
        }

        public string DisplaySpeed
        {
            get
            {
                if (!float.IsNaN(Point.Speed))
                {
                    return ((decimal)point.Speed).ToDisplaySpeed().ToString("0.#");
                }
                return "-1";
            }
        }

        public string DisplayPace
        {
            get { return WilksFormula.PaceToString((float) ((decimal)point.Speed).ToDisplayPace(), true); }
        }

        public string DisplayDateTime
        {
            get
            {
                return TimeSpan.FromSeconds((int)point.Duration).ToString();
            }
        }

        public string DisplayDistance
        {
            get
            {
                return Distance.ToDisplayDistance().ToString("0.###");
            }
        }

        public Location ToLocation()
        {
            return new Location(Point.Latitude,point.Longitude,point.Altitude);
        }

        public bool IsVirtual { get; set; }

    }
}
