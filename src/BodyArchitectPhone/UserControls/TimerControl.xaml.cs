using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.WP7.Annotations;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BodyArchitect.WP7.UserControls
{
    public partial class TimerControl:INotifyPropertyChanged
    {
        private DispatcherTimer timer;
        private bool _isStarted;
        public event EventHandler IsStartedChanged;
        public event EventHandler Tick;

        public TimerControl()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += OnTimerTick;
        }

        public bool IsStarted
        {
            get { return _isStarted; }
            set
            {
                if (_isStarted != value)
                {
                    _isStarted = value;
                    if (_isStarted)
                    {
                        StartTimer();
                    }
                    else
                    {
                        StopTimer();
                    }

                    OnPropertyChanged("IsStarted");
                    OnIsStartedChanged();
                }
            }
        }

        private void StartTimer()
        {
            Visibility = Visibility.Visible;
            updateTime();
            timer.Start();
            //ApplicationState.Current.IsTimerEnabled = true;
        }

        private void StopTimer()
        {
            Visibility = Visibility.Collapsed;
            timer.Stop();
            //ApplicationState.Current.IsTimerEnabled = false;
        }

        void OnTimerTick(object sender, EventArgs e)
        {
            updateTime();
            if (Tick != null)
            {
                Tick(this, e);
            }
        }

        void updateTime()
        {
            if (ApplicationState.Current.TimerStartTime == null)
            {
                tbTimer.Text = TimeSpan.Zero.ToString();
            }
            else
            {
                tbTimer.Text = TimeSpan.FromSeconds((int)(DateTime.Now - ApplicationState.Current.TimerStartTime.Value).TotalSeconds).ToString();    
            }
            
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnIsStartedChanged()
        {
            if (IsStartedChanged != null)
            {
                IsStartedChanged(this, EventArgs.Empty);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
