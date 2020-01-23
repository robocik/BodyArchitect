using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NUnit.Framework;
using ExerciseForceType = BodyArchitect.Model.ExerciseForceType;
using ExerciseType = BodyArchitect.Model.ExerciseType;
using MechanicsType = BodyArchitect.Model.MechanicsType;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_DeleteTrainingDay:TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        List<Customer> customers = new List<Customer>();

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                customers.Clear();
                profiles.Clear();
                profiles.Add(CreateProfile(Session, "test1"));
                profiles.Add(CreateProfile(Session, "test2"));
                profiles.Add(CreateProfile(Session, "test3"));
                profiles.Add(CreateProfile(Session, "test4"));
                profiles[0].Friends.Add(profiles[3]);
                profiles[3].Friends.Add(profiles[0]);

                var customer = CreateCustomer("Cust1", profiles[0]);
                customers.Add(customer);
                customer = CreateCustomer("Cust2", profiles[0]);
                customers.Add(customer);
                customer = CreateCustomer("Cust3", profiles[1]);
                customers.Add(customer);

                tx.Commit();
            }
        }

        [Test]
        public void DeleteTrainingDay()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            DateTime date = DateTime.Now.AddDays(-2);
            TrainingDayDTO day = new TrainingDayDTO(date);
            day.ProfileId = profile.GlobalId;
            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
                                 {
                                     var param = new DeleteTrainingDayParam();
                                     param.TrainingDayId = day.GlobalId;

                Service.DeleteTrainingDay(data.Token, param);
            });

            var count = Session.QueryOver<TrainingDay>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void DeleteTrainingDay_EntryWithReminder()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            DateTime date = DateTime.Now.AddDays(-2);
            TrainingDayDTO day = new TrainingDayDTO(date);
            day.ProfileId = profile.GlobalId;
            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.RemindBefore = TimeSpan.FromMinutes(10);
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new DeleteTrainingDayParam();
                param.TrainingDayId = day.GlobalId;

                Service.DeleteTrainingDay(data.Token, param);
            });

            var count = Session.QueryOver<ReminderItem>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void DeleteTrainingDay_AnotherProfile()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            DateTime date = DateTime.Now.AddDays(-2);
            TrainingDayDTO day = new TrainingDayDTO(date);
            day.ProfileId = profile.GlobalId;
            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            data = CreateNewSession((ProfileDTO)profiles[1].Tag, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new DeleteTrainingDayParam();
                param.TrainingDayId = day.GlobalId;

                Service.DeleteTrainingDay(data.Token, param);
            });
        }

        #region Statistics

        [Test]
        public void TestDeleteTrainingDay_LatestChanged()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            DateTime date = DateTime.Now.AddDays(-2);
            TrainingDayDTO day = new TrainingDayDTO(date);
            day.ProfileId = profile.GlobalId;

            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            
            date = DateTime.Now.AddDays(-1);
            day = new TrainingDayDTO(date);
            day.ProfileId = profile.GlobalId;

            sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(2, dbProfile.Statistics.TrainingDaysCount);
            Assert.AreEqual(date.Date, dbProfile.Statistics.LastEntryDate.Value);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new DeleteTrainingDayParam();
                param.TrainingDayId = day.GlobalId;

                Service.DeleteTrainingDay(data.Token, param);
            });

            dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(1, dbProfile.Statistics.TrainingDaysCount);
            Assert.AreEqual(DateTime.Now.AddDays(-2).Date, dbProfile.Statistics.LastEntryDate.Value);
        }

        [Test]
        public void TestDeleteTrainingDay_LatestNotChanged()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            DateTime date = DateTime.Now.AddDays(-2);
            TrainingDayDTO day = new TrainingDayDTO(date);
            day.ProfileId = profile.GlobalId;

            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            date = DateTime.Now;
            var day1 = new TrainingDayDTO(DateTime.Now);
            day1.ProfileId = profile.GlobalId;

            sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day1.AddEntry(sizeEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day1);
                day1 = result.TrainingDay;
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(2, dbProfile.Statistics.TrainingDaysCount);
            Assert.AreEqual(date.Date, dbProfile.Statistics.LastEntryDate.Value);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new DeleteTrainingDayParam();
                param.TrainingDayId = day.GlobalId;

                Service.DeleteTrainingDay(data.Token, param); ;
            });

            dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(1, dbProfile.Statistics.TrainingDaysCount);
            Assert.AreEqual(date.Date, dbProfile.Statistics.LastEntryDate.Value);
        }

        [Test]
        public void TestDeleteTrainingDay_Mode_OnlyWithoutMyTraining_TrainingDayShouldBeDeleted()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            DateTime date = DateTime.Now.AddDays(-2);
            TrainingDayDTO day = new TrainingDayDTO(date);
            day.ProfileId = profile.GlobalId;

            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new DeleteTrainingDayParam();
                param.TrainingDayId = day.GlobalId;
                param.Mode = DeleteTrainingDayMode.OnlyWithoutMyTraining;
                Service.DeleteTrainingDay(data.Token, param);
            });

            Assert.AreEqual(0, Session.QueryOver<TrainingDay>().RowCount());
            Assert.AreEqual(0, Session.QueryOver<SizeEntry>().RowCount());
        }

        [Test]
        public void TestDeleteTrainingDay_Mode_OnlyWithoutMyTraining_TrainingDayShouldNotChanged()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            DateTime date = DateTime.Now.AddDays(-2);
            var day = new TrainingDay(date);
            day.Profile = profiles[0];
            SizeEntry size = new SizeEntry();
            size.Wymiary=new Wymiary();
            MyTraining training = new A6WTraining();
            training.Name = "test";
            training.Profile = day.Profile;
            size.MyTraining = training;
            day.AddEntry(size);
            insertToDatabase(day);
            

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new DeleteTrainingDayParam();
                param.TrainingDayId = day.GlobalId;
                param.Mode = DeleteTrainingDayMode.OnlyWithoutMyTraining;
                Service.DeleteTrainingDay(data.Token, param);
            });

            Assert.AreEqual(1, Session.QueryOver<TrainingDay>().RowCount());
            Assert.AreEqual(1, Session.QueryOver<SizeEntry>().RowCount());
        }

        [Test]
        public void TestDeleteTrainingDay_Mode_OnlyWithoutMyTraining_TrainingDayShouldNotBeDeleted_OneEntryShouldBeDeletedOnly()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            DateTime date = DateTime.Now.AddDays(-2);
            var day = new TrainingDay(date);
            day.Profile = profiles[0];
            SizeEntry size = new SizeEntry();
            size.Wymiary = new Wymiary();
            MyTraining training = new A6WTraining();
            training.Name = "test";
            training.Profile = day.Profile;
            size.MyTraining = training;
            day.AddEntry(size);
            A6WEntry a6w = new A6WEntry();
            a6w.DayNumber = 1;
            day.AddEntry(a6w);
            insertToDatabase(day);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new DeleteTrainingDayParam();
                param.TrainingDayId = day.GlobalId;
                param.Mode = DeleteTrainingDayMode.OnlyWithoutMyTraining;
                Service.DeleteTrainingDay(data.Token, param);
            });

            Assert.AreEqual(1, Session.QueryOver<TrainingDay>().RowCount());
            Assert.AreEqual(1, Session.QueryOver<SizeEntry>().RowCount());
            Assert.AreEqual(0, Session.QueryOver<A6WEntry>().RowCount());
        }

        [Test]
        public void TestDeleteTrainingDay_RemoveAllTrainingDays()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            DateTime date = DateTime.Now.AddDays(-2);
            TrainingDayDTO day = new TrainingDayDTO(date);
            day.ProfileId = profile.GlobalId;

            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
           
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new DeleteTrainingDayParam();
                param.TrainingDayId = day.GlobalId;

                Service.DeleteTrainingDay(data.Token, param);
            });

            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(0, dbProfile.Statistics.TrainingDaysCount);
            Assert.AreEqual(null, dbProfile.Statistics.LastEntryDate);
            Assert.AreEqual(0, dbProfile.Statistics.SizeEntriesCount);
        }

     
        #endregion

        [Test]
        public void TestDeleteTrainingDay_DataInfo_Refresh()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            DateTime date = DateTime.Now.AddDays(-2);
            TrainingDayDTO day = new TrainingDayDTO(date);
            day.ProfileId = profile.GlobalId;

            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            var oldHash = dbProfile.DataInfo.TrainingDayHash;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
                                 {
                                     TimerService.UtcNow = DateTime.UtcNow.AddHours(1);
                                     var param = new DeleteTrainingDayParam();
                                     param.TrainingDayId = day.GlobalId;

                                     Service.DeleteTrainingDay(data.Token, param);
            });

            dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreNotEqual(oldHash,dbProfile.DataInfo.TrainingDayHash);
        }

     

        #region For customer

        [Test]
        public void ForCustomer_DeleteTrainingDay()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            DateTime date = DateTime.Now.AddDays(-2);
            TrainingDayDTO day = new TrainingDayDTO(date);
            day.ProfileId = profile.GlobalId;
            day.CustomerId = customers[0].GlobalId;
            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new DeleteTrainingDayParam();
                param.TrainingDayId = day.GlobalId;

                Service.DeleteTrainingDay(data.Token, param);
            });

            var count=Session.QueryOver<TrainingDay>().RowCount();
            Assert.AreEqual(0,count);
        }

        #endregion

        #region ExerciseProfileData

        [Test]
        public void ExerciseProfileData_RemoveMaxWeightDay_LastEntryForExercise()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            var entry = new StrengthTrainingEntry();
            entry.MyPlace = GetDefaultMyPlace(profiles[0]);
            day.AddEntry(entry);
            var item = new StrengthTrainingItem();
            item.Exercise = exercise;
            entry.AddEntry(item);
            var serie1 = new Serie("10x10");
            item.AddSerie(serie1);
            var serie2 = new Serie("2x40");
            item.AddSerie(serie2);
            var serie3 = new Serie("4x20");
            item.AddSerie(serie3);
            insertToDatabase(day);

            ExerciseProfileData exData = new ExerciseProfileData();
            exData.Exercise = exercise;
            exData.Profile = profiles[0];
            exData.Serie = serie2;
            exData.MaxWeight = 40;
            exData.Repetitions = 2;
            exData.TrainingDate = day.TrainingDate;
            insertToDatabase(exData);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                DeleteTrainingDayParam param = new DeleteTrainingDayParam();
                param.TrainingDayId = day.GlobalId;
                Service.DeleteTrainingDay(data.Token, param);
            });

            exData = Session.QueryOver<ExerciseProfileData>().SingleOrDefault();
            Assert.IsNull(exData);
        }

        [Test]
        public void ExerciseProfileData_RemoveMaxWeightDay_ChangeMaxWeightInRecords()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            var day = new TrainingDay(DateTime.Now.Date.AddDays(-1));
            day.Profile = profiles[0];

            var entry = new StrengthTrainingEntry();
            entry.MyPlace = GetDefaultMyPlace(profiles[0]);
            day.AddEntry(entry);
            var item = new StrengthTrainingItem();
            item.Exercise = exercise;
            entry.AddEntry(item);
            var serie1 = new Serie("10x10");
            item.AddSerie(serie1);
            var serie2 = new Serie("7x40");
            item.AddSerie(serie2);
            var serie3 = new Serie("4x20");
            item.AddSerie(serie3);
            insertToDatabase(day);

            var day1 = new TrainingDay(DateTime.Now.Date);
            day1.Profile = profiles[0];

            entry = new StrengthTrainingEntry();
            entry.MyPlace = GetDefaultMyPlace(profiles[0]);
            day1.AddEntry(entry);
            item = new StrengthTrainingItem();
            item.Exercise = exercise;
            entry.AddEntry(item);
            var serie11 = new Serie("10x5");
            item.AddSerie(serie11);
            var serie12 = new Serie("1x50");
            item.AddSerie(serie12);
            var serie13 = new Serie("4x11");
            item.AddSerie(serie13);
            insertToDatabase(day1);

            ExerciseProfileData exData = new ExerciseProfileData();
            exData.Exercise = exercise;
            exData.Profile = profiles[0];
            exData.Serie = serie12;
            exData.MaxWeight = 50;
            exData.Repetitions = 1;
            exData.TrainingDate = day1.TrainingDate;
            insertToDatabase(exData);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                DeleteTrainingDayParam param = new DeleteTrainingDayParam();
                param.TrainingDayId = day1.GlobalId;
                Service.DeleteTrainingDay(data.Token, param);
            });

            exData = Session.QueryOver<ExerciseProfileData>().SingleOrDefault();
            Assert.AreEqual(profiles[0], exData.Profile);
            Assert.AreEqual(exercise, exData.Exercise);
            Assert.AreEqual(40m, exData.MaxWeight);
            Assert.AreEqual(7, exData.Repetitions);
            Assert.AreEqual(DateTime.Now.Date.AddDays(-1), exData.TrainingDate);
            Assert.AreEqual(serie2.GlobalId, exData.Serie.GlobalId);
        }
        #endregion

        #region System entries

        [Test]
        [ExpectedException(typeof(ConsistencyException))]
        public void DeleteDayWithSystemEntry()
        {
            TrainingDay day = new TrainingDay(DateTime.Now);
            day.Profile = profiles[0];
            SizeEntry entry = new SizeEntry();
            entry.Wymiary = new Wymiary();
            entry.Wymiary.Height = 100;
            entry.Status = Model.EntryObjectStatus.System;
            day.AddEntry(entry);

            insertToDatabase(day);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new DeleteTrainingDayParam();
                param.TrainingDayId = day.GlobalId;

                Service.DeleteTrainingDay(data.Token, param);
            });
        }
        #endregion
    }
}
