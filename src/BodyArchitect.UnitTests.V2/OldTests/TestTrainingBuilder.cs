using System;

using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using NUnit.Framework;


namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestTrainingBuilder
    {
        TrainingPlan getTrainingPlanWithSets()
        {
            TrainingPlan fbw = new TrainingPlan();
            fbw.GlobalId = Guid.NewGuid();
            fbw.Name = "Test training plan";
            fbw.TrainingType = TrainingType.FBW;
            TrainingPlanDay planA = new TrainingPlanDay();
            planA.Name = "Plan A";
            planA.GlobalId = Guid.NewGuid();
            fbw.Comment = @"15: 60%CR*15/ 80%CR*15/ 100%CR*12-15p

10: 60%CR*10/ 80%CR*10/ 90%CR*3-5/ 100%CR*7-10p

5: 60%CR*5/ 70%CR*2-5/ 80%CR*5/ 90%CR*1-2/ 100%CR*2-5powt 
";
            fbw.AddDay(planA);
            TrainingPlanEntry entry1 = new TrainingPlanEntry();
            entry1.Exercise = new ExerciseLightDTO() { GlobalId = new Guid("F0AB1656-B94D-4665-9AC9-F02F100F6E8C") };
            entry1.GlobalId = Guid.NewGuid();
            entry1.Sets.Add(new TrainingPlanSerie(12));
            entry1.Sets.Add(new TrainingPlanSerie(10));
            entry1.Sets.Add(new TrainingPlanSerie(8));
            planA.AddEntry(entry1);

            entry1 = new TrainingPlanEntry();
            entry1.Exercise = new ExerciseLightDTO() { GlobalId = new Guid("3E06A130-B811-4E45-9285-F087403615BF") };
            entry1.GlobalId = Guid.NewGuid();
            entry1.Sets.Add(new TrainingPlanSerie(10));
            planA.AddEntry(entry1);

            entry1 = new TrainingPlanEntry();
            entry1.Exercise = new ExerciseLightDTO() { GlobalId = new Guid("AF9624D9-1051-4FAA-88BE-063099412021") };
            entry1.GlobalId = Guid.NewGuid();
            entry1.Sets.Add(new TrainingPlanSerie(12));
            entry1.Sets.Add(new TrainingPlanSerie(12));
            entry1.Sets.Add(new TrainingPlanSerie(10));
            entry1.Sets.Add(new TrainingPlanSerie(10));
            planA.AddEntry(entry1);

            entry1 = new TrainingPlanEntry();
            entry1.Exercise = new ExerciseLightDTO() { GlobalId = new Guid("C9219C4B-D3F3-4846-80B4-D1A88C60C236") };
            entry1.GlobalId = Guid.NewGuid();
            planA.AddEntry(entry1);

            entry1 = new TrainingPlanEntry();
            entry1.Exercise = new ExerciseLightDTO() { GlobalId = new Guid("8FDFEC0A-98DF-4542-8167-CA35D3586370") };
            entry1.GlobalId = Guid.NewGuid();
            planA.AddEntry(entry1);

            TrainingPlanDay planB = new TrainingPlanDay();
            planB.GlobalId = Guid.NewGuid();
            planB.Name = "Plan B";

            entry1 = new TrainingPlanEntry();
            entry1.Exercise = new ExerciseLightDTO() { GlobalId = new Guid("8FDFEC0A-98DF-4542-8167-CA35D3586370") };
            entry1.GlobalId = Guid.NewGuid();
            planB.AddEntry(entry1);
            fbw.AddDay(planB);
            return fbw;
        }

        TrainingPlan getTrainingPlanCardioWithSets()
        {
            TrainingPlan fbw = new TrainingPlan();
            fbw.GlobalId = Guid.NewGuid();
            fbw.Name = "Test training plan";
            fbw.TrainingType = TrainingType.FBW;
            TrainingPlanDay planA = new TrainingPlanDay();
            planA.Name = "Plan A";
            planA.GlobalId = Guid.NewGuid();
            fbw.Comment = @"15: 60%CR*15/ 80%CR*15/ 100%CR*12-15p

10: 60%CR*10/ 80%CR*10/ 90%CR*3-5/ 100%CR*7-10p

5: 60%CR*5/ 70%CR*2-5/ 80%CR*5/ 90%CR*1-2/ 100%CR*2-5powt 
";
            fbw.AddDay(planA);
            TrainingPlanEntry entry1 = new TrainingPlanEntry();
            entry1.Exercise = new ExerciseLightDTO() { GlobalId = new Guid("F0AB1656-B94D-4665-9AC9-F02F100F6E8C"),ExerciseType = ExerciseType.Cardio};
            entry1.GlobalId = Guid.NewGuid();
            entry1.Sets.Add(new TrainingPlanSerie(12,0));
            entry1.Sets.Add(new TrainingPlanSerie(10));
            entry1.Sets.Add(new TrainingPlanSerie(8));
            planA.AddEntry(entry1);
            return fbw;
        }

        [Test]
        public void TestFillEmptyEntryObjectWithoutRepetitions()
        {
            TrainingPlan plan = getTrainingPlanWithSets();
            var entry = new StrengthTrainingEntryDTO();
            TrainingBuilder builder = new TrainingBuilder();
            Assert.AreEqual(0, entry.Entries.Count);
            Assert.AreEqual(null, entry.TrainingPlanItemId);
            builder.FillTrainingFromPlan(plan.Days[0],entry);
            Assert.AreEqual(plan.Days[0].Entries.Count, entry.Entries.Count);
            Assert.AreEqual(plan.Days[0].GlobalId, entry.TrainingPlanItemId);
            for (int i = 0; i < plan.Days[0].Entries.Count; i++)
            {
                Assert.AreEqual(plan.Days[0].Entries[i].Exercise ,entry.Entries[i].Exercise);
                Assert.AreEqual(plan.Days[0].Entries[i].Sets.Count, entry.Entries[i].Series.Count);
                for (int j = 0; j < plan.Days[0].Entries[i].Sets.Count; j++)
                {
                    Assert.AreEqual(null, entry.Entries[i].Series[j].RepetitionNumber);
                }
            }
        }

        [Test]
        public void TestFillEmptyEntryObjectWithRepetitions()
        {
            TrainingPlan plan = getTrainingPlanWithSets();
            var entry = new StrengthTrainingEntryDTO();
            TrainingBuilder builder = new TrainingBuilder();
            Assert.AreEqual(0, entry.Entries.Count);
            Assert.AreEqual(null, entry.TrainingPlanItemId);
            builder.FillRepetitionNumber = true;
            builder.FillTrainingFromPlan(plan.Days[0], entry);
            Assert.AreEqual(plan.Days[0].Entries.Count, entry.Entries.Count);
            Assert.AreEqual(plan.Days[0].GlobalId, entry.TrainingPlanItemId);
            for (int i = 0; i < plan.Days[0].Entries.Count; i++)
            {
                Assert.AreEqual(plan.Days[0].Entries[i].Exercise, entry.Entries[i].Exercise);
                Assert.AreEqual(plan.Days[0].Entries[i].Sets.Count, entry.Entries[i].Series.Count);
                for (int j = 0; j < plan.Days[0].Entries[i].Sets.Count; j++)
                {
                    Assert.AreEqual(plan.Days[0].Entries[i].Sets[j].RepetitionNumberMin, entry.Entries[i].Series[j].RepetitionNumber);
                }
            }
        }

        [Test]
        public void TestFillEmptyEntryObjectWithRepetitions_Cardio()
        {
            TrainingPlan plan = getTrainingPlanCardioWithSets();
            var entry = new StrengthTrainingEntryDTO();
            TrainingBuilder builder = new TrainingBuilder();
            Assert.AreEqual(0, entry.Entries.Count);
            Assert.AreEqual(null, entry.TrainingPlanItemId);
            builder.FillRepetitionNumber = true;
            builder.FillTrainingFromPlan(plan.Days[0], entry);
            Assert.AreEqual(plan.Days[0].Entries.Count, entry.Entries.Count);
            Assert.AreEqual(plan.Days[0].GlobalId, entry.TrainingPlanItemId);
            for (int i = 0; i < plan.Days[0].Entries.Count; i++)
            {
                Assert.AreEqual(plan.Days[0].Entries[i].Exercise, entry.Entries[i].Exercise);
                Assert.AreEqual(plan.Days[0].Entries[i].Sets.Count, entry.Entries[i].Series.Count);
                for (int j = 0; j < plan.Days[0].Entries[i].Sets.Count; j++)
                {
                    Assert.AreEqual(plan.Days[0].Entries[i].Sets[j].RepetitionNumberMin, entry.Entries[i].Series[j].Weight);
                }
            }
        }
    }
}
