using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Settings;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace BodyArchitect.UnitTests.V2
{
    public abstract class TestWcfServiceBase<IService,ServiceClass> : TestServiceBase
        where ServiceClass : class,IService
    {
        private static ServiceHost myServiceHost_;
        private Binding binding;
        private ChannelFactory<IService> serviceFactory;
        private Uri address = new Uri("http://localhost:8000/TestService/MyService");

        public override void createTestFixture()
        {
            BodyArchitectAccessService.EnableLog(false);//to invoke static constructor which set Log to true
            base.createTestFixture();

            ExceptionHandler.Default.EmailFeaturesEnabled = false;
            //HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();

            var baseAddress = new Uri("http://localhost:8000/TestService");
            myServiceHost_ = new ServiceHost(typeof(ServiceClass), baseAddress);
            binding = new BasicHttpBinding();
            
            myServiceHost_.AddServiceEndpoint(typeof(IService), binding, address);
            myServiceHost_.Description.Behaviors.Remove(typeof(ServiceAuthorizationBehavior));
            var apiKeyAuthorization=new ServiceAuthorizationBehavior();
            apiKeyAuthorization.ServiceAuthorizationManager = new APIKeyAuthorization();
            myServiceHost_.Description.Behaviors.Add(apiKeyAuthorization);
            
            var smb = new ServiceMetadataBehavior { HttpGetEnabled = true };
            myServiceHost_.Description.Behaviors.Add(smb);
            myServiceHost_.Open();
        }

        protected override void endTestFixture()
        {
            base.endTestFixture();
            myServiceHost_.Close();
        }

        protected IService CreateServiceProxy(params AddressHeader[] headers)
        {
            var endAddress = new EndpointAddress(address, headers);
            serviceFactory = new ChannelFactory<IService>(binding, endAddress);
            return serviceFactory.CreateChannel();
        }

        protected override void endTest()
        {
            base.endTest();
            serviceFactory.Close();
        }

    }
}
