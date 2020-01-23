using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BodyArchitect.DataAccess.NHibernate.Mappings;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
using MappingConfiguration = NHibernate.Cfg.ConfigurationSchema.MappingConfiguration;

namespace BodyArchitect.UnitTests
{
    public class NHibernateTestFixtureBase
    {
        public const string DatabaseFilename = "TestDb.db";// "TestDb.sdf";

        protected ISessionFactory sessionFactory;
        protected Configuration configuration;
        private ISession session;

        public ISession Session
        {
            get { return session; }
        }

        [TestFixtureSetUp]
        public virtual void createTestFixture()
        {
            Dictionary<string, string> properties = CreatePropertiesForSqlLite();// CreatePropertiesForSqlCE();
            configuration = new Configuration { Properties = properties };

            try
            {
                var cfg = new Configuration();
                ModelMapper mapping = new ModelMapper();
                mapping.AddMappings(typeof(ProfileMapping).Assembly.GetTypes());
                cfg.AddMapping(mapping.CompileMappingForAllExplicitlyAddedEntities());
                cfg.Configure();

                sessionFactory = cfg.BuildSessionFactory();
            }
            catch (Exception ex)
            {
                string fgf = ex.Message;
            }


            CreateDatabase();
        }

        [SetUp]
        public virtual void setupTest()
        {
            CreateSchema();
            session = sessionFactory.OpenSession();
        }

        [TearDown]
        protected virtual void endTest()
        {
            session.Close();

        }

        [TestFixtureTearDown]
        protected virtual void endTestFixture()
        {
            if (sessionFactory != null)
            {
                sessionFactory.Close();
                sessionFactory = null;
            }
        }

        protected virtual Dictionary<string, string> CreatePropertiesForSqlCE()
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add("proxyfactory.factory_class", "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle");
            properties.Add("connection.driver_class", "NHibernate.Driver.SqlServerCeDriver");
            //properties.Add("connection.driver_class", "ManualMaster.UnitTests.MySqlServerCeDriver, ManualMaster.UnitTests");
            properties.Add("dialect", "NHibernate.Dialect.MsSqlCeDialect");
            properties.Add("connection.provider", "NHibernate.Connection.DriverConnectionProvider, NHibernate");
            string connectionString = string.Format("Data Source={0};", DatabaseFilename);
            properties.Add("connection.connection_string", connectionString);


            properties.Add("show_sql", "true");
            properties.Add("connection.release_mode", "on_close");
            return properties;
        }

        protected virtual Dictionary<string, string> CreatePropertiesForSqlLite()
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add("proxyfactory.factory_class", "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle");
            properties.Add("connection.driver_class", "NHibernate.Driver.SQLite20Driver");
            properties.Add("dialect", "NHibernate.Dialect.SQLiteDialect");
            //properties.Add("connection.provider", "NHibernate.Connection.DriverConnectionProvider, NHibernate");
            properties.Add("connection.provider", "BodyArchitect.UnitTests.SQLiteInMemoryTestingConnectionProvider, BodyArchitect.UnitTests");
            //string connectionString = string.Format("Data Source={0};Version=3;New=True;Compress=True;", DatabaseFilename);
            string connectionString = "Data Source=:memory:;Version=3;New=True;";
            properties.Add("connection.connection_string", connectionString);
            properties.Add("show_sql", "true");
            properties.Add("connection.release_mode", "on_close");
            properties.Add("query.substitutions", "true=1;false=0");
            return properties;
        }

        private void CreateDatabase()
        {
            //SqlCEDbHelper.CreateDatabaseFile(DatabaseFilename);
        }

        private void CreateSchema()
        {
            SchemaExport exp = new SchemaExport(configuration);
            exp.Execute(true, true, false);
        }

        protected void insertToDatabase(object obj)
        {
            ISession localSession = Session;
            localSession.SaveOrUpdate(obj);
            localSession.Flush();
            //localSession.Clear();
        }

        protected void deleteFromDatabase(object obj)
        {
            ISession localSession = Session;
            localSession.Delete(obj);
            //localSession.Flush();
            //localSession.Clear();
        }


        protected void insertToDatabase<T>(IEnumerable list)
        {
            ISession localSession = Session;
            foreach (T obj in list)
            {
                localSession.Save(obj);
            }
            //localSession.Flush();
            //localSession.Clear();
        }

        protected void refreshFromDb(IList list)
        {
            ISession localSession = Session;
            foreach (object obj in list)
            {
                localSession.Refresh(obj);
            }
        }

        protected void ClearSession()
        {
            Session.Close();
        }
    }
}
