using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Portable
{
    public class GPSPoint
    {
        public GPSPoint()
        {
        }

        public GPSPoint(float latitude, float longitude, float altitude, float speed, float duration)
        {
            Speed = speed;
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
            Duration = duration;
        }


        public static GPSPoint CreatePause(float duration)
        {
            return new GPSPoint(0, float.NaN, float.NaN, 0, duration);
        }
        public static GPSPoint CreateNotAvailable(float duration)
        {
            return new GPSPoint(float.NaN, 0, float.NaN, 0, duration);
        }

        public bool IsPause()
        {
            return float.IsNaN(Longitude);
        }

        public bool IsNotAvailable()
        {
            return float.IsNaN(Latitude );
    }

        public bool IsPoint()
        {
            return !IsPause() && !IsNotAvailable();
        }

        public float Speed { get; set; }

        public float Latitude { get; set; }

        public float Longitude { get; set; }

        public float Altitude { get; set; }

        public short? HearRate { get; set; }

        public float Duration { get; set; }

        
    }
}
