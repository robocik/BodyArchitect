using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;
using AvalonDock;
using AvalonDock.Controls;
using AvalonDock.Layout;
using AvalonDock.Layout.Serialization;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI.Cache;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.SchedulerEngine;
using BodyArchitect.Client.UI.Views;
using BodyArchitect.Client.UI.WindowsSevenIntegration;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Settings;
using BodyArchitect.Shared;
using Microsoft.Windows.Controls.Ribbon;

namespace BodyArchitect.Client.UI.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IBAWindow, IProgressWindow, INotifyPropertyChanged, IWeakEventListener, IHasFloatingPane
    {
        bool saveSizeAndLocation = true;
        private TasksManager tasksManager;
        //public event EventHandler ProfileConfigurationWizardShow;
        private MainWindowViewModel viewModel=new MainWindowViewModel();
        

        public RibbonTab  RibbonHomeTab
        {
            get { return Ribbon.Items.Cast<RibbonTab>().Where(x => x.Name == "HomeTab").SingleOrDefault(); }
        }

        

        #region From BaseWindow

        public virtual void NotifyOfPropertyChange<TProperty>(Expression<Func<TProperty>> property)
        {
            NotifyOfPropertyChange(property.GetMemberInfo().Name);
        }

        public virtual void NotifyOfPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void SaveGuiState()
        {
            if (SaveSizeAndLocation)
            {
                //here we ensure that only one instance of panel with specific ContentId will be added (there is a bug in avalon and sometimes in avalondock.config file we 
                //had the same panel (same ContentId) twice.
                var test = dockManager.Layout.Descendents().OfType<LayoutAnchorable>().GroupBy(x=>x.ContentId).Where(x=>x.Count()>1).ToList();
                foreach (var wrongGroup in test)
                {
                    //Skip(1) means that the first instance of ContentId is good so we skip it and remove the rest instances
                    foreach (var layoutAnchorable in wrongGroup.Skip(1))
                    {
                        try
                        {
                            layoutAnchorable.Close();
                        }
                        catch (Exception)
                        {
                        }
                        
                    }
                }
                try
                {
                    var serializer = new XmlLayoutSerializer(dockManager);

                    using (var stream = new StreamWriter(Constants.AvalonLayoutFile))
                        serializer.Serialize(stream);
                }
                catch (Exception)
                {
#if DEBUG
                    throw;
#endif

                }
                
            }
        }

        protected virtual void LoadGuiState()
        {
            if (UserContext.Current.Settings.GuiState.SaveGUI && File.Exists(Constants.AvalonLayoutFile))
            {
                try
                {
                    //Dictionary<string,int> loadedPanels = new Dictionary<string, int>();
                    var serializer = new XmlLayoutSerializer(dockManager);
                    //here we ensure that only one instance of panel with specific ContentId will be added (there is a bug in avalon and sometimes in avalondock.config file we 
                    //had the same panel (same ContentId) twice.
                    //serializer.LayoutSerializationCallback += (s, args) =>
                    //                                              {
                    //                                                  var contentId = args.Model.ContentId;
                    //                                                  if (contentId != null)
                    //                                                  {

                    //                                                      int count = 0;
                    //                                                      loadedPanels.TryGetValue(contentId, out count);
                    //                                                      if (count > 0)
                    //                                                      {
                    //                                                          args.Cancel = true;
                    //                                                          return;
                    //                                                      }
                    //                                                      loadedPanels.Add(contentId, 1);
                    //                                                  }
                    //                                              };
                    using (var stream = new StreamReader(Constants.AvalonLayoutFile))
                        serializer.Deserialize(stream);
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Default.Process(ex, Strings.MainWindow_ErrCannotRestoreGui, ErrorWindow.EMailReport);
                }
                
            }
        }

        public void CancelTask(CancellationTokenSource cancelSource)
        {
            TasksManager.CancelTask(cancelSource);
        }

        public void SetException(Exception ex)
        {
            TasksManager.SetException(ex);
        }

        public void Fill()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action(Fill));
            }
            else
            {
                viewModel.AccountType = EnumLocalizer.Default.Translate(UserContext.Current.ProfileInformation.Licence.CurrentAccountType);
                SetProgressStatus(UserContext.Current.LoginStatus == LoginStatus.InProgress, UserContext.Current.LoginStatus.ToString());
                setMainWindowTitle();

                //var calendarContent =
                //    PluginsManager.Instance.GetCalendarDayContent(
                //        UserContext.Settings.GuiState.CalendarOptions.CalendarTextType);
                //if (calendarContent != null)
                //{
                //    tsCalendarView.Text = string.Format(Strings.StatusBarCalendarViewText, calendarContent.Name);
                //}
                var layoutPanel = GetAllDocuments();
                if(layoutPanel.Count==0)
                {
                    ShowDashboard();
                }

                if(CurrentView!=null)
                {
                    CurrentView.Fill();
                }
                setCurrentTabInfo();
                //foreach (XtraTabPage tabPage in tcMainTabControl.TabPages)
                //{
                //    if (tabPage.Controls.Count > 0)
                //    {
                //        IMainTabControl control = (IMainTabControl)tabPage.Controls[0];
                //        Log.WriteVerbose("Fill tab: {0}", tabPage.Text);
                //        control.Fill();
                //    }
                //}

                
            }
        }

        private void setMainWindowTitle()
        {
            if (!Dispatcher.CheckAccess())
            {
                UIHelper.BeginInvoke(new Action(setMainWindowTitle), Dispatcher);
            }
            else
            {
                Log.WriteVerbose("setMainWindowTitle");
                if (UserContext.Current.SessionData != null && UserContext.Current.LoginStatus == LoginStatus.Logged)
                {
                    Title = string.Format(Strings.MainWindowTitle, Constants.ApplicationName, UserContext.Current.CurrentProfile.UserName);
                    sbpLogin.Text = UserContext.Current.CurrentProfile.UserName;
                }
                else
                {
                    Title = string.Format(Strings.MainWindowTitle, Constants.ApplicationName, Strings.NotLoggedText);
                    sbpLogin.Text = Strings.NotLoggedText;
                }
            }

        }

        void login()
        {
            
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action(login));
            }
            else
            {
                Log.WriteVerbose("MainWindow.login()");
                LoginWindow dlg = new LoginWindow();
                if (dlg.ShowDialog() == false)
                {
                    Log.WriteInfo("Login cancel");
                    if (UserContext.Current.LoginStatus != LoginStatus.Logged)
                    {//if there is no logged user then close the application
                        Log.WriteVerbose("there is no logged user - close the application");
                        Close();
                    }
                    
                    return;
                }

            }
        }

        protected virtual void LoginStatusChanged(LoginStatus newStatus)
        {
            
            SetProgressStatus(UserContext.Current.LoginStatus == LoginStatus.InProgress, UserContext.Current.LoginStatus.ToString());
            if (newStatus == LoginStatus.Logged && this.IsLoaded)
            {
                Scheduler.Ensure();
                Fill();
            }
            else if (/*newStatus == LoginStatus.LoginFailed ||*/ newStatus == LoginStatus.NotLogged)
            {
                //close all opened tabs
                var layoutPanel = GetAllDocuments();
                foreach (var document in layoutPanel)
                {
                    document.Close();
                }
                //layoutPanel.Children.Clear();
                setMainWindowTitle();
                login();
            }

            if (newStatus == LoginStatus.NotLogged)
            {
                Settings1.Default.AutoLoginPassword = Settings1.Default.AutoLoginUserName = string.Empty;
            }

        }

        //TODO: Changed status progress
        public void SetProgressStatus(bool showProgress, string message, params object[] args)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action<bool, string, object[]>(SetProgressStatus), showProgress, message, args);
            }
            else
            {
                statusProgressIndicator.ShowProgress = showProgress;
                statusProgressIndicator.Message = string.Format(message, args);
                //if(showProgress)
                //    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);
                //else
                //    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
            }

        }


        public bool SaveSizeAndLocation
        {
            get
            {
                return saveSizeAndLocation;
            }
            set
            {
                saveSizeAndLocation = value;
            }
        }

        protected void SaveProcess()
        {
            //if (GuiState.Default.WindowsState == null)
            //{
            //    Hashtable table = new Hashtable();
            //    GuiState.Default.WindowsState = table;
            //}
            //GuiState.Default.WindowsState[form.Name + "Size"] = form.Size;
            //GuiState.Default.WindowsState[form.Name + "WindowState"] = form.WindowState;

            //GuiState.Default.WindowsState[form.Name + "Location"] = form.Location;
        }

        protected void LoadProcess()
        {
            //if (GuiState.Default.WindowsState != null)
            //{
            //    Hashtable dict = GuiState.Default.WindowsState;
            //    if (form.FormBorderStyle == FormBorderStyle.Sizable && GuiState.Default.WindowsState.ContainsKey(form.Name + "Size"))
            //    {
            //        form.Size = (Size)dict[form.Name + "Size"];
            //    }
            //    if (GuiState.Default.WindowsState.ContainsKey(form.Name + "WindowState"))
            //    {
            //        form.WindowState = (FormWindowState)dict[form.Name + "WindowState"];
            //    }
            //    if (GuiState.Default.WindowsState.ContainsKey(form.Name + "Location"))
            //    {
            //        form.Location = (Point)dict[form.Name + "Location"];
            //    }
            //}
        }

        void UserContext_LoginStatusChanged(object sender, LoginStatusEventArgs e)
        {
            if (TasksManager == null)
            {
                return;
            }
            SynchronizationContext.Send(delegate
            {
                LoginStatusChanged(e.Status);
            }, null);
        }

        void BaseWindow_Loaded(object sender, RoutedEventArgs e)
        {
            rssReader.Fill();

            foreach (var plugin in PluginsManager.Instance.Modules)
            {
                plugin.CreateRibbon(Ribbon);
            }
            statusProgressIndicator.Connect(TasksManager);
            if (!Helper.IsDesignMode)
            {
                LoadProcess();
                LoadGuiState();
            }
            ShowDashboard();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            cancelCurrentTask();
            SaveProcess();
            SaveGuiState();
            UserContext.Current.LoginStatusChanged -= new EventHandler<LoginStatusEventArgs>(UserContext_LoginStatusChanged);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            tasksManager = new TasksManager(new DispatcherSynchronizationContext(Dispatcher));
            UserContext.Current.LoginStatusChanged += new EventHandler<LoginStatusEventArgs>(UserContext_LoginStatusChanged);

            
        }

        #region Thread operation

        [Browsable(false)]
        public SynchronizationContext SynchronizationContext
        {
            get { return TasksManager.SynchronizationContext; }
        }

        public void ThreadSafeClose(bool? result)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action<bool?>(ThreadSafeClose), result);
            }
            else
            {
                DialogResult = result;
                Close();
            }
        }


        private CancellationTokenSource tokenSource;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TasksManager TasksManager
        {
            get
            {
                return tasksManager;
            }
        }


        public Task LogUnhandledExceptions(Task task, Action<OperationContext> exceptionCallback, OperationContext operationContext)
        {
            task.ContinueWith(delegate(Task t)
            {
                Helper.EnsureThreadLocalized();
                if (exceptionCallback != null)
                {
                    Dispatcher.Invoke(new Action<OperationContext>(delegate(OperationContext context)
                    {
                        exceptionCallback(context);
                    }), operationContext);
                }
                else
                {
                    Dispatcher.Invoke(new Action<Task>(delegate(Task ta)
                    {
                        throw ta.Exception;
                    }), t);
                }

            }, TaskContinuationOptions.OnlyOnFaulted);
            return task;
        }

        public virtual CancellationTokenSource RunAsynchronousOperation(Action<OperationContext> method, Action<OperationContext> operationStateCallback = null, Action<OperationContext> exceptionCallback = null)
        {
            //cancelCurrentTask();
            tokenSource = TasksManager.RunAsynchronousOperation(method, operationStateCallback,
                                                                exceptionCallback);

            return tokenSource;
        }

        private void cancelCurrentTask()
        {
            if (tokenSource != null)
            {
                tokenSource.Cancel();
                tokenSource = null;
            }
        }

        #endregion

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            PropertyChangedEventManager.AddListener(UserContext.Current, this, string.Empty);
            DataContext = viewModel;
            Loaded += BaseWindow_Loaded;
            UIHelper.MainWindow = this;
            this.lstPerformance.ItemsSource = performanceEntries;
            LoadProcess();
            Closing += new CancelEventHandler(MainWindow_Closing);
            //fillCalendarView();
            //mnuShowToolTips.Checked = UserContext.Settings.GuiState.ShowToolTips;
            //splitContainerControl1.SplitterPosition = UserContext.Settings.GuiState.NewsPanelSize;
            //selectLanguage(Thread.CurrentThread.CurrentUICulture);
            //TThumbnailHelper.Initialize(this, layoutPanel, dockManager);
            //JumplistHelper.Initialize();
            //TaskbarProgressWrapper.Initialize(this);
            setTutorialLinks();
            localizeRibbon();
        }

        void localizeRibbon()
        {
            FieldInfo pi;

            pi = typeof(Microsoft.Windows.Controls.Ribbon.RibbonContextMenu).GetField("AddToQATText", (BindingFlags.NonPublic | BindingFlags.Static));
            pi.SetValue(null, "RibbonContext_AddToQATText".TranslateStrings());
            pi = typeof(Microsoft.Windows.Controls.Ribbon.RibbonContextMenu).GetField("RemoveFromQATText", (BindingFlags.NonPublic | BindingFlags.Static));
            pi.SetValue(null, "RibbonContext_RemoveFromQATText".TranslateStrings());
            pi = typeof(Microsoft.Windows.Controls.Ribbon.RibbonContextMenu).GetField("ShowQATAboveText", (BindingFlags.NonPublic | BindingFlags.Static));
            pi.SetValue(null, "RibbonContext_ShowQATAboveText".TranslateStrings());
            pi = typeof(Microsoft.Windows.Controls.Ribbon.RibbonContextMenu).GetField("ShowQATBelowText", (BindingFlags.NonPublic | BindingFlags.Static));
            pi.SetValue(null, "RibbonContext_ShowQATBelowText".TranslateStrings());
            pi = typeof(Microsoft.Windows.Controls.Ribbon.RibbonContextMenu).GetField("MaximizeTheRibbonText", (BindingFlags.NonPublic | BindingFlags.Static));
            pi.SetValue(null, "RibbonContext_MaximizeTheRibbonText".TranslateStrings());
            pi = typeof(Microsoft.Windows.Controls.Ribbon.RibbonContextMenu).GetField("MinimizeTheRibbonText", (BindingFlags.NonPublic | BindingFlags.Static));
            pi.SetValue(null, "RibbonContext_MinimizeTheRibbonText".TranslateStrings());
        }
        void logout()
        {
            bool res = ensureModifiedViews();
            if (res == false)
            {
                return;
            }
            UserContext.Current.Logout();
        }

        bool ensureModifiedViews()
        {
            var isModified =GetAllDocuments().Select(x => x.Content).OfType<Frame>().Select(x=>x.Content).OfType<IControlView>().Where(x => x.IsModified).Count() > 0;

            if (isModified && BAMessageBox.AskYesNo(EnumLocalizer.Default.GetStringsString("Message_MainWindow_AskForCancelModifiedContent_CloseApp")) == MessageBoxResult.No)
            {
                return false;
            }
            return true;
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            bool res = ensureModifiedViews();
            if(res==false)
            {
                e.Cancel = true;
            }
        }

        private void setTutorialLinks()
        {
            //mnuHelpTutorialAll.Tag = ControlHelper.TutorialAll;
            //mnuHelpTutorialConfigureProfile.Tag = ControlHelper.TutorialCreateProfile;
            //mnuHelpTutorialWorkoutLog.Tag = ControlHelper.TutorialWorkoutLog;
            //mnuHelpTutorialUsers.Tag = ControlHelper.TutorialUsers;
            //mnuHelpTutorialWorkoutPlans.Tag = ControlHelper.TutorialWorkoutPlans;
        }

        public void StartupMainWindow(object sender, EventArgs e)
        {
            Application.Current.MainWindow = this;
            Show();
            Fill();
        }

        private void btnLogout(object sender, RoutedEventArgs e)
        {
            logout();
        }
        
        private void sbpLogin_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                logout();
            }
        }

        public void AddLayoutAnchorable(LayoutAnchorable panel,AnchorableShowStrategy whereToAdd)
        {
            panel.AddToLayout(dockManager, AnchorableShowStrategy.Left);
        }

        public LayoutAnchorable GetAnchorable(string contentId)
        {
            var test= dockManager.Layout.Descendents()
                       .OfType<LayoutAnchorable>()
                       .Where(x => x.ContentId == contentId).ToList();
            return test.FirstOrDefault();
        }

        public void EnsureAnchorable(string title,string icon,string contentId,AnchorableShowStrategy whereToAdd)
        {
            var commentsPane = GetAnchorable(contentId);
            if (commentsPane == null)
            {
                commentsPane = new LayoutAnchorable();
                commentsPane.IconSource = icon.ToBitmap();
                commentsPane.Title = title;
                commentsPane.CanClose = false;
                commentsPane.AutoHideWidth = 100;

                commentsPane.ContentId = contentId;

                AddLayoutAnchorable(commentsPane, whereToAdd);
            }
        }

        public void EnsureVisible(string panelId)
        {
            var progressPane = MainWindow.Instance.GetAnchorable(panelId);
            if (progressPane != null)
            {
                if (progressPane.IsHidden)
                    progressPane.Show();
                else if (progressPane.IsVisible)
                    progressPane.IsActive = true;

                if(progressPane.IsHidden)
                {//this should never happens
                    if (progressPane.Parent != null)
                    {
                        progressPane.Parent.RemoveChild(progressPane);
                    }
                    AddLayoutAnchorable(progressPane, AnchorableShowStrategy.Right);
                    progressPane.IsActive = true;
                }
            }
        }

        public LayoutDocument GetCurrentDocument()
        {
            var test =
                dockManager.Layout.Descendents().OfType<LayoutDocument>().FirstOrDefault(x => x.Content == dockManager.ActiveContent);
            if (test != null)
            {
                return test;
            }
            if (dockManager.Layout.LastFocusedDocument is LayoutDocument)
            {
                return dockManager.Layout.Descendents().OfType<LayoutDocument>().FirstOrDefault(x => x == dockManager.Layout.LastFocusedDocument);
            }
            return dockManager.Layout.Descendents().OfType<LayoutDocument>().FirstOrDefault(x => x.Content is Frame && ((Frame)x.Content).Content == SelectedView);
        }


        public LayoutDocumentPane GetCurrentDocumentPane()
        {
            var currentDoc=GetCurrentDocument();
            if (currentDoc != null)
            {
                return (LayoutDocumentPane) currentDoc.Parent;
            }
            var test =
                dockManager.Layout.Descendents()
                           .OfType<LayoutDocumentPane>()
                           .FirstOrDefault();
            return test;
        }

        public IList<LayoutDocument> GetAllDocuments()
        {

            var test =dockManager.Layout.Descendents().OfType<LayoutDocument>().Where(x => x.Content is Frame).ToList();
            return test;
        }

        bool askedAboutModification = false;

        public void ShowPage(Uri pageUri, Func<PageContext> contextFactory = null, bool showInNewTab = false, bool selectNewTab = true)
        {
            try
            {

                Cursor = Cursors.Wait;
                LayoutDocumentPane currentLayoutPanel = GetCurrentDocumentPane();
                LayoutDocument currentDocument = GetCurrentDocument();
                askedAboutModification = false;
                //if user has docked layoutanchorable as document then we can't open new view in the same tab so we always open as a new
                bool withControl = Keyboard.Modifiers == ModifierKeys.Control || showInNewTab 
                    || (currentLayoutPanel != null && currentLayoutPanel.SelectedContent is LayoutAnchorable);
                PageContext context = null;
                if (contextFactory != null)
                {
                    context = contextFactory();
                }
                Frame frame = null;
                if (withControl || currentDocument == null || !(currentDocument.Content is Frame))
                {
                    frame = new Frame();
                    frame.NavigationUIVisibility = NavigationUIVisibility.Hidden;

                    frame.LoadCompleted += (a, e) =>
                    {
                        var page = e.Content as BasePage;
                        var ctx = (PageContext)e.ExtraData;
                        if (page != null && ctx != null)
                        {
                            page.PageContext = ctx;
                        }

                        page.Fill();
                        JournalEntry.SetName(page, page.Header);
                        
                    };
                    frame.Navigated += (a, e) =>
                    {
                    };
                    frame.ContentRendered += (a, e) =>
                    {
                        setCurrentTabInfo();
                        //askedAboutModification = false;
                    };
                    frame.Navigating += (a, e) =>
                    {
                        var modifyControl = ((Frame)a).Content as IControlView;
                        if (!askedAboutModification)
                        {//ask about modification only if we didn't do this (only one message box should be shown)
                            askForCancelModifiedContent(e, modifyControl);
                            askedAboutModification = false;
                        }
                    };
                }
                else
                {
                    frame = (Frame)currentDocument.Content;
                }


                var index = currentLayoutPanel.Children.Count;
                LayoutDocument doc = new LayoutDocument();
                doc.Content = frame;

                if (currentDocument != null)
                {
                    //check if current view is modified. If yes then ask the user first!
                    //it is not necessary to ask because the same question will be shown in Navigating event
                    var modifyControl = frame.Content as IControlView;

                    if (!withControl && modifyControl != null && modifyControl.IsModified)
                    {
                        if (BAMessageBox.AskWarningYesNo(EnumLocalizer.Default.GetStringsString("Message_MainWindow_ShowPage_ModifiedView")) ==MessageBoxResult.No)
                        {
                            return;
                        }
                        askedAboutModification = true;
                        //when we want to skip changes we must set IsModified to false because in Navigating event we will display the same question
                        modifyControl.IsModified = false;
                    }
                    if (!withControl)
                    {
                        index = currentLayoutPanel.IndexOfChild(currentLayoutPanel.SelectedContent);
                        currentLayoutPanel.RemoveChildAt(index);
                    }
                }

                currentLayoutPanel.InsertChildAt(index, doc);
                currentLayoutPanel.SelectedContentIndex = -1;
                currentLayoutPanel.SelectedContentIndex = index;

                frame.NavigationService.Navigate(pageUri, context);
            }
            finally
            {
                Cursor = Cursors.Arrow;
            }
        }

        
        private static void askForCancelModifiedContent(CancelEventArgs e, IControlView modifyControl)
        {
            if (modifyControl != null && modifyControl.IsModified &&
                BAMessageBox.AskWarningYesNo(EnumLocalizer.Default.GetStringsString("Message_MainWindow_askForCancelModifiedContent")) ==
                MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        public IControlView CurrentView
        {
            get
            {
                var layoutPanel = GetCurrentDocument();
                if (layoutPanel != null)
                {
                    var ctrl = (IControlView)((Frame)layoutPanel.Content).Content;
                    return ctrl;
                }
                return null;
            }
        }
        #region Implementation of IProgressWindow

        public void SetMessage(string message)
        {

        }

        public void SetProgress(int value)
        {
            UIHelper.BeginInvoke(()=>
                                     {
                                         busyProgress.Value = value;
                                     },Dispatcher);
            
        }

        public MethodParameters Parameters
        {
            get; set; }

        public static MainWindow Instance
        {
            get
            {
                return (MainWindow)Application.Current.MainWindow;
            }
        }

        

        public void ThreadSafeClose()
        {
            Dispatcher.BeginInvoke(new Action(delegate
            {
                pleaseWait.IsBusy = false;
            }));
        }

        public void ShowProgress(bool canBeCancelled, int? maxProgress)
        {
            pleaseWait.IsBusy = true;
            busyProgress.Value = 0;
            busyProgress.IsIndeterminate = maxProgress==null;
            btnProgressCancel.SetVisible(canBeCancelled);
            if (maxProgress != null)
            {
                busyProgress.Maximum = maxProgress.Value;
            }
        }

        #endregion

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if(CurrentView!=null)
            {
                CurrentView.RefreshView();
            }
        }

        private void rbtnNewCalendar_Click(object sender, RoutedEventArgs e)
        {
            ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.UI;component/Views/Calendar/NewCalendarView.xaml"));
        }

        private void rbtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            ShowDashboard();
        }

        private void rbtnWilksCalculator_Click(object sender, RoutedEventArgs e)
        {
            if (!UIHelper.EnsurePremiumLicence())
            {
                return;
            }
            WilksCalculatorWindow dlg = new WilksCalculatorWindow();
            dlg.ShowDialog();
        }

        

        public void ShowUserInformation(UserDTO user)
        {
            ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.UI;component/Views/PeopleView.xaml"), () =>
            {
                if (user != null)
                {
                    return new PageContext(user,null);
                }
                return null;
            });
        }

        public void ShowTrainingDayReadOnly(DateTime trainingDate,UserDTO user,Guid? customerId=null,IEntryObjectBuilderProvider builder=null)
        {
            if (user.IsDeleted || (!user.IsMe() && !user.HaveAccess(user.Privacy.CalendarView)))
            {
                BAMessageBox.ShowInfo(EnumLocalizer.Default.GetStringsString("MainWindow_ErrPrivateCalendar"));
                return;
            }

            TrainingDayDTO trainingday = null;
            PleaseWait.Run(delegate(MethodParameters arg)
            {
                try
                {
                    //we check pressing Ctrl button here because below we send request to the server and user could release this key before the request is completed (and training would be open in the same tab)
                    bool showInNewTab = Keyboard.Modifiers == ModifierKeys.Control;

                    WorkoutDayGetOperation getParam = new WorkoutDayGetOperation();
                    getParam.Operation = GetOperation.Current;
                    getParam.WorkoutDateTime = trainingDate;
                    getParam.UserId = user.GlobalId;
                    getParam.CustomerId = customerId;
                    trainingday = ServiceManager.GetTrainingDay(getParam);
                    bool canCreateNew = user.IsMe();
                    if (trainingday == null)
                    {
                        if(canCreateNew)
                        {
                            trainingday = new TrainingDayDTO(getParam.WorkoutDateTime.Value, user.GlobalId);
                            trainingday.CustomerId = customerId;
                        }
                        else
                        {
                            arg.CloseProgressWindow();

                            UIHelper.Invoke(() => BAMessageBox.ShowError(EnumLocalizer.Default.GetStringsString("MainWindow_ErrTrainingDayDoesntExist")), Dispatcher);
                            return;
                        }
                        
                    }
                    CustomerDTO customer = null;
                    if (customerId.HasValue)
                    {
                        customer = CustomersReposidory.Instance.GetItem(customerId.Value);
                    }

                    UIHelper.BeginInvoke(() => MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.UI;component/Windows/TrainingDayWindow.xaml"), () =>
                    {
                        var view = DomainObjectOperatonHelper.CreateTrainingDayWindow(trainingday, user, customer, builder);
                        return view;
                    }, showInNewTab), Dispatcher);

                }
                catch (Exception ex)
                {
                    arg.CloseProgressWindow();
                    UIHelper.Invoke(() => ExceptionHandler.Default.Process(ex, EnumLocalizer.Default.GetStringsString("MainWindow_ErrShowUserWorkoutLog"), ErrorWindow.EMailReport), Dispatcher);
                }


            });
        }

        public void ShowDashboard(Type userCtrl = null, bool showInNewTab = false, bool selectNewTab = true)
        {
            ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.UI;component/Views/DashboardView.xaml"),()=>
            {
                if(userCtrl!=null)
                {
                    return new DashboardPageContext(){ControlToShow = userCtrl};
                }
                return null;
            }, showInNewTab,selectNewTab);
        }

        private void rbtnPeople_Click(object sender, RoutedEventArgs e)
        {
            ShowUserInformation(null);
        }

        private void rbtnOptions_Click(object sender, RoutedEventArgs e)
        {
            OptionsWindow dlg = new OptionsWindow();
            if (dlg.ShowDialog() ==true)
            {
                Fill();
            }
        }

        private void rbtnStandardLog_Click(object sender, RoutedEventArgs e)
        {
            showLogFile(false);
        }

        private void rbtnErrorLog_Click(object sender, RoutedEventArgs e)
        {
            showLogFile( true);
        }

        void showLogFile( bool errorLog)
        {
            ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.UI;component/Views/LogView.xaml"),()=> new LogPageContext(errorLog));
        }

        private IControlView selectedView;

        public IControlView SelectedView
        {
            get { return selectedView; }
            set
            {
                if(selectedView!=value)
                {
                    selectedView = value;
                    NotifyOfPropertyChange(()=>SelectedView);
                }
            }
        }

        private bool isUpdatingAnchorables = false;

        public void UpdateAnchorables()
        {
            if (isUpdatingAnchorables)
            {
                return;
            }

            isUpdatingAnchorables = true;

            try
            {
                var panels = dockManager.Layout.Descendents().OfType<LayoutAnchorable>().ToList();
                foreach (var layoutAnchorable in panels)
                {
                    var itemPanels = SelectedView as IHasFloatingPane;
                    Control contentControl = this.GetContentForPane(layoutAnchorable.ContentId);
                    if (itemPanels != null && contentControl == null)
                    {
                        contentControl = itemPanels.GetContentForPane(layoutAnchorable.ContentId);
                    }
                    if (contentControl == null && SelectedView != null)
                    {
                        var entryObject = UIHelper.FindChild<IHasFloatingPane>((DependencyObject)SelectedView);
                        foreach (var hasFloatingPane in entryObject)
                        {
                            contentControl = hasFloatingPane.GetContentForPane(layoutAnchorable.ContentId);
                            if (contentControl != null)
                            {
                                break;
                            }
                        }
                    }
                    var group = layoutAnchorable.Parent as ILayoutGroup;
                    //sometimes this method has been invoked when the avalon dock is already changing ObservableCollection and we got exception.
                    //so i've added property IsChanging to avalon dock panels classes and we can only operate if this property is false (so observablecollection is not changed yet)
                    bool isAlreadyChanging = group != null && group.IsChanging;
                    if (layoutAnchorable.Content != contentControl && !isAlreadyChanging)
                    {

                        layoutAnchorable.Content = contentControl;
                        if (contentControl == null && !layoutAnchorable.IsHidden)
                        {
                            layoutAnchorable.Content = null;
                            layoutAnchorable.Hide(false, false);
                            //var anchorAsDocument=layoutAnchorable.Parent as LayoutDocumentPane;
                            //if (anchorAsDocument!=null)
                            //{//here we close document with layout anchorable docked as document
                            //    //without this new thread we get exception
                            //    layoutAnchorable.Hide(false,false);
                            //    //ThreadPool.QueueUserWorkItem((g)=> UIHelper.BeginInvoke(
                            //    //    delegate
                            //    //        {

                            //    //            ((LayoutAnchorable)g).Hide(false, false);
                            //    //        }, Dispatcher), layoutAnchorable);
                            //}
                            //else
                            //{
                            //    layoutAnchorable.Hide(false, false);    
                            //}
                        }
                        else if (contentControl != null && !layoutAnchorable.IsDisabled && layoutAnchorable.IsHidden)
                        {
                            layoutAnchorable.Show(false, false);


                            if (layoutAnchorable.IsHidden)
                            {//this should never happens but because of some bug in restoring state (see my comment and fix in LayoutRoot.CollectGarbage) some panels could be still hidden (my fix in collectgarbage should fix this problem but these code is as a additional level of protection)
                                if (layoutAnchorable.Parent != null)
                                {
                                    layoutAnchorable.Parent.RemoveChild(layoutAnchorable);
                                }
                                AddLayoutAnchorable(layoutAnchorable, AnchorableShowStrategy.Right);

                            }
                        }
                    }
                    else if (!isAlreadyChanging && layoutAnchorable.Content == null && layoutAnchorable.IsVisible)
                    {//cannot show panel without any content
                        layoutAnchorable.Hide(false, false);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex, "Something happened with your floating panes. To fix this problem we had to restore a default layout. Save your work and restart the application to finish repairing.",ErrorWindow.EMailReport);
                try
                {
                    MainWindow.Instance.SaveSizeAndLocation = false;
                    if (File.Exists(Constants.AvalonLayoutFile))
                    {
                        File.Delete(Constants.AvalonLayoutFile);
                    }
                }
                catch
                {
                }
            }
            finally
            {
                isUpdatingAnchorables = false;    
            }
            

            
        }

        //private void setCurrentTabInfo()
        //{
        //    var item = dockManager.ActiveContent as Frame;
        //    if (item !=null)
        //    {
        //        SelectedView =(IControlView) ((Frame)item).Content;
        //    }
        //    var layoutDocuments = GetCurrentDocument();
        //    viewModel.CanGoBack = viewModel.CanGoForward = false;
        //    if (layoutDocuments != null)
        //    {
        //        //var item = dockManager.ActiveContent as IControlView;
        //        //var frame = dockManager.ActiveContent as Frame;
        //        var frame = layoutDocuments.Content as Frame;

        //        if (frame != null)
        //        {
        //            viewModel.CanGoForward = frame.CanGoForward;
        //            viewModel.CanGoBack = frame.CanGoBack;
        //            viewModel.CurrentTabBackHistory.Clear();
        //            viewModel.CurrentTabForwardHistory.Clear();
        //            if (frame.CanGoBack)
        //            {
        //                foreach (JournalEntry entry in frame.BackStack)
        //                {
        //                    viewModel.CurrentTabBackHistory.Add(new ListItem<JournalEntry>(entry.Name, entry));
        //                }
        //            }
        //            if (frame.CanGoForward)
        //            {
        //                foreach (JournalEntry entry in frame.ForwardStack)
        //                {
        //                    viewModel.CurrentTabForwardHistory.Add(new ListItem<JournalEntry>(entry.Name, entry));
        //                }
        //            }

        //        }

        //        if (SelectedView != null)
        //        {
        //            sbAccountTypeFeature.Visibility = (UserContext.Current.ProfileInformation.Licence.CurrentAccountType < SelectedView.AccountType ? Visibility.Visible : Visibility.Collapsed);

        //            if (SelectedView.Customer != null)
        //            {
        //                sbCurrentUserViewText.Text = SelectedView.Customer.FullName;
        //            }
        //            else
        //            {
        //                var user = SelectedView.User;
        //                if (user == null)
        //                {
        //                    user = UserContext.Current.CurrentProfile;
        //                }
        //                if (user != null)
        //                {
        //                    sbCurrentUserViewText.Text = user.UserName;
        //                }
        //                else
        //                {
        //                    sbCurrentUserViewText.Text = EnumLocalizer.Default.GetStringsString("Message_MainWindow_setCurrentTabInfo_NotLogged");
        //                }
        //            }

        //            UpdateAnchorables();
        //        }




        //    }
        //}

        private void setCurrentTabInfo()
        {
            var layoutDocuments = GetCurrentDocument();
            viewModel.CanGoBack = viewModel.CanGoForward = false;
            if (layoutDocuments != null)
            {
                //var item = dockManager.ActiveContent as IControlView;
                IControlView item = null;
                //var frame = dockManager.ActiveContent as Frame;
                var frame = layoutDocuments.Content as Frame;

                if (frame != null)
                {
                    item = frame.Content as IControlView;
                    viewModel.CanGoForward = frame.CanGoForward;
                    viewModel.CanGoBack = frame.CanGoBack;
                    viewModel.CurrentTabBackHistory.Clear();
                    viewModel.CurrentTabForwardHistory.Clear();
                    if (frame.CanGoBack)
                    {
                        foreach (JournalEntry entry in frame.BackStack)
                        {
                            viewModel.CurrentTabBackHistory.Add(new ListItem<JournalEntry>(entry.Name, entry));
                        }
                    }
                    if (frame.CanGoForward)
                    {
                        foreach (JournalEntry entry in frame.ForwardStack)
                        {
                            viewModel.CurrentTabForwardHistory.Add(new ListItem<JournalEntry>(entry.Name, entry));
                        }
                    }

                }

                SelectedView = item;
                if (item != null)
                {
                    sbAccountTypeFeature.Visibility = (UserContext.Current.ProfileInformation.Licence.CurrentAccountType < item.AccountType ? Visibility.Visible : Visibility.Collapsed);

                    if (item.Customer != null)
                    {
                        sbCurrentUserViewText.Text = item.Customer.FullName;
                    }
                    else
                    {
                        var user = item.User;
                        if (user == null)
                        {
                            user = UserContext.Current.CurrentProfile;
                        }
                        if (user != null)
                        {
                            sbCurrentUserViewText.Text = user.UserName;
                        }
                        else
                        {
                            sbCurrentUserViewText.Text = EnumLocalizer.Default.GetStringsString("Message_MainWindow_setCurrentTabInfo_NotLogged");
                        }
                    }

                    UpdateAnchorables();
                }
            }
            else
            {//all document closed
                SelectedView = null;
                UpdateAnchorables();
            }
        }

        private void btnClearPerformanceList_Click(object sender, RoutedEventArgs e)
        {
            performanceEntries.Clear();
        }

        private void btnSavePerformanceList_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private ObservableCollection<string> performanceEntries = new ObservableCollection<string>();

        public void AddPerformanceMessage(string message)
        {
            if (!viewModel.ShowPerformanceOutput)
            {
                return;
            }
            UIHelper.BeginInvoke(delegate
                                {
                                    performanceEntries.Add(message);
                                },Dispatcher);
            
        }

        public void ShowNotification(NotifyObjectBase notification)
        {
            PopupNotificationCtrl ctrl = new PopupNotificationCtrl();
            if (myNotifyIcon.CustomBalloon != null)
            {
                //if notification window is displayed then copy all currently displayed items to new popup
                var tmp = (PopupNotificationCtrl)myNotifyIcon.CustomBalloon.Child;
                foreach (var notifyObject in tmp.NotifyContent)
                {
                    if ((DateTime.UtcNow - notification.DateTime).TotalSeconds < 7)
                    {
                        ctrl.NotifyContent.Add(notifyObject);
                    }
                }
                myNotifyIcon.CloseBalloon();
            }
            //latest notification on the top
            ctrl.NotifyContent.Insert(0, notification);
            myNotifyIcon.ShowCustomBalloon(ctrl, PopupAnimation.Slide, 6000);
        }



        private void rbtnMyTrainings_Click(object sender, RoutedEventArgs e)
        {
            ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.UI;component/Views/MyTrainingsView.xaml"));
        }

        private void rbtnNewReporting_Click(object sender, RoutedEventArgs e)
        {
            ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.UI;component/Views/NewReportingView.xaml"));
        }

        private void dockManager_ActiveContentChanged(object sender, EventArgs e)
        {
            setCurrentTabInfo();
        }

        private void DockManager_OnDocumentClosed(object sender, DocumentClosedEventArgs e)
        {
            var c = (Control)e.Document.Content;
            CloseTab(c);
        }
        private void dockManager_DocumentClosing(object sender, AvalonDock.DocumentClosingEventArgs e)
        {
            var modifyControl = ((Frame)e.Document.Content).Content as IControlView;
            askForCancelModifiedContent(e, modifyControl);
            if (!e.Cancel)
            {
                //var c = (Control)e.Document.Content;
                //CloseTab(c);
            }
            //GC.Collect();
        }

        //made public for thumbnail classes
        public void CloseTab(Control control)
        {
            viewModel.CurrentTabBackHistory.Clear();
            viewModel.CurrentTabForwardHistory.Clear();
            viewModel.CanGoForward = viewModel.CanGoBack = false;
            if (dockManager.ActiveContent != control)
            {//this is a specific case when user close document but selected is anchorable. in this case ActiveContentChanged event is not invoked
                var anotherDocument= dockManager.Layout.Descendents().OfType<LayoutDocument>().Where(x=>x.Content!=control).FirstOrDefault();
                dockManager.ActiveContent = anotherDocument;
            }
            //bugFixForAvalonDock(control);
        }

        private void bugFixForAvalonDock(Control c)
        {
            if (c.Parent != null)
            {
                var tmp = (LayoutPanel)((DockingManager)c.Parent).LayoutRootPanel.Model;
                var currentLayoutPanel = (LayoutDocumentPane)((LayoutDocumentPaneGroup)tmp.Children[0]).Children[0];

                var prop = typeof (LayoutDocumentPaneControl).GetField("_logicalChildren",
                                                                       BindingFlags.NonPublic | BindingFlags.Instance);
                var list = (IList<object>)prop.GetValue(currentLayoutPanel);
                if (list.Contains(c))
                {
                    typeof (LayoutDocumentPaneControl).InvokeMember("RemoveLogicalChild",
                                                                    BindingFlags.InvokeMethod | BindingFlags.NonPublic |
                                                                    BindingFlags.Instance, null, currentLayoutPanel,
                                                                    new object[] {c});
                    list.Remove(c);
                }
            }

            //if (c.Parent == null)
            {
                //closing floating window
                var itemsProp = typeof(DockingManager).GetField("_layoutItems",
                                                                   BindingFlags.NonPublic | BindingFlags.Instance);
                var list1 = (List<AvalonDock.Controls.LayoutItem>)itemsProp.GetValue(dockManager);
                var item11 = list1.Where(x => x.Model == c).ToList();
                if (item11.Count > 1)
                {
                    for (int index = item11.Count-1; index >=1; index--)
                    {
                        var layoutItem = item11[index];
                        list1.Remove(layoutItem);
                    }
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            var layoutPanel = GetCurrentDocument();
            if (layoutPanel != null)
            {
                
                var frame = layoutPanel.Content as Frame;
                if (frame != null && frame.CanGoBack)
                {
                    frame.GoBack();
                }
            }
        }

        private void btnBackToEntry_Click(object sender, RoutedEventArgs e)
        {
            goToHistoryEntry((MenuItem) sender);
            //BUG FIX in ribbon
            e.Handled = true;
        }

        void goToHistoryEntry(MenuItem menuItem)
        {
            var layoutPanel = GetCurrentDocument();
            if (layoutPanel != null)
            {
                var frame = layoutPanel.Content as Frame;
                if (frame != null)
                {
                    var pairEntry = menuItem.DataContext as ListItem<JournalEntry>;
                    if (pairEntry != null)
                    {
                        JournalEntry toGoBack = pairEntry.Value;
                        navigateToJournalEntry(toGoBack, frame);
                    }
                }
            }
        }

        void navigateToJournalEntry(JournalEntry entry, Frame frame)
        {//in WPF there is no method to navigate to specific JournalEntry (sic!) so I had to invoke it using relfection
            var prop = frame.GetType().GetField("_ownJournalScope",
                                                                   BindingFlags.NonPublic | BindingFlags.Instance);


            var journalScope = prop.GetValue(frame);
            journalScope.GetType().InvokeMember("NavigateToEntry",
                                                        BindingFlags.InvokeMethod | BindingFlags.NonPublic |
                                                        BindingFlags.Instance, null, journalScope, new object[] { entry });
        }

        private void btnForwardToEntry_Click(object sender, RoutedEventArgs e)
        {
            goToHistoryEntry((MenuItem)sender);
            //BUG FIX in ribbon
            e.Handled = true;
        }

        private void btnForward_Click(object sender, RoutedEventArgs e)
        {
            var layoutPanel = GetCurrentDocument();
            if (layoutPanel != null)
            {
                var frame = layoutPanel.Content as Frame;
                if (frame != null && frame.CanGoForward)
                {
                    frame.GoForward();
                }
            }
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            TaskbarProgressWrapper.UpdateProgressState(TaskbarProgressWrapper.State.Indeterminate);
            MessageBox.Show(TasksManager.StartedTasksCount.ToString());
            TaskbarProgressWrapper.UpdateProgressState(TaskbarProgressWrapper.State.NoProgress);
        }

        private void btnTest1_Click(object sender, RoutedEventArgs e)
        {
            TasksManager.ClearTasks();
        }

        private void rbtnImportLicenceKey_Click(object sender, RoutedEventArgs e)
        {
            ImportLicenceKeyWindow dlg = new ImportLicenceKeyWindow();
            if(dlg.ShowDialog()==true)
            {
                
            }
        }

        private void sbAccountType_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.UI;component/Views/ChangeAccountTypeView.xaml"));
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            UIHelper.BeginInvoke(() => viewModel.AccountType = EnumLocalizer.Default.Translate(UserContext.Current.ProfileInformation.Licence.CurrentAccountType), Dispatcher);
            return true;
        }

        private void sbAccountTypeFeature_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1) //Note that this is a lie, this does not check for a "real" click
            {
                ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.UI;component/Views/ChangeAccountTypeView.xaml"),
                    showInNewTab: true);
            }
        }

        private void rbtnMyPlaces_Click(object sender, RoutedEventArgs e)
        {
            ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.UI;component/Views/MyPlace/MyPlacesView.xaml"));
        }

        private void rbtnLanguageEnglish_Click(object sender, RoutedEventArgs e)
        {
            changeLanguage("en-US");
        }

        private void rbtnLanguagePolish_Click(object sender, RoutedEventArgs e)
        {
            changeLanguage("pl-PL");
        }

        void changeLanguage(string culture)
        {
            try
            {
                UserContext.Current.Settings.GuiState.Language = culture;
                BAMessageBox.ShowInfo(Strings.ResourceManager.GetString("MainWindow_Info_ChangeLanguageRestartNeeded", CultureInfo.GetCultureInfo(culture)));
            }
            catch (Exception)
            {
                
            }
        }

        private void rbtnFeaturedItems_Click(object sender, RoutedEventArgs e)
        {
            ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.UI;component/Views/FeaturedItemsView.xaml"));
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new AboutWindow();
            dlg.ShowDialog();
        }

        private void btnCancelProgress_Click(object sender, RoutedEventArgs e)
        {
            Parameters.Cancel = true;

        }

        //private void DockManager_OnFloatingPaneDropping(object sender, DockingManager.FloatingPaneDroppingEventArgs e)
        //{
        //    var test1 = e.FloatingWindow.Children.First() as LayoutDocument;

        //    if ((e.TargetType == DropTargetType.DocumentPaneDockBottom || e.TargetType == DropTargetType.DocumentPaneDockRight || e.TargetType == DropTargetType.DocumentPaneDockTop || e.TargetType == DropTargetType.DocumentPaneDockLeft || e.TargetType == DropTargetType.DocumentPaneDockInside) 
        //        && test1 == null)
        //    {
        //        BAMessageBox.ShowInfo("You cannot dock this window here!");
        //        e.Cancel = true;
        //    }
        //}

        public Control GetContentForPane(string paneId)
        {
            if(paneId=="RssReader")
            {
                return rssReader;
            }
            return null;
        }
    }

    public class SetTemplateSelector : DataTemplateSelector
    {
 
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is LayoutDocument)
            {
                return (DataTemplate) Application.Current.Resources["DocumentHeaderTemplate"];
            }
            else
            {
                return (DataTemplate) Application.Current.Resources["AnchorableHeaderTemplate"];
            }
        }

    }
}
