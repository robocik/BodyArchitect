using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service;
using BodyArchitect.Service.Model;
using NUnit.Framework;
using MessageType = BodyArchitect.Model.MessageType;

namespace BodyArchitect.UnitTests
{
    [TestFixture]
    public class TestService_CreateProfile:TestServiceBase
    {
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
                                     data=service.CreateProfile(this.ClientInformation, newProfile);
                                 });
            Profile dbProfile = Session.Get<Profile>(data.Profile.Id);
            var message = Session.QueryOver<Message>().Where(x => x.Receiver == dbProfile).SingleOrDefault();
            Assert.AreEqual(MessageType.ProfileCreated,message.MessageType);
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
                data = service.CreateProfile(this.ClientInformation, newProfile);
                Assert.IsTrue(((MockEmailService)service.EMailService).EMailSent);
            });
            
        }

    }
}
