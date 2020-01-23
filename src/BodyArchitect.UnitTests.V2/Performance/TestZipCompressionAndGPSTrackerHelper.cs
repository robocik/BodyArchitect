using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.GPSTracker;
using BodyArchitect.Client.Module.GPSTracker.Controls;
using BodyArchitect.Portable;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NUnit.Framework;

namespace BodyArchitect.UnitTests.V2.Performance
{
    [TestFixture]
    public class TestZipCompressionAndGPSTrackerHelper
    {
        [Test]
        public void TestAltitudeCorrection()
        {
            IList<GPSPoint> pkt = new List<GPSPoint>();
            pkt.Add(new GPSPoint(50.9313049f, 17.2975941f, 10, 4, 1));
            pkt.Add(new GPSPoint(50.9311333f, 17.29805f, 31, 41, 2));
            pkt.Add(new GPSPoint(50.9310341f, 17.2986717f, 38, 42, 3));
            pkt.Add(new GPSPoint(50.9312172f, 17.2992649f, 33, 44, 5));
            pkt.Add(new GPSPoint(50.9329834f, 17.3020668f, 35, 45, 43));
            pkt.Add(new GPSPoint(50.933815f, 17.3040714f, 35, 62, 60));
            GPSTrackerHelper.CorrectGpsData(pkt);
            Assert.AreEqual(10,pkt[0].Altitude);
            Assert.AreEqual(20.5f, pkt[1].Altitude);
            Assert.AreEqual(22.833334f, pkt[2].Altitude);
            Assert.AreEqual(21.583334f, pkt[3].Altitude);
            Assert.AreEqual(24.9791679f, pkt[4].Altitude);
            Assert.AreEqual(26.098959f, pkt[5].Altitude);

        }

        [Test]
        public void TestAltitudeCorrection_WithNotAvailable()
        {
            IList<GPSPoint> pkt = new List<GPSPoint>();
            pkt.Add(new GPSPoint(50.9313049f, 17.2975941f, 10, 4, 1));
            pkt.Add(new GPSPoint(50.9311333f, 17.29805f, 31, 41, 2));
            pkt.Add(new GPSPoint(50.9310341f, 17.2986717f, 38, 42, 3));
            pkt.Add(GPSPoint.CreateNotAvailable(42.1f));
            pkt.Add(new GPSPoint(50.9312172f, 17.2992649f, 33, 44, 5));
            pkt.Add(new GPSPoint(50.9329834f, 17.3020668f, 35, 45, 43));
            pkt.Add(new GPSPoint(50.933815f, 17.3040714f, 35, 62, 60));
            GPSTrackerHelper.CorrectGpsData(pkt);
            Assert.AreEqual(10, pkt[0].Altitude);
            Assert.AreEqual(20.5f, pkt[1].Altitude);
            Assert.AreEqual(22.833334f, pkt[2].Altitude);
            Assert.AreEqual(true, float.IsNaN(pkt[3].Altitude));
            Assert.AreEqual(33, pkt[4].Altitude);
            Assert.AreEqual(34, pkt[5].Altitude);
            Assert.AreEqual(34, pkt[6].Altitude);

        }

        [Test]
        public void TestAltitudeCorrection_WithPause()
        {
            IList<GPSPoint> pkt = new List<GPSPoint>();
            pkt.Add(new GPSPoint(50.9313049f, 17.2975941f, 10, 4, 1));
            pkt.Add(new GPSPoint(50.9311333f, 17.29805f, 31, 41, 2));
            pkt.Add(new GPSPoint(50.9310341f, 17.2986717f, 38, 42, 3));
            pkt.Add(GPSPoint.CreatePause(42.1f));
            pkt.Add(new GPSPoint(50.9312172f, 17.2992649f, 33, 44, 5));
            pkt.Add(new GPSPoint(50.9329834f, 17.3020668f, 35, 45, 43));
            pkt.Add(new GPSPoint(50.933815f, 17.3040714f, 35, 62, 60));
            GPSTrackerHelper.CorrectGpsData(pkt);
            Assert.AreEqual(10, pkt[0].Altitude);
            Assert.AreEqual(20.5f, pkt[1].Altitude);
            Assert.AreEqual(22.833334f, pkt[2].Altitude);
            Assert.AreEqual(true,float.IsNaN(pkt[3].Altitude));
            Assert.AreEqual(33, pkt[4].Altitude);
            Assert.AreEqual(34, pkt[5].Altitude);
            Assert.AreEqual(34, pkt[6].Altitude);

        }
        [Test]
        public void ProcessGPSPoints()
        {
            decimal lapLength = 200;
            IList<GPSPoint> pkt = new List<GPSPoint>();
            pkt.Add(new GPSPoint(50.9313049f, 17.2975941f, 3, 4, 1));
            pkt.Add(new GPSPoint(50.9311333f, 17.29805f, 31, 41, 2));
            pkt.Add(new GPSPoint(50.9310341f, 17.2986717f, 32, 42, 3));
            pkt.Add(new GPSPoint(50.9312172f, 17.2992649f, 34, 44, 5));
            pkt.Add(new GPSPoint(50.9329834f, 17.3020668f, 35, 45, 43));
            pkt.Add(new GPSPoint(50.933815f, 17.3040714f, 35, 62, 60));

            
            GPSPointsProcessor processor = new GPSPointsProcessor(pkt, lapLength);
            var points = processor.Points;
            IList<LapViewModel> laps = processor.Laps;

            Assert.AreEqual(8, points.Count);
            foreach (var info in laps)
            {
                var lapDistance = info.EndPoint.Distance - info.StartPoint.Distance;
                Assert.IsTrue(lapDistance % 200==0 || !info.FullLap);
            }
            for (int i = 1; i < points.Count; i++)
            {
                Assert.Less(points[i - 1].Point.Duration, points[i].Point.Duration);
                Assert.Less(points[i - 1].Distance, points[i].Distance);
            }
            for (int i = 0; i < laps.Count; i++)
            {
                var startIndex=points.IndexOf(laps[i].StartPoint);
                var endIndex = points.IndexOf(laps[i].EndPoint);

                var lapPoints = points.Skip(startIndex).Take(endIndex - (startIndex - 1)).ToList();

                GPSPointViewModel previousPoint = null;
                decimal distance = 0;
                for (int index = 0; index < lapPoints.Count; index++)
                {
                    var gpsPoint = lapPoints[index];
                    if (previousPoint != null)
                    {
                        var coordinate1 = previousPoint.Point.ToCoordinates();
                        var coordinate2 = gpsPoint.Point.ToCoordinates();
                        distance += (decimal)coordinate2.GetDistanceTo(coordinate1);
                    }
                    previousPoint = gpsPoint;
                }
                Assert.IsTrue(lapLength == (int)distance || !laps[i].FullLap);
                
            }
            //check laps
            Assert.AreEqual(200,laps[0].Distance);
            Assert.AreEqual(400, laps[1].Distance);
            Assert.AreEqual(574.847243m, Math.Round(laps[2].Distance, 6));

            Assert.AreEqual(true, laps[0].FullLap);
            Assert.AreEqual(true, laps[1].FullLap);
            Assert.AreEqual(false, laps[2].FullLap);

            Assert.AreEqual(true, laps[0].BestLap);
            Assert.AreEqual(false, laps[1].BestLap);
            Assert.AreEqual(false, laps[2].BestLap);

            Assert.AreEqual(13.766d, laps[0].LapTime.TotalSeconds);
            Assert.AreEqual(27.343d, laps[1].LapTime.TotalSeconds);
            Assert.AreEqual(17.890999999999998d, laps[2].LapTime.TotalSeconds);

            Assert.AreEqual(13.766d, laps[0].TotalTime.TotalSeconds);
            Assert.AreEqual(41.108999999999995d, laps[1].TotalTime.TotalSeconds);
            Assert.AreEqual(59.0d, laps[2].TotalTime.TotalSeconds);

            //check added points (virtual)
            Assert.AreEqual(true,points[4].IsVirtual);
            Assert.AreEqual(200, points[4].Distance);
            Assert.AreEqual(34, points[4].Point.Altitude);
            Assert.AreEqual(14.766058f, points[4].Point.Duration);
            Assert.AreEqual(50.93167f, points[4].Point.Latitude);
            Assert.AreEqual(17.2999859f, points[4].Point.Longitude);

            Assert.AreEqual(true, points[5].IsVirtual);
            Assert.AreEqual(400, points[5].Distance);
            Assert.AreEqual(34, points[5].Point.Altitude);
            Assert.AreEqual(42.1086121f, points[5].Point.Duration);
            Assert.AreEqual(50.9329453f, points[5].Point.Latitude);
            Assert.AreEqual(17.3020039f, points[5].Point.Longitude);
        }

        [Test]
        public void ProcessGPSPoints2()
        {
            decimal lapLength = 200;
            IList<GPSPoint> pkt = new List<GPSPoint>();
            pkt.Add(new GPSPoint(50.9313049f, 17.2975941f, 3, 4, 1));
            pkt.Add(new GPSPoint(50.9311333f, 17.29805f, 31, 41, 2));
            pkt.Add(new GPSPoint(50.9310341f, 17.2986717f, 32, 42, 3));
            pkt.Add(new GPSPoint(50.9312172f, 17.2992649f, 34, 44, 5));
            pkt.Add(new GPSPoint(50.9329834f, 17.3020668f, 35, 45, 43));
            pkt.Add(new GPSPoint(50.933815f, 17.3040714f, 35, 62, 60));
            pkt.Add(new GPSPoint(50.934818f, 17.3050714f, 35, 62, 65));
            pkt.Add(new GPSPoint(50.935823f, 17.3060714f, 35, 62, 73));

            GPSPointsProcessor processor = new GPSPointsProcessor(pkt, lapLength);
            var points = processor.Points;
            IList<LapViewModel> laps = processor.Laps;

            Assert.AreEqual(12, points.Count);
            foreach (var info in laps)
            {
                var lapDistance = info.EndPoint.Distance - info.StartPoint.Distance;
                Assert.IsTrue(lapDistance % 200 == 0 || !info.FullLap);
            }
            for (int i = 1; i < points.Count; i++)
            {
                Assert.Less(points[i - 1].Point.Duration, points[i].Point.Duration);
                Assert.Less(points[i - 1].Distance, points[i].Distance);
            }
            for (int i = 0; i < laps.Count; i++)
            {
                var startIndex = points.IndexOf(laps[i].StartPoint);
                var endIndex = points.IndexOf(laps[i].EndPoint);

                var lapPoints = points.Skip(startIndex).Take(endIndex - (startIndex - 1)).ToList();

                GPSPointViewModel previousPoint = null;
                decimal distance = 0;
                for (int index = 0; index < lapPoints.Count; index++)
                {
                    var gpsPoint = lapPoints[index];
                    if (previousPoint != null)
                    {
                        var coordinate1 = previousPoint.Point.ToCoordinates();
                        var coordinate2 = gpsPoint.Point.ToCoordinates();
                        distance += (decimal)coordinate2.GetDistanceTo(coordinate1);
                    }
                    previousPoint = gpsPoint;
                }
                Assert.IsTrue(lapLength == (int)Math.Round(distance) || !laps[i].FullLap);

            }

            //check laps
            Assert.AreEqual(200, laps[0].Distance);
            Assert.AreEqual(400, laps[1].Distance);
            Assert.AreEqual(600, laps[2].Distance);
            Assert.AreEqual(800, laps[3].Distance);
            Assert.AreEqual(838.5874804m, Math.Round(laps[4].Distance, 7));

            Assert.AreEqual(true, laps[0].FullLap);
            Assert.AreEqual(true, laps[1].FullLap);
            Assert.AreEqual(true, laps[2].FullLap);
            Assert.AreEqual(true, laps[3].FullLap);
            Assert.AreEqual(false, laps[4].FullLap);

            Assert.AreEqual(false, laps[0].BestLap);
            Assert.AreEqual(false, laps[1].BestLap);
            Assert.AreEqual(false, laps[2].BestLap);
            Assert.AreEqual(true, laps[3].BestLap);
            Assert.AreEqual(false, laps[4].BestLap);

            Assert.AreEqual(13.766d, laps[0].LapTime.TotalSeconds);
            Assert.AreEqual(27.343d, laps[1].LapTime.TotalSeconds);
            Assert.AreEqual(18.844999999999999d, laps[2].LapTime.TotalSeconds);
            Assert.AreEqual(9.7059999999999995d, laps[3].LapTime.TotalSeconds);
            Assert.AreEqual(2.3399999999999999d, laps[4].LapTime.TotalSeconds);

            Assert.AreEqual(13.766d, laps[0].TotalTime.TotalSeconds);
            Assert.AreEqual(41.108999999999995d, laps[1].TotalTime.TotalSeconds);
            Assert.AreEqual(59.954000000000001d, laps[2].TotalTime.TotalSeconds);
            Assert.AreEqual(69.659999999999997d, laps[3].TotalTime.TotalSeconds);
            Assert.AreEqual(72.0d, laps[4].TotalTime.TotalSeconds);

            //check added points (virtual)
            Assert.AreEqual(true, points[4].IsVirtual);
            Assert.AreEqual(200, points[4].Distance);
            Assert.AreEqual(34, points[4].Point.Altitude);
            Assert.AreEqual(14.766058f, points[4].Point.Duration);
            Assert.AreEqual(50.93167f, points[4].Point.Latitude);
            Assert.AreEqual(17.2999859f, points[4].Point.Longitude);

            Assert.AreEqual(true, points[5].IsVirtual);
            Assert.AreEqual(400, points[5].Distance);
            Assert.AreEqual(34, points[5].Point.Altitude);
            Assert.AreEqual(42.1086121f, points[5].Point.Duration);
            Assert.AreEqual(50.9329453f, points[5].Point.Latitude);
            Assert.AreEqual(17.3020039f, points[5].Point.Longitude);

            Assert.AreEqual(true, points[8].IsVirtual);
            Assert.AreEqual(600, points[8].Distance);
            Assert.AreEqual(35.0f, points[8].Point.Altitude);
            Assert.AreEqual(60.953949f, points[8].Point.Duration);
            Assert.AreEqual(50.9340057f, points[8].Point.Latitude);
            Assert.AreEqual(17.3042622f, points[8].Point.Longitude);

            Assert.AreEqual(true, points[10].IsVirtual);
            Assert.AreEqual(800, points[10].Distance);
            Assert.AreEqual(35.0f, points[10].Point.Altitude);
            Assert.AreEqual(70.6596832f, points[10].Point.Duration);
            Assert.AreEqual(50.9355278f, points[10].Point.Latitude);
            Assert.AreEqual(17.3057804f, points[10].Point.Longitude);
        }

        [Test]
        public void SharpZipLib_CompressAndDecompress()
        {
            Random rand = new Random();
            byte[] data = new byte[3000];
            rand.NextBytes(data);
            var compressed=data.ToZip();
            var decompressed=compressed.FromZip();
            for (int i = 0; i < data.Length; i++)
            {
                Assert.AreEqual(data[i],decompressed[i]);
            }
        }
    }
}
