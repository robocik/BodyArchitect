using BodyArchitect.Controls.ProgressIndicator;

namespace BodyArchitect.Controls.Forms
{
    partial class ProfileEditWindow
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProfileEditWindow));
            DevExpress.Utils.SuperToolTip superToolTip1 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipTitleItem toolTipTitleItem1 = new DevExpress.Utils.ToolTipTitleItem();
            DevExpress.Utils.ToolTipItem toolTipItem1 = new DevExpress.Utils.ToolTipItem();
            this.usrProfileEdit1 = new BodyArchitect.Controls.UserControls.usrProfileEdit();
            this.usrProgressIndicatorButtons1 = new BodyArchitect.Controls.ProgressIndicator.usrProgressIndicatorButtons();
            this.btnDeleteProfile = new DevExpress.XtraEditors.SimpleButton();
            this.SuspendLayout();
            // 
            // usrProfileEdit1
            // 
            resources.ApplyResources(this.usrProfileEdit1, "usrProfileEdit1");
            this.usrProfileEdit1.Name = "usrProfileEdit1";
            // 
            // usrProgressIndicatorButtons1
            // 
            resources.ApplyResources(this.usrProgressIndicatorButtons1, "usrProgressIndicatorButtons1");
            this.usrProgressIndicatorButtons1.CausesValidation = false;
            this.usrProgressIndicatorButtons1.Name = "usrProgressIndicatorButtons1";
            this.usrProgressIndicatorButtons1.OkClick += new System.EventHandler<BodyArchitect.Controls.ProgressIndicator.CancellationSourceEventArgs>(this.okButton1_Click);
            // 
            // btnDeleteProfile
            // 
            resources.ApplyResources(this.btnDeleteProfile, "btnDeleteProfile");
            this.btnDeleteProfile.Image = ((System.Drawing.Image)(resources.GetObject("btnDeleteProfile.Image")));
            this.btnDeleteProfile.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnDeleteProfile.Name = "btnDeleteProfile";
            superToolTip1.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
            resources.ApplyResources(toolTipTitleItem1, "toolTipTitleItem1");
            toolTipItem1.LeftIndent = 6;
            resources.ApplyResources(toolTipItem1, "toolTipItem1");
            superToolTip1.Items.Add(toolTipTitleItem1);
            superToolTip1.Items.Add(toolTipItem1);
            this.btnDeleteProfile.SuperTip = superToolTip1;
            this.btnDeleteProfile.Click += new System.EventHandler(this.btnDeleteProfile_Click);
            // 
            // ProfileEditWindow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnDeleteProfile);
            this.Controls.Add(this.usrProgressIndicatorButtons1);
            this.Controls.Add(this.usrProfileEdit1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProfileEditWindow";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.ProfileEditWindow_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private UserControls.usrProfileEdit usrProfileEdit1;
        private ProgressIndicator.usrProgressIndicatorButtons usrProgressIndicatorButtons1;
        private DevExpress.XtraEditors.SimpleButton btnDeleteProfile;

    }
}