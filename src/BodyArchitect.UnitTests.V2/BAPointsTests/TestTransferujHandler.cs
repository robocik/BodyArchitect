using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using BodyArchitect.Client.Common;
using BodyArchitect.Model;
using BodyArchitect.Portable.Exceptions;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Payments;
using BodyArchitect.Shared;
using NUnit.Framework;

namespace BodyArchitect.UnitTests.V2.BAPointsTests
{
    /*
     id=6025&
     tr_id=TR-USR-11APEX&
     tr_date=2012-08-21+19%3a37%3a50&
     tr_crc=1971bbd9-a4de-4a47-ae3d-a0b40072c210&
     tr_amount=5.00&tr_paid=5.00&
     tr_desc=Punkty+do+BodyArchitecta&
     tr_status=TRUE&
     tr_error=none&
     tr_email=dfg%40wp.pl&
     md5sum=ffce515c5a038434f6095af5b538bd82&*/
    public class TestableTranferujHandler : TransferujHandler
    {

        public TestableTranferujHandler(PaymentsHolder manager)
            : base(manager)
        {
            RequestIp = "195.149.229.109";
        }

        public bool SendConfirmationInvoked { get; set; }

        public string RequestIp { get; set; }

        protected override Dictionary<string, string> CreateAdditionalData(HttpContext context)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("UserHostAddress", RequestIp);
            return data;
        }
        protected override void SendConfirmation(HttpContext context)
        {
            SendConfirmationInvoked = true;
        }
    }

    [TestFixture]
    public class TestTransferujHandler : NHibernateTestFixtureBase
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
        public void CorrectOrder_BAPoints_30()
        {
            var handler = new TestableTranferujHandler(manager);

            NameValueCollection requestData = new NameValueCollection();
            requestData.Add("id", handler.TransferujId);
            requestData.Add("tr_id", "TR-USR-11APEX");
            requestData.Add("tr_date", "2012-08-21+19%3a37%3a50");
            requestData.Add("tr_crc",profile.GlobalId.ToString()+"|BAPoints_30");
            requestData.Add("tr_amount", "5.00");
            requestData.Add("tr_paid", "5.00");
            requestData.Add("tr_desc", "BAPoints_30");
            requestData.Add("tr_status", "TRUE");
            requestData.Add("tr_error", "none");
            requestData.Add("tr_email", "test@mail.com");
            var md5 =FormsAuthentication.HashPasswordForStoringInConfigFile(handler.TransferujId + requestData["tr_id"] + requestData["tr_amount"] + requestData["tr_crc"] + handler.TransferujKodPotwierdzajcy, "MD5");
            requestData.Add("md5sum", md5);
            
            var serviceSession = sessionFactory.OpenSession();
            using (serviceSession)
            {
                handler.ProcessOrderRequest(serviceSession, requestData, null);
            }
            Session.Clear();
            Assert.IsTrue(handler.SendConfirmationInvoked);
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(30, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.MinValue, dbProfile.Licence.LastPointOperationDate);

            var baPoints = Session.QueryOver<BAPoints>().SingleOrDefault();
            Assert.AreEqual(30, baPoints.Points);
            Assert.AreEqual(dbProfile.GlobalId, baPoints.Profile.GlobalId);
            Assert.AreEqual(requestData["tr_id"], baPoints.Identifier);
            Assert.AreEqual(BAPointsType.Transferuj, baPoints.Type);

        }

        [Test]
        public void CorrectOrder_BAPoints_120()
        {
            var handler = new TestableTranferujHandler(manager);

            NameValueCollection requestData = new NameValueCollection();
            requestData.Add("id", handler.TransferujId);
            requestData.Add("tr_id", "TR-USR-11APEX");
            requestData.Add("tr_date", "2012-08-21+19%3a37%3a50");
            requestData.Add("tr_crc", profile.GlobalId.ToString() + "|BAPoints_120");
            requestData.Add("tr_amount", "5.00");
            requestData.Add("tr_paid", "5.00");
            requestData.Add("tr_desc", "BAPoints_30");
            requestData.Add("tr_status", "TRUE");
            requestData.Add("tr_error", "none");
            requestData.Add("tr_email", "test@mail.com");
            var md5 = FormsAuthentication.HashPasswordForStoringInConfigFile(handler.TransferujId + requestData["tr_id"] + requestData["tr_amount"] + requestData["tr_crc"] + handler.TransferujKodPotwierdzajcy, "MD5");
            requestData.Add("md5sum", md5);

            var serviceSession = sessionFactory.OpenSession();
            using (serviceSession)
            {
                handler.ProcessOrderRequest(serviceSession, requestData, null);
            }
            Session.Clear();
            Assert.IsTrue(handler.SendConfirmationInvoked);
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(360, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.MinValue, dbProfile.Licence.LastPointOperationDate);

            var baPoints = Session.QueryOver<BAPoints>().SingleOrDefault();
            Assert.AreEqual(360, baPoints.Points);
            Assert.AreEqual(dbProfile.GlobalId, baPoints.Profile.GlobalId);
            Assert.AreEqual(requestData["tr_id"], baPoints.Identifier);
            Assert.AreEqual(BAPointsType.Transferuj, baPoints.Type);

        }

        [Test]
        public void WrongRequestIp()
        {
            var handler = new TestableTranferujHandler(manager);

            NameValueCollection requestData = new NameValueCollection();
            requestData.Add("id", handler.TransferujId);
            requestData.Add("tr_id", "TR-USR-11APEX");
            requestData.Add("tr_date", "2012-08-21+19%3a37%3a50");
            requestData.Add("tr_crc", profile.GlobalId.ToString() + "|BAPoints_30");
            requestData.Add("tr_amount", "5.00");
            requestData.Add("tr_paid", "5.00");
            requestData.Add("tr_desc", "BAPoints_30");
            requestData.Add("tr_status", "TRUE");
            requestData.Add("tr_error", "none");
            requestData.Add("tr_email", "test@mail.com");
            var md5 = FormsAuthentication.HashPasswordForStoringInConfigFile(handler.TransferujId + requestData["tr_id"] + requestData["tr_amount"] + requestData["tr_crc"] + handler.TransferujKodPotwierdzajcy, "MD5");
            requestData.Add("md5sum", md5);

            handler.RequestIp = "200.149.229.119";
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
            Assert.IsFalse(handler.SendConfirmationInvoked);
            Session.Clear();
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(0, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.MinValue, dbProfile.Licence.LastPointOperationDate);

            var baPoints = Session.QueryOver<BAPoints>().RowCount();
            Assert.AreEqual(0, baPoints);
        }

        [Test]
        public void WrongTransactionStatus()
        {
            var handler = new TestableTranferujHandler(manager);

            NameValueCollection requestData = new NameValueCollection();
            requestData.Add("id", handler.TransferujId);
            requestData.Add("tr_id", "TR-USR-11APEX");
            requestData.Add("tr_date", "2012-08-21+19%3a37%3a50");
            requestData.Add("tr_crc", profile.GlobalId.ToString() + "|BAPoints_30");
            requestData.Add("tr_amount", "5.00");
            requestData.Add("tr_paid", "5.00");
            requestData.Add("tr_desc", "BAPoints_30");
            requestData.Add("tr_status", "FALSE");
            requestData.Add("tr_error", "none");
            requestData.Add("tr_email", "test@mail.com");
            var md5 = FormsAuthentication.HashPasswordForStoringInConfigFile(handler.TransferujId + requestData["tr_id"] + requestData["tr_amount"] + requestData["tr_crc"] + handler.TransferujKodPotwierdzajcy, "MD5");
            requestData.Add("md5sum", md5);

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
            Assert.IsFalse(handler.SendConfirmationInvoked);
            Session.Clear();
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(0, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.MinValue, dbProfile.Licence.LastPointOperationDate);

            var baPoints = Session.QueryOver<BAPoints>().RowCount();
            Assert.AreEqual(0, baPoints);
        }

        [Test]
        public void TransactionHasError()
        {
            var handler = new TestableTranferujHandler(manager);

            NameValueCollection requestData = new NameValueCollection();
            requestData.Add("id", handler.TransferujId);
            requestData.Add("tr_id", "TR-USR-11APEX");
            requestData.Add("tr_date", "2012-08-21+19%3a37%3a50");
            requestData.Add("tr_crc", profile.GlobalId.ToString() + "|BAPoints_30");
            requestData.Add("tr_amount", "5.00");
            requestData.Add("tr_paid", "5.00");
            requestData.Add("tr_desc", "BAPoints_30");
            requestData.Add("tr_status", "TRUE");
            requestData.Add("tr_error", "surcharge");
            requestData.Add("tr_email", "test@mail.com");
            var md5 = FormsAuthentication.HashPasswordForStoringInConfigFile(handler.TransferujId + requestData["tr_id"] + requestData["tr_amount"] + requestData["tr_crc"] + handler.TransferujKodPotwierdzajcy, "MD5");
            requestData.Add("md5sum", md5);

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
            Assert.IsFalse(handler.SendConfirmationInvoked);
            Session.Clear();
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(0, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.MinValue, dbProfile.Licence.LastPointOperationDate);

            var baPoints = Session.QueryOver<BAPoints>().RowCount();
            Assert.AreEqual(0, baPoints);
        }

        [Test]
        public void MD5Incorrect()
        {
            var handler = new TestableTranferujHandler(manager);

            NameValueCollection requestData = new NameValueCollection();
            requestData.Add("id", handler.TransferujId);
            requestData.Add("tr_id", "TR-USR-11APEX");
            requestData.Add("tr_date", "2012-08-21+19%3a37%3a50");
            requestData.Add("tr_crc", profile.GlobalId.ToString() + "|BAPoints_30");
            requestData.Add("tr_amount", "5.00");
            requestData.Add("tr_paid", "5.00");
            requestData.Add("tr_desc", "BAPoints_30");
            requestData.Add("tr_status", "TRUE");
            requestData.Add("tr_error", "surcharge");
            requestData.Add("tr_email", "test@mail.com");
            requestData.Add("md5sum", "fdgfdgdfgdfgfdgf");

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
            Assert.IsFalse(handler.SendConfirmationInvoked);
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
            point.Identifier = "TR-USR-11APEX";
            point.Profile = profile;
            point.ImportedDate = DateTime.UtcNow;
            insertToDatabase(point);

            var handler = new TestableTranferujHandler(manager);

            NameValueCollection requestData = new NameValueCollection();
            requestData.Add("id", handler.TransferujId);
            requestData.Add("tr_id", "TR-USR-11APEX");
            requestData.Add("tr_date", "2012-08-21+19%3a37%3a50");
            requestData.Add("tr_crc", profile.GlobalId.ToString() + "|BAPoints_30");
            requestData.Add("tr_amount", "5.00");
            requestData.Add("tr_paid", "5.00");
            requestData.Add("tr_desc", "BAPoints_30");
            requestData.Add("tr_status", "TRUE");
            requestData.Add("tr_error", "none");
            requestData.Add("tr_email", "test@mail.com");
            var md5 = FormsAuthentication.HashPasswordForStoringInConfigFile(handler.TransferujId + requestData["tr_id"] + requestData["tr_amount"] + requestData["tr_crc"] + handler.TransferujKodPotwierdzajcy, "MD5");
            requestData.Add("md5sum", md5);
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
