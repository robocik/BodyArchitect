using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.DataAccess.NHibernate.Mappings;
using BodyArchitect.Model;
using BodyArchitect.Service.Admin;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace BodyArchitect.UnitTests.DbConverter.V4V5
{
    public class AdminServiceTestFixtureBase
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

            ModelMapper mapping = new ModelMapper();
            mapping.AddMappings(typeof(ProfileMapping).Assembly.GetTypes());
            configuration.AddMapping(mapping.CompileMappingForAllExplicitlyAddedEntities());

            sessionFactory = configuration.BuildSessionFactory();
            NHibernateFactory.Initialize(sessionFactory);

            CreateDatabase();
        }

        protected void Run(Action<ISession> run)
        {
            Session.Flush();
            using (var session = sessionFactory.OpenSession())
            using (var tran = session.BeginTransaction())
            {
                run(session);
                tran.Commit();
            }

            Session.Clear();
        }

        [SetUp]
        public virtual void setupTest()
        {
            CreateSchema();
            session = sessionFactory.OpenSession();
            var profile = new Profile();
            profile.UserName = "Admin";
            profile.Email = "dfgdfg@wp.pl";
            profile.Gender = Gender.Male;
            insertToDatabase(profile);
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


        protected void RunServiceMethod(Action<AdminService> method)
        {
            var serviceSession = sessionFactory.OpenSession();
            using (serviceSession)
            {
                var service = new AdminService();
                NHibernateContextExtension wcfExtencsion = new NHibernateContextExtension(serviceSession);
                NHibernateContext.UnitTestContext = wcfExtencsion;
                method(service);
            }
            Session.Clear();
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
            properties.Add("generate_statistics", "true");


            properties.Add("show_sql", "true");
            properties.Add("connection.release_mode", "on_close");
            return properties;
        }

        protected virtual Dictionary<string, string> CreatePropertiesForSqlLite()
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add("connection.driver_class", "NHibernate.Driver.SQLite20Driver");
            properties.Add("dialect", "NHibernate.Dialect.SQLiteDialect");
            //properties.Add("connection.provider", "NHibernate.Connection.DriverConnectionProvider, NHibernate");
            properties.Add("connection.provider", "BodyArchitect.UnitTests.DbConverter.V4V5.SQLiteInMemoryTestingConnectionProvider, BodyArchitect.UnitTests.DbConverter.V4V5");
            //string connectionString = string.Format("Data Source={0};Version=3;New=True;Compress=True;", DatabaseFilename);
            string connectionString = "Data Source=:memory:;Version=3;New=True;";
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
