using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.DataAccess.Converter.V4_V5;
using BodyArchitect.Model;
using BodyArchitect.Service.Admin;
using BodyArchitect.Service.Admin.Objects;
using BodyArchitect.Service.V2.Payments;
using NUnit.Framework;

namespace BodyArchitect.UnitTests.DbConverter.V4V5
{
    [TestFixture]
    public class TestAdministrationService : AdminServiceTestFixtureBase
    {
        [Test]
        public void SendMessage_All()
        {
            Profile oldProfile = new Profile();
            oldProfile.UserName = "test";
            oldProfile.Email = "gfg@gfg.pl";
            oldProfile.Birthday = DateTime.Now.Date;
            oldProfile.CreationDate = DateTime.Now.Date.AddDays(-1);
            oldProfile.CountryId = 2;
            oldProfile.Password = "test1";
            oldProfile.Privacy = new ProfilePrivacy();
            oldProfile.Settings = new ProfileSettings();
            insertToDatabase(oldProfile);

            oldProfile = new Profile();
            oldProfile.UserName = "test1";
            oldProfile.Email = "gfg@gfg1.pl";
            oldProfile.Birthday = DateTime.Now.Date;
            oldProfile.CreationDate = DateTime.Now.Date.AddDays(-1);
            oldProfile.CountryId = 2;
            oldProfile.Password = "test1";
            oldProfile.Privacy = new ProfilePrivacy();
            oldProfile.Settings = new ProfileSettings();
            insertToDatabase(oldProfile);

            oldProfile = new Profile();
            oldProfile.UserName = "test2";
            oldProfile.Email = "gfg@gfg3.pl";
            oldProfile.Birthday = DateTime.Now.Date;
            oldProfile.CreationDate = DateTime.Now.Date.AddDays(-1);
            oldProfile.CountryId = 2;
            oldProfile.Password = "test1";
            oldProfile.Privacy = new ProfilePrivacy();
            oldProfile.Settings = new ProfileSettings();
            insertToDatabase(oldProfile);

            RunServiceMethod(service=>
                                 {
                                     service.SendMessage("Test topic","Test message",SendMessageMode.All,new List<int>());
                                 });

            var count=this.Session.QueryOver<Message>().RowCount();
            Assert.AreEqual(3,count);
        }

        [Test]
        public void SendMessage_SelectedCountries()
        {
            Profile oldProfile = new Profile();
            oldProfile.UserName = "test";
            oldProfile.Email = "gfg@gfg.pl";
            oldProfile.Birthday = DateTime.Now.Date;
            oldProfile.CreationDate = DateTime.Now.Date.AddDays(-1);
            oldProfile.CountryId = 2;
            oldProfile.Password = "test1";
            oldProfile.Privacy = new ProfilePrivacy();
            oldProfile.Settings = new ProfileSettings();
            insertToDatabase(oldProfile);

            oldProfile = new Profile();
            oldProfile.UserName = "test1";
            oldProfile.Email = "gfg@gfg1.pl";
            oldProfile.Birthday = DateTime.Now.Date;
            oldProfile.CreationDate = DateTime.Now.Date.AddDays(-1);
            oldProfile.CountryId = 1;
            oldProfile.Password = "test1";
            oldProfile.Privacy = new ProfilePrivacy();
            oldProfile.Settings = new ProfileSettings();
            insertToDatabase(oldProfile);

            oldProfile = new Profile();
            oldProfile.UserName = "test2";
            oldProfile.Email = "gfg@gfg3.pl";
            oldProfile.Birthday = DateTime.Now.Date;
            oldProfile.CreationDate = DateTime.Now.Date.AddDays(-1);
            oldProfile.CountryId = 3;
            oldProfile.Password = "test1";
            oldProfile.Privacy = new ProfilePrivacy();
            oldProfile.Settings = new ProfileSettings();
            insertToDatabase(oldProfile);

            RunServiceMethod(service =>
            {
                service.SendMessage("Test topic", "Test message", SendMessageMode.SelectedCountries, new List<int>() { 1, 2 });
            });
            var count = this.Session.QueryOver<Message>().RowCount();
            Assert.AreEqual(2, count);
        }

        [Test]
        public void SendMessage_ExceptSelectedCountries()
        {
            Profile oldProfile = new Profile();
            oldProfile.UserName = "test";
            oldProfile.Email = "gfg@gfg.pl";
            oldProfile.Birthday = DateTime.Now.Date;
            oldProfile.CreationDate = DateTime.Now.Date.AddDays(-1);
            oldProfile.CountryId = 2;
            oldProfile.Password = "test1";
            oldProfile.Privacy = new ProfilePrivacy();
            oldProfile.Settings = new ProfileSettings();
            insertToDatabase(oldProfile);

            oldProfile = new Profile();
            oldProfile.UserName = "test1";
            oldProfile.Email = "gfg@gfg1.pl";
            oldProfile.Birthday = DateTime.Now.Date;
            oldProfile.CreationDate = DateTime.Now.Date.AddDays(-1);
            oldProfile.CountryId = 1;
            oldProfile.Password = "test1";
            oldProfile.Privacy = new ProfilePrivacy();
            oldProfile.Settings = new ProfileSettings();
            insertToDatabase(oldProfile);

            oldProfile = new Profile();
            oldProfile.UserName = "test2";
            oldProfile.Email = "gfg@gfg3.pl";
            oldProfile.Birthday = DateTime.Now.Date;
            oldProfile.CreationDate = DateTime.Now.Date.AddDays(-1);
            oldProfile.CountryId = 3;
            oldProfile.Password = "test1";
            oldProfile.Privacy = new ProfilePrivacy();
            oldProfile.Settings = new ProfileSettings();
            insertToDatabase(oldProfile);

            RunServiceMethod(service =>
            {
                service.SendMessage("Test topic", "Test message", SendMessageMode.ExceptSelectedCountries, new List<int>() { 1, 2 });
            });

            var count = this.Session.QueryOver<Message>().RowCount();
            Assert.AreEqual(1, count);
        }

        [Test]
        public void DeleteUnusedProfiles_TwoProfiles_OneDelete()
        {
            Profile oldProfile = new Profile();
            oldProfile.UserName = "test";
            oldProfile.Email = "gfg@gfg.pl";
            oldProfile.Birthday = DateTime.Now.Date;
            oldProfile.CreationDate = DateTime.Now.Date.AddDays(-1);
            oldProfile.CountryId = 2;
            oldProfile.Password = "test1";
            oldProfile.Privacy = new ProfilePrivacy();
            oldProfile.Settings = new ProfileSettings();
            oldProfile.Statistics = new ProfileStatistics();
            oldProfile.Statistics.LastLoginDate = DateTime.Now.AddMonths(-10);
            insertToDatabase(oldProfile.Statistics);
            insertToDatabase(oldProfile);

            oldProfile = new Profile();
            oldProfile.UserName = "test1";
            oldProfile.Email = "gf1g@gfg.pl";
            oldProfile.Birthday = DateTime.Now.Date;
            oldProfile.CreationDate = DateTime.Now.Date.AddDays(-1);
            oldProfile.CountryId = 2;
            oldProfile.Password = "test1";
            oldProfile.Privacy = new ProfilePrivacy();
            oldProfile.Settings = new ProfileSettings();
            oldProfile.Statistics = new ProfileStatistics();
            oldProfile.Statistics.LastLoginDate = DateTime.Now.AddMonths(-6);
            insertToDatabase(oldProfile.Statistics);
            insertToDatabase(oldProfile);

            RunServiceMethod(service =>
            {
                var unusedProfiles = service.DeleteOldProfiles(new DeleteOldProfilesParam());
                Assert.AreEqual(1, unusedProfiles.Count);
            });
            Assert.AreEqual(2, Session.QueryOver<Profile>().RowCount());
        }

        [Test]
        public void DeleteUnusedProfiles_ShouldRemove()
        {
            Profile oldProfile = new Profile();
            oldProfile.UserName = "test";
            oldProfile.Email = "gfg@gfg.pl";
            oldProfile.Birthday = DateTime.Now.Date;
            oldProfile.CreationDate = DateTime.Now.Date.AddDays(-1);
            oldProfile.CountryId = 2;
            oldProfile.Password = "test1";
            oldProfile.Privacy = new ProfilePrivacy();
            oldProfile.Settings = new ProfileSettings();
            oldProfile.Statistics=new ProfileStatistics();
            oldProfile.Statistics.LastLoginDate = DateTime.Now.AddMonths(-10);
            insertToDatabase(oldProfile.Statistics);
            insertToDatabase(oldProfile);

            LoginData login = new LoginData();
            login.LoginDateTime = DateTime.Now.AddMonths(-10);
            login.ProfileId = oldProfile.GlobalId;
            login.ClientInstanceId = Guid.NewGuid();
            login.ApplicationLanguage = "pl";
            login.ApplicationVersion = "1.0.0.0";
            insertToDatabase(login);


            RunServiceMethod(service =>
            {
                var unusedProfiles=service.DeleteOldProfiles(new DeleteOldProfilesParam());
                Assert.AreEqual(1,unusedProfiles.Count);
            });
            Assert.AreEqual(1,Session.QueryOver<Profile>().RowCount());
        }

        [Test]
        public void DeleteUnusedProfiles_ShoudntRemove_LastLoginDate()
        {
            Profile oldProfile = new Profile();
            oldProfile.UserName = "test";
            oldProfile.Email = "gfg@gfg.pl";
            oldProfile.Birthday = DateTime.Now.Date;
            oldProfile.CreationDate = DateTime.Now.Date.AddDays(-1);
            oldProfile.CountryId = 2;
            oldProfile.Password = "test1";
            oldProfile.Privacy = new ProfilePrivacy();
            oldProfile.Settings = new ProfileSettings();
            oldProfile.Statistics = new ProfileStatistics();
            oldProfile.Statistics.LastLoginDate = DateTime.Now.AddMonths(-6);
            insertToDatabase(oldProfile.Statistics);
            insertToDatabase(oldProfile);

            LoginData login = new LoginData();
            login.LoginDateTime = DateTime.Now.AddMonths(-10);
            login.ProfileId = oldProfile.GlobalId;
            login.ClientInstanceId = Guid.NewGuid();
            login.ApplicationLanguage = "pl";
            login.ApplicationVersion = "1.0.0.0";
            insertToDatabase(login);


            RunServiceMethod(service =>
            {
                var unusedProfiles = service.DeleteOldProfiles(new DeleteOldProfilesParam());
                Assert.AreEqual(0, unusedProfiles.Count);
            });

            Assert.AreEqual(2, Session.QueryOver<Profile>().RowCount());
        }

        [Test]
        public void DeleteUnusedProfiles_ShoudntRemove_HasTrainingDays()
        {
            Profile oldProfile = new Profile();
            oldProfile.UserName = "test";
            oldProfile.Email = "gfg@gfg.pl";
            oldProfile.Birthday = DateTime.Now.Date;
            oldProfile.CreationDate = DateTime.Now.Date.AddDays(-1);
            oldProfile.CountryId = 2;
            oldProfile.Password = "test1";
            oldProfile.Privacy = new ProfilePrivacy();
            oldProfile.Settings = new ProfileSettings();
            oldProfile.Statistics = new ProfileStatistics();
            oldProfile.Statistics.LastLoginDate = DateTime.Now.AddMonths(-16);
            oldProfile.Statistics.TrainingDaysCount = 1;
            insertToDatabase(oldProfile.Statistics);
            insertToDatabase(oldProfile);

            LoginData login = new LoginData();
            login.LoginDateTime = DateTime.Now.AddMonths(-10);
            login.ProfileId = oldProfile.GlobalId;
            login.ClientInstanceId = Guid.NewGuid();
            login.ApplicationLanguage = "pl";
            login.ApplicationVersion = "1.0.0.0";
            insertToDatabase(login);


            RunServiceMethod(service =>
            {
                var unusedProfiles = service.DeleteOldProfiles(new DeleteOldProfilesParam());
                Assert.AreEqual(0, unusedProfiles.Count);
            });

            Assert.AreEqual(2, Session.QueryOver<Profile>().RowCount());
        }
    }
}
