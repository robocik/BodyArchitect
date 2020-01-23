using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Web;

namespace Orchard.BodyArchitect.WCF
{
    public class APIKeyMessageInspector : IClientMessageInspector 
    {
        public const string HeaderAPIKey = "APIKey";
        public const string APIKey = "14375345-3755-46F7-AF3F-0D328E3A2CC0";
        public const string HeaderLanguage = "Lang";

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            request.Headers.Add(MessageHeader.CreateHeader(HeaderAPIKey, string.Empty, APIKey));
            //if (ServiceManager.Token != null)
            {
                request.Headers.Add(MessageHeader.CreateHeader(HeaderLanguage, string.Empty, CultureInfo.CurrentUICulture.Name));
            }
            return null;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            //Not used
        }

    }

    public class APIKeyBehavior : BehaviorExtensionElement, IEndpointBehavior
    {
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new APIKeyMessageInspector());
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public override System.Type BehaviorType
        {
            get { return typeof(APIKeyBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new APIKeyBehavior();
        }
    }
}