using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.DataAccess.Converter.V4_V5;
using BodyArchitect.Model.Old;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Payments;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
using ExerciseType = BodyArchitect.Model.Old.ExerciseType;
using Gender = BodyArchitect.Model.Old.Gender;
using PublishStatus = BodyArchitect.Model.Old.PublishStatus;
using WorkoutPlanPurpose = BodyArchitect.Model.Old.WorkoutPlanPurpose;

namespace BodyArchitect.UnitTests.DbConverter.V4V5
{
    public class NHibernateTestFixtureBase
    {
        public const string DatabaseOldFilename = "TestOldDb.db";
        public const string DatabaseNewFilename = "TestNewDb.db";

        protected Configuration configuration;
        private ISession sessionNew;
        private ISession sessionOld;

        public ISession SessionNew
        {
            get { return sessionNew; }
        }
        public ISession SessionOld
        {
            get { return sessionOld; }
        }

        [TestFixtureSetUp]
        public virtual void createTestFixture()
        {
            NHibernateConfigurator.ConfigureOldDatabase(CreatePropertiesForSqlLite(DatabaseOldFilename));
            NHibernateConfigurator.ConfigureNewDatabase(CreatePropertiesForSqlLite(DatabaseNewFilename));

            CreateDatabase();
        }


        [SetUp]
        public virtual void setupTest()
        {
            CreateSchema(NHibernateConfigurator.NewDbConfiguration);
            CreateSchema(NHibernateConfigurator.OldDbConfiguration);
            sessionNew = NHibernateConfigurator.NewSessionFactory.OpenSession();
            sessionOld = NHibernateConfigurator.OldSessionFactory.OpenSession();

            CreateProfile("BodyArchitect");
            CreateSupplement("steryd", new Guid("F099FF98-BB78-4E99-AFA3-EE0355974CD9"));
        }

        [TearDown]
        protected virtual void endTest()
        {
            sessionNew.Close();
            sessionOld.Close();
            sessionOld = null;
            sessionNew = null;
        }

        [TestFixtureTearDown]
        protected virtual void endTestFixture()
        {
            NHibernateConfigurator.Close();
        }

        protected Model.Old.TrainingPlan CreateWorkoutPlan(Model.Old.Profile profile,string planContent,Guid globalId)
        {
            Model.Old.TrainingPlan oldPlan = new TrainingPlan();
            oldPlan.Profile = profile;
            oldPlan.PlanContent = planContent;
            oldPlan.PublishDate = DateTime.Now.Date;
            oldPlan.Purpose = WorkoutPlanPurpose.Strength;
            oldPlan.Language = "pl";
            oldPlan.Status = PublishStatus.Published;
            oldPlan.Name = "yyyyy";
            oldPlan.Author = "Roemk";
            oldPlan.CreationDate = DateTime.UtcNow.Date.AddHours(1);
            oldPlan.Difficult = TrainingPlanDifficult.Advanced;
            oldPlan.GlobalId = globalId;
            oldPlan.TrainingType = TrainingType.HST;
            insertToOldDatabase(oldPlan);
            return oldPlan;
        }



        protected virtual Dictionary<string, string> CreatePropertiesForSqlLite(string dbFile)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add("connection.driver_class", "NHibernate.Driver.SQLite20Driver");
            properties.Add("dialect", "NHibernate.Dialect.SQLiteDialect");
            properties.Add("connection.provider", "NHibernate.Connection.DriverConnectionProvider, NHibernate");
            //properties.Add("connection.provider", "BodyArchitect.UnitTests.DbConverter.V4V5.SQLiteInMemoryTestingConnectionProvider, BodyArchitect.UnitTests.DbConverter.V4V5");
            string connectionString = string.Format("Data Source={0};Version=3;New=True;Compress=True;", dbFile);
            //string connectionString = "Data Source=:memory:;Version=3;New=True;";
            properties.Add("connection.connection_string", connectionString);
            properties.Add("show_sql", "true");
            properties.Add("connection.release_mode", "on_close");
            properties.Add("query.substitutions", "true=1;false=0");
            properties.Add("generate_statistics", "true");
            return properties;
        }

        private void CreateDatabase()
        {
            //SqlCEDbHelper.CreateDatabaseFile(DatabaseFilename);
        }

        private void CreateSchema(Configuration configuration)
        {
            SchemaExport exp = new SchemaExport(configuration);
            exp.Execute(true, true, false);
        }

        protected void insertToNewDatabase(object obj)
        {
            ISession localSession = SessionNew;
            localSession.SaveOrUpdate(obj);
            localSession.Flush();
            //localSession.Clear();
        }

        protected void insertToOldDatabase(object obj)
        {
            ISession localSession = SessionOld;
            localSession.SaveOrUpdate(obj);
            localSession.Flush();
            //localSession.Clear();
        }

        protected void insertToNewDatabase<T>(IEnumerable list)
        {
            ISession localSession = SessionNew;
            foreach (T obj in list)
            {
                localSession.Save(obj);
            }
        }

        public Suplement CreateSupplement(string name, Guid globalId)
        {
            Model.Old.Suplement ex = new Suplement();
            ex.Name = name;
            ex.SuplementId = globalId;
            insertToOldDatabase(ex);
            return ex;
        }

        public Exercise CreateExercise(string name,Guid globalId,ExerciseType exerciseType=ExerciseType.Klatka, Profile profile=null)
        {
            Model.Old.Exercise ex=new Exercise(globalId);
            ex.Name = name;
            ex.Shortcut = name;
            ex.ExerciseType = exerciseType;
            ex.Profile = profile;
            insertToOldDatabase(ex);
            return ex;
        }


        public Profile CreateProfile(string username, Country country = null, Gender gender = Gender.NotSet, AccountType accountType = AccountType.Instructor,bool isDeleted=false)
        {
            Profile profile1 = new Profile();
            if (country != null)
            {
                profile1.CountryId = country.GeoId;
            }
            profile1.IsDeleted = isDeleted;
            profile1.Privacy = new ProfilePrivacy();
            profile1.Settings = new ProfileSettings();
            profile1.Gender = gender;
            profile1.UserName = username;
            profile1.Email = username + "@wfg.pl";
            profile1.Birthday = DateTime.Now;
            profile1.Statistics = new ProfileStatistics();
            SessionOld.SaveOrUpdate(profile1.Statistics);
            SessionOld.SaveOrUpdate(profile1);
            SessionOld.Flush();
            return profile1;
        }

        protected void Convert()
        {
            PaymentsManager manager = new PaymentsManager();
            var xmlStream = Helper.GetResource("BodyArchitect.UnitTests.DbConverter.V4V5.BAPoints.xml");
            DatabaseConverter converter = new DatabaseConverter(new BACallbackMock(), manager.Load(xmlStream));
            converter.Convert();
        }

    }
}
