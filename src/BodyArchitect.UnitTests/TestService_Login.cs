using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using BodyArchitect.Common;
using BodyArchitect.Model;
using BodyArchitect.Service;
using BodyArchitect.Service.Model;
using BodyArchitect.Shared;
using NUnit.Framework;

namespace BodyArchitect.UnitTests
{
    [TestFixture]
    public class TestService_Login:TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();

        public override void BuildDatabase()
        {
            profiles.Clear();
            
            using (var tx = Session.BeginTransaction())
            {
                var profile = CreateProfile(Session, "Profile1");
                profile.Password = CryptographyHelper.ToSHA1Hash(profile.UserName);
                Session.Update(profile);
                profiles.Add(profile);
                profile = CreateProfile(Session, "Profile2");
                profile.IsDeleted = true;
                profile.Password = CryptographyHelper.ToSHA1Hash(profile.UserName);
                Session.Update(profile);
                profiles.Add(profile);
                profile = CreateProfile(Session, "Profile3");
                profile.Password = CryptographyHelper.ToSHA1Hash(profile.UserName);
                Session.Update(profile);
                profiles.Add(profile);

                tx.Commit();
            }
        }

        [Test]
        public void TestLogin()
        {
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ClientInformation info = new ClientInformation();
                info.ApplicationVersion = Constants.Version;
                info.ApplicationLanguage = "pl";
                info.Version = Const.ServiceVersion;
                info.ClientInstanceId = Guid.NewGuid();
                string password = CryptographyHelper.ToSHA1Hash("Profile1");

                var session=service.Login(info, "Profile1", password);
                Assert.IsNotNull(session);
            });
        }

        [Test]
        [ExpectedException(typeof(ProfileDeletedException))]
        public void TestLogin_DeletedProfile()
        {
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ClientInformation info = new ClientInformation();
                info.ApplicationVersion = Constants.Version;
                info.ApplicationLanguage = "pl";
                info.Version = Const.ServiceVersion;
                info.ClientInstanceId = Guid.NewGuid();
                string password = CryptographyHelper.ToSHA1Hash("Profile2");

                service.Login(info, "Profile2", password);
            });
        }

        [Test]
        public void TestLogin_Statistics()
        {
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ClientInformation info = new ClientInformation();
                info.ApplicationVersion = Constants.Version;
                info.ApplicationLanguage = "pl";
                info.Version = Const.ServiceVersion;
                info.ClientInstanceId = Guid.NewGuid();
                string password = CryptographyHelper.ToSHA1Hash("Profile1");

                service.Login(info, profiles[0].UserName, password);
            });

            var dbProfile = Session.Get<Profile>(profiles[0].Id);
            Assert.IsNotNull(dbProfile.Statistics.LastLoginDate);
        }
    }
}
