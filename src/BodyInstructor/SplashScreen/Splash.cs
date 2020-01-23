using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI.Controls;

namespace BodyArchitect.SplashScreen
{
    class Splash:ISplashScreen
    {
        private SplashWindow1 splashScreen;
        private string message;

        public SplashWindow1 SplashScreen
        {
            get { return splashScreen; }
        }

        public void HideSplashScreen()
        {
            if (SplashScreen != null)
            {
                SplashScreen.Dispatcher.BeginInvoke(new Action(() =>
                {
                    SplashScreen.Close();
                    splashScreen = null;
                }));
            }
        }

        public void SetSplashMessage(string text)
        {
            if (SplashScreen != null)
            {
                splashScreen.SetStatus(text);
            }
            else
            {
                message = text;
            }
        }

        public void ShowSplashScreen()
        {
            if (SplashScreen == null)
            {
                Thread thread = new Thread(() =>
                {
                    Helper.EnsureThreadLocalized();
                    splashScreen = new SplashWindow1();
                    SplashScreen.Closed += (sender2, e2) => SplashScreen.Dispatcher.InvokeShutdown();
                    if (!string.IsNullOrEmpty(message))
                    {
                        SplashScreen.Loaded += (sender2, e2) => SplashScreen.SetStatus(message);
                    }
                    SplashScreen.Show();
                    SplashScreen.Activate();
                    System.Windows.Threading.Dispatcher.Run();

                });

                thread.SetApartmentState(ApartmentState.STA);
                thread.IsBackground = false;
                thread.Start();
            }
        }

        public void SetStatus(string text)
        {
            if (SplashScreen != null)
            {
                splashScreen.SetStatus(text);
            }
            else
            {
                message = text;
            }
        }

        public void SetProgressBar(bool visible, int max, int value)
        {
            if (SplashScreen != null)
            {
                SplashScreen.SetProgressBar(visible, max, value);
            }
        }
    }
}
