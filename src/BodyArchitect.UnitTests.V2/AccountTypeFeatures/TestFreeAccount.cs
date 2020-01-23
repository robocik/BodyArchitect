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
using Privacy = BodyArchitect.Model.Privacy;
using PublishStatus = BodyArchitect.Model.PublishStatus;
using ReminderType = BodyArchitect.Model.ReminderType;
using ReportStatus = BodyArchitect.Model.ReportStatus;
using TrainingPlan = BodyArchitect.Model.TrainingPlan;
using WeatherCondition = BodyArchitect.Service.V2.Model.WeatherCondition;

namespace BodyArchitect.UnitTests.V2.AccountTypeFeatures
{
    [TestFixture]
    public class TestFreeAccount : TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        private Exercise exercise;

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                profiles.Add(CreateProfile(Session, "test1",accountType:AccountType.User));
                profiles.Add(CreateProfile(Session, "test2", accountType: AccountType.User));
                profiles.Add(CreateProfile(Session, "test3", accountType: AccountType.User));

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
            contentPlan.CreationDate = new DateTime(2011, 6, 28, 19, 18, 26);
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
        [ExpectedException(typeof(LicenceException))]
        public void SaveNewReminder()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var reminder = new ReminderItemDTO();
            reminder.Name = "name";
            reminder.DateTime = DateTime.UtcNow;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveReminder(data.Token, reminder);
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
        public void UpdateReminder()
        {
            var reminder = CreateReminder("test", profiles[0], DateTime.UtcNow.AddDays(1));
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var dto = reminder.Map<ReminderItemDTO>();
            dto.Name = "name";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveReminder(data.Token, dto);
            });

        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
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
                securityInfo.Licence.CurrentAccountType = BodyArchitect.Service.V2.Model.AccountType.User;
                WorkoutPlanOperationParam param = new WorkoutPlanOperationParam();
                param.WorkoutPlanId = plan.GlobalId;
                param.Operation = SupplementsCycleDefinitionOperation.Delete;
                Service.WorkoutPlanOperation(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
        public void SaveWorkoutPlan()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            SessionData data = CreateNewSession(profile, ClientInformation);

            var plan = createWorkoutPlanObject(profile);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveWorkoutPlan(data.Token, plan);
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
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
                securityInfo.Licence.CurrentAccountType = BodyArchitect.Service.V2.Model.AccountType.User;
                var param = new WorkoutPlanOperationParam();
                param.WorkoutPlanId = plan.GlobalId;
                param.Operation = SupplementsCycleDefinitionOperation.AddToFavorites;
                service.WorkoutPlanOperation(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
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
                securityInfo.Licence.CurrentAccountType = BodyArchitect.Service.V2.Model.AccountType.User;
                var param = new WorkoutPlanOperationParam();
                param.WorkoutPlanId = plan.GlobalId;
                param.Operation = SupplementsCycleDefinitionOperation.Publish;
                Service.WorkoutPlanOperation(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
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

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                TimerService.UtcNow = DateTime.UtcNow.AddDays(-2).Date;
                service.SaveExercise(data.Token, exercise);
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
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
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
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
                Service.SaveTrainingDay(data.Token, day);
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
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
                service.SaveTrainingDay(data.Token, day);
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
        public void SaveTrainingDay_AddEntryWithMyPlace()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var myGym = CreateMyPlace("gym", profiles[0]);
            var exercise = CreateExercise(Session, profiles[0], "test", "t", (Model.ExerciseType) ExerciseType.Nogi, MechanicsType.Isolation, (Model.ExerciseForceType) ExerciseForceType.Pull);

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
                Service.SaveTrainingDay(data.Token, day);
            });

        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
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
                service.SaveTrainingDay(data.Token, dtoDay);
            });

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
        [ExpectedException(typeof(LicenceException))]
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
                service.SaveTrainingDay(data.Token, dtoDay);
            });

        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
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
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveTrainingDay(data.Token, dayDto);
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
        public void SaveTrainingDay_UpdateSerieWithRestPause()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", (Model.ExerciseType) ExerciseType.Nogi, MechanicsType.Isolation,(Model.ExerciseForceType) ExerciseForceType.Pull);

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
            ((StrengthTrainingEntryDTO) dayDto.Objects[0]).Entries[0].Series[0].IsRestPause = true;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveTrainingDay(data.Token, dayDto);
            });
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
            ((StrengthTrainingEntryDTO) dayDto.Objects[0]).Entries[0].Series[1].IsCiezarBezSztangi = true;

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, dayDto);
                dayDto = result.TrainingDay;
            });
            var dbSerie=Session.Get<Serie>(((StrengthTrainingEntryDTO) dayDto.Objects[0]).Entries[0].Series[1].GlobalId);
            Assert.IsTrue(dbSerie.IsRestPause);
            Assert.IsTrue(dbSerie.IsCiezarBezSztangi);
        }





        [Test]
        [ExpectedException(typeof(LicenceException))]
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
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveTrainingDay(data.Token, dayDto);
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
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

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveTrainingDay(data.Token, dayDto);
            });
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
        [ExpectedException(typeof(LicenceException))]
        public void SaveTrainingDay_UpdateEntryWithAddedNewItemWithBarbellDoneWay()
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
            var item = new StrengthTrainingItemDTO();
            item.DoneWay = Service.V2.Model.ExerciseDoneWay.Barbell;
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            ((StrengthTrainingEntryDTO)dtoDay.Objects[0]).AddEntry(item);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveTrainingDay(data.Token, dtoDay);
            });
        }

        [Test]
        public void SaveTrainingDay_UpdateEntryWithAddedNewItemWithDefaultDoneWay()
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
            var item = new StrengthTrainingItemDTO();
            item.DoneWay = Service.V2.Model.ExerciseDoneWay.Default;
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            ((StrengthTrainingEntryDTO)dtoDay.Objects[0]).AddEntry(item);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                dtoDay = service.SaveTrainingDay(data.Token, dtoDay).TrainingDay;
            });

            var dbEntry = Session.Get<StrengthTrainingEntry>(dtoDay.Objects[0].GlobalId);
            Assert.AreEqual(2, dbEntry.Entries.Count);
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
        [ExpectedException(typeof(LicenceException))]
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
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
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
                service.SaveSupplementsCycleDefinition(data.Token, definition);
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
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
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
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
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
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
            Assert.AreEqual(2, result.AllItemsCount);
            Assert.AreEqual(BodyArchitect.Service.V2.Model.Privacy.Public, result.Items[0].Privacy.Sizes);
            Assert.AreEqual(BodyArchitect.Service.V2.Model.Privacy.Public, result.Items[1].Privacy.Sizes);
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
            Assert.AreEqual(2, result.AllItemsCount);
            Assert.AreEqual(BodyArchitect.Service.V2.Model.Privacy.Public, result.Items[0].Privacy.Sizes);
            Assert.AreEqual(BodyArchitect.Service.V2.Model.Privacy.Public, result.Items[1].Privacy.Sizes);
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
            Assert.AreEqual(BodyArchitect.Service.V2.Model.Privacy.Public, profileInfo.User.Privacy.Sizes);
            Assert.IsNotNull(profileInfo.Wymiary);
            Assert.AreEqual(100, profileInfo.Wymiary.Height);

            data = CreateNewSession(profile2, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.AreEqual(BodyArchitect.Service.V2.Model.Privacy.Public, profileInfo.User.Privacy.Sizes);
            Assert.IsNotNull(profileInfo.Wymiary);
            Assert.AreEqual(100, profileInfo.Wymiary.Height);
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
            Assert.AreEqual(BodyArchitect.Service.V2.Model.Privacy.Public, profileInfo.User.Privacy.Sizes);
            Assert.IsNotNull(profileInfo.Wymiary);
            Assert.AreEqual(100, profileInfo.Wymiary.Height);

            data = CreateNewSession(profile2, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                profileInfo = Service.GetProfileInformation(data.Token, criteria);
            });
            Assert.AreEqual(BodyArchitect.Service.V2.Model.Privacy.Public, profileInfo.User.Privacy.Sizes);
            Assert.IsNotNull(profileInfo.Wymiary);
            Assert.AreEqual(100, profileInfo.Wymiary.Height);
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
                Assert.AreEqual(BodyArchitect.Service.V2.Model.Privacy.Public, profileInfo.User.Privacy.CalendarView);
            });
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
            Assert.AreEqual(2, result.AllItemsCount);
            Assert.AreEqual(BodyArchitect.Service.V2.Model.Privacy.Public, result.Items[0].Privacy.CalendarView);
            Assert.AreEqual(BodyArchitect.Service.V2.Model.Privacy.Public, result.Items[1].Privacy.CalendarView);
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
            Assert.AreEqual(2, result.AllItemsCount);
            Assert.AreEqual(BodyArchitect.Service.V2.Model.Privacy.Public, result.Items[0].Privacy.CalendarView);
            Assert.AreEqual(BodyArchitect.Service.V2.Model.Privacy.Public, result.Items[1].Privacy.CalendarView);
        }

        [Test]
        public void GetTrainingDay_Private()
        {
            //free account have always public calendar (no matter what Privacy is set)
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
            Assert.IsNotNull(day);

            data = CreateNewSession(profile2, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.GetTrainingDay(data.Token, op, new RetrievingInfo());
            });
            Assert.IsNotNull(day);
        }

        [Test]
        public void GetTrainingDay_FriendsOnly()
        {
            //free account have always public calendar (no matter what Privacy is set)
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
            Assert.IsNotNull(day);
        }

        [Test]
        public void GetTrainingDays_FriendsOnly()
        {
            //free account have always public calendar (no matter what Privacy is set)
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
            Assert.AreEqual(1, days.AllItemsCount);

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
            //free account have always public calendar (no matter what Privacy is set)
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
            Assert.AreEqual(1, days.AllItemsCount);

            data = CreateNewSession(profile2, ClientInformation);
            searchCriteria = new WorkoutDaysSearchCriteria();
            searchCriteria.UserId = profiles[0].GlobalId;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                days = Service.GetTrainingDays(data.Token, searchCriteria, new PartialRetrievingInfo() { PageSize = PartialRetrievingInfo.AllElementsPageSize });
            });
            Assert.AreEqual(1, days.AllItemsCount);

        }

#endregion

        #region Reports

        [Test]
        [ExpectedException(typeof(LicenceException))]
        public void ReportExerciseWeight_AnotherProfile_Private()
        {
            //free user has always public reports
            setPrivacy(Privacy.Private);
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercise, new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, null));

            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.Exercises.Add(exercise.GlobalId);
                param.UserId = profiles[0].GlobalId;
                service.ReportExerciseWeight(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
        public void ReportExerciseWeight_AnotherProfile_Friends_ForUnknownUser()
        {
            //free user has always public reports
            setPrivacy(Privacy.FriendsOnly);
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercise, new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, null));

            var profile = (ProfileDTO)profiles[2].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.Exercises.Add(exercise.GlobalId);
                param.UserId = profiles[0].GlobalId;
                service.ReportExerciseWeight(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
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
                service.ReportExerciseWeight(data.Token, param);
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
        [ExpectedException(typeof(LicenceException))]
        public void ReportWeightRepetitions_AnotherProfile_Private()
        {
            //free user has always public reports
            setPrivacy(Privacy.Private);
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercise, new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, null));

            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportWeightRepetitionsParams();
                param.Exercises.Add(exercise.GlobalId);
                param.UserId = profiles[0].GlobalId;
                service.ReportWeightRepetitions(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
        public void ReportWeightRepetitions_AnotherProfile_Friends_ForUnknownUser()
        {
            //free user has always public reports
            setPrivacy(Privacy.FriendsOnly);
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercise, new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, null));

            var profile = (ProfileDTO)profiles[2].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportWeightRepetitionsParams();
                param.Exercises.Add(exercise.GlobalId);
                param.UserId = profiles[0].GlobalId;
                service.ReportWeightRepetitions(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
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
                service.ReportWeightRepetitions(data.Token, param);
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
        [ExpectedException(typeof(LicenceException))]
        public void ReportMeasurementsTime_AnotherProfile_Private()
        {
            //free user has always public reports
            setPrivacy(Privacy.Private);
            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 100, Klatka = 55 });

            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportMeasurementsTimeParams();
                param.UserId = profiles[0].GlobalId;
                service.ReportMeasurementsTime(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
        public void AnotherProfile_Friends_ForUnknownUser()
        {
            //free user has always public reports
            setPrivacy(Privacy.FriendsOnly);
            AddTrainingDaySize(profiles[0], null, DateTime.UtcNow.Date.AddDays(-10), new Wymiary() { Height = 100, Klatka = 55 });

            var profile = (ProfileDTO)profiles[2].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportMeasurementsTimeParams();
                param.UserId = profiles[0].GlobalId;
                service.ReportMeasurementsTime(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
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
                service.ReportExerciseWeight(data.Token, param);
            });
        }


        [Test]
        [ExpectedException(typeof(LicenceException))]
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
                service.ReportWeightRepetitions(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
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
                service.ReportExerciseWeight(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
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
                service.ReportWeightRepetitions(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
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
                service.ReportWeightRepetitions(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
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
                service.ReportMeasurementsTime(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
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
                service.ReportWeightRepetitions(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
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
                service.ReportWeightRepetitions(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
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
                service.ReportExerciseWeight(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
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
                service.ReportExerciseWeight(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
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
                service.ReportExerciseWeight(data.Token, param);
            });
        }
        #endregion

        #region Instructor

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
        [ExpectedException(typeof(LicenceException))]
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
        }

        [Test]
        public void SaveTrainingDay_SaveTwoTheSameExercises_InNewEntry()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", (Model.ExerciseType) ExerciseType.Nogi, MechanicsType.Isolation,
                           (Model.ExerciseForceType) ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);

            item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            serie = new SerieDTO("10x10");
            item.AddSerie(serie);


            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            Assert.AreEqual(2, ((StrengthTrainingEntryDTO)day.Objects[0]).Entries.Count);
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
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
                Service.SaveTrainingDay(data.Token, day);
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
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
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
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
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
        public void SaveNewCustomerGroup()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            CustomerGroupDTO group = new CustomerGroupDTO();
            group.Name = "Children";
            group.MaxPersons = 10;

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveCustomerGroup(data.Token, group);
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
        public void DeleteGroup()
        {
            var group = CreateCustomerGroup("test", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.DeleteCustomerGroup(data.Token, group.Map<CustomerGroupDTO>());
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
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
                service.SaveCustomerGroup(data.Token, dto);
            });

        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
        public void GetProducts()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var customer = CreateCustomer("cust1", profiles[0]);
            var activity = CreateActivity("Activity", profiles[0]);
            ScheduleEntry entry = new ScheduleEntry();
            entry.Profile = profiles[0];
            entry.Activity = activity;
            entry.MyPlace = GetDefaultMyPlace(profiles[0]);
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
                service.GetProducts(data.Token, param, retrievingInfo);
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
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
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveScheduleEntriesRange(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
        public void SaveChampionship()
        {
            var champ = CreateChampionshipEx(profiles[0], "test");
            var champDTO = champ.Map<ChampionshipDTO>();

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveChampionship(data.Token, champDTO);
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
        public void GetChampionships()
        {
            var championShip1 = CreateChampionship(profiles[0], "name1");

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var criteria = new GetChampionshipsCriteria();
                service.GetChampionships(data.Token, criteria, new PartialRetrievingInfo());
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
        public void DeleteScheduleEntry()
        {
            var activity = CreateActivity("act", profiles[0]);

            var entry = new ScheduleEntry();
            entry.StartTime = DateTime.UtcNow.Date.AddHours(7);
            entry.EndTime = entry.StartTime.AddMinutes(90);
            entry.MyPlace = GetDefaultMyPlace(profiles[0]);
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
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
        public void GetScheduleEntries()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new GetScheduleEntriesParam();
                var retrievingInfo = new PartialRetrievingInfo();
                service.GetScheduleEntries(data.Token, param, retrievingInfo);
            });
        }
        #endregion

        #region MyPlace

        [Test]
        [ExpectedException(typeof(LicenceException))]
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
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
        public void SaveNewMyPlace()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var gym = new MyPlaceDTO();
            gym.Name = "name";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveMyPlace(data.Token, gym);
            });
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
                Assert.IsFalse(entryDTO.GPSTrackerEntry.HasCoordinates);
            });

            var dbGps = Session.Get<GPSTrackerEntry>(param.Params.GPSTrackerEntryId);
            Assert.IsNull(dbGps.Coordinates);
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
        public void SaveTrainingDay_WeatherInGpsTracker_NewEntry()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", (Model.ExerciseType)ExerciseType.Cardio, MechanicsType.Isolation,
                           (Model.ExerciseForceType)ExerciseForceType.Pull);

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

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveTrainingDay(data.Token, day);
            });
        }

        [Test]
        [ExpectedException(typeof(LicenceException))]
        public void SaveTrainingDay_WeatherInGpsTracker_UpdateWeather()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", (Model.ExerciseType)ExerciseType.Cardio, MechanicsType.Isolation,
                           (Model.ExerciseForceType)ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDay day = new TrainingDay(DateTime.Now);
            day.Profile = profiles[0];

            var entry = new GPSTrackerEntry();
            day.AddEntry(entry);
            entry.Distance = 2;
            entry.EndDateTime = DateTime.Now.Date.AddDays(1);
            entry.Exercise = exercise;
            entry.Weather = new Weather();
            entry.Weather.Condition = (Model.WeatherCondition) WeatherCondition.Blizzard;
            entry.Weather.Temperature = 33;
            insertToDatabase(day);

            var dayDTO = day.Map<TrainingDayDTO>();
            ((GPSTrackerEntryDTO) dayDTO.Objects[0]).Weather.Condition = WeatherCondition.BlowingSnow;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveTrainingDay(data.Token, dayDTO);
            });
        }

        [Test]
        public void SaveTrainingDay_WeatherInGpsTracker_UpdateEntryWithoutChangingWeather()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", (Model.ExerciseType)ExerciseType.Cardio, MechanicsType.Isolation,
                           (Model.ExerciseForceType)ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDay day = new TrainingDay(DateTime.Now);
            day.Profile = profiles[0];

            var entry = new GPSTrackerEntry();
            day.AddEntry(entry);
            entry.Distance = 2;
            entry.EndDateTime = DateTime.Now.Date.AddDays(1);
            entry.Exercise = exercise;
            entry.Weather = new Weather();
            entry.Weather.Condition = (Model.WeatherCondition)WeatherCondition.Blizzard;
            entry.Weather.Temperature = 33;
            insertToDatabase(day);

            var dayDTO = day.Map<TrainingDayDTO>();
            ((GPSTrackerEntryDTO) dayDTO.Objects[0]).Distance = 1000;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var result=Service.SaveTrainingDay(data.Token, dayDTO);
                Assert.AreEqual(WeatherCondition.Blizzard,((GPSTrackerEntryDTO)result.TrainingDay.Objects[0]).Weather.Condition);
                Assert.AreEqual(1000, ((GPSTrackerEntryDTO)result.TrainingDay.Objects[0]).Distance);
            });
        }
        #endregion
    }
}
