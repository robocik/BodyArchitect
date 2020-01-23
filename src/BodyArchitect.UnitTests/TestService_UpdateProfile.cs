using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BodyArchitect.Common;
using BodyArchitect.Model;
using BodyArchitect.Service;
using BodyArchitect.Service.Model;
using BodyArchitect.Shared;
using NHibernate;
using NUnit.Framework;
using Gender = BodyArchitect.Service.Model.Gender;
using Privacy = BodyArchitect.Service.Model.Privacy;
using Role = BodyArchitect.Service.Model.Role;

namespace BodyArchitect.UnitTests
{
    [TestFixture]
    public class TestService_UpdateProfile:TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();

        public override void BuildDatabase()
        {
            profiles.Clear();
            using (var tx = Session.BeginTransaction())
            {
                var profile = CreateProfile(Session, "Profile1");
                profile.Wymiary = new Wymiary();
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

                UnitTestHelper.SetProperty(profiles[0].Tag, "Version", Session.Get<Profile>(profiles[0].Id).Version);
                UnitTestHelper.SetProperty(profiles[1].Tag, "Version", Session.Get<Profile>(profiles[1].Id).Version);
            }
        }

        [Test]
        public void Test_RemoveImage()
        {
            InternalBodyArchitectService tmp = new InternalBodyArchitectService(Session);
            string folder = tmp.ImagesFolder;
            Guid pictureId = Guid.NewGuid();
            File.CreateText(Path.Combine(folder, pictureId.ToString())).Close();
            profiles[0].Picture = new Picture(pictureId,"dfgdfgdfg");
            insertToDatabase(profiles[0]);

            UnitTestHelper.SetProperty(profiles[0].Tag, "Version", profiles[0].Version);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
                                 {
                                     
                                    ProfileUpdateData d = new ProfileUpdateData();
                                    d.Profile = profile;
                                     d.Profile.Picture = null;
                                    service.UpdateProfile(data.Token, d);
                                });
            Assert.IsFalse(File.Exists(Path.Combine(folder, pictureId.ToString())));
        }

        [Test]
        public void Test_ShouldntRemoveImage()
        {
            InternalBodyArchitectService tmp = new InternalBodyArchitectService(Session);
            string folder = tmp.ImagesFolder;
            Guid pictureId = Guid.NewGuid();
            File.CreateText(Path.Combine(folder, pictureId.ToString())).Close();
            profiles[0].Picture = new Picture(pictureId, "dfgdfgdfg");
            insertToDatabase(profiles[0]);

            UnitTestHelper.SetProperty(profiles[0].Tag, "Version", profiles[0].Version);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {

                ProfileUpdateData d = new ProfileUpdateData();
                d.Profile = profile;
                d.Profile.Picture = new PictureInfoDTO(pictureId,"fdgdf");
                service.UpdateProfile(data.Token, d);
            });
            Assert.IsTrue(File.Exists(Path.Combine(folder, pictureId.ToString())));
        }

        [Test]
        public void TestUpdateProfile_ExceptionDuringRemoveImage()
        {
            InternalBodyArchitectService tmp = new InternalBodyArchitectService(Session);
            string folder = tmp.ImagesFolder;
            Guid pictureId = Guid.NewGuid();
            var file=File.CreateText(Path.Combine(folder, pictureId.ToString()));
            profiles[0].Picture = new Picture(pictureId, "dfgdfgdfg");
            insertToDatabase(profiles[0]);
            UnitTestHelper.SetProperty(profiles[0].Tag, "Version", profiles[0].Version);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {

                ProfileUpdateData d = new ProfileUpdateData();
                d.Profile = profile;
                d.Profile.Picture = null;
                d.Profile.Gender = Gender.Male;
                service.UpdateProfile(data.Token, d);
            });
            Assert.IsTrue(File.Exists(Path.Combine(folder, pictureId.ToString())));
            var dbProfile = Session.Get<Profile>(profile.Id);
            Assert.AreEqual(Model.Gender.Male,dbProfile.Gender);
            file.Close();
        }

        [Test]
        //[ExpectedException(typeof(HibernateException))]
        public void Test_ChangeWymiary_WithNewInstance()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ProfileUpdateData d = new ProfileUpdateData();
                d.Profile = profile;
                d.Wymiary=new WymiaryDTO();
                d.Wymiary.Klatka = 11;
                Service.UpdateProfile(data.Token, d);
            });
            var dbProfile = Session.Get<Profile>(profile.Id);
            Assert.IsNotNull(dbProfile.Wymiary);
            Assert.AreEqual(11, dbProfile.Wymiary.Klatka);

            Assert.AreEqual(1, Session.QueryOver<Wymiary>().RowCount());
        }

        [Test]
        public void Test_ChangeWymiary()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ProfileUpdateData d = new ProfileUpdateData();
                d.Profile = profile;
                d.Wymiary = new WymiaryDTO();
                d.Wymiary.Id = profiles[0].Wymiary.Id;
                d.Wymiary.Klatka = 101;
                Service.UpdateProfile(data.Token, d);
            });
            var dbProfile = Session.Get<Profile>(profile.Id);
            Assert.IsNotNull(dbProfile.Wymiary);
            Assert.AreEqual(101, dbProfile.Wymiary.Klatka);

            Assert.AreEqual(1, Session.QueryOver<Wymiary>().RowCount());
        }

        [Test]
        public void Test_ChangeWymiary_AutomaticUpdateMeasurements()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ProfileUpdateData d = new ProfileUpdateData();
                d.Profile = profile;
                d.Wymiary = new WymiaryDTO();
                d.Wymiary.Id = profiles[0].Wymiary.Id;
                d.Wymiary.Klatka = 101;
                profile=Service.UpdateProfile(data.Token, d);
            });
            profile.Settings.AutomaticUpdateMeasurements = true;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ProfileUpdateData d = new ProfileUpdateData();
                d.Profile = profile;
                profile=Service.UpdateProfile(data.Token, d);
            });
            //this operation shouldn't change the measurements because of automatic update enabled
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ProfileUpdateData d = new ProfileUpdateData();
                d.Profile = profile;
                d.Wymiary = new WymiaryDTO();
                d.Wymiary.Id = profiles[0].Wymiary.Id;
                d.Wymiary.Klatka = 200;
                Service.UpdateProfile(data.Token, d);
            });
            var dbProfile = Session.Get<Profile>(profile.Id);
            Assert.IsNotNull(dbProfile.Wymiary);
            Assert.AreEqual(101, dbProfile.Wymiary.Klatka);

            Assert.AreEqual(1, Session.QueryOver<Wymiary>().RowCount());
        }


        [Test]
        public void Test_ChangeProfileData()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            DateTime time = DateTime.Now;

            profile.Birthday = time;
            profile.Gender = Gender.Male;
            profile.AboutInformation = "testInfo";
            profile.Privacy.CalendarView = Privacy.FriendsOnly;
            profile.Privacy.Sizes = Privacy.Public;
            profile.Privacy.Friends = Privacy.Public;
            profile.Picture=new PictureInfoDTO();
            profile.Picture.Hash = "testHash";
            Guid testPictureId = Guid.NewGuid();
            profile.Picture.PictureId = testPictureId;
            profile.CountryId = 6;
            
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ProfileUpdateData d = new ProfileUpdateData();
                d.Profile = profile;
                Service.UpdateProfile(data.Token, d);
            });

            var dbProfile = Session.Get<Profile>(profile.Id);
            Assert.AreEqual(time.Date, dbProfile.Birthday.Date);
            Assert.AreEqual(Model.Gender.Male, dbProfile.Gender);
            Assert.AreEqual("testInfo", dbProfile.AboutInformation);
            Assert.AreEqual(Model.Privacy.FriendsOnly, dbProfile.Privacy.CalendarView);
            Assert.AreEqual(Model.Privacy.Public, dbProfile.Privacy.Sizes);
            Assert.AreEqual(Model.Privacy.Public, dbProfile.Privacy.Friends);
            Assert.AreEqual("testHash", profile.Picture.Hash);
            Assert.AreEqual(testPictureId, profile.Picture.PictureId);
            Assert.AreEqual(6, profile.CountryId);
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void Test_ChangeEMail_Exception()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ProfileUpdateData d = new ProfileUpdateData();
                d.Profile = profile.Clone();
                d.Profile.Email = "gfdgdfg@fgfdg.pl";
                service.UpdateProfile(data.Token, d);
            });
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void Test_ChangeRole_Exception()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ProfileUpdateData d = new ProfileUpdateData();
                d.Profile = profile.Clone();
                UnitTestHelper.SetProperty(d.Profile, "Role", Role.Administrator);
                service.UpdateProfile(data.Token, d);
            });
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void Test_ChangeUserName_Exception()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ProfileUpdateData d = new ProfileUpdateData();
                d.Profile = profile.Clone();
                d.Profile.UserName = "gfdgdfpl";
                service.UpdateProfile(data.Token, d);
            });
        }

        [Test]
        public void Test_Bug_LoosingAllCollections()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            DateTime time = DateTime.Now;

            profile.Birthday = time;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
                                 {
                                     ProfileUpdateData d = new ProfileUpdateData();
                                     d.Profile = profile;
                                     Service.UpdateProfile(data.Token, d);
                                 });
            var dbProfile=Session.Get<Profile>(profile.Id);
            Assert.AreEqual(1,dbProfile.Friends.Count);
            Assert.AreEqual(time, profile.Birthday);
        }

        [Test]
        [ExpectedException(typeof(StaleObjectStateException))]
        public void Test_Bug_TwoInstancesUpdateProfile()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            profile.AboutInformation = "ver1";
            
            SessionData data1 = SecurityManager.CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ProfileUpdateData d = new ProfileUpdateData();
                d.Profile = profile;
                Service.UpdateProfile(data1.Token, d);
            });

            SessionData data2 = SecurityManager.CreateNewSession(profile, ClientInformation);

            profile.AboutInformation = "ver2";
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ProfileUpdateData d = new ProfileUpdateData();
                d.Profile = profile;
                Service.UpdateProfile(data2.Token, d);
            });
        }
    }
}
