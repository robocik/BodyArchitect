using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using Microsoft.Devices;
using Microsoft.Phone.Info;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Notification;

namespace BodyArchitect.WP7
{
    public class PushNotificationsHelper
    {
#if FREE
        private string channelName = "BodyArchitectFreeChannel";
#else
        private string channelName = "BodyArchitectChannel";
#endif
        
        private string serviceName = "BodyArchitectService";

        private HttpNotificationChannel httpChannel;
        private Guid profileId;

        public event EventHandler Completed;

        public void RegisterDevice(Guid profileId)
        {
            if (Microsoft.Devices.Environment.DeviceType == DeviceType.Emulator)
            {
                onCompeted();
                return;
            }
            //First, try to pick up existing channel
            httpChannel = HttpNotificationChannel.Find(channelName);
            this.profileId = profileId;
            if (httpChannel != null)
            {
                //httpChannel.UnbindToShellTile();
                //httpChannel.Close();

                SubscribeToChannelEvents();

                SubscribeToService();
            }
            else
            {
                try
                {
                    //Create new channel

                    httpChannel = new HttpNotificationChannel(channelName, serviceName);

                    SubscribeToChannelEvents();

                    httpChannel.Open();
                }
                catch (Exception ex)
                {
                    if(httpChannel!=null)
                    {
                        httpChannel.Close();
                    }
                    throw;
                }
                
            }
        }

        private void onCompeted()
        {
            if(Completed!=null)
            {
                Completed(this, EventArgs.Empty);
            }
        }

        private void SubscribeToChannelEvents()
        {
            //Register to UriUpdated event - occurs when channel successfully opens
            httpChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(httpChannel_ChannelUriUpdated);

            //Subscribed to Raw Notification
            httpChannel.HttpNotificationReceived += new EventHandler<HttpNotificationEventArgs>(httpChannel_HttpNotificationReceived);

            //Subscribe to Toast Notifications
            httpChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(httpChannel_ShellToastNotificationReceived);

            //general error handling for push channel
            httpChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(httpChannel_ExceptionOccurred);
        }

        void httpChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            // Optionally save the URI locally here - e.ChannelUri.ToString();

            SubscribeToService();
        }

        void httpChannel_ExceptionOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
            // Handle notification exceptions here
        }

        void httpChannel_HttpNotificationReceived(object sender, HttpNotificationEventArgs e)
        {
            // Handle raw notification here
        }

        void httpChannel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
        {
            // This runs when a toast notification is received while the app is running


        }



        public void Unregister()
        {
            httpChannel = HttpNotificationChannel.Find(channelName);
            if (httpChannel != null)
            {
                if (httpChannel.IsShellTileBound)
                {
                    httpChannel.UnbindToShellTile();
                }
                httpChannel.Close();
            }
            onCompeted();
        } 


        private void SubscribeToService()
        {

            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                // Handle no internet here
                onCompeted();
                return;
            }

            string id = Settings.ClientInstanceId.ToString();//ParseANID(UserExtendedProperties.GetValue("ANID") as string);





            var m = new ServiceManager<WP7RegisterCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<WP7RegisterCompletedEventArgs> operationCompleted)
            {
                client1.WP7RegisterAsync(id, httpChannel.ChannelUri.ToString(), profileId);

                client1.WP7RegisterCompleted -= operationCompleted;
                client1.WP7RegisterCompleted += operationCompleted;

            });
            m.OperationCompleted += (s, a1) =>
            {
                if (a1.Error == null)
                    ApplicationState.SynchronizationContext.Send((object s1) =>
                    {
                        BindingANotificationsChannelToAToastNotification();
                    }, null);
                onCompeted();
            };

            if (!m.Run(true))
            {
                
            }

        }

        private void BindingANotificationsChannelToAToastNotification()
        {
            //if (!_settings.UserHasBeenWarned)
            //{
            //    var result = MessageBox.Show("Turn on push notifications?" +
            //        Environment.NewLine + Environment.NewLine + "Privacy:" +
            //        Environment.NewLine + "http://example.com/privacy-policy/",
            //        "Push Notifications", MessageBoxButton.OKCancel);
            //    if (result == MessageBoxResult.OK)
            //    {
            //        _settings.UserHasBeenWarned = true;
            //    }
            //    else
            //    {
            //        return;
            //    }
            //}
            if (!httpChannel.IsShellTileBound)
            {
                httpChannel.BindToShellTile();
            }
        }

        //public string ParseANID(string anid)
        //{
        //    if (!String.IsNullOrEmpty(anid))
        //    {
        //        string[] parts = anid.Split('&');
        //        IEnumerable<string[]> pairs = parts.Select(part => part.Split('='));
        //        string id = pairs
        //            .Where(pair => pair.Length == 2 && pair[0] == "A")
        //            .Select(pair => pair[1])
        //            .FirstOrDefault();
        //        return id;
        //    }

        //    return Settings.ClientInstanceId.ToString();
        //}
    }
}
