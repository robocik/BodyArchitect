using System;
using System.Collections.Generic;
using System.Linq;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using NUnit.Framework;
using TrainingPlan = BodyArchitect.Service.V2.Model.TrainingPlans.TrainingPlan;
using TrainingPlanDay = BodyArchitect.Service.V2.Model.TrainingPlans.TrainingPlanDay;
using TrainingPlanEntry = BodyArchitect.Service.V2.Model.TrainingPlans.TrainingPlanEntry;
using TrainingPlanSerie = BodyArchitect.Service.V2.Model.TrainingPlans.TrainingPlanSerie;
using TrainingType = BodyArchitect.Service.V2.Model.TrainingPlans.TrainingType;


namespace BodyArchitect.UnitTests.V2.StrengthTraining
{
    [TestFixture]
    public class TestSplitPlanChecker
    {
        private IDictionary<Guid, Exercise> exercises;

        [TestFixtureSetUp]
        public void Setup()
        {
            ObjectsConverter.Configure();
            ExercisesBuilderPL exercisesBuilder = new ExercisesBuilderPL();
            exercises = exercisesBuilder.Create().ToDictionary(t => t.GlobalId);
        }

        [Test]
        public void TestGlobalRestSecondsNotSet()
        {
            TrainingPlan plan = new TrainingPlan();
            plan.TrainingType = TrainingType.Split;
            plan.RestSeconds = 0;
            plan.Name = "name";
            plan.Author = "author";
            checkSingle(plan, plan, TrainingPlanCheckItemStatus.Information, "TrainingPlan_RestSecond_Empty");
        }

        [Test]
        public void TestTrainingPlanNameNotSet()
        {
            TrainingPlan plan = new TrainingPlan();
            plan.TrainingType = TrainingType.Split;
            plan.Author = "test";
            plan.RestSeconds = 55;

            checkSingle(plan, plan, TrainingPlanCheckItemStatus.Error, "TrainingPlan_Name_Empty");
        }

        [Test]
        public void TestDropSetInNotOnlyLastSet()
        {
            TrainingPlan plan = new TrainingPlan();
            plan.RestSeconds = 60;
            plan.Name = "name";
            plan.Author = "author";
            plan.TrainingType = TrainingType.Split;
            var day = new TrainingPlanDay();
            day.Name = "day1";
            plan.AddDay(day);
            TrainingPlanEntry entry = new TrainingPlanEntry();
            entry.Exercise = new ExerciseLightDTO() { GlobalId = new Guid("9934E9E3-50DC-4B34-8B67-092084894740") };
            day.AddEntry(entry);
            entry.Sets.Add(new TrainingPlanSerie(10));
            entry.Sets.Add(new TrainingPlanSerie(10));
            var set1 = new TrainingPlanSerie(10);
            set1.DropSet = Service.V2.Model.DropSetType.IIDropSet;
            entry.Sets.Add(set1);
            var set2 = new TrainingPlanSerie(10);
            set2.DropSet = Service.V2.Model.DropSetType.IIDropSet;
            entry.Sets.Add(set2);

            checkSingle(plan, set1, TrainingPlanCheckItemStatus.Information, "TrainingPlan_DropSetInNotOnlyLastSet");
        }

        [Test]
        public void TestIsolationExerciseBeforeCompoud()
        {
            TrainingPlan plan = new TrainingPlan();
            plan.RestSeconds = 60;
            plan.Name = "name";
            plan.Author = "author";
            plan.TrainingType = TrainingType.Split;
            var day = new TrainingPlanDay();
            day.Name = "day1";
            plan.AddDay(day);
            TrainingPlanEntry entryIsolated = new TrainingPlanEntry();
            entryIsolated.Exercise = new ExerciseLightDTO() { GlobalId = new Guid("01C796E2-0119-4F95-9A9A-79F2890DF658") };
            day.AddEntry(entryIsolated);
            entryIsolated.Sets.Add(new TrainingPlanSerie(10));
            TrainingPlanEntry entryCompoud = new TrainingPlanEntry();
            entryCompoud.Exercise = new ExerciseLightDTO() { GlobalId = new Guid("9934E9E3-50DC-4B34-8B67-092084894740") };
            day.AddEntry(entryCompoud);
            entryCompoud.Sets.Add(new TrainingPlanSerie(10));
            
            checkSingle(plan, entryIsolated, TrainingPlanCheckItemStatus.Information, "Split_IsolationExerciseBeforeCompoud");
        }

        [Test]
        public void TestExerciseDoesntExist()
        {
            TrainingPlan plan = new TrainingPlan();
            plan.RestSeconds = 60;
            plan.Name = "name";
            plan.Author = "author";
            plan.TrainingType = TrainingType.Split;
            var day = new TrainingPlanDay();
            day.Name = "day1";
            plan.AddDay(day);
            TrainingPlanEntry entry = new TrainingPlanEntry();
            entry.Exercise = new ExerciseLightDTO() { GlobalId = new Guid("1134E9E3-50DC-4B34-8B67-092084894740") };
            day.AddEntry(entry);
            entry.Sets.Add(new TrainingPlanSerie(10));

            checkSingle(plan, entry, TrainingPlanCheckItemStatus.Warning, "TrainingPlan_ExerciseDoesntExist");
        }

        [Test]
        public void TestDayNameUnique()
        {
            TrainingPlan plan = new TrainingPlan();
            plan.RestSeconds = 60;
            plan.Name = "name";
            plan.Author = "author";
            plan.TrainingType = TrainingType.Split;
            var day1 = new TrainingPlanDay();
            day1.Name = "Day 1";
            plan.AddDay(day1);
            TrainingPlanEntry entry = new TrainingPlanEntry();
            entry.Exercise = new ExerciseLightDTO() { GlobalId = new Guid("9934E9E3-50DC-4B34-8B67-092084894740") };
            day1.AddEntry(entry);
            entry.Sets.Add(new TrainingPlanSerie(10));
            var day2 = new TrainingPlanDay();
            day2.Name = "Day 1";
            plan.AddDay(day2);
            entry = new TrainingPlanEntry();
            entry.Exercise = new ExerciseLightDTO() { GlobalId = new Guid("9934E9E3-50DC-4B34-8B67-092084894740") };
            day2.AddEntry(entry);
            entry.Sets.Add(new TrainingPlanSerie(10));

            checkSingle(plan, day2, TrainingPlanCheckItemStatus.Error, "TrainingPlan_DaysName_Unique");
        }

        [Test]
        public void TestDayNameEmpty()
        {
            TrainingPlan plan = new TrainingPlan();
            plan.RestSeconds = 60;
            plan.Name = "name";
            plan.Author = "author";
            plan.TrainingType = TrainingType.Split;
            var day1 = new TrainingPlanDay();
            plan.AddDay(day1);
            TrainingPlanEntry entry = new TrainingPlanEntry();
            entry.Exercise = new ExerciseLightDTO() { GlobalId = new Guid("9934E9E3-50DC-4B34-8B67-092084894740") };
            day1.AddEntry(entry);
            entry.Sets.Add(new TrainingPlanSerie(10));


            checkSingle(plan, day1, TrainingPlanCheckItemStatus.Error, "TrainingPlanDay_Name_Empty");
        }

        [Test]
        public void TestEmptyDay()
        {
            TrainingPlan plan = new TrainingPlan();
            plan.RestSeconds = 60;
            plan.Name = "name";
            plan.Author = "author";
            plan.TrainingType = TrainingType.Split;
            var day = new TrainingPlanDay();
            day.Name = "day1";
            plan.AddDay(day);
            checkSingle(plan, day, TrainingPlanCheckItemStatus.Information, "TrainingPlan_EmptyDay");
        }

        [Test]
        public void TestTwoIsolationExerciseBeforeCompoud()
        {
            TrainingPlan plan = new TrainingPlan();
            plan.RestSeconds = 60;
            plan.Name = "name";
            plan.Author = "author";
            plan.TrainingType = TrainingType.Split;
            var day = new TrainingPlanDay();
            day.Name = "day1";
            plan.AddDay(day);
            TrainingPlanEntry entryIsolated = new TrainingPlanEntry();
            entryIsolated.Exercise = new ExerciseLightDTO() { GlobalId = new Guid("01C796E2-0119-4F95-9A9A-79F2890DF658") };
            day.AddEntry(entryIsolated);
            entryIsolated.Sets.Add(new TrainingPlanSerie(10));
            TrainingPlanEntry secondIsolated = new TrainingPlanEntry();
            secondIsolated.Exercise = new ExerciseLightDTO() { GlobalId = new Guid("D6562C2A-BD74-46BE-8398-57B56BD698D5") };
            day.AddEntry(secondIsolated);
            secondIsolated.Sets.Add(new TrainingPlanSerie(10));
            TrainingPlanEntry entryCompoud = new TrainingPlanEntry();
            entryCompoud.Exercise = new ExerciseLightDTO() { GlobalId = new Guid("9934E9E3-50DC-4B34-8B67-092084894740") };
            day.AddEntry(entryCompoud);
            entryCompoud.Sets.Add(new TrainingPlanSerie(10));

            //Dictionary<Guid, ExerciseDTO> exerciseDtos = ObjectsConverter.ToExerciseDTO(exercises.Values).ToDictionary(t => t.GlobalId);
            Dictionary<Guid, ExerciseDTO> exerciseDtos = exercises.Values.Map<IList<ExerciseDTO>>().ToDictionary(t => t.GlobalId);
            TrainingPlanPack pack = TrainingPlanPack.Create(plan, exerciseDtos);
            SplitPlanChecker checker = new SplitPlanChecker(pack);
            var result = checker.Check(exerciseDtos);
            Assert.AreEqual(2, result.Results.Count);
            Assert.AreEqual(TrainingPlanCheckItemStatus.Information, result.Results[0].Status);
            Assert.AreEqual("Split_IsolationExerciseBeforeCompoud", result.Results[0].ResourceKey);
            Assert.AreEqual(entryIsolated.GlobalId, result.Results[0].TrainingPlanBase.GlobalId);
            Assert.AreEqual(TrainingPlanCheckItemStatus.Information, result.Results[1].Status);
            Assert.AreEqual("Split_IsolationExerciseBeforeCompoud", result.Results[1].ResourceKey);
            Assert.AreEqual(secondIsolated.GlobalId, result.Results[1].TrainingPlanBase.GlobalId);
        }

        private void checkSingle(TrainingPlan plan, BAGlobalObject planItem, TrainingPlanCheckItemStatus status, string resourceKey)
        {
            
            //Dictionary<Guid, ExerciseDTO> exerciseDtos = ObjectsConverter.ToExerciseDTO(exercises.Values).ToDictionary(t => t.GlobalId);
            Dictionary<Guid, ExerciseDTO> exerciseDtos = exercises.Values.Map<IList<ExerciseDTO>>().ToDictionary(t => t.GlobalId);
            TrainingPlanPack pack = TrainingPlanPack.Create(plan, exerciseDtos);
            SplitPlanChecker checker = new SplitPlanChecker(pack);
            var result = checker.Check(exerciseDtos);
            Assert.AreEqual(1, result.Results.Count);
            Assert.AreEqual(status , result.Results[0].Status);
            Assert.AreEqual(resourceKey, result.Results[0].ResourceKey);
            Assert.AreEqual(planItem.GlobalId, result.Results[0].TrainingPlanBase.GlobalId);
        }
    }
}
