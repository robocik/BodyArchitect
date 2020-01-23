using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using BodyArchitect.DataAccess.NHibernate.Mappings;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using ISession = NHibernate.ISession;

namespace BodyArchitect.DataAccess.NHibernate
{
    public class NHibernateContextInitializer : IInstanceContextInitializer
    {
        public void Initialize(InstanceContext instanceContext, Message message)
        {
            instanceContext.Extensions.Add(
                new NHibernateContextExtension(
                    NHibernateFactory.OpenSession()
                    )
                );
        }
    }

    public static class NHibernateFactory
    {
        private static ISessionFactory _sessionFactory;

        public static ITransaction BeginGetTransaction(this ISession session)
        {
            return session.BeginTransaction();
        }

        public static ITransaction BeginSaveTransaction(this ISession session)
        {
            //return session.BeginTransaction(IsolationLevel.Serializable);
            return session.BeginTransaction();
        }
       
        public static void Initialize()
        {
            //var fluentConf = Fluently.Configure().Mappings(delegate(MappingConfiguration mapping)
            //{
            //    mapping.FluentMappings.AddFromAssembly(Assembly.Load("BodyArchitect.DataAccess.NHibernate"));
            //});

            //Initialize(fluentConf.BuildSessionFactory());
            var cfg = new Configuration();

            ModelMapper mapping = new ModelMapper();
            mapping.AddMappings(typeof(ProfileMapping).Assembly.GetTypes());
            cfg.AddMapping(mapping.CompileMappingForAllExplicitlyAddedEntities());
            cfg.Configure();
            _sessionFactory = cfg.BuildSessionFactory();
        }

        public static void Initialize(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public static ISession OpenSession()
        {
            return _sessionFactory.OpenSession();
        }

    }

    public class NHibernateContextAttribute : Attribute, IContractBehavior
    {
        public ISessionFactory SessionFactory { private get; set; }

        public void Validate(ContractDescription contractDescription,
                                ServiceEndpoint endpoint)
        { }

        public void ApplyDispatchBehavior(ContractDescription contractDescription,
                                ServiceEndpoint endpoint,
                                DispatchRuntime dispatchRuntime)
        {
            dispatchRuntime.InstanceContextInitializers.Add(
                new NHibernateContextInitializer());
        }

        public void ApplyClientBehavior(ContractDescription contractDescription,
                                ServiceEndpoint endpoint,
                                ClientRuntime clientRuntime)
        { }

        public void AddBindingParameters(ContractDescription contractDescription,
                                ServiceEndpoint endpoint,
                                BindingParameterCollection bindingParameters)
        { }
    }

    public class NHibernateContext
    {
        public static NHibernateContextExtension UnitTestContext { get; set; }

        public static NHibernateContextExtension Current()
        {
            if (OperationContext.Current != null)
            {
                return OperationContext.Current.InstanceContext.Extensions.Find<NHibernateContextExtension>();
            }
            return UnitTestContext;
        }
    }
}
