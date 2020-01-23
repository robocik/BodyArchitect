using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NUnit.Framework;
using MessagePriority = BodyArchitect.Model.MessagePriority;
using ProfileNotification = BodyArchitect.Model.ProfileNotification;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_FriendsOperations:TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                profiles.Add(CreateProfile(Session,"test1"));
                profiles.Add(CreateProfile(Session, "test2"));
                profiles.Add(CreateProfile(Session, "test3"));

                profiles.Add(CreateProfile(Session, "test4"));
                profiles[3].IsDeleted = true;
                Session.Update(profiles[3]);
                tx.Commit();
            }
        }

        [Test]
        public void TestInviteAFriend_RemoveFromInviterFavoritesCollection()
        {
            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[1].Tag;

            profiles[0].FavoriteUsers.Add(profiles[1]);
            insertToDatabase(profiles[0]);

            SessionData data = CreateNewSession(inviter, ClientInformation);
            FriendInvitationDTO result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                result = service.InviteFriendOperation(data.Token, arg);
            });

            data = CreateNewSession(invited, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Operation = InviteFriendOperation.Accept;
                result = service.InviteFriendOperation(data.Token, arg);
            });
            var dbProfile = Session.Get<Profile>(inviter.GlobalId);
            Assert.AreEqual(1, dbProfile.Friends.Count);
            Assert.AreEqual(0,dbProfile.FavoriteUsers.Count);
        }

        [Test]
        public void TestInviteAFriend_RemoveFromInvitedFavoritesCollection()
        {
            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[1].Tag;

            profiles[1].FavoriteUsers.Add(profiles[0]);
            insertToDatabase(profiles[1]);

            SessionData data = CreateNewSession(inviter, ClientInformation);
            FriendInvitationDTO result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                result = service.InviteFriendOperation(data.Token, arg);
            });

            data = CreateNewSession(invited, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Operation = InviteFriendOperation.Accept;
                result = service.InviteFriendOperation(data.Token, arg);
            });
            var dbProfile = Session.Get<Profile>(invited.GlobalId);
            Assert.AreEqual(1, dbProfile.Friends.Count);
            Assert.AreEqual(0, dbProfile.FavoriteUsers.Count);
        }

        [Test]
        public void TestSendMessageAfterAccept()
        {
            profiles[1].Settings.NotificationSocial = ProfileNotification.Message;
            profiles[0].Settings.NotificationSocial = ProfileNotification.Message;
            insertToDatabase(profiles[1]);
            insertToDatabase(profiles[0]);

            FriendInvitationDTO result = null;

            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(inviter, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                result = Service.InviteFriendOperation(data.Token, arg);
                
            });
            
            data = CreateNewSession(invited, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Message = "test msg";
                arg.Operation = InviteFriendOperation.Accept;
                result = Service.InviteFriendOperation(data.Token, arg);
                Assert.IsFalse(((MockEmailService)Service.EMailService).EMailSent);
            });
            
            int count = Session.QueryOver<Message>().RowCount();
            Assert.AreEqual(1,count);
            var message=Session.QueryOver<Message>().SingleOrDefault();
            Assert.AreEqual(invited.GlobalId, message.Sender.GlobalId);
            Assert.AreEqual(inviter.GlobalId, message.Receiver.GlobalId);
            Assert.AreEqual(MessagePriority.System, message.Priority);
            Assert.IsNotNull( message.Content);
        }

        [Test]
        public void TestSendEMailAfterInvitation()
        {
            profiles[1].Settings.NotificationSocial = ProfileNotification.Email;
            profiles[0].Settings.NotificationSocial = ProfileNotification.Email;
            insertToDatabase(profiles[1]);
            insertToDatabase(profiles[0]);
            FriendInvitationDTO result = null;

            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(inviter, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                result = Service.InviteFriendOperation(data.Token, arg);
                Assert.IsTrue(((MockEmailService)Service.EMailService).EMailSent);
            });

            int count = Session.QueryOver<Message>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestDoNotSendMessageAfterInvitation()
        {
            profiles[1].Settings.NotificationSocial = ProfileNotification.Message;
            profiles[0].Settings.NotificationSocial = ProfileNotification.Message;
            insertToDatabase(profiles[1]);
            insertToDatabase(profiles[0]);
            FriendInvitationDTO result = null;

            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(inviter, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                result = Service.InviteFriendOperation(data.Token, arg);
                Assert.IsFalse(((MockEmailService)Service.EMailService).EMailSent);
            });

            int count = Session.QueryOver<Message>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestSendEMailAfterAccept()
        {
            profiles[1].Settings.NotificationSocial = ProfileNotification.Email;
            profiles[0].Settings.NotificationSocial = ProfileNotification.Email;
            insertToDatabase(profiles[1]);
            insertToDatabase(profiles[0]);
            FriendInvitationDTO result = null;

            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(inviter, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                result = Service.InviteFriendOperation(data.Token, arg);
            });

            data = CreateNewSession(invited, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Message = "test msg";
                arg.Operation = InviteFriendOperation.Accept;
                result = Service.InviteFriendOperation(data.Token, arg);
                Assert.IsTrue(((MockEmailService)Service.EMailService).EMailSent);
            });

            int count = Session.QueryOver<Message>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestSendMessageAfterRejectInvitation()
        {
            profiles[1].Settings.NotificationSocial = ProfileNotification.Message;
            profiles[0].Settings.NotificationSocial = ProfileNotification.Message;
            insertToDatabase(profiles[1]);
            insertToDatabase(profiles[0]);

            FriendInvitationDTO result = null;

            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(inviter, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                result = Service.InviteFriendOperation(data.Token, arg);
            });

            data = CreateNewSession(invited, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Operation = InviteFriendOperation.Reject;
                arg.Message = "test msg";
                result = Service.InviteFriendOperation(data.Token, arg);
                Assert.IsFalse(((MockEmailService)Service.EMailService).EMailSent);
            });

            int count = Session.QueryOver<Message>().RowCount();
            Assert.AreEqual(1, count);
            var message = Session.QueryOver<Message>().SingleOrDefault();
            Assert.IsNotNull(message.Content);
            Assert.AreEqual(MessagePriority.System, message.Priority);
            Assert.AreEqual(invited.GlobalId, message.Sender.GlobalId);
            Assert.AreEqual(inviter.GlobalId, message.Receiver.GlobalId);
        }

        [Test]
        public void TestSendEMailAfterRejectInvitation()
        {
            profiles[1].Settings.NotificationSocial = ProfileNotification.Email;
            profiles[0].Settings.NotificationSocial = ProfileNotification.Email;
            insertToDatabase(profiles[1]);
            insertToDatabase(profiles[0]);

            FriendInvitationDTO result = null;

            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(inviter, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                result = Service.InviteFriendOperation(data.Token, arg);
            });

            data = CreateNewSession(invited, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Operation = InviteFriendOperation.Reject;
                arg.Message = "test msg";
                result = Service.InviteFriendOperation(data.Token, arg);
                Assert.IsTrue(((MockEmailService)Service.EMailService).EMailSent);
            });

            int count = Session.QueryOver<Message>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestDisableSendEMailAfterRejectInvitation()
        {
            profiles[1].Settings.NotificationSocial = ProfileNotification.None;
            profiles[0].Settings.NotificationSocial = ProfileNotification.None;
            insertToDatabase(profiles[1]);
            insertToDatabase(profiles[0]);

            FriendInvitationDTO result = null;

            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(inviter, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                result = Service.InviteFriendOperation(data.Token, arg);
            });

            data = CreateNewSession(invited, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Operation = InviteFriendOperation.Reject;
                arg.Message = "test msg";
                result = Service.InviteFriendOperation(data.Token, arg);
                Assert.IsFalse(((MockEmailService)Service.EMailService).EMailSent);
            });

            int count = Session.QueryOver<Message>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestSendMessageAfterRejectFriendship()
        {
            profiles[1].Settings.NotificationSocial = ProfileNotification.Message;
            profiles[0].Settings.NotificationSocial = ProfileNotification.Message;
            insertToDatabase(profiles[1]);
            insertToDatabase(profiles[0]);
            FriendInvitationDTO result = null;

            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(inviter, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                result = Service.InviteFriendOperation(data.Token, arg);
            });

            data = CreateNewSession(invited, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Operation = InviteFriendOperation.Accept;
                result = Service.InviteFriendOperation(data.Token, arg);
            });
            //reject friendship
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Operation = InviteFriendOperation.Reject;
                arg.Message = "test msg";
                result = Service.InviteFriendOperation(data.Token, arg);
                Assert.IsFalse(((MockEmailService)Service.EMailService).EMailSent);
            });

            int count = Session.QueryOver<Message>().RowCount();
            //one message from accept and second from reject friendship
            Assert.AreEqual(2, count);
            var message = Session.QueryOver<Message>().List();
            Assert.AreEqual(invited.GlobalId, message[0].Sender.GlobalId);
            Assert.AreEqual(MessagePriority.System, message[0].Priority);
            Assert.AreEqual(inviter.GlobalId, message[0].Receiver.GlobalId);

            Assert.AreEqual(MessagePriority.System, message[1].Priority);
            Assert.AreEqual(invited.GlobalId, message[1].Sender.GlobalId);
            Assert.AreEqual(inviter.GlobalId, message[1].Receiver.GlobalId);
            Assert.IsNotNull(message[1].Content);
        }

        [Test]
        public void TestSendEMailAfterRejectFriendship()
        {
            profiles[1].Settings.NotificationSocial = ProfileNotification.Email;
            profiles[0].Settings.NotificationSocial = ProfileNotification.Email;
            insertToDatabase(profiles[1]);
            insertToDatabase(profiles[0]);

            FriendInvitationDTO result = null;

            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(inviter, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                result = Service.InviteFriendOperation(data.Token, arg);
            });

            data = CreateNewSession(invited, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Operation = InviteFriendOperation.Accept;
                result = Service.InviteFriendOperation(data.Token, arg);
            });
            //reject friendship
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Operation = InviteFriendOperation.Reject;
                arg.Message = "test msg";
                result = Service.InviteFriendOperation(data.Token, arg);
                Assert.IsTrue(((MockEmailService)Service.EMailService).EMailSent);
            });

            int count = Session.QueryOver<Message>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestDisableSendMessageAfterRejectFriendship()
        {
            profiles[1].Settings.NotificationSocial = ProfileNotification.None;
            profiles[0].Settings.NotificationSocial = ProfileNotification.None;
            insertToDatabase(profiles[1]);
            insertToDatabase(profiles[0]);

            FriendInvitationDTO result = null;

            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(inviter, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                result = Service.InviteFriendOperation(data.Token, arg);
            });

            data = CreateNewSession(invited, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Operation = InviteFriendOperation.Accept;
                result = Service.InviteFriendOperation(data.Token, arg);
            });
            //reject friendship
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Operation = InviteFriendOperation.Reject;
                arg.Message = "test msg";
                result = Service.InviteFriendOperation(data.Token, arg);
                Assert.IsFalse(((MockEmailService)Service.EMailService).EMailSent);
            });

            int count = Session.QueryOver<Message>().RowCount();
            Assert.AreEqual(0, count);
        }
        

        [Test]
        [ExpectedException(typeof(UserDeletedException))]
        public void TestInviteAFriend_DeletedProfileException()
        {
            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[3].Tag;
            SessionData data = CreateNewSession(inviter, ClientInformation);
            FriendInvitationDTO result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                result = Service.InviteFriendOperation(data.Token, arg);
            });

        }

        [Test]
        public void TestInviteAFriend()
        {
            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(inviter, ClientInformation);
            FriendInvitationDTO result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                result = Service.InviteFriendOperation(data.Token, arg);
            });
            Assert.IsNotNull(result);
            Assert.AreEqual(InvitationType.Invite, result.InvitationType);
            int count=Session.QueryOver<FriendInvitation>().RowCount();
            Assert.AreEqual(1,count);
            var invitation =Session.Get<FriendInvitation>(new FriendInvitation()
                                              {
                                                  Inviter=profiles[0],
                                                  Invited = profiles[1]
                                              });
            Assert.IsNotNull(invitation);
            Assert.AreEqual(DateTime.UtcNow.Day,invitation.CreateDate.Day);
            Assert.AreEqual(FriendInvitationType.Invite, invitation.InvitationType);
        }


        [Test]
        public void TestCancelInvitation()
        {
            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[1].Tag;
            FriendInvitationDTO result = null;
            
            SessionData data = CreateNewSession(inviter, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                //result = Service.InviteFriendOperation(data.Token, invited, InviteFriendOperation.Invite);
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                result = Service.InviteFriendOperation(data.Token, arg);
            });
            Assert.IsNotNull(result);
            Assert.AreEqual(InvitationType.Invite, result.InvitationType);
            int count = Session.QueryOver<FriendInvitation>().RowCount();
            Assert.AreEqual(1, count);
            //now the same user wants to cancel the invitation (means delete invitation object from the db)
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Reject;
                result = Service.InviteFriendOperation(data.Token, arg);
            });
            Assert.IsNull(result);
            count = Session.QueryOver<FriendInvitation>().RowCount();
            Assert.AreEqual(0,count);
        }

        [Test]
        public void TestInviteAFriend_DuplicateInvitation()
        {
            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[1].Tag;
            FriendInvitationDTO result = null;
            
            SessionData data = CreateNewSession(inviter, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                //result = Service.InviteFriendOperation(data.Token, invited, InviteFriendOperation.Invite);
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                result = Service.InviteFriendOperation(data.Token, arg);
            });
            Assert.IsNotNull(result);
            Assert.AreEqual(InvitationType.Invite,result.InvitationType);
            int count = Session.QueryOver<FriendInvitation>().RowCount();
            Assert.AreEqual(1, count);
            //we send invitation second time - in the database still should be one invitation
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                //result = Service.InviteFriendOperation(data.Token, invited, InviteFriendOperation.Invite);
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                result = Service.InviteFriendOperation(data.Token, arg);
            });
            Assert.IsNull(result);
            count = Session.QueryOver<FriendInvitation>().RowCount();
            Assert.AreEqual(1, count);
            var invitation = Session.QueryOver<FriendInvitation>().SingleOrDefault();
            Assert.IsNotNull(invitation);
            Assert.AreEqual(FriendInvitationType.Invite, invitation.InvitationType);
        }

        [Test]
        public void TestInviteAFriend_AcceptInvitation_UsingInviteOperation()
        {
            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[1].Tag;
            FriendInvitationDTO result = null;
            
            SessionData data = CreateNewSession(inviter, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                //result = Service.InviteFriendOperation(data.Token, invited, InviteFriendOperation.Invite);
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                result = Service.InviteFriendOperation(data.Token, arg);
            });
            Assert.IsNotNull(result);
            Assert.AreEqual(InvitationType.Invite, result.InvitationType);

            //so now invited user invoke the same method but with reversed arguments (in this case invited became iviter)
            data = CreateNewSession(invited, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                //result = Service.InviteFriendOperation(data.Token, inviter, InviteFriendOperation.Invite);
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Operation = InviteFriendOperation.Invite;
                result = Service.InviteFriendOperation(data.Token, arg);
            });
            Assert.IsNull(result);

            int count = Session.QueryOver<FriendInvitation>().RowCount();
            var profileInviter = Session.Get<Profile>(inviter.GlobalId);
            var profileInvited = Session.Get<Profile>(invited.GlobalId);
            Assert.AreEqual(1,profileInviter.Friends.Count);
            Assert.AreEqual(1, profileInvited.Friends.Count);
            Assert.AreEqual(profileInvited,profileInviter.Friends.ElementAt(0));
            Assert.AreEqual(profileInviter, profileInvited.Friends.ElementAt(0));
            Assert.AreEqual(0,count);
        }

        [Test]
        public void TestInviteAFriend_AcceptInvitation_UsingAcceptOperation()
        {
            FriendInvitationDTO result = null;
            
            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(inviter, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                //result = Service.InviteFriendOperation(data.Token, invited, InviteFriendOperation.Invite);
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                result = Service.InviteFriendOperation(data.Token, arg);
            });
            Assert.IsNotNull(result);
            Assert.AreEqual(InvitationType.Invite, result.InvitationType);

            data = CreateNewSession(invited, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                //result = Service.InviteFriendOperation(data.Token, inviter, InviteFriendOperation.Accept);
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Operation = InviteFriendOperation.Accept;
                result = Service.InviteFriendOperation(data.Token, arg);
            });
            Assert.IsNull(result);

            int count = Session.QueryOver<FriendInvitation>().RowCount();
            var profileInviter = Session.Get<Profile>(inviter.GlobalId);
            var profileInvited = Session.Get<Profile>(invited.GlobalId);
            Assert.AreEqual(1, profileInviter.Friends.Count);
            Assert.AreEqual(1, profileInvited.Friends.Count);
            Assert.AreEqual(profileInvited, profileInviter.Friends.ElementAt(0));
            Assert.AreEqual(profileInviter, profileInvited.Friends.ElementAt(0));
            Assert.AreEqual(0, count);
        }

        [Test]
        [ExpectedException(typeof(ProfileAlreadyFriendException))]
        public void TestInviteAFriend_AlreadyFriendsException()
        {
            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(inviter, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                //Service.InviteFriendOperation(data.Token, invited, InviteFriendOperation.Invite);
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                Service.InviteFriendOperation(data.Token, arg);
            });
            //accept invitation
            data = CreateNewSession(invited, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                //Service.InviteFriendOperation(data.Token, inviter, InviteFriendOperation.Accept);
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Operation = InviteFriendOperation.Accept;
                Service.InviteFriendOperation(data.Token, arg);
            });

            //this should throw exception because those users are friends already
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                //Service.InviteFriendOperation(data.Token, inviter, InviteFriendOperation.Invite);
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Operation = InviteFriendOperation.Invite;
                Service.InviteFriendOperation(data.Token, arg);
            });
        }

        [Test]
        public void TestInviteAFriend_RejectInvitation()
        {
            FriendInvitationDTO result = null;
            
            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(inviter, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                //Service.InviteFriendOperation(data.Token, invited, InviteFriendOperation.Invite);
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                Service.InviteFriendOperation(data.Token, arg);
            });
            //so now invited user invoke the same method but with reversed arguments (in this case invited became iviter)
            data = CreateNewSession(invited, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                //result = Service.InviteFriendOperation(data.Token, inviter, InviteFriendOperation.Reject);
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Operation = InviteFriendOperation.Reject;
                result = Service.InviteFriendOperation(data.Token, arg);
            });
            Assert.IsNull(result);
            int count = Session.QueryOver<FriendInvitation>().RowCount();
            var profileInviter = Session.Get<Profile>(inviter.GlobalId);
            var profileInvited = Session.Get<Profile>(invited.GlobalId);
            Assert.AreEqual(0, profileInviter.Friends.Count);
            Assert.AreEqual(0, profileInvited.Friends.Count);
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestInviteAFriend_RejectFriendship()
        {
            FriendInvitationDTO result = null;
            
            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(inviter, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                //Service.InviteFriendOperation(data.Token, invited, InviteFriendOperation.Invite);
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                Service.InviteFriendOperation(data.Token, arg);
            });
            //so now invited user invoke the same method but with reversed arguments (in this case invited became iviter)
            data = CreateNewSession(invited, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                //Service.InviteFriendOperation(data.Token, inviter, InviteFriendOperation.Accept);
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Operation = InviteFriendOperation.Accept;
                Service.InviteFriendOperation(data.Token, arg);
            });
            int count = Session.QueryOver<FriendInvitation>().RowCount();
            var profileInviter = Session.Get<Profile>(inviter.GlobalId);
            var profileInvited = Session.Get<Profile>(invited.GlobalId);
            Assert.AreEqual(1, profileInviter.Friends.Count);
            Assert.AreEqual(1, profileInvited.Friends.Count);

            data = CreateNewSession(invited, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                //result = Service.InviteFriendOperation(data.Token, inviter, InviteFriendOperation.Reject);
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Operation = InviteFriendOperation.Reject;
                result = Service.InviteFriendOperation(data.Token, arg);
            });
            Assert.IsNull(result);
            count = Session.QueryOver<FriendInvitation>().RowCount();
            profileInviter = Session.Get<Profile>(inviter.GlobalId);
            profileInvited = Session.Get<Profile>(invited.GlobalId);
            Assert.AreEqual(0, profileInviter.Friends.Count);
            Assert.AreEqual(0, profileInvited.Friends.Count);
            Assert.AreEqual(0, count);
        }

        [Test]
        [ExpectedException(typeof(CannotAcceptRejectInvitationDoesntExistException))]
        public void TestInviteAFriend_RejectAfterRejectFriendship()
        {
            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(inviter, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                //Service.InviteFriendOperation(data.Token, invited, InviteFriendOperation.Invite);
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                Service.InviteFriendOperation(data.Token, arg);
            });
            data = CreateNewSession(invited, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                //Service.InviteFriendOperation(data.Token, inviter, InviteFriendOperation.Accept);
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Operation = InviteFriendOperation.Accept;
                Service.InviteFriendOperation(data.Token, arg);
            });
            data = CreateNewSession(invited, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                //Service.InviteFriendOperation(data.Token, inviter, InviteFriendOperation.Reject);
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Operation = InviteFriendOperation.Reject;
                Service.InviteFriendOperation(data.Token, arg);
            });
            data = CreateNewSession(inviter, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
             {
                 //Service.InviteFriendOperation(data.Token, invited, InviteFriendOperation.Reject);
                 var arg = new InviteFriendOperationData();
                 arg.User = invited;
                 arg.Operation = InviteFriendOperation.Reject;
                 Service.InviteFriendOperation(data.Token, arg);
             });

        }

        #region Statistics

        [Test]
        public void TestAcceptInvitation_Statistics()
        {
            FriendInvitationDTO result = null;

            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(inviter, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                //result = Service.InviteFriendOperation(data.Token, invited, InviteFriendOperation.Invite);
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                result = Service.InviteFriendOperation(data.Token, arg);
            });

            data = CreateNewSession(invited, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Operation = InviteFriendOperation.Accept;
                result = Service.InviteFriendOperation(data.Token, arg);
            });
            var dbProfile = Session.Get<Profile>(inviter.GlobalId);
            var test=dbProfile.Friends.Count;
            Assert.AreEqual(1, dbProfile.Statistics.FriendsCount);
            dbProfile = Session.Get<Profile>(invited.GlobalId);
            Assert.AreEqual(1, dbProfile.Statistics.FriendsCount);

            //now we reject the friendship
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                //result = Service.InviteFriendOperation(data.Token, inviter, InviteFriendOperation.Reject);
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Operation = InviteFriendOperation.Reject;
                result = Service.InviteFriendOperation(data.Token, arg);
            });

            dbProfile = Session.Get<Profile>(inviter.GlobalId);
            Assert.AreEqual(0, dbProfile.Statistics.FriendsCount);
            dbProfile = Session.Get<Profile>(invited.GlobalId);
            Assert.AreEqual(0, dbProfile.Statistics.FriendsCount);
        }

        #endregion
    }
}
