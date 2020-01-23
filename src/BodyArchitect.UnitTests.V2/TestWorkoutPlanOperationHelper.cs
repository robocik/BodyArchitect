using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Module.StrengthTraining.Controls;
using BodyArchitect.Service.V2.Model;
using NUnit.Framework;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestWorkoutPlanOperationHelper
    {
        [Test]
        public void SetRestTime_SetsWithoutTimes()
        {
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            var set1 = new SerieDTO("2x4");
            item.AddSerie(set1);
            var set2 = new SerieDTO("12x3");
            item.AddSerie(set2);
            var set3 = new SerieDTO("12x43");
            item.AddSerie(set3);

            item.SetRestTime(TimeSpan.FromSeconds(30));

            Assert.AreEqual(30, (int)(set2.StartTime.Value - set1.EndTime.Value).TotalSeconds);
            Assert.AreEqual(30, (int)(set3.StartTime.Value - set2.EndTime.Value).TotalSeconds);
        }

        [Test]
        public void SetRestTime_AllSetsWithTimes()
        {
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            var set1 = new SerieDTO("2x4");
            set1.StartTime = DateTime.Today.AddDays(-2).AddMinutes(10);
            set1.EndTime = DateTime.Today.AddDays(-2).AddMinutes(20);
            item.AddSerie(set1);
            var set2 = new SerieDTO("12x3");
            set2.StartTime = DateTime.Today.AddDays(-2).AddMinutes(40);
            set2.EndTime = DateTime.Today.AddDays(-2).AddMinutes(60);
            item.AddSerie(set2);
            var set3 = new SerieDTO("12x43");
            set3.StartTime = DateTime.Today.AddDays(-2).AddMinutes(30);
            set3.EndTime = DateTime.Today.AddDays(-2).AddMinutes(70);
            item.AddSerie(set3);

            item.SetRestTime(TimeSpan.FromSeconds(30));

            Assert.AreEqual(30, (int)(set2.StartTime.Value - set1.EndTime.Value).TotalSeconds);
            Assert.AreEqual(30, (int)(set3.StartTime.Value - set2.EndTime.Value).TotalSeconds);

            Assert.AreEqual(10, (int)(set1.EndTime.Value - set1.StartTime.Value).TotalMinutes);
            Assert.AreEqual(20, (int)(set2.EndTime.Value - set2.StartTime.Value).TotalMinutes);
            Assert.AreEqual(40, (int)(set3.EndTime.Value - set3.StartTime.Value).TotalMinutes);

        }

        [Test]
        public void SetRestTime_OneSetWithTimes()
        {
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            var set1 = new SerieDTO("2x4");
            item.AddSerie(set1);
            var set2 = new SerieDTO("12x3");
            set2.StartTime = DateTime.Today.AddDays(-2).AddMinutes(40);
            set2.EndTime = DateTime.Today.AddDays(-2).AddMinutes(60);
            item.AddSerie(set2);
            var set3 = new SerieDTO("12x43");
            item.AddSerie(set3);

            item.SetRestTime(TimeSpan.FromSeconds(30));

            Assert.AreEqual(30, (int)(set2.StartTime.Value - set1.EndTime.Value).TotalSeconds);
            Assert.AreEqual(30, (int)(set3.StartTime.Value - set2.EndTime.Value).TotalSeconds);

            Assert.AreEqual(0 ,(int)(set1.EndTime.Value - set1.StartTime.Value).TotalMinutes);
            Assert.AreEqual(20, (int)(set2.EndTime.Value - set2.StartTime.Value).TotalMinutes);
            Assert.AreEqual(0, (int)(set3.EndTime.Value - set3.StartTime.Value).TotalMinutes);

        }
    }
}
