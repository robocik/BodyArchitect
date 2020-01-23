using BodyArchitect.Controls.UserControls;

namespace BodyArchitect.Controls.Forms
{
    partial class MainWindow
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.usrRssReader1 = new BodyArchitect.Controls.RssReader.usrRssReader();
            this.tcMainTabControl = new DevExpress.XtraTab.XtraTabControl();
            this.tpProfileInfo = new DevExpress.XtraTab.XtraTabPage();
            this.usrProfileInfoView1 = new BodyArchitect.Controls.UserControls.usrProfileInfoView();
            this.tpCalendar = new DevExpress.XtraTab.XtraTabPage();
            this.usrCalendarView1 = new BodyArchitect.Controls.UserControls.usrCalendarView();
            this.tpReports = new DevExpress.XtraTab.XtraTabPage();
            this.usrReporting1 = new BodyArchitect.Controls.Reporting.usrReporting();
            this.tpPeople = new DevExpress.XtraTab.XtraTabPage();
            this.usrProfilesBrowser1 = new BodyArchitect.Controls.UserControls.usrProfilesBrowser();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnuTools = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuProfileEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLogin = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLogout = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCalcs = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuBMI = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditCut = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuView = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuViewCurrentMonth = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewFirstEntry = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewLastEntry = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuViewLogFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLogStandard = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLogExceptions = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuShowToolTips = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLanguage = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLangEnglish = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLangPolish = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpTutorial = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpTutorialAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuHelpTutorialConfigureProfile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpTutorialWorkoutLog = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpTutorialUsers = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpTutorialWorkoutPlans = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.defaultToolTipController1 = new DevExpress.Utils.DefaultToolTipController(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.progressStatus = new BodyArchitect.Controls.UserControls.ToolStripProgressIndicator();
            this.sbpLogin = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsCalendarView = new System.Windows.Forms.ToolStripDropDownButton();
            this.defaultLookAndFeel1 = new DevExpress.LookAndFeel.DefaultLookAndFeel(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tcMainTabControl)).BeginInit();
            this.tcMainTabControl.SuspendLayout();
            this.tpProfileInfo.SuspendLayout();
            this.tpCalendar.SuspendLayout();
            this.tpReports.SuspendLayout();
            this.tpPeople.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerControl1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.splitContainerControl1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("splitContainerControl1.AllowHtmlText"))));
            resources.ApplyResources(this.splitContainerControl1, "splitContainerControl1");
            this.splitContainerControl1.Horizontal = false;
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.usrRssReader1);
            resources.ApplyResources(this.splitContainerControl1.Panel1, "splitContainerControl1.Panel1");
            this.splitContainerControl1.Panel2.Controls.Add(this.tcMainTabControl);
            resources.ApplyResources(this.splitContainerControl1.Panel2, "splitContainerControl1.Panel2");
            this.splitContainerControl1.SplitterPosition = 72;
            // 
            // usrRssReader1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.usrRssReader1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("usrRssReader1.AllowHtmlText"))));
            resources.ApplyResources(this.usrRssReader1, "usrRssReader1");
            this.usrRssReader1.Name = "usrRssReader1";
            // 
            // tcMainTabControl
            // 
            resources.ApplyResources(this.tcMainTabControl, "tcMainTabControl");
            this.tcMainTabControl.Name = "tcMainTabControl";
            this.tcMainTabControl.SelectedTabPage = this.tpProfileInfo;
            this.tcMainTabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tpProfileInfo,
            this.tpCalendar,
            this.tpReports,
            this.tpPeople});
            this.tcMainTabControl.CloseButtonClick += new System.EventHandler(this.tcMainTabControl_CloseButtonClick);
            // 
            // tpProfileInfo
            // 
            this.tpProfileInfo.Controls.Add(this.usrProfileInfoView1);
            this.tpProfileInfo.Image = global::BodyArchitect.Controls.Icons.Profile;
            this.tpProfileInfo.Name = "tpProfileInfo";
            this.tpProfileInfo.ShowCloseButton = DevExpress.Utils.DefaultBoolean.False;
            resources.ApplyResources(this.tpProfileInfo, "tpProfileInfo");
            // 
            // usrProfileInfoView1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.usrProfileInfoView1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("usrProfileInfoView1.AllowHtmlText"))));
            resources.ApplyResources(this.usrProfileInfoView1, "usrProfileInfoView1");
            this.usrProfileInfoView1.Name = "usrProfileInfoView1";
            // 
            // tpCalendar
            // 
            this.tpCalendar.Controls.Add(this.usrCalendarView1);
            this.tpCalendar.Image = ((System.Drawing.Image)(resources.GetObject("tpCalendar.Image")));
            this.tpCalendar.Name = "tpCalendar";
            this.tpCalendar.ShowCloseButton = DevExpress.Utils.DefaultBoolean.False;
            resources.ApplyResources(this.tpCalendar, "tpCalendar");
            // 
            // usrCalendarView1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.usrCalendarView1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("usrCalendarView1.AllowHtmlText"))));
            resources.ApplyResources(this.usrCalendarView1, "usrCalendarView1");
            this.usrCalendarView1.Name = "usrCalendarView1";
            // 
            // tpReports
            // 
            this.tpReports.Controls.Add(this.usrReporting1);
            this.tpReports.Image = ((System.Drawing.Image)(resources.GetObject("tpReports.Image")));
            this.tpReports.Name = "tpReports";
            this.tpReports.ShowCloseButton = DevExpress.Utils.DefaultBoolean.False;
            resources.ApplyResources(this.tpReports, "tpReports");
            // 
            // usrReporting1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.usrReporting1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("usrReporting1.AllowHtmlText"))));
            resources.ApplyResources(this.usrReporting1, "usrReporting1");
            this.usrReporting1.Name = "usrReporting1";
            // 
            // tpPeople
            // 
            this.tpPeople.Controls.Add(this.usrProfilesBrowser1);
            this.tpPeople.Image = global::BodyArchitect.Controls.Icons.ProfileChange;
            this.tpPeople.Name = "tpPeople";
            this.tpPeople.ShowCloseButton = DevExpress.Utils.DefaultBoolean.False;
            resources.ApplyResources(this.tpPeople, "tpPeople");
            // 
            // usrProfilesBrowser1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.usrProfilesBrowser1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("usrProfilesBrowser1.AllowHtmlText"))));
            resources.ApplyResources(this.usrProfilesBrowser1, "usrProfilesBrowser1");
            this.usrProfilesBrowser1.Name = "usrProfilesBrowser1";
            // 
            // menuStrip1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.menuStrip1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("menuStrip1.AllowHtmlText"))));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuTools,
            this.mnuCalcs,
            this.editToolStripMenuItem,
            this.mnuView,
            this.mnuOptions,
            this.mnuHelp});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // mnuTools
            // 
            this.mnuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuProfileEdit,
            this.mnuLogin,
            this.mnuLogout});
            this.mnuTools.Name = "mnuTools";
            resources.ApplyResources(this.mnuTools, "mnuTools");
            this.mnuTools.DropDownOpening += new System.EventHandler(this.mnuTools_DropDownOpening);
            // 
            // mnuProfileEdit
            // 
            this.mnuProfileEdit.Image = global::BodyArchitect.Controls.Icons.ProfileEdit;
            resources.ApplyResources(this.mnuProfileEdit, "mnuProfileEdit");
            this.mnuProfileEdit.Name = "mnuProfileEdit";
            this.mnuProfileEdit.Click += new System.EventHandler(this.mnuProfileEdit_Click);
            // 
            // mnuLogin
            // 
            this.mnuLogin.Name = "mnuLogin";
            resources.ApplyResources(this.mnuLogin, "mnuLogin");
            this.mnuLogin.Click += new System.EventHandler(this.mnuLogin_Click);
            // 
            // mnuLogout
            // 
            this.mnuLogout.Name = "mnuLogout";
            resources.ApplyResources(this.mnuLogout, "mnuLogout");
            this.mnuLogout.Click += new System.EventHandler(this.mnuLogout_Click);
            // 
            // mnuCalcs
            // 
            this.mnuCalcs.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuBMI});
            this.mnuCalcs.Name = "mnuCalcs";
            resources.ApplyResources(this.mnuCalcs, "mnuCalcs");
            // 
            // mnuBMI
            // 
            this.mnuBMI.Name = "mnuBMI";
            resources.ApplyResources(this.mnuBMI, "mnuBMI");
            this.mnuBMI.Click += new System.EventHandler(this.mnuBMI_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuEditCut,
            this.mnuEditCopy,
            this.mnuEditPaste});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            resources.ApplyResources(this.editToolStripMenuItem, "editToolStripMenuItem");
            this.editToolStripMenuItem.DropDownOpening += new System.EventHandler(this.editToolStripMenuItem_DropDownOpening);
            // 
            // mnuEditCut
            // 
            resources.ApplyResources(this.mnuEditCut, "mnuEditCut");
            this.mnuEditCut.Name = "mnuEditCut";
            this.mnuEditCut.Click += new System.EventHandler(this.mnuEditCut_Click);
            // 
            // mnuEditCopy
            // 
            resources.ApplyResources(this.mnuEditCopy, "mnuEditCopy");
            this.mnuEditCopy.Name = "mnuEditCopy";
            this.mnuEditCopy.Click += new System.EventHandler(this.mnuEditCopy_Click);
            // 
            // mnuEditPaste
            // 
            resources.ApplyResources(this.mnuEditPaste, "mnuEditPaste");
            this.mnuEditPaste.Name = "mnuEditPaste";
            this.mnuEditPaste.Click += new System.EventHandler(this.mnuEditPaste_Click);
            // 
            // mnuView
            // 
            this.mnuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuViewRefresh,
            this.toolStripMenuItem1,
            this.mnuViewCurrentMonth,
            this.mnuViewFirstEntry,
            this.mnuViewLastEntry,
            this.toolStripMenuItem2,
            this.mnuViewLogFiles});
            this.mnuView.Name = "mnuView";
            resources.ApplyResources(this.mnuView, "mnuView");
            this.mnuView.DropDownOpening += new System.EventHandler(this.mnuView_DropDownOpening);
            // 
            // mnuViewRefresh
            // 
            resources.ApplyResources(this.mnuViewRefresh, "mnuViewRefresh");
            this.mnuViewRefresh.Name = "mnuViewRefresh";
            this.mnuViewRefresh.Click += new System.EventHandler(this.mnuViewRefresh_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            // 
            // mnuViewCurrentMonth
            // 
            this.mnuViewCurrentMonth.Name = "mnuViewCurrentMonth";
            resources.ApplyResources(this.mnuViewCurrentMonth, "mnuViewCurrentMonth");
            this.mnuViewCurrentMonth.Click += new System.EventHandler(this.mnuViewCurrentMonth_Click);
            // 
            // mnuViewFirstEntry
            // 
            this.mnuViewFirstEntry.Name = "mnuViewFirstEntry";
            resources.ApplyResources(this.mnuViewFirstEntry, "mnuViewFirstEntry");
            this.mnuViewFirstEntry.Click += new System.EventHandler(this.mnuViewFirstEntry_Click);
            // 
            // mnuViewLastEntry
            // 
            this.mnuViewLastEntry.Name = "mnuViewLastEntry";
            resources.ApplyResources(this.mnuViewLastEntry, "mnuViewLastEntry");
            this.mnuViewLastEntry.Click += new System.EventHandler(this.mnuViewLastEntry_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            // 
            // mnuViewLogFiles
            // 
            this.mnuViewLogFiles.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuLogStandard,
            this.mnuLogExceptions});
            this.mnuViewLogFiles.Name = "mnuViewLogFiles";
            resources.ApplyResources(this.mnuViewLogFiles, "mnuViewLogFiles");
            // 
            // mnuLogStandard
            // 
            resources.ApplyResources(this.mnuLogStandard, "mnuLogStandard");
            this.mnuLogStandard.Name = "mnuLogStandard";
            this.mnuLogStandard.Click += new System.EventHandler(this.mnuLogStandard_Click);
            // 
            // mnuLogExceptions
            // 
            resources.ApplyResources(this.mnuLogExceptions, "mnuLogExceptions");
            this.mnuLogExceptions.Name = "mnuLogExceptions";
            this.mnuLogExceptions.Click += new System.EventHandler(this.mnuLogExceptions_Click);
            // 
            // mnuOptions
            // 
            this.mnuOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuShowToolTips,
            this.mnuLanguage,
            this.optionsToolStripMenuItem});
            this.mnuOptions.Name = "mnuOptions";
            resources.ApplyResources(this.mnuOptions, "mnuOptions");
            // 
            // mnuShowToolTips
            // 
            this.mnuShowToolTips.Image = global::BodyArchitect.Controls.Icons.ToolTip;
            resources.ApplyResources(this.mnuShowToolTips, "mnuShowToolTips");
            this.mnuShowToolTips.Name = "mnuShowToolTips";
            this.mnuShowToolTips.Click += new System.EventHandler(this.mnuShowToolTips_Click);
            // 
            // mnuLanguage
            // 
            this.mnuLanguage.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuLangEnglish,
            this.mnuLangPolish});
            this.mnuLanguage.Name = "mnuLanguage";
            resources.ApplyResources(this.mnuLanguage, "mnuLanguage");
            // 
            // mnuLangEnglish
            // 
            this.mnuLangEnglish.Image = global::BodyArchitect.Controls.Icons.England;
            resources.ApplyResources(this.mnuLangEnglish, "mnuLangEnglish");
            this.mnuLangEnglish.Name = "mnuLangEnglish";
            this.mnuLangEnglish.Tag = "en";
            this.mnuLangEnglish.Click += new System.EventHandler(this.mnuLangPolish_Click);
            // 
            // mnuLangPolish
            // 
            this.mnuLangPolish.Image = global::BodyArchitect.Controls.Icons.Poland;
            resources.ApplyResources(this.mnuLangPolish, "mnuLangPolish");
            this.mnuLangPolish.Name = "mnuLangPolish";
            this.mnuLangPolish.Tag = "pl";
            this.mnuLangPolish.Click += new System.EventHandler(this.mnuLangPolish_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            resources.ApplyResources(this.optionsToolStripMenuItem, "optionsToolStripMenuItem");
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // mnuHelp
            // 
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHelpTutorial,
            this.toolStripSeparator1,
            this.mnuAbout});
            this.mnuHelp.Name = "mnuHelp";
            resources.ApplyResources(this.mnuHelp, "mnuHelp");
            // 
            // mnuHelpTutorial
            // 
            this.mnuHelpTutorial.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHelpTutorialAll,
            this.toolStripMenuItem3,
            this.mnuHelpTutorialConfigureProfile,
            this.mnuHelpTutorialWorkoutLog,
            this.mnuHelpTutorialUsers,
            this.mnuHelpTutorialWorkoutPlans});
            this.mnuHelpTutorial.Name = "mnuHelpTutorial";
            resources.ApplyResources(this.mnuHelpTutorial, "mnuHelpTutorial");
            // 
            // mnuHelpTutorialAll
            // 
            resources.ApplyResources(this.mnuHelpTutorialAll, "mnuHelpTutorialAll");
            this.mnuHelpTutorialAll.Name = "mnuHelpTutorialAll";
            this.mnuHelpTutorialAll.Tag = "http://bodyarchitectonline.com/index.php/tutorial";
            this.mnuHelpTutorialAll.Click += new System.EventHandler(this.allToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
            // 
            // mnuHelpTutorialConfigureProfile
            // 
            this.mnuHelpTutorialConfigureProfile.Name = "mnuHelpTutorialConfigureProfile";
            resources.ApplyResources(this.mnuHelpTutorialConfigureProfile, "mnuHelpTutorialConfigureProfile");
            this.mnuHelpTutorialConfigureProfile.Tag = "http://www.youtube.com/watch?v=kiEITIU557U&feature=player_embedded";
            this.mnuHelpTutorialConfigureProfile.Click += new System.EventHandler(this.allToolStripMenuItem_Click);
            // 
            // mnuHelpTutorialWorkoutLog
            // 
            this.mnuHelpTutorialWorkoutLog.Name = "mnuHelpTutorialWorkoutLog";
            resources.ApplyResources(this.mnuHelpTutorialWorkoutLog, "mnuHelpTutorialWorkoutLog");
            this.mnuHelpTutorialWorkoutLog.Tag = "";
            this.mnuHelpTutorialWorkoutLog.Click += new System.EventHandler(this.allToolStripMenuItem_Click);
            // 
            // mnuHelpTutorialUsers
            // 
            this.mnuHelpTutorialUsers.Name = "mnuHelpTutorialUsers";
            resources.ApplyResources(this.mnuHelpTutorialUsers, "mnuHelpTutorialUsers");
            this.mnuHelpTutorialUsers.Tag = "";
            this.mnuHelpTutorialUsers.Click += new System.EventHandler(this.allToolStripMenuItem_Click);
            // 
            // mnuHelpTutorialWorkoutPlans
            // 
            this.mnuHelpTutorialWorkoutPlans.Name = "mnuHelpTutorialWorkoutPlans";
            resources.ApplyResources(this.mnuHelpTutorialWorkoutPlans, "mnuHelpTutorialWorkoutPlans");
            this.mnuHelpTutorialWorkoutPlans.Tag = "";
            this.mnuHelpTutorialWorkoutPlans.Click += new System.EventHandler(this.allToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // mnuAbout
            // 
            this.mnuAbout.Name = "mnuAbout";
            resources.ApplyResources(this.mnuAbout, "mnuAbout");
            this.mnuAbout.Click += new System.EventHandler(this.mnuAbout_Click);
            // 
            // defaultToolTipController1
            // 
            // 
            // 
            // 
            this.defaultToolTipController1.DefaultController.Active = false;
            this.defaultToolTipController1.DefaultController.CloseOnClick = DevExpress.Utils.DefaultBoolean.True;
            // 
            // statusStrip1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.statusStrip1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("statusStrip1.AllowHtmlText"))));
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressStatus,
            this.sbpLogin,
            this.tsCalendarView});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            // 
            // progressStatus
            // 
            this.progressStatus.Name = "progressStatus";
            resources.ApplyResources(this.progressStatus, "progressStatus");
            // 
            // sbpLogin
            // 
            this.sbpLogin.DoubleClickEnabled = true;
            resources.ApplyResources(this.sbpLogin, "sbpLogin");
            this.sbpLogin.Name = "sbpLogin";
            this.sbpLogin.DoubleClick += new System.EventHandler(this.sbpLogin_DoubleClick);
            // 
            // tsCalendarView
            // 
            resources.ApplyResources(this.tsCalendarView, "tsCalendarView");
            this.tsCalendarView.Name = "tsCalendarView";
            // 
            // MainWindow
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("$this.AllowHtmlText"))));
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerControl1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.MainWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tcMainTabControl)).EndInit();
            this.tcMainTabControl.ResumeLayout(false);
            this.tpProfileInfo.ResumeLayout(false);
            this.tpCalendar.ResumeLayout(false);
            this.tpReports.ResumeLayout(false);
            this.tpPeople.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuTools;
        private System.Windows.Forms.ToolStripMenuItem mnuProfileEdit;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private DevExpress.Utils.DefaultToolTipController defaultToolTipController1;
        private System.Windows.Forms.ToolStripMenuItem mnuOptions;
        private System.Windows.Forms.ToolStripMenuItem mnuShowToolTips;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private BodyArchitect.Controls.RssReader.usrRssReader usrRssReader1;
        private System.Windows.Forms.ToolStripMenuItem mnuAbout;
        private System.Windows.Forms.ToolStripMenuItem mnuView;
        private System.Windows.Forms.ToolStripMenuItem mnuViewCurrentMonth;
        private System.Windows.Forms.ToolStripMenuItem mnuLanguage;
        private System.Windows.Forms.ToolStripMenuItem mnuLangEnglish;
        private System.Windows.Forms.ToolStripMenuItem mnuLangPolish;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem mnuViewLogFiles;
        private System.Windows.Forms.ToolStripMenuItem mnuLogStandard;
        private System.Windows.Forms.ToolStripMenuItem mnuLogExceptions;
        private System.Windows.Forms.ToolStripMenuItem mnuViewLastEntry;
        private System.Windows.Forms.ToolStripMenuItem mnuViewFirstEntry;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuEditCut;
        private System.Windows.Forms.ToolStripMenuItem mnuEditCopy;
        private System.Windows.Forms.ToolStripMenuItem mnuEditPaste;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private DevExpress.XtraTab.XtraTabControl tcMainTabControl;
        private DevExpress.XtraTab.XtraTabPage tpCalendar;
        private UserControls.usrCalendarView usrCalendarView1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuCalcs;
        private System.Windows.Forms.ToolStripMenuItem mnuBMI;
        private DevExpress.XtraTab.XtraTabPage tpReports;
        private Reporting.usrReporting usrReporting1;
        private ToolStripProgressIndicator progressStatus;
        private DevExpress.LookAndFeel.DefaultLookAndFeel defaultLookAndFeel1;
        private System.Windows.Forms.ToolStripDropDownButton tsCalendarView;
        private System.Windows.Forms.ToolStripMenuItem mnuLogin;
        private System.Windows.Forms.ToolStripMenuItem mnuLogout;
        private System.Windows.Forms.ToolStripStatusLabel sbpLogin;
        private DevExpress.XtraTab.XtraTabPage tpPeople;
        private usrProfilesBrowser usrProfilesBrowser1;
        private DevExpress.XtraTab.XtraTabPage tpProfileInfo;
        private usrProfileInfoView usrProfileInfoView1;
        private System.Windows.Forms.ToolStripMenuItem mnuViewRefresh;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpTutorial;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpTutorialAll;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpTutorialConfigureProfile;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpTutorialWorkoutLog;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpTutorialUsers;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpTutorialWorkoutPlans;
        
    }
}