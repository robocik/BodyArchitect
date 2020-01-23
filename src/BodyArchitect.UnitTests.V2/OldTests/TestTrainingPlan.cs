using System;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using NUnit.Framework;
using SetType = BodyArchitect.Service.V2.Model.SetType;
using TrainingPlan = BodyArchitect.Service.V2.Model.TrainingPlans.TrainingPlan;
using TrainingPlanDay = BodyArchitect.Service.V2.Model.TrainingPlans.TrainingPlanDay;
using TrainingPlanEntry = BodyArchitect.Service.V2.Model.TrainingPlans.TrainingPlanEntry;
using TrainingPlanSerie = BodyArchitect.Service.V2.Model.TrainingPlans.TrainingPlanSerie;
using TrainingType = BodyArchitect.Service.V2.Model.TrainingPlans.TrainingType;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestTrainingPlan
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            ObjectsConverter.Configure();
        }
        [Test]
        public void Test()
        {
            Exercise exercise = new Exercise(Guid.NewGuid());
            exercise.Name = "exercise 1";
            exercise.Shortcut = "E1";
            Exercise exercise2 = new Exercise(Guid.NewGuid());
            exercise2.Name = "exercise 2";
            exercise2.Shortcut = "E2";
            Exercise exercise3 = new Exercise(Guid.NewGuid());
            exercise3.Name = "exercise 3";
            exercise3.Shortcut = "E3";
            Exercise exercise4 = new Exercise(Guid.NewGuid());
            exercise4.Name = "exercise 4";
            exercise4.Shortcut = "E4";
            TrainingPlan plan = new TrainingPlan();
            TrainingPlanDay day1= new TrainingPlanDay();
            plan.AddDay(day1);
            TrainingPlanEntry entry = new TrainingPlanEntry();
            entry.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.Sets.Add(new TrainingPlanSerie(12));
            entry.Sets.Add(new TrainingPlanSerie(10));
            entry.Sets.Add(new TrainingPlanSerie(8));
            day1.Entries.Add(entry);

            entry = new TrainingPlanEntry();
            entry.Exercise = exercise2.Map<ExerciseLightDTO>();
            entry.Sets.Add(new TrainingPlanSerie(10));
            entry.Sets.Add(new TrainingPlanSerie(10));
            entry.Sets.Add(new TrainingPlanSerie(8));
            entry.Sets.Add(new TrainingPlanSerie(6));
            day1.Entries.Add(entry);

            TrainingPlanDay day2 = new TrainingPlanDay();
            plan.AddDay(day2);
            TrainingPlanEntry entry3 = new TrainingPlanEntry();
            day2.Entries.Add(entry3);
            entry3.Exercise = exercise3.Map<ExerciseLightDTO>();
            entry3.Sets.Add(new TrainingPlanSerie(10));
            entry3.Sets.Add(new TrainingPlanSerie(8));
        }

        [Test]
        public void TestFBW_15_10_5Plan()
        {
            Exercise exercise = new Exercise(Guid.NewGuid());
            exercise.Name = "exercise 2";
            exercise.Shortcut = "E2";

            TrainingPlan plan = new TrainingPlan();
            plan.Comment = "15-10-5";
            plan.TrainingType = TrainingType.FBW;
            TrainingPlanDay dayA = new TrainingPlanDay();
            plan.AddDay(dayA);
            TrainingPlanEntry entry = new TrainingPlanEntry();
            entry.Exercise = exercise.Map<ExerciseLightDTO>();
            dayA.Entries.Add(entry);

            TrainingPlanDay dayB = new TrainingPlanDay();
            plan.AddDay(dayB);
            TrainingPlanEntry entry1= new TrainingPlanEntry();
            entry1.Exercise = exercise.Map<ExerciseLightDTO>();
            dayA.Entries.Add(entry1);

        }

    }
}
