using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using Microsoft.Devices;
using Microsoft.Phone.Info;

namespace BodyArchitect.WP7
{
    public class TrialManager
    {
        private int daysLeft;
        private static bool trialSimulationValue = true;

        public int DaysLeft
        {
            get { return daysLeft; }
        }

        protected virtual DateTime getCurrentDateTime()
        {
            return DateTime.UtcNow;
        }
        /// <summary>
        /// Gets the unique identifier for the device.
        /// </summary>
        /// <returns>A string representation of the unique device identifier.
        public string GetDeviceId()
        {
            object uniqueId;
            if (DeviceExtendedProperties.TryGetValue("DeviceUniqueId", out uniqueId))
            {
                return BitConverter.ToString((byte[])uniqueId);
            }
            else
            {
                return Settings.ClientId;
            }

        }

        public static bool ShouldDisplayTrialExpirationMessage()
        {
#if FREE 
            return false;//for free version we do not display trial expiration message
#else
            return ApplicationState.IsFree && !Settings.TrialExpiredMessageShowed;
#endif
        }

        public void DetermineIsTrail()
        {
#if FREE
            // return true if debugging with trial enabled (DebugTrial configuration is active)
            ApplicationState.IsFree = true;
#else

            bool isTrial = getTrialStatusFromLicence();

            if (isTrial)
            {
                //get from cache date of start trial
                if (Settings.TrialStarted != null)
                {
                    updateTrialStatus();
                }
                else
                {
                    //and now retrieve actual start trial date from service
                    retrieveTrialFromService();
                }
                
            }
            else
            {
                ApplicationState.IsFree = false;
                Settings.TrialStarted = null;//for pro version (after user paid) we remove this
            }
#endif
        }

        public string GetTrialStatus()
        {
#if !FREE
            if (Settings.TrialStarted != null )
            {
                int leftDays = getTrialLeftDays();
                if (leftDays > 0)
                {
                    return string.Format(ApplicationStrings.TrialStatusDaysLeft, leftDays);
                }
                else
                {
                   return ApplicationStrings.TrialStatusExpired;
                }
            }
#endif
            return null;
        }

        void updateTrialStatus()
        {
            var isTrial = getTrialStatusFromLicence();
            ApplicationState.IsFree =isTrial && getTrialLeftDays() <=0;
        }

        private int getTrialLeftDays()
        {
#if TRIAL_SIMULATION
            daysLeft =4 - (int)(getCurrentDateTime() - Settings.TrialStarted.Value).TotalMinutes;
#else
            daysLeft =14 - (int)(getCurrentDateTime() - Settings.TrialStarted.Value).TotalDays;
#endif
            return DaysLeft;
        }

        protected virtual void retrieveTrialFromService_Completed(WP7TrialStatusCompletedEventArgs e)
        {
            if (e.Error != null)
            {

            }
            else
            {
                Settings.TrialStarted = e.Result.TrialStarted;
                updateTrialStatus();
            }
        }

        protected virtual void retrieveTrialFromService()
        {
            if (Microsoft.Devices.Environment.DeviceType == DeviceType.Emulator || !NetworkInterface.GetIsNetworkAvailable())
            {
                retrieveTrialFromService_Completed(new WP7TrialStatusCompletedEventArgs(new object[] { new TrialStatusInfo() { TrialStarted = DateTime.UtcNow } }, null, false, null));
                return;
            }
            var service = ApplicationState.CreateService();
            service.WP7TrialStatusCompleted += delegate(object sender, WP7TrialStatusCompletedEventArgs e)
                                                   {

                                                       retrieveTrialFromService_Completed(e);
                                                   };
            service.WP7TrialStatusAsync(GetDeviceId());
        }

        public static  bool IsTrialFromLicence()
        {
#if TRIAL_SIMULATION && !RELEASE
            return trialSimulationValue;
#endif
            var license = new Microsoft.Phone.Marketplace.LicenseInformation();
            return license.IsTrial();
        }

        public static void Buy()
        {
            trialSimulationValue = false;
        }

        protected virtual bool getTrialStatusFromLicence()
        {
            return IsTrialFromLicence();
        }
    }
}
