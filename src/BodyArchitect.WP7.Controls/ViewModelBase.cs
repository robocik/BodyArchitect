using System;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;
using Microsoft.Phone.Controls;

namespace BodyArchitect.WP7.Controls
{
    public class FixedListPicker:ListPicker
    {
        public new int ItemCountThreshold
        {
            get { return (int)GetValue(ItemCountThresholdProperty); }
            set { SetValue(ItemCountThresholdProperty, value); }
        }
    }
    public class ViewModelBase : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsOffline
        {
            get { return ApplicationState.Current.IsOffline; }
        }
    }
}
