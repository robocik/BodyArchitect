using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Security;
using BodyArchitect.Client.Common;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Payments;
using BodyArchitect.Shared;
using NUnit.Framework;

namespace BodyArchitect.UnitTests.V2.BAPointsTests
{
    /*
     p24_id_sprzedawcy=17035&
     p24_session_id=ca89bef1-db08-4cbf-ab44-fa15e6414a81&
     p24_order_id=191015&
     p24_order_id_full=18191015&
     p24_kwota=500&
     p24_crc=ceae1c9335d29498cdc870230079b0cd&
     p24_karta=0&
     */
    public class TestablePrzelewy24Handler : Przelewy24Handler
    {

        public TestablePrzelewy24Handler(PaymentsHolder manager)
            : base(manager)
        {
            Response = @"RESULT
TRUE";
        }

        public string Response { get; set; }

        protected override string SendPayPalRequest(System.Web.HttpContext request)
        {
            return Response;
        }

    }

    [TestFixture]
    public class TestPrzelewy24Handler : NHibernateTestFixtureBase
    {
        private Profile profile;
        private PaymentsHolder manager;

        public override void createTestFixture()
        {
            base.createTestFixture();

            var xmlStream = DateTimeExtension.GetResource("BodyArchitect.UnitTests.V2.BAPointsTests.BAPoints.xml");
            var manager1 = new PaymentsManager();
            manager = manager1.Load(xmlStream);

        }
        public override void setupTest()
        {

            base.setupTest();

            profile = new Profile();
            profile.UserName = "test";
            profile.Email = "gfdgdfg@wp.pl";
            insertToDatabase(profile);
        }

        [Test]
        public void CorrectOrder_30()
        {
            var handler = new TestablePrzelewy24Handler(manager);

            NameValueCollection requestData = new NameValueCollection();
            requestData.Add("p24_id_sprzedawcy", handler.MyId);
            requestData.Add("p24_order_id", "191015");
            requestData.Add("p24_order_id_full", "18191015");
            requestData.Add("p24_kwota", "600");
            requestData.Add("p24_session_id", Guid.NewGuid()+"|"+profile.GlobalId.ToString()+"|BAPoints_30");
            requestData.Add("p24_crc", "fghgfhfrtyryr");
            requestData.Add("p24_karta", "0");
            

            var serviceSession = sessionFactory.OpenSession();
            using (serviceSession)
            {
                handler.ProcessOrderRequest(serviceSession, requestData, null);
            }
            Session.Clear();
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(30, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.MinValue, dbProfile.Licence.LastPointOperationDate);

            var baPoints = Session.QueryOver<BAPoints>().SingleOrDefault();
            Assert.AreEqual(30, baPoints.Points);
            Assert.AreEqual(dbProfile.GlobalId, baPoints.Profile.GlobalId);
            Assert.AreEqual(requestData["p24_order_id"], baPoints.Identifier);
            Assert.AreEqual(BAPointsType.Przelewy24, baPoints.Type);

        }

        [Test]
        public void CorrectOrder_120()
        {
            var handler = new TestablePrzelewy24Handler(manager);

            NameValueCollection requestData = new NameValueCollection();
            requestData.Add("p24_id_sprzedawcy", handler.MyId);
            requestData.Add("p24_order_id", "191015");
            requestData.Add("p24_order_id_full", "18191015");
            requestData.Add("p24_kwota", "9000");
            requestData.Add("p24_session_id", Guid.NewGuid() + "|" + profile.GlobalId.ToString() + "|BAPoints_120");
            requestData.Add("p24_crc", "fghgfhfrtyryr");
            requestData.Add("p24_karta", "0");


            var serviceSession = sessionFactory.OpenSession();
            using (serviceSession)
            {
                handler.ProcessOrderRequest(serviceSession, requestData, null);
            }
            Session.Clear();
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(360, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.MinValue, dbProfile.Licence.LastPointOperationDate);

            var baPoints = Session.QueryOver<BAPoints>().SingleOrDefault();
            Assert.AreEqual(360, baPoints.Points);
            Assert.AreEqual(dbProfile.GlobalId, baPoints.Profile.GlobalId);
            Assert.AreEqual(requestData["p24_order_id"], baPoints.Identifier);
            Assert.AreEqual(BAPointsType.Przelewy24, baPoints.Type);

        }

        [Test]
        public void InvalidSessionId()
        {
            var handler = new TestablePrzelewy24Handler(manager);

            NameValueCollection requestData = new NameValueCollection();
            requestData.Add("p24_id_sprzedawcy", handler.MyId);
            requestData.Add("p24_order_id", "191015");
            requestData.Add("p24_order_id_full", "18191015");
            requestData.Add("p24_kwota", "9000");
            requestData.Add("p24_session_id", Guid.NewGuid() + "|" + profile.GlobalId.ToString());
            requestData.Add("p24_crc", "fghgfhfrtyryr");
            requestData.Add("p24_karta", "0");


            var serviceSession = sessionFactory.OpenSession();
            using (serviceSession)
            {
                try
                {
                    handler.ProcessOrderRequest(serviceSession, requestData, null);
                    Assert.Fail();
                }
                catch (IndexOutOfRangeException)
                {
                }
            }
            Session.Clear();
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(0, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.MinValue, dbProfile.Licence.LastPointOperationDate);

            var baPoints = Session.QueryOver<BAPoints>().RowCount();
            Assert.AreEqual(0, baPoints);

        }

        [Test]
        public void InvalidOrder_BuyNow()
        {
            var handler = new TestablePrzelewy24Handler(manager);

            NameValueCollection requestData = new NameValueCollection();
            requestData.Add("p24_id_sprzedawcy", handler.MyId);
            requestData.Add("p24_order_id", "191015");
            requestData.Add("p24_order_id_full", "18191015");
            requestData.Add("p24_kwota", "500");
            requestData.Add("p24_session_id", Guid.NewGuid() + "|" + profile.GlobalId.ToString() + "|BAPoints_120");
            requestData.Add("p24_crc", "fghgfhfrtyryr");
            requestData.Add("p24_karta", "0");
            handler.Response = "INVALID";
            var serviceSession = sessionFactory.OpenSession();
            using (serviceSession)
            {
                try
                {
                    handler.ProcessOrderRequest(serviceSession, requestData, null);
                    Assert.Fail();
                }
                catch (InvalidDataException)
                {
                }

            }
            Session.Clear();
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(0, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.MinValue, dbProfile.Licence.LastPointOperationDate);

            var baPoints = Session.QueryOver<BAPoints>().RowCount();
            Assert.AreEqual(0, baPoints);

        }

        [Test]
        public void DuplicatedOrder()
        {
            BAPoints point = new BAPoints();
            point.Identifier = "191015";
            point.Profile = profile;
            point.ImportedDate = DateTime.UtcNow;
            insertToDatabase(point);

            var handler = new TestablePrzelewy24Handler(manager);

            NameValueCollection requestData = new NameValueCollection();
            requestData.Add("p24_id_sprzedawcy", handler.MyId);
            requestData.Add("p24_order_id", "191015");
            requestData.Add("p24_order_id_full", "18191015");
            requestData.Add("p24_kwota", "500");
            requestData.Add("p24_session_id", Guid.NewGuid() + "|" + profile.GlobalId.ToString() + "|BAPoints_120");
            requestData.Add("p24_crc", "fghgfhfrtyryr");
            requestData.Add("p24_karta", "0");
            var serviceSession = sessionFactory.OpenSession();
            using (serviceSession)
            {
                try
                {
                    handler.ProcessOrderRequest(serviceSession, requestData, null);
                    Assert.Fail();
                }
                catch (UniqueException)
                {
                }

            }
            Session.Clear();
            var baPoints = Session.QueryOver<BAPoints>().RowCount();
            Assert.AreEqual(1, baPoints);

        }
    }
}
