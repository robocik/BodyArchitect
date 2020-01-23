using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Controls;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Logger;

namespace BodyArchitect
{
    class UpdateManager
    {
        public volatile bool UpdateFinished=true;
        private CheckForUpdateCompletedEventArgs updateInfo;
        public bool RestartRequired { get; private set; }
        public bool UpdateFailed;

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
                ControlHelper.EnsureThreadLocalized();
                if (e.Error != null)
                {
                    UpdateFailed = true;
                    ExceptionHandler.Default.Process(e.Error, ApplicationStrings.ErrorDuringUpdatingApplication,
                                                     ErrorWindow.EMailReport);
                    return;
                }
                Log.WriteVerbose("Update completed");
                SplashScreenWindow.Current.SetProgressBar(false, 0, 0);
                Application.Restart();
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

        void CurrentDeployment_UpdateProgressChanged(object sender, DeploymentProgressChangedEventArgs e)
        {
            try
            {
                ControlHelper.EnsureThreadLocalized();
                if (e.State == DeploymentProgressState.DownloadingApplicationFiles)
                {
                    SplashScreenWindow.Current.SetStatus(ApplicationStrings.SplashScreen_DownloadingNewVersion,
                                                         ControlHelper.FormatBytes(e.BytesCompleted),
                                                         ControlHelper.FormatBytes(e.BytesTotal), updateInfo.AvailableVersion);
                    SplashScreenWindow.Current.SetProgressBar(true, (int)e.BytesTotal, (int)e.BytesCompleted);
                }
                else
                {
                    SplashScreenWindow.Current.SetStatus(ApplicationStrings.SplashScreen_NewVersionFound, updateInfo.AvailableVersion);
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
                ControlHelper.EnsureThreadLocalized();
                updateInfo = e;
                if (e.Error != null)
                {
                    UpdateFailed = true;
                    ExceptionHandler.Default.Process(e.Error, ApplicationStrings.ErrorDuringCheckingUpdates,ErrorWindow.EMailReport);
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
                ControlHelper.EnsureThreadLocalized();
                SplashScreenWindow.Current.SetStatus(ApplicationStrings.SplashScreen_CheckingNewVersion);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex);
            }
            
        }
    }
}
