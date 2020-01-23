using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.Client.WP7.ModelExtensions;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.Controls.Animations;
using BodyArchitect.WP7.ViewModel;

namespace BodyArchitect.WP7.Pages
{
    public partial class FeaturedPage
    {
        private ObservableCollection<FeaturedEntryObjectDTO> lastTrainings = new ObservableCollection<FeaturedEntryObjectDTO>();
        private ObservableCollection<FeaturedEntryObjectDTO> lastBlogs = new ObservableCollection<FeaturedEntryObjectDTO>();
        private ObservableCollection<ExerciseRecordViewModel> records = new ObservableCollection<ExerciseRecordViewModel>();

        public FeaturedPage()
        {
            InitializeComponent();
            DataContext = this;
            AnimationContext = LayoutRoot;
        }

        public ObservableCollection<FeaturedEntryObjectDTO> LastTrainings
        {
            get { return lastTrainings; }
        }

        public ObservableCollection<FeaturedEntryObjectDTO> LastBlogs
        {
            get { return lastBlogs; }
        }

        public bool IsOfflineMode
        {
            get { return ApplicationState.Current.IsOffline; }
        }


        protected override AnimatorHelperBase GetAnimation(AnimationType animationType, Uri toOrFrom)
        {
            if (animationType == AnimationType.NavigateForwardIn || animationType == AnimationType.NavigateBackwardIn)
                return new SlideUpAnimator() { RootElement = LayoutRoot };
            else
                return new SlideDownAnimator() { RootElement = LayoutRoot };
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(IsOfflineMode)
            {
                //return;
            }
            if (!ApplicationState.Current.Cache.Featured.IsLoaded)
            {
                ApplicationState.Current.Cache.Featured.Loaded+=new EventHandler(Featured_Loaded);
                progressBar.ShowProgress(true, ApplicationStrings.FeaturedPage_RetrievingFeaturedItems);
                ApplicationState.Current.Cache.Featured.Load();
            }
            else
            {
                fillFeatured();
            }
        }

        private void fillFeatured()
        {
            LastTrainings.Clear();
            LastBlogs.Clear();
            Records.Clear();
            foreach (var entry in ApplicationState.Current.Cache.Featured.Item.LatestStrengthTrainings)
            {
                LastTrainings.Add(entry);
            }
            foreach (var entry in ApplicationState.Current.Cache.Featured.Item.LatestBlogs)
            {
                LastBlogs.Add(entry);
            }
            foreach (var entry in ApplicationState.Current.Cache.Featured.Item.Records)
            {
                Records.Add(new ExerciseRecordViewModel(entry));
            }
        }

        private void Featured_Loaded(object s, EventArgs a)
        {
            ApplicationState.Current.Cache.Featured.Loaded -= Featured_Loaded;
            if (ApplicationState.Current.Cache.Featured.IsLoaded)
            {
                fillFeatured();
            }
            else
            {
                BAMessageBox.ShowError(ApplicationStrings.FeaturedPage_ErrCannotRetrieveFeaturedItems);
            }
            progressBar.ShowProgress(false);
        }

        public TrainingDayDTO SelectedTrainingDay { get; set; }

        public ObservableCollection<ExerciseRecordViewModel> Records
        {
            get { return records; }
        }


        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            TrainingDayEntrySelectorPage page = e.Content as TrainingDayEntrySelectorPage;
            if (page != null)
            {
                page.SelectedDate = SelectedTrainingDay.TrainingDate;
                page.SelectedDayDTO = SelectedTrainingDay;
            }

            base.OnNavigatedFrom(e);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            ApplicationState.Current.CurrentBrowsingTrainingDays = null;
            base.OnBackKeyPress(e);
        }

        private void lstLastBlogs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var list = (ListBox)sender;
            if (e.AddedItems.Count > 0)
            {
                var selectedEntry = (FeaturedEntryObjectDTO)e.AddedItems[0];
                retrieveTrainingDay(selectedEntry.DateTime, selectedEntry.User);
            }
            list.SelectedIndex = -1;
        }

        private void retrieveTrainingDay(DateTime date,UserDTO user)
        {
            progressBar.ShowProgress(true, ApplicationStrings.FeaturedPage_RetrievingTrainingDay);
            var m = new ServiceManager<GetTrainingDayCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<GetTrainingDayCompletedEventArgs> operationCompleted)
                                                                             {
                                                                                 WorkoutDayGetOperation param = new WorkoutDayGetOperation();
                                                                                 param.WorkoutDateTime = date;
                                                                                 param.UserId = user.GlobalId;
                                                                                 client1.GetTrainingDayAsync(ApplicationState.Current.SessionData.Token, param, new RetrievingInfo());
                                                                                 client1.GetTrainingDayCompleted -= operationCompleted;
                                                                                 client1.GetTrainingDayCompleted += operationCompleted;

                                                                             });
            m.OperationCompleted += (s1, a1) =>
                                        {
                                            progressBar.ShowProgress(false);
                                            if (a1.Error != null || a1.Result.Result==null)
                                            {
                                                BAMessageBox.ShowError(ApplicationStrings.FeaturedPage_ErrCannotRetrieveTrainingDay);
                                                return;
                                            }
                                            SelectedTrainingDay = a1.Result.Result;
                                            ApplicationState.Current.CurrentBrowsingTrainingDays = new TrainingDaysHolder(user);
                                            ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays.Add(date,new TrainingDayInfo(SelectedTrainingDay));
                                            ApplicationState.Current.CurrentBrowsingTrainingDays.RetrievedMonths.Add(date.MonthDate());
                                            this.Navigate("/Pages/TrainingDayEntrySelectorPage.xaml");
                                        };
            if (!m.Run())
            {
                progressBar.ShowProgress(false);
                if (ApplicationState.Current.IsOffline)
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrOfflineMode);
                }
                else
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);
                }
            }
        }

        private void lstRecords_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var list = (ListBox)sender;
            if (e.AddedItems.Count > 0)
            {
                var item = (ExerciseRecordViewModel)e.AddedItems[0];
                if(item.CalendarAvailable)
                {
                    retrieveTrainingDay(item.Item.TrainingDate,item.Item.User);
                }
                else
                {
                    BAMessageBox.ShowInfo(ApplicationStrings.FeaturedPage_InfoPrivateCalendar);
                }
            }
            list.SelectedIndex = -1;
        }

    }
}