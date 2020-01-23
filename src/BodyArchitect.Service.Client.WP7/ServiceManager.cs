using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Portable;
using BodyArchitect.Portable.Exceptions;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using BodyArchitect.WP7.Client.WCF;
using Microsoft.Phone.Net.NetworkInformation;
using Newtonsoft.Json;
using NetworkInterface = System.Net.NetworkInformation.NetworkInterface;

namespace BodyArchitect.Service.Client.WP7
{
    public class ServiceManager<T> where T : AsyncCompletedEventArgs
    {

        public class ServiceManagerOperationResult:EventArgs
        {
            private T result;
            private Exception _error;

            public ServiceManagerOperationResult(T res)
            {
                result = res;
            }

            public ServiceManagerOperationResult(T res,Exception ex)
            {
                result = res;
                _error = ex;
            }

            public T Result
            {
                get { return result; }
            }

            public Exception Error
            {
                get { return _error; }
            }
        }
        private Action<BodyArchitectAccessServiceClient, EventHandler<T>> method;
        private bool relog = false;
        private BodyArchitectAccessServiceClient client;

        public ServiceManager(Action<BodyArchitectAccessServiceClient, EventHandler<T>> method)
        {
            this.method = method;
        }
        public event EventHandler<ServiceManagerOperationResult> OperationCompleted;


        private void operationEvent(object sender,T e)
        {
            if (e.Error is FaultException<BAAuthenticationException> && !relog)
            {
                relog = true;
                ClientInformation info = Settings.GetClientInformation();

                client.LoginCompleted += (s, a) =>
                {
                    if (a.Result != null)
                    {
                        ApplicationState.Current.SessionData = a.Result;
                        ApplicationState.Current.SessionData.Token.Language = ApplicationState.CurrentServiceLanguage;
                        using (OperationContextScope scope = new OperationContextScope(client.InnerChannel))
                        {
                            ApplicationState.AddCustomHeaders();
                            method(client, operationEvent);
                        }
                        
                    }
                    else
                    {
                        onOperationCompleted(e,a.Error);  
                    }
                };
                using (OperationContextScope scope = new OperationContextScope(client.InnerChannel))
                {
                    ApplicationState.AddCustomHeaders();
                    client.LoginAsync(info, ApplicationState.Current.TempUserName, ApplicationState.Current.TempPassword);
                }
                
            }
           else
           {
               onOperationCompleted(e);  
           }
            
        }

        private void onOperationCompleted(T e,Exception ex=null)
        {
            if (OperationCompleted != null)
            {
                OperationCompleted(this, new ServiceManagerOperationResult(e, ex??e.Error));
            }
        }

        public bool Run(bool withoutLogging=false)
        {
            if (!NetworkInterface.GetIsNetworkAvailable() || !withoutLogging && ApplicationState.Current.IsOffline )
            {
                return false;
            }
            client = ApplicationState.CreateService();
            using (OperationContextScope scope = new OperationContextScope(client.InnerChannel))
            {
                ApplicationState.AddCustomHeaders();
                method(client, operationEvent);
            }
            
            return true;
        }

        
    }
}
