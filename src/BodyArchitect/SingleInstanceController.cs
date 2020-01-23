using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Net;
using BodyArchitect.Controls.Cache;
using BodyArchitect.WCF;
using BodyArchitect.Controls;
using BodyArchitect.Settings;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.VisualBasic.ApplicationServices;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Logger;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Common;
using System.Threading;
using System.Windows.Forms;
using BodyArchitect.Common.Plugins;
using Microsoft.Win32;

namespace BodyArchitect
{
    public class SingleInstanceController
    : WindowsFormsApplicationBase
    {
        //private static SingleInstanceController instance;
        private UpdateManager updateManager;

        public bool SingleInstance
        {
            get
            {
                bool isSingle = false;
                if(ConfigurationManager.AppSettings["IsSingleInstance"]!=null)
                {
                    bool.TryParse(ConfigurationManager.AppSettings["IsSingleInstance"], out isSingle);
                }
                return isSingle;
            }
        }

        public SingleInstanceController()
        {
            ServicePointManager.Expect100Continue = Settings1.Default.Expect100Continue;
            // Set whether the application is single instance)
            this.IsSingleInstance = SingleInstance;
            //instance = this;
            this.SaveMySettingsOnExit = true;
            updateManager=new UpdateManager();

            configureEntLib();
            this.StartupNextInstance += new StartupNextInstanceEventHandler(this_StartupNextInstance);
            ServiceManager.ClientInstanceId = UserContext.Settings.ClientInstanceId;
        }


        private void configureEntLib()
        {
            var formatterStandard = new FormatterBuilder().TextFormatterNamed("Text Formatter").UsingTemplate(
                "Timestamp: {timestamp}{newline}Message: {message}{newline}Severity: {severity}{newline}Machine: {machine}{newline}Process Name: {processName}{newline}Extended Properties: {dictionary({key} - {value}{newline})}");
            var conf = new ConfigurationSourceBuilder();
            var logConfig = conf.ConfigureLogging();
            logConfig.WithOptions.LogToCategoryNamed("Exception").SendTo.RollingFile(
                "ExceptionFileListener").WithHeader("----------------------------------------").WithFooter("----------------------------------------").ToFile(UserContext.Settings.ExceptionsLogFile);
            logConfig.WithOptions.LogToCategoryNamed("General").SendTo.RollingFile(
                "FlatFile TraceListener").WithHeader("----------------------------------------").WithFooter("----------------------------------------").FormatWith(formatterStandard).ToFile(UserContext.Settings.StandardLogFile);
            logConfig.WithOptions.LogToCategoryNamed("email").SendTo.Email("email").FormatWith(formatterStandard).UsingSmtpServer(
                ApplicationSettings.MailSmtp).WithUserNameAndPassword(ApplicationSettings.MailUserName,
                                                                      ApplicationSettings.MailPassword).To(
                                                                          ApplicationSettings.MailAccount).From(
                                                                              ApplicationSettings.MailAccount).UseSSL(true);

            //configure cache
            var cacheCfg = conf.ConfigureCaching();
            cacheCfg.ForCacheManagerNamed(PicturesCache.PicturesCacheName).StoreInIsolatedStorage(
                "Isolated Storage Cache Store").UsePartition("PicturesCache1");

            cacheCfg.ForCacheManagerNamed("ErrorCache").StoreInMemory();

            var configSource = new DictionaryConfigurationSource();
            conf.UpdateConfigurationWithReplace(configSource);

            EnterpriseLibraryContainer.Current= EnterpriseLibraryContainer.CreateDefaultContainer(configSource);

            BodyArchitect.Logger.Log.EnableExceptionLog = Settings1.Default.LogErrorEnabled;
            BodyArchitect.Logger.Log.EnableStandardLog = Settings1.Default.LogStandardEnabled;
        }

        protected override void OnShutdown()
        {
            UserContext.Settings.Save();
            UserContext.Logout(LoginStatus.NotLogged,false);
            base.OnShutdown();
        }


        private void SetAddRemoveProgramsIcon()
        {
            //only run if deployed 
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed
               && Deployment.IsFirstRun)
            {
                Logger.Log.WriteInfo("Setting uninstall icon");
                try
                {
                    string iconSourcePath = Path.Combine(System.Windows.Forms.Application.StartupPath, "MainIcon.ico");
                    if (!File.Exists(iconSourcePath))
                    {
                        Logger.Log.WriteWarning("Icon file not found");
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

        protected override bool OnUnhandledException(Microsoft.VisualBasic.ApplicationServices.UnhandledExceptionEventArgs e)
        {
            HideSplashScreen();
            ExceptionHandler.Default.Process(e.Exception, ApplicationStrings.ErrorUnhandledException, ErrorWindow.EMailReport);
            return true;
        }

        private void setSplashStatus(string text)
        {
            SplashScreenWindow wnd = SplashScreen as SplashScreenWindow;
            if (wnd != null && (updateManager == null || updateManager.UpdateFinished))
            {
                wnd.SetStatus(text);
            }
        }

        /// <summary>
        /// This method sends request to the special page which is used to count how many users we have
        /// </summary>
        void sendRequest()
        {
            if (!UserContext.Settings.SendRequests)
            {
                return;
            }
            ThreadPool.QueueUserWorkItem(delegate(Object stateInfo)
            {
                try
                {

                    string url = string.Format(UserContext.Settings.BaUpdateCheckAddress, UserContext.Settings.ClientInstanceId, Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName, Constants.Version);
                    HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
                    myRequest.UseDefaultCredentials = true;
                    Stream objStream = myRequest.GetResponse().GetResponseStream();
                    StreamReader sr = new StreamReader(objStream);
                    string response = sr.ReadToEnd();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Default.Process(ex);
                }
            }
            );
        }

        protected override bool OnStartup(StartupEventArgs eventArgs)
        {
            ShowSplashScreen();
            setSplashStatus(ApplicationStrings.InfoInicializingApplication);

            SetAddRemoveProgramsIcon();
            Logger.Log.WriteInfo("OnStartup starts. VERSION: {0}", Constants.Version);
            updateManager.StartUpdate();

            
            Exception initializationException = null;
            try
            {
                PluginsManager.Instance.LoadPlugins(Application.StartupPath);
                PluginsManager.Instance.InitializePlugins();
                if (this.IsNetworkDeployed)
                {
                    sendRequest();
                }
            }
            catch (Exception ex)
            {
                initializationException = ex;
            }
            
            while (!updateManager.UpdateFinished)
            {
                DoEvents();
            }

            if (updateManager.RestartRequired)
            {
                Logger.Log.WriteInfo("Update completed. Restarting application");
                return false;
            }
            if (initializationException!=null)
            {
                HideSplashScreen();
                ExceptionHandler.Default.Process(initializationException, "Error during inicialization. Application will be closed.",ErrorWindow.EMailReport);
                return false;
            }


            setSplashStatus(ApplicationStrings.SplasScreen_PreparingGUI);

            LoginWindow.ShowForceUpdate = updateManager.UpdateFailed;
            
            return base.OnStartup(eventArgs);
        }


        
        public new void HideSplashScreen()
        {
            base.HideSplashScreen();
        }

        void this_StartupNextInstance(object sender, StartupNextInstanceEventArgs e)
        {
            // Here you get the control when any other instance is
            // invoked apart from the first one.
            // You have args here in e.CommandLine.

            // You custom code which should be run on other instances
            if (MainForm != null)
            {
                MainForm.Activate();
                MainForm.BringToFront();
            }
        }

        protected override void OnCreateMainForm()
        {
            if (this.MainForm == null)
            {
                // Instantiate your main application form
                this.MainForm = MainWindow.Instance;
                this.MainForm.WindowState = FormWindowState.Maximized;
                MainWindow.Instance.ProfileConfigurationWizardShow +=
                    new EventHandler(Instance_ProfileConfigurationWizardShow);
            }
        }

        void Instance_ProfileConfigurationWizardShow(object sender, EventArgs e)
        {
            ProfileConfigurationWizard dlg = new ProfileConfigurationWizard();
            if(dlg.ShowDialog(MainWindow.Instance)==DialogResult.OK)
            {
                
            }
            MainWindow.Instance.Fill();
        }

        protected override void OnCreateSplashScreen()
        {
            var splash = SplashScreenWindow.Current;

            splash.SetFade = true;  // This fades the splash screen in and out. However, when using TransparencyKey, doesn't not display correctly.

            SplashScreen = splash;
        }
    }

}
