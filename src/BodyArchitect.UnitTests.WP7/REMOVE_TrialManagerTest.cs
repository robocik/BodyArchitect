using System;
using System.IO.IsolatedStorage;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BodyArchitect.UnitTests.WP7
{
    [TestClass]
    public class TrialManagerTest
    {
        [TestInitialize]
        public void Setup()
        {
            ApplicationState.IsFree = false;
            IsolatedStorageSettings.ApplicationSettings.Clear();
        }

        [TestMethod]
        public void FirstRunTrial()
        {
            var manager = new MockTrialManager();
            manager.DetermineIsTrail();
            Assert.IsFalse(ApplicationState.IsFree);//trial mode is a time limited full application
            Assert.IsTrue(manager.RetrievedFromService);
            Assert.IsNotNull(Settings.TrialStarted);
            Assert.AreEqual(DateTime.UtcNow.Date,Settings.TrialStarted.Value.Date);
        }

        [TestMethod]
        public void FirstRunPro()
        {
            var manager = new MockTrialManager();
            manager.LicenceTrial = false;
            manager.DetermineIsTrail();
            Assert.IsFalse(manager.RetrievedFromService);
            Assert.IsFalse(ApplicationState.IsFree);
            Assert.IsNull(Settings.TrialStarted);
        }

        [TestMethod]
        public void FirstRunTrialAndBuyBeforeExpiration()
        {
            var manager = new MockTrialManager();
            manager.DetermineIsTrail();
            Assert.IsFalse(ApplicationState.IsFree);//trial mode is a time limited full application
            Assert.IsNotNull(Settings.TrialStarted);
            Assert.IsTrue(manager.RetrievedFromService);
            Assert.AreEqual(DateTime.UtcNow.Date, Settings.TrialStarted.Value.Date);
            manager.RetrievedFromService = false;
            //buy now
            manager.LicenceTrial = false;
            manager.DetermineIsTrail();
            Assert.IsFalse(manager.RetrievedFromService);
            Assert.IsFalse(ApplicationState.IsFree);
            Assert.IsNull(Settings.TrialStarted);
        }

        [TestMethod]
        public void FirstRunTrialAndBuyAfterExpiration()
        {
            var manager = new MockTrialManager();
            var startTime = DateTime.UtcNow.AddDays(-300);

            Settings.TrialStarted = startTime;
            manager.DetermineIsTrail();
            Assert.IsTrue(ApplicationState.IsFree);//trial mode after expiration is a free version
            Assert.IsNotNull(Settings.TrialStarted);
            Assert.IsFalse(manager.RetrievedFromService);
            Assert.AreEqual(startTime, Settings.TrialStarted.Value);
            manager.RetrievedFromService = false;
            //buy now
            manager.LicenceTrial = false;
            manager.DetermineIsTrail();
            Assert.IsFalse(manager.RetrievedFromService);
            Assert.IsFalse(ApplicationState.IsFree);
            Assert.IsNull(Settings.TrialStarted);
        }

    }

    public class MockTrialManager:TrialManager
    {
        public bool LicenceTrial=true;
        public DateTime? TrialStarted=DateTime.UtcNow;
        private DateTime currentDateTime=DateTime.UtcNow;
        public bool RetrievedFromService;

        protected override DateTime getCurrentDateTime()
        {
            return currentDateTime;
        }
        protected override void retrieveTrialFromService()
        {
            RetrievedFromService = true;
            retrieveTrialFromService_Completed(new WP7TrialStatusCompletedEventArgs(new object[]{new TrialStatusInfo(){TrialStarted = TrialStarted}},null,false,null ) );
        }

        protected override bool getTrialStatusFromLicence()
        {
            return LicenceTrial;
        }

        
    }
}
