namespace BodyArchitect.Controls.UserControls
{
    partial class usrProfileNotifications
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrProfileNotifications));
            this.chkNotifyFriendChangedCalendar = new DevExpress.XtraEditors.CheckEdit();
            this.chkNotifyBlogComment = new DevExpress.XtraEditors.CheckEdit();
            this.chkNotifyPlanVoted = new DevExpress.XtraEditors.CheckEdit();
            this.chkNotfiyExerciseVoted = new DevExpress.XtraEditors.CheckEdit();
            ((System.ComponentModel.ISupportInitialize)(this.chkNotifyFriendChangedCalendar.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkNotifyBlogComment.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkNotifyPlanVoted.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkNotfiyExerciseVoted.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // chkNotifyFriendChangedCalendar
            // 
            resources.ApplyResources(this.chkNotifyFriendChangedCalendar, "chkNotifyFriendChangedCalendar");
            this.chkNotifyFriendChangedCalendar.Name = "chkNotifyFriendChangedCalendar";
            this.chkNotifyFriendChangedCalendar.Properties.AutoWidth = true;
            this.chkNotifyFriendChangedCalendar.Properties.Caption = resources.GetString("chkNotifyFriendChangedCalendar.Properties.Caption");
            // 
            // chkNotifyBlogComment
            // 
            resources.ApplyResources(this.chkNotifyBlogComment, "chkNotifyBlogComment");
            this.chkNotifyBlogComment.Name = "chkNotifyBlogComment";
            this.chkNotifyBlogComment.Properties.AutoWidth = true;
            this.chkNotifyBlogComment.Properties.Caption = resources.GetString("chkNotifyBlogComment.Properties.Caption");
            // 
            // chkNotifyPlanVoted
            // 
            resources.ApplyResources(this.chkNotifyPlanVoted, "chkNotifyPlanVoted");
            this.chkNotifyPlanVoted.Name = "chkNotifyPlanVoted";
            this.chkNotifyPlanVoted.Properties.AutoWidth = true;
            this.chkNotifyPlanVoted.Properties.Caption = resources.GetString("chkNotifyPlanVoted.Properties.Caption");
            // 
            // chkNotfiyExerciseVoted
            // 
            resources.ApplyResources(this.chkNotfiyExerciseVoted, "chkNotfiyExerciseVoted");
            this.chkNotfiyExerciseVoted.Name = "chkNotfiyExerciseVoted";
            this.chkNotfiyExerciseVoted.Properties.AutoWidth = true;
            this.chkNotfiyExerciseVoted.Properties.Caption = resources.GetString("chkNotfiyExerciseVoted.Properties.Caption");
            // 
            // usrProfileNotifications
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkNotfiyExerciseVoted);
            this.Controls.Add(this.chkNotifyPlanVoted);
            this.Controls.Add(this.chkNotifyBlogComment);
            this.Controls.Add(this.chkNotifyFriendChangedCalendar);
            this.Name = "usrProfileNotifications";
            ((System.ComponentModel.ISupportInitialize)(this.chkNotifyFriendChangedCalendar.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkNotifyBlogComment.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkNotifyPlanVoted.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkNotfiyExerciseVoted.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.CheckEdit chkNotifyFriendChangedCalendar;
        private DevExpress.XtraEditors.CheckEdit chkNotifyBlogComment;
        private DevExpress.XtraEditors.CheckEdit chkNotifyPlanVoted;
        private DevExpress.XtraEditors.CheckEdit chkNotfiyExerciseVoted;
    }
}
