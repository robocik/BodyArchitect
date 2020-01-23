using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Deployment.Application;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using BodyArchitect;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Cache;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.SchedulerEngine;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Settings;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using BodyArchitect.Portable;

namespace BodyInstructor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        
        private UpdateManager updateManager;


        private void HandleCommandlineArg(string arg)
        {
            if (arg == null)
                return;
            Dispatcher.BeginInvoke(new Action(() =>
                                             {
                                                 //TODO: define possible arguments
                                                 switch (arg)
                                                 {
                                                     case "--test":
                                                         MessageBox.Show(arg);
                                                         break;
                                                     case "--not-test":
                                                         MessageBox.Show(arg, "Argument");
                                                         break;
                                                     default:
                                                         MessageBox.Show(arg, "Argument", MessageBoxButton.OK, MessageBoxImage.Information);
                                                         break;
                                                 }
                                             }));
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            Application.Current.MainWindow.Visibility = Visibility.Visible;
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                new Action(delegate()
                {
                    HandleCommandlineArg(args.Count > 1 ? args[1] : null);
                    Current.MainWindow.WindowState = WindowState.Normal;
                    Current.MainWindow.Activate();

                })
            );
            return true;
        }

        private void configureLogging()
        {
            var formatterStandard = new FormatterBuilder().TextFormatterNamed("Text Formatter").UsingTemplate(
                "Timestamp: {timestamp}{newline}Message: {message}{newline}Severity: {severity}{newline}Machine: {machine}{newline}Process Name: {processName}{newline}Extended Properties: {dictionary({key} - {value}{newline})}");
            var conf = new ConfigurationSourceBuilder();
            var logConfig = conf.ConfigureLogging();
            logConfig.WithOptions.LogToCategoryNamed("Exception").SendTo.RollingFile(
                "ExceptionFileListener").WithHeader("----------------------------------------").WithFooter("----------------------------------------").ToFile(UserContext.Current.Settings.ExceptionsLogFile);
            logConfig.WithOptions.LogToCategoryNamed("General").SendTo.RollingFile(
                "FlatFile TraceListener").WithHeader("----------------------------------------").WithFooter("----------------------------------------").FormatWith(formatterStandard).ToFile(UserContext.Current.Settings.StandardLogFile);
            logConfig.WithOptions.LogToCategoryNamed("email").SendTo.Email("email").FormatWith(formatterStandard).UsingSmtpServer(
                ApplicationSettings.MailSmtp).WithUserNameAndPassword(ApplicationSettings.MailUserName,
                                                                      ApplicationSettings.MailPassword).To(
                                                                          ApplicationSettings.MailAccount).From(
                                                                              ApplicationSettings.MailAccount).UseSSL(true);

            ////configure cache
            var cacheCfg = conf.ConfigureCaching();
            cacheCfg.ForCacheManagerNamed(PicturesCache.PicturesCacheName).StoreInIsolatedStorage(
                "Isolated Storage Cache Store").UsePartition("PicturesCache1");

            cacheCfg.ForCacheManagerNamed("ErrorCache").StoreInMemory();

            cacheCfg.ForCacheManagerNamed(TranslationsCache.TranslationsCacheName).StoreInIsolatedStorage(
                "Isolated Storage TranslationsCache").UsePartition("TranslationCache");

            var configSource = new DictionaryConfigurationSource();
            conf.UpdateConfigurationWithReplace(configSource);

            EnterpriseLibraryContainer.Current = EnterpriseLibraryContainer.CreateDefaultContainer(configSource);

            BodyArchitect.Logger.Log.EnableExceptionLog = Settings1.Default.LogErrorEnabled;
            BodyArchitect.Logger.Log.EnableStandardLog = Settings1.Default.LogStandardEnabled;
        }

        private void setSplashStatus(string text)
        {
            if (updateManager == null || updateManager.UpdateFinished)
            {
                Program.SplashScreen.SetSplashMessage(text);
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            setSplashStatus(Strings.InfoInicializingApplication);
            updateManager = new UpdateManager(Program.SplashScreen);
            ApplicationSettings.ServerUrl = ServiceManager.ServerAddress;
            ServiceManager.ClientInstanceId = UserContext.Current.Settings.ClientInstanceId;
            configureLogging();
            updateManager.StartUpdate();
            BodyArchitect.Logger.Log.WriteInfo("OnStartup starts. VERSION: {0}", Constants.Version);
            MainWindow main = new MainWindow();
            LoginWindow login = new LoginWindow();

            Exception initializationException = null;
            try
            {
                PluginsManager.Instance.LoadPlugins(Helper.StartUpPath);
                PluginsManager.Instance.InitializePlugins();
                Scheduler.Init();
                //BUG FIX: Without this sometimes images for Ok and Cancel button is are not visible
                ImageButtonExt.SetImage(main, "pack://application:,,,/BodyArchitect.Client.Resources;component/Images/CancelButton.gif".ToBitmap());
            }
            catch (Exception ex)
            {
                initializationException = ex;
            }
            while (!updateManager.UpdateFinished)
            {
                UIHelper.DoEvents();
            }

            if (updateManager.RestartRequired)
            {
                Log.WriteInfo("Update completed. Restarting application");
                Program.SplashScreen.HideSplashScreen();
                System.Windows.Forms.Application.Restart();
                Application.Current.Shutdown();
                return;
            }
            if (initializationException != null)
            {
                Program.SplashScreen.HideSplashScreen();

                ExceptionHandler.Default.Process(initializationException, Strings.ErrApplicationStartup, ErrorWindow.EMailReport);
                Application.Current.Shutdown();
                return;
            }


            setSplashStatus(Strings.SplasScreen_PreparingGUI);

            LoginWindow.ShowForceUpdate = updateManager.UpdateFailed;

            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            Application.Current.MainWindow = login;

            login.LoginSuccessful += main.StartupMainWindow;
            login.Show();
            login.Activate();
            Program.SplashScreen.HideSplashScreen();

            Dispatcher.BeginInvoke(new Action(() => HandleCommandlineArg(e.Args.Length > 0 ? e.Args[0] : null)));
            base.OnStartup(e);
            
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            UserContext.Current.Settings.Save();
            Scheduler.Close();
            UserContext.Current.Logout(LoginStatus.NotLogged, false);
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Program.SplashScreen.HideSplashScreen();
            e.Handled = true;
            ExceptionHandler.Default.Process(e.Exception, Strings.ErrorUnhandledException, ErrorWindow.EMailReport);
        }

        
    }

}
