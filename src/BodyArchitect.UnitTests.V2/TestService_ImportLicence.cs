using System;
using System.Collections.Generic;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using NUnit.Framework;
using AccountType = BodyArchitect.Model.AccountType;
using Constants = BodyArchitect.Portable.Constants;
using LicenceType = BodyArchitect.Service.V2.LicenceType;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestLicenceGenerator
    {
        [Test]
        public void GenerateAndDecryptLicence()
        {
            LicenceGenerator generator = new LicenceGenerator();
            var test=generator.GenerateLicenceKey(30);
            var licenceInfo=generator.GetLicence(test);

            Assert.AreEqual(30, licenceInfo.BAPoints);
            Assert.AreNotEqual(Guid.Empty, licenceInfo.Id);
        }
    }

    [TestFixture]
    public class TestService_ImportLicence : TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        Guid key = new Guid("EB17BC2A-94FD-4E65-8751-15730F69E7F5");

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                profiles.Add(CreateProfile(Session, "test1"));
                profiles.Add(CreateProfile(Session, "test2"));
                tx.Commit();
            }
        }

        [Test]
        public void ImportLicence_DataInLicenceKey()
        {
            LicenceGenerator generator = new LicenceGenerator();
            var goodLicenceKey = generator.GenerateLicenceKey(30);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
                    {
                        ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date.AddHours(4);
                        service.ImportLicence(data.Token, goodLicenceKey);
                    });

            var licenceInfo = generator.GetLicence(goodLicenceKey);

            var licence=Session.QueryOver<BAPoints>().SingleOrDefault();
            Assert.AreEqual(30, licence.Points);
            Assert.AreEqual(BAPointsType.Serialkey, licence.Type);
            Assert.AreEqual(DateTime.UtcNow.Date.AddHours(4), licence.ImportedDate);
            Assert.AreEqual(licenceInfo.Id.ToString(), licence.Identifier);
        }


        [Test]
        public void ImportLicence_DataInLicenceKey_LoginDataSet()
        {
            LicenceGenerator generator = new LicenceGenerator();
            var goodLicenceKey = generator.GenerateLicenceKey(30);
            string password = CryptographyHelper.ToSHA1Hash(profiles[0].UserName);
            profiles[0].Password = password;
            insertToDatabase(profiles[0]);

            APIKey apiKey = new APIKey();
            apiKey.ApiKey = key;
            apiKey.ApplicationName = "UnitTest";
            apiKey.EMail = "mail@mail.com";
            apiKey.RegisterDateTime = DateTime.UtcNow;
            insertToDatabase(apiKey);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                //first login to have LoginData
                service.Configuration.CurrentApiKey = key;
                ClientInformation info = new ClientInformation();
                info.ApplicationVersion = Constants.Version;
                info.ApplicationLanguage = "pl";
                info.Version = Const.ServiceVersion;
                info.ClientInstanceId = Guid.NewGuid();
                

                var sessionData=service.Login(info, profiles[0].UserName, password);

                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date.AddHours(4);
                service.ImportLicence(sessionData.Token, goodLicenceKey);
            });
            
            var licence = Session.QueryOver<BAPoints>().SingleOrDefault();
            Assert.IsNotNull(licence.LoginData);
        }

        [Test]
        public void ImportLicence_ProfileAccountTypeShouldntBeUpdated()
        {
            LicenceGenerator generator = new LicenceGenerator();
            var goodLicenceKey = generator.GenerateLicenceKey(30);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ((MockTimerService) service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date.AddHours(4);
                service.ImportLicence(data.Token, goodLicenceKey);
            });


            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(AccountType.Instructor, dbProfile.Licence.AccountType);
        }

        [Test]
        public void ImportLicence_SecurityManagerCurrentAccountTypeShouldntBeUpdated()
        {
            LicenceGenerator generator = new LicenceGenerator();
            var goodLicenceKey = generator.GenerateLicenceKey(30);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
                                 {
                                     var securityInfo=SecurityManager.EnsureAuthentication(data.Token);
                                     securityInfo.Licence.CurrentAccountType = Service.V2.Model.AccountType.PremiumUser;
                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date.AddHours(4);
                service.ImportLicence(data.Token, goodLicenceKey);

                Assert.AreEqual(Service.V2.Model.AccountType.PremiumUser, securityInfo.Licence.CurrentAccountType);
            });

        }

        [Test]
        public void ImportLicence_ProfileBAPointsShouldBeUpdated()
        {
            LicenceGenerator generator = new LicenceGenerator();
            var goodLicenceKey = generator.GenerateLicenceKey(30);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date.AddHours(4);
                service.ImportLicence(data.Token, goodLicenceKey);
            });


            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(30, dbProfile.Licence.BAPoints);
        }

        [Test]
        public void ImportLicence_SecuriyManagerUpdated()
        {
            LicenceGenerator generator = new LicenceGenerator();
            var goodLicenceKey = generator.GenerateLicenceKey( 30);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date.AddHours(4);
                service.ImportLicence(data.Token, goodLicenceKey);

                var securityInfo = SecurityManager.EnsureAuthentication(data.Token);
                Assert.AreEqual(30, securityInfo.Licence.BAPoints);
            });
        }

        

        [Test]
        public void ImportLicence_TwoDifferentKeys_TwoDifferentAccountType_AddPoints()
        {
            LicenceGenerator generator = new LicenceGenerator();
            var goodLicenceKey1 = generator.GenerateLicenceKey( 30);
            var goodLicenceKey2 = generator.GenerateLicenceKey( 60);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date.AddHours(4);
                service.ImportLicence(data.Token, goodLicenceKey1);

                service.ImportLicence(data.Token, goodLicenceKey2);
            });

            Assert.AreEqual(2,Session.QueryOver<BAPoints>().RowCount());

            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(90, dbProfile.Licence.BAPoints);
        }


        [Test]
        [ExpectedException(typeof(AlreadyOccupiedException))]
        public void ImportLicence_TwiceTheSameLicence()
        {
            LicenceGenerator generator = new LicenceGenerator();
            var goodLicenceKey = generator.GenerateLicenceKey(30);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.ImportLicence(data.Token, goodLicenceKey);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.ImportLicence(data.Token, goodLicenceKey);
            });
        }
    }
}
