using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Web.Configuration;
using System.Web.Hosting;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;

namespace BodyArchitect.Service.V2
{
    public class ServiceConfiguration
    {
        private string imagesFolder;
        private bool? maintenanceMode;
        private bool? requireActivateNewProfile;
        private ITimerService timerService;
        private Guid? apiKey;
        private PaymentsHolder paymentsHolder;

        public IMethodInvoker MethodInvoker { get; set; }

        public ServiceConfiguration(ITimerService timerService)
        {
            this.timerService = timerService;
        }

        

        public Guid CurrentApiKey
        {
            get
            {
                if (apiKey==null)
                {
                    return new Guid(OperationContext.Current.IncomingMessageHeaders.GetHeader<string>("APIKey", ""));
                }
                return apiKey.Value;
            }
            set { apiKey = value; }
        }

        public ServiceConfiguration()
        {
            timerService = new TimerService();
        }

        public ITimerService TimerService
        {
            get { return timerService; }
        }

        

        public string ImagesFolder
        {
            get
            {
                if (imagesFolder != null)
                {
                    return imagesFolder;
                }
                if (WebConfigurationManager.AppSettings["ImagesFolder"] != null)
                {
                    return WebConfigurationManager.AppSettings["ImagesFolder"];
                }

                return HostingEnvironment.MapPath("~/Images");
            }
            set { imagesFolder = value; }
        }

        public bool IsMaintenanceMode
        {
            get
            {
                if (maintenanceMode.HasValue)
                {
                    return maintenanceMode.Value;
                }
                bool mode = false;
                if (WebConfigurationManager.AppSettings["IsMaintenanceMode"] != null)
                {
                    bool.TryParse(WebConfigurationManager.AppSettings["IsMaintenanceMode"], out mode);
                }
                return mode;
            }
            set { maintenanceMode = value; }
        }

        public bool RequireActivateNewProfile
        {
            get
            {
                if (requireActivateNewProfile.HasValue)
                {
                    return requireActivateNewProfile.Value;
                }
                bool res = false;
                if (WebConfigurationManager.AppSettings["RequireActivateNewProfile"] != null)
                {
                    bool.TryParse(WebConfigurationManager.AppSettings["RequireActivateNewProfile"], out res);
                }
                return res;
            }
            set { requireActivateNewProfile = value; }
        }

        public string ServerUrl
        {
            get
            {
                if (WebConfigurationManager.AppSettings["Server"] != null)
                {
                    return WebConfigurationManager.AppSettings["Server"];
                }
                return string.Empty;
            }
        }

        public PaymentsHolder Payments
        {
            get { return paymentsHolder; }
            set { paymentsHolder = value; }
        }
    }
}
