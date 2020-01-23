using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using BodyArchitect.Portable;

namespace BodyArchitect.Client.WCF
{
    public class APIKeyMessageInspector : IClientMessageInspector
    {
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            request.Headers.Add(MessageHeader.CreateHeader(Constants.HeaderAPIKey, string.Empty, Shared.Constants.APIKey));
            //if (ServiceManager.Token != null)
            {
                request.Headers.Add(MessageHeader.CreateHeader(Constants.HeaderLanguage, string.Empty, CultureInfo.CurrentUICulture.Name));
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
