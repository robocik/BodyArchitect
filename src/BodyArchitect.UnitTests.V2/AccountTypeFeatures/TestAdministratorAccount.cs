using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Portable;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Service.V2.Model.Reports;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using BodyArchitect.Shared;
using NUnit.Framework;
using Newtonsoft.Json;
using AccountType = BodyArchitect.Model.AccountType;
using ExerciseDoneWay = BodyArchitect.Model.ExerciseDoneWay;
using ExerciseForceType = BodyArchitect.Service.V2.Model.ExerciseForceType;
using ExerciseType = BodyArchitect.Service.V2.Model.ExerciseType;
using Gender = BodyArchitect.Service.V2.Model.Gender;
using MechanicsType = BodyArchitect.Model.MechanicsType;
using Mood = BodyArchitect.Model.Mood;
using Privacy = BodyArchitect.Model.Privacy;
using PublishStatus = BodyArchitect.Model.PublishStatus;
using ReminderType = BodyArchitect.Model.ReminderType;
using ReportStatus = BodyArchitect.Model.ReportStatus;
using TrainingPlan = BodyArchitect.Model.TrainingPlan;
using WeatherCondition = BodyArchitect.Service.V2.Model.WeatherCondition;

namespace BodyArchitect.UnitTests.V2.AccountTypeFeatures
{
    [TestFixture]
    public class TestAdministratorAccount : TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        private Exercise exercise;

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                profiles.Add(CreateProfile(Session, "test1", accountType: AccountType.Administrator));
                profiles.Add(CreateProfile(Session, "test2", accountType: AccountType.Administrator));
                profiles.Add(CreateProfile(Session, "test3", accountType: AccountType.Administrator));

                profiles[0].Statistics.SupplementEntriesCount = 40;
                profiles[0].Statistics.StrengthTrainingEntriesCount = 40;
                //set friendship
                profiles[0].Friends.Add(profiles[1]);
                profiles[1].Friends.Add(profiles[0]);
                Session.Update(profiles[0]);
                Session.Update(profiles[1]);

                exercise = CreateExercise(Session, null, "fg", "gf");
                tx.Commit();
            }
        }

        Service.V2.Model.TrainingPlans.TrainingPlan createWorkoutPlanObject(ProfileDTO profile)
        {
            var contentPlan = new Service.V2.Model.TrainingPlans.TrainingPlan();
            contentPlan.Author = "rtyt";
            contentPlan.CreationDate = new DateTime(2011,6,28,19,18,26);
            contentPlan.Name = "rtyre";
            contentPlan.Profile = profile;
            contentPlan.Language = "en";
            var day = new Service.V2.Model.TrainingPlans.TrainingPlanDay();
            day.Name = "Day 1";
            contentPlan.AddDay(day);
            var entry = new Service.V2.Model.TrainingPlans.TrainingPlanEntry();
            entry.Exercise = exercise.Map<ExerciseLightDTO>();
            day.AddEntry(entry);
            return contentPlan;
        }

        [Test]
        public void SaveNewReminder()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var reminder = new ReminderItemDTO();
            reminder.Name = "name";
            reminder.DateTime = DateTime.UtcNow;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var res = service.SaveReminder(data.Token, reminder);
                Assert.AreNotEqual(Guid.Empty, res.GlobalId);
                reminder.ProfileId = res.ProfileId;
                UnitTestHelper.CompareObjects(res, reminder, true);
            });
            var dbItem = Session.QueryOver<ReminderItem>().SingleOrDefault();
            Assert.AreEqual(ReminderType.Custom, dbItem.Type);

        }

        [Test]
        public void UpdateReminder()
        {
            var reminder = CreateReminder("test", profiles[0], DateTime.UtcNow.AddDays(1));
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var dto = reminder.Map<ReminderItemDTO>();
            dto.Name = "name";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var res = service.SaveReminder(data.Token, dto);
                UnitTestHelper.CompareObjects(res, dto, true);
            });
            var dbItem = Session.QueryOver<ReminderItem>().SingleOrDefault();
            Assert.AreEqual(ReminderType.Custom, dbItem.Type);

        }

        [Test]
        public void DeleteWorkoutPlan()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            SessionData data = CreateNewSession(profile, ClientInformation);
            var plan = createWorkoutPlanObject(profile);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var securityInfo = SecurityManager.EnsureAuthentication(data.Token);
                securityInfo.Licence.CurrentAccountType = BodyArchitect.Service.V2.Model.AccountType.Administrator;
                plan = Service.SaveWorkoutPlan(data.Token, plan);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var securityInfo = SecurityManager.EnsureAuthentication(data.Token);
                securityInfo.Licence.CurrentAccountType = BodyArchitect.Service.V2.Model.AccountType.Administrator;
                WorkoutPlanOperationParam param = new WorkoutPlanOperationParam();
                param.WorkoutPlanId = plan.GlobalId;
                param.Operation = SupplementsCycleDefinitionOperation.Delete;
                Service.WorkoutPlanOperation(data.Token, param);
            });
            var res = Session.QueryOver<Model.TrainingPlan>().RowCount();
            Assert.AreEqual(0, res);
        }

        [Test]
        public void PublishWorkoutPlan()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            SessionData data = CreateNewSession(profile, ClientInformation);
            var plan = createWorkoutPlanObject(profile);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var securityInfo = SecurityManager.EnsureAuthentication(data.Token);
                securityInfo.Licence.CurrentAccountType = BodyArchitect.Service.V2.Model.AccountType.Administrator;
                plan = Service.SaveWorkoutPlan(data.Token, plan);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var securityInfo = SecurityManager.EnsureAuthentication(data.Token);
                securityInfo.Licence.CurrentAccountType = BodyArchitect.Service.V2.Model.AccountType.Administrator;
                var param = new WorkoutPlanOperationParam();
                param.WorkoutPlanId = plan.GlobalId;
                param.Operation = SupplementsCycleDefinitionOperation.Publish;
                Service.WorkoutPlanOperation(data.Token, param);
            });
            var dbPlan = Session.Get<TrainingPlan>(plan.GlobalId);
            Assert.AreEqual(PublishStatus.Published, dbPlan.Status);
            Assert.IsNotNull(dbPlan.PublishDate);
        }

        [Test]
        public void AddWorkoutPlanToFavorites()
        {
            var profile = (ProfileDTO)profiles[1].Tag;

            SessionData data = CreateNewSession((ProfileDTO)profiles[0].Tag, ClientInformation);
            var plan = createWorkoutPlanObject(profile);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var securityInfo = SecurityManager.EnsureAuthentication(data.Token);
                securityInfo.Licence.CurrentAccountType = BodyArchitect.Service.V2.Model.AccountType.Administrator;
                plan = Service.SaveWorkoutPlan(data.Token, plan);
            });
            var dbPlan = Session.Get<BodyArchitect.Model.TrainingPlan>(plan.GlobalId);
            dbPlan.Status = PublishStatus.Published;
            insertToDatabase(dbPlan);
            data = CreateNewSession((ProfileDTO)profiles[1].Tag, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var securityInfo = SecurityManager.EnsureAuthentication(data.Token);
                securityInfo.Licence.CurrentAccountType = BodyArchitect.Service.V2.Model.AccountType.Administrator;
                var param = new WorkoutPlanOperationParam();
                param.WorkoutPlanId = plan.GlobalId;
                param.Operation = SupplementsCycleDefinitionOperation.AddToFavorites;
                service.WorkoutPlanOperation(data.Token, param);
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(1, dbProfile.FavoriteWorkoutPlans.Count);
            Assert.AreEqual(plan.GlobalId, dbProfile.FavoriteWorkoutPlans.ElementAt(0).GlobalId);
        }

        [Test]
        public void SaveWorkoutPlan()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            SessionData data = CreateNewSession(profile, ClientInformation);

            var plan = createWorkoutPlanObject(profile);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var res = Service.SaveWorkoutPlan(data.Token, plan);
                Assert.IsNotNull(res);
            });
        }

        [Test]
        public void CreateExercise()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            SessionData data = CreateNewSession(profile, ClientInformation);
            ExerciseDTO exercise = new ExerciseDTO();
            exercise.Name = "test1";
            exercise.Profile = profile;
            exercise.Shortcut = "SC";
            exercise.ExerciseType = ExerciseType.Lydki;
            exercise.Description = "desc";
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.Url = "url";

            ExerciseDTO exerciseDto = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                TimerService.UtcNow = DateTime.UtcNow.AddDays(-2).Date;
                exerciseDto = service.SaveExercise(data.Token, exercise);
            });
            var dbExercise = Session.Get<Exercise>(exerciseDto.GlobalId);
            Assert.IsNotNull(dbExercise);
        }

        [Test]
        public void AddExerciseToFavorites()
        {
            var exercise = CreateExercise(Session, profiles[1], "ex1", "ex1");

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ExerciseOperationParam();
                param.ExerciseId = exercise.GlobalId;
                param.Operation = FavoriteOperation.Add;
                service.ExerciseOperation(data.Token, param);
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(1, dbProfile.FavoriteExercises.Count);
            Assert.AreEqual(exercise.GlobalId, dbProfile.FavoriteExercises.ElementAt(0).GlobalId);
        }

        [Test]
        public void SaveTrainingDay_AddEntryWithMyPlace()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var myGym = CreateMyPlace("gym", profiles[0]);
            var exercise = CreateExercise(Session, profiles[0], "test", "t", Model.ExerciseType.Nogi, MechanicsType.Isolation, Model.ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            entry.MyPlace = myGym.Map<MyPlaceDTO>();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day).TrainingDay;
            });


            var dbEntry = Session.Get<StrengthTrainingEntry>(day.Objects[0].GlobalId);
            Assert.AreEqual(myGym.GlobalId, dbEntry.MyPlace.GlobalId);

        }
        [Test]
        public void SaveTrainingDay_SetDoneWay_StrengthTrainingitem()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", Model.ExerciseType.Nogi, MechanicsType.Isolation,
                           Model.ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            item.DoneWay = Service.V2.Model.ExerciseDoneWay.Dumbbell;
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);


            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                day = service.SaveTrainingDay(data.Token, day).TrainingDay;
            });
            Assert.AreEqual(Service.V2.Model.ExerciseDoneWay.Dumbbell, ((StrengthTrainingEntryDTO)day.Objects[0]).Entries[0].DoneWay);
            var dbItem = Session.Get<StrengthTrainingItem>(((StrengthTrainingEntryDTO)day.Objects[0]).Entries[0].GlobalId);
            Assert.AreEqual(BodyArchitect.Model.ExerciseDoneWay.Dumbbell, dbItem.DoneWay);
        }

        [Test]
        public void SaveTrainingDay_UpdateEntryWithDifferentMyPlace()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var myGym = CreateMyPlace("gym", profiles[0]);
            var exercise = CreateExercise(Session, profiles[0], "test", "t", (Model.ExerciseType)ExerciseType.Nogi, MechanicsType.Isolation, (Model.ExerciseForceType)ExerciseForceType.Pull);


            TrainingDay day = new TrainingDay(DateTime.UtcNow.AddDays(-1));
            day.Profile = profiles[0];
            var entry = new StrengthTrainingEntry();
            entry.MyPlace = myGym;
            entry.AddEntry(new StrengthTrainingItem() { Exercise = exercise });
            day.AddEntry(entry);
            insertToDatabase(day);

            SessionData data = CreateNewSession(profile, ClientInformation);

            var dtoDay = day.Map<TrainingDayDTO>();
            var defaultMyPlace = GetDefaultMyPlace(profiles[0]);
            ((StrengthTrainingEntryDTO)dtoDay.Objects[0]).MyPlace = defaultMyPlace.Map<MyPlaceDTO>();
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                dtoDay = service.SaveTrainingDay(data.Token, dtoDay).TrainingDay;
            });


            var dbEntry = Session.Get<StrengthTrainingEntry>(dtoDay.Objects[0].GlobalId);
            Assert.AreEqual(defaultMyPlace.GlobalId, dbEntry.MyPlace.GlobalId);

        }

        [Test]
        public void SaveTrainingDay_UpdateEntryWithoutChaningMyPlace()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var myGym = CreateMyPlace("gym", profiles[0]);
            var exercise = CreateExercise(Session, profiles[0], "test", "t", (Model.ExerciseType)ExerciseType.Nogi, MechanicsType.Isolation, (Model.ExerciseForceType)ExerciseForceType.Pull);


            TrainingDay day = new TrainingDay(DateTime.UtcNow.AddDays(-1));
            day.Profile = profiles[0];
            var entry = new StrengthTrainingEntry();
            entry.MyPlace = myGym;
            entry.AddEntry(new StrengthTrainingItem() { Exercise = exercise });
            day.AddEntry(entry);
            insertToDatabase(day);

            SessionData data = CreateNewSession(profile, ClientInformation);

            var dtoDay = day.Map<TrainingDayDTO>();
            var defaultMyPlace = GetDefaultMyPlace(profiles[0]);
            ((StrengthTrainingEntryDTO)dtoDay.Objects[0]).Comment = "test";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                dtoDay = service.SaveTrainingDay(data.Token, dtoDay).TrainingDay;
            });


            var dbEntry = Session.Get<StrengthTrainingEntry>(dtoDay.Objects[0].GlobalId);
            Assert.AreEqual(myGym.GlobalId, dbEntry.MyPlace.GlobalId);
            Assert.AreEqual("test", dbEntry.Comment);

        }

        [Test]
        public void SaveTrainingDay_AddNewSerieWithRestPause()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", (Model.ExerciseType)ExerciseType.Nogi, MechanicsType.Isolation, (Model.ExerciseForceType)ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            var day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];

            var entry = new StrengthTrainingEntry();
            entry.MyPlace = GetDefaultMyPlace(profiles[0]);
            day.AddEntry(entry);
            var item = new StrengthTrainingItem();
            item.Exercise = exercise;
            entry.AddEntry(item);
            var serie1 = new Serie("1x");
            serie1.Position = 0;
            item.AddSerie(serie1);
            var serie2 = new Serie("2x");
            serie2.Position = 1;
            item.AddSerie(serie2);

            var dayDto = day.Map<TrainingDayDTO>();
            SerieDTO newSet = new SerieDTO();
            newSet.Weight = 30;
            newSet.IsRestPause = true;
            ((StrengthTrainingEntryDTO)dayDto.Objects[0]).Entries[0].AddSerie(newSet);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, dayDto);
                dayDto = result.TrainingDay;
            });
            var dbSerie = Session.Get<Serie>(((StrengthTrainingEntryDTO)dayDto.Objects[0]).Entries[0].Series[2].GlobalId);
            Assert.IsTrue(dbSerie.IsRestPause);
        }

        [Test]
        public void SaveTrainingDay_UpdateSerieWithRestPause()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", (Model.ExerciseType)ExerciseType.Nogi, MechanicsType.Isolation, (Model.ExerciseForceType)ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            var day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];

            var entry = new StrengthTrainingEntry();
            entry.MyPlace = GetDefaultMyPlace(profiles[0]);
            day.AddEntry(entry);
            var item = new StrengthTrainingItem();
            item.Exercise = exercise;
            entry.AddEntry(item);
            var serie1 = new Serie("1x");
            serie1.Position = 0;
            item.AddSerie(serie1);
            var serie2 = new Serie("2x");
            serie2.Position = 1;
            item.AddSerie(serie2);
            var serie3 = new Serie("3x");
            serie3.Position = 2;
            item.AddSerie(serie3);
            insertToDatabase(day);

            var dayDto = day.Map<TrainingDayDTO>();
            ((StrengthTrainingEntryDTO)dayDto.Objects[0]).Entries[0].Series[0].IsRestPause = true;

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, dayDto);
                dayDto = result.TrainingDay;
            });
            var dbSerie = Session.Get<Serie>(((StrengthTrainingEntryDTO)dayDto.Objects[0]).Entries[0].Series[0].GlobalId);
            Assert.IsTrue(dbSerie.IsRestPause);
        }

        [Test]
        public void SaveTrainingDay_UpdateSerieWithRestPauseButWithoutChanigingRestPause()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", (Model.ExerciseType)ExerciseType.Nogi, MechanicsType.Isolation, (Model.ExerciseForceType)ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            var day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];

            var entry = new StrengthTrainingEntry();
            entry.MyPlace = GetDefaultMyPlace(profiles[0]);
            day.AddEntry(entry);
            var item = new StrengthTrainingItem();
            item.Exercise = exercise;
            entry.AddEntry(item);
            var serie1 = new Serie("1x");
            serie1.Position = 0;
            item.AddSerie(serie1);
            var serie2 = new Serie("2x");
            serie2.Position = 1;
            serie2.IsRestPause = true;
            item.AddSerie(serie2);
            var serie3 = new Serie("3x");
            serie3.Position = 2;
            item.AddSerie(serie3);
            insertToDatabase(day);

            var dayDto = day.Map<TrainingDayDTO>();
            ((StrengthTrainingEntryDTO)dayDto.Objects[0]).Entries[0].Series[1].IsCiezarBezSztangi = true;

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, dayDto);
                dayDto = result.TrainingDay;
            });
            var dbSerie = Session.Get<Serie>(((StrengthTrainingEntryDTO)dayDto.Objects[0]).Entries[0].Series[1].GlobalId);
            Assert.IsTrue(dbSerie.IsRestPause);
            Assert.IsTrue(dbSerie.IsCiezarBezSztangi);
        }

        [Test]
        public void SaveTrainingDay_AddNewSerieWithSuperSlow()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", (Model.ExerciseType)ExerciseType.Nogi, MechanicsType.Isolation, (Model.ExerciseForceType)ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            var day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];

            var entry = new StrengthTrainingEntry();
            entry.MyPlace = GetDefaultMyPlace(profiles[0]);
            day.AddEntry(entry);
            var item = new StrengthTrainingItem();
            item.Exercise = exercise;
            entry.AddEntry(item);
            var serie1 = new Serie("1x");
            serie1.Position = 0;
            item.AddSerie(serie1);
            var serie2 = new Serie("2x");
            serie2.Position = 1;
            item.AddSerie(serie2);

            var dayDto = day.Map<TrainingDayDTO>();
            SerieDTO newSet = new SerieDTO();
            newSet.Weight = 30;
            newSet.IsSuperSlow = true;
            ((StrengthTrainingEntryDTO)dayDto.Objects[0]).Entries[0].AddSerie(newSet);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, dayDto);
                dayDto = result.TrainingDay;
            });
            var dbSerie = Session.Get<Serie>(((StrengthTrainingEntryDTO)dayDto.Objects[0]).Entries[0].Series[2].GlobalId);
            Assert.IsTrue(dbSerie.IsSuperSlow);
        }

        [Test]
        public void SaveTrainingDay_UpdateSerieWithSuperSlow()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", (Model.ExerciseType)ExerciseType.Nogi, MechanicsType.Isolation, (Model.ExerciseForceType)ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            var day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];

            var entry = new StrengthTrainingEntry();
            entry.MyPlace = GetDefaultMyPlace(profiles[0]);
            day.AddEntry(entry);
            var item = new StrengthTrainingItem();
            item.Exercise = exercise;
            entry.AddEntry(item);
            var serie1 = new Serie("1x");
            serie1.Position = 0;
            item.AddSerie(serie1);
            var serie2 = new Serie("2x");
            serie2.Position = 1;
            item.AddSerie(serie2);
            var serie3 = new Serie("3x");
            serie3.Position = 2;
            item.AddSerie(serie3);
            insertToDatabase(day);

            var dayDto = day.Map<TrainingDayDTO>();
            ((StrengthTrainingEntryDTO)dayDto.Objects[0]).Entries[0].Series[0].IsSuperSlow = true;

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, dayDto);
                dayDto = result.TrainingDay;
            });
            var dbSerie = Session.Get<Serie>(((StrengthTrainingEntryDTO)dayDto.Objects[0]).Entries[0].Series[0].GlobalId);
            Assert.IsTrue(dbSerie.IsSuperSlow);
        }

        [Test]
        public void SaveTrainingDay_UpdateSerieWithSuperSlowButWithoutChanigingSuperSlow()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", (Model.ExerciseType)ExerciseType.Nogi, MechanicsType.Isolation, (Model.ExerciseForceType)ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            var day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];

            var entry = new StrengthTrainingEntry();
            entry.MyPlace = GetDefaultMyPlace(profiles[0]);
            day.AddEntry(entry);
            var item = new StrengthTrainingItem();
            item.Exercise = exercise;
            entry.AddEntry(item);
            var serie1 = new Serie("1x");
            serie1.Position = 0;
            item.AddSerie(serie1);
            var serie2 = new Serie("2x");
            serie2.Position = 1;
            serie2.IsSuperSlow = true;
            item.AddSerie(serie2);
            var serie3 = new Serie("3x");
            serie3.Position = 2;
            item.AddSerie(serie3);
            insertToDatabase(day);

            var dayDto = day.Map<TrainingDayDTO>();
            ((StrengthTrainingEntryDTO)dayDto.Objects[0]).Entries[0].Series[1].IsCiezarBezSztangi = true;

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, dayDto);
                dayDto = result.TrainingDay;
            });
            var dbSerie = Session.Get<Serie>(((StrengthTrainingEntryDTO)dayDto.Objects[0]).Entries[0].Series[1].GlobalId);
            Assert.IsTrue(dbSerie.IsSuperSlow);
            Assert.IsTrue(dbSerie.IsCiezarBezSztangi);
        }
        
        [Test]
        public void SaveTrainingDay_UpdateEntryWithDifferentDoneWay()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var myGym = CreateMyPlace("gym", profiles[0]);
            var exercise = CreateExercise(Session, profiles[0], "test", "t", (Model.ExerciseType)ExerciseType.Nogi, MechanicsType.Isolation, (Model.ExerciseForceType)ExerciseForceType.Pull);


            TrainingDay day = new TrainingDay(DateTime.UtcNow.AddDays(-1));
            day.Profile = profiles[0];
            var entry = new StrengthTrainingEntry();
            entry.MyPlace = myGym;
            entry.AddEntry(new StrengthTrainingItem() { Exercise = exercise, DoneWay = ExerciseDoneWay.Barbell });
            day.AddEntry(entry);
            insertToDatabase(day);

            SessionData data = CreateNewSession(profile, ClientInformation);

            var dtoDay = day.Map<TrainingDayDTO>();
            ((StrengthTrainingEntryDTO)dtoDay.Objects[0]).Entries[0].DoneWay = Service.V2.Model.ExerciseDoneWay.Default;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                dtoDay = service.SaveTrainingDay(data.Token, dtoDay).TrainingDay;
            });


            var dbEntry = Session.Get<StrengthTrainingEntry>(dtoDay.Objects[0].GlobalId);
            Assert.AreEqual(ExerciseDoneWay.Default, dbEntry.Entries.ElementAt(0).DoneWay);

        }

        [Test]
        public void SaveTrainingDay_UpdateEntryWithoutChaningDoneWay()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var myGym = CreateMyPlace("gym", profiles[0]);
            var exercise = CreateExercise(Session, profiles[0], "test", "t", (Model.ExerciseType)ExerciseType.Nogi, MechanicsType.Isolation, (Model.ExerciseForceType)ExerciseForceType.Pull);


            TrainingDay day = new TrainingDay(DateTime.UtcNow.AddDays(-1));
            day.Profile = profiles[0];
            var entry = new StrengthTrainingEntry();
            entry.MyPlace = myGym;
            entry.AddEntry(new StrengthTrainingItem() { Exercise = exercise });
            day.AddEntry(entry);
            insertToDatabase(day);

            SessionData data = CreateNewSession(profile, ClientInformation);

            var dtoDay = day.Map<TrainingDayDTO>();
            ((StrengthTrainingEntryDTO)dtoDay.Objects[0]).Comment = "test";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                dtoDay = service.SaveTrainingDay(data.Token, dtoDay).TrainingDay;
            });


            var dbEntry = Session.Get<StrengthTrainingEntry>(dtoDay.Objects[0].GlobalId);
            Assert.AreEqual(myGym.GlobalId, dbEntry.MyPlace.GlobalId);
            Assert.AreEqual("test", dbEntry.Comment);

        }

        [Test]
        public void SaveTrainingDay_Future()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.UtcNow.AddDays(2));
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ((MockTimerService)Service.Configuration.TimerService).UtcNow = DateTime.UtcNow.AddDays(2);
                day = Service.SaveTrainingDay(data.Token, day).TrainingDay;
            });
            var count = Session.QueryOver<TrainingDay>().RowCount();
            Assert.AreEqual(1, count);
        }

        [Test]
        public void DeleteTrainingDay_Future()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            DateTime date = DateTime.Now.AddDays(2);
            TrainingDay day = new TrainingDay(date);
            day.Profile = profiles[0];
            SizeEntry sizeEntry = new SizeEntry();
            sizeEntry.Wymiary = new Wymiary();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);
            insertToDatabase(day);

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
        public void SaveNewSupplementCycleDefinition()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var supplement = CreateSupplement("supp");
            var definition = new SupplementCycleDefinitionDTO();
            definition.Name = "name";
            definition.Language = "en";
            var week = new SupplementCycleWeekDTO();
            definition.Weeks.Add(week);
            var dosage = new SupplementCycleDosageDTO();
            dosage.Supplement = supplement.Map<SuplementDTO>();
            week.Dosages.Add(dosage);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                TimerService.UtcNow = DateTime.UtcNow.Date.AddDays(1);
                var savedDefinition = service.SaveSupplementsCycleDefinition(data.Token, definition);
                Assert.AreEqual(profile.GlobalId, savedDefinition.Profile.GlobalId);
                Assert.AreEqual(DateTime.UtcNow.Date.AddDays(1), savedDefinition.CreationDate);
                definition.CreationDate = savedDefinition.CreationDate;
                UnitTestHelper.CompareObjects(definition, savedDefinition, true);

                var dbDef = Session.Get<SupplementCycleDefinition>(savedDefinition.GlobalId);
                UnitTestHelper.CompareObjects(savedDefinition, dbDef.Map<SupplementCycleDefinitionDTO>());
            });
        }

        [Test]
        public void DeleteCycleDefinition()
        {
            var supp = CreateSupplement("ttt");
            var def = CreateSupplementsCycleDefinition("test", supp, profiles[0], PublishStatus.Private);
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new SupplementsCycleDefinitionOperationParam();
                param.Operation = SupplementsCycleDefinitionOperation.Delete;
                param.SupplementsCycleDefinitionId = def.GlobalId;
                service.SupplementsCycleDefinitionOperation(data.Token, param);
            });

            Assert.AreEqual(0, Session.QueryOver<SupplementCycleDefinition>().RowCount());
            Assert.AreEqual(0, Session.QueryOver<SupplementCycleWeek>().RowCount());
            Assert.AreEqual(0, Session.QueryOver<SupplementCycleDosage>().RowCount());
        }

        [Test]
        public void SupplementsCycleDefinitionAddToFavorites()
        {
            var supp = CreateSupplement("ttt");
            var def = CreateSupplementsCycleDefinition("test", supp, profiles[1]);
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new SupplementsCycleDefinitionOperationParam();
                param.Operation = SupplementsCycleDefinitionOperation.AddToFavorites;
                param.SupplementsCycleDefinitionId = def.GlobalId;
                service.SupplementsCycleDefinitionOperation(data.Token, param);
            });

            Assert.AreEqual(1, Session.QueryOver<SupplementCycleDefinition>().RowCount());
            var dbProfile = Session.Get<Profile>(profile1.GlobalId);
            Assert.AreEqual(1, dbProfile.FavoriteSupplementCycleDefinitions.Count);
        }

        [Test]
        public void SupplementsCycleDefinitionPublish()
        {
            var supp = CreateSupplement("ttt");
            var def = CreateSupplementsCycleDefinition("test", supp, profiles[0], PublishStatus.Private);
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                TimerService.UtcNow = DateTime.UtcNow.Date.AddDays(-1);
                var param = new SupplementsCycleDefinitionOperationParam();
                param.Operation = SupplementsCycleDefinitionOperation.Publish;
                param.SupplementsCycleDefinitionId = def.GlobalId;
                service.SupplementsCycleDefinitionOperation(data.Token, param);
            });

            var cycle = Session.QueryOver<SupplementCycleDefinition>().SingleOrDefault();
            Assert.AreEqual(PublishStatus.Published, cycle.Status);
            Assert.AreEqual(DateTime.UtcNow.Date.AddDays(-1), cycle.PublishDate);
        }

        #region Sizes privacy

        [Test]
        public void GetUsersWithAccessibleSizes()
        {
            setSizesPrivacy(Privacy.Private, profiles[0]);
            setSizesPrivacy(Privacy.FriendsOnly, profiles[1]);
            setSizesPrivacy(Privacy.Private, profiles[2]);
            var criteria = UserSearchCriteria.CreateAllCriteria();
            criteria.AccessSizes = PrivacyCriteria.Accessible;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<UserSearchDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetUsers(data.Token, criteria, pageInfo);
            });
            Assert.AreEqual(1, result.AllItemsCount);
            Assert.AreEqual(profiles[1].GlobalId, result.Items[0].GlobalId);
            Assert.AreEqual(BodyArchitect.Service.V2.Model.Privacy.FriendsOnly, result.Items[0].Privacy.Sizes);
        }

        [Test]
        public void GetUsersWithAccessibleSizesAnotherUser()
        {
            setSizesPrivacy(Privacy.Private, profiles[0]);
            setSizesPrivacy(Privacy.FriendsOnly, profiles[1]);
            setSizesPrivacy(Privacy.Private, profiles[2]);
            var criteria = UserSearchCriteria.CreateAllCriteria();
            criteria.AccessSizes = PrivacyCriteria.Accessible;

            var profile = (ProfileDTO)profiles[2].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<UserSearchDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetUsers(data.Token, criteria, pageInfo);
            });
            Assert.AreEqual(0, result.AllItemsCount);
        }

        [Test]
        public void GetProfileInformation_Wymiary_Myselft()
        {
            setSizesPrivacy(Privacy.Private);
            profiles[0].Wymiary = new Wymiary();
            profiles[0].Wymiary.Height = 100;
            insertToDatabase(profiles[0]);

            var profile0 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile0, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var profileInfo = Service.GetProfileInformation(data.Token, criteria);
                Assert.IsNotNull(profileInfo.Wymiary);
                Assert.AreEqual(100, profileInfo.Wymiary.Height);
                Assert.AreEqual(BodyArchitect.Service.V2.Model.Privacy.Private, profileInfo.User.Privacy.Sizes);
            });

        }

        [Test]
        public void GetProfileInformation_Wymiary_PrivacyFriendsOnly()
        {
            profiles[0].Wymiary = new Wymiary();
            profiles[0].Wymiary.Height = 100;
            insertToDatabase(profiles[0]);

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
            Assert.AreEqual(BodyArchitect.Service.V2.Model.Privacy.FriendsOnly, profileInfo.User.Privacy.Sizes);
            Assert.IsNotNull(profileInfo.Wymiary);
            Assert.AreEqual(100, profileInfo.Wymiary.Height);

            data = CreateNewSession(profile2, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.AreEqual(BodyArchitect.Service.V2.Model.Privacy.FriendsOnly, profileInfo.User.Privacy.Sizes);
            Assert.IsNull(profileInfo.Wymiary);
        }

        [Test]
        public void GetProfileInformation_Wymiary_PrivacyPrivate()
        {
            profiles[0].Wymiary = new Wymiary();
            profiles[0].Wymiary.Height = 100;
            insertToDatabase(profiles[0]);

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
            Assert.AreEqual(BodyArchitect.Service.V2.Model.Privacy.Private, profileInfo.User.Privacy.Sizes);
            Assert.IsNull(profileInfo.Wymiary);
        }

        private void setSizesPrivacy(Privacy newPrivacy, Profile profile = null)
        {
            if (profile == null)
            {
                profile = profiles[0];
            }
            profile.Privacy.Sizes = newPrivacy;
            Session.Update(profile);
            Session.Flush();
            Session.Clear();
        }
        #endregion

        #region Calendar privacy

        [Test]
        public void GetProfileInformation_MySelf()
        {

            var profile0 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile0, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var profileInfo = Service.GetProfileInformation(data.Token, criteria);
                Assert.AreEqual(BodyArchitect.Service.V2.Model.Privacy.Private, profileInfo.User.Privacy.CalendarView);
            });
        }

        [Test]
        public void GetProfileInformation_OtherUser()
        {

            var profile0 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile0, ClientInformation);
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            criteria.UserId = profiles[0].GlobalId;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var profileInfo = Service.GetProfileInformation(data.Token, criteria);
                Assert.AreEqual(BodyArchitect.Service.V2.Model.Privacy.Private, profileInfo.User.Privacy.CalendarView);
            });
        }

        [Test]
        public void GetTrainingDays_FriendsOnly()
        {
            setPrivacy(Privacy.FriendsOnly);
            var day = new TrainingDay(DateTime.Now.AddDays(-2));
            day.Profile = profiles[0];
            var sizeEntry = new SizeEntry();
            sizeEntry.Wymiary = new Wymiary();
            sizeEntry.Wymiary.Height = 213;
            day.AddEntry(sizeEntry);
            insertToDatabase(day);

            PagedResult<TrainingDayDTO> days = null;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            var profile2 = (ProfileDTO)profiles[2].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            WorkoutDaysSearchCriteria searchCriteria = new WorkoutDaysSearchCriteria();
            searchCriteria.UserId = profiles[0].GlobalId;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                days = Service.GetTrainingDays(data.Token, searchCriteria, new PartialRetrievingInfo() { PageSize = PartialRetrievingInfo.AllElementsPageSize });
            });
            Assert.AreEqual(1, days.AllItemsCount);

            data = CreateNewSession(profile2, ClientInformation);
            searchCriteria = new WorkoutDaysSearchCriteria();
            searchCriteria.UserId = profiles[0].GlobalId;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                days = Service.GetTrainingDays(data.Token, searchCriteria, new PartialRetrievingInfo() { PageSize = PartialRetrievingInfo.AllElementsPageSize });
            });
            Assert.AreEqual(0, days.AllItemsCount);

        }

        private void setPrivacy(Privacy newPrivacy, Profile profile = null)
        {
            if (profile == null)
            {
                profile = profiles[0];
            }
            profile.Privacy.CalendarView = newPrivacy;
            Session.Update(profile);
            Session.Flush();
            Session.Clear();
        }


        [Test]
        public void GetTrainingDays_Private()
        {
            setPrivacy(Privacy.Private);

            var day = new TrainingDay(DateTime.Now.AddDays(-2));
            day.Profile = profiles[0];
            var sizeEntry = new SizeEntry();
            sizeEntry.Wymiary = new Wymiary();
            sizeEntry.Wymiary.Height = 213;
            day.AddEntry(sizeEntry);
            insertToDatabase(day);

            PagedResult<TrainingDayDTO> days = null;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            var profile2 = (ProfileDTO)profiles[2].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            WorkoutDaysSearchCriteria searchCriteria = new WorkoutDaysSearchCriteria();
            searchCriteria.UserId = profiles[0].GlobalId;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                days = Service.GetTrainingDays(data.Token, searchCriteria, new PartialRetrievingInfo() { PageSize = PartialRetrievingInfo.AllElementsPageSize });
            });
            Assert.AreEqual(0, days.AllItemsCount);

            data = CreateNewSession(profile2, ClientInformation);
            searchCriteria = new WorkoutDaysSearchCriteria();
            searchCriteria.UserId = profiles[0].GlobalId;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                days = Service.GetTrainingDays(data.Token, searchCriteria, new PartialRetrievingInfo() { PageSize = PartialRetrievingInfo.AllElementsPageSize });
            });
            Assert.AreEqual(0, days.AllItemsCount);

        }

        [Test]
        public void GetTrainingDay_Private()
        {
            setPrivacy(Privacy.Private);
            var day1 = new TrainingDay(DateTime.Now.AddDays(-2).Date);
            day1.Profile = profiles[0];
            var sizeEntry = new SizeEntry();
            sizeEntry.Wymiary = new Wymiary();
            sizeEntry.Wymiary.Height = 213;
            day1.AddEntry(sizeEntry);
            insertToDatabase(day1);

            var profile1 = (ProfileDTO)profiles[1].Tag;
            var profile2 = (ProfileDTO)profiles[2].Tag;

            WorkoutDayGetOperation op = new WorkoutDayGetOperation();
            op.Operation = GetOperation.Last;
            op.UserId = profiles[0].GlobalId;
            op.WorkoutDateTime = day1.TrainingDate;
            TrainingDayDTO day = null;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.GetTrainingDay(data.Token, op, new RetrievingInfo());
            });
            Assert.IsNull(day);

            data = CreateNewSession(profile2, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.GetTrainingDay(data.Token, op, new RetrievingInfo());
            });
            Assert.IsNull(day);
        }

        [Test]
        public void GetTrainingDay_FriendsOnly()
        {
            setPrivacy(Privacy.FriendsOnly);
            var day1 = new TrainingDay(DateTime.Now.AddDays(-2).Date);
            day1.Profile = profiles[0];
            var sizeEntry = new SizeEntry();
            sizeEntry.Wymiary = new Wymiary();
            sizeEntry.Wymiary.Height = 213;
            day1.AddEntry(sizeEntry);
            insertToDatabase(day1);

            var profile1 = (ProfileDTO)profiles[1].Tag;
            var profile2 = (ProfileDTO)profiles[2].Tag;

            WorkoutDayGetOperation op = new WorkoutDayGetOperation();
            op.Operation = GetOperation.Last;
            op.UserId = profiles[0].GlobalId;
            op.WorkoutDateTime = day1.TrainingDate;
            TrainingDayDTO day = null;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.GetTrainingDay(data.Token, op, new RetrievingInfo());
            });
            Assert.IsNotNull(day);

            data = CreateNewSession(profile2, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.GetTrainingDay(data.Token, op, new RetrievingInfo());
            });
            Assert.IsNull(day);
        }

        [Test]
        public void GetUsersWithAccessibleCalendar()
        {
            setPrivacy(Privacy.Private, profiles[0]);
            setPrivacy(Privacy.FriendsOnly, profiles[1]);
            setPrivacy(Privacy.Private, profiles[2]);
            var criteria = UserSearchCriteria.CreateAllCriteria();
            criteria.AccessCalendar = PrivacyCriteria.Accessible;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<UserSearchDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetUsers(data.Token, criteria, pageInfo);
            });
            Assert.AreEqual(1, result.AllItemsCount);
            Assert.AreEqual(profiles[1].GlobalId, result.Items[0].GlobalId);
            Assert.AreEqual(BodyArchitect.Service.V2.Model.Privacy.FriendsOnly, result.Items[0].Privacy.CalendarView);
        }

        [Test]
        public void GetUsersWithAccessibleAnotherUser()
        {
            setPrivacy(Privacy.Private, profiles[0]);
            setPrivacy(Privacy.FriendsOnly, profiles[1]);
            setPrivacy(Privacy.Private, profiles[2]);
            var criteria = UserSearchCriteria.CreateAllCriteria();
            criteria.AccessCalendar = PrivacyCriteria.Accessible;

            var profile = (ProfileDTO)profiles[2].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<UserSearchDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetUsers(data.Token, criteria, pageInfo);
            });
            Assert.AreEqual(0, result.AllItemsCount);
        }
        #endregion

        #region Reports

        [Test]
        public void ReportExerciseWeight_AnotherProfile_Private()
        {
            setPrivacy(Privacy.Private);
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercise, new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, null));

            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.Exercises.Add(exercise.GlobalId);
                param.UserId = profiles[0].GlobalId;
                var list = service.ReportExerciseWeight(data.Token, param);

                Assert.AreEqual(0, list.Count);
            });
        }

        [Test]
        public void ReportExerciseWeight_AnotherProfile_Friends_ForUnknownUser()
        {
            setPrivacy(Privacy.FriendsOnly);
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercise, new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, null));

            var profile = (ProfileDTO)profiles[2].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.Exercises.Add(exercise.GlobalId);
                param.UserId = profiles[0].GlobalId;
                var list = service.ReportExerciseWeight(data.Token, param);

                Assert.AreEqual(0, list.Count);
            });
        }

        [Test]
        public void ReportExerciseWeight_AnotherProfile_Friends_ForMyFriend()
        {
            setPrivacy(Privacy.FriendsOnly);

            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercise, new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, null));

            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.UserId = profiles[0].GlobalId;
                param.Exercises.Add(exercise.GlobalId);
                var list = service.ReportExerciseWeight(data.Token, param);

                Assert.AreEqual(1, list.Count);
            });
        }



        StrengthTrainingEntry addTrainingDaySet(Profile profile, Customer customer, DateTime date, Exercise exercise, params Tuple<int?, decimal?>[] sets)
        {
            var trainingDay = new TrainingDay(date);
            trainingDay.Customer = customer;
            trainingDay.Profile = profile;
            StrengthTrainingEntry entry = new StrengthTrainingEntry();
            entry.MyPlace = GetDefaultMyPlace(profile);
            trainingDay.AddEntry(entry);
            StrengthTrainingItem item = new StrengthTrainingItem();
            item.Exercise = exercise;
            entry.AddEntry(item);

            foreach (var tuple in sets)
            {
                Serie set1 = new Serie();
                set1.RepetitionNumber = tuple.Item1;
                set1.Weight = tuple.Item2;
                item.AddSerie(set1);
            }
            insertToDatabase(trainingDay);
            return entry;
        }

        [Test]
        public void ReportWeightRepetitions_AnotherProfile_Private()
        {
            setPrivacy(Privacy.Private);
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercise, new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, null));

            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportWeightRepetitionsParams();
                param.Exercises.Add(exercise.GlobalId);
                param.UserId = profiles[0].GlobalId;
                var list = service.ReportWeightRepetitions(data.Token, param);
                Assert.AreEqual(0, list.Count);
            });
        }

        [Test]
        public void ReportWeightRepetitions_AnotherProfile_Friends_ForUnknownUser()
        {
            setPrivacy(Privacy.FriendsOnly);
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercise, new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, null));

            var profile = (ProfileDTO)profiles[2].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportWeightRepetitionsParams();
                param.Exercises.Add(exercise.GlobalId);
                param.UserId = profiles[0].GlobalId;
                var list = service.ReportWeightRepetitions(data.Token, param);

                Assert.AreEqual(0, list.Count);
            });
        }

        [Test]
        public void ReportMeasurementsTime_ReportWeightRepetitions_AnotherProfile_Friends_ForMyFriend()
        {
            setPrivacy(Privacy.FriendsOnly);

            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercise, new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, null));

            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportWeightRepetitionsParams();
                param.UserId = profiles[0].GlobalId;
                param.Exercises.Add(exercise.GlobalId);
                var list = service.ReportWeightRepetitions(data.Token, param);

                Assert.AreEqual(1, list.Count);
            });
        }

        void AddTrainingDaySize(Profile profile, Customer customer, DateTime date, Wymiary wymiary, ReportStatus reportStatus = ReportStatus.ShowInReport)
        {
            var trainingDay = new TrainingDay(date);
            trainingDay.Customer = customer;
            trainingDay.Profile = profile;
            var entry = new SizeEntry();
            entry.Wymiary = wymiary;
            entry.ReportStatus = reportStatus;
            trainingDay.AddEntry(entry);

            insertToDatabase(trainingDay);
        }

        [Test]
        public void ReportMeasurementsTime_AnotherProfile_Private()
        {
            setPrivacy(Privacy.Private);
            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 100, Klatka = 55 });

            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportMeasurementsTimeParams();
                param.UserId = profiles[0].GlobalId;
                var list = service.ReportMeasurementsTime(data.Token, param);
                Assert.AreEqual(0, list.Count);
            });
        }

        [Test]
        public void AnotherProfile_Friends_ForUnknownUser()
        {
            setPrivacy(Privacy.FriendsOnly);
            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 100, Klatka = 55 });

            var profile = (ProfileDTO)profiles[2].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportMeasurementsTimeParams();
                param.UserId = profiles[0].GlobalId;
                var list = service.ReportMeasurementsTime(data.Token, param);
                Assert.AreEqual(0, list.Count);
            });
        }

        [Test]
        public void ReportMeasurementsTime_AnotherProfile_Friends_ForMyFriend()
        {
            setPrivacy(Privacy.FriendsOnly);

            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 100, Klatka = 55 });

            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportMeasurementsTimeParams();
                param.UserId = profiles[0].GlobalId;
                var list = service.ReportMeasurementsTime(data.Token, param);
                Assert.AreEqual(1, list.Count);
            });
        }

        [Test]
        public void ReportWeightRepetitions_EntriesWithDifferentMyPlace_ReturnAll()
        {
            var myPlace = CreateMyPlace("place1", profiles[0]);
            var item = addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercise, new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, 60));
            item.MyPlace = myPlace;
            insertToDatabase(item);
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercise, new Tuple<int?, decimal?>(5, 56));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercise, new Tuple<int?, decimal?>(5, 67));


            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportWeightRepetitionsParams();
                param.Exercises.Add(exercise.GlobalId);
                var list = service.ReportWeightRepetitions(data.Token, param);

                Assert.Greater(list.Count,0);
            });
        }

        [Test]
        public void ReportWeightRepetitions_EntriesWithDifferentMyPlace_ReturnOnlyForOneMyPlace()
        {
            var myPlace = CreateMyPlace("place1", profiles[0]);
            var item = addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercise, new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, 60));
            item.MyPlace = myPlace;
            insertToDatabase(item);
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercise, new Tuple<int?, decimal?>(5, 56));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercise, new Tuple<int?, decimal?>(5, 67));


            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportWeightRepetitionsParams();
                param.Exercises.Add(exercise.GlobalId);
                param.MyPlaces.Add(myPlace.GlobalId);
                var list = service.ReportWeightRepetitions(data.Token, param);

                Assert.Greater(list.Count, 0);
            });
        }

        [Test]
        public void ReportExerciseWeight_EntriesWithDifferentMyPlace_ReturnAll()
        {
            var myPlace = CreateMyPlace("place1", profiles[0]);
            var item = addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercise, new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, 60));
            item.MyPlace = myPlace;
            insertToDatabase(item);
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercise, new Tuple<int?, decimal?>(5, 56));

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.Exercises.Add(exercise.GlobalId);
                var list = service.ReportExerciseWeight(data.Token, param);

                Assert.AreEqual(2,list.Count);
            });
        }

        [Test]
        public void ReportWeightRepetitions_SetsWithRestPauseTrueOnly()
        {
            var item = addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercise, new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, 60));
            item.Entries.ElementAt(0).Series.ElementAt(0).IsRestPause = true;
            insertToDatabase(item);

            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercise, new Tuple<int?, decimal?>(5, 56));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercise, new Tuple<int?, decimal?>(5, 67));


            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportWeightRepetitionsParams();
                param.Exercises.Add(exercise.GlobalId);
                param.RestPause = true;
                var list = service.ReportWeightRepetitions(data.Token, param);
                Assert.AreEqual(1, list.Count);
            });
        }

        [Test]
        public void ReportExerciseWeight_SetsWithRestPauseTrueOnly()
        {
            var item = addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercise, new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, 60));
            item.Entries.ElementAt(0).Series.ElementAt(0).IsRestPause = true;
            insertToDatabase(item);
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercise, new Tuple<int?, decimal?>(5, 56));

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.Exercises.Add(exercise.GlobalId);
                param.RestPause = true;
                var res=service.ReportExerciseWeight(data.Token, param);
                Assert.AreEqual(1, res.Count);
            });
        }

        [Test]
        public void ReportExerciseWeight_EntriesWithDifferentMyPlace_ReturnOnlyForOneMyPlace()
        {
            var myPlace = CreateMyPlace("place1", profiles[0]);
            var item = addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercise, new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, 60));
            item.MyPlace = myPlace;
            insertToDatabase(item);
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercise, new Tuple<int?, decimal?>(5, 56));

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.Exercises.Add(exercise.GlobalId);
                param.MyPlaces.Add(myPlace.GlobalId);
                var list = service.ReportExerciseWeight(data.Token, param);

                Assert.AreEqual(1,list.Count);
            });
        }

        [Test]
        public void ReportExerciseWeight_EntriesWithDifferentDoneWay_ReturnAll()
        {
            var item = addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercise, new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, 60));
            item.Entries.ElementAt(0).DoneWay = ExerciseDoneWay.Barbell;
            insertToDatabase(item);
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercise, new Tuple<int?, decimal?>(5, 56));

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.Exercises.Add(exercise.GlobalId);
                var list = service.ReportExerciseWeight(data.Token, param);

                Assert.AreEqual(2, list.Count);
            });
        }

        [Test]
        public void ReportExerciseWeight_EntriesWithDifferentDoneWay_ReturnOnlySpecific()
        {
            var item = addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercise, new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, 60));
            item.Entries.ElementAt(0).DoneWay = ExerciseDoneWay.Barbell;
            insertToDatabase(item);
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercise, new Tuple<int?, decimal?>(5, 56));

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.Exercises.Add(exercise.GlobalId);
                param.DoneWays.Add(Service.V2.Model.ExerciseDoneWay.Barbell);
                var list = service.ReportExerciseWeight(data.Token, param);

                Assert.AreEqual(1, list.Count);
            });
        }

        [Test]
        public void ReportWeightRepetitions_EntriesWithDifferentDoneWays_ReturnAll()
        {
            var item = addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercise, new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, 60));
            item.Entries.ElementAt(0).DoneWay = ExerciseDoneWay.Barbell;
            insertToDatabase(item);
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercise, new Tuple<int?, decimal?>(5, 56));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercise, new Tuple<int?, decimal?>(5, 67));


            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportWeightRepetitionsParams();
                param.Exercises.Add(exercise.GlobalId);
                var list = service.ReportWeightRepetitions(data.Token, param);

                Assert.Greater(list.Count, 0);
            });
        }

        [Test]
        public void ReportWeightRepetitions_EntriesWithDifferentDoneWays_ReturnOnlySpecific()
        {
            var item = addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercise, new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, 60));
            item.Entries.ElementAt(0).DoneWay = ExerciseDoneWay.Barbell;
            insertToDatabase(item);
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercise, new Tuple<int?, decimal?>(5, 56));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercise, new Tuple<int?, decimal?>(5, 67));


            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportWeightRepetitionsParams();
                param.Exercises.Add(exercise.GlobalId);
                param.DoneWays.Add(Service.V2.Model.ExerciseDoneWay.Barbell);
                var list = service.ReportWeightRepetitions(data.Token, param);

                Assert.Greater(list.Count, 0);
            });
        }
        #endregion

        #region Instructor

        [Test]
        public void DeleteCustomer()
        {
            var customer = CreateCustomer("name", profiles[0]);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteCustomer(data.Token, customerDto);
            });
            Assert.That(Session.QueryOver<Customer>().RowCount(), Is.EqualTo(0));
        }

        [Test]
        public void SaveNewCustomer()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = new CustomerDTO();
            customerDto.LastName = customerDto.FirstName = "name";
            customerDto.Gender = Gender.Male;
            customerDto.Birthday = DateTime.UtcNow.AddYears(-20);
            customerDto.Email = "email@net.com";
            customerDto.PhoneNumber = "234553";

            CustomerDTO savedCustomer = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                savedCustomer = Service.SaveCustomer(data.Token, customerDto);
                customerDto.ProfileId = savedCustomer.ProfileId;
            });
            var dbCustomer = Session.Get<Customer>(savedCustomer.GlobalId);
            Assert.IsNotNull(dbCustomer);
        }

        [Test]
        public void UpdateCustomer()
        {
            var customer = CreateCustomer("name", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();
            customerDto.LastName = "new last";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveCustomer(data.Token, customerDto);
            });
            Assert.AreEqual(1, Session.QueryOver<Customer>().RowCount());
        }


        [Test]
        public void SaveTrainingDay_ForCustomer()
        {
            var cust = CreateCustomer("cust", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.UtcNow.AddDays(2));
            day.ProfileId = profile.GlobalId;
            day.CustomerId = cust.GlobalId;
            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ((MockTimerService)Service.Configuration.TimerService).UtcNow = DateTime.UtcNow.AddDays(2);
                day = Service.SaveTrainingDay(data.Token, day).TrainingDay;
            });
            var count = Session.QueryOver<TrainingDay>().RowCount();
            Assert.AreEqual(1, count);
        }

        [Test]
        public void DeleteTrainingDay_ForCustomer()
        {
            var cust = CreateCustomer("cust", profiles[0]);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            DateTime date = DateTime.Now.AddDays(2);
            TrainingDay day = new TrainingDay(date);
            day.Profile = profiles[0];
            day.Customer = cust;
            SizeEntry sizeEntry = new SizeEntry();
            sizeEntry.Wymiary = new Wymiary();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);
            insertToDatabase(day);

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
        public void SaveNewCustomerGroup()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            CustomerGroupDTO group = new CustomerGroupDTO();
            group.Name = "Children";
            group.MaxPersons = 10;
            group.Color = System.Drawing.Color.Aqua.ToColorString();
            CustomerGroupDTO savedGroup = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                savedGroup = service.SaveCustomerGroup(data.Token, group);
                Assert.AreEqual(profile.GlobalId, savedGroup.ProfileId);
                group.ProfileId = savedGroup.ProfileId;
                Assert.AreNotEqual(Guid.Empty, savedGroup.GlobalId);
                Assert.AreEqual(1, savedGroup.Version);
            });
            UnitTestHelper.CompareObjects(group, savedGroup, true);
            var db = Session.QueryOver<CustomerGroup>().SingleOrDefault();
            Assert.AreEqual(profile.GlobalId, db.Profile.GlobalId);
        }

        [Test]
        public void DeleteGroup()
        {
            var group = CreateCustomerGroup("test", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.DeleteCustomerGroup(data.Token, group.Map<CustomerGroupDTO>());
            });
            var count = Session.QueryOver<CustomerGroup>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void UpdateCustomerGroup()
        {
            var group = CreateCustomerGroup("test", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var dto = group.Map<CustomerGroupDTO>();
            dto.Name = "name";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                TimerService.UtcNow = DateTime.UtcNow.AddHours(2);
                var res = service.SaveCustomerGroup(data.Token, dto);
                UnitTestHelper.CompareDateTime(res.CreationDate, group.CreationDate);
                UnitTestHelper.CompareObjects(dto, res, true);
            });
            Assert.AreEqual(1, Session.QueryOver<CustomerGroup>().RowCount());

        }

        [Test]
        public void GetProducts()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var customer = CreateCustomer("cust1", profiles[0]);
            var activity = CreateActivity("Activity", profiles[0]);
            ScheduleEntry entry = new ScheduleEntry();
            entry.Profile = profiles[0];
            entry.MyPlace = GetDefaultMyPlace(profiles[0]);
            entry.Activity = activity;
            Session.Save(entry);
            ScheduleEntryReservation reservation = new ScheduleEntryReservation();
            reservation.Profile = profiles[0];
            reservation.DateTime = DateTime.UtcNow.Date.AddDays(-1);
            reservation.Customer = customer;
            reservation.Name = "test1";
            reservation.EnterDateTime = DateTime.UtcNow.AddDays(-1);
            reservation.LeaveDateTime = DateTime.UtcNow.AddDays(-1);
            reservation.ScheduleEntry = entry;
            insertToDatabase(reservation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new GetProductsParam();
                var retrievingInfo = new PartialRetrievingInfo();
                var list = service.GetProducts(data.Token, param, retrievingInfo);
                Assert.AreEqual(1, list.AllItemsCount);
            });
        }

        [Test]
        public void SaveChampionship()
        {
            var champ = CreateChampionshipEx(profiles[0], "test");
            var champDTO = champ.Map<ChampionshipDTO>();
            champDTO.Name = "other name";
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveChampionship(data.Token, champDTO);
            });

            var dbResult = Session.QueryOver<Championship>().SingleOrDefault();
            Assert.AreEqual(champDTO.Name, dbResult.Name);
        }

        [Test]
        public void GetChampionships()
        {
            var championShip1 = CreateChampionship(profiles[0], "name1");

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var criteria = new GetChampionshipsCriteria();
                var res = service.GetChampionships(data.Token, criteria, new PartialRetrievingInfo());

                Assert.AreEqual(1, res.AllItemsCount);
                Assert.AreEqual(1, res.Items.Count);
            });
        }

        [Test]
        public void SaveScheduleEntryRange()
        {
            var activity = CreateActivity("act", profiles[0]);
            SaveScheduleEntryRangeParam param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow.Date;
            param.EndDay = DateTime.UtcNow.Date.AddDays(7);
            var entry = new ScheduleEntryDTO();
            entry.StartTime = param.StartDay.AddHours(7);
            entry.EndTime = entry.StartTime.AddMinutes(90);
            entry.ActivityId = activity.GlobalId;
            entry.MaxPersons = 3;
            param.Entries.Add(entry);

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            IList<ScheduleEntryBaseDTO> list = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                list = service.SaveScheduleEntriesRange(data.Token, param);
                Assert.AreNotEqual(Guid.Empty, list[0].GlobalId);
            });
            var dbResult = Session.QueryOver<ScheduleEntry>().List();
            Assert.AreEqual(1, dbResult.Count);
        }

        [Test]
        public void DeleteScheduleEntry()
        {
            var activity = CreateActivity("act", profiles[0]);

            var entry = new ScheduleEntry();
            entry.MyPlace = GetDefaultMyPlace(profiles[0]);
            entry.StartTime = DateTime.UtcNow.Date.AddHours(7);
            entry.EndTime = entry.StartTime.AddMinutes(90);
            entry.Activity = activity;
            entry.Profile = profiles[0];
            entry.MaxPersons = 3;
            insertToDatabase(entry);

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteScheduleEntry(data.Token, entry.Map<ScheduleEntryDTO>());
            });
            Assert.AreEqual(0, Session.QueryOver<ScheduleEntry>().RowCount());
        }

        [Test]
        public void GetScheduleEntries()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new GetScheduleEntriesParam();
                var retrievingInfo = new PartialRetrievingInfo();
                var list = service.GetScheduleEntries(data.Token, param, retrievingInfo);
                Assert.AreEqual(0, list.AllItemsCount);
            });
        }
        #endregion

        #region MyPlace

        [Test]
        public void DeleteMyPlace()
        {
            var gym = CreateMyPlace("test", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new MyPlaceOperationParam();
                param.MyPlaceId = gym.GlobalId;
                param.Operation = MyPlaceOperationType.Delete;
                Service.MyPlaceOperation(data.Token, param);
            });
            var count = Session.QueryOver<MyPlace>().RowCount();
            Assert.AreEqual(3, count);
        }

        [Test]
        public void SaveNewMyPlace()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var gym = new MyPlaceDTO();
            gym.Name = "name";
            gym.Color = System.Drawing.Color.Aqua.ToColorString();
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var res = service.SaveMyPlace(data.Token, gym);
                Assert.Greater(res.CreationDate, DateTime.MinValue);
                Assert.AreNotEqual(Guid.Empty, res.GlobalId);
                Assert.AreEqual(profile.GlobalId, res.ProfileId);
            });
            Assert.AreEqual(4, Session.QueryOver<MyPlace>().RowCount());
        }
        #endregion

        #region GPS tracker

        [Test]
        public void SaveAndReadListOfCoordinates_StreamVersion()
        {
            var exercise = CreateExercise(Session, null, "test", "t", Model.ExerciseType.Cardio, MechanicsType.Isolation,
                           Model.ExerciseForceType.Pull);

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
            pkt.Add(new GPSPoint(12, 22, 32, 42, 3));
            pkt.Add(new GPSPoint(13, 23, 33, 43, 4));
            pkt.Add(new GPSPoint(14, 24, 34, 44, 5));
            pkt.Add(new GPSPoint(15, 25, 35, 45, 6));

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

            var dbGps=Session.Get<GPSTrackerEntry>(param.Params.GPSTrackerEntryId);
            Assert.IsNotNull(dbGps.Coordinates);
            Assert.AreNotEqual(0,dbGps.Coordinates.Content.Length);
        }

        [Test]
        public void SaveTrainingDay_WeatherInGpsTracker()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", (Model.ExerciseType) ExerciseType.Cardio, MechanicsType.Isolation,
                           (Model.ExerciseForceType) ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            var size = new SizeEntryDTO();
            size.Wymiary = new WymiaryDTO();
            day.AddEntry(size);
            var entry = new GPSTrackerEntryDTO();
            day.AddEntry(entry);
            entry.Distance = 2;
            entry.EndDateTime = DateTime.Now.Date.AddDays(1);
            entry.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.Weather = new WeatherDTO();
            entry.Weather.Condition = WeatherCondition.Blizzard;
            entry.Weather.Temperature = 33;

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var dbEntry = Session.Get<GPSTrackerEntry>(day.Objects[0].GlobalId);
            Assert.AreEqual(Model.WeatherCondition.Blizzard, dbEntry.Weather.Condition);
            Assert.AreEqual(33, dbEntry.Weather.Temperature);
        }
        #endregion
    }
}
