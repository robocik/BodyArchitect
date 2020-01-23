using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.GPSTracker.Controls;
using BodyArchitect.Portable;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.GPSTracker
{
    public class GPSPointsProcessor
    {
        private IList<GPSPointViewModel> pointViewModels;
        private IList<LapViewModel> laps;

        public GPSPointsProcessor(IList<GPSPoint> points,decimal lapLength)
        {
            pointViewModels=processGPSPoints(points);
            laps=fillLapsInfo(pointViewModels, lapLength);
        }

        public IList<GPSPointViewModel> Points
        {
            get { return pointViewModels; }
        }

        public IList<LapViewModel> Laps
        {
            get { return laps; }
        }

        private IList<GPSPointViewModel> processGPSPoints(IList<GPSPoint> points)
        {
            var viewModels = new List<GPSPointViewModel>();
            decimal distance = 0;

            GPSPoint previousPoint = null;
            for (int index = 0; index < points.Count; index++)
            {
                var gpsPoint = points[index];
                if (gpsPoint.IsPause())
                {
                    previousPoint = null;
                }
                if (gpsPoint.IsPoint() && previousPoint != null)
                {
                    var coordinate1 = previousPoint.ToCoordinates();
                    var coordinate2 = gpsPoint.ToCoordinates();
                    distance += (decimal)coordinate2.GetDistanceTo(coordinate1);
                }
                GPSPointViewModel pointViewModel = new GPSPointViewModel(gpsPoint);
                pointViewModel.Distance = distance;
                viewModels.Add(pointViewModel);
                if (gpsPoint.IsPoint())
                {
                    previousPoint = gpsPoint;
                }
            }


            
            return viewModels;
        }

        private IList<LapViewModel> fillLapsInfo(IList<GPSPointViewModel> points, decimal lapLength)
        {
            IList<LapViewModel> laps = new List<LapViewModel>();

            var veryFirst = points.FirstOrDefault();
            GPSPointViewModel start = veryFirst;
            LapViewModel bestLap = null;
            decimal lapDistance = 0;
            for (int i = 1; i < points.Count; i++)
            {
                lapDistance = points[i].Distance - start.Distance;

                bool isLastPoint = i == points.Count - 1;
                if (lapDistance >= lapLength || isLastPoint)
                {
                    var endLapPoint = points[i];
                    //if distance is higher than exact lap length then create virtual gps point and calculate speed and duration
                    if (lapDistance > lapLength)
                    {
                        var above = lapDistance - lapLength;
                        var twoLastPointTime = (decimal)(points[i].Point.Duration - points[i - 1].Point.Duration);
                        var twoLastPointsDistance = points[i].Distance - points[i - 1].Distance;
                        var midDuration =twoLastPointTime- ((twoLastPointTime * above) / twoLastPointsDistance) + (decimal)points[i - 1].Point.Duration;

                        var bearing = GPSTrackerHelper.CalculateInitialBearing(points[i - 1].Point, points[i].Point);
                        var midPoint = GPSTrackerHelper.CalculateDestination(points[i - 1].Point, bearing, (double)((laps.Count * lapLength + (lapDistance - above)) - points[i - 1].Distance));



                        midPoint.Duration = (float) midDuration;
                        midPoint.Speed = points[i].Point.Speed;
                        GPSPointViewModel midPointViewModel = new GPSPointViewModel(midPoint);
                        midPointViewModel.Distance = (laps.Count + 1) * lapLength;
                        midPointViewModel.IsVirtual = true;
                        endLapPoint = midPointViewModel;//inject new virtual point as a end lap point
                        //now we should insert mid point between two points
                        points.Insert(i, midPointViewModel);
                        
                    }


                    var lapViewModel = new LapViewModel(start, endLapPoint);
                    lapViewModel.LapTime = TimeSpan.FromSeconds(lapViewModel.EndPoint.Point.Duration - lapViewModel.StartPoint.Point.Duration);
                    lapViewModel.TotalTime = TimeSpan.FromSeconds(lapViewModel.EndPoint.Point.Duration - veryFirst.Point.Duration);
                    lapViewModel.Speed = lapViewModel.LapTime.TotalSeconds > 0 ? (lapDistance / (decimal)lapViewModel.LapTime.TotalSeconds): 0;
                    laps.Add(lapViewModel);
                    lapViewModel.Nr = laps.Count;
                    lapViewModel.Distance = lapViewModel.EndPoint.Distance;
                    start = endLapPoint;

                    if (isLastPoint && lapDistance < lapLength)
                    {//if last lap is smaller than lap length
                        lapViewModel.FullLap = false;
                    }
                    if (bestLap == null || (bestLap.LapTime > lapViewModel.LapTime && lapViewModel.FullLap))
                    {
                        bestLap = lapViewModel;
                    }
                }
            }

            if (bestLap != null)
            {
                bestLap.BestLap = true;
            }
            return laps;
        }
    }
}
