using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Model;
using BodyArchitect.Portable.Exceptions;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Payments;
using BodyArchitect.Shared;
using NUnit.Framework;

namespace BodyArchitect.UnitTests.V2.BAPointsTests
{
    /*mc_gross=5.00
protection_eligibility=Ineligible
payer_id=RN6GCSKGJTEX6
tax=0.00
payment_date=10%3a48%3a17+Jul+22%2c+2012+PDT
payment_status=Completed
charset=windows-1252
first_name=romek&mc_fee=0.50
notify_version=3.5
custom=a00beb3d-c515-4e99-aa7d-09601127e83
payer_status=verified
business=romanp_1342816399_biz%40poczta.onet.pl
quantity=1
verify_sign=Af8sHB-060pOLR-LG5.MoHIVj1-AjIq0yeXETUljXPcNuXgPnlYYpW0
payer_email=romanp_1342869042_per40poczta.onet.pl
txn_id=6AY89642EM656753M
payment_type=instant
btn_id=2557186
last_name=Podstawa
receiver_email=romanp_1342816399_biz40poczta.onet.pl
payment_fee=
shipping_discount=0.00
insurance_amount=0.00
receiver_id=TBGBLQFCAV8SS
txn_type=web_accept
item_name=Buy30+points
discount=0.00
mc_currency=EUR
item_number=BAPoints_30
residence_country=US
test_ipn=1
handling_amount=0.00
shipping_method=Default
transaction_subject=a00beb3d-c515-4e99-aa7d-09601127e83
payment_gross=
shipping=0.00
ipn_track_id=c695ab3dadd7a&*/
    public class TestablePayPalHandler:PayPalHandler
    {
        private string receiverEmail;

        public TestablePayPalHandler(PaymentsHolder manager, string receiverEmail)
            : base(manager)
        {
            this.receiverEmail = receiverEmail;
        }

        public string PayPalResponse { get; set; }

        protected override string SendPayPalRequest(System.Web.HttpRequest request)
        {
            return PayPalResponse;
        }

        public override string ReceiverEmail
        {
            get { return receiverEmail; }
        }
    }

    [TestFixture]
    public class TestPayPalHandler : NHibernateTestFixtureBase
    {
        private Profile profile;
        private PaymentsHolder manager;

        public override void createTestFixture()
        {
            base.createTestFixture();

            var xmlStream = DateTimeExtension.GetResource("BodyArchitect.UnitTests.V2.BAPointsTests.BAPoints.xml");
            var manager1 = new PaymentsManager();
            manager=manager1.Load(xmlStream);

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
        public void InvalidOrder_BuyNow()
        {
            NameValueCollection requestData = new NameValueCollection();
            requestData.Add("receiver_email", "receiver@mail.com");
            requestData.Add("txn_id", "6AY89642EM656753M");
            requestData.Add("payment_status", "Pending");
            requestData.Add("txn_type", "web_accept");
            requestData.Add("quantity", "1");
            requestData.Add("item_number", "BAPoints_30");
            requestData.Add("custom", profile.GlobalId.ToString());

            TestablePayPalHandler handler = new TestablePayPalHandler(manager,"receiver@mail.com");
            handler.PayPalResponse = "INVALID";
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
        public void WrongReceiverMail_BuyNow()
        {
            NameValueCollection requestData = new NameValueCollection();
            requestData.Add("receiver_email", "wrongMail@mail.com");
            requestData.Add("txn_id", "6AY89642EM656753M");
            requestData.Add("payment_status", "Pending");
            requestData.Add("txn_type", "web_accept");
            requestData.Add("quantity", "1");
            requestData.Add("item_number", "BAPoints_30");
            requestData.Add("custom", profile.GlobalId.ToString());

            TestablePayPalHandler handler = new TestablePayPalHandler(manager,"receiver@mail.com");
            handler.PayPalResponse = "VERIFIED";
            var serviceSession = sessionFactory.OpenSession();
            using (serviceSession)
            {
                try
                {
                    handler.ProcessOrderRequest(serviceSession, requestData, null);
                    Assert.Fail();
                }
                catch (ValidationException)
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
        public void PendingOrder_BuyNow()
        {
            NameValueCollection requestData = new NameValueCollection();
            requestData.Add("receiver_email", "receiver@mail.com");
            requestData.Add("txn_id", "6AY89642EM656753M");
            requestData.Add("payment_status", "Pending");
            requestData.Add("txn_type", "web_accept");
            requestData.Add("quantity", "1");
            requestData.Add("item_number", "BAPoints_30");
            requestData.Add("custom", profile.GlobalId.ToString());

            TestablePayPalHandler handler = new TestablePayPalHandler(manager,"receiver@mail.com");
            handler.PayPalResponse = "VERIFIED";
            var serviceSession = sessionFactory.OpenSession();
            using (serviceSession)
            {
                handler.ProcessOrderRequest(serviceSession, requestData, null);
            }
            Session.Clear();
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(0, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.MinValue, dbProfile.Licence.LastPointOperationDate);

            var baPoints = Session.QueryOver<BAPoints>().RowCount();
            Assert.AreEqual(0,baPoints);

        }

        [Test]
        public void CompletedOrder_BuyNow_BAPoints_120()
        {
            NameValueCollection requestData = new NameValueCollection();
            requestData.Add("receiver_email", "receiver@mail.com");
            requestData.Add("txn_id", "6AY89642EM656753M");
            requestData.Add("payment_status", "Completed");
            requestData.Add("txn_type", "web_accept");
            requestData.Add("quantity", "1");
            requestData.Add("item_number", "BAPoints_120");
            requestData.Add("custom", profile.GlobalId.ToString());

            TestablePayPalHandler handler = new TestablePayPalHandler(manager, "receiver@mail.com");
            handler.PayPalResponse = "VERIFIED";
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
            Assert.AreEqual(requestData["txn_id"], baPoints.Identifier);
            Assert.AreEqual(BAPointsType.PayPal, baPoints.Type);
        }

        [Test]
        public void CompletedOrder_BuyNow_BAPoints_30()
        {
            NameValueCollection requestData = new NameValueCollection();
            requestData.Add("receiver_email", "receiver@mail.com");
            requestData.Add("txn_id", "6AY89642EM656753M");
            requestData.Add("payment_status", "Completed");
            requestData.Add("txn_type", "web_accept");
            requestData.Add("quantity", "1");
            requestData.Add("item_number", "BAPoints_30");
            requestData.Add("custom", profile.GlobalId.ToString());

            TestablePayPalHandler handler = new TestablePayPalHandler(manager,"receiver@mail.com");
            handler.PayPalResponse = "VERIFIED";
            var serviceSession = sessionFactory.OpenSession();
            using (serviceSession)
            {
                handler.ProcessOrderRequest(serviceSession, requestData, null);
            }
            Session.Clear();
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(30,dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.MinValue,dbProfile.Licence.LastPointOperationDate);

            var baPoints=Session.QueryOver<BAPoints>().SingleOrDefault();
            Assert.AreEqual(30, baPoints.Points);
            Assert.AreEqual(dbProfile.GlobalId, baPoints.Profile.GlobalId);
            Assert.AreEqual(requestData["txn_id"], baPoints.Identifier);
            Assert.AreEqual(BAPointsType.PayPal, baPoints.Type);
        }

        [Test]
        public void DuplicatedOrder_BuyNow()
        {
            BAPoints point = new BAPoints();
            point.Identifier = "6AY89642EM656753M";
            point.Profile = profile;
            point.ImportedDate = DateTime.UtcNow;
            insertToDatabase(point);

            NameValueCollection requestData = new NameValueCollection();
            requestData.Add("receiver_email", "receiver@mail.com");
            requestData.Add("txn_id", "6AY89642EM656753M");
            requestData.Add("payment_status", "Completed");
            requestData.Add("txn_type", "web_accept");
            requestData.Add("quantity", "1");
            requestData.Add("item_number", "BAPoints_30");
            requestData.Add("custom", profile.GlobalId.ToString());

            TestablePayPalHandler handler = new TestablePayPalHandler(manager,"receiver@mail.com");
            handler.PayPalResponse = "VERIFIED";
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
