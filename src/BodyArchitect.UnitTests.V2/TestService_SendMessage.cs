using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NUnit.Framework;
using MessagePriority = BodyArchitect.Service.V2.Model.MessagePriority;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_SendMessage:TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();

        public override void BuildDatabase()
        {
            profiles.Clear();
            using (var tx = Session.BeginTransaction())
            {
                var profile = CreateProfile(Session, "Profile1");
                profiles.Add(profile);
                profile = CreateProfile(Session, "Profile2");
                profiles.Add(profile);
                profile = CreateProfile(Session, "Profile3");
                profiles.Add(profile);

                profile = CreateProfile(Session, "Profile4");
                profile.IsDeleted = true;
                Session.Update(profile);
                profiles.Add(profile);

                //set friendship
                profiles[0].Friends.Add(profiles[1]);
                profiles[1].Friends.Add(profiles[0]);
                Session.Update(profiles[0]);
                Session.Update(profiles[1]);

                tx.Commit();
            }
        }

        [Test]
        [ExpectedException(typeof(UserDeletedException))]
        public void TestSendToDeletedProfile_Exception()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[3].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                MessageDTO message = new MessageDTO();
                message.Sender = profile;
                message.Receiver = profile1;
                message.Topic = "test topic";
                message.Content = "test content";
                message.Priority = MessagePriority.Normal;
                Service.SendMessage(data.Token, message);
            });
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestSendCustomMessageWithSystemPriority_Exception()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                MessageDTO message = new MessageDTO();
                message.Sender = profile;
                message.Receiver = profile;
                message.Topic = "test topic";
                message.Content = "test content";
                message.Priority = MessagePriority.System;
                Service.SendMessage(data.Token, message);
            });
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSendWithoutRecevier_Exception()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                MessageDTO message = new MessageDTO();
                message.Sender = profile;
                message.Topic = "test topic";
                message.Content = "test content";
                Service.SendMessage(data.Token, message);
            });
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void TestSendFromDifferentUser_Exception()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                MessageDTO message = new MessageDTO();
                message.Sender = profile1;
                message.Receiver = profile1;
                message.Topic = "test topic";
                message.Content = "test content";
                Service.SendMessage(data.Token, message);
            });
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSendWithoutSender_Exception()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                MessageDTO message = new MessageDTO();
                message.Receiver = profile1;
                message.Topic = "test topic";
                message.Content = "test content";
                Service.SendMessage(data.Token, message);
            });
        }

        [Test]
        public void TestSendCustomMessageToTheUser()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                MessageDTO message = new MessageDTO();
                message.Sender = profile;
                message.Receiver = profile1;
                message.Topic = "test topic";
                message.Content = "test content";
                Service.SendMessage(data.Token, message);
            });
            var messageDb =Session.QueryOver<Message>().SingleOrDefault();
            Assert.IsNotNull(messageDb);
            Assert.AreEqual("test topic", messageDb.Topic);
            Assert.AreEqual("test content", messageDb.Content);
            Assert.AreEqual(profile.GlobalId, messageDb.Sender.GlobalId);
            Assert.AreEqual(profile1.GlobalId, messageDb.Receiver.GlobalId);
        }


        [Test]
        public void TestSendCustomMessageToTheUser_DataInfo()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var oldHash = profiles[1].DataInfo.MessageHash;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                MessageDTO message = new MessageDTO();
                message.Sender = profile;
                message.Receiver = profile1;
                message.Topic = "test topic";
                message.Content = "test content";
                Service.SendMessage(data.Token, message);
            });
            var dbProfile = Session.Get<Profile>(profile1.GlobalId);
            Assert.AreNotEqual(dbProfile.DataInfo.MessageHash, oldHash);
        }
    }
}
