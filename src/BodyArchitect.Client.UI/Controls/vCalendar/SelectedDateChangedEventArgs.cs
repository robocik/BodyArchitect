using System;
using System.Windows;

namespace vhCalendar
{
    public class DayDoubleClickEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Constructor for the event args
        /// </summary>
        public DayDoubleClickEventArgs() {}
        /// <summary>
        /// Gets/Sets the new date that was set
        /// </summary>
        public DateTime NewDate { get; set; }
        /// <summary>
        /// Gets/Sets the old date that was set
        /// </summary>
        public DateTime OldDate { get; set; }
    }

    /// <summary>
    /// Delegate for the SelectedDateChanged event
    /// </summary>
    /// <param name="sender">The object that raised the event</param>
    /// <param name="e">Event arguments for the SelectedDateChanged event</param>
    public delegate void DayDoubleClickEventHandler(object sender, DayDoubleClickEventArgs e);

    /// <summary>
    /// Routed event args for SelectedDateChanged
    /// </summary>
    public class SelectedDateChangedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Constructor for the event args
        /// </summary>
        /// <param name="routedEvent">The event for which the args will be passed</param>
        public SelectedDateChangedEventArgs(RoutedEvent routedEvent) : base(routedEvent) { }
        /// <summary>
        /// Gets/Sets the new date that was set
        /// </summary>
        public DateTime NewDate { get; set; }
        /// <summary>
        /// Gets/Sets the old date that was set
        /// </summary>
        public DateTime OldDate { get; set; }
    }

    /// <summary>
    /// Delegate for the SelectedDateChanged event
    /// </summary>
    /// <param name="sender">The object that raised the event</param>
    /// <param name="e">Event arguments for the SelectedDateChanged event</param>
    public delegate void SelectedDateChangedEventHandler(object sender, SelectedDateChangedEventArgs e);


    public class DisplayDateChangedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Constructor for the event args
        /// </summary>
        /// <param name="routedEvent">The event for which the args will be passed</param>
        public DisplayDateChangedEventArgs(RoutedEvent routedEvent) : base(routedEvent) { }
        /// <summary>
        /// Gets/Sets the new date that was set
        /// </summary>
        public DateTime NewDate { get; set; }
        /// <summary>
        /// Gets/Sets the old date that was set
        /// </summary>
        public DateTime OldDate { get; set; }
    }

    /// <summary>
    /// Delegate for the SelectedDateChanged event
    /// </summary>
    /// <param name="sender">The object that raised the event</param>
    /// <param name="e">Event arguments for the SelectedDateChanged event</param>
    public delegate void DisplayDateChangedEventHandler(object sender, DisplayDateChangedEventArgs e);
}
