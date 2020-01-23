using System;
using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Model;
using BodyArchitect.Portable;

namespace BodyArchitect.Service.V2
{
    public class APIKeyManager
    {
        private static APIKeyManager apiKeyManager;

        public static APIKeyManager Instance
        {
            get
            {
                if(apiKeyManager==null)
                {
                    apiKeyManager=new APIKeyManager();
                    apiKeyManager.Init();
                }
                return apiKeyManager;
            }
        }

        private Dictionary<Guid, APIKey> cache = new Dictionary<Guid, APIKey>();

        public void Init()
        {
            using (var session = NHibernateFactory.OpenSession())
            {
                var keys = session.QueryOver<APIKey>().List();
                foreach (var key in keys)
                {
                    cache.Add(key.ApiKey, key);
                }
            }
        }

        public APIKey GetApiKey(Guid apiKey)
        {
            APIKey apiKeyObject;
            cache.TryGetValue(apiKey, out apiKeyObject);
            return apiKeyObject;
        }
    }
    public class APIKeyAuthorization : ServiceAuthorizationManager
    {
        public const string APIKEY = "APIKey";

        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            return IsValidAPIKey(operationContext);
        }


        public bool IsValidAPIKey(OperationContext operationContext)
        {
            
            int index=operationContext.IncomingMessageHeaders.FindHeader(Constants.HeaderAPIKey, "");
            if (index > -1)
            {
                var key = operationContext.IncomingMessageHeaders.GetHeader<string>(Constants.HeaderAPIKey, "");
                Guid apiKey;
                // Convert the string into a Guid and validate it
                if (Guid.TryParse(key, out apiKey) && APIKeyManager.Instance.GetApiKey(apiKey) != null)
                {

                    try
                    {
                        //if client app set Language header then change the language of the service
                        index = operationContext.IncomingMessageHeaders.FindHeader(Constants.HeaderLanguage, "");
                        if (index > -1)
                        {
                            var lang = operationContext.IncomingMessageHeaders.GetHeader<string>(Constants.HeaderLanguage, "");
                            Thread.CurrentThread.CurrentCulture=Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
                        }
                    }
                    catch (Exception)
                    {
                    }

                    return true;
                }
            }
            
            
            //api key not exists
            return false;
        }
    }
}
