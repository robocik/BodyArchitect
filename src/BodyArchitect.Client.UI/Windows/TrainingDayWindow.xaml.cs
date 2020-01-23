using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AvalonDock.Layout;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI.Cache;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.SchedulerEngine;
using BodyArchitect.Client.UI.Social;
using BodyArchitect.Client.UI.UserControls;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Settings;
using BodyArchitect.Shared;

namespace BodyArchitect.Client.UI.Windows
{
    /// <summary>
    /// Interaction logic for TrainingDayWindow.xaml
    /// </summary>
    public partial class TrainingDayWindow : IHasFloatingPane
    {
        ObservableCollection<ImageSourceListItem<Type>> entryItems = new ObservableCollection<ImageSourceListItem<Type>>();
        private TrainingDayDTO day;
        public event CancelEventHandler TrainingDayChanging;
        public event EventHandler<TrainingDayChangedEventArgs> TrainingDayChanged;
        private ObservableCollection<TabItemViewModel> tabs = new ObservableCollection<TabItemViewModel>();
        private FilterType filterType;
        private TrainingDayCommentsControl commentsControl;

        public TrainingDayWindow()
        {
            InitializeComponent();
            DataContext = this;
            xtraTabControl1.ItemsSource = tabs;
            createEntryObjectMenu();
            CanAdd=CanSave=CanNext = CanPrevious = CanShare = true;
            MainWindow.Instance.EnsureAnchorable(Strings.TrainingDayWindow_Header_Comments, "pack://application:,,,/BodyArchitect.Client.Resources;component/Images/Comments16.png", "TrainingDayCommentsControl", AnchorableShowStrategy.Left);
        }

        public Control GetContentForPane(string paneId)
        {
            if (paneId == "TrainingDayCommentsControl")
            {
                if (commentsControl == null)
                {
                    commentsControl = new TrainingDayCommentsControl();
                    commentsControl.ObjectChanged -= CommentsControlObjectChanged;
                    commentsControl.ObjectChanged += CommentsControlObjectChanged;
                    commentsControl.ReadOnly = ReadOnly;
                    commentsControl.Fill(day);
                }
                
                return commentsControl;
            }
            return null;
        }

        void CommentsControlObjectChanged(object sender, EventArgs e)
        {
            day.AllowComments = commentsControl.AllowComments;
            SetModifiedFlag();
        }

        #region Ribbon

        private bool canAdd;
        private bool canNext;
        private bool canPrevious;
        private bool canSave;
        private bool canShare;
        private bool canRename;
        //private bool showComments;
        

        public bool IsFilterAll
        {
            get { return filterType == FilterType.All;  }
        }

        public bool IsFilterOnlyReservations
        {
            get { return filterType == FilterType.OnlyReservations; }
        }

        public bool IsFilterOnlyNotReservations
        {
            get { return filterType == FilterType.OnlyNotReservations; }
        }

        public bool ShowEditTab
        {
            get { return !ReadOnly; }
        }


        public bool CanAdd
        {
            get { return canAdd; }
            set
            {
                canAdd = value;
                NotifyOfPropertyChange(() => CanAdd);
            }
        }

        public bool CanRename
        {
            get { return canRename; }
            set
            {
                canRename = value;
                NotifyOfPropertyChange(() => CanRename);
            }
        }

        public bool CanShare
        {
            get { return canShare; }
            set
            {
                canShare = value;
                NotifyOfPropertyChange(() => CanShare);
            }
        }

        public bool CanSave
        {
            get { return canSave; }
            set
            {
                canSave = value;
                NotifyOfPropertyChange(() => CanSave);
            }
        }

        public bool CanNext
        {
            get { return canNext; }
            set
            {
                canNext = value;
                NotifyOfPropertyChange(()=>CanNext);
            }
        }

        public bool CanPrevious
        {
            get { return canPrevious; }
            set
            {
                canPrevious = value;
                NotifyOfPropertyChange(() => CanPrevious);
            }
        }

        public IList<ImageSourceListItem<Type>> EntryItems
        {
            get { return entryItems; }
        }
        #endregion

        //public bool FillRequired { get; private set; }

        private bool ReadOnly { get; set; }

        public TrainingDayDTO CurrentDay
        {
            get
            {
                return day;
            }
        }

        public ObservableCollection<TabItemViewModel> Tabs
        {
            get { return tabs; }
        }

        protected IEntryObjectBuilderProvider EntryObjectBuilder { get; private set; }


        private void onTrainingDayChanging(CancelEventArgs e)
        {
            if (TrainingDayChanging != null)
            {
                TrainingDayChanging(this, e);
            }
        }

        private void onTrainingDayChanged(TrainingDayChangedEventArgs e)
        {
            if (TrainingDayChanged != null)
            {
                TrainingDayChanged(this, e);
            }
        }

        void refreshTabs(FilterType filterType, SaveTrainingDayResult saveResult)
        {
            this.filterType = filterType;
            //tsbFilterAll.IsChecked = filterType == FilterType.All;
            //tsbFilterOnlyReservation.IsChecked = filterType == FilterType.OnlyReservations;
            //tsbFilterOnlyNotReservation.IsChecked = filterType == FilterType.OnlyNotReservations;
            tabs.Clear();
            for (int index = day.Objects.Count - 1; index >= 0; index--)
            {
                var entry = day.Objects.ElementAt(index);
                bool showByFilter = filterType == FilterType.All || (filterType == FilterType.OnlyReservations && entry.ReservationId.HasValue) || (filterType == FilterType.OnlyNotReservations && !entry.ReservationId.HasValue);
                if (showByFilter && !createNewEntryControl(entry, false, saveResult))
                {
                    //exception during creating new entry so we delete it from training day
                    day.RemoveEntry(entry);
                }
            }
            NotifyOfPropertyChange(()=>IsFilterAll);
            NotifyOfPropertyChange(() => IsFilterOnlyNotReservations);
            NotifyOfPropertyChange(() => IsFilterOnlyReservations);
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.Wait;
            try
            {
                var menuItem = (MenuItem)sender;
                var pairEntry = (ImageSourceListItem<Type>)menuItem.DataContext;

                EntryObjectInstanceAttribute entryAttribute = new EntryObjectInstanceAttribute();
                var attributes = pairEntry.Value.GetCustomAttributes(typeof(EntryObjectInstanceAttribute), true);
                if (attributes.Length > 0)
                {
                    entryAttribute = (EntryObjectInstanceAttribute)attributes[0];
                }
                //if this type can be only once added then we need to check if we can add it
                if (entryAttribute.Instance == EntryObjectInstance.Single)
                {
                    if (day.ContainsSpecifiedEntry(pairEntry.Value))
                    {
                        BAMessageBox.ShowError(Strings.ErrorEntryObjectTypeAlreadyExists);
                        return;
                    }
                }
                var entry = day.AddEntry(pairEntry.Value);
                if(day.TrainingDate.IsFuture())
                {//for entries in future set planned status
                    entry.Status = EntryObjectStatus.Planned;
                }
                if (EntryObjectBuilder != null)
                {
                    EntryObjectBuilder.EntryObjectCreated(entry);
                }
                try
                {

                    createNewEntryControl(entry, true, null);
                    
                    SetModifiedFlag();
                    Cursor = Cursors.Arrow;
                }
                catch (TrainingIntegrationException ex)
                {
                    day.RemoveEntry(entry);
                    ExceptionHandler.Default.Process(ex, Strings.ErrorTrainingIntegrity, ErrorWindow.MessageBox);
                }
                catch (Exception ex)
                {
                    day.RemoveEntry(entry);
                    ExceptionHandler.Default.Process(ex, Strings.ErrorUnhandledException, ErrorWindow.EMailReport);
                }
            }
            finally
            {
                Cursor = Cursors.Arrow;
            }

        }



        private bool createNewEntryControl(EntryObjectDTO entry, bool @select, SaveTrainingDayResult saveResult)
        {
            if (entry.IsLoaded)
            {
                try
                {
                    var ctrl = (Control)Activator.CreateInstance(EntryObjectControlManager.Instance.Controls[entry.GetType()]);
                    // var tabPage = new TabItem();
                    string text = entry.GetEntryObjectText();

                    ctrl.Tag = entry;
                    var entryCtrl = (IEntryObjectControl)ctrl;
                    entryCtrl.ReadOnly = ReadOnly || entry.Status==EntryObjectStatus.System;

                    TabItemViewModel tabItem = new TabItemViewModel();
                    tabItem.CloseButtonVisible = !ReadOnly;
                    tabItem.Header = text;
                    tabItem.Content = ctrl;
                    tabItem.IsFromReservation = entry.ReservationId.HasValue;
                    tabItem.EntryObject = entry;
                    tabs.Add(tabItem);
                    xtraTabControl1.SelectedItem = tabItem;

                    //EntryObjectDTO originalEntry = null;
                    //if(originalDay!=null)
                    //{
                    //    originalEntry=originalDay.Objects.Where(x => x.GlobalId == entry.GlobalId).SingleOrDefault();
                    //    if(originalEntry==null)
                    //    {//when originalDay is not null then we now that this is a Saving mode. So we imitate empty instance to have a comparison
                    //        originalEntry = (EntryObjectDTO)Activator.CreateInstance(entry.GetType());
                    //    }
                    //}

                    entryCtrl.Fill(entry, saveResult);
                    updateButtons();
                }
                catch (TrainingIntegrationException ex)
                {
                    ExceptionHandler.Default.Process(ex, Strings.ErrorTrainingIntegrity, ErrorWindow.MessageBox);
                    return false;
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Default.Process(ex, Strings.ErrorUnhandledException, ErrorWindow.EMailReport);
                    return false;
                }

            }
            return true;
        }

        void updateButtons()
        {
            CanRename = xtraTabControl1.SelectedItem != null;
            if (commentsControl != null)
            {
                commentsControl.ReadOnly = ReadOnly;
                //commentsControl.IsCommentsAvailable = !day.IsNew && day.AllowComments;
            }

            CanShare = !ReadOnly;
        }

        void createEntryObjectMenu()
        {
            foreach (var entryControl in EntryObjectControlManager.Instance.Controls)
            {
                //first check if we have A6W entry. If yes then skip it from Add menu
                if (!entryControl.Key.CanBeManuallyAdded())
                {//TODO:Add this to the server part (in SaveTrainingDay method)
                    continue;
                }
                string text = EntryObjectLocalizationManager.Instance.GetString(entryControl.Key, EnumLocalizer.EntryObjectName);
                string description = EntryObjectLocalizationManager.Instance.GetString(entryControl.Key, EnumLocalizer.EntryObjectShortDescription);
                var image=PluginsManager.Instance.GetEntryObjectProvider(entryControl.Key).ModuleImage;
                var item = new ImageSourceListItem<Type>(text,image,entryControl.Key);
                item.Description = description;
                EntryItems.Add(item);
            }
        }



        private bool validateControls()
        {
            foreach (TabItemViewModel tabPage in Tabs)
            {
                IValidationControl entryControl = tabPage.Content as IValidationControl;
                if (entryControl != null)
                {
                    if (!entryControl.ValidateControl())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool SaveTrainingDay(bool isWindowClosing)
        {

            bool res = true;
            Guid? customerId = null;
            Guid? userId = null;
            TrainingDayDTO originalDay = null;
            Dispatcher.Invoke(new Action(delegate
            {
                if (!validateControls())
                {
                    res = false;
                    return;
                }
                customerId = Customer != null ? Customer.GlobalId : (Guid?)null;
                userId = User != null ? User.GlobalId : (Guid?)null;
                foreach (TabItemViewModel tabPage in xtraTabControl1.Items)
                {
                    IEntryObjectControl entryControl = (IEntryObjectControl)tabPage.Content;
                    entryControl.UpdateEntryObject();
                }
                originalDay = TrainingDayPageContext.Day;
                if (commentsControl != null)
                {
                    day.AllowComments = commentsControl.AllowComments;
                }
            }));

            if (!res)
            {
                return false;
            }

            if (day.GlobalId == Constants.UnsavedGlobalId && day.IsEmpty)
            {
                return true;
            }

                var result = ServiceManager.SaveTrainingDay(day);
                day = result.TrainingDay;
                TrainingDayDTO tempDay = null;
                var cache = TrainingDaysReposidory.GetCache(customerId, userId);
                if (day == null)
                {
                    tempDay=day = new TrainingDayDTO(originalDay.TrainingDate);
                    cache.Remove(originalDay.TrainingDate);
                }
                else
                {
                    tempDay = day.StandardClone();
                    cache.Add(tempDay);
                }
            ensureRemindersRefreshed(originalDay, day);


            Dispatcher.Invoke(new Action(delegate
            {
                //if (TrainingDayPageContext.Day.IsNew)
                //{
                //    rowSplitter.Process(tempDay.IsNew);
                //}
                TrainingDayPageContext.Day = tempDay;
                SetModifiedFlag();

                if (commentsControl != null)
                {
                    commentsControl.Fill(day);
                }
                foreach (var tabItemViewModel in Tabs)
                {
                    var ctrl=(IEntryObjectControl)tabItemViewModel.Content;
                    ctrl.AfterSave(false);
                }

                Tabs.Clear();
                refreshTabs(filterType, result);
            }));


            return true;
        }

        void ensureRemindersRefreshed(TrainingDayDTO oldDay,TrainingDayDTO newDay)
        {
            //TODO: Implement smarter reminder cache refresh here
            ReminderItemsReposidory.Instance.ClearCache();
        }

        #region Implementation of IControlView

        void populateFromContext()
        {
            ReadOnly = User != null && User.GlobalId != UserContext.Current.CurrentProfile.GlobalId;
            day = TrainingDayPageContext.Day;
            if (!ReadOnly && TrainingDayPageContext.Day!=null)
            {//if we are in edit mode we must operate on copy because user can cancel his changes
                day = TrainingDayPageContext.Day.StandardClone();
            }
            if (commentsControl != null)
            {
                commentsControl.Fill(day);
            }
            
            
            EntryObjectBuilder = ((TrainingDayPageContext)PageContext).Builder;
        }

        DateTime getDate()
        {
            if(TrainingDayPageContext.Day!=null)
            {
                return TrainingDayPageContext.Day.TrainingDate;
            }
            if (TrainingDayPageContext.DateTime.HasValue)
            {
                return TrainingDayPageContext.DateTime.Value;
            }
            return DateTime.MinValue;
        }
        public override void Fill()
        {
            Header = string.Format("TrainingDayWindow_Fill_Header_Day".TranslateStrings(), getDate().ToShortDateString(),
                                     Customer != null ? Customer.FullName : User.UserName);


            if (TrainingDayPageContext.DateTime != null && TrainingDayPageContext.Day == null)
            {
                fillTrainingDayFromDate();
            }
            else
            {
                IsInProgress = true;
                populateFromContext();
                rebuildMainTabs();
                IsInProgress = false;
            }
        }

        private void fillTrainingDayFromDate()
        {
            DateTime date = TrainingDayPageContext.DateTime.Value;
            var param = new WorkoutDayGetOperation();
            param.WorkoutDateTime = date;
            param.CustomerId = Customer!=null?Customer.GlobalId:(Guid?) null;
            param.UserId = User != null ? User.GlobalId : (Guid?)null;
            IsInProgress = true;
            ParentWindow.RunAsynchronousOperation(delegate(OperationContext context)
                         {
                             //first check the cache and retrieve only when there is no this day in the cache
                             var cache = TrainingDaysReposidory.GetCache(param.CustomerId, param.UserId);
                             var tempDay = cache.GetByDate(date);
                             if (tempDay == null)
                             {
                                 tempDay = ServiceManager.GetTrainingDay(param);
                                 cache.Add(tempDay);
                             }
                             UIHelper.BeginInvoke(()=>
                                                 {
                                                     TrainingDayPageContext.Day = tempDay;
                                                     populateFromContext();
                                                     rebuildMainTabs();
                                                     IsInProgress = false;
                                                 },Dispatcher);

                         });
        }

        private void rebuildMainTabs()
        {
            Tabs.Clear();
            refreshTabs(FilterType.All, null);
            updateButtons();
        }


        public override void RefreshView()
        {
            if (IsModified && BAMessageBox.AskWarningYesNo("Message_RefreshView_ModifiedDay".TranslateStrings()) == MessageBoxResult.No)
            {
                return;
            }
            if (!ReadOnly)
            {
                day = TrainingDayPageContext.Day.StandardClone();
            }
            Fill();
            SetModifiedFlag();
        }

        public TrainingDayPageContext TrainingDayPageContext
        {
            get { return (TrainingDayPageContext)PageContext; }
        }



        public override Uri HeaderIcon
        {
            get { return "OpenTrainingDay.png".ToResourceUrl(); }
        }

        //public static readonly DependencyProperty IsModifiedProperty =
        //    DependencyProperty.Register("IsModified", typeof(bool), typeof(TrainingDayWindow), new UIPropertyMetadata(false));

        #endregion


        void btnDeleteEntry_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            var tabItemViewModel = (TabItemViewModel)button.Tag;
            var obj = tabItemViewModel.EntryObject;
            if (BAMessageBox.AskYesNo(Strings.QAskForDeletingEntryObject, tabItemViewModel.Header) == MessageBoxResult.Yes)
            {
                day.Objects.Remove(obj);
                Tabs.Remove(tabItemViewModel);
                updateButtons();
                SetModifiedFlag();

                if(xtraTabControl1.SelectedItem==null)
                {//we deleted last entry (training day is empty so we must update floating panes)
                    MainWindow.Instance.UpdateAnchorables();
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(commentsControl);

            PleaseWait.Run(x =>
            {
                try
                {
                    if (SaveTrainingDay(true))
                    {
                        //FillRequired = true;
                    }
                }
                catch (LicenceException ex)
                {
                    UIHelper.Invoke(() =>
                    {
                        this.ParentWindow.SetException(ex);
                        ExceptionHandler.Default.Process(ex, Strings.ErrorLicence, ErrorWindow.MessageBox);
                    }, Dispatcher);

                }
                catch (OldDataException ex)
                {
                    UIHelper.Invoke(()=>
                                        {
                                            this.ParentWindow.SetException(ex);
                                            ExceptionHandler.Default.Process(ex, Strings.ErrorOldTrainingDay, ErrorWindow.MessageBox);
                                        },Dispatcher);
                    
                }
                catch (TrainingIntegrationException ex)
                {
                    UIHelper.Invoke(() =>
                    {
                        this.ParentWindow.SetException(ex);
                        ExceptionHandler.Default.Process(ex, Strings.ErrorTrainingIntegrity, ErrorWindow.MessageBox);
                    }, Dispatcher);
                    
                }
                catch (Exception ex)
                {
                    UIHelper.Invoke(() =>
                    {
                        this.ParentWindow.SetException(ex);
                        ExceptionHandler.Default.Process(ex, Strings.ErrorSaveTrainingDay, ErrorWindow.MessageBox);
                    }, Dispatcher);

                }
            });

        }

        private void btnRenameEntry_Click(object sender, RoutedEventArgs e)
        {
            var selectedTab = (TabItemViewModel)xtraTabControl1.SelectedItem;
            if (selectedTab == null)
            {
                return;
            }
            var obj = selectedTab.EntryObject;
            InputWindow dlg = new InputWindow();
            dlg.MaxLength = Constants.NameColumnLength;
            dlg.Value = obj.Name;
            if (dlg.ShowDialog() == true)
            {
                obj.Name = dlg.Value;
                selectedTab.Header = obj.GetEntryObjectText();
                SetModifiedFlag();
            }
        }

        private void tbsShowComments_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.EnsureVisible("TrainingDayCommentsControl");
            
        }

        private void tbsShowDetails_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.EnsureVisible("EntryObjectDetails");
        }

        private void tbsShowProgress_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.EnsureVisible("EntryObjectProgress");
        }

        private void btnPreviousEntry_Click(object sender, RoutedEventArgs e)
        {
            previousNextEntry(CurrentDay, GetOperation.Previous, Strings.InfoPreviousEntryLimit);
        }

        private void btnNextEntry_Click(object sender, RoutedEventArgs e)
        {
            previousNextEntry(CurrentDay, GetOperation.Next, Strings.InfoNextEntryLimit);
        }

        private void previousNextEntry(TrainingDayDTO currentDay, GetOperation operationType, string limitMessage)
        {
            CancelEventArgs e = new CancelEventArgs();
            onTrainingDayChanging(e);
            if (e.Cancel)
            {
                return;
            }
            WorkoutDayGetOperation operation = new WorkoutDayGetOperation();
            operation.Operation = operationType;
            operation.UserId = User.GlobalId;
            if (Customer != null)
            {
                operation.CustomerId = Customer.GlobalId;
            }
            operation.WorkoutDateTime = currentDay.TrainingDate;
            ParentWindow.RunAsynchronousOperation(delegate(OperationContext context)
            {
                
                var day = ServiceManager.GetTrainingDay(operation);
                context.CancellatioToken.ThrowIfCancellationRequested();
                UIHelper.BeginInvoke(new Action(delegate
                {
                    if (day != null)
                    {
                        if (day.TrainingDate.IsFuture() && !UIHelper.EnsurePremiumLicence())
                        {
                            return;
                        }
                        DateTime oldDate = CurrentDay.TrainingDate;
                        TrainingDayPageContext.Day = day;
                        Fill();
                        //populateFromContext();
                        //Fill(day, User, Customer, EntryObjectBuilder);
                        onTrainingDayChanged(new TrainingDayChangedEventArgs(oldDate, CurrentDay.TrainingDate));
                    }
                    else
                    {
                        BAMessageBox.ShowInfo(limitMessage);//ApplicationStrings.InfoNextEntryLimit);
                    }
                }), Dispatcher);
            }, delegate(OperationContext ctx)
            {
                CanNext = CanPrevious = ctx.State != OperationState.Started;
            });
        }

        private void tbsFilterAll_Click(object sender, RoutedEventArgs e)
        {
            refreshTabs(FilterType.All, null);
            //BUG FIX
            e.Handled = true;
        }

        private void tsbFilterOnlyReservations_Click(object sender, RoutedEventArgs e)
        {
            refreshTabs(FilterType.OnlyReservations, null);
            //BUG FIX
            e.Handled = true;
        }

        private void tsbFilterOnlyNotReservations_Click(object sender, RoutedEventArgs e)
        {
            refreshTabs(FilterType.OnlyNotReservations, null);
            //BUG FIX
            e.Handled = true;
        }

        private void tbsShareToFacebook_Click(object sender, RoutedEventArgs e)
        {
            //BUG FIX
            e.Handled = true;

            var selectedTab = (TabItemViewModel)xtraTabControl1.SelectedItem;
            if(selectedTab==null)
            {
                return;
            }
            var obj = selectedTab.EntryObject;
            if (obj.Status==EntryObjectStatus.Planned)
            {
                BAMessageBox.ShowInfo("TrainingDayWindow_ErrShareToFacebook_PlannedEntry".TranslateStrings());
                return;
            }

            //todo:add code where user hasnt' configured facebook account
            if (BAMessageBox.AskYesNo("Message_TrainingDayWindow_tbsShareToFacebook_Click".TranslateStrings()) == MessageBoxResult.Yes)
            {
                if (string.IsNullOrEmpty(Settings1.Default.FacebookToken))
                {
                    FacebookLoginWindow dlg = new FacebookLoginWindow();
                    if (dlg.ShowDialog() == false)
                    {
                        return;
                    }
                }
               
                SocialNetworkShare share = new SocialNetworkShare();
                if (share.PostEntryObject(obj))
                {
                    BAMessageBox.ShowInfo(Strings.TrainingDayWindow_MsgFacebokShareCompleted);    
                }
                else
                {
                    BAMessageBox.ShowInfo(Strings.TrainingDayWindow_MsgFacebokShareNotImplemented);    
                }
                
            }

        }


        public void SetModifiedFlag()
        {
            if (!ReadOnly)
            {
                var val = day.IsModified(TrainingDayPageContext.Day);
                IsModified = val;
                NotifyOfPropertyChange(() => IsModified);
            }
        }

        private void xtraTabControl1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CanRename = CanShare = xtraTabControl1.SelectedItem != null;
        }
    }

    [Serializable]
    public class TrainingDayPageContext : PageContext
    {
        private TrainingDayDTO day;
        private IEntryObjectBuilderProvider builder;
        private DateTime? dateTime;

        public TrainingDayPageContext(UserDTO user, CustomerDTO customer, TrainingDayDTO day, IEntryObjectBuilderProvider builder)
            : base(user, customer)
        {
            this.day = day;
            this.builder = builder;
        }

        public TrainingDayPageContext()
        {
            
        }


        public TrainingDayDTO Day
        {
            get { return day; }
            set { day = value; }
        }

        public IEntryObjectBuilderProvider Builder
        {
            get { return builder; }
            set { builder = value; }
        }

        public DateTime? DateTime
        {
            get { return dateTime; }
            set { dateTime = value; }
        }
    }

    public class TabItemViewModel : ViewModelBase
    {
        public bool CloseButtonVisible { get; set; }

        private string _header;
        public string Header
        {
            get { return _header; }
            set
            {
                _header = value;
                NotifyOfPropertyChange(() => Header);
            }
        }

        public FrameworkElement Content { get; set; }

        public EntryObjectDTO EntryObject { get; set; }

        public bool IsFromReservation { get; set; }
    }

    enum FilterType
    {
        All,
        OnlyReservations,
        OnlyNotReservations
    }

    public class TrainingDayChangedEventArgs : EventArgs
    {
        public TrainingDayChangedEventArgs(DateTime oldDate, DateTime newDate)
        {
            OldDate = oldDate;
            NewDate = newDate;
        }

        public DateTime OldDate { get; private set; }

        public DateTime NewDate { get; private set; }
    }
}
