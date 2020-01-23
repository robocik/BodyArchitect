using System;
using System.Collections.Generic;
using BodyArchitect.DataAccess.NHibernate.Mappings;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Mapping.ByCode;
using Configuration = NHibernate.Cfg.Configuration;

namespace BodyArchitect.DataAccess.Converter.V4_V5
{
    public static class NHibernateConfigurator
    {
        public static ISessionFactory OldSessionFactory { get; private set; }
        public static Configuration NewDbConfiguration { get; private set; }
        public static ISessionFactory NewSessionFactory { get; private set; }

        public static Configuration OldDbConfiguration { get; private set; }

        public static void ConfigureMySqlDatabases(bool showSql=true)
        {
            //ConfigureOldDatabase(CreatePropertiesForMySql("dbOld", showSql));
            ConfigureNewDatabase(CreatePropertiesForMySql("db", showSql));
        }

    //    public static void ConfigureOldDatabase(Dictionary<string, string> properties)
    //    {
            
    //        var configuration = new Configuration { Properties = properties };

    //        OldSessionFactory = Fluently.Configure(configuration).Database(MySQLConfiguration.Standard)
    //.Mappings(m => m.FluentMappings.AddFromAssemblyOf<BodyArchitect.DataAccess.Converter.V4_V5.Mappings.Old.ProfileMapping>())
    //.ExposeConfiguration((cfg =>
    //                          {
    //                              var timeout = TimeSpan.FromMinutes(30).TotalSeconds;
    //                              cfg.SetProperty("command_timeout", timeout.ToString());
    //                          })).BuildSessionFactory();
    //        OldDbConfiguration = configuration;
    //    }

        public static void ConfigureNewDatabase(Dictionary<string, string> properties)
        {
            var configuration = new Configuration { Properties = properties };

            ModelMapper mapping = new ModelMapper();
            mapping.AddMappings(typeof(ProfileMapping).Assembly.GetTypes());
            configuration.AddMapping(mapping.CompileMappingForAllExplicitlyAddedEntities());
            NewDbConfiguration = configuration;
            NewSessionFactory = configuration.BuildSessionFactory();
        }

        static private  Dictionary<string, string> CreatePropertiesForMySql(string connectionString,bool showSql=true)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add("connection.driver_class", "NHibernate.Driver.MySqlDataDriver");
            properties.Add("dialect", "BodyArchitect.DataAccess.NHibernate.FixedMySql5Dialect, BodyArchitect.DataAccess.NHibernate");
            properties.Add("connection.connection_string_name", connectionString);
            properties.Add("show_sql", showSql.ToString());
            properties.Add("adonet.batch_size", "0");
            properties.Add("batch_size", "50");
            var timeout = TimeSpan.FromMinutes(30).TotalSeconds;
            properties.Add("command_timeout ", timeout.ToString());
            
            return properties;
        }

        public static void Close()
        {
            if(NewSessionFactory!=null)
            {
                NewSessionFactory.Close();
                NewSessionFactory = null;
            }
            if (OldSessionFactory != null)
            {
                OldSessionFactory.Close();
                OldSessionFactory = null;
            }
            OldDbConfiguration = null;
            NewDbConfiguration = null;
        }
    }
}
