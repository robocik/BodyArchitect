using System;
using System.Collections.Generic;
using System.IO;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using NUnit.Framework;
using TrainingPlanSerie = BodyArchitect.Service.V2.Model.TrainingPlans.TrainingPlanSerie;


namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestModel
    {

        #region Serie

        [Test]
        public void TestSerieStringConstructor()
        {
            string serieString = "12x60";
            Serie serie = new Serie(serieString);
            Assert.AreEqual(12,serie.RepetitionNumber);
            Assert.AreEqual(60, serie.Weight);
        }

        [Test]
        public void TestChangeSerieValueToEmptyWeight()
        {
            string serieString = "12x60";
            Serie serie = new Serie(serieString);
            Assert.AreEqual(12, serie.RepetitionNumber);
            Assert.AreEqual(60, serie.Weight);
            serie.SetFromString("12x");
            Assert.AreEqual(12, serie.RepetitionNumber);
            Assert.AreEqual(null, serie.Weight);
        }

        [Test]
        public void TestChangeSerieValueToEmptyRepetition()
        {
            string serieString = "12x60";
            Serie serie = new Serie(serieString);
            Assert.AreEqual(12, serie.RepetitionNumber);
            Assert.AreEqual(60, serie.Weight);
            serie.SetFromString("x5");
            Assert.AreEqual(null, serie.RepetitionNumber);
            Assert.AreEqual(5, serie.Weight);
        }

        [Test]
        public void TestSetFromStringConstructor()
        {
            string serieString = "12x60";
            Serie serie = new Serie();
            serie.SetFromString(serieString);
            Assert.AreEqual(12, serie.RepetitionNumber);
            Assert.AreEqual(60, serie.Weight);
        }

        [Test]
        public void TestSetFromEmptyString()
        {
            string serieString = string.Empty;
            Serie serie = new Serie();
            serie.SetFromString(serieString);
            Assert.AreEqual(null, serie.RepetitionNumber);
            Assert.AreEqual(null, serie.Weight);
        }

        [Test]
        public void TestSetFromStringConstructor_WeightEmpty()
        {
            string serieString = "12x";
            Serie serie = new Serie();
            serie.SetFromString(serieString);
            Assert.AreEqual(12, serie.RepetitionNumber);
            Assert.IsNull(serie.Weight);
        }

        [Test]
        public void TestSetFromStringConstructor_RepetitionEmpty()
        {
            string serieString = "x60";
            Serie serie = new Serie();
            serie.SetFromString(serieString);
            Assert.AreEqual(60, serie.Weight);
            Assert.IsNull(serie.RepetitionNumber);
        }
        #endregion

        #region TrainingPlanSerie

        [Test]
        public void TestRepetitionRange_EmptyRange()
        {
            TrainingPlanSerie serie = new TrainingPlanSerie(10,12);
            serie.FromString(null);
            Assert.AreEqual(null, serie.RepetitionNumberMin);
            Assert.AreEqual(null, serie.RepetitionNumberMax);
        }

        [Test]
        public void TestRepetitionRange_MinMax()
        {
            TrainingPlanSerie serie = new TrainingPlanSerie();
            serie.FromString("10-12");
            Assert.AreEqual(10,serie.RepetitionNumberMin.Value);
            Assert.AreEqual(12, serie.RepetitionNumberMax.Value);
        }

        [Test]
        public void TestRepetitionRange_MinOnly()
        {
            TrainingPlanSerie serie = new TrainingPlanSerie();
            serie.FromString("10-");
            Assert.AreEqual(10, serie.RepetitionNumberMin.Value);
            Assert.IsNull(serie.RepetitionNumberMax);
        }

        [Test]
        public void TestRepetitionRange_MaxOnly()
        {
            TrainingPlanSerie serie = new TrainingPlanSerie();
            serie.FromString("-10");
            Assert.AreEqual(10, serie.RepetitionNumberMax.Value);
            Assert.IsNull(serie.RepetitionNumberMin);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestRepetitionRange_MinMax_Wrong()
        {
            TrainingPlanSerie serie = new TrainingPlanSerie();
            serie.FromString("12-10");//min is bigger than max
        }

        [Test]
        public void TestRepetitionRange_MinMaxEqual()
        {
            TrainingPlanSerie serie = new TrainingPlanSerie();
            serie.FromString("10-10");
            Assert.AreEqual(10, serie.RepetitionNumberMin.Value);
            Assert.AreEqual(10, serie.RepetitionNumberMax.Value);
        }

        [Test]
        public void TestRepetitionRange_MinMaxEqual_OneNumber()
        {
            TrainingPlanSerie serie = new TrainingPlanSerie();
            serie.FromString("10");
            Assert.AreEqual(10, serie.RepetitionNumberMin.Value);
            Assert.AreEqual(10, serie.RepetitionNumberMax.Value);
        }
        #endregion
    }
}
