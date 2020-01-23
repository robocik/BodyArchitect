using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls.Animations;
using BodyArchitect.WP7.UserControls;
using BodyArchitect.WP7.ViewModel;
using Coding4Fun.Phone.Controls.Toolkit;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BodyArchitect.WP7.Pages
{
    public abstract class SetPageBase : StrengthTrainingPageBase
    {

        private DispatcherTimer timer;
        private Button btnStart;
        private TimeSpanPicker timePicker;
        private TimerControl ctrlTimer;

        protected SetPageBase()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += OnTimerTick;
        }

        public SetViewModel SelectedSetView { get; set; }

        public SerieDTO SelectedSet { get; set; }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            StateHelper stateHelper = new StateHelper(this.State);

            var startTimer = stateHelper.GetValue<bool>("TimerEnabled", false);
            if (startTimer)
            {
                updateTime();
                StartTimer(false);
            }
            ctrlTimer.IsStarted = ApplicationState.Current.TrainingDay.TrainingDay.IsMine && ApplicationState.Current.IsTimerEnabled;
            var item = stateHelper.GetValue<Guid>("SelectedSetId", Guid.Empty);
            if (item != Guid.Empty && (SelectedSet == null || SelectedSet.InstanceId == item))
            {
                //SelectedSet = ApplicationState.Current.TrainingDay.TrainingDay.StrengthWorkout.GetSet(item);
                SelectedSet = Entry.GetSet(item);
            }
            SelectedSetView = CreateViewModel(SelectedSet);
            DataContext = SelectedSetView;


        }

        protected abstract SetViewModel CreateViewModel(SerieDTO set);

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.State["SelectedSetId"] = SelectedSet.InstanceId;
            this.State["TimerEnabled"] = timer.IsEnabled;

           
            var commentPage = e.Content as CommentPage;
            if (commentPage != null)
            {
                commentPage.CommentableObject = SelectedSet;
            }
            ctrlTimer.IsStarted = false;
            if (timer.IsEnabled)
            {
                StopTimer(false);
            }
        }

        protected void Delete_Click()
        {
            if (!ApplicationState.Current.TrainingDay.TrainingDay.IsMine)
            {
                BAMessageBox.ShowError(ApplicationStrings.ErrCannotModifyEntriesOfAnotherUser);
                return;
            }
            SelectedSetView.Delete();
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private DateTime buttonClickTime;
        private bool stopping;

        void OnTimerTick(object sender, EventArgs e)
        {
            if (!stopping)
            {
                updateTime();    
            }
            
            //after 1 second we should enable again button (we disable it to prevent accidentialy pressing this button)
            if ((DateTime.Now - buttonClickTime).TotalSeconds > 1)
            {
                btnStart.IsEnabled = EditMode;
                if (stopping)
                {
                    timer.Stop();
                }
            }
        }

        private void updateTime()
        {
            var model = SelectedSetView;
            model.SetEndTime();
        }

        public void Connect(Button btnStart, TimeSpanPicker timePicker, TimerControl ctrlTimer)
        {
            this.btnStart = btnStart;
            btnStart.Click -= btnStartTimer_Click;
            btnStart.Click += btnStartTimer_Click;
            this.ctrlTimer = ctrlTimer;
            this.timePicker = timePicker;
        }

        protected  virtual void StopTimer(bool resetValues)
        {
            //SelectedSetView.Set.EndTime = DateTime.Now;
            SelectedSetView.SetEndTime();
            stopping = true;
            //timer.Stop();
            timePicker.IsEnabled = true;
            btnStart.Content = ApplicationStrings.CardioSetPage_StartButton;
            if (resetValues)
            {
                ApplicationState.Current.TimerStartTime = DateTime.Now;
            }
        }


        protected virtual void StartTimer(bool resetValues)
        {
            if (!ApplicationState.Current.TrainingDay.TrainingDay.IsMine)
            {
                BAMessageBox.ShowError(ApplicationStrings.ErrCannotModifyEntriesOfAnotherUser);
                return;
            }
            stopping = false;
            
            if (resetValues)
            {
                //SelectedSetView.Set.StartTime = DateTime.Now;
                SelectedSetView.SetStartTime();
                ApplicationState.Current.TimerStartTime = DateTime.Now;
            }

            timePicker.IsEnabled = false;
            timer.Start();
            btnStart.Content = ApplicationStrings.CardioSetPage_StopButton;
        }

        private void btnStartTimer_Click(object sender, RoutedEventArgs e)
        {
            if (!UpgradeAccountControl.EnsureAccountType(ApplicationStrings.Feature_Premium_StrengthTrainingTimer, this))
            {
                return;
            }
            //after click we disable the button for one second to prevent accidential clicking second time
            buttonClickTime = DateTime.Now;
            btnStart.IsEnabled = false;
            timePicker.Focus();
            if (timer.IsEnabled)
            {
                StopTimer(true);
            }
            else
            {
                StartTimer(true);
            }

        }
    }
}
