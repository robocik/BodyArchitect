using System;
using System.Security.Authentication;
using System.Threading;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using NUnit.Framework;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestSecurityManager
    {
        private ProfileDTO profile;
        private SecurityManager securityManager;
        private ClientInformation clientInformation;

        [TestFixtureSetUp]
        public void setupFixture()
        {
            profile = new ProfileDTO();
            profile.UserName = "test";

            var conf = new ConfigurationSourceBuilder();
            conf.ConfigureCaching().ForCacheManagerNamed(SecurityManager.AuthenticationCacheName).StoreInMemory();
            
            var configSource = new DictionaryConfigurationSource();
            conf.UpdateConfigurationWithReplace(configSource);

            EnterpriseLibraryContainer.Current = EnterpriseLibraryContainer.CreateDefaultContainer(configSource);
        }

        [SetUp]
        public void testSetup()
        {
            securityManager = new SecurityManager();
            clientInformation = new ClientInformation(){Version=Const.ServiceVersion};
        }

        [TearDown]
        public void testEnd()
        {
            securityManager.Dispose();
        }
        [Test]
        public void TestCreatingSessionDataWithEnsuringAuthentication()
        {
            securityManager.SessionTimeout = 2;//2 seconds for testing
            SessionData sessionData = securityManager.CreateNewSession(profile, clientInformation,new LoginData());
            Assert.AreEqual(1,securityManager.LoggedClients);
            securityManager.EnsureAuthentication(sessionData.Token);
            Assert.AreEqual(1, securityManager.LoggedClients);
        }

        [Test]
        [ExpectedException(typeof(BodyArchitect.Portable.Exceptions.AuthenticationException))]
        public void TestEnsuringAuthentication_NotAuthenticatedClient()
        {
            securityManager.SessionTimeout = 2;//2 seconds for testing
            Token token = new Token(Guid.NewGuid());//token created without securitymanager. Hacker attack:)
            Assert.AreEqual(0, securityManager.LoggedClients);
            securityManager.EnsureAuthentication(token);
        }

        [Test]
        public void TestEnsuringAuthentication_SessionTimeout()
        {
            securityManager.SessionTimeout = 2;//2 seconds for testing
            SessionData sessionData = securityManager.CreateNewSession(profile, clientInformation, new LoginData());
            Assert.AreEqual(1, securityManager.LoggedClients);
            securityManager.EnsureAuthentication(sessionData.Token);
            Assert.AreEqual(1, securityManager.LoggedClients);

            Thread.Sleep(3000);//3 seconds
            try
            {
                securityManager.EnsureAuthentication(sessionData.Token);
                Assert.Fail("Should throw AuthenticationException exception because of session timeout");
            }
            catch (BodyArchitect.Portable.Exceptions.AuthenticationException)
            {
            }
            
            Assert.AreEqual(0, securityManager.LoggedClients);
        }

        [Test]
        public void TestLogout()
        {
            SessionData sessionData = securityManager.CreateNewSession(profile, clientInformation, new LoginData());
            Assert.AreEqual(1, securityManager.LoggedClients);
            securityManager.Remove(sessionData.Token);
            Assert.AreEqual(0, securityManager.LoggedClients);
        }
    }
}
