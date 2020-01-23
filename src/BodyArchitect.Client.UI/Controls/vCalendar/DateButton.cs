#region Directives

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Controls.Calendar;

#endregion

namespace vhCalendar
{
    public class MyDateButton:DateButton
    {
        
    }
    public class DateButton : Button
    {
        #region Constructor
        static DateButton()
        {
            PropertyMetadata isSelectedMetaData = new PropertyMetadata
            {
                DefaultValue = false,
            };
            IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(DateButton), isSelectedMetaData);

            PropertyMetadata isTodaysDateMetaData = new PropertyMetadata
            {
                DefaultValue = false,
            };
            IsTodaysDateProperty = DependencyProperty.Register("IsTodaysDate", typeof(bool), typeof(DateButton), isTodaysDateMetaData);

            PropertyMetadata isBlackOutMetadata = new PropertyMetadata
            {
                DefaultValue = false,
            };
            IsBlackOutProperty = DependencyProperty.Register("IsBlackOut", typeof(bool), typeof(DateButton), isBlackOutMetadata);

            PropertyMetadata isCurrentMonthMetaData = new PropertyMetadata
            {
                DefaultValue = true,
            };
            IsCurrentMonthProperty = DependencyProperty.Register("IsCurrentMonth", typeof(bool), typeof(DateButton), isCurrentMonthMetaData);
        }
        #endregion

        #region DisplayDateStart
        /// <summary>
        /// Gets/Sets the minimum date that is displayed and selected
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty;

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        #endregion

        #region IsBlackOut
        /// <summary>
        /// Gets/Sets button is blacked out
        /// </summary>
        public static readonly DependencyProperty IsBlackOutProperty;

        public bool IsBlackOut
        {
            get { return (bool)GetValue(IsBlackOutProperty); }
            set { SetValue(IsBlackOutProperty, value); }
        }
        #endregion

        #region IsCurrentMonth
        /// <summary>
        /// Gets/Sets button is a member of current month
        /// </summary>
        public static readonly DependencyProperty IsCurrentMonthProperty;

        public bool IsCurrentMonth
        {
            get { return (bool)GetValue(IsCurrentMonthProperty); }
            set { SetValue(IsCurrentMonthProperty, value); }
        }
        #endregion

        #region IsTodaysDate
        /// <summary>
        /// Gets/Sets todays date selection
        /// </summary>
        public static readonly DependencyProperty IsTodaysDateProperty;

        public bool IsTodaysDate
        {
            get { return (bool)GetValue(IsTodaysDateProperty); }
            set { SetValue(IsTodaysDateProperty, value); }
        }
        #endregion

        #region DayDoubleClick

        protected override void OnMouseDoubleClick(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            //prevent double click on the listbox showing entry objects. DayDobuleClick should occured only when user double click
            //on the content part of button
            DependencyObject src = (DependencyObject)(e.OriginalSource);
            var scrollBar=src.FindVisualParent<ScrollViewer>();
            if (scrollBar == null && Tag is DateTime)
            {
                RaiseDayDoubleClickEvent();
            }
        }

        private void RaiseDayDoubleClickEvent()
        {
            DayDoubleClickEventArgs e = new DayDoubleClickEventArgs();
            e.RoutedEvent = DayDoubleClickEvent;
            e.Source = this;
            e.NewDate = (DateTime)Tag;
            OnDayDoubleClick(e);
        }

        public static readonly RoutedEvent DayDoubleClickEvent =
            EventManager.RegisterRoutedEvent("DayDoubleClick", RoutingStrategy.Bubble,
            typeof(DayDoubleClickEventArgs), typeof(DateButton));

        public event RoutedEventHandler DayDoubleClick
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

        protected virtual void OnDayDoubleClick(DayDoubleClickEventArgs e)
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
            typeof(ExDragEventArgs), typeof(DateButton));

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

        #region ExDragOver

        protected override void OnDragOver(DragEventArgs e)
        {
            RaiseExDragOverEvent(e);
            base.OnDragOver(e);
        }

        private void RaiseExDragOverEvent(DragEventArgs dropArg)
        {
            ExDragEventArgs e = new ExDragEventArgs(dropArg, this);
            e.RoutedEvent = ExDragOverEvent;
            e.Source = this;
            OnExDragOver(e);
        }

        

        public static readonly RoutedEvent ExDragOverEvent =
            EventManager.RegisterRoutedEvent("ExDragOver", RoutingStrategy.Bubble,
            typeof(ExDragEventArgs), typeof(DateButton));

        public event RoutedEventHandler ExDragOver
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

        protected virtual void OnExDragOver(ExDragEventArgs e)
        {
            RaiseEvent(e);
        }

        #endregion
    }
}
