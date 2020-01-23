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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI.Controls.Calendar.Common;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace BodyArchitect.Client.UI.Controls.Calendar
{

    public class ExDragEventArgs : RoutedEventArgs
    {
        public ExDragEventArgs(DragEventArgs e,Control directTarget)
        {
            this.DirectTarget = directTarget;
            DragEventArgs = e;
        }

        public Control DirectTarget { get; private set; }

        public DragEventArgs DragEventArgs { get; private set; }

    }
    [TemplatePart(Name = Calendar.ElementDay, Type = typeof(CalendarDay))]
    [TemplatePart(Name = "PART_Day1", Type = typeof(CalendarDay))]
    [TemplatePart(Name = Calendar.ElementDayHeader, Type = typeof(CalendarDayHeader))]
    [TemplatePart(Name = Calendar.ElementLedger, Type = typeof(CalendarLedger))]
    [TemplatePart(Name = Calendar.ElementScrollViewer, Type = typeof(ScrollViewer))]
    public partial class Calendar : Control,ICalendarControl
    {
        private const string ElementDay = "PART_Day";
        private const string ElementDayHeader = "PART_DayHeader";
        private const string ElementLedger = "PART_Ledger";
        private const string ElementScrollViewer = "PART_ScrollViewer";
        public event EventHandler SelectedAppointmentChanged;

        public Calendar()
        {
            PeekStartHour = 7;
            PeekEndHour = 18;
            ShowOffPeekHours = true;
        }


        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register("ReadOnly", typeof(bool), typeof(Calendar), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender,
                    new PropertyChangedCallback(OnReadOnlyChanged)));


        protected virtual  void OnSelectedAppointmentChanged()
        {
            if(SelectedAppointmentChanged!=null)
            {
                SelectedAppointmentChanged(this, EventArgs.Empty);
            }
        }

        public IAppointment SelectedAppointment
        {
            get { return (IAppointment)GetValue(SelectedAppointmentProperty); }
            set
            {
                SetValue(SelectedAppointmentProperty, value);
            }
        }


        public static readonly DependencyProperty SelectedAppointmentProperty =
            DependencyProperty.Register("SelectedAppointment", typeof(IAppointment), typeof(Calendar), new UIPropertyMetadata(null, OnSelectedAppointmentChanged));

        private static void OnSelectedAppointmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (Calendar)d;

            foreach (var appointment in ctrl.Appointments)
            {
                var test = ctrl.ContainerFromItem(appointment);
                test.SetSelection(false);
            }
            var sel = ctrl.ContainerFromItem((IAppointment)e.NewValue);
            if (sel != null)
            {
                sel.SetSelection(true);
            }

            ctrl.OnSelectedAppointmentChanged();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if(e.Key==Key.Delete && SelectedAppointment!=null && !ReadOnly)
            {
                RoutedEventArgs e1 = new RoutedEventArgs();
                e1.RoutedEvent = DeleteAppointmentEvent;
                e1.Source = SelectedAppointment;
                RaiseEvent(e1);
            }
        }

        private CalendarAppointmentItem ContainerFromItem(IAppointment appointment)
        {
            var test = (CalendarAppointmentItem)_day.ItemContainerGenerator.ContainerFromItem(appointment);
            if (test == null)
            {
                test = (CalendarAppointmentItem)_day1.ItemContainerGenerator.ContainerFromItem(appointment);
            }
            if (test == null)
            {
                test = (CalendarAppointmentItem)_day2.ItemContainerGenerator.ContainerFromItem(appointment);
            }
            if (test == null)
            {
                test = (CalendarAppointmentItem)_day3.ItemContainerGenerator.ContainerFromItem(appointment);
            }
            if (test == null)
            {
                test = (CalendarAppointmentItem)_day4.ItemContainerGenerator.ContainerFromItem(appointment);
            }
            if (test == null)
            {
                test = (CalendarAppointmentItem)_day5.ItemContainerGenerator.ContainerFromItem(appointment);
            }
            if (test == null)
            {
                test = (CalendarAppointmentItem)_day6.ItemContainerGenerator.ContainerFromItem(appointment);
            }
            return test;
        }

        [Category("Calendar")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set { SetValue(ReadOnlyProperty, value); }
        }

        private static void OnReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Calendar)d).OnReadOnlyChanged();
        }

        protected virtual void OnReadOnlyChanged()
        {
            AllowDrop = !ReadOnly;
        }

        //protected override void OnDragEnter(DragEventArgs e)
        //{

        //    e.Effects = DragDropEffects.None;
        //    if ((e.Data.GetDataPresent("myFormat") || e.Data.GetDataPresent("UslugaDTO")) && (e.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey &&
        //(e.AllowedEffects & DragDropEffects.Copy) == DragDropEffects.Copy)
        //    {
        //        e.Effects = DragDropEffects.Copy;
        //    }
        //    else
        //        if ((e.Data.GetDataPresent("myFormat") || e.Data.GetDataPresent("UslugaDTO")) && (e.AllowedEffects & DragDropEffects.Move) == DragDropEffects.Move)
        //        {
        //            e.Effects = DragDropEffects.Move;

        //        }
        //    base.OnDragEnter(e);
        //}

        //protected override void OnDragOver(DragEventArgs e)
        //{
        //    e.Effects = DragDropEffects.None;
        //    if ((e.Data.GetDataPresent("myFormat") || e.Data.GetDataPresent("UslugaDTO")) && (e.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey &&
        //(e.AllowedEffects & DragDropEffects.Copy) == DragDropEffects.Copy)
        //    {
        //        e.Effects = DragDropEffects.Copy;
        //    }
        //    else
        //        if ((e.Data.GetDataPresent("myFormat") || e.Data.GetDataPresent("UslugaDTO")) && (e.AllowedEffects & DragDropEffects.Move) == DragDropEffects.Move)
        //        {
        //            e.Effects = DragDropEffects.Move;

        //        }


        //    e.Handled = true;
        //    base.OnDragOver(e);
        //}

        static Calendar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Calendar), new FrameworkPropertyMetadata(typeof(Calendar)));

            CommandManager.RegisterClassCommandBinding(typeof(Calendar), new CommandBinding(NextDay, new ExecutedRoutedEventHandler(OnExecutedNextDay), new CanExecuteRoutedEventHandler(OnCanExecuteNextDay)));
            CommandManager.RegisterClassCommandBinding(typeof(Calendar), new CommandBinding(PreviousDay, new ExecutedRoutedEventHandler(OnExecutedPreviousDay), new CanExecuteRoutedEventHandler(OnCanExecutePreviousDay)));
        }

        #region CalendarTimeslotItemStyle

        public static readonly DependencyProperty CalendarTimeslotItemStyleProperty =
            DependencyProperty.Register("CalendarTimeslotItemStyle", typeof(Style), typeof(Calendar));

        [Category("Calendar")]
        //[Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Style CalendarTimeslotItemStyle
        {
            get { return (Style)GetValue(CalendarTimeslotItemStyleProperty); }
            set { SetValue(CalendarTimeslotItemStyleProperty, value); }
        }

        #endregion

        #region CalendarLedgerItemStyle

        public static readonly DependencyProperty CalendarLedgerItemStyleProperty =
            DependencyProperty.Register("CalendarLedgerItemStyle", typeof(Style), typeof(Calendar));

        [Category("Calendar")]
        //[Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Style CalendarLedgerItemStyle
        {
            get { return (Style)GetValue(CalendarLedgerItemStyleProperty); }
            set { SetValue(CalendarLedgerItemStyleProperty, value); }
        }

        #endregion

        #region CalendarAppointmentItemStyle

        public static readonly DependencyProperty CalendarAppointmentItemStyleProperty =
            DependencyProperty.Register("CalendarAppointmentItemStyle", typeof(Style), typeof(Calendar));

        [Category("Calendar")]
        //[Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Style CalendarAppointmentItemStyle
        {
            get { return (Style)GetValue(CalendarAppointmentItemStyleProperty); }
            set { SetValue(CalendarAppointmentItemStyleProperty, value); }
        }

        #endregion

        #region AddAppointment

        public static readonly RoutedEvent AddAppointmentEvent = 
            CalendarTimeslotItem.AddAppointmentEvent.AddOwner(typeof(CalendarDay));

        public event RoutedEventHandler AddAppointment
        {
            add
            {
                AddHandler(AddAppointmentEvent, value);
            }
            remove
            {
                RemoveHandler(AddAppointmentEvent, value);
            }
        }

        #endregion

        #region ExDrop

        public static readonly RoutedEvent ExDropEvent =
            CalendarTimeslotItem.ExDropEvent.AddOwner(typeof(CalendarDay));

        public event RoutedEventHandler ExDrop
        {
            add
            {
                AddHandler(ExDropEvent, value);
            }
            remove
            {
                RemoveHandler(ExDropEvent, value);
            }
        }

        #endregion

        #region EditAppointment

        public static readonly RoutedEvent EditAppointmentEvent =
            CalendarAppointmentItem.EditAppointmentEvent.AddOwner(typeof(CalendarDay));

        public event RoutedEventHandler EditAppointment
        {
            add
            {
                AddHandler(EditAppointmentEvent, value);
            }
            remove
            {
                RemoveHandler(EditAppointmentEvent, value);
            }
        }

        #endregion

        #region DeleteAppointment

        public static readonly RoutedEvent DeleteAppointmentEvent =
            CalendarAppointmentItem.DeleteAppointmentEvent.AddOwner(typeof(CalendarDay));

        public event RoutedEventHandler DeleteAppointment
        {
            add
            {
                AddHandler(DeleteAppointmentEvent, value);
            }
            remove
            {
                RemoveHandler(DeleteAppointmentEvent, value);
            }
        }

        #endregion

        #region DeleteAppointment

        public static readonly RoutedEvent ChangeAppointmentTimeEvent =
            CalendarAppointmentItem.ChangeAppointmentTimeEvent.AddOwner(typeof(CalendarDay));

        public event RoutedEventHandler ChangeAppointmentTime
        {
            add
            {
                AddHandler(ChangeAppointmentTimeEvent, value);
            }
            remove
            {
                RemoveHandler(ChangeAppointmentTimeEvent, value);
            }
        }

        #endregion

        #region Appointments

        public static readonly DependencyProperty AppointmentsProperty =
            DependencyProperty.Register("Appointments", typeof(IEnumerable<IAppointment>), typeof(Calendar),
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(Calendar.OnAppointmentsChanged)));

        public IEnumerable<IAppointment> Appointments
        {
            get { return (IEnumerable<IAppointment>)GetValue(AppointmentsProperty); }
            set { SetValue(AppointmentsProperty, value); }
        }

        private static void OnAppointmentsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Calendar)d).OnAppointmentsChanged(e);
        }

        protected virtual void OnAppointmentsChanged(DependencyPropertyChangedEventArgs e)
        {
            fillDays();

            INotifyCollectionChanged appointments = Appointments as INotifyCollectionChanged;
            if (appointments != null)
            {
                appointments.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Appointments_CollectionChanged);
            }
            FilterAppointments();
        }

        void Appointments_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            FilterAppointments();
        }

        #endregion        
       
        #region CurrentDate

        public static readonly DependencyProperty CurrentDateProperty =
            DependencyProperty.Register("CurrentDate", typeof(DateTime), typeof(Calendar),
                new FrameworkPropertyMetadata(DateTime.Now, FrameworkPropertyMetadataOptions.AffectsRender, OnCurrentDateChanged));

        [Category("Calendar")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public DateTime CurrentDate
        {
            get { return (DateTime)GetValue(CurrentDateProperty); }
            set { SetValue(CurrentDateProperty, value); }
        }

        public event EventHandler CurrentDateChanged;

        private static void OnCurrentDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Calendar)d).OnCurrentDateChanged(e);
        }

        protected virtual void OnCurrentDateChanged(DependencyPropertyChangedEventArgs e)
        {
            fillHeaders();
            fillDays();
            FilterAppointments();
            if(CurrentDateChanged!=null)
            {
                CurrentDateChanged(this, EventArgs.Empty);
            }
        }

        public void SetPeekTime(int startHour, int endHour)
        {
            PeekStartHour = startHour;
            PeekEndHour = endHour;
            fillDays();
        }

        public int PeekStartHour
        {
            get;
            private set;
        }


        public int PeekEndHour
        {
            get;
            private set;
        }

        public static readonly DependencyProperty ShowSingleDayProperty =
            DependencyProperty.Register("ShowSingleDay", typeof(bool), typeof(Calendar),
                new FrameworkPropertyMetadata(false,
                    new PropertyChangedCallback(OnShowSingleDayChanged)));

        [Category("Calendar")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public bool ShowSingleDay
        {
            get { return (bool)GetValue(ShowSingleDayProperty); }
            set { SetValue(ShowSingleDayProperty, value); }
        }

        private static void OnShowSingleDayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Calendar)d).OnShowSingleDayChanged(e);
        }

        protected virtual void OnShowSingleDayChanged(DependencyPropertyChangedEventArgs e)
        {
            //fillDays();
            buildControl();
        }


        private bool _showOffPeekHours=true;
        public bool ShowOffPeekHours
        {
            get { return _showOffPeekHours; }
            set
            {
                if (_showOffPeekHours != value)
                {
                    _showOffPeekHours = value;
                    fillDays();
                }
            }
        }

        public DateTime StartDate
        {
            get { return ShowSingleDay?CurrentDate:CurrentDate.StartOfWeek(); }
        }

        public DateTime EndDate
        {
            get { return ShowSingleDay ? CurrentDate : CurrentDate.StartOfWeek().AddDays(6); }
        }

        void fillHeaders()
        {
            if (_dayHeader == null)
            {
                return;
            }
            if(ShowSingleDay)
            {
                _dayHeader.CurrentDate = CurrentDate;
            }
            else
            {
                var start = StartDate;
                _dayHeader.CurrentDate = start;
                _dayHeader1.CurrentDate = start.AddDays(1);
                _dayHeader2.CurrentDate = start.AddDays(2);
                _dayHeader3.CurrentDate = start.AddDays(3);
                _dayHeader4.CurrentDate = start.AddDays(4);
                _dayHeader5.CurrentDate = start.AddDays(5);
                _dayHeader6.CurrentDate = start.AddDays(6);
            }
            
        }

        void fillDays()
        {
            if(_day==null)
            {
                return;
            }
            
            if(ShowSingleDay)
            {
                _day.CurrentDate = CurrentDate;
            }
            else
            {
                var start = StartDate;
                _day.CurrentDate = start;
                _day1.CurrentDate = start.AddDays(1);
                _day2.CurrentDate = start.AddDays(2);
                _day3.CurrentDate = start.AddDays(3);
                _day4.CurrentDate = start.AddDays(4);
                _day5.CurrentDate = start.AddDays(5);
                _day6.CurrentDate = start.AddDays(6);
            }
            
        }
        #endregion             

        #region PeakTimeslotBackground

        public static readonly DependencyProperty PeakTimeslotBackgroundProperty =
            DependencyProperty.Register("PeakTimeslotBackground", typeof(Brush), typeof(Calendar),
                new FrameworkPropertyMetadata((Brush)Brushes.White,
                    new PropertyChangedCallback(OnPeakTimeslotBackgroundChanged)));

        [Category("Calendar")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public Brush PeakTimeslotBackground
        {
            get { return (Brush)GetValue(PeakTimeslotBackgroundProperty); }
            set { SetValue(PeakTimeslotBackgroundProperty, value); }
        }

        private static void OnPeakTimeslotBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Calendar)d).OnPeakTimeslotBackgroundChanged(e);
        }

        protected virtual void OnPeakTimeslotBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            fillDays();
        }

        #endregion

        #region OffPeakTimeslotBackground

        public static readonly DependencyProperty OffPeakTimeslotBackgroundProperty =
            DependencyProperty.Register("OffPeakTimeslotBackground", typeof(Brush), typeof(Calendar),
                new FrameworkPropertyMetadata((Brush)Brushes.LightCyan,
                    new PropertyChangedCallback(OnOffPeakTimeslotBackgroundChanged)));

        [Category("Calendar")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public Brush OffPeakTimeslotBackground
        {
            get { return (Brush)GetValue(OffPeakTimeslotBackgroundProperty); }
            set { SetValue(OffPeakTimeslotBackgroundProperty, value); }
        }

        private static void OnOffPeakTimeslotBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Calendar)d).OnOffPeakTimeslotBackgroundChanged(e);
        }

        protected virtual void OnOffPeakTimeslotBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            fillDays();
        }

        #endregion

        #region Header colors

        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(Calendar),
                new FrameworkPropertyMetadata((Brush)Brushes.LightGray,
                    new PropertyChangedCallback(HeaderBackgroundBackgroundChanged)));

        [Category("Calendar")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public Brush HeaderBackground
        {
            get { return (Brush)GetValue(HeaderBackgroundProperty); }
            set { SetValue(HeaderBackgroundProperty, value); }
        }

        private static void HeaderBackgroundBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Calendar)d).OnHeaderBackgroundBackgroundChanged(e);
        }

        protected virtual void OnHeaderBackgroundBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            fillHeaders();
        }

        public static readonly DependencyProperty TodayHeaderBackgroundProperty =
            DependencyProperty.Register("TodayHeaderBackground", typeof(Brush), typeof(Calendar),
                new FrameworkPropertyMetadata((Brush)Brushes.LightGoldenrodYellow,
                    new PropertyChangedCallback(TodayHeaderBackgroundBackgroundChanged)));

        [Category("Calendar")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public Brush TodayHeaderBackground
        {
            get { return (Brush)GetValue(TodayHeaderBackgroundProperty); }
            set { SetValue(TodayHeaderBackgroundProperty, value); }
        }

        protected List<CalendarDay> Days
        {
            get { return days; }
        }

        private static void TodayHeaderBackgroundBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Calendar)d).OnTodayHeaderBackgroundBackgroundChanged(e);
        }

        protected virtual void OnTodayHeaderBackgroundBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            fillHeaders();
        }

        #endregion

        

        private void FilterAppointments()
        {
            if (_day == null || Appointments==null)
            {
                return;
            }
            _day.ItemsSource = Appointments.ByDate(_day.CurrentDate);
            if (!ShowSingleDay)
            {
                _day1.ItemsSource = Appointments.ByDate(_day1.CurrentDate);
                _day2.ItemsSource = Appointments.ByDate(_day2.CurrentDate);
                _day3.ItemsSource = Appointments.ByDate(_day3.CurrentDate);
                _day4.ItemsSource = Appointments.ByDate(_day4.CurrentDate);
                _day5.ItemsSource = Appointments.ByDate(_day5.CurrentDate);
                _day6.ItemsSource = Appointments.ByDate(_day6.CurrentDate);

            }
        }

        
        CalendarLedger _ledger;
        CalendarDayHeader _dayHeader;
        CalendarDayHeader _dayHeader1;
        CalendarDayHeader _dayHeader2;
        CalendarDayHeader _dayHeader3;
        CalendarDayHeader _dayHeader4;
        CalendarDayHeader _dayHeader5;
        CalendarDayHeader _dayHeader6;
        ScrollViewer _scrollViewer;

        private List<CalendarDay> days;
        CalendarDay _day;
        CalendarDay _day1;
        private CalendarDay _day2;
        CalendarDay _day3;
        CalendarDay _day4;
        CalendarDay _day5;
        CalendarDay _day6;


        public void RefreshAppointments()
        {
            FilterAppointments();
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //_ledger = GetTemplateChild(ElementLedger) as CalendarLedger;
            //if (_ledger != null)
            //{
            //    _ledger.Owner = this;
            //}
            


            buildControl();
            _scrollViewer = GetTemplateChild(ElementScrollViewer) as ScrollViewer;
        }


        private void buildControl()
        {
            var grid = GetTemplateChild("calendarGrid") as Grid;
            if (grid==null)
            {
                return;
            }
            grid.Children.Clear();
            grid.ColumnDefinitions.Clear();
            grid.ColumnDefinitions.Add(new ColumnDefinition() {Width = new GridLength(50)});
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            if (!ShowSingleDay)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            Brush borderBrush= (Brush)Application.Current.Resources["NormalBorderBrush"];
            _ledger = new CalendarLedger();
            _ledger.Owner = this;
            Grid.SetRow(_ledger, 1);
            Grid.SetColumn(_ledger, 0);
            grid.Children.Add(_ledger);


            Border border = new Border();
            border.BorderBrush = borderBrush;
            border.BorderThickness = new Thickness(0, 0, 1, 1);
            Grid.SetRow(border, 0);
            Grid.SetColumn(border, 0);
            grid.Children.Add(border);


            _day = new CalendarDay();
            days = new List<CalendarDay>();
            Days.Add(_day);
            border = new Border { BorderBrush = borderBrush, BorderThickness = new Thickness(0, 0, 1, 0), Child = _day };
            Grid.SetRow(border, 1);
            Grid.SetColumn(border, 1);
            grid.Children.Add(border);

            _dayHeader = new CalendarDayHeader();
            border = new Border {BorderBrush = borderBrush, BorderThickness = new Thickness(0,0,1,0), Child = _dayHeader};
            Grid.SetRow(border, 0);
            Grid.SetColumn(border, 1);
            grid.Children.Add(border);


            if (!ShowSingleDay)
            {
                _day1 = new CalendarDay();
                Days.Add(_day1);
                border = new Border {BorderBrush = borderBrush, BorderThickness = new Thickness(0,0,1,0), Child = _day1};
                Grid.SetRow(border, 1);
                Grid.SetColumn(border, 2);
                grid.Children.Add(border);

                _dayHeader1 = new CalendarDayHeader();
                border = new Border {BorderBrush = borderBrush, BorderThickness = new Thickness(0,0,1,0), Child = _dayHeader1};
                Grid.SetRow(border, 0);
                Grid.SetColumn(border, 2);
                grid.Children.Add(border);

                _day2 = new CalendarDay();
                Days.Add(_day2);
                border = new Border {BorderBrush = borderBrush, BorderThickness = new Thickness(0,0,1,0), Child = _day2};
                Grid.SetRow(border, 1);
                Grid.SetColumn(border, 3);
                grid.Children.Add(border);

                _dayHeader2 = new CalendarDayHeader();
                border = new Border {BorderBrush = borderBrush, BorderThickness = new Thickness(0,0,1,0), Child = _dayHeader2};
                Grid.SetRow(border, 0);
                Grid.SetColumn(border, 3);
                grid.Children.Add(border);

                _day3 = new CalendarDay();
                Days.Add(_day3);
                border = new Border {BorderBrush = borderBrush, BorderThickness = new Thickness(0,0,1,0), Child = _day3};
                Grid.SetRow(border, 1);
                Grid.SetColumn(border, 4);
                grid.Children.Add(border);

                _dayHeader3 = new CalendarDayHeader();
                border = new Border {BorderBrush = borderBrush, BorderThickness = new Thickness(0,0,1,0), Child = _dayHeader3};
                Grid.SetRow(border, 0);
                Grid.SetColumn(border, 4);
                grid.Children.Add(border);

                _day4 = new CalendarDay();
                Days.Add(_day4);
                border = new Border {BorderBrush = borderBrush, BorderThickness = new Thickness(0,0,1,0), Child = _day4};
                Grid.SetRow(border, 1);
                Grid.SetColumn(border, 5);
                grid.Children.Add(border);

                _dayHeader4 = new CalendarDayHeader();
                border = new Border {BorderBrush = borderBrush, BorderThickness = new Thickness(0,0,1,0), Child = _dayHeader4};
                Grid.SetRow(border, 0);
                Grid.SetColumn(border, 5);
                grid.Children.Add(border);

                _day5 = new CalendarDay();
                Days.Add(_day5);
                border = new Border {BorderBrush = borderBrush, BorderThickness = new Thickness(0,0,1,0), Child = _day5};
                Grid.SetRow(border, 1);
                Grid.SetColumn(border, 6);
                grid.Children.Add(border);

                _dayHeader5 = new CalendarDayHeader();
                border = new Border {BorderBrush = borderBrush, BorderThickness = new Thickness(0,0,1,0), Child = _dayHeader5};
                Grid.SetRow(border, 0);
                Grid.SetColumn(border, 6);
                grid.Children.Add(border);

                _day6 = new CalendarDay();
                Days.Add(_day6);
                border = new Border {BorderBrush = borderBrush, BorderThickness = new Thickness(0), Child = _day6};
                Grid.SetRow(border, 1);
                Grid.SetColumn(border, 7);
                grid.Children.Add(border);

                _dayHeader6 = new CalendarDayHeader();
                border = new Border {BorderBrush = borderBrush, BorderThickness = new Thickness(0), Child = _dayHeader6};
                Grid.SetRow(border, 0);
                Grid.SetColumn(border, 7);
                grid.Children.Add(border);
            }


            //_day = GetTemplateChild(ElementDay) as CalendarDay;
            //_day1 = GetTemplateChild("PART_Day1") as CalendarDay;
            //_day2 = GetTemplateChild("PART_Day2") as CalendarDay;
            //_day3 = GetTemplateChild("PART_Day3") as CalendarDay;
            //_day4 = GetTemplateChild("PART_Day4") as CalendarDay;
            //_day5 = GetTemplateChild("PART_Day5") as CalendarDay;
            //_day6 = GetTemplateChild("PART_Day6") as CalendarDay;
            ////if (_day != null)
            ////{
            ////    _day.Owner = this;
            ////}

            //_dayHeader = GetTemplateChild(ElementDayHeader) as CalendarDayHeader;
            //_dayHeader1 = GetTemplateChild("PART_DayHeader1") as CalendarDayHeader;
            //_dayHeader2 = GetTemplateChild("PART_DayHeader2") as CalendarDayHeader;
            //_dayHeader3 = GetTemplateChild("PART_DayHeader3") as CalendarDayHeader;
            //_dayHeader4 = GetTemplateChild("PART_DayHeader4") as CalendarDayHeader;
            //_dayHeader5 = GetTemplateChild("PART_DayHeader5") as CalendarDayHeader;
            //_dayHeader6 = GetTemplateChild("PART_DayHeader6") as CalendarDayHeader;
            //if (_dayHeader != null)
            //{
            //    _dayHeader.Owner = this;
            //}
            fillHeaders();
            fillDays();
            FilterAppointments();
        }


        public void ScrollToHome()
        {
            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollToHome();
            }
        }

        public void ScrollToOffset(double offset)
        {
            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollToHorizontalOffset(offset);
            }
        }

        #region NextDay/PreviousDay

        public static readonly RoutedCommand NextDay = new RoutedCommand("NextDay", typeof(Calendar));
        public static readonly RoutedCommand PreviousDay = new RoutedCommand("PreviousDay", typeof(Calendar));
        public event CancelEventHandler CurrentDayChanging;

        void onCurrentDayChanging(CancelEventArgs e)
        {
            if(CurrentDayChanging!=null)
            {
                CurrentDayChanging(this, e);
            }
        }
        private static void OnCanExecuteNextDay(object sender, CanExecuteRoutedEventArgs e)
        {
            ((Calendar)sender).OnCanExecuteNextDay(e);
        }

        private static void OnExecutedNextDay(object sender, ExecutedRoutedEventArgs e)
        {
            ((Calendar)sender).OnExecutedNextDay(e);
        }

        protected virtual void OnCanExecuteNextDay(CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = false;
        }

        protected virtual void OnExecutedNextDay(ExecutedRoutedEventArgs e)
        {
            CancelEventArgs arg = new CancelEventArgs(false);
            onCurrentDayChanging(arg);
            if(arg.Cancel)
            {
                return;
            }
            int interval = ShowSingleDay ? 1 : 7;
            CurrentDate += TimeSpan.FromDays(interval);
            e.Handled = true;            
        }

        private static void OnCanExecutePreviousDay(object sender, CanExecuteRoutedEventArgs e)
        {
            ((Calendar)sender).OnCanExecutePreviousDay(e);
        }

        private static void OnExecutedPreviousDay(object sender, ExecutedRoutedEventArgs e)
        {
            ((Calendar)sender).OnExecutedPreviousDay(e);
        }

        protected virtual void OnCanExecutePreviousDay(CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = false;
        }

        protected virtual void OnExecutedPreviousDay(ExecutedRoutedEventArgs e)
        {
            CancelEventArgs arg = new CancelEventArgs(false);
            onCurrentDayChanging(arg);
            if (arg.Cancel)
            {
                return;
            }
            int interval = ShowSingleDay ? 1 : 7;
            CurrentDate -= TimeSpan.FromDays(interval);
            e.Handled = true;
        }

        #endregion
    }
}
