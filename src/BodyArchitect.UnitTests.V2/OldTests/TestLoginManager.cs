//using System;
//using System.Collections.Generic;
//using BodyArchitect.DataAccess;
//using NUnit.Framework;

//using BodyArchitect.Common;
//using BodyArchitect.Model;

//namespace BodyArchitect.UnitTests.V2
//{
//    [TestFixture]
//    public class TestLoginManager:LoginManager
//    {
//        private List<Profile> profiles=new List<Profile>();
//        private bool retrieveProfiles;
//        private bool createNewProfile;
//        private bool loginUser;
//        private Profile profileToLogin;
//        private Profile profileToCreate;

//        public TestLoginManager() : base()
//        {
//        }

//        [SetUp]
//        public void Setup()
//        {
//            reset();
//        }
//        private void reset()
//        {
//            profiles=new List<Profile>();
//            retrieveProfiles = false;
//            createNewProfile = false;
//            profileToLogin = null;
//            loginUser = false;
//            UserContext.Settings.AutoLoginProfileId = 0;
//            UserContext.CreateUserContext(null);
//        }
//        protected override IList<Profile> RetrieveProfiles()
//        {
//            retrieveProfiles = true;
//            return profiles;
//        }

//        protected override Profile CreateNewProfile()
//        {
//            createNewProfile = true;
//            return profileToCreate;
//        }

//        protected override Profile LoginUser()
//        {
//            loginUser = true;
//            return profileToLogin;
//        }
//        [Test]
//        public void TestOneProfileWithoutPassword()
//        {
//            TestLoginManager logn = this;
//            Profile profile = createProfile("romek", null);
//            Profile loggedProfile = logn.Login();
//            assertLoginResult(loggedProfile, profile,true,false,false);
//        }

//        [Test]
//        public void TestNoProfile()
//        {
//            TestLoginManager logn = this;
//            profileToCreate = createProfile("test", "test",false);
//            Profile loggedProfile = logn.Login();
//            assertLoginResult(loggedProfile, profileToCreate, true, true, false);
//        }

//        [Test]
//        public void TestOneProfileWithPassword()
//        {
//            TestLoginManager logn = this;
//            Profile profile = createProfile("romek", "test");
//            profileToLogin = profile;//we set that as login profile will be our tested profile
//            Profile loggedProfile = logn.Login();
//            assertLoginResult(loggedProfile, profile, true, false,true);
//        }

//        [Test]
//        public void TestOneProfileWithPasswordAndAutoLoginButProfilNotExists()
//        {
//            TestLoginManager logn = this;
//            Profile profile = createProfile("romek", "test");
//            UserContext.Settings.AutoLoginProfileId = 7;//non existing profile id
//            profileToLogin = profile;
//            Profile loggedProfile = logn.Login();
//            assertLoginResult(loggedProfile, profile, true, false, true);
//        }

//        [Test]
//        public void TestOneProfileWithPasswordAndAutoLogin()
//        {
//            TestLoginManager logn = this;
//            Profile profile = createProfile("romek", "test");
//            UserContext.Settings.AutoLoginProfileId = profile.GlobalId;
//            Profile loggedProfile = logn.Login();
//            assertLoginResult(loggedProfile, profile, true, false, false);
//        }

//        [Test]
//        public void TestTwoProfilesWithAutoLogin()
//        {
//            TestLoginManager logn = this;
//            Profile profile1 = createProfile("romek", null);
//            Profile profile2 = createProfile("witek", null);
//            UserContext.Settings.AutoLoginProfileId = profile2.Id;
//            Profile loggedProfile = logn.Login();
//            assertLoginResult(loggedProfile, profile2, true, false, false);
//        }

//        [Test]
//        public void TestTwoProfiles()
//        {
//            TestLoginManager logn = this;
//            Profile profile1 = createProfile("romek",null);
//            Profile profile2 = createProfile("witek", null);
//            profileToLogin = profile2;
//            Profile loggedProfile = logn.Login();
//            assertLoginResult(loggedProfile, profile2, true, false, true);
//        }

//        private Profile createProfile(string name, string password)
//        {
//            return createProfile(name, password, true);
//        }

//        private Profile createProfile(string name,string password,bool addToRetrieveList)
//        {
//            Profile profile1 = new Profile();
//            profile1.UserName = name;
//            profile1.Password = password;
//            if (addToRetrieveList)
//            {
//                profiles.Add(profile1);
//                profile1.Id = profiles.Count;
//            }
//            else
//            {
//                Random r= new Random();
//                profile1.Id = r.Next(100);
//            }
//            return profile1;
//        }

//        private void assertLoginResult(Profile loggedProfile, Profile profile, bool retrieve, bool createNew, bool login)
//        {
//            Assert.IsNotNull(loggedProfile);
//            Assert.AreEqual(loggedprofile.GlobalId, profile.GlobalId);
//            Assert.AreEqual(UserContext.Currentprofile.GlobalId, profile.GlobalId);
//            Assert.AreEqual(retrieve, retrieveProfiles);
//            Assert.AreEqual(createNew, createNewProfile);
//            Assert.AreEqual(login, loginUser);
//        }
//    }
//}
