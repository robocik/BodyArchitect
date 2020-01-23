using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Services;
using NUnit.Framework;
using DosageType = BodyArchitect.Model.DosageType;
using SupplementCycleDayRepetitions = BodyArchitect.Service.V2.Model.SupplementCycleDayRepetitions;
using TimeType = BodyArchitect.Model.TimeType;

namespace BodyArchitect.UnitTests.V2.SupplementsCycles
{
    [TestFixture]
    public class TestSupplementsCycleRepetiter
    {
        [Test]
        public void WeeksNumberTheSameAsInDefinition_NoNeedRepetit()
        {
            var definition = createCycleForRepetitions1();
            var repetiter = new SupplementsCycleRepetiter();
            var newDef=repetiter.Preapre(definition, 7);
            for (int i = 0; i < definition.Weeks.Count; i++)
            {
                Assert.AreEqual(newDef.Weeks.ElementAt(i).CycleWeekStart, definition.Weeks.ElementAt(i).CycleWeekStart);
                Assert.AreEqual(newDef.Weeks.ElementAt(i).CycleWeekEnd, definition.Weeks.ElementAt(i).CycleWeekEnd);
            }
        }

        [Test]
        public void WeeksNumberLonger_NeedRepetit_1()
        {
            var definition = createCycleForRepetitions1();
            var repetiter = new SupplementsCycleRepetiter();
            var newDef = repetiter.Preapre(definition, 14);
            for (int i = 0; i < definition.Weeks.Count; i++)
            {
                Assert.AreEqual(newDef.Weeks.ElementAt(i).CycleWeekStart, definition.Weeks.ElementAt(i).CycleWeekStart);
                Assert.AreEqual(newDef.Weeks.ElementAt(i).CycleWeekEnd, definition.Weeks.ElementAt(i).CycleWeekEnd);
            }

            Assert.AreEqual(8, newDef.Weeks.Count);

            Assert.AreEqual(8,newDef.Weeks.ElementAt(4).CycleWeekStart);
            Assert.AreEqual(9,newDef.Weeks.ElementAt(4).CycleWeekEnd);
            Assert.AreEqual("Training week1",newDef.Weeks.ElementAt(4).Name);
            Assert.AreNotEqual(definition.Weeks.ElementAt(0).GlobalId, newDef.Weeks.ElementAt(4).GlobalId);

            Assert.AreEqual( 10,newDef.Weeks.ElementAt(5).CycleWeekStart);
            Assert.AreEqual(11,newDef.Weeks.ElementAt(5).CycleWeekEnd);
            Assert.AreEqual( "Training week3",newDef.Weeks.ElementAt(5).Name);
            Assert.AreNotEqual(definition.Weeks.ElementAt(2).GlobalId, newDef.Weeks.ElementAt(5).GlobalId);

            Assert.AreEqual(12,newDef.Weeks.ElementAt(6).CycleWeekStart);
            Assert.AreEqual(13,newDef.Weeks.ElementAt(6).CycleWeekEnd);
            Assert.AreEqual("Training week1",newDef.Weeks.ElementAt(6).Name);
            Assert.AreNotEqual(definition.Weeks.ElementAt(0).GlobalId, newDef.Weeks.ElementAt(6).GlobalId);

            Assert.AreEqual(14,newDef.Weeks.ElementAt(7).CycleWeekStart, 14);
            Assert.AreEqual(14,newDef.Weeks.ElementAt(7).CycleWeekEnd, 14);
            Assert.AreEqual("Training week3",newDef.Weeks.ElementAt(7).Name);
            Assert.AreNotEqual(definition.Weeks.ElementAt(2).GlobalId, newDef.Weeks.ElementAt(7).GlobalId);
        }

        [Test]
        public void WeeksNumberShorter_NoNeedRepetit_1()
        {
            var definition = createCycleForRepetitions1();
            var repetiter = new SupplementsCycleRepetiter();
            var newDef = repetiter.Preapre(definition, 5);
            for (int i = 0; i < definition.Weeks.Count; i++)
            {
                Assert.AreEqual(newDef.Weeks.ElementAt(i).CycleWeekStart, definition.Weeks.ElementAt(i).CycleWeekStart);
                Assert.AreEqual(newDef.Weeks.ElementAt(i).CycleWeekEnd, definition.Weeks.ElementAt(i).CycleWeekEnd);
            }

            Assert.AreEqual(4, newDef.Weeks.Count);
        }

        [Test]
        public void WeeksNumberLonger_NeedRepetit_2()
        {
            var definition = createCycleForRepetitions2();
            var repetiter = new SupplementsCycleRepetiter();
            var newDef = repetiter.Preapre(definition, 14);
            for (int i = 0; i < definition.Weeks.Count; i++)
            {
                Assert.AreEqual(newDef.Weeks.ElementAt(i).CycleWeekStart, definition.Weeks.ElementAt(i).CycleWeekStart);
                Assert.AreEqual(newDef.Weeks.ElementAt(i).CycleWeekEnd, definition.Weeks.ElementAt(i).CycleWeekEnd);
            }

            Assert.AreEqual(9,newDef.Weeks.Count);

            Assert.AreEqual(8, newDef.Weeks.ElementAt(4).CycleWeekStart);
            Assert.AreEqual(9, newDef.Weeks.ElementAt(4).CycleWeekEnd);
            Assert.AreEqual("Training week3", newDef.Weeks.ElementAt(4).Name);

            Assert.AreEqual(10, newDef.Weeks.ElementAt(5).CycleWeekStart);
            Assert.AreEqual(10, newDef.Weeks.ElementAt(5).CycleWeekEnd);
            Assert.AreEqual("Training week4", newDef.Weeks.ElementAt(5).Name);

            Assert.AreEqual(11, newDef.Weeks.ElementAt(6).CycleWeekStart);
            Assert.AreEqual(12, newDef.Weeks.ElementAt(6).CycleWeekEnd);
            Assert.AreEqual("Training week3", newDef.Weeks.ElementAt(6).Name);

            Assert.AreEqual(13, newDef.Weeks.ElementAt(7).CycleWeekStart);
            Assert.AreEqual(13, newDef.Weeks.ElementAt(7).CycleWeekEnd);
            Assert.AreEqual("Training week4", newDef.Weeks.ElementAt(7).Name);

            Assert.AreEqual(14, newDef.Weeks.ElementAt(8).CycleWeekStart);
            Assert.AreEqual(15, newDef.Weeks.ElementAt(8).CycleWeekEnd);
            Assert.AreEqual("Training week3", newDef.Weeks.ElementAt(8).Name);
        }

        protected SupplementCycleDosageDTO CreateDosage(decimal dosageValue, SuplementDTO supplement, SupplementCycleDayRepetitions repetitions = SupplementCycleDayRepetitions.OnceAWeek, Service.V2.Model.DosageType dosageType = Service.V2.Model.DosageType.MiliGrams, Service.V2.Model.TimeType timeType = Service.V2.Model.TimeType.NotSet)
        {
            var dosage = new SupplementCycleDosageDTO();
            dosage.Dosage = dosageValue;
            dosage.DosageType = dosageType;
            dosage.Repetitions = repetitions;
            dosage.Supplement = supplement;
            dosage.TimeType =  timeType;
            return dosage;
        }

        private SupplementCycleDefinitionDTO createCycleForRepetitions1()
        {
            var supple = new SuplementDTO();
            var cycleDefinition = new SupplementCycleDefinitionDTO();
            cycleDefinition.Name = "sterydy";
            cycleDefinition.Language = "en";
            var week = new SupplementCycleWeekDTO();
            week.Name = "Training week1";
            week.CycleWeekStart = 1;
            week.CycleWeekEnd = 2;
            cycleDefinition.Weeks.Add(week);
            var dosage = CreateDosage(400, supple);
            week.Dosages.Add(dosage);
            dosage = CreateDosage(750, supple);
            week.Dosages.Add(dosage);

            week = new SupplementCycleWeekDTO();
            week.Name = "Training week2";
            week.CycleWeekStart = 3;
            week.CycleWeekEnd = 4;
            week.IsRepetitable = false;
            cycleDefinition.Weeks.Add(week);
            dosage = CreateDosage(400, supple);
            week.Dosages.Add(dosage);
            dosage = CreateDosage(750, supple);
            week.Dosages.Add(dosage);

            week = new SupplementCycleWeekDTO();
            week.Name = "Training week3";
            week.CycleWeekStart = 5;
            week.CycleWeekEnd = 6;
            cycleDefinition.Weeks.Add(week);
            dosage = CreateDosage(300, supple);
            week.Dosages.Add(dosage);
            dosage = CreateDosage(700, supple);
            week.Dosages.Add(dosage);

            week = new SupplementCycleWeekDTO();
            week.Name = "Training week4";
            week.CycleWeekStart = 7;
            week.CycleWeekEnd = 7;
            week.IsRepetitable = false;
            cycleDefinition.Weeks.Add(week);
            dosage = CreateDosage(410, supple);
            week.Dosages.Add(dosage);
            dosage = CreateDosage(650, supple);
            week.Dosages.Add(dosage);


            return cycleDefinition;
        }

        private SupplementCycleDefinitionDTO createCycleForRepetitions2()
        {
            SuplementDTO supple = new SuplementDTO();
            var cycleDefinition = new SupplementCycleDefinitionDTO();
            cycleDefinition.Name = "sterydy";
            cycleDefinition.Language = "en";
            var week = new SupplementCycleWeekDTO();
            week.Name = "Training week1";
            week.CycleWeekStart = 1;
            week.CycleWeekEnd = 2;
            week.IsRepetitable = false;
            cycleDefinition.Weeks.Add(week);
            var dosage = CreateDosage(400, supple);
            week.Dosages.Add(dosage);
            dosage = CreateDosage(750, supple);
            week.Dosages.Add(dosage);

            week = new SupplementCycleWeekDTO();
            week.Name = "Training week2";
            week.CycleWeekStart = 3;
            week.CycleWeekEnd = 4;
            week.IsRepetitable = false;
            cycleDefinition.Weeks.Add(week);
            dosage = CreateDosage(400, supple);
            week.Dosages.Add(dosage);
            dosage = CreateDosage(750, supple);
            week.Dosages.Add(dosage);

            week = new SupplementCycleWeekDTO();
            week.Name = "Training week3";
            week.CycleWeekStart = 5;
            week.CycleWeekEnd = 6;
            cycleDefinition.Weeks.Add(week);
            dosage = CreateDosage(300, supple);
            week.Dosages.Add(dosage);
            dosage = CreateDosage(700, supple);
            week.Dosages.Add(dosage);

            week = new SupplementCycleWeekDTO();
            week.Name = "Training week4";
            week.CycleWeekStart = 7;
            week.CycleWeekEnd = 7;
            cycleDefinition.Weeks.Add(week);
            dosage = CreateDosage(410, supple);
            week.Dosages.Add(dosage);
            dosage = CreateDosage(650, supple);
            week.Dosages.Add(dosage);


            return cycleDefinition;
        }
    }
}
