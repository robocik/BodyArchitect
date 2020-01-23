using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI.Controls;
using Xceed.Wpf.Toolkit;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls
{
    /// <summary>
    /// Interaction logic for TimerWindow.xaml
    /// </summary>
    public partial class TimerWindow
    {
        private DispatcherTimer timer;
        private DateTime? startTime=null;
        private DateTime? endTime;

        public TimerWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            Closing += new EventHandler<System.ComponentModel.CancelEventArgs>(TimerWindow_Closing);
            
        }

        protected override void OnWindowStatePropertyChanged(Xceed.Wpf.Toolkit.WindowState oldValue, Xceed.Wpf.Toolkit.WindowState newValue)
        {
            base.OnWindowStatePropertyChanged(oldValue, newValue);

            updateButtons(false);
            lblTime.Text = TimeSpan.Zero.ToString();
            chkStartTimer.IsChecked = UserContext.Current.Settings.GuiState.StartTimerAfterOpeningWindow;
            if (newValue == Xceed.Wpf.Toolkit.WindowState.Open)
            {
                DialogResult = null;
                if (UserContext.Current.Settings.GuiState.StartTimerAfterOpeningWindow)
                {
                    startTimer();
                }
            }
            
        }

        public DateTime? StartTime
        {
            get { return startTime; }
        }

        public DateTime? EndTime
        {
            get { return endTime; }
        }

        //public SetViewModel Set { get; private set; }

        //public bool IsForRest { get; private set; }




        void TimerWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            timer.IsEnabled = false;
            endTime = DateTime.Now;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            lblTime.Text = TimeSpan.FromSeconds((int)(DateTime.Now - startTime.Value).TotalSeconds).ToString();
        }

        void updateButtons(bool start)
        {
            btnStart.SetVisible(!start);
            btnStop.SetVisible(start);
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            startTimer();
        }

        private void startTimer()
        {
            updateButtons(true);
            timer.IsEnabled = true;
            startTime = DateTime.Now;
        }

        public DataGridCellInfo Cell { get; set; }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }


        private void chkStartTimer_Checked(object sender, RoutedEventArgs e)
        {
            UserContext.Current.Settings.GuiState.StartTimerAfterOpeningWindow = chkStartTimer.IsChecked.Value;
        }



    }
}
