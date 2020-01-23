using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Portable;

namespace BodyArchitect.Client.Module.GPSTracker.Controls
{
    public class LapViewModel : ViewModelBase
    {
        private int nr;
        private GPSPointViewModel startPoint;
        private GPSPointViewModel endPoint;
        private TimeSpan lapTime;
        private TimeSpan totalTime;
        private decimal distance;
        private decimal speed;
        private bool bestLap;
        private bool fullLap = true;

        public int Nr
        {
            get { return nr; }
            set
            {
                nr = value;
            }
        }

        public LapViewModel(GPSPointViewModel startPoint, GPSPointViewModel endPoint)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
        }

        public GPSPointViewModel StartPoint
        {
            get { return startPoint; }
        }



        public GPSPointViewModel EndPoint
        {
            get { return endPoint; }
        }

        public TimeSpan LapTime
        {
            get { return lapTime; }
            set { lapTime = value; }
        }

        public TimeSpan DisplayLapTime
        {
            get
            {
                return TimeSpan.FromSeconds((int)LapTime.TotalSeconds);
            }
        }

        public TimeSpan DisplayTotalTime
        {
            get
            {
                return TimeSpan.FromSeconds((int)TotalTime.TotalSeconds);
            }
        }

        public decimal DisplayDistance
        {
            get { return Distance.ToDisplayDistance(); }
        }

        public decimal DisplaySpeed
        {
            get { return Speed.ToDisplaySpeed(); }
        }

        public string Pace
        {
            get { return WilksFormula.PaceToString((float)Speed.ToDisplayPace(), true); }
        }


        public TimeSpan TotalTime
        {
            get { return totalTime; }
            set { totalTime = value; }
        }

        public bool FullLap
        {
            get { return fullLap; }
            set { fullLap = value; }
        }

        public bool BestLap
        {
            get { return bestLap; }
            set { bestLap = value; }
        }

        public decimal Distance
        {
            get { return distance; }
            set { distance = value; }
        }


        public decimal Speed
        {
            get { return speed; }
            set { speed = value; }
        }

    }
}
