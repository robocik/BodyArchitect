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
using System.Windows.Controls.Primitives;
using BodyArchitect.Client.UI.Converters;
using Microsoft.Windows.Controls;
//using Microsoft.Windows.Design;
using BodyArchitect.Client.UI.Controls.Calendar.Common;

namespace BodyArchitect.Client.UI.Controls.Calendar
{
    
    [TemplateVisualState(Name = CalendarTimeslotItem.StateNormal, GroupName = CalendarTimeslotItem.GroupCommon)]
    [TemplateVisualState(Name = CalendarTimeslotItem.StateMouseOver, GroupName = CalendarTimeslotItem.GroupCommon)]
    [TemplateVisualState(Name = CalendarTimeslotItem.StateDisabled, GroupName = CalendarTimeslotItem.GroupCommon)]
    //[ToolboxBrowsable(false)]
    public partial class CalendarTimeslotItem : ButtonBase
    {
        public const string StateNormal = "Normal";
        public const string StateMouseOver = "MouseOver";
        public const string StateDisabled = "Disabled";

        public const string GroupCommon = "CommonStates";
        private ICalendarControl calendar;

        public CalendarTimeslotItem(ICalendarControl calendar)
        {
            this.calendar = calendar;
            AllowDrop = true;
            Binding result = new Binding("ReadOnly");
            result.Source = calendar;
            result.Converter = new NegationConverter();
            SetBinding(IsHitTestVisibleProperty, result);
        }

        
        static CalendarTimeslotItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarTimeslotItem), new FrameworkPropertyMetadata(typeof(CalendarTimeslotItem)));
        }

        #region AddAppointment

        private void RaiseAddAppointmentEvent()
        {
            RoutedEventArgs e = new RoutedEventArgs();
            e.RoutedEvent = AddAppointmentEvent;
            e.Source = this;
            
            OnAddAppointment(e);
        }

        public static readonly RoutedEvent AddAppointmentEvent = 
            EventManager.RegisterRoutedEvent("AddAppointment", RoutingStrategy.Bubble,
            typeof(RoutedEventArgs), typeof(CalendarTimeslotItem));

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

        protected virtual void OnAddAppointment(RoutedEventArgs e)
        {
            RaiseEvent(e);
        }

        #endregion

        #region ExDrop

        protected override void OnDrop(DragEventArgs e)
        {
            RaiseExDropEvent(e);
            base.OnDrop(e);
        }

        private void RaiseExDropEvent(DragEventArgs dropArg)
        {
            ExDragEventArgs e = new ExDragEventArgs(dropArg, this);
            e.RoutedEvent = ExDropEvent;
            e.Source = this;

            OnExDrop(e);
        }

        public static readonly RoutedEvent ExDropEvent =
            EventManager.RegisterRoutedEvent("ExDrop", RoutingStrategy.Bubble,
            typeof(ExDragEventArgs), typeof(CalendarTimeslotItem));

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

        protected virtual void OnExDrop(ExDragEventArgs e)
        {
            RaiseEvent(e);
        }

        #endregion

        protected override void OnClick()
        {
            base.OnClick();
            if(calendar.ReadOnly)
            {
                if (calendar != null)
                {
                    calendar.SelectedAppointment = null;
                }    
            }
            else
            {
                RaiseAddAppointmentEvent();
            }
            
           
        }


        #region StartTime

        /// <summary>
        /// StartTime Dependency Property
        /// </summary>
        public static readonly DependencyProperty StartTimeProperty =
            DependencyProperty.Register("StartTime", typeof(DateTime), typeof(CalendarTimeslotItem),
                new FrameworkPropertyMetadata((DateTime)DateTime.Now));

        /// <summary>
        /// Gets or sets the StartTime property.  This dependency property 
        /// indicates ....
        /// </summary>
        public DateTime StartTime
        {
            get { return (DateTime)GetValue(StartTimeProperty); }
            set { SetValue(StartTimeProperty, value); }
        }

        #endregion

        #region EndTime

        /// <summary>
        /// StartTime Dependency Property
        /// </summary>
        public static readonly DependencyProperty EndTimeProperty =
            DependencyProperty.Register("EndTime", typeof(DateTime), typeof(CalendarTimeslotItem),
                new FrameworkPropertyMetadata((DateTime)DateTime.Now));

        /// <summary>
        /// Gets or sets the StartTime property.  This dependency property 
        /// indicates ....
        /// </summary>
        public DateTime EndTime
        {
            get { return (DateTime)GetValue(EndTimeProperty); }
            set { SetValue(EndTimeProperty, value); }
        }

        #endregion
    }
}
