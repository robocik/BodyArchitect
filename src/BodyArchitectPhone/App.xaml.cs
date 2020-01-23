using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AdRotator;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7;
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.Controls.Animations;
using BodyArchitect.WP7.Controls.Cache;
using BodyArchitect.WP7.UserControls;
using BugSense;
using BugSense.Core.Model;
using ImageTools.IO;
using ImageTools.IO.Bmp;
using ImageTools.IO.Gif;
using ImageTools.IO.Jpeg;
using ImageTools.IO.Png;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Info;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace BodyArchitectPhone
{
    public partial class App : Application
    {
        private AdRotatorControl ctrlAd;

        public static App CustomApp
        {
            get { return (App) Current; }
        }
        
        /// <summary>
        /// Provides easy access to the root frame of the Custom Application.
        /// </summary>
        /// <returns>The root frame of the Custom Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }


        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            var errorManager = new ExceptionManager(Current);
            errorManager.UnhandledException += Application_UnhandledException;
            BugSenseHandler.Instance.InitAndStartSession(errorManager, "1c3b051f");
            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are being GPU accelerated with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;
            }

            // Standard Silverlight initialization
            InitializeComponent();

            // Custom-specific initialization
            InitializeCustomApplication();
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            setCorrectLanguage();
            
            //NonLinearNavigationService.Instance.Initialize(RootFrame);
            Settings.RunsCount++;
            ThreadPool.QueueUserWorkItem(delegate
                                             {
                                                 Decoders.AddDecoder<BmpDecoder>();
                                                 Decoders.AddDecoder<PngDecoder>();
                                                 Decoders.AddDecoder<GifDecoder>();
                                                 Decoders.AddDecoder<JpegDecoder>();
                                             });
            
            PagesState.Current=new PagesState();
            PicturesCache.Load();
        }

        void setCorrectLanguage()
        {
            if (Settings.ExercisesLanguage != null)
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(Settings.ExercisesLanguage);
                //Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;
            }
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            if (!e.IsApplicationInstancePreserved)
            {
                setCorrectLanguage();
                ThreadPool.QueueUserWorkItem(delegate
                                                 {
                                                     Decoders.AddDecoder<BmpDecoder>();
                                                     Decoders.AddDecoder<PngDecoder>();
                                                     Decoders.AddDecoder<GifDecoder>();
                                                     Decoders.AddDecoder<JpegDecoder>();
                                                 });
                ApplicationState.Current = ApplicationState.LoadState();
                PagesState.Current = PagesState.LoadState();
                PicturesCache.Load();
            }
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            if (ApplicationState.Current != null && !ApplicationState.Current.Crash)
            {
                ApplicationState.Current.SaveState(false);
            }
            if (PagesState.Current!=null)
            {
                PagesState.Current.SaveState();
            }
            PicturesCache.Instance.Save();
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            if (ApplicationState.Current != null && !ApplicationState.Current.Crash)
            {
                ApplicationState.Current.SaveState(false);
            }
            PicturesCache.Instance.Save();
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            ApplicationState.Current.SaveState(true);
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
            else
            {

                //EmailComposeTask emailComposeTask = new EmailComposeTask();

                //emailComposeTask.Subject = "Unhandled exception occured";
                //emailComposeTask.Body = e.ExceptionObject.StackTrace;
                //emailComposeTask.To = "recipient@example.com";

                //emailComposeTask.Show();

#if DEBUG
                MessageBox.Show(e.ExceptionObject.Message);
                MessageBox.Show(e.ExceptionObject.StackTrace);
#else
                BAMessageBox.ShowError(ApplicationStrings.ErrUnhandledErrorOccured);
#endif
            }
        }

        #region Custom application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializeCustomApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            //RootFrame = new PhoneApplicationFrame();
            var test = (Style) Resources["mainFrameStyle"];
            var frame = new FadeOrientationChangesFrame() { Style = (Style)Resources["mainFrameStyle"] };
            frame.Duration = TimeSpan.FromSeconds(1);
            RootFrame = frame;
            RootFrame.Navigated += CompleteInitializeCustomApplication;
            RootFrame.Navigating += RootFrame_Navigating;
            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        void RootFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            
            EnsureAdVisible(e);
        }

        public void EnsureAdVisible(NavigatingCancelEventArgs e)
        {
            if (ctrlAd == null)
            {
                return;
            }
            if (ApplicationState.Current == null || ApplicationState.Current.IsPremium ||
                (e!=null && e.Uri.OriginalString.Contains("AboutPage.xaml")))
            {
                ctrlAd.Visibility = Visibility.Collapsed;
            }
            else
            {
                ctrlAd.Visibility = Visibility.Visible;
            }
            if (ctrlAd.Visibility == Visibility.Visible)
            {
                ctrlAd.Invalidate();
            }
        }


        // Do not add any additional code to this method
        private void CompleteInitializeCustomApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializeCustomApplication;
            ApplicationState.SynchronizationContext =  SynchronizationContext.Current;
        }

        #endregion

        private void AddRotator_OnLoaded(object sender, RoutedEventArgs e)
        {
            ctrlAd = (AdRotatorControl) sender;
        }

        public void ShowAdverts(bool show)
        {
            //if (ctrlAd == null)
            //{
            //    return;
            //}
            //if (show)
            //{
            //    ctrlAd.Visibility = Visibility.Visible;
            //    ctrlAd.Invalidate();
            //}
            //else
            //{
            //    ctrlAd.Visibility = Visibility.Collapsed;
            //}
        }

        private void btnRemoveAdds_Click(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var currentPage=RootFrame.Content as PhoneApplicationPage;
            UpgradeAccountControl.EnsureAccountType(ApplicationStrings.Feature_Premium_NoAds, (DependencyObject)currentPage);
        }
    }
}