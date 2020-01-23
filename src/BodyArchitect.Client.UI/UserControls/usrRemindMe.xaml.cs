using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using BodyArchitect.Client.UI.Converters;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.UI.UserControls
{
    /// <summary>
    /// Interaction logic for usrRemindMe.xaml
    /// </summary>
    public partial class usrRemindMe
    {
        public usrRemindMe()
        {
            InitializeComponent();
        }


        public IRemindable Entry
        {
            get { return (IRemindable)GetValue(EntryProperty); }
            set
            {
                SetValue(EntryProperty, value);
            }
        }


        public static readonly DependencyProperty EntryProperty =
            DependencyProperty.Register("Entry", typeof(IRemindable), typeof(usrRemindMe), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, OnEntryChanged));

        private static void OnEntryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (usrRemindMe)d;

            ctrl.update();
        }

        private void update()
        {
            NotifyOfPropertyChange(() => HasReminder);
        }

        public bool HasReminder
        {
            get
            {
                if (Entry != null)
                {
                    return Entry.RemindBefore != null;
                }
                return false;
            }
            set
            {
                if (!value)
                {
                    Entry.RemindBefore = null;
                }
                else
                {
                    Entry.RemindBefore = TimeSpan.Zero;
                }
                NotifyOfPropertyChange(() => HasReminder);
                NotifyOfPropertyChange(() => Entry);
            }
        }
    }
}
