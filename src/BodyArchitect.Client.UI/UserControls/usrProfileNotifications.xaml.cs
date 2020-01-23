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
using BodyArchitect.Client.Common;
using BodyArchitect.Service.V2.Model;
using Xceed.Wpf.Toolkit;

namespace BodyArchitect.Client.UI.UserControls
{

    public partial class usrProfileNotifications
    {
        public usrProfileNotifications()
        {
            InitializeComponent();
        }
        
        public void Fill(ProfileDTO profile)
        {

            chkNotifyFriendChangedCalendarMessage.IsChecked=((profile.Settings.NotificationFriendChangedCalendar & ProfileNotification.Message)==ProfileNotification.Message);
            chkNotifyFriendChangedCalendarEMail.IsChecked=((profile.Settings.NotificationFriendChangedCalendar & ProfileNotification.Email)==ProfileNotification.Email);

            chkNotifyFollowerChangedCalendarMessage.IsChecked = ((profile.Settings.NotificationFollowersChangedCalendar & ProfileNotification.Message) == ProfileNotification.Message);
            chkNotifyFollowerChangedCalendarEMail.IsChecked = ((profile.Settings.NotificationFollowersChangedCalendar & ProfileNotification.Email) == ProfileNotification.Email);
            
            chkNotifyPlanVotedMessage.IsChecked = ((profile.Settings.NotificationVoted & ProfileNotification.Message) == ProfileNotification.Message);
            chkNotifyPlanVotedEMail.IsChecked = ((profile.Settings.NotificationVoted & ProfileNotification.Email) == ProfileNotification.Email);

            chkNotifyTrainingDayCommentMessage.IsChecked = ((profile.Settings.NotificationBlogCommentAdded & ProfileNotification.Message) == ProfileNotification.Message);
            chkNotifyTrainingDayCommentEMail.IsChecked = ((profile.Settings.NotificationBlogCommentAdded & ProfileNotification.Email) == ProfileNotification.Email);

            chkNotifyPlanSocialMessage.IsChecked = ((profile.Settings.NotificationSocial & ProfileNotification.Message) == ProfileNotification.Message);
            chkNotifyPlanSocialEMail.IsChecked = ((profile.Settings.NotificationSocial & ProfileNotification.Email) == ProfileNotification.Email);

        }

        public void Save(ProfileDTO profile)
        {
            profile.Settings.NotificationVoted = getNotification(chkNotifyPlanVotedMessage.IsChecked.Value, chkNotifyPlanVotedEMail.IsChecked.Value);
            profile.Settings.NotificationBlogCommentAdded = getNotification(chkNotifyTrainingDayCommentMessage.IsChecked.Value, chkNotifyTrainingDayCommentEMail.IsChecked.Value);
            profile.Settings.NotificationFriendChangedCalendar = getNotification(chkNotifyFriendChangedCalendarMessage.IsChecked.Value, chkNotifyFriendChangedCalendarEMail.IsChecked.Value);
            profile.Settings.NotificationSocial = getNotification(chkNotifyPlanSocialMessage.IsChecked.Value, chkNotifyPlanSocialEMail.IsChecked.Value);
            profile.Settings.NotificationFollowersChangedCalendar = getNotification(chkNotifyFollowerChangedCalendarMessage.IsChecked.Value, chkNotifyFollowerChangedCalendarEMail.IsChecked.Value);

        }

        ProfileNotification getNotification(bool message,bool email)
        {
            ProfileNotification notification = ProfileNotification.None;
            if (message)
            {
                notification |= ProfileNotification.Message;
            }
            if (email)
            {
                notification |= ProfileNotification.Email;
            }
            return notification;
        }

    }
}
