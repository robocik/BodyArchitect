namespace BodyArchitect.Controls.UserControls
{
    partial class usrProfilesBrowser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrProfilesBrowser));
            DevExpress.Utils.SuperToolTip superToolTip1 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipItem toolTipItem1 = new DevExpress.Utils.ToolTipItem();
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.tpMyContacts = new DevExpress.XtraTab.XtraTabPage();
            this.hostMyContacts = new System.Windows.Forms.Integration.ElementHost();
            this.lstMyContacts = new BodyArchitect.Controls.WPF.UsersListView();
            this.tpBrowser = new DevExpress.XtraTab.XtraTabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.baGroupControl1 = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.chkSortOrder = new DevExpress.XtraEditors.CheckButton();
            this.cmbSortOrder = new DevExpress.XtraEditors.ComboBoxEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.lblStatus = new DevExpress.XtraEditors.LabelControl();
            this.cmbWorkoutPlans = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblWorkoutPlans = new DevExpress.XtraEditors.LabelControl();
            this.cmbPhoto = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblPicture = new DevExpress.XtraEditors.LabelControl();
            this.cmbAccessSizes = new DevExpress.XtraEditors.ComboBoxEdit();
            this.cmbAccessCalendar = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblAccessSizes = new DevExpress.XtraEditors.LabelControl();
            this.lblAccessCalendar = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.cmbSearchGroups = new DevExpress.XtraEditors.CheckedComboBoxEdit();
            this.lblGenders = new DevExpress.XtraEditors.LabelControl();
            this.cmbGenders = new DevExpress.XtraEditors.CheckedComboBoxEdit();
            this.cmbCountries = new DevExpress.XtraEditors.CheckedComboBoxEdit();
            this.lblCountry = new DevExpress.XtraEditors.LabelControl();
            this.txtUserName = new DevExpress.XtraEditors.TextEdit();
            this.lblUserName = new DevExpress.XtraEditors.LabelControl();
            this.progressIndicator1 = new BodyArchitect.Controls.UserControls.ProgressIndicator();
            this.btnMoreResults = new DevExpress.XtraEditors.SimpleButton();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.hostBrowser = new System.Windows.Forms.Integration.ElementHost();
            this.lstUserBrowser = new BodyArchitect.Controls.WPF.UsersListView();
            this.usrUserInformation1 = new BodyArchitect.Controls.UserControls.usrUserInformation();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.xtraTabControl1.SuspendLayout();
            this.tpMyContacts.SuspendLayout();
            this.tpBrowser.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.baGroupControl1)).BeginInit();
            this.baGroupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbSortOrder.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbWorkoutPlans.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbPhoto.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbAccessSizes.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbAccessCalendar.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbSearchGroups.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbGenders.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbCountries.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUserName.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainerControl1
            // 
            resources.ApplyResources(this.splitContainerControl1, "splitContainerControl1");
            this.splitContainerControl1.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.xtraTabControl1);
            resources.ApplyResources(this.splitContainerControl1.Panel1, "splitContainerControl1.Panel1");
            this.splitContainerControl1.Panel2.Controls.Add(this.usrUserInformation1);
            resources.ApplyResources(this.splitContainerControl1.Panel2, "splitContainerControl1.Panel2");
            this.splitContainerControl1.SplitterPosition = 309;
            // 
            // xtraTabControl1
            // 
            resources.ApplyResources(this.xtraTabControl1, "xtraTabControl1");
            this.xtraTabControl1.Name = "xtraTabControl1";
            this.xtraTabControl1.SelectedTabPage = this.tpMyContacts;
            this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tpMyContacts,
            this.tpBrowser});
            // 
            // tpMyContacts
            // 
            this.tpMyContacts.Controls.Add(this.hostMyContacts);
            this.tpMyContacts.Name = "tpMyContacts";
            resources.ApplyResources(this.tpMyContacts, "tpMyContacts");
            // 
            // hostMyContacts
            // 
            resources.ApplyResources(this.hostMyContacts, "hostMyContacts");
            this.hostMyContacts.Name = "hostMyContacts";
            this.hostMyContacts.Child = this.lstMyContacts;
            // 
            // tpBrowser
            // 
            this.tpBrowser.Controls.Add(this.tableLayoutPanel2);
            this.tpBrowser.Name = "tpBrowser";
            resources.ApplyResources(this.tpBrowser, "tpBrowser");
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.baGroupControl1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.hostBrowser, 0, 1);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // baGroupControl1
            // 
            this.baGroupControl1.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("baGroupControl1.AppearanceCaption.BackColor")));
            this.baGroupControl1.AppearanceCaption.Options.UseBackColor = true;
            this.baGroupControl1.AppearanceCaption.Options.UseTextOptions = true;
            this.baGroupControl1.AppearanceCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
            this.baGroupControl1.Controls.Add(this.chkSortOrder);
            this.baGroupControl1.Controls.Add(this.cmbSortOrder);
            this.baGroupControl1.Controls.Add(this.labelControl2);
            this.baGroupControl1.Controls.Add(this.lblStatus);
            this.baGroupControl1.Controls.Add(this.cmbWorkoutPlans);
            this.baGroupControl1.Controls.Add(this.lblWorkoutPlans);
            this.baGroupControl1.Controls.Add(this.cmbPhoto);
            this.baGroupControl1.Controls.Add(this.lblPicture);
            this.baGroupControl1.Controls.Add(this.cmbAccessSizes);
            this.baGroupControl1.Controls.Add(this.cmbAccessCalendar);
            this.baGroupControl1.Controls.Add(this.lblAccessSizes);
            this.baGroupControl1.Controls.Add(this.lblAccessCalendar);
            this.baGroupControl1.Controls.Add(this.labelControl1);
            this.baGroupControl1.Controls.Add(this.cmbSearchGroups);
            this.baGroupControl1.Controls.Add(this.lblGenders);
            this.baGroupControl1.Controls.Add(this.cmbGenders);
            this.baGroupControl1.Controls.Add(this.cmbCountries);
            this.baGroupControl1.Controls.Add(this.lblCountry);
            this.baGroupControl1.Controls.Add(this.txtUserName);
            this.baGroupControl1.Controls.Add(this.lblUserName);
            this.baGroupControl1.Controls.Add(this.progressIndicator1);
            this.baGroupControl1.Controls.Add(this.btnMoreResults);
            this.baGroupControl1.Controls.Add(this.btnSearch);
            resources.ApplyResources(this.baGroupControl1, "baGroupControl1");
            this.baGroupControl1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.baGroupControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.baGroupControl1.Name = "baGroupControl1";
            // 
            // chkSortOrder
            // 
            this.chkSortOrder.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("chkSortOrder.Appearance.BackColor")));
            this.chkSortOrder.Appearance.Options.UseBackColor = true;
            this.chkSortOrder.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.chkSortOrder.Image = ((System.Drawing.Image)(resources.GetObject("chkSortOrder.Image")));
            resources.ApplyResources(this.chkSortOrder, "chkSortOrder");
            this.chkSortOrder.Name = "chkSortOrder";
            resources.ApplyResources(toolTipItem1, "toolTipItem1");
            superToolTip1.Items.Add(toolTipItem1);
            this.chkSortOrder.SuperTip = superToolTip1;
            // 
            // cmbSortOrder
            // 
            resources.ApplyResources(this.cmbSortOrder, "cmbSortOrder");
            this.cmbSortOrder.Name = "cmbSortOrder";
            this.cmbSortOrder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cmbSortOrder.Properties.Buttons"))))});
            this.cmbSortOrder.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("labelControl2.Appearance.Font")));
            resources.ApplyResources(this.labelControl2, "labelControl2");
            this.labelControl2.Name = "labelControl2";
            // 
            // lblStatus
            // 
            resources.ApplyResources(this.lblStatus, "lblStatus");
            this.lblStatus.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblStatus.Name = "lblStatus";
            // 
            // cmbWorkoutPlans
            // 
            resources.ApplyResources(this.cmbWorkoutPlans, "cmbWorkoutPlans");
            this.cmbWorkoutPlans.Name = "cmbWorkoutPlans";
            this.cmbWorkoutPlans.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cmbWorkoutPlans.Properties.Buttons"))))});
            this.cmbWorkoutPlans.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // lblWorkoutPlans
            // 
            resources.ApplyResources(this.lblWorkoutPlans, "lblWorkoutPlans");
            this.lblWorkoutPlans.Name = "lblWorkoutPlans";
            // 
            // cmbPhoto
            // 
            resources.ApplyResources(this.cmbPhoto, "cmbPhoto");
            this.cmbPhoto.Name = "cmbPhoto";
            this.cmbPhoto.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cmbPhoto.Properties.Buttons"))))});
            this.cmbPhoto.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // lblPicture
            // 
            resources.ApplyResources(this.lblPicture, "lblPicture");
            this.lblPicture.Name = "lblPicture";
            // 
            // cmbAccessSizes
            // 
            resources.ApplyResources(this.cmbAccessSizes, "cmbAccessSizes");
            this.cmbAccessSizes.Name = "cmbAccessSizes";
            this.cmbAccessSizes.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cmbAccessSizes.Properties.Buttons"))))});
            this.cmbAccessSizes.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // cmbAccessCalendar
            // 
            resources.ApplyResources(this.cmbAccessCalendar, "cmbAccessCalendar");
            this.cmbAccessCalendar.Name = "cmbAccessCalendar";
            this.cmbAccessCalendar.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cmbAccessCalendar.Properties.Buttons"))))});
            this.cmbAccessCalendar.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // lblAccessSizes
            // 
            resources.ApplyResources(this.lblAccessSizes, "lblAccessSizes");
            this.lblAccessSizes.Name = "lblAccessSizes";
            // 
            // lblAccessCalendar
            // 
            resources.ApplyResources(this.lblAccessCalendar, "lblAccessCalendar");
            this.lblAccessCalendar.Name = "lblAccessCalendar";
            // 
            // labelControl1
            // 
            resources.ApplyResources(this.labelControl1, "labelControl1");
            this.labelControl1.Name = "labelControl1";
            // 
            // cmbSearchGroups
            // 
            this.cmbSearchGroups.EditValue = global::BodyArchitect.Controls.Localization.DomainModelStrings.AchievementRank_Rank1;
            resources.ApplyResources(this.cmbSearchGroups, "cmbSearchGroups");
            this.cmbSearchGroups.Name = "cmbSearchGroups";
            this.cmbSearchGroups.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cmbSearchGroups.Properties.Buttons"))))});
            this.cmbSearchGroups.Properties.NullValuePrompt = resources.GetString("cmbSearchGroups.Properties.NullValuePrompt");
            this.cmbSearchGroups.Properties.NullValuePromptShowForEmptyValue = ((bool)(resources.GetObject("cmbSearchGroups.Properties.NullValuePromptShowForEmptyValue")));
            this.cmbSearchGroups.Properties.SelectAllItemCaption = resources.GetString("cmbSearchGroups.Properties.SelectAllItemCaption");
            // 
            // lblGenders
            // 
            resources.ApplyResources(this.lblGenders, "lblGenders");
            this.lblGenders.Name = "lblGenders";
            // 
            // cmbGenders
            // 
            this.cmbGenders.EditValue = global::BodyArchitect.Controls.Localization.DomainModelStrings.AchievementRank_Rank1;
            resources.ApplyResources(this.cmbGenders, "cmbGenders");
            this.cmbGenders.Name = "cmbGenders";
            this.cmbGenders.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cmbGenders.Properties.Buttons"))))});
            this.cmbGenders.Properties.NullValuePrompt = resources.GetString("cmbGenders.Properties.NullValuePrompt");
            this.cmbGenders.Properties.NullValuePromptShowForEmptyValue = ((bool)(resources.GetObject("cmbGenders.Properties.NullValuePromptShowForEmptyValue")));
            this.cmbGenders.Properties.SelectAllItemCaption = resources.GetString("cmbGenders.Properties.SelectAllItemCaption");
            // 
            // cmbCountries
            // 
            this.cmbCountries.EditValue = global::BodyArchitect.Controls.Localization.DomainModelStrings.AchievementRank_Rank1;
            resources.ApplyResources(this.cmbCountries, "cmbCountries");
            this.cmbCountries.Name = "cmbCountries";
            this.cmbCountries.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cmbCountries.Properties.Buttons"))))});
            this.cmbCountries.Properties.NullValuePrompt = resources.GetString("cmbCountries.Properties.NullValuePrompt");
            this.cmbCountries.Properties.NullValuePromptShowForEmptyValue = ((bool)(resources.GetObject("cmbCountries.Properties.NullValuePromptShowForEmptyValue")));
            this.cmbCountries.Properties.SelectAllItemCaption = resources.GetString("cmbCountries.Properties.SelectAllItemCaption");
            // 
            // lblCountry
            // 
            resources.ApplyResources(this.lblCountry, "lblCountry");
            this.lblCountry.Name = "lblCountry";
            // 
            // txtUserName
            // 
            resources.ApplyResources(this.txtUserName, "txtUserName");
            this.txtUserName.Name = "txtUserName";
            // 
            // lblUserName
            // 
            resources.ApplyResources(this.lblUserName, "lblUserName");
            this.lblUserName.Name = "lblUserName";
            // 
            // progressIndicator1
            // 
            resources.ApplyResources(this.progressIndicator1, "progressIndicator1");
            this.progressIndicator1.Name = "progressIndicator1";
            this.progressIndicator1.Percentage = 0F;
            // 
            // btnMoreResults
            // 
            resources.ApplyResources(this.btnMoreResults, "btnMoreResults");
            this.btnMoreResults.Image = ((System.Drawing.Image)(resources.GetObject("btnMoreResults.Image")));
            this.btnMoreResults.Name = "btnMoreResults";
            this.btnMoreResults.Click += new System.EventHandler(this.btnMoreResults_Click);
            // 
            // btnSearch
            // 
            resources.ApplyResources(this.btnSearch, "btnSearch");
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // hostBrowser
            // 
            resources.ApplyResources(this.hostBrowser, "hostBrowser");
            this.hostBrowser.BackColor = System.Drawing.Color.Transparent;
            this.hostBrowser.Name = "hostBrowser";
            this.hostBrowser.Child = this.lstUserBrowser;
            // 
            // usrUserInformation1
            // 
            resources.ApplyResources(this.usrUserInformation1, "usrUserInformation1");
            this.usrUserInformation1.Name = "usrUserInformation1";
            // 
            // usrProfilesBrowser
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerControl1);
            this.Name = "usrProfilesBrowser";
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            this.xtraTabControl1.ResumeLayout(false);
            this.tpMyContacts.ResumeLayout(false);
            this.tpBrowser.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.baGroupControl1)).EndInit();
            this.baGroupControl1.ResumeLayout(false);
            this.baGroupControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbSortOrder.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbWorkoutPlans.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbPhoto.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbAccessSizes.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbAccessCalendar.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbSearchGroups.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbGenders.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbCountries.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUserName.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private usrUserInformation usrUserInformation1;
        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraTab.XtraTabPage tpMyContacts;
        private DevExpress.XtraTab.XtraTabPage tpBrowser;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private BaGroupControl baGroupControl1;
        private DevExpress.XtraEditors.TextEdit txtUserName;
        private DevExpress.XtraEditors.LabelControl lblUserName;
        private ProgressIndicator progressIndicator1;
        private DevExpress.XtraEditors.SimpleButton btnMoreResults;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraEditors.LabelControl lblCountry;
        private DevExpress.XtraEditors.CheckedComboBoxEdit cmbCountries;
        private DevExpress.XtraEditors.LabelControl lblGenders;
        private DevExpress.XtraEditors.CheckedComboBoxEdit cmbGenders;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.CheckedComboBoxEdit cmbSearchGroups;
        private DevExpress.XtraEditors.LabelControl lblAccessSizes;
        private DevExpress.XtraEditors.LabelControl lblAccessCalendar;
        private DevExpress.XtraEditors.ComboBoxEdit cmbAccessSizes;
        private DevExpress.XtraEditors.ComboBoxEdit cmbAccessCalendar;
        private DevExpress.XtraEditors.LabelControl lblWorkoutPlans;
        private DevExpress.XtraEditors.ComboBoxEdit cmbPhoto;
        private DevExpress.XtraEditors.LabelControl lblPicture;
        private DevExpress.XtraEditors.ComboBoxEdit cmbWorkoutPlans;
        private DevExpress.XtraEditors.LabelControl lblStatus;
        private System.Windows.Forms.Integration.ElementHost hostBrowser;
        private BodyArchitect.Controls.WPF.UsersListView lstUserBrowser;
        private System.Windows.Forms.Integration.ElementHost hostMyContacts;
        private WPF.UsersListView lstMyContacts;
        private DevExpress.XtraEditors.ComboBoxEdit cmbSortOrder;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.CheckButton chkSortOrder;


    }
}
