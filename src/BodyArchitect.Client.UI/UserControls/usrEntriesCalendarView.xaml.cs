using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BodyArchitect.Client.UI.Views;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Settings;
using vhCalendar;

namespace BodyArchitect.Client.UI.UserControls
{
    public enum CalendarFilter
    {
        All,
        OnlyDone,
        OnlyPlanned
    }
    /// <summary>
    /// Interaction logic for usrEntriesCalendarView.xaml
    /// </summary>
    public partial class usrEntriesCalendarView 
    {
        CalendarViewDayInfo dayInfoFiller = new CalendarViewDayInfo();
        private CalendarFilter filter = CalendarFilter.All;
        private IEnumerable<TrainingDayDTO> days;

        public usrEntriesCalendarView()
        {
            InitializeComponent();
            monthCalendar1.Theme = Themes.OfficeSilver.ToString();
            ShowTrainingDaySummary = true;
            filter = (CalendarFilter) GuiState.Default.CalendarViewMode;
        }

        public vhCalendar.Calendar CalendarControl
        {
            get { return monthCalendar1; }
        }

        

        public void Fill(IEnumerable<TrainingDayDTO> days)
        {
            if (days==null)
            {
                return;
            }
            this.days = days;
            dayInfoFiller.FilterView = FilterView;
            CalendarControl.Appointments.Clear();
            //DateTime start = DateTime.Now;
            foreach (var day in days)
            {
                addTrainingDayInfo(day);
            }
            CalendarControl.RefreshAppointments();
        }

        

        private void addTrainingDayInfo(TrainingDayDTO day)
        {
            if (day != null)
            {
                var item = dayInfoFiller.AddDayInfo(day, ShowTrainingDaySummary);
                if (item != null)
                {
                    CalendarControl.Appointments.Add(item);
                }
                //item.Tag = day;
            }
        }

        public bool ShowTrainingDaySummary { get; set; }

        public DateTime ActiveMonthDateTime
        {
            get { return (DateTime)GetValue(ActiveMonthDateTimeProperty); }
            set
            {
                SetValue(ActiveMonthDateTimeProperty, value);
            }
        }

        /// <summary> 
        ///Gets or sets a collection used to generate the content of the ComboBox 
        /// </summary> 
        public IEnumerable<TrainingDayDTO> ItemsSource
        {
            get { return (IEnumerable<TrainingDayDTO>)GetValue(ItemsSourceProperty); }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public CalendarFilter FilterView
        {
            get { return filter; }
            set
            {
                if (filter != value)
                {
                    filter = value;
                    GuiState.Default.CalendarViewMode = (int) value;
                    Fill(days);
                }
            }
        }

        public static readonly DependencyProperty ActiveMonthDateTimeProperty =
            DependencyProperty.Register("ActiveMonthDateTime", typeof(DateTime), typeof(usrEntriesCalendarView), new UIPropertyMetadata(DateTime.Now.Date));

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<TrainingDayDTO>), typeof(usrEntriesCalendarView), new UIPropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (usrEntriesCalendarView)d;
            var old = e.OldValue as ObservableCollection<TrainingDayDTO>;

            if (old != null)
                old.CollectionChanged -= ctrl.OnWorkCollectionChanged;

            var n = e.NewValue as ObservableCollection<TrainingDayDTO>;

            if (n != null)
                n.CollectionChanged += ctrl.OnWorkCollectionChanged;
            
            
            ctrl.Fill((IEnumerable<TrainingDayDTO>)e.NewValue);


        }

        private void OnWorkCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Fill(ItemsSource);
        }


        public void UpdateDay(TrainingDayDTO newItem)
        {
            CalendarControl.RemoveDateInfo(newItem.TrainingDate,false);
            addTrainingDayInfo(newItem);
            CalendarControl.RefreshAppointments();
        }

        public void RemoveDay(TrainingDayDTO newItem)
        {
            CalendarControl.RemoveDateInfo(newItem.TrainingDate);
        }

    }
}
