namespace BodyArchitect.Controls.UserControls
{
    partial class usrUserInfo
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrUserInfo));
            this.progressIndicator1 = new BodyArchitect.Controls.UserControls.ProgressIndicator();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.grAbout = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.txtAbout = new DevExpress.XtraEditors.MemoEdit();
            this.usrProfileListEntry1 = new BodyArchitect.Controls.UserControls.usrProfileListEntry();
            this.grInfo = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.lblLastLoggedTime = new DevExpress.XtraEditors.LabelControl();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblInvitationsCountStatus = new DevExpress.XtraEditors.LabelControl();
            this.lblMessagesCountStatus = new DevExpress.XtraEditors.LabelControl();
            this.lblProfileConfigurationWizard = new DevExpress.XtraEditors.LabelControl();
            this.lblNoStatus = new DevExpress.XtraEditors.LabelControl();
            this.lblProfileNotActivated = new DevExpress.XtraEditors.LabelControl();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.grAwards = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.picRedStar = new System.Windows.Forms.PictureBox();
            this.picBlueStar = new System.Windows.Forms.PictureBox();
            this.picGreenStar = new System.Windows.Forms.PictureBox();
            this.grStatistics = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.lvStatistics = new System.Windows.Forms.ListView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.toolTipController1 = new DevExpress.Utils.ToolTipController(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grAbout)).BeginInit();
            this.grAbout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtAbout.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grInfo)).BeginInit();
            this.grInfo.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grAwards)).BeginInit();
            this.grAwards.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picRedStar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBlueStar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picGreenStar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grStatistics)).BeginInit();
            this.grStatistics.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressIndicator1
            // 
            resources.ApplyResources(this.progressIndicator1, "progressIndicator1");
            this.progressIndicator1.Name = "progressIndicator1";
            this.progressIndicator1.Percentage = 0F;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.progressIndicator1, 1, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.usrProfileListEntry1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.grInfo, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel4, 0, 2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.grAbout);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // grAbout
            // 
            this.grAbout.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("grAbout.AppearanceCaption.BackColor")));
            this.grAbout.AppearanceCaption.Options.UseBackColor = true;
            this.grAbout.AppearanceCaption.Options.UseTextOptions = true;
            this.grAbout.AppearanceCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
            this.grAbout.Controls.Add(this.txtAbout);
            resources.ApplyResources(this.grAbout, "grAbout");
            this.grAbout.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.grAbout.LookAndFeel.UseDefaultLookAndFeel = false;
            this.grAbout.Name = "grAbout";
            // 
            // txtAbout
            // 
            resources.ApplyResources(this.txtAbout, "txtAbout");
            this.txtAbout.Name = "txtAbout";
            this.txtAbout.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("txtAbout.Properties.Appearance.BackColor")));
            this.txtAbout.Properties.Appearance.Options.UseBackColor = true;
            this.txtAbout.Properties.NullText = resources.GetString("txtAbout.Properties.NullText");
            this.txtAbout.Properties.ReadOnly = true;
            // 
            // usrProfileListEntry1
            // 
            this.usrProfileListEntry1.AllowRedirectToDetails = false;
            resources.ApplyResources(this.usrProfileListEntry1, "usrProfileListEntry1");
            this.usrProfileListEntry1.Name = "usrProfileListEntry1";
            // 
            // grInfo
            // 
            this.grInfo.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("grInfo.AppearanceCaption.BackColor")));
            this.grInfo.AppearanceCaption.Options.UseBackColor = true;
            this.grInfo.AppearanceCaption.Options.UseTextOptions = true;
            this.grInfo.AppearanceCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
            this.grInfo.Controls.Add(this.tableLayoutPanel3);
            resources.ApplyResources(this.grInfo, "grInfo");
            this.grInfo.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.grInfo.LookAndFeel.UseDefaultLookAndFeel = false;
            this.grInfo.Name = "grInfo";
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.lblLastLoggedTime, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // lblLastLoggedTime
            // 
            resources.ApplyResources(this.lblLastLoggedTime, "lblLastLoggedTime");
            this.lblLastLoggedTime.Name = "lblLastLoggedTime";
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Controls.Add(this.lblInvitationsCountStatus);
            this.flowLayoutPanel1.Controls.Add(this.lblMessagesCountStatus);
            this.flowLayoutPanel1.Controls.Add(this.lblProfileConfigurationWizard);
            this.flowLayoutPanel1.Controls.Add(this.lblNoStatus);
            this.flowLayoutPanel1.Controls.Add(this.lblProfileNotActivated);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // lblInvitationsCountStatus
            // 
            this.lblInvitationsCountStatus.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lblInvitationsCountStatus.Appearance.Font")));
            this.lblInvitationsCountStatus.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("lblInvitationsCountStatus.Appearance.ForeColor")));
            this.lblInvitationsCountStatus.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.lblInvitationsCountStatus, "lblInvitationsCountStatus");
            this.lblInvitationsCountStatus.Name = "lblInvitationsCountStatus";
            this.lblInvitationsCountStatus.Click += new System.EventHandler(this.lblInvitationsCountStatus_Click);
            // 
            // lblMessagesCountStatus
            // 
            this.lblMessagesCountStatus.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lblMessagesCountStatus.Appearance.Font")));
            this.lblMessagesCountStatus.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("lblMessagesCountStatus.Appearance.ForeColor")));
            this.lblMessagesCountStatus.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.lblMessagesCountStatus, "lblMessagesCountStatus");
            this.lblMessagesCountStatus.Name = "lblMessagesCountStatus";
            this.lblMessagesCountStatus.Click += new System.EventHandler(this.lblMessagesCountStatus_Click);
            // 
            // lblProfileConfigurationWizard
            // 
            this.lblProfileConfigurationWizard.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lblProfileConfigurationWizard.Appearance.Font")));
            this.lblProfileConfigurationWizard.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("lblProfileConfigurationWizard.Appearance.ForeColor")));
            this.lblProfileConfigurationWizard.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.lblProfileConfigurationWizard, "lblProfileConfigurationWizard");
            this.lblProfileConfigurationWizard.Name = "lblProfileConfigurationWizard";
            this.lblProfileConfigurationWizard.Click += new System.EventHandler(this.lblProfileConfigurationWizard_Click);
            // 
            // lblNoStatus
            // 
            this.lblNoStatus.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lblNoStatus.Appearance.Font")));
            this.lblNoStatus.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("lblNoStatus.Appearance.ForeColor")));
            this.lblNoStatus.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.lblNoStatus, "lblNoStatus");
            this.lblNoStatus.Name = "lblNoStatus";
            // 
            // lblProfileNotActivated
            // 
            this.lblProfileNotActivated.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lblProfileNotActivated.Appearance.Font")));
            this.lblProfileNotActivated.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("lblProfileNotActivated.Appearance.ForeColor")));
            this.lblProfileNotActivated.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.lblProfileNotActivated, "lblProfileNotActivated");
            this.lblProfileNotActivated.Name = "lblProfileNotActivated";
            // 
            // tableLayoutPanel4
            // 
            resources.ApplyResources(this.tableLayoutPanel4, "tableLayoutPanel4");
            this.tableLayoutPanel4.Controls.Add(this.grAwards, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.grStatistics, 0, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            // 
            // grAwards
            // 
            this.grAwards.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("grAwards.AppearanceCaption.BackColor")));
            this.grAwards.AppearanceCaption.Options.UseBackColor = true;
            this.grAwards.AppearanceCaption.Options.UseTextOptions = true;
            this.grAwards.AppearanceCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
            this.grAwards.Controls.Add(this.flowLayoutPanel3);
            resources.ApplyResources(this.grAwards, "grAwards");
            this.grAwards.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.grAwards.LookAndFeel.UseDefaultLookAndFeel = false;
            this.grAwards.Name = "grAwards";
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.picRedStar);
            this.flowLayoutPanel3.Controls.Add(this.picBlueStar);
            this.flowLayoutPanel3.Controls.Add(this.picGreenStar);
            resources.ApplyResources(this.flowLayoutPanel3, "flowLayoutPanel3");
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            // 
            // picRedStar
            // 
            resources.ApplyResources(this.picRedStar, "picRedStar");
            this.picRedStar.Name = "picRedStar";
            this.picRedStar.TabStop = false;
            // 
            // picBlueStar
            // 
            resources.ApplyResources(this.picBlueStar, "picBlueStar");
            this.picBlueStar.Name = "picBlueStar";
            this.picBlueStar.TabStop = false;
            // 
            // picGreenStar
            // 
            resources.ApplyResources(this.picGreenStar, "picGreenStar");
            this.picGreenStar.Name = "picGreenStar";
            this.picGreenStar.TabStop = false;
            // 
            // grStatistics
            // 
            this.grStatistics.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("grStatistics.AppearanceCaption.BackColor")));
            this.grStatistics.AppearanceCaption.Options.UseBackColor = true;
            this.grStatistics.AppearanceCaption.Options.UseTextOptions = true;
            this.grStatistics.AppearanceCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
            this.grStatistics.Controls.Add(this.lvStatistics);
            resources.ApplyResources(this.grStatistics, "grStatistics");
            this.grStatistics.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.grStatistics.LookAndFeel.UseDefaultLookAndFeel = false;
            this.grStatistics.Name = "grStatistics";
            // 
            // lvStatistics
            // 
            this.lvStatistics.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.lvStatistics, "lvStatistics");
            this.lvStatistics.MultiSelect = false;
            this.lvStatistics.Name = "lvStatistics";
            this.lvStatistics.ShowItemToolTips = true;
            this.lvStatistics.SmallImageList = this.imageList1;
            this.lvStatistics.UseCompatibleStateImageBehavior = false;
            this.lvStatistics.View = System.Windows.Forms.View.List;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Rank6");
            this.imageList1.Images.SetKeyName(1, "Rank5");
            this.imageList1.Images.SetKeyName(2, "Rank4");
            this.imageList1.Images.SetKeyName(3, "Rank3");
            this.imageList1.Images.SetKeyName(4, "Rank2");
            this.imageList1.Images.SetKeyName(5, "Rank1");
            // 
            // usrUserInfo
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "usrUserInfo";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grAbout)).EndInit();
            this.grAbout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtAbout.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grInfo)).EndInit();
            this.grInfo.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grAwards)).EndInit();
            this.grAwards.ResumeLayout(false);
            this.flowLayoutPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picRedStar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBlueStar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picGreenStar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grStatistics)).EndInit();
            this.grStatistics.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ProgressIndicator progressIndicator1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel1;
        private BaGroupControl grInfo;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private DevExpress.XtraEditors.LabelControl lblLastLoggedTime;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private DevExpress.XtraEditors.LabelControl lblInvitationsCountStatus;
        private DevExpress.XtraEditors.LabelControl lblMessagesCountStatus;
        private DevExpress.XtraEditors.LabelControl lblProfileConfigurationWizard;
        private DevExpress.XtraEditors.LabelControl lblNoStatus;
        private DevExpress.XtraEditors.LabelControl lblProfileNotActivated;
        private usrProfileListEntry usrProfileListEntry1;
        private BaGroupControl grAbout;
        private DevExpress.XtraEditors.MemoEdit txtAbout;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private BaGroupControl grAwards;
        private BaGroupControl grStatistics;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.PictureBox picRedStar;
        private System.Windows.Forms.PictureBox picBlueStar;
        private System.Windows.Forms.PictureBox picGreenStar;
        private DevExpress.Utils.ToolTipController toolTipController1;
        private System.Windows.Forms.ListView lvStatistics;
        private System.Windows.Forms.ImageList imageList1;

    }
}
