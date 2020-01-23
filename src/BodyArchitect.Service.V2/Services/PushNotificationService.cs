using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Configuration;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using NHibernate;
using WindowsPhone.Recipes.Push.Messasges;

namespace BodyArchitect.Service.V2.Services
{
    public interface IPushNotificationService
    {
        void SendLiveTile(ISession session, Profile user,object param);
        void SendLiveTile(string uri, string backgroundImageUrl, string title, int count);
    }

    public class MessagesCountPushNotification : PushNotificationService
    {
        public override void SendLiveTile(ISession session, Profile user, object param)
        {
            var devices = session.QueryOver<WP7PushNotification>().Where(x => x.ProfileId == user.GlobalId).List();
            var messagesCount = session.QueryOver<Message>().Where(x => x.Receiver == user).RowCount();
            foreach (var device in devices)
            {
                try
                {
                    SendLiveTile(device.URI, "WP7TileImage.png", null, messagesCount);
                }
                catch (Exception ex)
                {
                    Log.Write(ex, Guid.NewGuid());
                }
            }
        }
    }

    public class NotReadedMessagesPushNotification : PushNotificationService
    {
        public override void SendLiveTile(ISession session, Profile user, object param)
        {
            bool adding = (bool) param;
            if(!adding)
            {//when we remove the message then we shouldn't send anything
                return;
            }
            var devices = session.QueryOver<WP7PushNotification>().Where(x => x.ProfileId == user.GlobalId).List();
            foreach (var device in devices)
            {
                try
                {
                    device.Counter++;
                    SendLiveTile(device.URI, "WP7TileImage.png", null, device.Counter);
                    session.SaveOrUpdate(device);
                    session.Flush();
                }
                catch (Exception ex)
                {
                    Log.Write(ex, Guid.NewGuid());
                }
            }
        }
    }
    public abstract class PushNotificationService : IPushNotificationService
    {

        public abstract void SendLiveTile(ISession session, Profile user, object param);

        public bool SendRealMessages
        {
            get
            {
                bool sendRealMessages = true;
                if (WebConfigurationManager.AppSettings["SendRealMessages"] != null)
                {
                    bool.TryParse(WebConfigurationManager.AppSettings["SendRealMessages"], out sendRealMessages);
                }

                return sendRealMessages;
            }
        }

        public void SendLiveTile(string uri, string backgroundImageUrl, string title, int count)
        {
            if (count > 99 || !SendRealMessages)
            {//counter for live tile must be 1-99 number (http://windowsteamblog.com/windows_phone/b/wpdev/archive/2011/01/14/windows-push-notification-server-side-helper-library.aspx)
                return;
            }
            var tileMsg = new TilePushNotificationMessage(MessageSendPriority.High)
            {
                Count = count,
                Title = title
            };

            tileMsg.SendAsync(new Uri(uri),delegate (MessageSendResult res)
                                               {
                                                   Log.WriteInfo("DeviceConnectionStatus:"+res.DeviceConnectionStatus);
                                                   Log.WriteInfo("StatusCode:" + res.StatusCode);
                                                   Log.WriteInfo("NotificationStatus:" + res.NotificationStatus);
                                                   Log.WriteInfo("SubscriptionStatus:" + res.SubscriptionStatus);
                                               },
                                               delegate(MessageSendResult res)
                                               {
                                                   Log.WriteInfo("DeviceConnectionStatus:" + res.DeviceConnectionStatus);
                                                   Log.WriteInfo("StatusCode:" + res.StatusCode);
                                                   Log.WriteInfo("NotificationStatus:" + res.NotificationStatus);
                                                   Log.WriteInfo("SubscriptionStatus:" + res.SubscriptionStatus);
                                               });

        }
    }
}
