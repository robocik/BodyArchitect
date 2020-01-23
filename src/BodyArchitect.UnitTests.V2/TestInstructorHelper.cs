using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Module.Instructor;
using BodyArchitect.Service.V2.Model;
using NUnit.Framework;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestInstructorHelper
    {
        [Test]
        public void Less52Category1()
        {
            ChampionshipDTO dto = new ChampionshipDTO();
            var category = ModelHelper.GetWeightCategory(dto.WeightMenCategories, 30);
            Assert.AreEqual(52, category);
        }

        [Test]
        public void Less52Category2_Limit()
        {
            ChampionshipDTO dto = new ChampionshipDTO();
            var category = ModelHelper.GetWeightCategory(dto.WeightMenCategories, 52);
            Assert.AreEqual(52, category);
        }

        [Test]
        public void Over52Category2_Limit()
        {
            ChampionshipDTO dto = new ChampionshipDTO();
            var category = ModelHelper.GetWeightCategory(dto.WeightMenCategories, 52.01m);
            Assert.AreEqual(56, category);
        }

        [Test]
        public void Over125Category1()
        {
            ChampionshipDTO dto = new ChampionshipDTO();
            var category = ModelHelper.GetWeightCategory(dto.WeightMenCategories, 200);
            Assert.AreEqual(decimal.MaxValue, category);
        }

        [Test]
        public void Over125Category_Limit()
        {
            ChampionshipDTO dto = new ChampionshipDTO();
            var category = ModelHelper.GetWeightCategory(dto.WeightMenCategories, 125.01m);
            Assert.AreEqual(decimal.MaxValue, category);
        }

        [Test]
        public void Highest125Category_Limit()
        {
            ChampionshipDTO dto = new ChampionshipDTO();
            var category = ModelHelper.GetWeightCategory(dto.WeightMenCategories, 125.0m);
            Assert.AreEqual(125, category);
        }

        [Test]
        public void MiddleCategory1()
        {
            ChampionshipDTO dto = new ChampionshipDTO();
            var category=ModelHelper.GetWeightCategory(dto.WeightMenCategories, 70m);
            Assert.AreEqual(75,category);
        }

        [Test]
        public void MiddleCategory2_UpperLimit()
        {
            ChampionshipDTO dto = new ChampionshipDTO();
            var category = ModelHelper.GetWeightCategory(dto.WeightMenCategories, 90);
            Assert.AreEqual(90, category);
        }

        [Test]
        public void MiddleCategory2_LowerLimit()
        {
            ChampionshipDTO dto = new ChampionshipDTO();
            var category = ModelHelper.GetWeightCategory(dto.WeightMenCategories, 90.01m);
            Assert.AreEqual(100, category);
        }
    }
}
