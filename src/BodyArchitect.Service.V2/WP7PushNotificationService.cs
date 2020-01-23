using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Model;

namespace BodyArchitect.Service.V2
{
    [ServiceContract]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class WP7PushNotificationService
    {
        [WebInvoke(UriTemplate = "register?deviceid={deviceid}&uri={uri}",
    ResponseFormat = WebMessageFormat.Json,
    Method = "GET")]
        [OperationContract]
        public string Register(string deviceid, string uri)
        {
            if (!String.IsNullOrWhiteSpace(deviceid))
            {
                var session = NHibernateContext.Current().Session;
                using (var tx = session.BeginGetTransaction())
                {
                    var res=session.QueryOver<WP7PushNotification>().Where(x => x.DeviceID == deviceid);
                    var device = res.SingleOrDefault();

                    if (device != null)
                    {
                        // Do we need to update the URI?
                        if (device.URI != uri)
                            device.URI = uri;
                    }
                    else
                    {
                        device = new WP7PushNotification()
                        {
                            DeviceID = deviceid,
                            URI = uri,
                            Added = DateTime.Now,
                            Modified = DateTime.Now
                        };

                        session.SaveOrUpdate(device);
                    }

                    tx.Commit();
                }
            }
            return deviceid;
        }
    }
}
