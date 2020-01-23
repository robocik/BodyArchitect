using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Portable;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.WP7.Controls;
using Microsoft.Phone.Controls.Maps.Platform;

namespace BodyArchitect.WP7.Utils
{
    public class GPSPointViewModel : ViewModelBase
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
                    return ((decimal)Point.Altitude).ToDisplayAltitude().ToString("0.#");
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
            get
            {
                if (!float.IsNaN(point.Speed))
                {
                    return WilksFormula.PaceToString((float)((decimal)point.Speed).ToDisplayPace(), true);
                }
                return "-";
            }
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
                return Distance.ToDisplayDistance().ToString("0.##");
            }
        }


        public bool IsVirtual { get; set; }

    }
}
