using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using BodyArchitect.Client.UI.Controls.Calendar.Common;
using BodyArchitect.Client.UI.Controls.Calendar.Controls;
using BodyArchitect.Client.UI.Converters;

//using Microsoft.Windows.Design;

namespace BodyArchitect.Client.UI.Controls.Calendar
{
    

    [TemplateVisualState(Name = CalendarAppointmentItem.StateNormal, GroupName = CalendarAppointmentItem.GroupCommon)]
    [TemplateVisualState(Name = CalendarAppointmentItem.StateMouseOver, GroupName = CalendarAppointmentItem.GroupCommon)]
    [TemplateVisualState(Name = CalendarAppointmentItem.StateDisabled, GroupName = CalendarAppointmentItem.GroupCommon)]
    [TemplateVisualState(Name = CalendarAppointmentItem.StateSelected, GroupName = CalendarAppointmentItem.GroupCommon)]
    //[ToolboxBrowsable(false)]
    public class CalendarAppointmentItem : ContentControl
    {
        public const string StateNormal = "Normal";
        public const string StateMouseOver = "MouseOver";
        public const string StateDisabled = "Disabled";
        public const string StateSelected = "Selected";

        public const string GroupCommon = "CommonStates";
        DragDropHelper dragDrop = new DragDropHelper();
        private ICalendarControl owner;

        public CalendarAppointmentItem(ICalendarControl owner)
        {
            this.owner = owner;
            dragDrop.SetDragSource(this,(FrameworkElement)owner,delegate
                                                  {
                                                      StartDragDrop();
                                                  });
            AllowDrop = true;
            this.MouseEnter +=
            new MouseEventHandler(SimpleButton_MouseEnter);
            this.MouseLeave +=
                new MouseEventHandler(SimpleButton_MouseLeave);
        }

        public void SetSelection(bool isSelected)
        {
            //var vsgs = VisualStateManager.GetVisualStateGroups(VisualTreeHelper.GetChild(this, 0) as FrameworkElement);
            //var vsg = vsgs[0];
            //// this is correctly reported as "Unselected"
            //var currentState = ((VisualStateGroup)vsg).CurrentState.Name;
            //if (currentState == "romek")
            //{
            //    Name = currentState;
            //}
            if (isSelected)
            {
                VisualStateManager.GoToState(this, StateSelected, true);
            }
            else
            {
                VisualStateManager.GoToState(this, StateNormal, true);
            }
        }


        static CalendarAppointmentItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarAppointmentItem), new FrameworkPropertyMetadata(typeof(CalendarAppointmentItem)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //CloseButton
            var closeButton = GetTemplateChild("CloseButton") as Button;
            var decrementButton = GetTemplateChild("DecrementButton") as Button;
            var incrementButton = GetTemplateChild("IncrementButton") as Button;
            var panelButtons = GetTemplateChild("panelButtons") as StackPanel;
            var lblPersons = GetTemplateChild("lblPersons") as TextBlock;

            
            if (closeButton != null)
            {
                closeButton.Click += new RoutedEventHandler(closeButton_Click);
                decrementButton.Click += new RoutedEventHandler(decrementButton_Click);
                incrementButton.Click += new RoutedEventHandler(incrementButton_Click);

                MultiBinding multiBinding = new MultiBinding();
                multiBinding.Converter = new BoolToVisibilityMultiBinding();
                Binding result = new Binding("ReadOnly");
                result.Source = this.Owner;
                multiBinding.Bindings.Add(result);
                result = new Binding("ReadOnly");
                multiBinding.Bindings.Add(result);
                //result.Source = this.Owner;
                //result.Converter = new NegateBoolToVisibilityConverter();
                panelButtons.SetBinding(Calendar.VisibilityProperty, multiBinding);
                //multiBinding = new MultiBinding();
                //multiBinding.Converter = new ReadOnlyToVisibilityMultiBinding();
                //result = new Binding("ReadOnly");
                //result.Source = this.Owner;
                //multiBinding.Bindings.Add(result);
                result = new Binding("ReadOnly");
                //multiBinding.Bindings.Add(result);
                //result.Source = this.Owner;
                result.Converter = new BooleanToVisibilityConverter();
                lblPersons.SetBinding(Calendar.VisibilityProperty, result);
                
            }
            if (owner.SelectedAppointment == this.Content)
            {
                SetSelection(true);
            }
        }

        #region ChangeAppointmentTime

        private void RaiseChangeAppointmentTimeEvent(TimeSpan changeTime)
        {
            ChangeAppointmentTimeEventArgs e = new ChangeAppointmentTimeEventArgs(changeTime);
            e.RoutedEvent = ChangeAppointmentTimeEvent;
            e.Source = this;

            OnChangeAppointmentTime(e);
        }

        public static readonly RoutedEvent ChangeAppointmentTimeEvent =
            EventManager.RegisterRoutedEvent("ChangeAppointmentTime", RoutingStrategy.Bubble,
            typeof(ChangeAppointmentTimeEventArgs), typeof(CalendarAppointmentItem));

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

        protected virtual void OnChangeAppointmentTime(RoutedEventArgs e)
        {
            RaiseEvent(e);
        }

        #endregion

        void incrementButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseChangeAppointmentTimeEvent(TimeSpan.FromMinutes(30));
        }

        void decrementButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseChangeAppointmentTimeEvent(TimeSpan.FromMinutes(-30));
        }

        void closeButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseDeleteAppointmentEvent();
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            if (!(newContent is IAppointment))
            {
                owner = null;
            }
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (owner != null)
            {
                owner.SelectedAppointment = (IAppointment)this.Content;
            }
            base.OnMouseLeftButtonDown(e);
            
        }

        void SimpleButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (owner.SelectedAppointment != this.Content)
            {
                VisualStateManager.GoToState(this, StateMouseOver, true);
            }
           
        }

        void SimpleButton_MouseLeave(object sender, MouseEventArgs e)
        {
            if (owner!=null && owner.SelectedAppointment != this.Content)
            {
                VisualStateManager.GoToState(this, StateNormal, true);
            }
        }
        
        #region StartTime/EndTime

        public static readonly DependencyProperty StartTimeProperty =
            TimeslotPanel.StartTimeProperty.AddOwner(typeof(CalendarAppointmentItem));

        public bool StartTime
        {
            get { return (bool)GetValue(StartTimeProperty); }
            set { SetValue(StartTimeProperty, value); }
        }

        public static readonly DependencyProperty EndTimeProperty =
            TimeslotPanel.EndTimeProperty.AddOwner(typeof(CalendarAppointmentItem));

        public bool EndTime
        {
            get { return (bool)GetValue(EndTimeProperty); }
            set { SetValue(EndTimeProperty, value); }
        }

        public ICalendarControl Owner
        {
            get { return owner; }
        }

        #endregion
        

        protected virtual void StartDragDrop()
        {
            IAppointment data = (IAppointment) this.Content;
            if (Owner.ReadOnly || data.ReadOnly)
            {
                return;
            }
            DataObject dragData = new DataObject("myFormat", this.Content);
            DragDrop.DoDragDrop(this, dragData, DragDropEffects.Move | DragDropEffects.Copy);
            
        }


        #region EditAppointment

        private void RaiseEditAppointmentEvent()
        {
            RoutedEventArgs e = new RoutedEventArgs();
            e.RoutedEvent = EditAppointmentEvent;
            e.Source = this;

            OnEditAppointment(e);
        }

        public static readonly RoutedEvent EditAppointmentEvent =
            EventManager.RegisterRoutedEvent("EditAppointment", RoutingStrategy.Bubble,
            typeof(RoutedEventArgs), typeof(CalendarAppointmentItem));

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

        protected virtual void OnEditAppointment(RoutedEventArgs e)
        {
            RaiseEvent(e);
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            
            base.OnMouseDoubleClick(e);
            var t = (FrameworkElement)e.Source;
            var button=t.FindVisualParent<Button>();
            //raise edit only when we double click on the entry itself - not on the small buttons in right top corner
            if (button==null /*!Owner.ReadOnly*/)
            {
                RaiseEditAppointmentEvent();
            }
        }
        #endregion

        #region DeleteAppointment

        private void RaiseDeleteAppointmentEvent()
        {
            RoutedEventArgs e = new RoutedEventArgs();
            e.RoutedEvent = DeleteAppointmentEvent;
            e.Source = this.Content;

            OnDeleteAppointment(e);
        }

        public static readonly RoutedEvent DeleteAppointmentEvent =
            EventManager.RegisterRoutedEvent("DeleteAppointment", RoutingStrategy.Bubble,
            typeof(RoutedEventArgs), typeof(CalendarAppointmentItem));

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

        protected virtual void OnDeleteAppointment(RoutedEventArgs e)
        {
            RaiseEvent(e);
        }

        #endregion
    }
}
