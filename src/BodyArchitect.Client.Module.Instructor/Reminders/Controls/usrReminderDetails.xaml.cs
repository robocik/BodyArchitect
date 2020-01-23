using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.Instructor.Controls
{
    /// <summary>
    /// Interaction logic for usrReminderDetails.xaml
    /// </summary>
    public partial class usrReminderDetails : IEditableControl
    {
        private ReminderItemDTO reminder;

        public usrReminderDetails()
        {
            InitializeComponent();
        }

        public ReminderItemDTO ReminderItem
        {
            get { return (ReminderItemDTO)GetValue(ReminderItemProperty); }
            set
            {
                SetValue(ReminderItemProperty, value);
            }
        }


        public static readonly DependencyProperty ReminderItemProperty =
            DependencyProperty.Register("ReminderItem", typeof(ReminderItemDTO), typeof(usrReminderDetails), new UIPropertyMetadata(null, OnProductChanged));

        private static void OnProductChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (usrReminderDetails)d;
            ctrl.Object = e.NewValue;
        }

        #region Implementation of IEditableControl

        public object Object
        {
            get { return DataContext; }
            set
            {
                if (!object.ReferenceEquals(reminder ,(ReminderItemDTO)value))
                {
                    ReminderItem = (ReminderItemDTO) value;
                    reminder = (ReminderItemDTO) value;
                    DataContext = value;
                }
            }
        }

        public bool ReadOnly
        {
            get; set;
        }

        public object Save()
        {
            return ServiceManager.SaveReminder(reminder);
        }

        #endregion
    }
}
