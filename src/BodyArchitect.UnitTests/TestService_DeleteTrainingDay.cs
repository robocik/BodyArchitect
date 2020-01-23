using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service;
using BodyArchitect.Service.Model;
using NUnit.Framework;

namespace BodyArchitect.UnitTests
{
    [TestFixture]
    public class TestService_DeleteTrainingDay:TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                profiles.Add(CreateProfile(Session, "test1"));
                profiles.Add(CreateProfile(Session, "test2"));
                profiles.Add(CreateProfile(Session, "test3"));
                profiles.Add(CreateProfile(Session, "test4"));
                profiles[0].Friends.Add(profiles[3]);
                profiles[3].Friends.Add(profiles[0]);
                tx.Commit();
            }
        }

        #region Statistics

        [Test]
        public void TestDeleteTrainingDay_LatestChanged()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            DateTime date = DateTime.Now.AddDays(-2);
            TrainingDayDTO day = new TrainingDayDTO(date);
            day.ProfileId = profile.Id;

            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });
            
            date = DateTime.Now.AddDays(-1);
            day = new TrainingDayDTO(date);
            day.ProfileId = profile.Id;

            sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });
            var dbProfile = Session.Get<Profile>(profile.Id);
            Assert.AreEqual(2, dbProfile.Statistics.TrainingDaysCount);
            Assert.AreEqual(date.Date, dbProfile.Statistics.LastEntryDate.Value);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.DeleteTrainingDay(data.Token, day);
            });

            dbProfile = Session.Get<Profile>(profile.Id);
            Assert.AreEqual(1, dbProfile.Statistics.TrainingDaysCount);
            Assert.AreEqual(DateTime.Now.AddDays(-2).Date, dbProfile.Statistics.LastEntryDate.Value);
        }

        [Test]
        public void TestDeleteTrainingDay_LatestNotChanged()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            DateTime date = DateTime.Now.AddDays(-2);
            TrainingDayDTO day = new TrainingDayDTO(date);
            day.ProfileId = profile.Id;

            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });
            date = DateTime.Now;
            var day1 = new TrainingDayDTO(DateTime.Now);
            day1.ProfileId = profile.Id;

            sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day1.AddEntry(sizeEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day1 = Service.SaveTrainingDay(data.Token, day1);
            });
            var dbProfile = Session.Get<Profile>(profile.Id);
            Assert.AreEqual(2, dbProfile.Statistics.TrainingDaysCount);
            Assert.AreEqual(date.Date, dbProfile.Statistics.LastEntryDate.Value);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.DeleteTrainingDay(data.Token, day);
            });

            dbProfile = Session.Get<Profile>(profile.Id);
            Assert.AreEqual(1, dbProfile.Statistics.TrainingDaysCount);
            Assert.AreEqual(date.Date, dbProfile.Statistics.LastEntryDate.Value);
        }

        [Test]
        public void TestDeleteTrainingDay_RemoveAllTrainingDays()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            DateTime date = DateTime.Now.AddDays(-2);
            TrainingDayDTO day = new TrainingDayDTO(date);
            day.ProfileId = profile.Id;

            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });
           
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.DeleteTrainingDay(data.Token, day);
            });

            var dbProfile = Session.Get<Profile>(profile.Id);
            Assert.AreEqual(0, dbProfile.Statistics.TrainingDaysCount);
            Assert.AreEqual(null, dbProfile.Statistics.LastEntryDate);
            Assert.AreEqual(0, dbProfile.Statistics.SizeEntriesCount);
        }


        #endregion
    }
}
