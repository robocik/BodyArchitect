using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Service.Model;
using DevExpress.XtraEditors;

namespace BodyArchitect.Controls.UserControls
{
    public partial class usrProfileNotifications : DevExpress.XtraEditors.XtraUserControl
    {
        public usrProfileNotifications()
        {
            InitializeComponent();
        }

        public void Fill(ProfileDTO profile)
        {
            chkNotifyPlanVoted.Checked = profile.Settings.NotificationWorkoutPlanVoted;
            chkNotifyFriendChangedCalendar.Checked = profile.Settings.NotificationFriendChangedCalendar;
            chkNotfiyExerciseVoted.Checked = profile.Settings.NotificationExerciseVoted;
            chkNotifyBlogComment.Checked = profile.Settings.NotificationBlogCommentAdded;
        }

        public void Save(ProfileDTO profile)
        {
            profile.Settings.NotificationWorkoutPlanVoted = chkNotifyPlanVoted.Checked;
            profile.Settings.NotificationFriendChangedCalendar = chkNotifyFriendChangedCalendar.Checked;
            profile.Settings.NotificationExerciseVoted = chkNotfiyExerciseVoted.Checked;
            profile.Settings.NotificationBlogCommentAdded = chkNotifyBlogComment.Checked;
        }
    }
}
