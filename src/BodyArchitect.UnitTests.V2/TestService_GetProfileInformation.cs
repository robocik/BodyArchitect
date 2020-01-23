using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Payments;
using NUnit.Framework;
using AccountType = BodyArchitect.Service.V2.Model.AccountType;
using Privacy = BodyArchitect.Model.Privacy;

namespace BodyArchitect.UnitTests.V2
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

        private void setCalendarPrivacy(Privacy newPrivacy)
        {
            profiles[0].Privacy.CalendarView = newPrivacy;
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
            SessionData data = CreateNewSession(profile0, ClientInformation);
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
            SessionData data = CreateNewSession(profile1, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            criteria.UserId = profile0.GlobalId;
            ProfileInformationDTO profileInfo = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.IsTrue(UnitTestHelper.CompareDateTime(profile0.Birthday, profileInfo.Birthday.Value));

            data = CreateNewSession(profile2, ClientInformation);

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
            SessionData data = CreateNewSession(profile1, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            criteria.UserId = profile0.GlobalId;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.IsTrue(UnitTestHelper.CompareDateTime(profile0.Birthday, profileInfo.Birthday.Value));

            data = CreateNewSession(profile2, ClientInformation);

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
            SessionData data = CreateNewSession(profile1, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            criteria.UserId = profile0.GlobalId;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.AreEqual(null, profileInfo.Birthday);

            data = CreateNewSession(profile2, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.AreEqual(null, profileInfo.Birthday);
        }
        #endregion


        #region Licence

        [Test]
        public void TestLicence_Myselft_DifferenceBetweenCurrentAccountTypeAndAccountType()
        {
            profiles[0].Licence.BAPoints = 10;
            profiles[0].Licence.AccountType = Model.AccountType.PremiumUser;
            profiles[0].Licence.LastPointOperationDate = DateTime.UtcNow;
            insertToDatabase(profiles[0]);

            var profile0 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile0, ClientInformation);
            var securityInfo=SecurityManager.EnsureAuthentication(data.Token);
            securityInfo.Licence.CurrentAccountType = AccountType.Instructor;
            
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();


            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var profileInfo = service.GetProfileInformation(data.Token, criteria);
                Assert.AreEqual(AccountType.Instructor, profileInfo.Licence.CurrentAccountType);
                Assert.AreEqual(AccountType.PremiumUser, profileInfo.Licence.AccountType);
                Assert.AreEqual(10, profileInfo.Licence.BAPoints);
            });
        }

        [Test]
        public void TestLicence_Myselft_PaymentsData()
        {
            profiles[0].Licence.BAPoints = 10;
            profiles[0].Licence.LastPointOperationDate = DateTime.UtcNow;
            insertToDatabase(profiles[0]);

            var profile0 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile0, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();


            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var manager = new PaymentsManager();
                var accountTypes = new Dictionary<AccountType, PaymentAccountType>()
                                       {
                                           { AccountType.Instructor, new PaymentAccountType(){AccountType = AccountType.Instructor,Points = 3,PromotionPoints = 1,PromotionStartDate = DateTime.UtcNow} },
                                           { AccountType.PremiumUser, new PaymentAccountType(){AccountType = AccountType.PremiumUser,Points = 1 } },
                                       };
                var itemsToBuy = new Dictionary<string, int>();
                service.Configuration.Payments = manager.Load(accountTypes, itemsToBuy, 10);

                var profileInfo = service.GetProfileInformation(data.Token, criteria);
                Assert.AreEqual(10, profileInfo.Licence.Payments.Kara);
                Assert.AreEqual(3, profileInfo.Licence.Payments.AccountTypes[AccountType.Instructor].Points);
                Assert.AreEqual(1, profileInfo.Licence.Payments.AccountTypes[AccountType.Instructor].PromotionPoints);
                Assert.AreEqual(null, profileInfo.Licence.Payments.AccountTypes[AccountType.PremiumUser].PromotionPoints);
                Assert.AreEqual(1, profileInfo.Licence.Payments.AccountTypes[AccountType.PremiumUser].Points);
            });
        }

        [Test]
        public void TestLicence_Myselft()
        {
            profiles[0].Licence.BAPoints = 10;
            profiles[0].Licence.LastPointOperationDate = DateTime.UtcNow;
            insertToDatabase(profiles[0]);

            var profile0 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile0, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var profileInfo = Service.GetProfileInformation(data.Token, criteria);
                Assert.AreEqual(AccountType.Instructor, profileInfo.Licence.CurrentAccountType);
                Assert.AreEqual(AccountType.Instructor, profileInfo.Licence.AccountType);
                Assert.AreEqual(10, profileInfo.Licence.BAPoints);
            });
        }

        [Test]
        public void TestLicence_Others()
        {
            profiles[0].Licence.BAPoints = 10;
            profiles[0].Licence.AccountType = Model.AccountType.PremiumUser;
            profiles[0].Licence.LastPointOperationDate = DateTime.UtcNow;
            insertToDatabase(profiles[0]);

            var profile0 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile0, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            criteria.UserId = profiles[0].GlobalId;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var profileInfo = Service.GetProfileInformation(data.Token, criteria);
                Assert.IsNull(profileInfo.Licence);
            });
        }

        #endregion

        [Test]
        public void TestInvitations_Myselft()
        {
            FriendInvitation invitation = new FriendInvitation();
            invitation.Inviter = profiles[0];
            invitation.Invited = profiles[1];
            invitation.InvitationType = FriendInvitationType.Invite;
            invitation.CreateDate = DateTime.UtcNow;
            insertToDatabase(invitation);

            var profile0 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile0, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var profileInfo = Service.GetProfileInformation(data.Token, criteria);
                Assert.AreEqual(1, profileInfo.Invitations.Count);
            });

        }

        [Test]
        public void TestInvitations_Others()
        {
            FriendInvitation invitation = new FriendInvitation();
            invitation.Inviter = profiles[0];
            invitation.Invited = profiles[1];
            invitation.InvitationType = FriendInvitationType.Invite;
            invitation.CreateDate = DateTime.UtcNow;
            insertToDatabase(invitation);

            var profile0 = (ProfileDTO)profiles[1].Tag;
            var profile1 = (ProfileDTO)profiles[2].Tag;

            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            criteria.UserId = profiles[0].GlobalId;
            SessionData data = CreateNewSession(profile0, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var profileInfo = Service.GetProfileInformation(data.Token, criteria);
                Assert.AreEqual(0, profileInfo.Invitations.Count);
            });

            data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var profileInfo = Service.GetProfileInformation(data.Token, criteria);
                Assert.AreEqual(0, profileInfo.Invitations.Count);
            });

        }

        [Test]
        public void TestFavoriteUsers_Myselft()
        {
            profiles[0].FavoriteUsers.Add(profiles[1]);
            insertToDatabase(profiles[0]);

            var profile0 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile0, ClientInformation);
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
            criteria.UserId = profiles[0].GlobalId;
            SessionData data = CreateNewSession(profile0, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var profileInfo = Service.GetProfileInformation(data.Token, criteria);
                Assert.AreEqual(0, profileInfo.FavoriteUsers.Count);
            });

            data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var profileInfo = Service.GetProfileInformation(data.Token, criteria);
                Assert.AreEqual(0, profileInfo.FavoriteUsers.Count);
            });

        }

        
        [Test]
        public void TestFriends_Myselft()
        {
            var profile0 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile0, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var profileInfo = Service.GetProfileInformation(data.Token, criteria);
                Assert.AreEqual(1, profileInfo.Friends.Count);
            });

        }

        //[Test]
        //public void TestFriends_PrivacyPublic_Test()
        //{
        //    setFriendsPrivacy(Privacy.Public);

        //    var profile = CreateProfile(Session, "Profile12");
        //    profiles[0].Friends.Add(profile);

        //    profile = CreateProfile(Session, "Profile21");
        //    profiles[0].Friends.Add(profile);
        //    profile = CreateProfile(Session, "Profile22");
        //    profiles[0].Friends.Add(profile);
        //    insertToDatabase(profiles[0]);

        //    var profile0 = (ProfileDTO)profiles[0].Tag;
        //    var profile1 = (ProfileDTO)profiles[1].Tag;
        //    var profile2 = (ProfileDTO)profiles[2].Tag;
        //    SessionData data = CreateNewSession(profile1, ClientInformation);
        //    GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
        //    criteria.UserId = profile0.GlobalId;
        //    ProfileInformationDTO profileInfo = null;
        //    RunServiceMethod(delegate(InternalBodyArchitectService Service)
        //    {
        //        profileInfo = Service.GetProfileInformation(data.Token, criteria);
        //    });
        //    Assert.AreEqual(1, profileInfo.Friends.Count);

        //    data = CreateNewSession(profile2, ClientInformation);

        //    RunServiceMethod(delegate(InternalBodyArchitectService Service)
        //    {
        //        profileInfo = Service.GetProfileInformation(data.Token, criteria);
        //    });
        //    Assert.AreEqual(1, profileInfo.Friends.Count);
        //}

        [Test]
        public void TestFriends_PrivacyPublic()
        {
            setFriendsPrivacy(Privacy.Public);

            var profile0 = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            var profile2 = (ProfileDTO)profiles[2].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            criteria.UserId = profile0.GlobalId;
            ProfileInformationDTO profileInfo = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.AreEqual(1, profileInfo.Friends.Count);

            data = CreateNewSession(profile2, ClientInformation);

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
            SessionData data = CreateNewSession(profile1, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            criteria.UserId = profile0.GlobalId;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.AreEqual(1, profileInfo.Friends.Count);

            data = CreateNewSession(profile2, ClientInformation);

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
            SessionData data = CreateNewSession(profile1, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            criteria.UserId = profile0.GlobalId;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.AreEqual(0, profileInfo.Friends.Count);

            data = CreateNewSession(profile2, ClientInformation);

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
            SessionData data = CreateNewSession(profile0, ClientInformation);
            GetProfileInformationCriteria criteria=new GetProfileInformationCriteria();

            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var profileInfo = Service.GetProfileInformation(data.Token, criteria);
                Assert.IsNotNull(profileInfo.Wymiary);
                Assert.AreEqual(100, profileInfo.Wymiary.Height);
            });
            
        }

        [Test]
        public void TestWymiary_PrivacyFriendsOnly()
        {
            setSizesPrivacy(Privacy.FriendsOnly);
            ProfileInformationDTO profileInfo = null;
            var profile0 = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            var profile2 = (ProfileDTO)profiles[2].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            criteria.UserId = profile0.GlobalId;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.IsNotNull(profileInfo.Wymiary);
            Assert.AreEqual(100, profileInfo.Wymiary.Height);

            data = CreateNewSession(profile2, ClientInformation);

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
            SessionData data = CreateNewSession(profile1, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            criteria.UserId = profile0.GlobalId;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.IsNull(profileInfo.Wymiary);

            data = CreateNewSession(profile2, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.IsNull(profileInfo.Wymiary);
        }

        [Test]
        public void TestWymiary_PrivacyPublic()
        {
            setSizesPrivacy(Privacy.Public);
            
            var profile0 = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            var profile2 = (ProfileDTO)profiles[2].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            criteria.UserId = profile0.GlobalId;
            ProfileInformationDTO profileInfo = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.IsNotNull(profileInfo.Wymiary);
            Assert.AreEqual(100, profileInfo.Wymiary.Height);

            data = CreateNewSession(profile2, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.IsNotNull(profileInfo.Wymiary);
            Assert.AreEqual(100, profileInfo.Wymiary.Height);
        }

        

        [Test]
        public void DataInfo_Myselft()
        {
            var value = Guid.NewGuid();
            profiles[0].DataInfo.WorkoutPlanHash = value;
            insertToDatabase(profiles[0]);

            var profile0 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile0, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var profileInfo = Service.GetProfileInformation(data.Token, criteria);
                Assert.IsNotNull(profileInfo.DataInfo);
                Assert.AreEqual(value,profileInfo.DataInfo.WorkoutPlanHash);
            });
        }

        [Test]
        public void DataInfo_Others()
        {
            var value = Guid.NewGuid();
            profiles[0].DataInfo.WorkoutPlanHash = value;
            insertToDatabase(profiles[0]);

            var profile0 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile0, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            criteria.UserId = profiles[0].GlobalId;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var profileInfo = Service.GetProfileInformation(data.Token, criteria);
                Assert.IsNotNull(profileInfo.DataInfo);
                Assert.AreEqual(value, profileInfo.DataInfo.WorkoutPlanHash);
            });

        }

        [Test]
        public void IsOffline()
        {
            
            var profile0 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile0, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            criteria.UserId = profiles[0].GlobalId;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var profileInfo = Service.GetProfileInformation(data.Token, criteria);
                Assert.IsFalse(profileInfo.User.IsOnline);
            });

        }

        [Test]
        public void IsOnline()
        {
            CreateNewSession((ProfileDTO)profiles[1].Tag, ClientInformation);

            var profile0 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile0, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            criteria.UserId = profiles[1].GlobalId;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var profileInfo = Service.GetProfileInformation(data.Token, criteria);
                Assert.IsTrue(profileInfo.User.IsOnline);
            });

        }

        #region Records

        

        [Test]
        public void Records_SkipOtherUsersRecords()
        {
            var exercise = CreateExercise(Session, null, "ex1", "ex1");
            exercise.UseInRecords = true;
            insertToDatabase(exercise);
            var exercise1 = CreateExercise(Session, null, "ex2", "ex2");

            CreateExerciseRecord(exercise, profiles[0], new Tuple<int, decimal>(2, 50), DateTime.UtcNow.Date.AddDays(-3));
            CreateExerciseRecord(exercise, profiles[1], new Tuple<int, decimal>(2, 66), DateTime.UtcNow.Date.AddDays(-13));
            CreateExerciseRecord(exercise1, profiles[0], new Tuple<int, decimal>(12, 150), DateTime.UtcNow.Date.AddDays(-13));

            var profile0 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile0, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();


            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var profileInfo = service.GetProfileInformation(data.Token, criteria);
                Assert.AreEqual(1, profileInfo.Records.Count);
                Assert.AreEqual(null, profileInfo.Records[0].CustomerId);
                Assert.AreEqual(exercise.GlobalId, profileInfo.Records[0].Exercise.GlobalId);
                Assert.AreEqual(50, profileInfo.Records[0].MaxWeight);
                Assert.AreEqual(2, profileInfo.Records[0].Repetitions);
                Assert.AreEqual(DateTime.UtcNow.Date.AddDays(-3), profileInfo.Records[0].TrainingDate);
                Assert.AreNotEqual(Guid.Empty, profileInfo.Records[0].SerieId);
            });

        }

        [Test]
        public void Records_SkipCustomersRecords()
        {
            var exercise = CreateExercise(Session, null, "ex1", "ex1");
            exercise.UseInRecords = true;
            insertToDatabase(exercise);
            var exercise1 = CreateExercise(Session, null, "ex2", "ex2");
            var customer = CreateCustomer("cust",profiles[0]);

            CreateExerciseRecord(exercise, profiles[0], new Tuple<int, decimal>(2, 50), DateTime.UtcNow.Date.AddDays(-3));
            CreateExerciseRecord(exercise, profiles[0], new Tuple<int, decimal>(2, 66), DateTime.UtcNow.Date.AddDays(-13), customer);
            CreateExerciseRecord(exercise1, profiles[0], new Tuple<int, decimal>(12, 150), DateTime.UtcNow.Date.AddDays(-13));

            var profile0 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile0, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();


            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var profileInfo = service.GetProfileInformation(data.Token, criteria);
                Assert.AreEqual(1, profileInfo.Records.Count);
                Assert.AreEqual(null, profileInfo.Records[0].CustomerId);
                Assert.AreEqual(exercise.GlobalId, profileInfo.Records[0].Exercise.GlobalId);
                Assert.AreEqual(50, profileInfo.Records[0].MaxWeight);
                Assert.AreEqual(2, profileInfo.Records[0].Repetitions);
                Assert.AreEqual(DateTime.UtcNow.Date.AddDays(-3), profileInfo.Records[0].TrainingDate);
                Assert.AreNotEqual(Guid.Empty, profileInfo.Records[0].SerieId);
            });

        }

        [Test]
        public void Records_Myselft()
        {
            var exercise = CreateExercise(Session, null, "ex1", "ex1");
            exercise.UseInRecords = true;
            insertToDatabase(exercise);
            var exercise1 = CreateExercise(Session, null, "ex2", "ex2");

            CreateExerciseRecord(exercise, profiles[0], new Tuple<int, decimal>(2, 50), DateTime.UtcNow.Date.AddDays(-3));
            CreateExerciseRecord(exercise1, profiles[0], new Tuple<int, decimal>(12, 150), DateTime.UtcNow.Date.AddDays(-13));

            var profile0 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile0, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();


            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var profileInfo = service.GetProfileInformation(data.Token, criteria);
                Assert.AreEqual(1, profileInfo.Records.Count);
                Assert.AreEqual(null, profileInfo.Records[0].CustomerId);
                Assert.AreEqual(exercise.GlobalId, profileInfo.Records[0].Exercise.GlobalId);
                Assert.AreEqual(50, profileInfo.Records[0].MaxWeight);
                Assert.AreEqual(2, profileInfo.Records[0].Repetitions);
                Assert.AreEqual(DateTime.UtcNow.Date.AddDays(-3), profileInfo.Records[0].TrainingDate);
                Assert.AreNotEqual(Guid.Empty,profileInfo.Records[0].SerieId);
            });

        }

        [Test]
        public void Records_PrivacyPrivate()
        {
            setCalendarPrivacy(Privacy.Private);
            var exercise = CreateExercise(Session, null, "ex1", "ex1");
            exercise.UseInRecords = true;
            insertToDatabase(exercise);
            var exercise1 = CreateExercise(Session, null, "ex2", "ex2");

            CreateExerciseRecord(exercise, profiles[0], new Tuple<int, decimal>(2, 50), DateTime.UtcNow.Date.AddDays(-3));
            CreateExerciseRecord(exercise1, profiles[0], new Tuple<int, decimal>(12, 150), DateTime.UtcNow.Date.AddDays(-13));

            ProfileInformationDTO profileInfo = null;
            var profile0 = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            var profile2 = (ProfileDTO)profiles[2].Tag;

            SessionData data = CreateNewSession(profile1, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            criteria.UserId = profile0.GlobalId;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.AreEqual(0, profileInfo.Records.Count);

            data = CreateNewSession(profile2, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.AreEqual(0, profileInfo.Records.Count);

        }

        [Test]
        public void Records_PrivacyFriends()
        {
            setCalendarPrivacy(Privacy.FriendsOnly);
            var exercise = CreateExercise(Session, null, "ex1", "ex1");
            exercise.UseInRecords = true;
            insertToDatabase(exercise);
            var exercise1 = CreateExercise(Session, null, "ex2", "ex2");

            CreateExerciseRecord(exercise, profiles[0], new Tuple<int, decimal>(2, 50), DateTime.UtcNow.Date.AddDays(-3));
            CreateExerciseRecord(exercise1, profiles[0], new Tuple<int, decimal>(12, 150), DateTime.UtcNow.Date.AddDays(-13));

            ProfileInformationDTO profileInfo = null;
            var profile0 = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            var profile2 = (ProfileDTO)profiles[2].Tag;

            SessionData data = CreateNewSession(profile1, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            criteria.UserId = profile0.GlobalId;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.AreEqual(1, profileInfo.Records.Count);

            data = CreateNewSession(profile2, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.AreEqual(0, profileInfo.Records.Count);


        }

        [Test]
        public void Records_PrivacyPublic()
        {
            setCalendarPrivacy(Privacy.Public);
            var exercise = CreateExercise(Session, null, "ex1", "ex1");
            exercise.UseInRecords = true;
            insertToDatabase(exercise);
            var exercise1 = CreateExercise(Session, null, "ex2", "ex2");

            CreateExerciseRecord(exercise, profiles[0], new Tuple<int, decimal>(2, 50), DateTime.UtcNow.Date.AddDays(-3));
            CreateExerciseRecord(exercise1, profiles[0], new Tuple<int, decimal>(12, 150), DateTime.UtcNow.Date.AddDays(-13));

            ProfileInformationDTO profileInfo = null;
            var profile0 = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            var profile2 = (ProfileDTO)profiles[2].Tag;

            SessionData data = CreateNewSession(profile1, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            criteria.UserId = profile0.GlobalId;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.AreEqual(1, profileInfo.Records.Count);

            data = CreateNewSession(profile2, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.AreEqual(1, profileInfo.Records.Count);


        }
        #endregion
    }
}
