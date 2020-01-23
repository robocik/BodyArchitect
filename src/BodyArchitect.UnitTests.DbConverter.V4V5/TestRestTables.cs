using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.DataAccess.Converter.V4_V5;
using BodyArchitect.Model.Old;
using BodyArchitect.Service.V2.Payments;
using NUnit.Framework;
using Profile = BodyArchitect.Model.Profile;

namespace BodyArchitect.UnitTests.DbConverter.V4V5
{
    public class TestRestTables : NHibernateTestFixtureBase
    {
        [Test]
        public void IsDeleteProfileExists()
        {
            Convert();
            var deletedProfile=SessionNew.QueryOver<Profile>().Where(x=>x.IsDeleted).SingleOrDefault();
            Assert.IsNotNull(deletedProfile.DataInfo);
            Assert.IsNotNull(deletedProfile.Settings);
            Assert.IsNull(deletedProfile.Statistics);
            Assert.IsNull(deletedProfile.Wymiary);
            Assert.IsFalse(deletedProfile.Privacy.Searchable);
        }

        [Test]
        public void IsMissingExerciseExists()
        {
            Convert();
            var missingExercise = SessionNew.QueryOver<Model.Exercise>().SingleOrDefault();
            Assert.IsNotNull(missingExercise);

        }

        [Test]
        public void ConvertWp7PushNotifications()
        {
            var profile = CreateProfile("profile1");
            Model.Old.WP7PushNotification wp71=new WP7PushNotification();
            wp71.Added = DateTime.UtcNow.Date;
            wp71.Counter = 1;
            wp71.DeviceID = "Test1";
            wp71.LiveTile = true;
            wp71.LiveTileFrequency = 3;
            wp71.LiveTileLastUpdate = DateTime.UtcNow.Date.AddDays(-2);
            wp71.Modified = DateTime.UtcNow.AddDays(-10).Date;
            wp71.ProfileId = profile.Id;
            wp71.PushNotifications = true;
            wp71.URI = "gfdgdfgdfg";
            insertToOldDatabase(wp71);

            Convert();

            var dbProfile = SessionNew.QueryOver<Model.Profile>().Where(x => x.UserName=="profile1").SingleOrDefault();
            var newWp71 = SessionNew.QueryOver<Model.WP7PushNotification>().SingleOrDefault();
            Assert.AreEqual(wp71.Added,newWp71.Added);
            Assert.AreEqual(wp71.Counter, newWp71.Counter);
            Assert.AreEqual(wp71.DeviceID, newWp71.DeviceID);
            Assert.AreEqual(wp71.LiveTile, newWp71.LiveTile);
            Assert.AreEqual(wp71.LiveTileFrequency, newWp71.LiveTileFrequency);
            Assert.AreEqual(wp71.LiveTileLastUpdate, newWp71.LiveTileLastUpdate);
            Assert.AreEqual(wp71.Modified, newWp71.Modified);
            Assert.AreEqual(dbProfile.GlobalId, newWp71.ProfileId);
            Assert.AreEqual(wp71.PushNotifications, newWp71.PushNotifications);
            Assert.AreEqual(wp71.URI, newWp71.URI);
        }

        [Test]
        public void ConvertWp7PushNotifications_FromDeletedProfile()
        {
            var profile = CreateProfile("profile1", isDeleted: true);
            Model.Old.WP7PushNotification wp71 = new WP7PushNotification();
            wp71.Added = DateTime.UtcNow.Date;
            wp71.Counter = 1;
            wp71.DeviceID = "Test1";
            wp71.LiveTile = true;
            wp71.LiveTileFrequency = 3;
            wp71.LiveTileLastUpdate = DateTime.UtcNow.Date.AddDays(-2);
            wp71.Modified = DateTime.UtcNow.AddDays(-10).Date;
            wp71.ProfileId = profile.Id;
            wp71.PushNotifications = true;
            wp71.URI = "gfdgdfgdfg";
            insertToOldDatabase(wp71);

            Convert();

            var count = SessionNew.QueryOver<Model.WP7PushNotification>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void ConvertLoginData()
        {
            var profile = CreateProfile("profile1");
            var wp71 = new LoginData();
            wp71.ProfileId = profile.Id;
            wp71.ApplicationLanguage = "pl";
            wp71.ApplicationVersion = "1.0";
            wp71.ClientInstanceId = Guid.NewGuid();
            wp71.LoginDateTime = DateTime.Now.Date;
            wp71.Platform = PlatformType.Linux;
            wp71.PlatformVersion = "1.";
            insertToOldDatabase(wp71);

            Convert();

            var dbProfile = SessionNew.QueryOver<Model.Profile>().Where(x => x.UserName == "profile1").SingleOrDefault();
            var newWp71 = SessionNew.QueryOver<Model.LoginData>().SingleOrDefault();
            Assert.AreEqual(wp71.ApplicationLanguage, newWp71.ApplicationLanguage);
            Assert.AreEqual(wp71.ApplicationVersion, newWp71.ApplicationVersion);
            Assert.AreEqual(wp71.ClientInstanceId, newWp71.ClientInstanceId);
            Assert.AreEqual(wp71.LoginDateTime, newWp71.LoginDateTime);
            Assert.AreEqual((int)wp71.Platform, (int)newWp71.Platform);
            Assert.AreEqual(wp71.PlatformVersion, newWp71.PlatformVersion);
            Assert.AreEqual(dbProfile.GlobalId, newWp71.ProfileId);
        }

        [Test]
        public void ConvertLoginData_FromDeletedProfile()
        {
            var profile = CreateProfile("profile1",isDeleted:true);
            var wp71 = new LoginData();
            wp71.ProfileId = profile.Id;
            wp71.ApplicationLanguage = "pl";
            wp71.ApplicationVersion = "1.0";
            wp71.ClientInstanceId = Guid.NewGuid();
            wp71.LoginDateTime = DateTime.Now.Date;
            wp71.Platform = PlatformType.Linux;
            wp71.PlatformVersion = "1.";
            insertToOldDatabase(wp71);

            Convert();


            var count = SessionNew.QueryOver<Model.LoginData>().RowCount();
            Assert.AreEqual(0,count);
        }
    }
}
