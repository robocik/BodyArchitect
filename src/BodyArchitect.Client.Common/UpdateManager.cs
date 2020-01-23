using System;
using System.Deployment.Application;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Logger;
using Microsoft.Win32;

namespace BodyArchitect.Client.Common
{
    public class UpdateManager
    {
        public volatile bool UpdateFinished=true;
        private CheckForUpdateCompletedEventArgs updateInfo;
        public bool RestartRequired { get; private set; }
        public bool UpdateFailed;
        private ISplashScreen splashScreen;

        public UpdateManager(ISplashScreen splashScreen)
        {
            // TODO: Complete member initialization
            this.splashScreen = splashScreen;
            setAddRemoveProgramsIcon();
        }

        public void StartUpdate()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                Log.WriteVerbose("Application is running under ClickOnce");
                UpdateFinished = false;
                ApplicationDeployment.CurrentDeployment.CheckForUpdateProgressChanged += new DeploymentProgressChangedEventHandler(CurrentDeployment_CheckForUpdateProgressChanged);
                ApplicationDeployment.CurrentDeployment.CheckForUpdateCompleted += new CheckForUpdateCompletedEventHandler(CurrentDeployment_CheckForUpdateCompleted);
                ApplicationDeployment.CurrentDeployment.UpdateProgressChanged += new DeploymentProgressChangedEventHandler(CurrentDeployment_UpdateProgressChanged);
                ApplicationDeployment.CurrentDeployment.UpdateCompleted += new System.ComponentModel.AsyncCompletedEventHandler(CurrentDeployment_UpdateCompleted);
                ApplicationDeployment.CurrentDeployment.CheckForUpdateAsync();
            }
        }

        void CurrentDeployment_UpdateCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            try
            {
                Helper.EnsureThreadLocalized();
                if (e.Error != null)
                {
                    UpdateFailed = true;
                    ExceptionHandler.Default.Process(e.Error, Strings.ErrorDuringUpdatingApplication,
                                                     ErrorWindow.MessageBox);
                    return;
                }
                Log.WriteVerbose("Update completed");
                splashScreen.SetProgressBar(false, 0, 0);
                
                //Application.Restart();
                //Process.Start(Helper.StartUpPath);
                //Application.Current.Shutdown();


                RestartRequired = true;
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex);
            }
            finally
            {
                UpdateFinished = true;
            }
        }

        private void setAddRemoveProgramsIcon()
        {
            //only run if deployed 
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed
               && System.Deployment.Application.ApplicationDeployment.CurrentDeployment.IsFirstRun)
            {
                //Logger.Log.WriteInfo("Setting uninstall icon");
                try
                {
                    string iconSourcePath = Path.Combine(Helper.StartUpPath, "MainIcon.ico");
                    if (!File.Exists(iconSourcePath))
                    {
                        //Logger.Log.WriteWarning("Icon file not found");
                        return;
                    }
                    RegistryKey myUninstallKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall");
                    string[] mySubKeyNames = myUninstallKey.GetSubKeyNames();
                    for (int i = 0; i < mySubKeyNames.Length; i++)
                    {
                        RegistryKey myKey = myUninstallKey.OpenSubKey(mySubKeyNames[i], true);
                        object myValue = myKey.GetValue("DisplayName");
                        if (myValue != null && myValue.ToString() == "BodyArchitect")
                        {
                            myKey.SetValue("DisplayIcon", iconSourcePath);
                            break;
                        }
                    }
                }
                catch { }
            }
        }

        void CurrentDeployment_UpdateProgressChanged(object sender, DeploymentProgressChangedEventArgs e)
        {
            try
            {
                Helper.EnsureThreadLocalized();
                if (e.State == DeploymentProgressState.DownloadingApplicationFiles)
                {
                    splashScreen.SetStatus(string.Format(Strings.SplashScreen_DownloadingNewVersion,
                                                         Helper.FormatBytes(e.BytesCompleted),
                                                         Helper.FormatBytes(e.BytesTotal), updateInfo.AvailableVersion));
                    splashScreen.SetProgressBar(true, (int)e.BytesTotal, (int)e.BytesCompleted);
                }
                else
                {
                    splashScreen.SetStatus(string.Format(Strings.SplashScreen_NewVersionFound, updateInfo.AvailableVersion));
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex);
            }
            
        }

        void CurrentDeployment_CheckForUpdateCompleted(object sender, CheckForUpdateCompletedEventArgs e)
        {
            try
            {
                Helper.EnsureThreadLocalized();
                updateInfo = e;
                if (e.Error != null)
                {
                    UpdateFailed = true;
                    ExceptionHandler.Default.Process(e.Error, Strings.ErrorDuringCheckingUpdates, ErrorWindow.MessageBox);
                    UpdateFinished = true;
                    return;
                }
                if (e.UpdateAvailable)
                {
                    Log.WriteVerbose("Update available");
                    ApplicationDeployment.CurrentDeployment.UpdateAsync();

                }
                else
                {
                    Log.WriteVerbose("Update is not available");
                    UpdateFinished = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex);
            }
            
        }

        void CurrentDeployment_CheckForUpdateProgressChanged(object sender, DeploymentProgressChangedEventArgs e)
        {
            try
            {
                Helper.EnsureThreadLocalized();
                splashScreen.SetStatus(Strings.SplashScreen_CheckingNewVersion);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex);
            }
            
        }


        //public static bool IsFirstRunAfterUpdate
        //{
        //    get
        //    {
        //        if (!ApplicationDeployment.IsNetworkDeployed)
        //            return false; // not applicable == bool default value

        //        if (!File.Exists(VersionFileName))
        //            return true;

        //        return (GetLastRunVersion() != ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString());
        //    }
        //}

        //public static void StoreCurrentVersion()
        //{
        //    File.WriteAllText(VersionFileName, ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString());
        //}

        //public static string GetLastRunVersion()
        //{
        //    using (var stream = File.OpenText(VersionFileName))
        //    {
        //        return stream.ReadToEnd();
        //    }
        //}

        //public static string VersionFileName
        //{
        //    get
        //    {
        //        StringBuilder filename = new StringBuilder(Files.LocalFilesPath);
        //        if (!filename.ToString().EndsWith(@"\"))
        //            filename.Append(@"\");
        //        filename.Append(@"versioninfo.dat");
        //        return filename.ToString();
        //    }
        //}
    }
}
