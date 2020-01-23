using System;
using System.Configuration;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using System.Windows.Markup;
using System.Windows.Threading;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Logger;
using BodyArchitect.Portable.Exceptions;
using BodyArchitect.Shared;
using BodyArchitect.SplashScreen;
using BodyInstructor;
using WPFLocalizeExtension.Engine;


namespace BodyArchitect
{
    static class Program
    {
        private static Splash splash = new Splash();
        private const string Unique = "BodyArchitectApp";
        

        public static bool SingleInstance
        {
            get
            {
                bool isSingle = false;
                if (ConfigurationManager.AppSettings["IsSingleInstance"] != null)
                {
                    bool.TryParse(ConfigurationManager.AppSettings["IsSingleInstance"], out isSingle);
                }
                return isSingle;
            }
        }

        public static Splash SplashScreen
        {
            get { return splash; }
        }


        [STAThread]
        public static void Main()
        {
            Helper.EnsureThreadLocalized();
            if (!SingleInstance || SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo(Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName);
                ExceptionHandler.Default.ShowEmailReportWindow += new EventHandler<ErrorEventArgs>(Default_ShowEmailReportWindow);
                ExceptionHandler.Default.ShowMessageBoxWindow += new EventHandler<ErrorEventArgs>(Default_ShowMessageBoxWindow);
                

                var application = new App();
                SplashScreen.ShowSplashScreen();
                application.InitializeComponent();
                //for wpf localization (http://stackoverflow.com/questions/520115/stringformat-localization-problem/520334#520334)
                FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
                application.Run();


                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }

        static void Default_ShowMessageBoxWindow(object sender, ErrorEventArgs e)
        {
            if(e.Exception is ValidationException)
            {
                BAMessageBox.ShowValidationError(((ValidationException)e.Exception).Results);
            }
            else
            {
                BAMessageBox.ShowError(e.DisplayMessage, e.ErrorId);    
            }
            
        }

        static void Default_ShowEmailReportWindow(object sender, ErrorEventArgs e)
        {
            if(Dispatcher.CurrentDispatcher.CheckAccess())
            {
                SendErrorWindow dlg = new SendErrorWindow();
                dlg.Fill(e.Exception);
                e.ShouldSend = dlg.ShowDialog() == true;
                e.ApplyAlways = dlg.ApplyAlways;
            }
            else
            {
                Dispatcher.CurrentDispatcher.Invoke(new Action(delegate
                                                                   {
                                                                       SendErrorWindow dlg = new SendErrorWindow();
                                                                       dlg.Fill(e.Exception);
                                                                       e.ShouldSend = dlg.ShowDialog() == true;
                                                                       e.ApplyAlways = dlg.ApplyAlways;
                                                                   }));
            }
            

        }
    }
}
