using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Portable;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NUnit.Framework;
using BodyArchitect.Model;

namespace BodyArchitect.UnitTests.V2
{
    public class MyEntry1 : SpecificEntryObjectDTO
    {
    }

    public class MyEntry2 : SpecificEntryObjectDTO
    {
    }

    public class MyEntry3 : EntryObjectDTO
    {
    }

    [TestFixture]
    public class TestModelHelper
    {
        [Test]
        public void TestSplitByType_All_ExtensionMethod()
        {
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.AddEntry(new MyEntry1());
            day.AddEntry(new MyEntry3());
            day.AddEntry(new MyEntry1());
            var groups = day.SplitByType(false);
            Assert.AreEqual(2, groups.Count);
            Assert.AreEqual(2, groups[typeof(MyEntry1)].Count);
            Assert.AreEqual(1, groups[typeof(MyEntry3)].Count);
        }

        [Test]
        public void TestSplitByType_LoadedOnly_ExtensionMethod()
        {
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.AddEntry(new MyEntry1());
            day.AddEntry(new MyEntry3());
            day.AddEntry(new MyEntry1());
            var groups = day.SplitByType(true);
            Assert.AreEqual(1, groups.Count);
            Assert.AreEqual(2, groups[typeof(MyEntry1)].Count);
        }

        [Test]
        public void TestSplitByType_ExtensionMethod()
        {
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.AddEntry(new MyEntry1());
            day.AddEntry(new MyEntry2());
            day.AddEntry(new MyEntry1());
            var groups = day.SplitByType(true);
            Assert.AreEqual(2, groups.Count);
            Assert.AreEqual(2, groups[typeof(MyEntry1)].Count);
            Assert.AreEqual(1, groups[typeof(MyEntry2)].Count);
        }

        [Test]
        public void TestSplitByType_EmptyList_ExtensionMethod()
        {
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            var groups = day.SplitByType(true);
            Assert.AreEqual(0, groups.Count);
        }

        #region BAHelper

        [Test]
        public void TestMovedToNewDate()
        {
            DateTime dateTime = new DateTime(2010,8,3,7,52,0,0);
            DateTime newDateTime = new DateTime(2010, 8, 5, 4, 33, 0, 0);
            DateTime movedDateTime = DateHelper.MoveToNewDate(dateTime, newDateTime);
            Assert.AreEqual(2010,movedDateTime.Year);
            Assert.AreEqual(8, movedDateTime.Month);
            Assert.AreEqual(5, movedDateTime.Day);
            Assert.AreEqual(7, movedDateTime.Hour);
            Assert.AreEqual(52, movedDateTime.Minute);
            Assert.AreEqual(0, movedDateTime.Second);
        }
        #endregion
    }
}
