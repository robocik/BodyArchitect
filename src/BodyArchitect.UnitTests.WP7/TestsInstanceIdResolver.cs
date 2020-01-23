using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BodyArchitect.UnitTests.WP7
{
    [TestClass]
    public class TestsInstanceIdResolver
    {
        [TestMethod]
        public void TwoEntries_SameType_FindingByGlobalId()
        {
            TrainingDayDTO daySource = new TrainingDayDTO();
            daySource.GlobalId = Guid.NewGuid();
            daySource.InstanceId = Guid.NewGuid();
            SizeEntryDTO size = new SizeEntryDTO();
            size.GlobalId = Guid.NewGuid();
            size.InstanceId = Guid.NewGuid();
            daySource.Objects.Add(size);
            size.TrainingDay = daySource;
            SizeEntryDTO tracker = new SizeEntryDTO();
            tracker.InstanceId = Guid.NewGuid();
            tracker.GlobalId = Guid.NewGuid();
            daySource.Objects.Add(tracker);
            tracker.TrainingDay = daySource;

            var dayCopy = daySource.Copy();
            dayCopy.InstanceId = Guid.Empty;
            dayCopy.Objects[0].InstanceId = Guid.Empty;
            dayCopy.Objects[1].InstanceId = Guid.Empty;

            dayCopy.FillInstaneId(daySource);

            Assert.AreEqual(daySource.InstanceId, dayCopy.InstanceId);
            Assert.AreEqual(size.InstanceId, dayCopy.Objects.Single(x => x.GlobalId == size.GlobalId).InstanceId);
            Assert.AreEqual(tracker.InstanceId, dayCopy.Objects.Single(x => x.GlobalId == tracker.GlobalId).InstanceId);
        }

        [TestMethod]
        public void TwoEntries_EachDifferentType()
        {
            TrainingDayDTO daySource = new TrainingDayDTO();
            daySource.InstanceId = Guid.NewGuid();
            SizeEntryDTO size=new SizeEntryDTO();
            size.InstanceId = Guid.NewGuid();
            daySource.Objects.Add(size);
            size.TrainingDay = daySource;
            GPSTrackerEntryDTO tracker = new GPSTrackerEntryDTO();
            tracker.InstanceId = Guid.NewGuid();
            daySource.Objects.Add(tracker);
            tracker.TrainingDay = daySource;

            var dayCopy = daySource.Copy();
            dayCopy.GlobalId = Guid.NewGuid();
            dayCopy.Objects[0].GlobalId = Guid.NewGuid();
            dayCopy.Objects[1].GlobalId = Guid.NewGuid();
            dayCopy.InstanceId = Guid.Empty;
            dayCopy.Objects[0].InstanceId = Guid.Empty;
            dayCopy.Objects[1].InstanceId = Guid.Empty;

            dayCopy.FillInstaneId(daySource);

            Assert.AreEqual(daySource.InstanceId,dayCopy.InstanceId);
            Assert.AreEqual(daySource.Objects.OfType<SizeEntryDTO>().Single().InstanceId, dayCopy.Objects.OfType<SizeEntryDTO>().Single().InstanceId);
            Assert.AreEqual(daySource.Objects.OfType<GPSTrackerEntryDTO>().Single().InstanceId, dayCopy.Objects.OfType<GPSTrackerEntryDTO>().Single().InstanceId);
        }

        [TestMethod]
        public void TwoEntries_SameType_OneIsNew()
        {
            TrainingDayDTO daySource = new TrainingDayDTO();
            daySource.InstanceId = Guid.NewGuid();
            daySource.GlobalId = Guid.NewGuid();
            SizeEntryDTO size = new SizeEntryDTO();
            size.ReservationId = Guid.NewGuid();//for testing purposes
            size.InstanceId = Guid.NewGuid();
            size.GlobalId = Guid.NewGuid();
            daySource.Objects.Add(size);
            size.TrainingDay = daySource;
            SizeEntryDTO tracker = new SizeEntryDTO();
            tracker.InstanceId = Guid.NewGuid();
            tracker.ReservationId = Guid.NewGuid();//for testing purposes
            daySource.Objects.Add(tracker);
            tracker.TrainingDay = daySource;

            var dayCopy = daySource.Copy();
            dayCopy.Objects[1].GlobalId = Guid.NewGuid();
            dayCopy.InstanceId = Guid.Empty;
            dayCopy.Objects[0].InstanceId = Guid.Empty;
            dayCopy.Objects[1].InstanceId = Guid.Empty;

            dayCopy.FillInstaneId(daySource);

            Assert.AreEqual(daySource.InstanceId, dayCopy.InstanceId);
            Assert.AreEqual(size.ReservationId,dayCopy.Objects.Where(x => x.InstanceId == size.InstanceId).SingleOrDefault().ReservationId);
            Assert.AreEqual(tracker.ReservationId, dayCopy.Objects.Where(x => x.InstanceId == tracker.InstanceId).SingleOrDefault().ReservationId);
        }

        [TestMethod]
        public void TwoEntries_SameType_OneIsNew_WithFillOldGlobalId()
        {
            TrainingDayDTO daySource = new TrainingDayDTO();
            daySource.InstanceId = Guid.NewGuid();
            daySource.GlobalId = Guid.NewGuid();
            SizeEntryDTO size = new SizeEntryDTO();
            size.ReservationId = Guid.NewGuid();//for testing purposes
            size.InstanceId = Guid.NewGuid();
            size.GlobalId = Guid.NewGuid();
            daySource.Objects.Add(size);
            size.TrainingDay = daySource;
            SizeEntryDTO tracker = new SizeEntryDTO();
            tracker.InstanceId = Guid.NewGuid();
            tracker.ReservationId = Guid.NewGuid();//for testing purposes
            daySource.Objects.Add(tracker);
            tracker.TrainingDay = daySource;

            var dayCopy = daySource.Copy();
            dayCopy.Objects[1].GlobalId = Guid.NewGuid();
            dayCopy.InstanceId = Guid.Empty;
            dayCopy.Objects[0].InstanceId = Guid.Empty;
            dayCopy.Objects[1].InstanceId = Guid.Empty;

            dayCopy.FillInstaneId(daySource,true);

            Assert.AreEqual(daySource.GlobalId, dayCopy.GlobalId);
            Assert.AreEqual(size.GlobalId, dayCopy.Objects.Where(x => x.InstanceId == size.InstanceId).SingleOrDefault().GlobalId);
            Assert.AreEqual(tracker.GlobalId, dayCopy.Objects.Where(x => x.InstanceId == tracker.InstanceId).SingleOrDefault().GlobalId);
        }

       
    }
}
