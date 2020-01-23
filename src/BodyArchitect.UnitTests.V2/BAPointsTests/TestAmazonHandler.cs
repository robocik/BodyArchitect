using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Service.V2.Payments;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NUnit.Framework;
using BodyArchitect.Client.Common;
using BodyArchitect.Model;
using System.Collections.Specialized;
using System.IO;

namespace BodyArchitect.UnitTests.V2.BAPointsTests
{
    public class TestableAmazonHandler : AmazonHandler
    {
        private string receiverEmail;

        public TestableAmazonHandler(PaymentsHolder manager, string receiverEmail)
            : base(manager)
        {
            this.receiverEmail = receiverEmail;
        }

        public bool AmazonResponse { get; set; }

        protected override bool SendAmazonRequest(System.Web.HttpRequest request)
        {
            return AmazonResponse;
        }

        public override string ReceiverEmail
        {
            get { return receiverEmail; }
        }
    }

    [TestFixture]
    public class TestAmazonHandler : NHibernateTestFixtureBase
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
        public void InvalidOrder_BuyNow()
        {
            NameValueCollection requestData = new NameValueCollection();
            requestData.Add("buyerEmail", "vader@deathstar.com");
            requestData.Add("transactionId", "174IG322ASL4JBRAQQHNDPM3H2FTMFLNQ8I");
            requestData.Add("status", "PS");
            requestData.Add("transactionAmount", "USD 5.000000");
            requestData.Add("paymentReason", "BAPoints_30");
            requestData.Add("operation", "pay");
            requestData.Add("referenceId", profile.GlobalId.ToString());
            requestData.Add("signatureVersion", "2");
            requestData.Add("certificateUrl", "https://fps.sandbox.amazonaws.com/certs/090911/PKICert.pem%3frequestId%3d15n8qoapsoddqfe380kvbpmb4h0cy47zhl9n1f1tv07aofo");
            requestData.Add("paymentMethod", "CC");
            requestData.Add("signatureMethod", "RSA-SHA1");
            requestData.Add("recipientEmail", "shik@gmail.com");
            requestData.Add("transactionDate", "1344945621");
            requestData.Add("buyerName", "Darth Vader");
            requestData.Add("signature", "pHaRbT6UjbHEykNhatobDr8TVVTRdrAM2Xr3mRzTgEbeWk8flESxznXO4EfDDiM5b0u0YLeJoJTj%0ar6hihivc1bZqMZ6XuBTS/qWK/aO6KHzGUVtAEwWt1mCtLaJBdxlJrrO86F/GkYmg9mBwQw9847gl%0abyDetj1ntIa%2bHS8RN4M%3d");

            var handler = new TestableAmazonHandler(manager, "receiver@mail.com");
            handler.AmazonResponse = false;
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
        public void PendingOrder_BuyNow()
        {
            NameValueCollection requestData = new NameValueCollection();
            requestData.Add("buyerEmail", "vader@deathstar.com");
            requestData.Add("transactionId", "174IG322ASL4JBRAQQHNDPM3H2FTMFLNQ8I");
            requestData.Add("status", "PI");        //Payment Initiated, it will take 5s - 48h to complete
            requestData.Add("transactionAmount", "USD 5.000000");
            requestData.Add("paymentReason", "BAPoints_30");
            requestData.Add("operation", "pay");
            requestData.Add("referenceId", profile.GlobalId.ToString());
            requestData.Add("signatureVersion", "2");
            requestData.Add("certificateUrl", "https://fps.sandbox.amazonaws.com/certs/090911/PKICert.pem%3frequestId%3d15n8qoapsoddqfe380kvbpmb4h0cy47zhl9n1f1tv07aofo");
            requestData.Add("paymentMethod", "CC");
            requestData.Add("signatureMethod", "RSA-SHA1");
            requestData.Add("recipientEmail", "shik@gmail.com");
            requestData.Add("transactionDate", "1344945621");
            requestData.Add("buyerName", "Darth Vader");
            requestData.Add("signature", "pHaRbT6UjbHEykNhatobDr8TVVTRdrAM2Xr3mRzTgEbeWk8flESxznXO4EfDDiM5b0u0YLeJoJTj%0ar6hihivc1bZqMZ6XuBTS/qWK/aO6KHzGUVtAEwWt1mCtLaJBdxlJrrO86F/GkYmg9mBwQw9847gl%0abyDetj1ntIa%2bHS8RN4M%3d");


            TestableAmazonHandler handler = new TestableAmazonHandler(manager, "receiver@mail.com");
            handler.AmazonResponse = true;
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
            Assert.AreEqual(0, baPoints);

        }

        [Test]
        public void CompletedOrder_BuyNow()
        {
            NameValueCollection requestData = new NameValueCollection();
            requestData.Add("buyerEmail", "vader@deathstar.com");
            requestData.Add("transactionId", "174IG322ASL4JBRAQQHNDPM3H2FTMFLNQ8I");
            requestData.Add("status", "PS");
            requestData.Add("transactionAmount", "USD 5.000000");
            requestData.Add("paymentReason", "BAPoints_30");
            requestData.Add("operation", "pay");
            requestData.Add("referenceId", profile.GlobalId.ToString());
            requestData.Add("signatureVersion", "2");
            requestData.Add("certificateUrl", "https://fps.sandbox.amazonaws.com/certs/090911/PKICert.pem%3frequestId%3d15n8qoapsoddqfe380kvbpmb4h0cy47zhl9n1f1tv07aofo");
            requestData.Add("paymentMethod", "CC");
            requestData.Add("signatureMethod", "RSA-SHA1");
            requestData.Add("recipientEmail", "shik@gmail.com");
            requestData.Add("transactionDate", "1344945621");
            requestData.Add("buyerName", "Darth Vader");
            requestData.Add("signature", "pHaRbT6UjbHEykNhatobDr8TVVTRdrAM2Xr3mRzTgEbeWk8flESxznXO4EfDDiM5b0u0YLeJoJTj%0ar6hihivc1bZqMZ6XuBTS/qWK/aO6KHzGUVtAEwWt1mCtLaJBdxlJrrO86F/GkYmg9mBwQw9847gl%0abyDetj1ntIa%2bHS8RN4M%3d");

            var handler = new TestableAmazonHandler(manager, "receiver@mail.com");
            handler.AmazonResponse = true;
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
            Assert.AreEqual(requestData["transactionId"], baPoints.Identifier);
            Assert.AreEqual(BAPointsType.PayPal, baPoints.Type);
        }

        [Test]
        public void DuplicatedOrder_BuyNow()
        {
            BAPoints point = new BAPoints();
            point.Identifier = "174IG322ASL4JBRAQQHNDPM3H2FTMFLNQ8I";
            point.Profile = profile;
            point.ImportedDate = DateTime.UtcNow;
            insertToDatabase(point);

            NameValueCollection requestData = new NameValueCollection();
            requestData.Add("buyerEmail", "vader@deathstar.com");
            requestData.Add("transactionId", "174IG322ASL4JBRAQQHNDPM3H2FTMFLNQ8I");
            requestData.Add("status", "PS");
            requestData.Add("transactionAmount", "USD 5.000000");
            requestData.Add("paymentReason", "BAPoints_30");
            requestData.Add("operation", "pay");
            requestData.Add("referenceId", profile.GlobalId.ToString());
            requestData.Add("signatureVersion", "2");
            requestData.Add("certificateUrl", "https://fps.sandbox.amazonaws.com/certs/090911/PKICert.pem%3frequestId%3d15n8qoapsoddqfe380kvbpmb4h0cy47zhl9n1f1tv07aofo");
            requestData.Add("paymentMethod", "CC");
            requestData.Add("signatureMethod", "RSA-SHA1");
            requestData.Add("recipientEmail", "shik@gmail.com");
            requestData.Add("transactionDate", "1344945621");
            requestData.Add("buyerName", "Darth Vader");
            requestData.Add("signature", "pHaRbT6UjbHEykNhatobDr8TVVTRdrAM2Xr3mRzTgEbeWk8flESxznXO4EfDDiM5b0u0YLeJoJTj%0ar6hihivc1bZqMZ6XuBTS/qWK/aO6KHzGUVtAEwWt1mCtLaJBdxlJrrO86F/GkYmg9mBwQw9847gl%0abyDetj1ntIa%2bHS8RN4M%3d");

            var handler = new TestableAmazonHandler(manager, "receiver@mail.com");
            handler.AmazonResponse = true;
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
