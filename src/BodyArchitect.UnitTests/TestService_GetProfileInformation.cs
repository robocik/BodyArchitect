using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service;
using BodyArchitect.Service.Model;
using NUnit.Framework;
using MessageType = BodyArchitect.Model.MessageType;
using Privacy = BodyArchitect.Model.Privacy;

namespace BodyArchitect.UnitTests
{
    [TestFixture]
    public class TestService_GetProfileInformation:TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();

        public override void BuildDatabase()
        {
            profiles.Clear();
            using (var tx = Session.BeginTransaction())
            {
                var profile = CreateProfile(Session, "Profile1");
                profile.Wymiary=new Wymiary();
                profile.Wymiary.Height = 100;
                Session.Save(profile.Wymiary);
                Session.SaveOrUpdate(profile.Wymiary);
                profiles.Add(profile);
                profile = CreateProfile(Session, "Profile2");
                profiles.Add(profile);
                profile = CreateProfile(Session, "Profile3");
                profiles.Add(profile);

                //set friendship
                profiles[0].Friends.Add(profiles[1]);
                profiles[1].Friends.Add(profiles[0]);
                Session.Update(profiles[0]);
                Session.Update(profiles[1]);

                tx.Commit();
            }
        }

        private void setSizesPrivacy(Privacy newPrivacy)
        {
            profiles[0].Privacy.Sizes = newPrivacy;
            Session.Update(profiles[0]);
            Session.Flush();
            Session.Clear();
        }

        private void setBirthdayPrivacy(Privacy newPrivacy)
        {
            profiles[0].Privacy.BirthdayDate = newPrivacy;
            Session.Update(profiles[0]);
            Session.Flush();
            Session.Clear();
        }

        private void setFriendsPrivacy(Privacy newPrivacy)
        {
            profiles[0].Privacy.Friends = newPrivacy;
            Session.Update(profiles[0]);
            Session.Flush();
            Session.Clear();
        }

        #region BirthdayDate

        [Test]
        public void TestBirthdayDate_Myselft()
        {
            var profile0 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile0, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var profileInfo = Service.GetProfileInformation(data.Token, criteria);
                Assert.IsTrue(UnitTestHelper.CompareDateTime(profile0.Birthday, profileInfo.Birthday.Value));
            });

        }

        [Test]
        public void TestBirthdayDate_PrivacyPublic()
        {
            setBirthdayPrivacy(Privacy.Public);

            var profile0 = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            var profile2 = (ProfileDTO)profiles[2].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            criteria.UserId = profile0.Id;
            ProfileInformationDTO profileInfo = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.IsTrue(UnitTestHelper.CompareDateTime(profile0.Birthday, profileInfo.Birthday.Value));

            data = SecurityManager.CreateNewSession(profile2, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.IsTrue(UnitTestHelper.CompareDateTime(profile0.Birthday, profileInfo.Birthday.Value));
        }

        [Test]
        public void TestBirthdayDate_PrivacyFriendsOnly()
        {
            setBirthdayPrivacy(Privacy.FriendsOnly);
            ProfileInformationDTO profileInfo = null;
            var profile0 = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            var profile2 = (ProfileDTO)profiles[2].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            criteria.UserId = profile0.Id;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.IsTrue(UnitTestHelper.CompareDateTime(profile0.Birthday, profileInfo.Birthday.Value));

            data = SecurityManager.CreateNewSession(profile2, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.AreEqual(null, profileInfo.Birthday);
        }

        [Test]
        public void TestBirthdayDate_PrivacyPrivate()
        {
            setBirthdayPrivacy(Privacy.Private);
            ProfileInformationDTO profileInfo = null;
            var profile0 = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            var profile2 = (ProfileDTO)profiles[2].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            criteria.UserId = profile0.Id;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.AreEqual(null, profileInfo.Birthday);

            data = SecurityManager.CreateNewSession(profile2, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.AreEqual(null, profileInfo.Birthday);
        }
        #endregion


        #region Messages

        [Test]
        public void TestMessages_MyselftAsSender()
        {
            Message msg = new Message();
            msg.MessageType = MessageType.InvitationAccepted;
            msg.Sender = profiles[0];
            msg.Receiver = profiles[1];
            insertToDatabase(msg);

            var profile0 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile0, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var profileInfo = Service.GetProfileInformation(data.Token, criteria);
                Assert.AreEqual(0, profileInfo.Messages.Count);
            });
        }

        [Test]
        public void TestMessages_MyselftAsRecevier()
        {
            Message msg = new Message();
            msg.MessageType = MessageType.InvitationAccepted;
            msg.Sender = profiles[1];
            msg.Receiver = profiles[0];
            insertToDatabase(msg);

            var profile0 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile0, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var profileInfo = Service.GetProfileInformation(data.Token, criteria);
                Assert.AreEqual(1, profileInfo.Messages.Count);
                Assert.AreEqual(profiles[1].Id, profileInfo.Messages[0].Sender.Id);
                Assert.AreEqual(profile0.Id, profileInfo.Messages[0].Receiver.Id);
            });
        }

        [Test]
        public void TestMessages_Other()
        {
            Message msg = new Message();
            msg.MessageType = MessageType.InvitationAccepted;
            msg.Sender = profiles[1];
            msg.Receiver = profiles[0];
            insertToDatabase(msg);

            var profile0 = (ProfileDTO)profiles[2].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile0, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            criteria.UserId = profiles[0].Id;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var profileInfo = Service.GetProfileInformation(data.Token, criteria);
                Assert.AreEqual(0, profileInfo.Messages.Count);
            });
        }

        #endregion

        [Test]
        public void TestInvitations_Myselft()
        {
            FriendInvitation invitation = new FriendInvitation();
            invitation.Inviter = profiles[0];
            invitation.Invited = profiles[1];
            invitation.InvitationType = FriendInvitationType.Invitation;
            invitation.CreateDate = DateTime.UtcNow;
            insertToDatabase(invitation);

            var profile0 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile0, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var profileInfo = Service.GetProfileInformation(data.Token, criteria);
                Assert.AreEqual(1, profileInfo.Invitations.Count);
            });

        }

        [Test]
        public void TestFavoriteUsers_Myselft()
        {
            profiles[0].FavoriteUsers.Add(profiles[1]);
            insertToDatabase(profiles[0]);

            var profile0 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile0, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var profileInfo = Service.GetProfileInformation(data.Token, criteria);
                Assert.AreEqual(1, profileInfo.FavoriteUsers.Count);
            });
        }

        [Test]
        public void TestFavoritesUsers_Others()
        {
            profiles[0].FavoriteUsers.Add(profiles[1]);
            insertToDatabase(profiles[0]);
            var profile0 = (ProfileDTO)profiles[1].Tag;
            var profile1 = (ProfileDTO)profiles[2].Tag;

            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            criteria.UserId = profiles[0].Id;
            SessionData data = SecurityManager.CreateNewSession(profile0, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var profileInfo = Service.GetProfileInformation(data.Token, criteria);
                Assert.AreEqual(0, profileInfo.FavoriteUsers.Count);
            });

            data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var profileInfo = Service.GetProfileInformation(data.Token, criteria);
                Assert.AreEqual(0, profileInfo.FavoriteUsers.Count);
            });

        }

        [Test]
        public void TestInvitations_Others()
        {
            FriendInvitation invitation = new FriendInvitation();
            invitation.Inviter = profiles[0];
            invitation.Invited = profiles[1];
            invitation.InvitationType = FriendInvitationType.Invitation;
            invitation.CreateDate = DateTime.UtcNow;
            insertToDatabase(invitation);

            var profile0 = (ProfileDTO)profiles[1].Tag;
            var profile1 = (ProfileDTO)profiles[2].Tag;
            
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            criteria.UserId = profiles[0].Id;
            SessionData data = SecurityManager.CreateNewSession(profile0, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var profileInfo = Service.GetProfileInformation(data.Token, criteria);
                Assert.AreEqual(0, profileInfo.Invitations.Count);
            });

            data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var profileInfo = Service.GetProfileInformation(data.Token, criteria);
                Assert.AreEqual(0, profileInfo.Invitations.Count);
            });

        }

        [Test]
        public void TestFriends_Myselft()
        {
            var profile0 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile0, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var profileInfo = Service.GetProfileInformation(data.Token, criteria);
                Assert.AreEqual(1, profileInfo.Friends.Count);
            });

        }

        [Test]
        public void TestFriends_PrivacyPublic()
        {
            setFriendsPrivacy(Privacy.Public);

            var profile0 = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            var profile2 = (ProfileDTO)profiles[2].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            criteria.UserId = profile0.Id;
            ProfileInformationDTO profileInfo = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.AreEqual(1, profileInfo.Friends.Count);

            data = SecurityManager.CreateNewSession(profile2, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.AreEqual(1, profileInfo.Friends.Count);
        }

        [Test]
        public void TestFriends_PrivacyFriendsOnly()
        {
            setFriendsPrivacy(Privacy.FriendsOnly);
            ProfileInformationDTO profileInfo = null;
            var profile0 = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            var profile2 = (ProfileDTO)profiles[2].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            criteria.UserId = profile0.Id;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.AreEqual(1, profileInfo.Friends.Count);

            data = SecurityManager.CreateNewSession(profile2, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.AreEqual(0, profileInfo.Friends.Count);
        }

        [Test]
        public void TestFriends_PrivacyPrivate()
        {
            setFriendsPrivacy(Privacy.Private);
            ProfileInformationDTO profileInfo = null;
            var profile0 = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            var profile2 = (ProfileDTO)profiles[2].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            criteria.UserId = profile0.Id;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.AreEqual(0, profileInfo.Friends.Count);

            data = SecurityManager.CreateNewSession(profile2, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.AreEqual(0, profileInfo.Friends.Count);
        }

        [Test]
        public void TestWymiary_Myselft()
        {
            var profile0 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile0, ClientInformation);
            GetProfileInformationCriteria criteria=new GetProfileInformationCriteria();

            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var profileInfo = Service.GetProfileInformation(data.Token, criteria);
                Assert.IsNotNull(profileInfo.Wymiary);
                Assert.AreEqual(100, profileInfo.Wymiary.Height);
            });
            
        }

        [Test]
        public void TestWymiary_PrivacyPublic()
        {
            setSizesPrivacy(Privacy.Public);
            
            var profile0 = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            var profile2 = (ProfileDTO)profiles[2].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            criteria.UserId = profile0.Id;
            ProfileInformationDTO profileInfo = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.IsNotNull(profileInfo.Wymiary);
            Assert.AreEqual(100, profileInfo.Wymiary.Height);

            data = SecurityManager.CreateNewSession(profile2, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.IsNotNull(profileInfo.Wymiary);
            Assert.AreEqual(100, profileInfo.Wymiary.Height);
        }

        [Test]
        public void TestWymiary_PrivacyFriendsOnly()
        {
            setSizesPrivacy(Privacy.FriendsOnly);
            ProfileInformationDTO profileInfo = null;
            var profile0 = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            var profile2 = (ProfileDTO)profiles[2].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            criteria.UserId = profile0.Id;
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.IsNotNull(profileInfo.Wymiary);
            Assert.AreEqual(100, profileInfo.Wymiary.Height);

            data = SecurityManager.CreateNewSession(profile2, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.IsNull(profileInfo.Wymiary);
        }

        [Test]
        public void TestWymiary_PrivacyPrivate()
        {
            setSizesPrivacy(Privacy.Private);
            ProfileInformationDTO profileInfo = null;
            var profile0 = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            var profile2 = (ProfileDTO)profiles[2].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            criteria.UserId = profile0.Id;
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.IsNull(profileInfo.Wymiary);

            data = SecurityManager.CreateNewSession(profile2, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.IsNull(profileInfo.Wymiary);
        }
    }
}
