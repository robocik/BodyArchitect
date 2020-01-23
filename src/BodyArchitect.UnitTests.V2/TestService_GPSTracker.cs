using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Portable;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using Newtonsoft.Json;
using NUnit.Framework;
using EntryObjectStatus = BodyArchitect.Model.EntryObjectStatus;
using ExerciseForceType = BodyArchitect.Model.ExerciseForceType;
using ExerciseType = BodyArchitect.Model.ExerciseType;
using Gender = BodyArchitect.Model.Gender;
using MechanicsType = BodyArchitect.Model.MechanicsType;
using Mood = BodyArchitect.Service.V2.Model.Mood;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_GPSTracker : TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        List<Customer> customers = new List<Customer>();
        private APIKey apiKey;
        private Guid key = Guid.NewGuid();

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                customers.Clear();
                profiles.Add(CreateProfile(Session, "test1"));
                profiles.Add(CreateProfile(Session, "test2"));
                profiles.Add(CreateProfile(Session, "test3"));
                profiles.Add(CreateProfile(Session, "test4"));
                profiles[0].Friends.Add(profiles[3]);
                profiles[3].Friends.Add(profiles[0]);

                profiles[2].FavoriteUsers.Add(profiles[0]);

                var customer = CreateCustomer("Cust1", profiles[0]);
                customers.Add(customer);
                customer = CreateCustomer("Cust2", profiles[0]);
                customers.Add(customer);
                customer = CreateCustomer("Cust3", profiles[1]);
                customers.Add(customer);

                apiKey = new APIKey();
                apiKey.ApiKey = key;
                apiKey.ApplicationName = "UnitTest";
                apiKey.EMail = "mail@mail.com";
                apiKey.RegisterDateTime = DateTime.UtcNow;
                insertToDatabase(apiKey);
                tx.Commit();
            }
        }


        #region SaveTrainingDay

        [Test]
        public void SaveTrainingDay_PersistAllProperties()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Cardio, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            var size = new SizeEntryDTO();
            size.Wymiary=new WymiaryDTO();
            day.AddEntry(size);
            var entry = new GPSTrackerEntryDTO();
            day.AddEntry(entry);
            entry.Distance = 2;
            entry.EndDateTime =DateTime.Now.Date.AddDays(1);
            entry.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.MaxAltitude = 3;
            entry.MaxSpeed = 4;
            entry.MinAltitude = 5;
            entry.Mood = Mood.Bad;
            entry.StartDateTime = DateTime.Now.Date.AddDays(-1);
            entry.TotalAscent = 6;
            entry.TotalDescent = 7;

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var dbEntry=Session.Get<GPSTrackerEntry>(day.Objects[0].GlobalId);
            Assert.AreEqual(1.15740740740741E-05m, dbEntry.AvgSpeed);
            Assert.AreEqual(2, dbEntry.Distance);
            Assert.AreEqual(DateTime.Now.Date.AddDays(1), dbEntry.EndDateTime);
            Assert.AreEqual(exercise.GlobalId, dbEntry.Exercise.GlobalId);
            Assert.AreEqual(3, dbEntry.MaxAltitude);
            Assert.AreEqual(4, dbEntry.MaxSpeed);
            Assert.AreEqual(5, dbEntry.MinAltitude);
            Assert.AreEqual(BodyArchitect.Model.Mood.Bad, dbEntry.Mood);
            Assert.AreEqual(DateTime.Now.Date.AddDays(-1), dbEntry.StartDateTime);
            Assert.AreEqual(6, dbEntry.TotalAscent);
            Assert.AreEqual(7, dbEntry.TotalDescent);
        }

        [Test]
        public void DeleteCoordinatesWhenWeRemoveEntry()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Cardio, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            TrainingDay day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];
            GPSTrackerEntry entry = new GPSTrackerEntry();
            entry.Exercise = exercise;
            entry.Coordinates = new GPSCoordinates();
            entry.Coordinates.Content=new byte[20];
            day.AddEntry(entry);
            insertToDatabase(day);

            Assert.AreEqual(1, Session.QueryOver<GPSTrackerEntry>().RowCount());
            Assert.AreEqual(1, Session.QueryOver<GPSCoordinates>().RowCount());

            var dayDTO = day.Map<TrainingDayDTO>();
            dayDTO.Objects.Clear();
            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, dayDTO);
                Assert.IsNull(result.TrainingDay);
            });
            Assert.AreEqual(0, Session.QueryOver<GPSTrackerEntry>().RowCount());
            Assert.AreEqual(0, Session.QueryOver<GPSCoordinates>().RowCount());
        }

        [Test]
        public void SaveTrainingDay_UpdateEntry_DoNotRemoveCoordinates()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Cardio, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            TrainingDay day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];
            GPSTrackerEntry entry = new GPSTrackerEntry();
            entry.Exercise = exercise;
            entry.Coordinates = new GPSCoordinates();
            entry.Coordinates.Content = new byte[20];
            day.AddEntry(entry);
            insertToDatabase(day);

            SessionData data = CreateNewSession(profile, ClientInformation);

            var dayDTO = day.Map<TrainingDayDTO>();

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, dayDTO);
                dayDTO = result.TrainingDay;
            });

            var dbEntry = Session.Get<GPSTrackerEntry>(dayDTO.Objects[0].GlobalId);
            Assert.AreEqual(1,Session.QueryOver<GPSCoordinates>().RowCount());
            Assert.IsNotNull(dbEntry.Coordinates);
            Assert.AreEqual(20,dbEntry.Coordinates.Content.Length);

        }

        [Test]
        public void SaveTrainingDay_UpdateEntry_CalculateAvgSpeed()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Cardio, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            TrainingDay day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];
            GPSTrackerEntry entry = new GPSTrackerEntry();
            entry.Exercise = exercise;
            entry.Distance = 2000;//2km
            entry.Duration = (decimal?) TimeSpan.FromMinutes(30).TotalSeconds;
            day.AddEntry(entry);
            insertToDatabase(day);

            SessionData data = CreateNewSession(profile, ClientInformation);

            var dayDTO = day.Map<TrainingDayDTO>();

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, dayDTO);
                dayDTO = result.TrainingDay;
            });

            var dbEntry = Session.Get<GPSTrackerEntry>(dayDTO.Objects[0].GlobalId);
            Assert.AreEqual(1.11111111111111m, dbEntry.AvgSpeed.Value);

        }

        [Test]
        public void SaveTrainingDay_CalculateCalories_ProfileHasAllWymiary()
        {
            profiles[0].Wymiary=new Wymiary();
            profiles[0].Wymiary.Weight = 93;
            profiles[0].Wymiary.Height = 188;
            profiles[0].Birthday = DateTime.Now.AddDays(-3).AddYears(-32);
            profiles[0].Gender = Gender.Male;
            insertToDatabase(profiles[0].Wymiary);
            insertToDatabase(profiles[0]);

            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Cardio, MechanicsType.Isolation,
                           ExerciseForceType.Pull);
            exercise.Met = 8.0m;
            insertToDatabase(exercise);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            var entry = new GPSTrackerEntryDTO();
            day.AddEntry(entry);
            entry.AvgSpeed = 1;
            entry.Distance = 4000;
            entry.EndDateTime = DateTime.Now.Date.AddDays(1);
            entry.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.MaxAltitude = 3;
            entry.MaxSpeed = 4;
            entry.MinAltitude = 5;
            entry.Duration = (decimal?) TimeSpan.FromMinutes(60).TotalSeconds;
            entry.Mood = Mood.Bad;
            entry.StartDateTime = DateTime.Now.Date.AddDays(-1);
            entry.TotalAscent = 6;
            entry.TotalDescent = 7;

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var dbEntry = Session.Get<GPSTrackerEntry>(day.Objects[0].GlobalId);
            Assert.AreEqual(689m, dbEntry.Calories);
        }

        [Test]
        public void SaveTrainingDay_CalculateCalories_CustomerHasAllWymiary()
        {
            profiles[0].Wymiary = new Wymiary();
            profiles[0].Wymiary.Weight = 63;
            profiles[0].Wymiary.Height = 138;
            profiles[0].Birthday = DateTime.Now.AddDays(-3).AddYears(-22);
            profiles[0].Gender = Gender.Female;
            insertToDatabase(profiles[0].Wymiary);
            insertToDatabase(profiles[0]);

            Customer customer = CreateCustomer("cust", profiles[0], birthday: DateTime.Now.AddDays(-3).AddYears(-32));
            customer.Wymiary = new Wymiary();
            customer.Wymiary.Weight = 93;
            customer.Wymiary.Height = 188;
            insertToDatabase(customer.Wymiary);
            insertToDatabase(customer);

            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Cardio, MechanicsType.Isolation,
                           ExerciseForceType.Pull);
            exercise.Met = 8.0m;
            insertToDatabase(exercise);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            day.CustomerId = customer.GlobalId;
            var entry = new GPSTrackerEntryDTO();
            day.AddEntry(entry);
            entry.AvgSpeed = 1;
            entry.Distance = 4000;
            entry.EndDateTime = DateTime.Now.Date.AddDays(1);
            entry.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.MaxAltitude = 3;
            entry.MaxSpeed = 4;
            entry.MinAltitude = 5;
            entry.Duration = (decimal?)TimeSpan.FromMinutes(60).TotalSeconds;
            entry.Mood = Mood.Bad;
            entry.StartDateTime = DateTime.Now.Date.AddDays(-1);
            entry.TotalAscent = 6;
            entry.TotalDescent = 7;

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var dbEntry = Session.Get<GPSTrackerEntry>(day.Objects[0].GlobalId);
            Assert.AreEqual(689m, dbEntry.Calories);
        }

        [Test]
        public void SaveTrainingDay_CalculateCalories_ProfileWithoutHeight()
        {
            profiles[0].Wymiary = new Wymiary();
            profiles[0].Wymiary.Weight = 93;
            profiles[0].Birthday = DateTime.Now.AddDays(-3).AddYears(-32);
            profiles[0].Gender = Gender.Male;
            insertToDatabase(profiles[0].Wymiary);
            insertToDatabase(profiles[0]);

            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Cardio, MechanicsType.Isolation,
                           ExerciseForceType.Pull);
            exercise.Met = 8.0m;
            insertToDatabase(exercise);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            var entry = new GPSTrackerEntryDTO();
            day.AddEntry(entry);
            entry.AvgSpeed = 1;
            entry.Distance = 4000;
            entry.EndDateTime = DateTime.Now.Date.AddDays(1);
            entry.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.MaxAltitude = 3;
            entry.MaxSpeed = 4;
            entry.MinAltitude = 5;
            entry.Duration = (decimal?)TimeSpan.FromMinutes(60).TotalSeconds;
            entry.Mood = Mood.Bad;
            entry.StartDateTime = DateTime.Now.Date.AddDays(-1);
            entry.TotalAscent = 6;
            entry.TotalDescent = 7;

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var dbEntry = Session.Get<GPSTrackerEntry>(day.Objects[0].GlobalId);
            Assert.AreEqual(672m, dbEntry.Calories);
        }

        [Test]
        public void SaveTrainingDay_CalculateCalories_ProfileWithoutWeight()
        {
            profiles[0].Wymiary = new Wymiary();
            profiles[0].Wymiary.Height = 188;
            profiles[0].Birthday = DateTime.Now.AddDays(-3).AddYears(-32);
            profiles[0].Gender = Gender.Male;
            insertToDatabase(profiles[0].Wymiary);
            insertToDatabase(profiles[0]);

            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Cardio, MechanicsType.Isolation,
                           ExerciseForceType.Pull);
            exercise.Met = 8.0m;
            insertToDatabase(exercise);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            var entry = new GPSTrackerEntryDTO();
            day.AddEntry(entry);
            entry.AvgSpeed = 1;
            entry.Distance = 4000;
            entry.EndDateTime = DateTime.Now.Date.AddDays(1);
            entry.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.MaxAltitude = 3;
            entry.MaxSpeed = 4;
            entry.MinAltitude = 5;
            entry.Duration = (decimal?)TimeSpan.FromMinutes(60).TotalSeconds;
            entry.Mood = Mood.Bad;
            entry.StartDateTime = DateTime.Now.Date.AddDays(-1);
            entry.TotalAscent = 6;
            entry.TotalDescent = 7;

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var dbEntry = Session.Get<GPSTrackerEntry>(day.Objects[0].GlobalId);
            Assert.AreEqual(722m, dbEntry.Calories);
        }

        [Test]
        public void SaveTrainingDay_CalculateCalories_ProfileWithoutGender()
        {
            profiles[0].Wymiary = new Wymiary();
            profiles[0].Wymiary.Height = 188;
            profiles[0].Wymiary.Weight = 93;
            profiles[0].Birthday = DateTime.Now.AddDays(-3).AddYears(-32);
            profiles[0].Gender = Gender.NotSet;
            insertToDatabase(profiles[0].Wymiary);
            insertToDatabase(profiles[0]);

            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Cardio, MechanicsType.Isolation,
                           ExerciseForceType.Pull);
            exercise.Met = 8.0m;
            insertToDatabase(exercise);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            var entry = new GPSTrackerEntryDTO();
            day.AddEntry(entry);
            entry.AvgSpeed = 1;
            entry.Distance = 4000;
            entry.EndDateTime = DateTime.Now.Date.AddDays(1);
            entry.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.MaxAltitude = 3;
            entry.MaxSpeed = 4;
            entry.MinAltitude = 5;
            entry.Duration = (decimal?)TimeSpan.FromMinutes(60).TotalSeconds;
            entry.Mood = Mood.Bad;
            entry.StartDateTime = DateTime.Now.Date.AddDays(-1);
            entry.TotalAscent = 6;
            entry.TotalDescent = 7;

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var dbEntry = Session.Get<GPSTrackerEntry>(day.Objects[0].GlobalId);
            Assert.AreEqual(689m, dbEntry.Calories);
        }

        [Test]
        public void SaveTrainingDay_CalculateCalories_CustomerWithoutBirthday()
        {
            Customer customer = CreateCustomer("cust", profiles[0]);
            customer.Wymiary = new Wymiary();
            customer.Wymiary.Weight = 93;
            customer.Wymiary.Height = 188;
            insertToDatabase(customer.Wymiary);
            insertToDatabase(customer);

            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Cardio, MechanicsType.Isolation,
                           ExerciseForceType.Pull);
            exercise.Met = 8.0m;
            insertToDatabase(exercise);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            day.CustomerId = customer.GlobalId;
            var entry = new GPSTrackerEntryDTO();
            day.AddEntry(entry);
            entry.AvgSpeed = 1;
            entry.Distance = 4000;
            entry.EndDateTime = DateTime.Now.Date.AddDays(1);
            entry.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.MaxAltitude = 3;
            entry.MaxSpeed = 4;
            entry.MinAltitude = 5;
            entry.Duration = (decimal?)TimeSpan.FromMinutes(60).TotalSeconds;
            entry.Mood = Mood.Bad;
            entry.StartDateTime = DateTime.Now.Date.AddDays(-1);
            entry.TotalAscent = 6;
            entry.TotalDescent = 7;

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var dbEntry = Session.Get<GPSTrackerEntry>(day.Objects[0].GlobalId);
            Assert.AreEqual(694m, dbEntry.Calories);
        }
        #endregion

        #region GPS coordinates

        [Test]
        public void DeleteCoordinates_StreamVersion()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Cardio, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            TrainingDay day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];
            GPSTrackerEntry entry = new GPSTrackerEntry();
            entry.Exercise = exercise;
            entry.Coordinates = new GPSCoordinates();
            entry.Coordinates.Content = new byte[20];
            day.AddEntry(entry);
            insertToDatabase(day);

            TrainingDay day1 = new TrainingDay(DateTime.Now.Date);
            day1.Profile = profiles[0];
            entry = new GPSTrackerEntry();
            entry.Exercise = exercise;
            entry.Coordinates = new GPSCoordinates();
            entry.Coordinates.Content = new byte[20];
            day1.AddEntry(entry);
            insertToDatabase(day1);

            Assert.AreEqual(2, Session.QueryOver<GPSTrackerEntry>().RowCount());
            Assert.AreEqual(2, Session.QueryOver<GPSCoordinates>().RowCount());


            var param = new GPSOperationParam();
            param.Params.GPSTrackerEntryId = entry.GlobalId;
            param.Params.Operation = GPSCoordinatesOperationType.DeleteCoordinates;
            param.SessionId = data.Token.SessionId;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var entryDTO = Service.GPSCoordinatesOperation(param);
                Assert.IsFalse(entryDTO.GPSTrackerEntry.HasCoordinates);
            });
            Assert.AreEqual(2, Session.QueryOver<GPSTrackerEntry>().RowCount());
            Assert.AreEqual(1, Session.QueryOver<GPSCoordinates>().RowCount());
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void GPSCoordinatesOperation_StreamVersion_AnotherProfile()
        {
            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Cardio, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            TrainingDay day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];
            GPSTrackerEntry entry = new GPSTrackerEntry();
            entry.Exercise = exercise;
            entry.Coordinates = new GPSCoordinates();
            entry.Coordinates.Content = new byte[20];
            day.AddEntry(entry);
            insertToDatabase(day);

            Assert.AreEqual(1, Session.QueryOver<GPSTrackerEntry>().RowCount());
            Assert.AreEqual(1, Session.QueryOver<GPSCoordinates>().RowCount());


            var param = new GPSOperationParam();
            param.Params.GPSTrackerEntryId = entry.GlobalId;
            param.Params.Operation = GPSCoordinatesOperationType.DeleteCoordinates;
            param.SessionId = data.Token.SessionId;

            RunServiceMethod(delegate(InternalBodyArchitectService s)
            {
                s.GPSCoordinatesOperation(param);
            });
        }

        [Test]
        public void SaveAndReadListOfCoordinates_StreamVersion()
        {
            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Cardio, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            TrainingDay day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];
            GPSTrackerEntry entry = new GPSTrackerEntry();
            entry.Exercise = exercise;
            day.AddEntry(entry);
            Session.Save(day);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var param = new GPSOperationParam();
            param.Params.GPSTrackerEntryId = entry.GlobalId;
            param.Params.Operation = GPSCoordinatesOperationType.UpdateCoordinates;
            
            IList<GPSPoint> pkt = new List<GPSPoint>();
            pkt.Add(new GPSPoint(1, 2, 3, 4, 1));
            pkt.Add(new GPSPoint(11, 21, 31, 41, 2));
            pkt.Add(new GPSPoint(12, 22, 32, 42,3));
            pkt.Add(new GPSPoint(13, 23, 33, 43, 4));
            pkt.Add(new GPSPoint(14, 24, 34, 44, 5));
            pkt.Add(new GPSPoint(15, 25, 35, 45, 6));

            var json=JsonConvert.SerializeObject(pkt);
            var jsonBytes=UTF8Encoding.UTF8.GetBytes(json);
            param.CoordinatesStream = new MemoryStream(jsonBytes.ToZip());
            param.CoordinatesStream.Seek(0, SeekOrigin.Begin);
            param.SessionId = data.Token.SessionId;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var entryDTO = service.GPSCoordinatesOperation( param);
                Assert.IsTrue(entryDTO.GPSTrackerEntry.HasCoordinates);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService service)
                                 {
                                     var t = new GetGPSCoordinatesParam() {GPSTrackerEntryId = entry.GlobalId};
                                     t.SessionId = data.Token.SessionId;
                var points = service.GetGPSCoordinates(t);

                                     json=UTF8Encoding.UTF8.GetString(points.Stream.FromZip());
                                     var result = JsonConvert.DeserializeObject<IList<GPSPoint>>(json);

                                     Assert.AreEqual(pkt.Count, result.Count);
                for (int i = 0; i < pkt.Count; i++)
                {
                    Assert.AreEqual(pkt[i].Altitude, result[i].Altitude);
                    Assert.AreEqual(pkt[i].Latitude, result[i].Latitude);
                    Assert.AreEqual(pkt[i].Longitude, result[i].Longitude);
                    Assert.AreEqual(pkt[i].Duration, result[i].Duration);
                    Assert.AreEqual(pkt[i].Speed, result[i].Speed);

                }
            });
        }

        [Test]
        public void SaveAndReadListOfCoordinates_WithNaNSpeed()
        {
            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Cardio, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            TrainingDay day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];
            GPSTrackerEntry entry = new GPSTrackerEntry();
            entry.Exercise = exercise;
            day.AddEntry(entry);
            Session.Save(day);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var param = new GPSOperationParam();
            param.Params.GPSTrackerEntryId = entry.GlobalId;
            param.Params.Operation = GPSCoordinatesOperationType.UpdateCoordinates;

            IList<GPSPoint> pkt = new List<GPSPoint>();
            pkt.Add(new GPSPoint(1, 2, 3,float.NaN, 1));
            pkt.Add(new GPSPoint(11, 21, 31, float.NaN,2));
            pkt.Add(new GPSPoint(12, 22, 32, float.NaN, 3));
            pkt.Add(new GPSPoint(13, 23, 33, float.NaN, 4));
            pkt.Add(new GPSPoint(14, 24, 34, float.NaN,5));
            pkt.Add(new GPSPoint(15, 25, 35, float.NaN, 1.4f));

            var json = JsonConvert.SerializeObject(pkt);
            var jsonBytes = UTF8Encoding.UTF8.GetBytes(json);
            param.CoordinatesStream = new MemoryStream(jsonBytes.ToZip());
            param.CoordinatesStream.Seek(0, SeekOrigin.Begin);
            param.SessionId = data.Token.SessionId;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var entryDTO = service.GPSCoordinatesOperation(param);
                Assert.IsTrue(entryDTO.GPSTrackerEntry.HasCoordinates);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var t = new GetGPSCoordinatesParam() { GPSTrackerEntryId = entry.GlobalId };
                t.SessionId = data.Token.SessionId;
                var points = service.GetGPSCoordinates(t);

                json = UTF8Encoding.UTF8.GetString(points.Stream.FromZip());
                var result = JsonConvert.DeserializeObject<IList<GPSPoint>>(json);

                Assert.AreEqual(pkt.Count, result.Count);
                for (int i = 0; i < pkt.Count; i++)
                {
                    Assert.AreEqual(pkt[i].Altitude, result[i].Altitude);
                    Assert.AreEqual(pkt[i].Latitude, result[i].Latitude);
                    Assert.AreEqual(pkt[i].Longitude, result[i].Longitude);
                    Assert.AreEqual(pkt[i].Duration, result[i].Duration);
                    Assert.AreEqual(pkt[i].Speed, result[i].Speed);

                }
            });
        }

        [Test]
        public void SaveAndReadListOfCoordinates_WithNotAvailablePoints()
        {
            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Cardio, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            TrainingDay day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];
            GPSTrackerEntry entry = new GPSTrackerEntry();
            entry.Exercise = exercise;
            day.AddEntry(entry);
            Session.Save(day);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var param = new GPSOperationParam();
            param.Params.GPSTrackerEntryId = entry.GlobalId;
            param.Params.Operation = GPSCoordinatesOperationType.UpdateCoordinates;

            IList<GPSPoint> pkt = new List<GPSPoint>();
            pkt.Add(new GPSPoint(1, 2, 3, float.NaN, 1));
            pkt.Add(new GPSPoint(11, 21, 31, float.NaN, 2));
            pkt.Add(new GPSPoint(12, 22, 32, float.NaN, 3));
            pkt.Add(GPSPoint.CreateNotAvailable(4));
            pkt.Add(new GPSPoint(14, 24, 34, float.NaN, 5));
            pkt.Add(new GPSPoint(15, 25, 35, float.NaN, 1.4f));

            var json = JsonConvert.SerializeObject(pkt);
            var jsonBytes = UTF8Encoding.UTF8.GetBytes(json);
            param.CoordinatesStream = new MemoryStream(jsonBytes.ToZip());
            param.CoordinatesStream.Seek(0, SeekOrigin.Begin);
            param.SessionId = data.Token.SessionId;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var entryDTO = service.GPSCoordinatesOperation(param);
                Assert.IsTrue(entryDTO.GPSTrackerEntry.HasCoordinates);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var t = new GetGPSCoordinatesParam() { GPSTrackerEntryId = entry.GlobalId };
                t.SessionId = data.Token.SessionId;
                var points = service.GetGPSCoordinates(t);

                json = UTF8Encoding.UTF8.GetString(points.Stream.FromZip());
                var result = JsonConvert.DeserializeObject<IList<GPSPoint>>(json);

                Assert.AreEqual(pkt.Count, result.Count);
                for (int i = 0; i < pkt.Count; i++)
                {
                    Assert.AreEqual(pkt[i].Altitude, result[i].Altitude);
                    Assert.AreEqual(pkt[i].Latitude, result[i].Latitude);
                    Assert.AreEqual(pkt[i].Longitude, result[i].Longitude);
                    Assert.AreEqual(pkt[i].Duration, result[i].Duration);
                    Assert.AreEqual(pkt[i].Speed, result[i].Speed);

                }
            });
        }

        [Test]
        public void SaveAndReadListOfCoordinates_WithNaNAltitue()
        {
            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Cardio, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            TrainingDay day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];
            GPSTrackerEntry entry = new GPSTrackerEntry();
            entry.Exercise = exercise;
            day.AddEntry(entry);
            Session.Save(day);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var param = new GPSOperationParam();
            param.Params.GPSTrackerEntryId = entry.GlobalId;
            param.Params.Operation = GPSCoordinatesOperationType.UpdateCoordinates;

            IList<GPSPoint> pkt = new List<GPSPoint>();
            pkt.Add(new GPSPoint(1, 2, float.NaN, 4, 1));
            pkt.Add(new GPSPoint(11, 21, float.NaN, 41, 2));
            pkt.Add(new GPSPoint(12, 22, float.NaN, 42, 3));
            pkt.Add(new GPSPoint(13, 23, float.NaN, 43, 4));
            pkt.Add(new GPSPoint(14, 24, float.NaN, 44, 5));
            pkt.Add(new GPSPoint(15, 25, float.NaN, 45, 6));

            var json = JsonConvert.SerializeObject(pkt);
            var jsonBytes = UTF8Encoding.UTF8.GetBytes(json);
            param.CoordinatesStream = new MemoryStream(jsonBytes.ToZip());
            param.CoordinatesStream.Seek(0, SeekOrigin.Begin);
            param.SessionId = data.Token.SessionId;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var entryDTO = service.GPSCoordinatesOperation(param);
                Assert.IsTrue(entryDTO.GPSTrackerEntry.HasCoordinates);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var t = new GetGPSCoordinatesParam() { GPSTrackerEntryId = entry.GlobalId };
                t.SessionId = data.Token.SessionId;
                var points = service.GetGPSCoordinates(t);

                json = UTF8Encoding.UTF8.GetString(points.Stream.FromZip());
                var result = JsonConvert.DeserializeObject<IList<GPSPoint>>(json);

                Assert.AreEqual(pkt.Count, result.Count);
                for (int i = 0; i < pkt.Count; i++)
                {
                    Assert.AreEqual(pkt[i].Altitude, result[i].Altitude);
                    Assert.AreEqual(pkt[i].Latitude, result[i].Latitude);
                    Assert.AreEqual(pkt[i].Longitude, result[i].Longitude);
                    Assert.AreEqual(pkt[i].Duration, result[i].Duration);
                    Assert.AreEqual(pkt[i].Speed, result[i].Speed);

                }
            });
        }

        [Test]
        public void SaveAndReadListOfCoordinates_OnePoint()
        {
            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Cardio, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            TrainingDay day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];
            GPSTrackerEntry entry = new GPSTrackerEntry();
            entry.Exercise = exercise;
            day.AddEntry(entry);
            Session.Save(day);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var param = new GPSOperationParam();
            param.Params.GPSTrackerEntryId = entry.GlobalId;
            param.Params.Operation = GPSCoordinatesOperationType.UpdateCoordinates;

            IList<GPSPoint> pkt = new List<GPSPoint>();
            pkt.Add(new GPSPoint(1, 2, 3, 4, 1));

            var json = JsonConvert.SerializeObject(pkt);
            var jsonBytes = UTF8Encoding.UTF8.GetBytes(json);
            param.CoordinatesStream = new MemoryStream(jsonBytes.ToZip());
            param.CoordinatesStream.Seek(0, SeekOrigin.Begin);
            param.SessionId = data.Token.SessionId;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var entryDTO = service.GPSCoordinatesOperation(param);
                Assert.IsTrue(entryDTO.GPSTrackerEntry.HasCoordinates);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var t = new GetGPSCoordinatesParam() { GPSTrackerEntryId = entry.GlobalId };
                t.SessionId = data.Token.SessionId;
                var points = service.GetGPSCoordinates(t);

                json = UTF8Encoding.UTF8.GetString(points.Stream.FromZip());
                var result = JsonConvert.DeserializeObject<IList<GPSPoint>>(json);

                Assert.AreEqual(pkt.Count, result.Count);
                for (int i = 0; i < pkt.Count; i++)
                {
                    Assert.AreEqual(pkt[i].Altitude, result[i].Altitude);
                    Assert.AreEqual(pkt[i].Latitude, result[i].Latitude);
                    Assert.AreEqual(pkt[i].Longitude, result[i].Longitude);
                    Assert.AreEqual(pkt[i].Duration, result[i].Duration);
                    Assert.AreEqual(pkt[i].Speed, result[i].Speed);

                }
            });
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SaveVeryBigListOfCoordinates_StreamVersion()
        {
            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Cardio, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            TrainingDay day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];
            GPSTrackerEntry entry = new GPSTrackerEntry();
            entry.Exercise = exercise;
            day.AddEntry(entry);
            Session.Save(day);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var param = new GPSOperationParam();
            param.Params.GPSTrackerEntryId = entry.GlobalId;
            param.Params.Operation = GPSCoordinatesOperationType.UpdateCoordinates;

            IList<GPSPoint> pkt = new List<GPSPoint>();
            for (int i = 0; i < 21601; i++)
            {
                pkt.Add(new GPSPoint(i, 2, 3, 4,1));
            }

            var json = JsonConvert.SerializeObject(pkt);
            var jsonBytes = UTF8Encoding.UTF8.GetBytes(json);
            param.CoordinatesStream = new MemoryStream(jsonBytes.ToZip());
            param.CoordinatesStream.Seek(0, SeekOrigin.Begin);
            param.SessionId = data.Token.SessionId;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.GPSCoordinatesOperation(param);
            });

        }

        [Test]
        public void SaveEmptyListOfCoordinates_DeletesCoordinatesFromDB_StreamVersion()
        {
            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Cardio, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            TrainingDay day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];
            GPSTrackerEntry entry = new GPSTrackerEntry();
            entry.Exercise = exercise;
            day.AddEntry(entry);
            Session.Save(day);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var param = new GPSOperationParam();
            param.Params.GPSTrackerEntryId = entry.GlobalId;
            param.Params.Operation = GPSCoordinatesOperationType.UpdateCoordinates;

            IList<GPSPoint> pkt = new List<GPSPoint>();

            var json = JsonConvert.SerializeObject(pkt);
            var jsonBytes = UTF8Encoding.UTF8.GetBytes(json);
            param.CoordinatesStream = new MemoryStream(jsonBytes.ToZip());
            param.CoordinatesStream.Seek(0, SeekOrigin.Begin);
            param.SessionId = data.Token.SessionId;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var entryDTO = service.GPSCoordinatesOperation(param);
                Assert.IsFalse(entryDTO.GPSTrackerEntry.HasCoordinates);
            });
            Assert.AreEqual(0,Session.QueryOver<GPSCoordinates>().RowCount());
        }

        [Test]
        [ExpectedException(typeof(ConsistencyException))]
        public void SaveWrongStream_DeletesCoordinatesFromDB_StreamVersion()
        {
            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Cardio, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            TrainingDay day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];
            GPSTrackerEntry entry = new GPSTrackerEntry();
            entry.Exercise = exercise;
            day.AddEntry(entry);
            Session.Save(day);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var param = new GPSOperationParam();
            param.Params.GPSTrackerEntryId = entry.GlobalId;
            param.Params.Operation = GPSCoordinatesOperationType.UpdateCoordinates;
            
            var json = JsonConvert.SerializeObject(data);
            var jsonBytes = UTF8Encoding.UTF8.GetBytes(json);
            param.CoordinatesStream = new MemoryStream(jsonBytes.ToZip());
            param.CoordinatesStream.Seek(0, SeekOrigin.Begin);
            param.SessionId = data.Token.SessionId;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.GPSCoordinatesOperation(param);
            });
        }

        [Test]
        [ExpectedException(typeof(ConsistencyException))]
        public void SaveWithoutZipStream_DeletesCoordinatesFromDB_StreamVersion()
        {
            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Cardio, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            TrainingDay day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];
            GPSTrackerEntry entry = new GPSTrackerEntry();
            entry.Exercise = exercise;
            day.AddEntry(entry);
            Session.Save(day);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var param = new GPSOperationParam();
            param.Params.GPSTrackerEntryId = entry.GlobalId;
            param.Params.Operation = GPSCoordinatesOperationType.UpdateCoordinates;

            IList<GPSPoint> pkt = new List<GPSPoint>();
            var json = JsonConvert.SerializeObject(pkt);
            var jsonBytes = UTF8Encoding.UTF8.GetBytes(json);
            param.CoordinatesStream = new MemoryStream(jsonBytes);
            param.CoordinatesStream.Seek(0, SeekOrigin.Begin);
            param.SessionId = data.Token.SessionId;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.GPSCoordinatesOperation(param);
            });
        }

        [Test]
        public void Save_SetEntryStatusToDone()
        {
            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Cardio, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            TrainingDay day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];
            GPSTrackerEntry entry = new GPSTrackerEntry();
            entry.Exercise = exercise;
            entry.Status = EntryObjectStatus.Planned;
            day.AddEntry(entry);
            Session.Save(day);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var param = new GPSOperationParam();
            param.Params.GPSTrackerEntryId = entry.GlobalId;
            param.Params.Operation = GPSCoordinatesOperationType.UpdateCoordinates;

            IList<GPSPoint> pkt = new List<GPSPoint>();
            pkt.Add(new GPSPoint(1, 2, 3, 4, 1));
            pkt.Add(new GPSPoint(11, 21, 31, 41, 2));
            pkt.Add(new GPSPoint(12, 22, 32, 42, 3));
            pkt.Add(new GPSPoint(13, 23, 33, 43, 4));
            pkt.Add(new GPSPoint(14, 24, 34, 44, 5));
            pkt.Add(new GPSPoint(15, 25, 35, 45,1.5f));

            var json = JsonConvert.SerializeObject(pkt);
            var jsonBytes = UTF8Encoding.UTF8.GetBytes(json);
            param.CoordinatesStream = new MemoryStream(jsonBytes.ToZip());
            param.CoordinatesStream.Seek(0, SeekOrigin.Begin);
            param.SessionId = data.Token.SessionId;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var entryDTO = service.GPSCoordinatesOperation(param);
                Assert.AreEqual(Service.V2.Model.EntryObjectStatus.Done,entryDTO.GPSTrackerEntry.Status);
            });
            var dbEntry=Session.Get<GPSTrackerEntry>(param.Params.GPSTrackerEntryId);
            Assert.AreEqual(EntryObjectStatus.Done, dbEntry.Status);
        }

        #region Calculations

        [Test]
        public void Save_WithAltitudeCorrection()
        {
            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Cardio, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            TrainingDay day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];
            GPSTrackerEntry entry = new GPSTrackerEntry();
            entry.Exercise = exercise;
            entry.StartDateTime = DateTime.Today;
            entry.Status = EntryObjectStatus.Planned;
            day.AddEntry(entry);
            Session.Save(day);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var param = new GPSOperationParam();
            param.Params.GPSTrackerEntryId = entry.GlobalId;
            param.Params.Operation = GPSCoordinatesOperationType.UpdateCoordinatesWithCorrection;

            IList<GPSPoint> pkt = new List<GPSPoint>();
            pkt.Add(new GPSPoint(1, 2, 3, 4, 1));
            pkt.Add(new GPSPoint(11, 21, 31, 41, 2));
            pkt.Add(new GPSPoint(12, 22, 32, 42, 3));
            pkt.Add(new GPSPoint(13, 23, 23, 43, 4));
            pkt.Add(new GPSPoint(14, 24, 14, 44, 5));
            pkt.Add(new GPSPoint(15, 25, 35, 45, 6));

            var json = JsonConvert.SerializeObject(pkt);
            var jsonBytes = UTF8Encoding.UTF8.GetBytes(json);
            param.CoordinatesStream = new MemoryStream(jsonBytes.ToZip());
            param.CoordinatesStream.Seek(0, SeekOrigin.Begin);
            param.SessionId = data.Token.SessionId;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var entryDTO = service.GPSCoordinatesOperation(param);
                //max altitude
                Assert.AreEqual(20.81771m, entryDTO.GPSTrackerEntry.MaxAltitude.Value);
                //min altitude
                Assert.AreEqual(3, entryDTO.GPSTrackerEntry.MinAltitude.Value);
                //total ascents
                Assert.AreEqual(20.0677089691162m, entryDTO.GPSTrackerEntry.TotalAscent.Value);
                //total descents
                Assert.AreEqual(2.25m, entryDTO.GPSTrackerEntry.TotalDescent.Value);
            });
            var dbEntry = Session.Get<GPSTrackerEntry>(param.Params.GPSTrackerEntryId);
            //max altitude
            Assert.AreEqual(20.81771m, dbEntry.MaxAltitude.Value);
            //min altitude
            Assert.AreEqual(3, dbEntry.MinAltitude.Value);
            //total ascents
            Assert.AreEqual(20.0677089691162m, dbEntry.TotalAscent.Value);
            //total descents
            Assert.AreEqual(2.25m, dbEntry.TotalDescent.Value);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var t = new GetGPSCoordinatesParam() { GPSTrackerEntryId = entry.GlobalId };
                t.SessionId = data.Token.SessionId;
                var points = service.GetGPSCoordinates(t);

                json = UTF8Encoding.UTF8.GetString(points.Stream.FromZip());
                var result = JsonConvert.DeserializeObject<IList<GPSPoint>>(json);

                Assert.AreEqual(pkt.Count, result.Count);
                for (int i = 0; i < pkt.Count; i++)
                {
                    Assert.AreEqual(pkt[i].Latitude, result[i].Latitude);
                    Assert.AreEqual(pkt[i].Longitude, result[i].Longitude);
                    Assert.AreEqual(pkt[i].Duration, result[i].Duration);
                    Assert.AreEqual(pkt[i].Speed, result[i].Speed);
                }
                Assert.AreEqual(3, result[0].Altitude);
                Assert.AreEqual(17.0f, result[1].Altitude);
                Assert.AreEqual(17.333334f, result[2].Altitude);
                Assert.AreEqual(15.083334f, result[3].Altitude);
                Assert.AreEqual(15.854167f, result[4].Altitude);
                Assert.AreEqual(20.817709f, result[5].Altitude);
            });
        }

        [Test]
        public void Save_Calculatations()
        {
            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Cardio, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            TrainingDay day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];
            GPSTrackerEntry entry = new GPSTrackerEntry();
            entry.Exercise = exercise;
            entry.StartDateTime = DateTime.Today;
            entry.Status = EntryObjectStatus.Planned;
            day.AddEntry(entry);
            Session.Save(day);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var param = new GPSOperationParam();
            param.Params.GPSTrackerEntryId = entry.GlobalId;
            param.Params.Operation = GPSCoordinatesOperationType.UpdateCoordinates;

            IList<GPSPoint> pkt = new List<GPSPoint>();
            pkt.Add(new GPSPoint(1, 2, 3, 4, 1));
            pkt.Add(new GPSPoint(11, 21, 31, 41, 2));
            pkt.Add(new GPSPoint(12, 22, 32, 42, 3));
            pkt.Add(new GPSPoint(13, 23, 23, 43, 4));
            pkt.Add(new GPSPoint(14, 24, 14, 44, 5));
            pkt.Add(new GPSPoint(15, 25, 35, 45, 6));

            var json = JsonConvert.SerializeObject(pkt);
            var jsonBytes = UTF8Encoding.UTF8.GetBytes(json);
            param.CoordinatesStream = new MemoryStream(jsonBytes.ToZip());
            param.CoordinatesStream.Seek(0, SeekOrigin.Begin);
            param.SessionId = data.Token.SessionId;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var entryDTO = service.GPSCoordinatesOperation(param);
                //distance
                Assert.IsNotNull(entryDTO.GPSTrackerEntry.Distance);
                Assert.AreNotEqual(0, entryDTO.GPSTrackerEntry.Distance.Value);
                //max speed
                Assert.IsNotNull(entryDTO.GPSTrackerEntry.MaxSpeed);
                Assert.AreEqual(45, entryDTO.GPSTrackerEntry.MaxSpeed.Value);
                //average speed
                Assert.IsNotNull(entryDTO.GPSTrackerEntry.AvgSpeed);
                Assert.AreEqual(499702.41356793m, entryDTO.GPSTrackerEntry.AvgSpeed.Value);
                //max altitude
                Assert.IsNotNull(entryDTO.GPSTrackerEntry.MaxAltitude);
                Assert.AreEqual(35, entryDTO.GPSTrackerEntry.MaxAltitude.Value);
                //min altitude
                Assert.IsNotNull(entryDTO.GPSTrackerEntry.MinAltitude);
                Assert.AreEqual(3, entryDTO.GPSTrackerEntry.MinAltitude.Value);
                //total ascents
                Assert.IsNotNull(entryDTO.GPSTrackerEntry.TotalAscent);
                Assert.AreEqual(50, entryDTO.GPSTrackerEntry.TotalAscent.Value);
                //total descents
                Assert.IsNotNull(entryDTO.GPSTrackerEntry.TotalDescent);
                Assert.AreEqual(18, entryDTO.GPSTrackerEntry.TotalDescent.Value);
                //duration
                Assert.IsNotNull(entryDTO.GPSTrackerEntry.Duration);
                Assert.AreEqual(6, entryDTO.GPSTrackerEntry.Duration.Value);
                //start and end
                Assert.AreEqual(DateTime.Today,entryDTO.GPSTrackerEntry.StartDateTime.Value);
                Assert.IsNull(entryDTO.GPSTrackerEntry.EndDateTime);
            });
            var dbEntry = Session.Get<GPSTrackerEntry>(param.Params.GPSTrackerEntryId);
            //distance
            Assert.IsNotNull(dbEntry.Distance);
            Assert.AreNotEqual(0, dbEntry.Distance.Value);
            //max speed
            Assert.IsNotNull(dbEntry.MaxSpeed);
            Assert.AreEqual(45, dbEntry.MaxSpeed.Value);
            //average speed
            Assert.IsNotNull(dbEntry.AvgSpeed);
            Assert.AreEqual(499702.41356793m, dbEntry.AvgSpeed.Value);
            //max altitude
            Assert.IsNotNull(dbEntry.MaxAltitude);
            Assert.AreEqual(35, dbEntry.MaxAltitude.Value);
            //min altitude
            Assert.IsNotNull(dbEntry.MinAltitude);
            Assert.AreEqual(3, dbEntry.MinAltitude.Value);
            //total ascents
            Assert.IsNotNull(dbEntry.TotalAscent);
            Assert.AreEqual(50, dbEntry.TotalAscent.Value);
            //total descents
            Assert.IsNotNull(dbEntry.TotalDescent);
            Assert.AreEqual(18, dbEntry.TotalDescent.Value);
            //duration
            Assert.IsNotNull(dbEntry.Duration);
            Assert.AreEqual(6, dbEntry.Duration.Value);
            //start and end
            Assert.AreEqual(DateTime.Today, dbEntry.StartDateTime.Value);
            Assert.IsNull(dbEntry.EndDateTime);
        }

        [Test]
        public void Save_Calculatations_WhenEntryHasZeroInsteadNull()
        {
            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Cardio, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            TrainingDay day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];
            GPSTrackerEntry entry = new GPSTrackerEntry();
            entry.Exercise = exercise;
            entry.Distance = 0;
            entry.Duration = 0;
            entry.AvgSpeed = 0;
            entry.Calories = 0;
            entry.StartDateTime = DateTime.Today;
            entry.Status = EntryObjectStatus.Planned;
            day.AddEntry(entry);
            Session.Save(day);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var param = new GPSOperationParam();
            param.Params.GPSTrackerEntryId = entry.GlobalId;
            param.Params.Operation = GPSCoordinatesOperationType.UpdateCoordinates;

            IList<GPSPoint> pkt = new List<GPSPoint>();
            pkt.Add(new GPSPoint(1, 2, 3, 4, 1));
            pkt.Add(new GPSPoint(11, 21, 31, 41, 2));
            pkt.Add(new GPSPoint(12, 22, 32, 42, 3));
            pkt.Add(new GPSPoint(13, 23, 23, 43, 4));
            pkt.Add(new GPSPoint(14, 24, 14, 44, 5));
            pkt.Add(new GPSPoint(15, 25, 35, 45, 6));

            var json = JsonConvert.SerializeObject(pkt);
            var jsonBytes = UTF8Encoding.UTF8.GetBytes(json);
            param.CoordinatesStream = new MemoryStream(jsonBytes.ToZip());
            param.CoordinatesStream.Seek(0, SeekOrigin.Begin);
            param.SessionId = data.Token.SessionId;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var entryDTO = service.GPSCoordinatesOperation(param);
                //distance
                Assert.IsNotNull(entryDTO.GPSTrackerEntry.Distance);
                Assert.AreNotEqual(0, entryDTO.GPSTrackerEntry.Distance.Value);
                //max speed
                Assert.IsNotNull(entryDTO.GPSTrackerEntry.MaxSpeed);
                Assert.AreEqual(45, entryDTO.GPSTrackerEntry.MaxSpeed.Value);
                //average speed
                Assert.IsNotNull(entryDTO.GPSTrackerEntry.AvgSpeed);
                Assert.AreEqual(499702.41356793m, entryDTO.GPSTrackerEntry.AvgSpeed.Value);
                //max altitude
                Assert.IsNotNull(entryDTO.GPSTrackerEntry.MaxAltitude);
                Assert.AreEqual(35, entryDTO.GPSTrackerEntry.MaxAltitude.Value);
                //min altitude
                Assert.IsNotNull(entryDTO.GPSTrackerEntry.MinAltitude);
                Assert.AreEqual(3, entryDTO.GPSTrackerEntry.MinAltitude.Value);
                //total ascents
                Assert.IsNotNull(entryDTO.GPSTrackerEntry.TotalAscent);
                Assert.AreEqual(50, entryDTO.GPSTrackerEntry.TotalAscent.Value);
                //total descents
                Assert.IsNotNull(entryDTO.GPSTrackerEntry.TotalDescent);
                Assert.AreEqual(18, entryDTO.GPSTrackerEntry.TotalDescent.Value);
                //duration
                Assert.IsNotNull(entryDTO.GPSTrackerEntry.Duration);
                Assert.AreEqual(6, entryDTO.GPSTrackerEntry.Duration.Value);
                //start and end
                Assert.AreEqual(DateTime.Today, entryDTO.GPSTrackerEntry.StartDateTime.Value);
                Assert.IsNull(entryDTO.GPSTrackerEntry.EndDateTime);
            });
            var dbEntry = Session.Get<GPSTrackerEntry>(param.Params.GPSTrackerEntryId);
            //distance
            Assert.IsNotNull(dbEntry.Distance);
            Assert.AreNotEqual(0, dbEntry.Distance.Value);
            //max speed
            Assert.IsNotNull(dbEntry.MaxSpeed);
            Assert.AreEqual(45, dbEntry.MaxSpeed.Value);
            //average speed
            Assert.IsNotNull(dbEntry.AvgSpeed);
            Assert.AreEqual(499702.41356793m, dbEntry.AvgSpeed.Value);
            //max altitude
            Assert.IsNotNull(dbEntry.MaxAltitude);
            Assert.AreEqual(35, dbEntry.MaxAltitude.Value);
            //min altitude
            Assert.IsNotNull(dbEntry.MinAltitude);
            Assert.AreEqual(3, dbEntry.MinAltitude.Value);
            //total ascents
            Assert.IsNotNull(dbEntry.TotalAscent);
            Assert.AreEqual(50, dbEntry.TotalAscent.Value);
            //total descents
            Assert.IsNotNull(dbEntry.TotalDescent);
            Assert.AreEqual(18, dbEntry.TotalDescent.Value);
            //duration
            Assert.IsNotNull(dbEntry.Duration);
            Assert.AreEqual(6, dbEntry.Duration.Value);
            //start and end
            Assert.AreEqual(DateTime.Today, dbEntry.StartDateTime.Value);
            Assert.IsNull(dbEntry.EndDateTime);
        }

        [Test]
        public void Save_SortPointsByDateTime()
        {
            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Cardio, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            TrainingDay day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];
            GPSTrackerEntry entry = new GPSTrackerEntry();
            entry.Exercise = exercise;
            entry.Status = EntryObjectStatus.Planned;
            day.AddEntry(entry);
            Session.Save(day);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var param = new GPSOperationParam();
            param.Params.GPSTrackerEntryId = entry.GlobalId;
            param.Params.Operation = GPSCoordinatesOperationType.UpdateCoordinates;

            IList<GPSPoint> pkt = new List<GPSPoint>();
            pkt.Add(new GPSPoint(1, 2, 3, 4, 1));
            pkt.Add(new GPSPoint(11, 21, 31, 41, 2));
            pkt.Add(new GPSPoint(12, 22, 32, 42, 3));
            pkt.Add(new GPSPoint(13, 23, 23, 43, 4));
            //now switch datetime of two last points so the order in the list is different that the order by datetime
            pkt.Add(new GPSPoint(14, 24, 14, 44,5));
            pkt.Add(new GPSPoint(15, 25, 35, 45, 6));

            var json = JsonConvert.SerializeObject(pkt);
            var jsonBytes = UTF8Encoding.UTF8.GetBytes(json);
            param.CoordinatesStream = new MemoryStream(jsonBytes.ToZip());
            param.CoordinatesStream.Seek(0, SeekOrigin.Begin);
            param.SessionId = data.Token.SessionId;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var entryDTO = service.GPSCoordinatesOperation(param);
                Assert.AreEqual(6, entryDTO.GPSTrackerEntry.Duration.Value);
            });
        }

        [Test]
        public void Save_CalculateCalories_ForProfile()
        {
            profiles[0].Wymiary = new Wymiary();
            profiles[0].Wymiary.Weight = 93;
            profiles[0].Wymiary.Height = 188;
            profiles[0].Birthday = DateTime.Now.AddDays(-3).AddYears(-32);
            profiles[0].Gender = Gender.Male;
            insertToDatabase(profiles[0].Wymiary);
            insertToDatabase(profiles[0]);


            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Cardio, MechanicsType.Isolation,
                           ExerciseForceType.Pull);
            exercise.Met = 8.0m;
            insertToDatabase(exercise);


            TrainingDay day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];
            GPSTrackerEntry entry = new GPSTrackerEntry();
            entry.Exercise = exercise;
            entry.Status = EntryObjectStatus.Planned;
            day.AddEntry(entry);
            Session.Save(day);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var param = new GPSOperationParam();
            param.Params.GPSTrackerEntryId = entry.GlobalId;
            param.Params.Operation = GPSCoordinatesOperationType.UpdateCoordinates;

            IList<GPSPoint> pkt = new List<GPSPoint>();
            pkt.Add(new GPSPoint(1, 2, 3, 4, 1));
            pkt.Add(new GPSPoint(11, 21, 31, 41, 2));
            pkt.Add(new GPSPoint(12, 22, 32, 42, 3));
            pkt.Add(new GPSPoint(13, 23, 23, 43, 4));
            pkt.Add(new GPSPoint(14, 24, 14, 44,5));
            pkt.Add(new GPSPoint(15, 25, 35, 45, 6));

            var json = JsonConvert.SerializeObject(pkt);
            var jsonBytes = UTF8Encoding.UTF8.GetBytes(json);
            param.CoordinatesStream = new MemoryStream(jsonBytes.ToZip());
            param.CoordinatesStream.Seek(0, SeekOrigin.Begin);
            param.SessionId = data.Token.SessionId;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var entryDTO = service.GPSCoordinatesOperation(param);
                Assert.IsNotNull( entryDTO.GPSTrackerEntry.Calories);
            });
        }

        [Test]
        public void Save_CalculateAverageSpeed()
        {
            profiles[0].Wymiary = new Wymiary();
            profiles[0].Wymiary.Weight = 93;
            profiles[0].Wymiary.Height = 188;
            profiles[0].Birthday = DateTime.Now.AddDays(-3).AddYears(-32);
            profiles[0].Gender = Gender.Male;
            insertToDatabase(profiles[0].Wymiary);
            insertToDatabase(profiles[0]);


            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Cardio, MechanicsType.Isolation,
                           ExerciseForceType.Pull);
            exercise.Met = 8.0m;
            insertToDatabase(exercise);


            TrainingDay day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];
            GPSTrackerEntry entry = new GPSTrackerEntry();
            entry.Exercise = exercise;
            entry.Status = EntryObjectStatus.Planned;
            day.AddEntry(entry);
            Session.Save(day);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var param = new GPSOperationParam();
            param.Params.GPSTrackerEntryId = entry.GlobalId;
            param.Params.Operation = GPSCoordinatesOperationType.UpdateCoordinates;

            IList<GPSPoint> pkt = new List<GPSPoint>();
            pkt.Add(new GPSPoint(1, 2, 3, 4, 1));
            pkt.Add(new GPSPoint(11, 21, 31, 41, 2));
            pkt.Add(new GPSPoint(12, 22, 32, 42, 3));
            pkt.Add(new GPSPoint(13, 23, 23, 43, 4));
            pkt.Add(new GPSPoint(14, 24, 14, 44, 5));
            pkt.Add(new GPSPoint(15, 25, 35, 45, 6));

            var json = JsonConvert.SerializeObject(pkt);
            var jsonBytes = UTF8Encoding.UTF8.GetBytes(json);
            param.CoordinatesStream = new MemoryStream(jsonBytes.ToZip());
            param.CoordinatesStream.Seek(0, SeekOrigin.Begin);
            param.SessionId = data.Token.SessionId;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var entryDTO = service.GPSCoordinatesOperation(param);
                Assert.IsNotNull(entryDTO.GPSTrackerEntry.AvgSpeed);
                var calculated = entryDTO.GPSTrackerEntry.Distance/entryDTO.GPSTrackerEntry.Duration;
                string fgdfg = "";
            });
        }

        [Test]
        public void Save_DoNotOverwriteCalories_ForProfile()
        {
            profiles[0].Wymiary = new Wymiary();
            profiles[0].Wymiary.Weight = 93;
            profiles[0].Wymiary.Height = 188;
            profiles[0].Birthday = DateTime.Now.AddDays(-3).AddYears(-32);
            profiles[0].Gender = Gender.Male;
            insertToDatabase(profiles[0].Wymiary);
            insertToDatabase(profiles[0]);


            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Cardio, MechanicsType.Isolation,
                           ExerciseForceType.Pull);
            exercise.Met = 8.0m;
            insertToDatabase(exercise);


            TrainingDay day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];
            GPSTrackerEntry entry = new GPSTrackerEntry();
            entry.Exercise = exercise;
            entry.Calories = 200;
            day.AddEntry(entry);
            Session.Save(day);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var param = new GPSOperationParam();
            param.Params.GPSTrackerEntryId = entry.GlobalId;
            param.Params.Operation = GPSCoordinatesOperationType.UpdateCoordinates;

            IList<GPSPoint> pkt = new List<GPSPoint>();
            pkt.Add(new GPSPoint(1, 2, 3, 4, 1));
            pkt.Add(new GPSPoint(11, 21, 31, 41,2));
            pkt.Add(new GPSPoint(12, 22, 32, 42, 3));
            pkt.Add(new GPSPoint(13, 23, 23, 43, 4));
            pkt.Add(new GPSPoint(14, 24, 14, 44, 5));
            pkt.Add(new GPSPoint(15, 25, 35, 45, 6));

            var json = JsonConvert.SerializeObject(pkt);
            var jsonBytes = UTF8Encoding.UTF8.GetBytes(json);
            param.CoordinatesStream = new MemoryStream(jsonBytes.ToZip());
            param.CoordinatesStream.Seek(0, SeekOrigin.Begin);
            param.SessionId = data.Token.SessionId;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var entryDTO = service.GPSCoordinatesOperation(param);
                Assert.AreEqual(200m,entryDTO.GPSTrackerEntry.Calories);
            });
        }
        #endregion
        #endregion
        
    }
}
