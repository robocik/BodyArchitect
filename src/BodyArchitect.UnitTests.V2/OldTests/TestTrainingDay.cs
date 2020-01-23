using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NUnit.Framework;
using BodyArchitect.Model;

namespace BodyArchitect.UnitTests.V2
{
    

    [TestFixture]
    public class TestTrainingDay
    {
        [EntryObjectOperations(EntryObjectOperation.Copy | EntryObjectOperation.Move)]
        class MyEntry1 : SpecificEntryObjectDTO
        {
            public override bool IsEmpty
            {
                get { return false; }
            }
        }

        [EntryObjectOperationsAttribute(EntryObjectOperation.None)]
        class MyEntry2 : SpecificEntryObjectDTO
        {
            public override bool IsEmpty
            {
                get { return true; }
            }
        }

        [EntryObjectOperationsAttribute(EntryObjectOperation.Copy | EntryObjectOperation.Move)]
        class MyEntry3 : SpecificEntryObjectDTO, IMovable
        {
            public DateTime NewDateTime;

            public void Move(DateTime newDateTime)
            {
                NewDateTime=newDateTime;
            }
        }


        [Test]
        public void TestCanMove_False()
        {
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.AddEntry(new MyEntry1());
            day.AddEntry(new MyEntry2());
            day.AddEntry(new MyEntry3());
            Assert.IsFalse(day.CanMove);
        }

        [Test]
        public void TestCanMove_True()
        {
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.AddEntry(new MyEntry1());
            day.AddEntry(new MyEntry1());
            day.AddEntry(new MyEntry3());
            Assert.IsTrue(day.CanMove);
        }

        [Test]
        public void TestChangeDate()
        {
            DateTime now = DateTime.Now;
            DateTime newDate = now.AddDays(2);
            TrainingDayDTO day = new TrainingDayDTO(now);
            day.AddEntry(new MyEntry1());
            day.AddEntry(new MyEntry1());
            var entry3 = new MyEntry3();
            day.AddEntry(entry3);
            day.ChangeDate(newDate);
            Assert.AreEqual(newDate.Date, day.TrainingDate);
            Assert.AreEqual(newDate, entry3.NewDateTime);

        }

        [Test]
        public void TestIsEmpty_ManyEntries_False()
        {
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.AddEntry(new MyEntry1());
            day.AddEntry(new MyEntry1());
            day.AddEntry(new MyEntry3());
            Assert.IsFalse((day.IsEmpty));

        }

        [Test]
        public void TestIsEmpty_ManyEntries_OneEntryIsEmpty_False()
        {
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.AddEntry(new MyEntry1());
            day.AddEntry(new MyEntry2());
            day.AddEntry(new MyEntry3());
            Assert.IsFalse((day.IsEmpty));

        }

        [Test]
        public void TestIsEmpty_ManyEntries_AllEmpty_True()
        {
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.AddEntry(new MyEntry2());
            day.AddEntry(new MyEntry2());
            day.AddEntry(new MyEntry2());
            Assert.IsTrue(day.IsEmpty);

        }

        [Test]
        public void TestIsEmpty_WithoutEntries_True()
        {
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            Assert.IsTrue(day.IsEmpty);

        }
    }
}
