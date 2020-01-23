
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using AdRotator;
using AdRotator.Model;
using BodyArchitect.Service.Client.WP7;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BodyArchitect.WP7.Controls
{
    public class BodyArchitectPage : PhoneApplicationPage,INotifyPropertyChanged
    {

        public BodyArchitectPage()
        {
            this.Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            SystemTray.Opacity = 0;
            SystemTray.ForegroundColor = Color.FromArgb(1, 186, 201, 222);
            SystemTray.IsVisible = Settings.ShowSystemTray;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string name)
        {
            if(PropertyChanged!=null)
            {
                PropertyChanged(this,new PropertyChangedEventArgs(name));
            }
        }
    }
}
