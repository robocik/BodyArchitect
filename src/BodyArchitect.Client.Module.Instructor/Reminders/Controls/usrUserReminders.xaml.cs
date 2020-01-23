using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.Instructor.Reminders.Controls
{
    [Export(typeof(IUserDetailControlBuilder))]
    public class usrUserRemindersBuilder : IUserDetailControlBuilder
    {
        public IUserDetailControl Create()
        {
            return new usrUserReminders();
        }
    }

    public partial class usrUserReminders : IUserDetailControl
    {
        private usrUserRemindersViewModel viewModel;

        public usrUserReminders()
        {
            InitializeComponent();
            viewModel = new usrUserRemindersViewModel();
        }

        public void Fill(ProfileInformationDTO user, bool isActive)
        {
            viewModel.Fill(ParentWindow, user, isActive);
            Binding binding = new Binding("Caption");
            binding.Mode = BindingMode.OneWay;
            SetBinding(CaptionProperty, binding);
            DataContext = viewModel;
        }

        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set
            {
                SetValue(CaptionProperty, value);
            }
        }

        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register("Caption", typeof(string), typeof(usrUserReminders), new UIPropertyMetadata("usrUserReminders_Caption_Notifications".TranslateInstructor()));

        public ImageSource SmallImage
        {
            get
            {
                return "pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Images/Notification.png".ToBitmap();
            }
        }

        public bool UpdateGui(ProfileInformationDTO user)
        {
            return user != null && user.User.IsMe() ;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button) sender;
            var notifyObject =(NotifyObject) btn.Tag;
            notifyObject.DeleteEvent(notifyObject);
        }

        private void lstNotifications_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var item = (NotifyObject)lstNotifications.GetClickedItem(e);
            if (item != null && item.ClickEvent!=null)
            {
                item.ClickEvent.Invoke(item);
            }
        }
    }
}
