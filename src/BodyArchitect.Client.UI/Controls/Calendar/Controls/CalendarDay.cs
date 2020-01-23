using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BodyArchitect.Client.UI.Controls.Calendar.Common;

//using Microsoft.Windows.Design;

namespace BodyArchitect.Client.UI.Controls.Calendar
{
    [TemplatePart(Name = CalendarDay.ElementTimeslotItems, Type = typeof(StackPanel))]
    //[ToolboxBrowsable(false)]
    public sealed class CalendarDay : ItemsControl
    {
        private const string ElementTimeslotItems = "PART_TimeslotItems";

        StackPanel _dayItems;


        static CalendarDay()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarDay), new FrameworkPropertyMetadata(typeof(CalendarDay)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _dayItems = GetTemplateChild(ElementTimeslotItems) as StackPanel;

            PopulateDay();
        }

        private DateTime _currentDate;
        public DateTime CurrentDate
        {
            get { return _currentDate; }
            set
            {
                if (_currentDate.Date!=value.Date)
                {
                    _currentDate = value;
                    PopulateDay();
                }
            }
        }

        public IList<CalendarTimeslotItem> TimeSlots
        {
            get { return _dayItems.Children.Cast<CalendarTimeslotItem>().ToList(); }
        }

        public void SetTimeslotBackground(CalendarTimeslotItem timeslot,int startHour)
        {
            if (!Owner.ShowOffPeekHours || (startHour >= Owner.PeekStartHour && startHour <= Owner.PeekEndHour))
                timeslot.SetBinding(Calendar.BackgroundProperty, GetOwnerBinding("PeakTimeslotBackground"));
            else
                timeslot.SetBinding(Calendar.BackgroundProperty, GetOwnerBinding("OffPeakTimeslotBackground"));
        }
        public void PopulateDay()
        {
            if (_dayItems != null)
            {
                _dayItems.Children.Clear();

                DateTime startTime = new DateTime(CurrentDate.Year, CurrentDate.Month, CurrentDate.Day, 0, 0, 0);
                for (int i = 0; i < 48; i++)
                {                   
                    CalendarTimeslotItem timeslot = new CalendarTimeslotItem(Owner);
                    timeslot.StartTime = startTime;
                    timeslot.EndTime = startTime + TimeSpan.FromMinutes(30);

                    SetTimeslotBackground(timeslot,startTime.Hour);

                    timeslot.SetBinding(CalendarTimeslotItem.StyleProperty, GetOwnerBinding("CalendarTimeslotItemStyle"));
                    _dayItems.Children.Add(timeslot);

                    startTime = startTime + TimeSpan.FromMinutes(30);
                }
            }
            if (Owner != null)
            {
                Owner.ScrollToHome();
            }
        }

        #region ItemsControl Container Override

        protected override DependencyObject GetContainerForItemOverride()
        {            
            return new CalendarAppointmentItem(Owner);
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is CalendarAppointmentItem);
        }

        #endregion

        public ICalendarControl Owner
        {
            get { return UIHelper.FindVisualParent<ICalendarControl>(this); }
        }

        private BindingBase GetOwnerBinding(string propertyName)
        {
            Binding result = new Binding(propertyName);
            result.Source = this.Owner;
            return result;
        }
    }
}
