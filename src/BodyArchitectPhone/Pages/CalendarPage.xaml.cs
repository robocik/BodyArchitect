using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Client.Common;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.Client.WP7.ModelExtensions;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.UserControls;
using BodyArchitect.WP7.ViewModel;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WPControls;

namespace BodyArchitect.WP7.Pages
{
    public class CalendarColorConverter:IDateToBrushConverter
    {
        public Brush Convert(DateTime dateTime, bool isSelected, Brush defaultValue, BrushType brushType)
        {
            if (brushType == BrushType.Background)
            {
                if (dateTime == DateTime.Today)
                {
                    return ColorName.Orange;
                }
                if (ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays != null && ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays.Where(one => one.Key == dateTime).Any() && !isSelected)
                {
                    return (Brush)Application.Current.Resources["CustomAccentBrush"];
                }
                else
                {
                    return defaultValue;
                } 

            }
            return defaultValue;
        }
    }
    public class CalendarItem:ViewModelBase,ISupportCalendarItem
    {
        public DateTime CalendarItemDate { get; private set; }

        public CalendarItem(DateTime calendarItemDate, TrainingDayInfo trainingDay)
        {
            Item = trainingDay;
            CalendarItemDate = calendarItemDate;
        }

        public IEnumerable<Brush>  Items
        {
            get { return Item.TrainingDay.Objects.Select(x => getBrush(x)); }
        }

        Brush getBrush(EntryObjectDTO entry)
        {
            Type type = entry.GetType();
            if (type == typeof (StrengthTrainingEntryDTO))
            {
                return EntryObjectColors.StrengthTraining;
            }
            else if (type == typeof(SuplementsEntryDTO))
            {
                return EntryObjectColors.Supplements;
            }
            if (type == typeof(BlogEntryDTO))
            {
                return EntryObjectColors.Blog;
            }
            if (type == typeof(GPSTrackerEntryDTO))
            {
                return EntryObjectColors.GPSTracker;
            }
            if (type == typeof(A6WEntryDTO))
            {
                return EntryObjectColors.A6W;
            }
            if (type == typeof(SizeEntryDTO))
            {
                return EntryObjectColors.Measurements;
            }
            return null;
        }

        public TrainingDayInfo Item { get; private set; }
    }

    public partial class CalendarPage
    {
        private DateTime selectedDate;

        public CalendarPage()
        {
            InitializeComponent();
        }
        
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            ApplicationState.Current.CurrentBrowsingTrainingDays = null;
            base.OnBackKeyPress(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (e.Content is TrainingDayEntrySelectorPage)
            {
                (e.Content as TrainingDayEntrySelectorPage).SelectedDate = selectedDate;
            }

            State["CalendarCurrentDate"] = mainCalendar.SelectedDate;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            StateHelper stateHelper = new StateHelper(this.State);
            var item = stateHelper.GetValue<DateTime>("CalendarCurrentDate", DateTime.Now);
            fillCalendar(item.MonthDate());
        }


        private void fillCalendar(DateTime monthDate)
        {
            if (!ApplicationState.Current.CurrentBrowsingTrainingDays.IsMonthLoaded(monthDate))
            {
                if (ApplicationState.Current.IsOffline)
                {
                    fillCalendarData(monthDate, ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays);
                    BAMessageBox.ShowInfo(ApplicationStrings.CalendarPage_EntriesNotRetrieved);
                }
                else
                {
                    getCurrentTrainingDays(monthDate);
                }
                
            }
            else
            {
                fillCalendarData(monthDate, ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays);
            }
           
        }

        private void fillCalendarData(DateTime monthDate,Dictionary<DateTime, TrainingDayInfo> days)
        {
            var list = new List<ISupportCalendarItem>();
            foreach (var item in days)
            {
                list.Add(new CalendarItem(item.Key,item.Value));
            }
            mainCalendar.DatesSource = list;
            mainCalendar.RefreshInfo();
        }


        private void getCurrentTrainingDays(DateTime monthDate)
        {
            progressBar.ShowProgress(true, ApplicationStrings.TrainingDaySelectorControl_ProgressRetrieveEntries);
            mainCalendar.IsHitTestVisible = false;
            ApplicationState.Current.TrainingDaysRetrieved +=new EventHandler<DateEventArgs>(Current_TrainingDaysRetrieved);
            ApplicationState.Current.RetrieveMonth(monthDate, ApplicationState.Current.CurrentBrowsingTrainingDays);
        }

        private void Current_TrainingDaysRetrieved(object sender, DateEventArgs e)
        {
            ApplicationState.Current.TrainingDaysRetrieved -= Current_TrainingDaysRetrieved;
            fillCalendarData(e.Date, ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays);
            progressBar.ShowProgress(false);
            mainCalendar.IsHitTestVisible = true;
            
        }

        private void PageOrientation_Changed(object sender, OrientationChangedEventArgs e)
        {
            if (e.Orientation == PageOrientation.PortraitUp || e.Orientation == PageOrientation.PortraitDown)
            {
                TitlePanel.Visibility = System.Windows.Visibility.Visible;
                ContentPanel.Margin = new Thickness(0, 0, 0, 0);
            }
            else
            {
                TitlePanel.Visibility = System.Windows.Visibility.Collapsed;
                ContentPanel.Margin = new Thickness(32, 0, 0, 0);
            }
            
        }

        private void MainCalendar_OnMonthChanged(object sender, WPControls.MonthChangedEventArgs e)
        {
            fillCalendar(new DateTime(e.Year, e.Month,1));
        }

        private void MainCalendar_OnDateClicked(object sender, WPControls.SelectionChangedEventArgs e)
        {
            selectedDate = e.SelectedDate;
            if (selectedDate.IsFuture() && !UpgradeAccountControl.EnsureAccountType(ApplicationStrings.Feature_Premium_AddEntriesInFuture, this))
            {
                return;
            }
            if (ApplicationState.Current.CurrentBrowsingTrainingDays.IsMine || ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays.ContainsKey(selectedDate))
            {
                this.Navigate("/Pages/TrainingDayEntrySelectorPage.xaml");
            }
        }

       
    }
}