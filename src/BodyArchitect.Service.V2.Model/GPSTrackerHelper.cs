using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using BodyArchitect.Portable;

namespace BodyArchitect.Service.V2.Model
{
    public static class GPSTrackerHelper
    {
        public static GeoCoordinate ToCoordinates(this GPSPoint point)
        {
            return new GeoCoordinate(point.Latitude, point.Longitude);
        }

        public static void CorrectGpsData(IList<GPSPoint> points)
        {
            Queue<GPSPoint> avgPoints = new Queue<GPSPoint>();
            for (int i = 0; i < points.Count; i++)
            {
                var currentPoint = points[i];
                if (! currentPoint.IsPoint())
                {//every pause or not available reset average calculation
                    avgPoints.Clear();
                }
                if (!float.IsNaN(currentPoint.Altitude) && currentPoint.IsPoint())
                {
                    if (avgPoints.Count == 4)//we take 3 previous items and the current to calculate the average altitude
                    {
                        avgPoints.Dequeue(); //remove older element
                    }
                    avgPoints.Enqueue(currentPoint);
                    
                    var avgAltitude = avgPoints.Average(x => x.Altitude);
                    points[i].Altitude = avgAltitude;
                    
                }
            }
        }

        private static double degreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private static double radianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        public static double CalculateInitialBearing(GPSPoint posA, GPSPoint posB)
        {
            var latA = degreeToRadian(posA.Latitude);
            var latB = degreeToRadian(posB.Latitude);
            var dLon = degreeToRadian(posB.Longitude - posA.Longitude);
            var y = Math.Sin(dLon) * Math.Cos(latB);
            var x = Math.Cos(latA) * Math.Sin(latB) -
                    Math.Sin(latA) * Math.Cos(latB) * Math.Cos(dLon);
            var brng = Math.Atan2(y, x);
            return (radianToDegree(brng) + 360)%360;
        }

        public static GPSPoint CalculateDestination(GPSPoint start, double bearing, double distance)
        {
            distance = distance/1000;//to km
            var radBearing = degreeToRadian(bearing);
            
            var R = 6371;
            var radDistance = distance / R;
            var lat = degreeToRadian(start.Latitude);
            var lon=degreeToRadian(start.Longitude);

            var lat2 = Math.Asin(Math.Sin(lat) * Math.Cos(radDistance) +
              Math.Cos(lat) * Math.Sin(radDistance) * Math.Cos(radBearing));
            var lon2 = lon + Math.Atan2(Math.Sin(radBearing) * Math.Sin(radDistance) * Math.Cos(lat),
                                 Math.Cos(radDistance) - Math.Sin(lat) * Math.Sin(lat2));
            lon2 = (lon2+3*Math.PI) % (2*Math.PI) - Math.PI;  // normalise to -180..+180º
            return new GPSPoint((float) radianToDegree(lat2), (float) radianToDegree(lon2), start.Altitude, 0, 0);

        }

        public static GPSPoint CalculateMidPoint(GPSPoint posA, GPSPoint posB)
        {
            GPSPoint midPoint = new GPSPoint();

            double dLon = degreeToRadian(posB.Longitude - posA.Longitude);
            double Bx = Math.Cos(degreeToRadian(posB.Latitude)) * Math.Cos(dLon);
            double By = Math.Cos(degreeToRadian(posB.Latitude)) * Math.Sin(dLon);

            midPoint.Latitude = (float) degreeToRadian(Math.Atan2(
                Math.Sin(degreeToRadian(posA.Latitude)) + Math.Sin(degreeToRadian(posB.Latitude)),
                Math.Sqrt(
                    (Math.Cos(degreeToRadian(posA.Latitude)) + Bx) *
                    (Math.Cos(degreeToRadian(posA.Latitude)) + Bx) + By * By)));

            midPoint.Longitude = (float) (posA.Longitude + radianToDegree(Math.Atan2(By, Math.Cos(degreeToRadian(posA.Latitude)) + Bx)));

            return midPoint;
        }

        public static double GetDistance(this IList<GPSPoint> points)
        {
            return gpsPointsOperation(points, (previous, current) =>
                {
                    var coordinate1 = new GeoCoordinate(previous.Latitude, previous.Longitude, previous.Altitude);
                    var coordinate2 = new GeoCoordinate(current.Latitude, current.Longitude, current.Altitude);
                    return coordinate2.GetDistanceTo(coordinate1);
                });
        }

        static double gpsPointsOperation(IList<GPSPoint> points, Func<GPSPoint,GPSPoint,double> action )
        {
            double totalAscents = 0;

            GPSPoint previousPoint = null;
            for (int index = 0; index < points.Count; index++)
            {
                var gpsPoint = points[index];
                if (gpsPoint.IsPause())
                {
                    previousPoint = null;
                    continue;
                }
                if (gpsPoint.IsNotAvailable())
                {
                    continue;
                }
                if (previousPoint != null)
                {
                    totalAscents += action(previousPoint, gpsPoint);
                }
                previousPoint = gpsPoint;
            }
            return totalAscents;
        }

        public static double GetTotalAscents(this IList<GPSPoint> points)
        {
            return gpsPointsOperation(points,(previous, current) =>
                {
                    var asc = current.Altitude - previous.Altitude;
                    if (asc > 0)
                    {
                        return asc;
                    }
                    return 0;
                });
        }

        public static double GetTotalDescends(this IList<GPSPoint> points)
        {
            return gpsPointsOperation(points, (previous, current) =>
            {
                var desc = current.Altitude - previous.Altitude;
                if (desc < 0)
                {
                    return Math.Abs(desc);
                }
                return 0;
            });
        }
    }
}
