namespace BodyArchitect.Controls.UserControls
{
    partial class usrProfileEdit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrProfileEdit));
            this.tcTabControl = new DevExpress.XtraTab.XtraTabControl();
            this.tpInfo = new DevExpress.XtraTab.XtraTabPage();
            this.usrCreateProfile1 = new BodyArchitect.Controls.UserControls.usrCreateProfile();
            this.tpPersonalInfo = new DevExpress.XtraTab.XtraTabPage();
            this.usrProfilePersonalInfo1 = new BodyArchitect.Controls.UserControls.usrProfilePersonalInfo();
            this.tpSizes = new DevExpress.XtraTab.XtraTabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.usrWymiaryEditor1 = new BodyArchitect.Controls.UserControls.usrWymiaryEditor();
            this.chkAutomaticUpdateMeasurements = new DevExpress.XtraEditors.CheckEdit();
            this.tpPrivacy = new DevExpress.XtraTab.XtraTabPage();
            this.usrProfilePrivacy1 = new BodyArchitect.Controls.UserControls.usrProfilePrivacy();
            this.tpNotifications = new DevExpress.XtraTab.XtraTabPage();
            this.usrProfileNotifications1 = new BodyArchitect.Controls.UserControls.usrProfileNotifications();
            ((System.ComponentModel.ISupportInitialize)(this.tcTabControl)).BeginInit();
            this.tcTabControl.SuspendLayout();
            this.tpInfo.SuspendLayout();
            this.tpPersonalInfo.SuspendLayout();
            this.tpSizes.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkAutomaticUpdateMeasurements.Properties)).BeginInit();
            this.tpPrivacy.SuspendLayout();
            this.tpNotifications.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcTabControl
            // 
            resources.ApplyResources(this.tcTabControl, "tcTabControl");
            this.tcTabControl.Name = "tcTabControl";
            this.tcTabControl.SelectedTabPage = this.tpInfo;
            this.tcTabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tpInfo,
            this.tpPersonalInfo,
            this.tpSizes,
            this.tpPrivacy,
            this.tpNotifications});
            this.tcTabControl.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.xtraTabControl1_SelectedPageChanged);
            // 
            // tpInfo
            // 
            this.tpInfo.Controls.Add(this.usrCreateProfile1);
            this.tpInfo.Name = "tpInfo";
            resources.ApplyResources(this.tpInfo, "tpInfo");
            // 
            // usrCreateProfile1
            // 
            resources.ApplyResources(this.usrCreateProfile1, "usrCreateProfile1");
            this.usrCreateProfile1.Name = "usrCreateProfile1";
            // 
            // tpPersonalInfo
            // 
            this.tpPersonalInfo.Controls.Add(this.usrProfilePersonalInfo1);
            this.tpPersonalInfo.Name = "tpPersonalInfo";
            resources.ApplyResources(this.tpPersonalInfo, "tpPersonalInfo");
            // 
            // usrProfilePersonalInfo1
            // 
            resources.ApplyResources(this.usrProfilePersonalInfo1, "usrProfilePersonalInfo1");
            this.usrProfilePersonalInfo1.Name = "usrProfilePersonalInfo1";
            // 
            // tpSizes
            // 
            this.tpSizes.Controls.Add(this.tableLayoutPanel1);
            this.tpSizes.Name = "tpSizes";
            resources.ApplyResources(this.tpSizes, "tpSizes");
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.usrWymiaryEditor1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.chkAutomaticUpdateMeasurements, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // usrWymiaryEditor1
            // 
            resources.ApplyResources(this.usrWymiaryEditor1, "usrWymiaryEditor1");
            this.usrWymiaryEditor1.Name = "usrWymiaryEditor1";
            // 
            // chkAutomaticUpdateMeasurements
            // 
            resources.ApplyResources(this.chkAutomaticUpdateMeasurements, "chkAutomaticUpdateMeasurements");
            this.chkAutomaticUpdateMeasurements.Name = "chkAutomaticUpdateMeasurements";
            this.chkAutomaticUpdateMeasurements.Properties.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("chkAutomaticUpdateMeasurements.Properties.Appearance.Font")));
            this.chkAutomaticUpdateMeasurements.Properties.Appearance.Options.UseFont = true;
            this.chkAutomaticUpdateMeasurements.Properties.AutoWidth = true;
            this.chkAutomaticUpdateMeasurements.Properties.Caption = resources.GetString("chkAutomaticUpdateMeasurements.Properties.Caption");
            this.chkAutomaticUpdateMeasurements.CheckedChanged += new System.EventHandler(this.chkAutomaticUpdateMeasurements_CheckedChanged);
            // 
            // tpPrivacy
            // 
            this.tpPrivacy.Controls.Add(this.usrProfilePrivacy1);
            this.tpPrivacy.Name = "tpPrivacy";
            resources.ApplyResources(this.tpPrivacy, "tpPrivacy");
            // 
            // usrProfilePrivacy1
            // 
            resources.ApplyResources(this.usrProfilePrivacy1, "usrProfilePrivacy1");
            this.usrProfilePrivacy1.Name = "usrProfilePrivacy1";
            // 
            // tpNotifications
            // 
            this.tpNotifications.Controls.Add(this.usrProfileNotifications1);
            this.tpNotifications.Name = "tpNotifications";
            resources.ApplyResources(this.tpNotifications, "tpNotifications");
            // 
            // usrProfileNotifications1
            // 
            resources.ApplyResources(this.usrProfileNotifications1, "usrProfileNotifications1");
            this.usrProfileNotifications1.Name = "usrProfileNotifications1";
            // 
            // usrProfileEdit
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tcTabControl);
            this.Name = "usrProfileEdit";
            ((System.ComponentModel.ISupportInitialize)(this.tcTabControl)).EndInit();
            this.tcTabControl.ResumeLayout(false);
            this.tpInfo.ResumeLayout(false);
            this.tpPersonalInfo.ResumeLayout(false);
            this.tpSizes.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkAutomaticUpdateMeasurements.Properties)).EndInit();
            this.tpPrivacy.ResumeLayout(false);
            this.tpNotifications.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTab.XtraTabControl tcTabControl;
        private DevExpress.XtraTab.XtraTabPage tpInfo;
        private usrCreateProfile usrCreateProfile1;
        private DevExpress.XtraTab.XtraTabPage tpSizes;
        private DevExpress.XtraTab.XtraTabPage tpPersonalInfo;
        private usrProfilePersonalInfo usrProfilePersonalInfo1;
        private DevExpress.XtraTab.XtraTabPage tpPrivacy;
        private usrProfilePrivacy usrProfilePrivacy1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private usrWymiaryEditor usrWymiaryEditor1;
        private DevExpress.XtraEditors.CheckEdit chkAutomaticUpdateMeasurements;
        private DevExpress.XtraTab.XtraTabPage tpNotifications;
        private usrProfileNotifications usrProfileNotifications1;

    }
}
