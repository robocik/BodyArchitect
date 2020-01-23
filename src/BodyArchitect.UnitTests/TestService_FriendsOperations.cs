using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service;
using BodyArchitect.Service.Model;
using BodyArchitect.Shared;
using NUnit.Framework;
using MessagePriority = BodyArchitect.Model.MessagePriority;
using MessageType = BodyArchitect.Model.MessageType;

namespace BodyArchitect.UnitTests
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

            SessionData data = SecurityManager.CreateNewSession(inviter, ClientInformation);
            FriendInvitationDTO result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                result = service.InviteFriendOperation(data.Token, arg);
            });

            data = SecurityManager.CreateNewSession(invited, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Operation = InviteFriendOperation.Accept;
                result = service.InviteFriendOperation(data.Token, arg);
            });
            var dbProfile = Session.Get<Profile>(inviter.Id);
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

            SessionData data = SecurityManager.CreateNewSession(inviter, ClientInformation);
            FriendInvitationDTO result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                result = service.InviteFriendOperation(data.Token, arg);
            });

            data = SecurityManager.CreateNewSession(invited, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Operation = InviteFriendOperation.Accept;
                result = service.InviteFriendOperation(data.Token, arg);
            });
            var dbProfile = Session.Get<Profile>(invited.Id);
            Assert.AreEqual(1, dbProfile.Friends.Count);
            Assert.AreEqual(0, dbProfile.FavoriteUsers.Count);
        }

        [Test]
        public void TestSendMessageAfterAccept()
        {
            FriendInvitationDTO result = null;

            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(inviter, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                result = Service.InviteFriendOperation(data.Token, arg);
            });
            
            data = SecurityManager.CreateNewSession(invited, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Message = "test msg";
                arg.Operation = InviteFriendOperation.Accept;
                result = Service.InviteFriendOperation(data.Token, arg);
            });
            
            int count = Session.QueryOver<Message>().RowCount();
            Assert.AreEqual(1,count);
            var message=Session.QueryOver<Message>().SingleOrDefault();
            Assert.AreEqual(MessageType.InvitationAccepted,message.MessageType);
            Assert.AreEqual(invited.Id, message.Sender.Id);
            Assert.AreEqual(inviter.Id, message.Receiver.Id);
            Assert.AreEqual(MessagePriority.System, message.Priority);
            Assert.AreEqual("test msg", message.Content);
        }

        [Test]
        public void TestSendMessageAfterRejectInvitation()
        {
            FriendInvitationDTO result = null;

            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(inviter, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                result = Service.InviteFriendOperation(data.Token, arg);
            });

            data = SecurityManager.CreateNewSession(invited, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Operation = InviteFriendOperation.Reject;
                arg.Message = "test msg";
                result = Service.InviteFriendOperation(data.Token, arg);
            });

            int count = Session.QueryOver<Message>().RowCount();
            Assert.AreEqual(1, count);
            var message = Session.QueryOver<Message>().SingleOrDefault();
            Assert.AreEqual(MessageType.InvitationRejected, message.MessageType);
            Assert.AreEqual("test msg", message.Content);
            Assert.AreEqual(MessagePriority.System, message.Priority);
            Assert.AreEqual(invited.Id, message.Sender.Id);
            Assert.AreEqual(inviter.Id, message.Receiver.Id);
        }

        [Test]
        public void TestSendMessageAfterRejectFriendship()
        {
            FriendInvitationDTO result = null;

            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(inviter, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                result = Service.InviteFriendOperation(data.Token, arg);
            });

            data = SecurityManager.CreateNewSession(invited, ClientInformation);

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
            });

            int count = Session.QueryOver<Message>().RowCount();
            //one message from accept and second from reject friendship
            Assert.AreEqual(2, count);
            var message = Session.QueryOver<Message>().List();
            Assert.AreEqual(MessageType.InvitationAccepted, message[0].MessageType);
            Assert.AreEqual(invited.Id, message[0].Sender.Id);
            Assert.AreEqual(MessagePriority.System, message[0].Priority);
            Assert.AreEqual(inviter.Id, message[0].Receiver.Id);

            Assert.AreEqual(MessageType.FriendshipRejected, message[1].MessageType);
            Assert.AreEqual(MessagePriority.System, message[1].Priority);
            Assert.AreEqual(invited.Id, message[1].Sender.Id);
            Assert.AreEqual(inviter.Id, message[1].Receiver.Id);
            Assert.AreEqual("test msg", message[1].Content);
        }
        [Test]
        public void TestGetProfileInformations_WithRejectedFriendship()
        {
            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(inviter, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                Service.InviteFriendOperation(data.Token,arg);
            });
            
            data = SecurityManager.CreateNewSession(invited, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Operation = InviteFriendOperation.Accept;
                Service.InviteFriendOperation(data.Token, arg);

            });
            int count = Session.QueryOver<FriendInvitation>().RowCount();
            Assert.AreEqual(0, count);
            //reject friendship
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Operation = InviteFriendOperation.Reject;
                Service.InviteFriendOperation(data.Token, arg);
            });
            count = Session.QueryOver<FriendInvitation>().RowCount();
            Assert.AreEqual(0, count);

            GetProfileInformationCriteria criteria=new GetProfileInformationCriteria();
            ProfileInformationDTO info = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                info = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.AreEqual(0, info.Messages.Count);
            data = SecurityManager.CreateNewSession(inviter, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                info = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.AreEqual(2, info.Messages.Count);
        }

        [Test]
        public void TestGetProfileInformations_WithRejectedInvitation()
        {
            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(inviter, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                Service.InviteFriendOperation(data.Token, arg);
            });
            data = SecurityManager.CreateNewSession(invited, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Operation = InviteFriendOperation.Reject;
                Service.InviteFriendOperation(data.Token, arg);
            });
            int count = Session.QueryOver<FriendInvitation>().RowCount();
            Assert.AreEqual(0, count);

            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();

            ProfileInformationDTO info = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                info = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.AreEqual(0,info.Messages.Count);

            data = SecurityManager.CreateNewSession(inviter, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                info = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.AreEqual(1, info.Messages.Count);
        }

        [Test]
        [ExpectedException(typeof(UserDeletedException))]
        public void TestInviteAFriend_DeletedProfileException()
        {
            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[3].Tag;
            SessionData data = SecurityManager.CreateNewSession(inviter, ClientInformation);
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
            SessionData data = SecurityManager.CreateNewSession(inviter, ClientInformation);
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
            Assert.AreEqual(DateTime.Now.Day,invitation.CreateDate.Day);
            Assert.AreEqual(FriendInvitationType.Invitation, invitation.InvitationType);
        }


        [Test]
        public void TestCancelInvitation()
        {
            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[1].Tag;
            FriendInvitationDTO result = null;
            
            SessionData data = SecurityManager.CreateNewSession(inviter, ClientInformation);

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
            
            SessionData data = SecurityManager.CreateNewSession(inviter, ClientInformation);
            
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
            Assert.AreEqual(FriendInvitationType.Invitation, invitation.InvitationType);
        }

        [Test]
        public void TestInviteAFriend_AcceptInvitation_UsingInviteOperation()
        {
            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[1].Tag;
            FriendInvitationDTO result = null;
            
            SessionData data = SecurityManager.CreateNewSession(inviter, ClientInformation);
            
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
            data = SecurityManager.CreateNewSession(invited, ClientInformation);
            
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
            var profileInviter=Session.Get<Profile>(inviter.Id);
            var profileInvited = Session.Get<Profile>(invited.Id);
            Assert.AreEqual(1,profileInviter.Friends.Count);
            Assert.AreEqual(1, profileInvited.Friends.Count);
            Assert.AreEqual(profileInvited,profileInviter.Friends[0]);
            Assert.AreEqual(profileInviter, profileInvited.Friends[0]);
            Assert.AreEqual(0,count);
        }

        [Test]
        public void TestInviteAFriend_AcceptInvitation_UsingAcceptOperation()
        {
            FriendInvitationDTO result = null;
            
            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(inviter, ClientInformation);
            
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

            data = SecurityManager.CreateNewSession(invited, ClientInformation);
            
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
            var profileInviter = Session.Get<Profile>(inviter.Id);
            var profileInvited = Session.Get<Profile>(invited.Id);
            Assert.AreEqual(1, profileInviter.Friends.Count);
            Assert.AreEqual(1, profileInvited.Friends.Count);
            Assert.AreEqual(profileInvited, profileInviter.Friends[0]);
            Assert.AreEqual(profileInviter, profileInvited.Friends[0]);
            Assert.AreEqual(0, count);
        }

        [Test]
        [ExpectedException(typeof(ProfileAlreadyFriendException))]
        public void TestInviteAFriend_AlreadyFriendsException()
        {
            ProfileDTO inviter = (ProfileDTO)profiles[0].Tag;
            ProfileDTO invited = (ProfileDTO)profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(inviter, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                //Service.InviteFriendOperation(data.Token, invited, InviteFriendOperation.Invite);
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                Service.InviteFriendOperation(data.Token, arg);
            });
            //accept invitation
            data = SecurityManager.CreateNewSession(invited, ClientInformation);
            
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
            SessionData data = SecurityManager.CreateNewSession(inviter, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                //Service.InviteFriendOperation(data.Token, invited, InviteFriendOperation.Invite);
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                Service.InviteFriendOperation(data.Token, arg);
            });
            //so now invited user invoke the same method but with reversed arguments (in this case invited became iviter)
            data = SecurityManager.CreateNewSession(invited, ClientInformation);
            
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
            var profileInviter = Session.Get<Profile>(inviter.Id);
            var profileInvited = Session.Get<Profile>(invited.Id);
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
            SessionData data = SecurityManager.CreateNewSession(inviter, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                //Service.InviteFriendOperation(data.Token, invited, InviteFriendOperation.Invite);
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                Service.InviteFriendOperation(data.Token, arg);
            });
            //so now invited user invoke the same method but with reversed arguments (in this case invited became iviter)
            data = SecurityManager.CreateNewSession(invited, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                //Service.InviteFriendOperation(data.Token, inviter, InviteFriendOperation.Accept);
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Operation = InviteFriendOperation.Accept;
                Service.InviteFriendOperation(data.Token, arg);
            });
            int count = Session.QueryOver<FriendInvitation>().RowCount();
            var profileInviter = Session.Get<Profile>(inviter.Id);
            var profileInvited = Session.Get<Profile>(invited.Id);
            Assert.AreEqual(1, profileInviter.Friends.Count);
            Assert.AreEqual(1, profileInvited.Friends.Count);

            data = SecurityManager.CreateNewSession(invited, ClientInformation);
            
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
            profileInviter = Session.Get<Profile>(inviter.Id);
            profileInvited = Session.Get<Profile>(invited.Id);
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
            SessionData data = SecurityManager.CreateNewSession(inviter, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                //Service.InviteFriendOperation(data.Token, invited, InviteFriendOperation.Invite);
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                Service.InviteFriendOperation(data.Token, arg);
            });
            data = SecurityManager.CreateNewSession(invited, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                //Service.InviteFriendOperation(data.Token, inviter, InviteFriendOperation.Accept);
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Operation = InviteFriendOperation.Accept;
                Service.InviteFriendOperation(data.Token, arg);
            });
            data = SecurityManager.CreateNewSession(invited, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                //Service.InviteFriendOperation(data.Token, inviter, InviteFriendOperation.Reject);
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Operation = InviteFriendOperation.Reject;
                Service.InviteFriendOperation(data.Token, arg);
            });
            data = SecurityManager.CreateNewSession(inviter, ClientInformation);
            
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
            SessionData data = SecurityManager.CreateNewSession(inviter, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                //result = Service.InviteFriendOperation(data.Token, invited, InviteFriendOperation.Invite);
                var arg = new InviteFriendOperationData();
                arg.User = invited;
                arg.Operation = InviteFriendOperation.Invite;
                result = Service.InviteFriendOperation(data.Token, arg);
            });

            data = SecurityManager.CreateNewSession(invited, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var arg = new InviteFriendOperationData();
                arg.User = inviter;
                arg.Operation = InviteFriendOperation.Accept;
                result = Service.InviteFriendOperation(data.Token, arg);
            });
            var dbProfile = Session.Get<Profile>(inviter.Id);
            Assert.AreEqual(1, dbProfile.Statistics.FriendsCount);
            dbProfile = Session.Get<Profile>(invited.Id);
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

            dbProfile = Session.Get<Profile>(inviter.Id);
            Assert.AreEqual(0, dbProfile.Statistics.FriendsCount);
            dbProfile = Session.Get<Profile>(invited.Id);
            Assert.AreEqual(0, dbProfile.Statistics.FriendsCount);
        }

        #endregion
    }
}
