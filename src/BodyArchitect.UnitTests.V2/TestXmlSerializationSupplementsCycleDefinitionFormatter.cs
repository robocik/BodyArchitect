//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using BodyArchitect.Service.V2.Model;
//using NUnit.Framework;

//namespace BodyArchitect.UnitTests.V2
//{
//    [TestFixture]
//    public class TestXmlSerializationSupplementsCycleDefinitionFormatter
//    {
//        [Test]
//        public void ToAndFromXml()
//        {
//            var contentPlan = new SupplementCycleDefinitionDTO();
//            contentPlan.Name = "Plan name";
//            contentPlan.GlobalId = Guid.NewGuid();
//            SupplementCycleWeekDTO week = new SupplementCycleWeekDTO();
//            week.GlobalId = Guid.NewGuid();
//            week.Name = "Day 1";
//            week.CycleWeekStart = 1;
//            week.CycleWeekEnd = 6;
//            contentPlan.Weeks.Add(week);
//            var entry = new SupplementCycleDosageDTO();
//            entry.GlobalId = Guid.NewGuid();
//            entry.Supplement = new SuplementDTO() { GlobalId = Guid.NewGuid() };
//            entry.DosageType = DosageType.Grams;
//            entry.Repetitions=SupplementCycleDayRepetitions.TrainingDay;
//            entry.GroupName = "testName";
//            entry.Dosage = 3;
//            entry.TimeType = TimeType.OnEmptyStomach;
//            week.Dosages.Add(entry);

//            var formatter = new XmlSerializationSupplementsCycleDefinitionFormatter();
//            var xml = formatter.ToXml(contentPlan).ToString();
//            bool getExercisesInvoked = false;
//            var testPlan = formatter.FromXml(xml, delegate
//            {
//                getExercisesInvoked = true;
//                Dictionary<Guid, SuplementDTO> exercises = new Dictionary<Guid, SuplementDTO>();
//                exercises.Add(entry.Supplement.GlobalId, new SuplementDTO() { GlobalId = entry.Supplement.GlobalId, Name = "test ex" });
//                return exercises;
//            });
//            Assert.IsTrue(getExercisesInvoked);
//            UnitTestHelper.CompareObjects(contentPlan, testPlan);
//        }
//    }
//}
