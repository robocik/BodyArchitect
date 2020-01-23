using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Controls.UserControls;
using BodyArchitect.Service.Model;
using BodyArchitect.Settings;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using BodyArchitect.Common;
using BodyArchitect.Common.Plugins;
using BodyArchitect.Controls;
using BodyArchitect.Controls.Calculators;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Logger;

using DevExpress.XtraEditors.Controls;
using System.Reflection;
using System.Threading;
using System.Deployment.Application;
using System.Globalization;
using BodyArchitect.Shared;
using DevExpress.XtraTab;
using DevExpress.XtraTab.ViewInfo;


namespace BodyArchitect.Controls.Forms
{
    public partial class MainWindow : BaseWindow, IMainWindow
    {
        static  MainWindow instance;

        public event EventHandler ProfileConfigurationWizardShow;

        public MainWindow()
        {
            InitializeComponent();
            
            ControlHelper.MainWindow = this;
            UserContext.Settings.GuiState.LoadProcess(this);
            fillCalendarView();
            Localizer.Active = new DevExpressControlsLocalizer();
            defaultToolTipController1.DefaultController.Active = UserContext.Settings.GuiState.ShowToolTips;
            mnuShowToolTips.Checked = UserContext.Settings.GuiState.ShowToolTips;
            splitContainerControl1.SplitterPosition = UserContext.Settings.GuiState.NewsPanelSize;
            selectLanguage(Thread.CurrentThread.CurrentUICulture);
            
            foreach( var plugin in PluginsManager.Instance.Modules)
            {
                Log.WriteVerbose("Plugin {0}: Create gui", plugin.GlobalId);
                plugin.CreateGui(this);
                plugin.CreateMainMenuItems(this.MainMenuStrip);
            }
            setTutorialLinks();

        }

        private void setTutorialLinks()
        {
            mnuHelpTutorialAll.Tag = ControlHelper.TutorialAll;
            mnuHelpTutorialConfigureProfile.Tag = ControlHelper.TutorialCreateProfile;
            mnuHelpTutorialWorkoutLog.Tag = ControlHelper.TutorialWorkoutLog;
            mnuHelpTutorialUsers.Tag = ControlHelper.TutorialUsers;
            mnuHelpTutorialWorkoutPlans.Tag = ControlHelper.TutorialWorkoutPlans;
        }
        protected override void LoginStatusChanged(LoginStatus newStatus)
        {
            Log.WriteVerbose("LoginStatus changed: {0}", newStatus);
            removeAdditionalTabPages();
            SetProgressStatus(UserContext.LoginStatus == LoginStatus.InProgress, UserContext.LoginStatus.ToString());
            if (newStatus == LoginStatus.Logged)
            {
                Fill();
            }
            else if (/*newStatus == LoginStatus.LoginFailed ||*/ newStatus == LoginStatus.NotLogged)
            {
                setMainWindowTitle();
                login();
            }

            if (newStatus == LoginStatus.NotLogged)
            {
                Settings1.Default.AutoLoginPassword = Settings1.Default.AutoLoginUserName = string.Empty;
            }
        }

        void removeAdditionalTabPages()
        {
            Log.WriteVerbose("removeAdditionalTabPages");
            for (int index = tcMainTabControl.TabPages.Count-1; index >=0 ; index--)
            {
                XtraTabPage tabPage = tcMainTabControl.TabPages[index];
                if (tabPage.ShowCloseButton == DefaultBoolean.True)
                {
                    tcMainTabControl.TabPages.Remove(tabPage);
                }
            }
        }

        void login()
        {
            if(InvokeRequired)
            {
                BeginInvoke(new Action(login));
            }
            else
            {
                Log.WriteVerbose("MainWindow.login()");
                LoginWindow dlg = new LoginWindow();
                if (dlg.ShowDialog() == DialogResult.Cancel)
                {
                    Log.WriteInfo("Login cancel");
                    if (UserContext.LoginStatus != LoginStatus.Logged)
                    {//if there is no logged user then close the application
                        Log.WriteVerbose("there is no logged user - close the application");
                        Close();
                    }
                    return;
                }
                
            }
        }
        void fillCalendarView()
        {
            Log.WriteVerbose("fillCalendarView");
            foreach (var dayContent in PluginsManager.Instance.CalendarDayContents)
            {
                ToolStripMenuItem item = (ToolStripMenuItem)tsCalendarView.DropDownItems.Add(dayContent.Name);
                item.Tag = dayContent;
                item.Click += new EventHandler(calendarView_Click);
                if(UserContext.Settings.GuiState.CalendarOptions.CalendarTextType==dayContent.GlobalId)
                {
                    item.Checked = true;
                    Log.WriteVerbose("Checked: {0}", dayContent.GlobalId);
                }
            }
        }

        void calendarView_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem) sender;
            foreach (ToolStripMenuItem item in tsCalendarView.DropDownItems)
            {
                item.Checked = false;
            }
            ICalendarDayContent dayContent = (ICalendarDayContent) menuItem.Tag;
            UserContext.Settings.GuiState.CalendarOptions.CalendarTextType = dayContent.GlobalId;
            UserContext.Settings.GuiState.Save();
            menuItem.Checked = true;
            
            Fill();
        }

        public void SetProgressStatus(bool showProgress,string message, params object[] args)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<bool,string, object[]>(SetProgressStatus),showProgress, message, args);
            }
            else
            {
                if (progressStatus!=null && progressStatus.ProgressIndicatorStatus != null)
                {
                    progressStatus.ShowProgress = showProgress;
                    progressStatus.ProgressIndicatorStatus.Message = string.Format(message, args);
                }
            }

        }

        public static MainWindow Instance
        {
            get
            {
                if(instance==null)
                {
                    instance= new MainWindow();
                }
                return instance;
            }
        }



        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            UserContext.Settings.GuiState.NewsPanelSize = splitContainerControl1.SplitterPosition;
            UserContext.Settings.GuiState.SaveProcess(this);
            base.OnFormClosed(e);
        }

        protected override void OnLoad(EventArgs e)
        {
           
            base.OnLoad(e);

            Log.WriteInfo("Login window is showing");
            LoginWindow dlg = new LoginWindow();
            //dlg.ShowForceUpdate = updateManager.UpdateFailed;

            if (dlg.ShowDialog() == DialogResult.Cancel)
            {
                Logger.Log.WriteInfo("Login cancel");
                //Hide();
                Application.Exit();
                return;
            }
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            progressStatus.Connect(TasksManager);
            usrRssReader1.Fill();
            Fill();
        }

        public void ShowProfileConfigurationWizard()
        {
            if(ProfileConfigurationWizardShow!=null)
            {
                ProfileConfigurationWizardShow(this, EventArgs.Empty);
            }
        }

        private void mnuProfileEdit_Click(object sender, EventArgs e)
        {

            try
            {
                ProfileEditWindow dlg = new ProfileEditWindow();
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //SessionData.Session.Refresh(UserContext.CurrentProfile);
                    //Text = string.Format(ApplicationStrings.MainWindowTitle, Constants.ApplicationName, UserContext.CurrentProfile.UserName);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorLoadProfile, ErrorWindow.EMailReport);
            }
        }

        public void Fill()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(Fill));
            }
            else
            {
                SetProgressStatus(UserContext.LoginStatus==LoginStatus.InProgress,UserContext.LoginStatus.ToString());
                setMainWindowTitle();

                Log.WriteVerbose("Fill. CalendarDayContent: {0}",UserContext.Settings.GuiState.CalendarOptions.CalendarTextType);
                var calendarContent =
                    PluginsManager.Instance.GetCalendarDayContent(
                        UserContext.Settings.GuiState.CalendarOptions.CalendarTextType);
                if (calendarContent != null)
                {
                    tsCalendarView.Text = string.Format(ApplicationStrings.StatusBarCalendarViewText,calendarContent.Name);
                }
                
                foreach (XtraTabPage tabPage in tcMainTabControl.TabPages)
                {
                    if (tabPage.Controls.Count > 0)
                    {
                        IMainTabControl control = (IMainTabControl) tabPage.Controls[0];
                        Log.WriteVerbose("Fill tab: {0}",tabPage.Text);
                        control.Fill();
                    }
                }
            }
        }

        private void setMainWindowTitle()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(setMainWindowTitle));
            }
            else
            {
                Log.WriteVerbose("setMainWindowTitle");
                if (UserContext.SessionData != null && UserContext.LoginStatus == LoginStatus.Logged)
                {
                    Text = string.Format(ApplicationStrings.MainWindowTitle, Constants.ApplicationName, UserContext.CurrentProfile.UserName);
                    sbpLogin.Text = UserContext.CurrentProfile.UserName;
                }
                else
                {
                    Text = string.Format(ApplicationStrings.MainWindowTitle, Constants.ApplicationName, ApplicationStrings.NotLoggedText);
                    sbpLogin.Text = ApplicationStrings.NotLoggedText;
                }
            }
            
        }

        private void mnuShowToolTips_Click(object sender, EventArgs e)
        {
            mnuShowToolTips.Checked = !mnuShowToolTips.Checked;
            Log.WriteVerbose("mnuShowToolTips_Click: {0}", mnuShowToolTips.Checked);
            UserContext.Settings.GuiState.ShowToolTips = mnuShowToolTips.Checked;
            this.defaultToolTipController1.DefaultController.Active = UserContext.Settings.GuiState.ShowToolTips;
        }

        private void mnuAbout_Click(object sender, EventArgs e)
        {
            AboutWindow dlg = new AboutWindow();
            dlg.ShowDialog(this);
        }

        private void mnuLangPolish_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem itemLang = (ToolStripMenuItem)sender;
            if (itemLang.Checked)
            {//if user select language which is currently selected then simply return
                return;
            }
            Log.WriteVerbose("mnuLangPolish_Click");
            changeLanguage(itemLang.Tag.ToString());
            foreach (ToolStripMenuItem menuItem in mnuLanguage.DropDownItems)
            {
                menuItem.Checked = false;
            }
            itemLang.Checked = true;
        }

        private void changeLanguage(string lang)
        {
            Log.WriteVerbose("changeLanguage: {0}",lang);
            var language =new CultureInfo(lang);
            Thread.CurrentThread.CurrentUICulture = language;
            UserContext.Settings.GuiState.Language = lang;
            FMMessageBox.ShowInfo(ApplicationStrings.InfoChangeLanguageRestart);
         
        }

        void selectLanguage(CultureInfo info)
        {
            //first we need to get neutral culture (en) not specific one(en-US) because languages menu contains neutral items
            CultureInfo lang = info.IsNeutralCulture ? info : info.Parent;
            foreach (ToolStripMenuItem menuItem in mnuLanguage.DropDownItems)
            {
                if (menuItem.Tag.ToString() == lang.Name)
                {
                    menuItem.Checked = true;
                    return;
                }
            }
        }

        private void mnuLogStandard_Click(object sender, EventArgs e)
        {
            showLogFile(UserContext.Settings.StandardLogFile,false);
        }

        private void mnuLogExceptions_Click(object sender, EventArgs e)
        {
            showLogFile(UserContext.Settings.ExceptionsLogFile,true);
        }

        void showLogFile(string logFile,bool errorLog)
        {
            try
            {
                LogViewer dlg = new LogViewer(logFile,errorLog);
                dlg.ShowDialog();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex,ApplicationStrings.ErrorCannotShowLogFile,ErrorWindow.EMailReport);
            }
        }

        IMainTabControl SelectedTabPageControl
        {
            get { return (IMainTabControl)tcMainTabControl.SelectedTabPage.Controls[0]; }
        }

        private void mnuViewLastEntry_Click(object sender, EventArgs e)
        {
            var ctrl = SelectedTabPageControl as usrCalendarView;
            if (ctrl != null)
            {
                ctrl.GoToTheLastEntry();
            }
        }

        private void mnuViewCurrentMonth_Click(object sender, EventArgs e)
        {
            var ctrl = SelectedTabPageControl as usrCalendarView;
            if (ctrl != null)
            {
                ctrl.SetActiveMonth(DateTime.Now);
            }
        }

        private void mnuViewFirstEntry_Click(object sender, EventArgs e)
        {
            var ctrl = SelectedTabPageControl as usrCalendarView;
            if (ctrl != null)
            {
                ctrl.GoToTheFirstEntry();
            }
        }

        private void mnuEditCopy_Click(object sender, EventArgs e)
        {
            usrCalendarView1.Copy();
        }

        private void mnuEditPaste_Click(object sender, EventArgs e)
        {
            usrCalendarView1.Paste();
        }

        private void mnuEditCut_Click(object sender, EventArgs e)
        {
            usrCalendarView1.Cut();
        }

        private void editToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            
            mnuEditPaste.Enabled =SelectedTabPageControl == usrCalendarView1 && usrCalendarView1.CanPaste;
            mnuEditCopy.Enabled = SelectedTabPageControl == usrCalendarView1 && usrCalendarView1.CanCopy;
            mnuEditCut.Enabled = SelectedTabPageControl == usrCalendarView1 && usrCalendarView1.CanCut;
        }


        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OptionsWindow dlg = new OptionsWindow();
            if(dlg.ShowDialog(this)==System.Windows.Forms.DialogResult.OK)
            {
                Fill();
            }
        }

        private void mnuBMI_Click(object sender, EventArgs e)
        {
            BMIWindow wnd = new BMIWindow();
            wnd.ShowDialog(this);
        }

        private void mnuLogout_Click(object sender, EventArgs e)
        {
            UserContext.Logout();
        }

        private void mnuLogin_Click(object sender, EventArgs e)
        {
            login();
        }

        private void mnuTools_DropDownOpening(object sender, EventArgs e)
        {
            mnuLogin.Visible = UserContext.LoginStatus == LoginStatus.NotLogged;
            mnuLogout.Visible = UserContext.LoginStatus == LoginStatus.Logged;
            this.mnuProfileEdit.Enabled = UserContext.SessionData != null;
        }

        public void ShowUserInformation(UserDTO user)
        {
            if (user != null && !user.IsDeleted)
            {
                ShowUserInformation(user.UserName);
            }
        }

        public void ShowUserInformation(string userName)
        {
            tcMainTabControl.SelectedTabPage = tpPeople;
            usrProfilesBrowser1.Fill(userName);
        }

        public XtraTabPage AddTabPage(IMainTabControl userControl, string title, Image icon,bool show=false)
        {
            Control ctrl = (Control) userControl;
            XtraTabPage tab = new XtraTabPage();
            tab.ShowCloseButton = DefaultBoolean.False;
            tab.Text = title;
            tab.Image = icon;
            ctrl.Dock = DockStyle.Fill;
            tab.Controls.Add(ctrl);
            tcMainTabControl.TabPages.Add(tab);
            if (show)
            {
                tcMainTabControl.SelectedTabPage = tab;
            }
            return tab;
        }

        private void sbpLogin_DoubleClick(object sender, EventArgs e)
        {
            UserContext.Logout();
        }

        private void tcMainTabControl_CloseButtonClick(object sender, EventArgs e)
        {
            ClosePageButtonEventArgs closeArgs = (ClosePageButtonEventArgs)e;
            XtraTabPage page = (XtraTabPage)closeArgs.Page;
            tcMainTabControl.TabPages.Remove(page);
        }

        private void mnuView_DropDownOpening(object sender, EventArgs e)
        {
            mnuViewFirstEntry.Enabled= SelectedTabPageControl is usrCalendarView;
            mnuViewLastEntry.Enabled = SelectedTabPageControl is usrCalendarView;
            mnuViewCurrentMonth.Enabled = SelectedTabPageControl is usrCalendarView;
        }

        private void mnuViewRefresh_Click(object sender, EventArgs e)
        {
            this.SelectedTabPageControl.RefreshView();
        }

        private void allToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem) sender;
            ControlHelper.OpenUrl((string)item.Tag);
        }

    }

}
