using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AutoMapper;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Model;
using BodyArchitect.Service;
using BodyArchitect.Service.Model;
using BodyArchitect.Service.Services;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using NHibernate;
using ExerciseDifficult = BodyArchitect.Model.ExerciseDifficult;
using ExerciseForceType = BodyArchitect.Model.ExerciseForceType;
using ExerciseType = BodyArchitect.Model.ExerciseType;
using Gender = BodyArchitect.Model.Gender;
using MechanicsType = BodyArchitect.Model.MechanicsType;
using Profile = BodyArchitect.Model.Profile;
using PublishStatus = BodyArchitect.Model.PublishStatus;
using Role = BodyArchitect.Model.Role;
using WorkoutPlanPurpose = BodyArchitect.Model.WorkoutPlanPurpose;

namespace BodyArchitect.UnitTests
{
    public class MockEmailService:IEMailService
    {
        public bool EMailSent { get; private set; }

        public void SendEMail(Profile profile, string subject, string message, params object[] args)
        {
            EMailSent = true;
        }

        public void Reset()
        {
            EMailSent = false;
        }
    }

    class MockPushNotificationService:IPushNotificationService
    {
        public void SendLiveTile(ISession session, Profile user,object param)
        {
            
        }

        public void SendLiveTile(string uri, string backgroundImageUrl, string title, int count)
        {
            
        }
    }

    public abstract class TestServiceBase : NHibernateTestFixtureBase
    {

        private SecurityManager manager;
        private ClientInformation clientInformation;

        public string ImagesFolder { get; private set; }

        private void configureLogging()
        {
            //var formatterStandard = new FormatterBuilder().TextFormatterNamed("Text Formatter").UsingTemplate(
            //    "Timestamp: {timestamp}{newline}Message: {message}{newline}Severity: {severity}{newline}Machine: {machine}{newline}Process Name: {processName}{newline}Extended Properties: {dictionary({key} - {value}{newline})}");
            //var conf = new ConfigurationSourceBuilder();
            //var logConfig = conf.ConfigureLogging();
            //logConfig.WithOptions.LogToCategoryNamed("Exception").SendTo.RollingFile(
            //    "ExceptionFileListener").WithHeader("----------------------------------------").WithFooter("----------------------------------------").ToFile(UserContext.Settings.ExceptionsLogFile);
            //logConfig.WithOptions.LogToCategoryNamed("General").SendTo.RollingFile(
            //    "FlatFile TraceListener").WithHeader("----------------------------------------").WithFooter("----------------------------------------").FormatWith(formatterStandard).ToFile(UserContext.Settings.StandardLogFile);
            //logConfig.WithOptions.LogToCategoryNamed("email").SendTo.Email("email").FormatWith(formatterStandard).UsingSmtpServer(
            //    ApplicationSettings.MailSmtp).WithUserNameAndPassword(ApplicationSettings.MailUserName,
            //                                                          ApplicationSettings.MailPassword).To(
            //                                                              ApplicationSettings.MailAccount).From(
            //                                                                  ApplicationSettings.MailAccount).UseSSL(true);
            //var configSource = new DictionaryConfigurationSource();
            //conf.UpdateConfigurationWithReplace(configSource);

            //EnterpriseLibraryContainer.Current = EnterpriseLibraryContainer.CreateDefaultContainer(configSource);

            BodyArchitect.Logger.Log.EnableExceptionLog = false;
            BodyArchitect.Logger.Log.EnableStandardLog = false;
        }


        public static TrainingPlan CreatePlan(ISession session, Profile profile1, string name, TrainingPlanDifficult difficult, TrainingType type, bool isPublished, string language, WorkoutPlanPurpose purpose, int days)
        {
            var workoutPlan = new TrainingPlan();
            workoutPlan.GlobalId = Guid.NewGuid();
            workoutPlan.Profile = profile1;
            workoutPlan.DaysCount = days;
            workoutPlan.Name = name;
            workoutPlan.Purpose = purpose;
            workoutPlan.Language = language;
            workoutPlan.TrainingType = type;
            workoutPlan.Difficult = difficult;
            workoutPlan.Author = "test";
            workoutPlan.PlanContent = "plan content";
            workoutPlan.Status = isPublished ? PublishStatus.Published : PublishStatus.Private;
            if (isPublished)
            {
                workoutPlan.PublishDate = DateTime.UtcNow;
            }
            session.Save(workoutPlan);
            workoutPlan.Tag = Mapper.Map<TrainingPlan, WorkoutPlanDTO>(workoutPlan);
            return workoutPlan;
        }

        public static Exercise CreateExercise(ISession session, Profile profile, string name, string shortCut, PublishStatus status, ExerciseType exerciseType = ExerciseType.Klatka, MechanicsType mechanicsType = MechanicsType.Compound, ExerciseForceType forceType = ExerciseForceType.Push, ExerciseDifficult difficult = ExerciseDifficult.One)
        {
            Exercise exercise = new Exercise(Guid.NewGuid());
            exercise.Profile = profile;
            exercise.Name = name;
            exercise.Shortcut = shortCut;
            exercise.ExerciseType = exerciseType;
            exercise.Status = status;
            if (status == PublishStatus.Published)
            {
                exercise.PublishDate = DateTime.UtcNow;
                
            }
            exercise.ExerciseForceType = forceType;
            exercise.MechanicsType = mechanicsType;
            exercise.Difficult = ExerciseDifficult.NotSet;
            session.Save(exercise);
            session.Flush();
            return exercise;
        }
        public override void createTestFixture()
        {
            base.createTestFixture();
            ObjectsConverter.Configure();
            configureLogging();
            clientInformation = new ClientInformation() { Version = Const.ServiceVersion };
            clientInformation.ApplicationLanguage = "en";
            clientInformation.PlatformVersion = "fgfdgdfg";
            clientInformation.ApplicationVersion = "1.0.0.0";
            var conf = new ConfigurationSourceBuilder();
            conf.ConfigureCaching().ForCacheManagerNamed(SecurityManager.AuthenticationCacheName).StoreInMemory();

            var configSource = new DictionaryConfigurationSource();
            conf.UpdateConfigurationWithReplace(configSource);

            EnterpriseLibraryContainer.Current = EnterpriseLibraryContainer.CreateDefaultContainer(configSource);
        }
        protected void RunServiceMethod(Action<InternalBodyArchitectService> method)
        {
            var serviceSession = sessionFactory.OpenSession();
            using (serviceSession)
            {
                var service = new InternalBodyArchitectService(serviceSession,SecurityManager, new MockEmailService(), new MockPushNotificationService());
                service.ImagesFolder = ImagesFolder;
                NHibernateContextExtension wcfExtencsion = new NHibernateContextExtension(serviceSession);
                NHibernateContext.UnitTestContext = wcfExtencsion;
                method(service);
            }
            Session.Clear();
        }
        public static Profile CreateProfile(ISession session, string username,Country country=null,Gender gender=Gender.NotSet)
        {
            Profile profile1 = new Profile();
            if(country!=null)
            {
                profile1.CountryId = country.GeoId;
            }
            profile1.Privacy=new ProfilePrivacy();
            profile1.Settings=new ProfileSettings();
            profile1.Gender = gender;
            profile1.UserName = username;
            profile1.Email = username + "@wfg.pl";
            profile1.Birthday = DateTime.Now;
            profile1.Statistics=new ProfileStatistics();
            session.SaveOrUpdate(profile1.Statistics);
            session.SaveOrUpdate(profile1);
            ProfileDTO dto = new ProfileDTO();
            dto.Id = profile1.Id;
            dto.UserName = profile1.UserName;
            dto.Birthday = profile1.Birthday;
            dto.Email = profile1.Email;
            dto.Settings = new ProfileSettingsDTO();
            dto.Settings.Id = profile1.Settings.Id;
            UnitTestHelper.SetProperty(dto, "Version", profile1.Version);
            profile1.Tag = dto;
            return profile1;
        }

        protected override void endTestFixture()
        {
            if (Directory.Exists(ImagesFolder))
            {
                Directory.Delete(ImagesFolder,true);
            }
            base.endTestFixture();
        }

        public SecurityManager SecurityManager
        {
            get { return manager; }
        }

        public ClientInformation ClientInformation
        {
            get { return clientInformation; }
        }

        public virtual void BuildDatabase()
        {
            
        }



        public override void setupTest()
        {
            base.setupTest();

            Profile profile = new Profile();
            profile.UserName = ProfileDTO.AdministratorName;
            profile.Email = "fdgfg@gmail.com";
            profile.Birthday = DateTime.Now.AddYears(-30);
            profile.Privacy = new ProfilePrivacy();
            profile.Privacy.Searchable = false;
            profile.CountryId = Country.GetByTwoLetters("PL").GeoId;
            profile.Role = Role.Administrator;
            insertToDatabase(profile);

            BuildDatabase();
            Session.Clear();
            manager = new SecurityManager();
           
            ImagesFolder = Path.Combine(Environment.CurrentDirectory, "Images");
            if (!Directory.Exists(ImagesFolder))
            {
                Directory.CreateDirectory(ImagesFolder);
            }
            
            
        }
    }
}
