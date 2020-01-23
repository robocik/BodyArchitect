using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service;
using BodyArchitect.Service.Model;
using BodyArchitect.Shared;
using NUnit.Framework;
using MessageType = BodyArchitect.Model.MessageType;

namespace BodyArchitect.UnitTests
{
    [TestFixture]
    public class TestService_MessageOperation:TestServiceBase
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


                tx.Commit();
            }
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void TestDeleteMessage_ForAnotherUser()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                MessageDTO message = new MessageDTO();
                message.Sender = profile;
                message.Receiver = profile1;
                message.MessageType = (Service.Model.MessageType) MessageType.Custom;
                message.Topic = "test topic";
                message.Content = "test content";
                Service.SendMessage(data.Token, message);
            });
            var dbMessage=Session.QueryOver<Message>().SingleOrDefault();
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                MessageOperationParam arg = new MessageOperationParam();
                arg.Operation = MessageOperationType.Delete;
                arg.MessageId = dbMessage.Id;
                Service.MessageOperation(data.Token, arg);
            });
        }

        [Test]
        public void TestDeleteMessage()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                MessageDTO message = new MessageDTO();
                message.Sender = profile;
                message.Receiver = profile1;
                message.MessageType = (Service.Model.MessageType)MessageType.Custom;
                message.Topic = "test topic";
                message.Content = "test content";
                Service.SendMessage(data.Token, message);
            });
            var dbMessage = Session.QueryOver<Message>().SingleOrDefault();
            data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                MessageOperationParam arg = new MessageOperationParam();
                arg.Operation = MessageOperationType.Delete;
                arg.MessageId = dbMessage.Id;
                Service.MessageOperation(data.Token, arg);
            });
            Assert.AreEqual(0,Session.QueryOver<Message>().RowCount());
        }
    }
}
