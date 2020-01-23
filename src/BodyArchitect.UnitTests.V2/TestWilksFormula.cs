using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Portable;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Services;
using NUnit.Framework;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestWilksFormula
    {
        #region For men

        [Test]
        public void Men1_FromTables()
        {
            decimal bodyWeight = 100;
            decimal liftedWeight = 340;
            Assert.AreEqual(206.924, WilksFormula.CalculateForMenUsingTables(bodyWeight, liftedWeight));
        }

        [Test]
        public void Men2_FromTables()
        {
            decimal bodyWeight = 76.5m;
            decimal liftedWeight = 245.5m;
            Assert.AreEqual(172.56195, WilksFormula.CalculateForMenUsingTables(bodyWeight, liftedWeight));
            
        }

        [Test]
        public void Men3_LowerThan40()
        {
            decimal bodyWeight = 32m;
            decimal liftedWeight = 134.5m;
            Assert.AreEqual(247.693m,Math.Round(WilksFormula.CalculateForMen(bodyWeight, liftedWeight),4));
        }

        [Test]
        public void Men4_MoreThan206()
        {
            decimal bodyWeight = 241m;
            decimal liftedWeight = 200.5m;
            Assert.AreEqual(121.4661m, Math.Round(WilksFormula.CalculateForMen(bodyWeight, liftedWeight),4));
        }
        #endregion

        #region For women

        [Test]
        public void Women1_FromTables()
        {
            decimal bodyWeight = 60;
            decimal liftedWeight = 70;
            Assert.AreEqual(78.043, WilksFormula.CalculateForWomenUsingTables(bodyWeight, liftedWeight));

        }

        [Test]
        public void Women2_FromTables()
        {
            decimal bodyWeight = 100;
            decimal liftedWeight = 340;
            Assert.AreEqual(283.084, WilksFormula.CalculateForWomenUsingTables(bodyWeight, liftedWeight));

        }

        [Test]
        public void Women3_LowerThan40()
        {
            decimal bodyWeight = 32m;
            decimal liftedWeight = 134.5m;
            Assert.AreEqual(220.4120m, Math.Round(WilksFormula.CalculateForWomen(bodyWeight, liftedWeight), 4));
        }

        [Test]
        public void Women4_MoreThan206()
        {
            decimal bodyWeight = 170m;
            decimal liftedWeight = 200.5m;
            Assert.AreEqual(158.9952m, Math.Round(WilksFormula.CalculateForWomen(bodyWeight, liftedWeight), 4));
        }
        #endregion

        #region Calories calculator

        [Test]
        public void Calculator1()
        {
            var calories=WilksFormula.CalculateCalories(90, TimeSpan.FromMinutes(70), 3.62M);
            Assert.AreEqual(837M, calories);
        }


        [Test]
        public void CalculatorUsingMet_Male()
        {
            var calories = WilksFormula.CalculateCaloriesUsingMET(true, 8.0m, TimeSpan.FromMinutes(60), 32, 93, 188);
            Assert.AreEqual(689m, calories);
        }

        [Test]
        public void CalculatorUsingMet_Female()
        {
            var calories = WilksFormula.CalculateCaloriesUsingMET(false, 8.0m, TimeSpan.FromMinutes(60), 32, 93, 188);
            Assert.AreEqual(581m, (int)calories);
        }
        #endregion

        #region Pace calculator

        [Test]
        public void PaceFromDistanceAndTime()
        {
            var pace = TimeSpan.FromMinutes(50).TotalSeconds / 35;
            var paceTimeString = WilksFormula.PaceToString((float) pace);
            Assert.AreEqual("00:01:25",paceTimeString);
        }

        [Test]
        public void PaceFromSpeed()
        {
            var speed =11.66666666666667;//m/s
            var pace = WilksFormula.SpeedToPace((float)speed);
            Assert.AreEqual("00:01:25", WilksFormula.PaceToString(pace));
        }

        [Test]
        public void PaceFromSpeed_ShowWithoutHours()
        {
            var speed = 11.66666666666667;//m/s
            var pace = WilksFormula.SpeedToPace((float)speed);
            Assert.AreEqual("01:25", WilksFormula.PaceToString(pace,true));
        }

        [Test]
        public void SpeedToPace()
        {
            var speed = 12;//m/s
            var pace = WilksFormula.SpeedToPace((float)speed);
            Assert.AreEqual("01:23", WilksFormula.PaceToString(pace, true)); //min/km
            pace = WilksFormula.SpeedToPace((float)speed,true);
            Assert.AreEqual("02:15", WilksFormula.PaceToString(pace, true));//min/mile
        }

        #endregion
    }
}
