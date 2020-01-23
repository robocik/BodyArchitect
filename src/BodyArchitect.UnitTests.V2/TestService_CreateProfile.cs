using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using NUnit.Framework;
using AccountType = BodyArchitect.Model.AccountType;
using LengthType = BodyArchitect.Service.V2.Model.LengthType;
using WeightType = BodyArchitect.Service.V2.Model.WeightType;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_CreateProfile:TestServiceBase
    {
        private Guid key = new Guid("EB17BC2A-94FD-4E65-8751-15730F69E7F5");
        public override void BuildDatabase()
        {

            using (var tx = Session.BeginTransaction())
            {

                APIKey apiKey = new APIKey();
                apiKey.ApiKey = key;
                apiKey.ApplicationName = "UnitTest";
                apiKey.EMail = "mail@mail.com";
                apiKey.RegisterDateTime = DateTime.UtcNow;
                insertToDatabase(apiKey);
                tx.Commit();
            }
        }

        [Test]
        public void TestLicenceCreation_InDb_NewlyCreatedUserHas30DaysPremiumAccount()
        {
            ProfileDTO newProfile = new ProfileDTO();
            newProfile.UserName = "test";
            newProfile.Password = "test";
            newProfile.Email = "fdgfdg@fdgdfg.pl";
            newProfile.Birthday = DateTime.Now.AddYears(-20);
            newProfile.CountryId = Country.Countries[1].GeoId;
            SessionData data = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date;
                service.Configuration.CurrentApiKey = key;
                data = service.CreateProfile(this.ClientInformation, newProfile);
            });

            Profile dbProfile = Session.Get<Profile>(data.Profile.GlobalId);
            Assert.IsNotNull(dbProfile.Licence);
            Assert.AreEqual(AccountType.PremiumUser, dbProfile.Licence.AccountType);
            Assert.AreEqual(30, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.UtcNow.Date, dbProfile.Licence.LastPointOperationDate);
        }

        [Test]
        public void TestLicenceCreation_InSecurityManager_NewlyCreatedUserHas30DaysPremiumAccount()
        {
            ProfileDTO newProfile = new ProfileDTO();
            newProfile.UserName = "test";
            newProfile.Password = "test";
            newProfile.Email = "fdgfdg@fdgdfg.pl";
            newProfile.Birthday = DateTime.Now.AddYears(-20);
            newProfile.CountryId = Country.Countries[1].GeoId;
            SessionData data = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date;
                service.Configuration.CurrentApiKey = key;
                data = service.CreateProfile(this.ClientInformation, newProfile);
            });

            var securityInfo=SecurityManager.EnsureAuthentication(data.Token);
            Assert.AreEqual(Service.V2.Model.AccountType.PremiumUser, securityInfo.Licence.CurrentAccountType);
            Assert.AreEqual(Service.V2.Model.AccountType.PremiumUser, securityInfo.Licence.AccountType);
            Assert.AreEqual(30, securityInfo.Licence.BAPoints);
        }

        [Test]
        public void TestSendMessageAfterProfileCreated_NoActivation()
        {
            ProfileDTO newProfile = new ProfileDTO();
            newProfile.UserName = "test";
            newProfile.Password = "test";
            newProfile.Email = "fdgfdg@fdgdfg.pl";
            newProfile.Birthday = DateTime.Now.AddYears(-20);
            newProfile.CountryId = Country.Countries[1].GeoId;
            SessionData data=null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
                                 {
                                     service.Configuration.CurrentApiKey = key;
                                     data=service.CreateProfile(this.ClientInformation, newProfile);
                                 });
            Profile dbProfile = Session.Get<Profile>(data.Profile.GlobalId);
            var message = Session.QueryOver<Message>().Where(x => x.Receiver == dbProfile).SingleOrDefault();
            Assert.IsNotNull( message.Content);
        }

        [Test]
        public void CreateProfileWithSettings()
        {
            ProfileDTO newProfile = new ProfileDTO();
            newProfile.UserName = "test";
            newProfile.Password = "test";
            newProfile.Email = "fdgfdg@fdgdfg.pl";
            newProfile.Settings = new ProfileSettingsDTO();
            newProfile.Settings.WeightType = WeightType.Pounds;
            newProfile.Settings.LengthType = LengthType.Inchs;
            newProfile.Birthday = DateTime.Now.AddYears(-20);
            newProfile.CountryId = Country.Countries[1].GeoId;
            SessionData data = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.Configuration.CurrentApiKey = key;
                data = service.CreateProfile(this.ClientInformation, newProfile);
            });
            var dbProfile = Session.Get<Profile>(data.Profile.GlobalId);
            Assert.AreEqual(Model.WeightType.Pounds,dbProfile.Settings.WeightType);
            Assert.AreEqual(Model.LengthType.Inchs, dbProfile.Settings.LengthType);
        }

        [Test]
        public void TestSendEMailMessageAfterProfileCreated()
        {
            ProfileDTO newProfile = new ProfileDTO();
            newProfile.UserName = "test";
            newProfile.Password = "test";
            newProfile.Email = "fdgfdg@fdgdfg.pl";
            newProfile.Birthday = DateTime.Now.AddYears(-20);
            newProfile.CountryId = Country.Countries[1].GeoId;
            SessionData data = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.Configuration.CurrentApiKey = key;
                data = service.CreateProfile(this.ClientInformation, newProfile);
                Assert.IsTrue(((MockEmailService)service.EMailService).EMailSent);
            });
            
        }

        [Test]
        public void CreateDataInfo()
        {
            ProfileDTO newProfile = new ProfileDTO();
            newProfile.UserName = "test";
            newProfile.Password = "test";
            newProfile.Email = "fdgfdg@fdgdfg.pl";
            newProfile.Birthday = DateTime.Now.AddYears(-20);
            newProfile.CountryId = Country.Countries[1].GeoId;
            SessionData data = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.Configuration.CurrentApiKey = key;
                data = service.CreateProfile(this.ClientInformation, newProfile);
            });
            Profile dbProfile = Session.Get<Profile>(data.Profile.GlobalId);
            Assert.IsNotNull(dbProfile.DataInfo);
            var count = Session.QueryOver<DataInfo>().RowCount();
            Assert.AreEqual(1, count);
        }

        [Test]
        public void CreateDefaultMyPlace()
        {
            ProfileDTO newProfile = new ProfileDTO();
            newProfile.UserName = "test";
            newProfile.Password = "test";
            newProfile.Email = "fdgfdg@fdgdfg.pl";
            newProfile.Birthday = DateTime.Now.AddYears(-20);
            newProfile.CountryId = Country.Countries[1].GeoId;
            SessionData data = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date;
                service.Configuration.CurrentApiKey = key;
                data = service.CreateProfile(this.ClientInformation, newProfile);
            });
            var myPlace = Session.QueryOver<MyPlace>().Where(x => x.Profile.GlobalId == data.Profile.GlobalId).SingleOrDefault();
            Assert.IsNotNull(myPlace);
            Assert.IsTrue(myPlace.IsDefault);
            Assert.IsTrue(myPlace.IsSystem);
        }

    }
}
