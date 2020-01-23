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
    public class TestService_AutomaticMeasurementsUpdate : TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                var profile = CreateProfile(Session, "test1");
                profiles.Add(profile);
                profile.Settings.AutomaticUpdateMeasurements = true;
                Session.Update(profile);
                tx.Commit();
            }
        }

        [Test]
        public void Test_SaveTrainingDay_AutomaticUpdateDisable()
        {
            profiles[0].Settings.AutomaticUpdateMeasurements = false;
            insertToDatabase(profiles[0]);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveTrainingDay(data.Token, day);
            });
            var dbProfile = Session.Get<Profile>(profile.Id);
            Assert.IsNull(dbProfile.Wymiary);
        }

        [Test]
        public void Test_SaveTrainingDay_UpdateSize()
        {
            var profile = (ProfileDTO) profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
                                 {
                                     service.SaveTrainingDay(data.Token, day);
                                 });
            var dbProfile = Session.Get<Profile>(profile.Id);
            Assert.AreEqual(100, dbProfile.Wymiary.Height);
        }

        [Test]
        public void Test_SaveTrainingDay_UpdateSize_TwoTimes()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.AddDays(-2));
            day.ProfileId = profile.Id;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveTrainingDay(data.Token, day);
            });

            day = new TrainingDayDTO(DateTime.Now.AddDays(-1));
            day.ProfileId = profile.Id;
            entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 50;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveTrainingDay(data.Token, day);
            });
            var dbProfile = Session.Get<Profile>(profile.Id);
            Assert.AreEqual(50, dbProfile.Wymiary.Height);
        }

        [Test]
        public void Test_SaveTrainingDay_UpdateSize_TwoTimesDifferentSizes()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.AddDays(-2));
            day.ProfileId = profile.Id;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveTrainingDay(data.Token, day);
            });

            day = new TrainingDayDTO(DateTime.Now.AddDays(-1));
            day.ProfileId = profile.Id;
            entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Weight = 50;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveTrainingDay(data.Token, day);
            });
            var dbProfile = Session.Get<Profile>(profile.Id);
            Assert.AreEqual(100, dbProfile.Wymiary.Height);
            Assert.AreEqual(50, dbProfile.Wymiary.Weight);
        }

        [Test]
        public void Test_SaveTrainingDay_UpdateSize_TwoTimes_SecondInThePast()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.AddDays(-1));
            day.ProfileId = profile.Id;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveTrainingDay(data.Token, day);
            });

            day = new TrainingDayDTO(DateTime.Now.AddDays(-2));
            day.ProfileId = profile.Id;
            entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 50;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveTrainingDay(data.Token, day);
            });
            var dbProfile = Session.Get<Profile>(profile.Id);
            Assert.AreEqual(100, dbProfile.Wymiary.Height);
        }

        [Test]
        public void Test_DeleteTrainingDay_UpdateSize()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.AddDays(-2));
            day.ProfileId = profile.Id;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveTrainingDay(data.Token, day);
            });

            day = new TrainingDayDTO(DateTime.Now.AddDays(-1));
            day.ProfileId = profile.Id;
            entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 50;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                day=service.SaveTrainingDay(data.Token, day);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteTrainingDay(data.Token, day);
            });

            var dbProfile = Session.Get<Profile>(profile.Id);
            Assert.AreEqual(100, dbProfile.Wymiary.Height);
        }
    }
}
