using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Payments;
using NUnit.Framework;

namespace BodyArchitect.UnitTests.V2.BAPointsTests
{
    [TestFixture]
    public class TestPaymentsManager
    {
        [Test]
        public void GetPoints()
        {
            var xmlStream = DateTimeExtension.GetResource("BodyArchitect.UnitTests.V2.BAPointsTests.BAPoints.xml");
            PaymentsManager manager1 = new PaymentsManager();
            var manager=manager1.Load(xmlStream);
            Assert.AreEqual(1,manager.GetPoints(AccountType.PremiumUser).Points);
            Assert.IsNull( manager.GetPoints(AccountType.PremiumUser).PromotionPoints);
            Assert.IsNull(manager.GetPoints(AccountType.PremiumUser).PromotionStartDate);

            Assert.AreEqual(2, manager.GetPoints(AccountType.Instructor).Points);
            Assert.IsNull(manager.GetPoints(AccountType.Instructor).PromotionPoints);
            Assert.IsNull(manager.GetPoints(AccountType.Instructor).PromotionStartDate);

            Assert.AreEqual(0, manager.GetPoints(AccountType.User).Points);
            Assert.IsNull(manager.GetPoints(AccountType.User).PromotionPoints);
            Assert.IsNull(manager.GetPoints(AccountType.User).PromotionStartDate);

            Assert.AreEqual(0, manager.GetPoints(AccountType.Administrator).Points);
            Assert.IsNull(manager.GetPoints(AccountType.Administrator).PromotionPoints);
            Assert.IsNull(manager.GetPoints(AccountType.Administrator).PromotionStartDate);

        }

        [Test]
        public void GetPointsForOffert()
        {
            var xmlStream = DateTimeExtension.GetResource("BodyArchitect.UnitTests.V2.BAPointsTests.BAPoints.xml");
            PaymentsManager manager1 = new PaymentsManager();
            var manager=manager1.Load(xmlStream);
            Assert.AreEqual(30, manager.GetPointsForOffert("BAPoints_30"));
            Assert.AreEqual(0, manager.GetPointsForOffert("NotExistingOffert"));
        }
    }
}
