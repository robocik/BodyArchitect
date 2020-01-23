using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Portable;
using BodyArchitect.Service.V2.Model;
using NUnit.Framework;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestGpsHelper
    {
        [Test]
        public void GetDistance()
        {
            IList<GPSPoint> pkt = new List<GPSPoint>();
            pkt.Add(new GPSPoint(50.9313049f, 17.2975941f, 3, 4, 1));
            pkt.Add(new GPSPoint(50.9311333f, 17.29805f, 31, 41, 2));
            pkt.Add(new GPSPoint(50.9310341f, 17.2986717f, 32, 42, 3));
            pkt.Add(new GPSPoint(50.9312172f, 17.2992649f, 34, 44, 5));
            pkt.Add(new GPSPoint(50.9329834f, 17.3020668f, 35, 45, 6));
            pkt.Add(new GPSPoint(50.933815f, 17.3040714f, 35, 45,7));

            var distance=GPSTrackerHelper.GetDistance(pkt);
            Assert.AreEqual(574.84724282491675,distance);
        }

        [Test]
        public void GetDistance_WithNotAvailablePoint()
        {
            IList<GPSPoint> pkt = new List<GPSPoint>();
            pkt.Add(new GPSPoint(50.9313049f, 17.2975941f, 3, 4, 1));
            pkt.Add(new GPSPoint(50.9311333f, 17.29805f, 31, 41, 2));
            pkt.Add(new GPSPoint(50.9310341f, 17.2986717f, 32, 42, 3));
            pkt.Add(new GPSPoint(50.9312172f, 17.2992649f, 34, 44, 5));
            pkt.Add(GPSPoint.CreateNotAvailable(5.1f));
            pkt.Add(new GPSPoint(50.9329834f, 17.3020668f, 35, 45, 6));
            pkt.Add(new GPSPoint(50.933815f, 17.3040714f, 35, 45, 7));

            var distance = GPSTrackerHelper.GetDistance(pkt);
            Assert.AreEqual(574.84724282491675, distance);
        }

        [Test]
        public void GetDistance_WithPausePoint()
        {
            IList<GPSPoint> pkt = new List<GPSPoint>();
            pkt.Add(new GPSPoint(50.9313049f, 17.2975941f, 3, 4, 1));
            pkt.Add(new GPSPoint(50.9311333f, 17.29805f, 31, 41, 2));
            pkt.Add(new GPSPoint(50.9310341f, 17.2986717f, 32, 42, 3));
            pkt.Add(new GPSPoint(50.9312172f, 17.2992649f, 34, 44, 5));
            pkt.Add(GPSPoint.CreatePause(5.1f));
            pkt.Add(new GPSPoint(50.9329834f, 17.3020668f, 35, 45, 6));
            pkt.Add(new GPSPoint(50.933815f, 17.3040714f, 35, 45, 7));

            var distance = GPSTrackerHelper.GetDistance(pkt);
            Assert.AreEqual(296.89223243105414, distance);
        }
    }
}
