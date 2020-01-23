using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Model.WP7;
using NHibernate;

namespace BodyArchitect.Service.V2.Services
{
    public class WP7Service : ServiceBase
    {
        private IPushNotificationService pushNotificationService;

        public WP7Service(ISession session, SecurityInfo securityInfo, ServiceConfiguration configuration,IPushNotificationService pushNotificationService) : base(session, securityInfo, configuration)
        {
            this.pushNotificationService = pushNotificationService;
        }

        public string WP7Register(string deviceid, string uri, Guid profileId)
        {
            if (!String.IsNullOrWhiteSpace(deviceid))
            {
                var session = Session;
                using (var tx = session.BeginSaveTransaction())
                {
                    var res = session.QueryOver<WP7PushNotification>().Where(x => x.DeviceID == deviceid);
                    var device = res.SingleOrDefault();

                    if (device != null)
                    {
                        // Do we need to update the URI?
                        if (device.URI != uri)
                            device.URI = uri;
                        device.ProfileId = profileId;
                        device.Modified = Configuration.TimerService.UtcNow;
                    }
                    else
                    {
                        device = new WP7PushNotification()
                        {
                            DeviceID = deviceid,
                            URI = uri,
                            Added = Configuration.TimerService.UtcNow,
                            Modified = Configuration.TimerService.UtcNow,
                            ProfileId = profileId
                        };

                        session.SaveOrUpdate(device);
                    }

                    tx.Commit();
                }
            }
            return deviceid;
        }

        public void WP7Unregister(string deviceid)
        {
            if (!String.IsNullOrWhiteSpace(deviceid))
            {
                var session = Session;
                using (var tx = session.BeginSaveTransaction())
                {
                    var res = session.QueryOver<WP7PushNotification>().Where(x => x.DeviceID == deviceid);
                    var device = res.SingleOrDefault();

                    if (device != null)
                    {
                        session.Delete(device);
                    }
                    tx.Commit();
                }
            }
        }

        public void WP7ClearCounter(Profile profile)
        {
            var session = Session;
            using (var tx = session.BeginGetTransaction())
            {
                var res = session.QueryOver<WP7PushNotification>().Where(x => x.ProfileId == profile.GlobalId);
                foreach (var device in res.List())
                {
                    device.Counter = 0;
                    pushNotificationService.SendLiveTile(device.URI, "WP7TileImage.png", null, device.Counter);
                }

                tx.Commit();
            }
        }

        public void WP7ClearCounter(string deviceid)
        {
            if (!String.IsNullOrWhiteSpace(deviceid))
            {
                var session = Session;
                using (var tx = session.BeginGetTransaction())
                {
                    var res = session.QueryOver<WP7PushNotification>().Where(x => x.DeviceID == deviceid);
                    var device = res.SingleOrDefault();

                    if (device != null)
                    {
                        device.Counter = 0;
                        pushNotificationService.SendLiveTile(device.URI, "WP7TileImage.png", null, device.Counter);
                    }

                    tx.Commit();
                }
            }
        }

        //TODO:Remove this method
        public TrialStatusInfo WP7TrialStatus(string deviceId)
        {
            TrialStatusInfo statusInfo = new TrialStatusInfo();

            if (!String.IsNullOrWhiteSpace(deviceId))
            {
                var session = Session;

                using (var tx = session.BeginSaveTransaction())
                {
                    var res = session.QueryOver<WP7Trial>().Where(x => x.DeviceId == deviceId);
                    var device = res.SingleOrDefault();

                    if (device == null)
                    {
                        device = new WP7Trial()
                        {
                            DeviceId = deviceId,
                            TrialStartedDate = Configuration.TimerService.UtcNow
                        };
                        session.SaveOrUpdate(device);
                    }

                    statusInfo.TrialStarted = device.TrialStartedDate;
                    tx.Commit();
                }

            }
            return statusInfo;
        }
    }
}
