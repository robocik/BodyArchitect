//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Linq;
//using System.Text;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;
//using BodyArchitect.Client.Common;
//using BodyArchitect.Client.Common.Plugins;
//using BodyArchitect.Client.Resources.Localization;
//using BodyArchitect.Client.UI.Controls;
//using BodyArchitect.Client.UI.Social;
//using BodyArchitect.Client.UI.Windows;
//using BodyArchitect.Client.WCF;
//using BodyArchitect.Logger;
//using BodyArchitect.Service.V2.Model;
//using BodyArchitect.Service.V2.Model.Instructor;
//using BodyArchitect.Settings;
//using BodyArchitect.Shared;
//namespace BodyArchitect.Client.UI.UserControls
//{
//    //TODO:REMOVE THIS CLASS
//    /// <summary>
//    /// Interaction logic for usrTrainingDay.xaml
//    /// </summary>
//    public partial class usrTrainingDay
//    {
//        private TrainingDayDTO day;
//        private bool dayRemoved;
//        public event CancelEventHandler TrainingDayChanging;
//        public event EventHandler<TrainingDayChangedEventArgs> TrainingDayChanged;
//        private ObservableCollection<TabItemViewModel> tabs = new ObservableCollection<TabItemViewModel>();

//        public usrTrainingDay()
//        {
//            InitializeComponent();
//            xtraTabControl1.ItemsSource = tabs;
//            createEntryObjectMenu();
//            fillSuperTips();
//            updateButtons();
//            //toolStrip1.ShowItemToolTips = UserContext.Settings.GuiState.ShowToolTips;
//        }

//        private void onTrainingDayChanging(CancelEventArgs e)
//        {
//            if(TrainingDayChanging!=null)
//            {
//                TrainingDayChanging(this, e);
//            }
//        }

//        private void onTrainingDayChanged(TrainingDayChangedEventArgs e)
//        {
//            if (TrainingDayChanged != null)
//            {
//                TrainingDayChanged(this, e);
//            }
//        }
//        public bool ReadOnly
//        {
//            get { return day == null || day.ProfileId != UserContext.Current.CurrentProfile.GlobalId; }
//        }

//        public CustomerDTO Customer { get; private set; }

//        public TrainingDayDTO CurrentDay
//        {
//            get
//            {
//                return day;
//            }
//            private set
//            {
//                day = value;
//            }
//        }

//        void createEntryObjectMenu()
//        {
//            foreach (var entryControl in EntryObjectControlManager.Instance.Controls)
//            {
//                //first check if we have A6W entry. If yes then skip it from Add menu
//                if (!entryControl.Key.CanBeManuallyAdded())
//                {//TODO:Add this to the server part (in SaveTrainingDay method)
//                    continue;
//                }

//                string text = EntryObjectLocalizationManager.Instance.GetString(entryControl.Key, EnumLocalizer.EntryObjectName);
//                MenuItem tsMenuItem = new MenuItem();

//                StackPanel panel = new StackPanel();
//                panel.Orientation = Orientation.Horizontal;
//                Image img = new Image();
//                img.Source = PluginsManager.Instance.GetEntryObjectProvider(entryControl.Key).ModuleImage;
//                panel.Children.Add(img);
//                TextBlock textBlock = new TextBlock();
//                textBlock.Text = text;

//                //textBlock.Style = (Style)this.FindResource("accordionHeaderText");
//                panel.Children.Add(textBlock);

//                tsMenuItem.Header = panel;
//                tsMenuItem.Tag = entryControl;
//                tsMenuItem.Click += tsStrengthTraining_Click;
//                tsAddMenu.Children.Add(tsMenuItem);
//            }
//        }

//        void fillSuperTips()
//        {
//            //ControlHelper.AddSuperTip(this.txtDate, lblDate.Text, Strings.AddTrainingDay_DateTE);
//        }

//        void updateButtons()
//        {
//            tsRename.IsEnabled = xtraTabControl1.SelectedItem != null;
//            tsRename.SetVisible(!ReadOnly);
//            //show delete button only when we open existing day (saved in db)
//            this.tsbDeleteTrainingDay.IsEnabled = day != null && day.GlobalId != Constants.UnsavedGlobalId;
//            tsbDeleteTrainingDay.SetVisible(!ReadOnly);
//            this.tbAddEntry.SetVisible(!ReadOnly);
//            toolStripSeparator1.SetVisible(!ReadOnly);
//            toolStripSeparator2.SetVisible(!ReadOnly);
//        }

//        public bool DayRemoved
//        {
//            get { return dayRemoved; }
//        }

//        public UserDTO User { get; private set; }


//        public ObservableCollection<TabItemViewModel> Tabs
//        {
//            get { return tabs; }
//        }

//        public void Fill(TrainingDayDTO day, UserDTO user,CustomerDTO customer, IEntryObjectBuilderProvider builder)
//        {
//            this.day = day;
//            EntryObjectBuilder = builder;
//            User = user;
//            Customer = customer;
//            Tabs.Clear();
//            txtDate.Text = day.TrainingDate.ToShortDateString();
//            refreshTabs(FilterType.All);
//            updateButtons();
//        }

//        protected IEntryObjectBuilderProvider EntryObjectBuilder { get;private  set; }

//        void refreshTabs(FilterType filterType)
//        {
//            tsbFilterAll.IsChecked = filterType == FilterType.All;
//            tsbFilterOnlyReservation.IsChecked = filterType == FilterType.OnlyReservations;
//            tsbFilterOnlyNotReservation.IsChecked = filterType == FilterType.OnlyNotReservations;
//            //xtraTabControl1.Items.Clear();
//            tabs.Clear();
//            for (int index = day.Objects.Count - 1; index >= 0; index--)
//            {
//                var entry = day.Objects.ElementAt(index);
//                bool showByFilter = filterType == FilterType.All || (filterType == FilterType.OnlyReservations && entry.ReservationId.HasValue) || (filterType == FilterType.OnlyNotReservations && !entry.ReservationId.HasValue);
//                if (showByFilter && !createNewEntryControl(entry, false))
//                {
//                    //exception during creating new entry so we delete it from training day
//                    day.RemoveEntry(entry);
//                }
//            }
//        }
        

//        private bool createNewEntryControl(EntryObjectDTO entry, bool select)
//        {
//            if (entry.IsLoaded )
//            {
//                try
//                {

//                    var ctrl = (Control)Activator.CreateInstance(EntryObjectControlManager.Instance.Controls[entry.GetType()]);
//                   // var tabPage = new TabItem();
//                    string text = entry.GetEntryObjectText();

//                    //StackPanel panel = new StackPanel();
//                    //panel.Orientation = Orientation.Horizontal;
//                    //Image img = new Image();
//                    //panel.Children.Add(img);
//                    //TextBlock textBlock = new TextBlock();
//                    //textBlock.Text = text;

//                    ////textBlock.Style = (Style)this.FindResource("accordionHeaderText");
//                    //panel.Children.Add(textBlock);
//                    //Button btn = new Button();
//                    //btn.Tag = tabPage;
//                    //btn.Click += new RoutedEventHandler(btnDeleteEntry_Click);
//                    //btn.Style = (Style) Application.Current.FindResource("CloseableTabItemButtonStyle");
//                    //panel.Children.Add(btn);
//                    //tabPage.Header = panel;

//                    //tabPage.Content = ctrl;
//                    //TODO:FINISH
//                    ctrl.Tag =entry;
//                    //xtraTabControl1.Items.Add(tabPage);
                    

//                    TabItemViewModel tabItem = new TabItemViewModel();
//                    tabItem.CloseButtonVisible = !ReadOnly;
//                    tabItem.Header = text;
//                    tabItem.Content = ctrl;
//                    tabItem.IsFromReservation = entry.ReservationId.HasValue;
//                    tabItem.EntryObject = entry;
//                    tabs.Add(tabItem);
//                    xtraTabControl1.SelectedItem = tabItem;
//                    var entryCtrl = (IEntryObjectControl)ctrl;
//                    entryCtrl.ReadOnly = ReadOnly;
//                    entryCtrl.Fill(entry,null);
//                    updateButtons();
//                }
//                catch (TrainingIntegrationException ex)
//                {
//                    ExceptionHandler.Default.Process(ex, Strings.ErrorTrainingIntegrity, ErrorWindow.MessageBox);
//                    return false;
//                }
//                catch (Exception ex)
//                {
//                    ExceptionHandler.Default.Process(ex, Strings.ErrorUnhandledException, ErrorWindow.EMailReport);
//                    return false;
//                }
                
//            }
//            return true;
//        }
        
        

//        public bool SaveTrainingDay(bool isWindowClosing)
//        {

//            //bool res = true;
//            //Dispatcher.Invoke(new Action(delegate
//            //    {
//            //        if (!validateControls())
//            //        {
//            //            res= false;
//            //            return;
//            //        }
//            //        foreach (TabItemViewModel tabPage in xtraTabControl1.Items)
//            //        {
//            //            IEntryObjectControl entryControl = (IEntryObjectControl)tabPage.Content;
//            //            entryControl.UpdateEntryObject();
//            //        }
//            //    }));
//            //if(!res)
//            //{
//            //    return false;
//            //}

//            //if (day.Id==Constants.UnsavedObjectId && day.IsEmpty)
//            //{
//            //    return true;
//            //}

//            //day = ServiceManager.SaveTrainingDay(day);

//            //if (isWindowClosing)
//            //{//optimalization - we are closing window so we don't need to refresh the tabs (code below)
//            //    return true;
//            //}
//            //for (int index = Tabs.Count-1; index >=0; index--)
//            //{
//            //    TabItemViewModel tabPage = Tabs[index];
//            //    IEntryObjectControl entryControl = tabPage.Content as IEntryObjectControl;
//            //    if (entryControl != null)
//            //    {
//            //        Dispatcher.Invoke(new Action(delegate
//            //                   {
//            //                       entryControl.AfterSave(isWindowClosing);
//            //                       Control ctrl = (Control) tabPage.Content;
//            //                       EntryObjectDTO entry = (EntryObjectDTO)ctrl.Tag;
//            //                       //here we assume that training day contains only one instance of the specific entry type
//            //                       var newEntry =
//            //                           day.GetSpecifiedEntries(entry.GetType());
//            //                       if (newEntry != null)
//            //                       {
//            //                           ctrl.Tag = newEntry;
//            //                           entryControl.Fill(newEntry);
//            //                       }
//            //                       else
//            //                       {
//            //                           Tabs.Remove(tabPage);
//            //                       }
//            //                   }), null);
//            //    }
//            //}

//            return true;
//        }

//        private bool validateControls()
//        {
//            foreach (TabItemViewModel tabPage in Tabs)
//            {
//                IValidationControl entryControl = tabPage.Content as IValidationControl;
//                if (entryControl != null)
//                {
//                    if(!entryControl.ValidateControl())
//                    {
//                        return false;
//                    }
//                }
//            }
//            return true;
//        }

//        private void tsStrengthTraining_Click(object sender, EventArgs e)
//        {
//            Cursor = Cursors.Wait;
//            try
//            {
//                var menuItem = (MenuItem)sender;
//                KeyValuePair<Type, Type> pairEntry = (KeyValuePair<Type, Type>)menuItem.Tag;

//                EntryObjectInstanceAttribute entryAttribute = new EntryObjectInstanceAttribute();
//                var attributes = pairEntry.Key.GetCustomAttributes(typeof(EntryObjectInstanceAttribute), true);
//                if (attributes.Length > 0)
//                {
//                    entryAttribute = (EntryObjectInstanceAttribute)attributes[0];
//                }
//                //if this type can be only once added then we need to check if we can add it
//                if (entryAttribute.Instance == EntryObjectInstance.Single)
//                {
//                    if (day.ContainsSpecifiedEntry(pairEntry.Key))
//                    {
//                        BAMessageBox.ShowError(Strings.ErrorEntryObjectTypeAlreadyExists);
//                        return;
//                    }
//                }
//                var entry = day.CreateEntryObject(pairEntry.Key);
//                if(EntryObjectBuilder!=null)
//                {
//                    EntryObjectBuilder.EntryObjectCreated(entry);
//                }
//                try
//                {

//                    createNewEntryControl(entry, true);

//                    day.AddEntry(entry);
//                    Cursor = Cursors.Arrow;
//                }
//                catch (TrainingIntegrationException ex)
//                {
//                    ExceptionHandler.Default.Process(ex, Strings.ErrorTrainingIntegrity, ErrorWindow.MessageBox);
//                }
//                catch (Exception ex)
//                {
//                    ExceptionHandler.Default.Process(ex, Strings.ErrorUnhandledException, ErrorWindow.EMailReport);
//                }
//            }
//            finally
//            {
//                Cursor = Cursors.Arrow;
//            }
            
//        }

//        //private void xtraTabControl1_CloseButtonClick(object sender, EventArgs e)
//        //{
//        //    ClosePageButtonEventArgs closeArgs =(ClosePageButtonEventArgs) e;
//        //    XtraTabPage page = (XtraTabPage)closeArgs.Page;
//        //    var obj = (EntryObjectDTO)page.Tag;
//        //    if (FMMessageBox.AskYesNo(ApplicationStrings.QAskForDeletingEntryObject, page.Text) == DialogResult.Yes)
//        //    {
//        //        day.Objects.Remove(obj);
//        //        xtraTabControl1.TabPages.Remove(page);
//        //        updateButtons();
//        //    }
//        //}

//        private void tsRename_Click(object sender, EventArgs e)
//        {
//            var selectedTab = (TabItemViewModel) xtraTabControl1.SelectedItem;
//            var obj = selectedTab.EntryObject;
//            InputWindow dlg = new InputWindow();
//            dlg.MaxLength = Constants.NameColumnLength;
//            dlg.Value = obj.Name;
//            if (dlg.ShowDialog() == true)
//            {
//                obj.Name = dlg.Value;
//                selectedTab.Header = obj.GetEntryObjectText();
//            }
//        }

//        private void tsbDeleteTrainingDay_Click(object sender, EventArgs e)
//        {
//            if (DomainObjectOperatonHelper.DeleteTrainingDay(day))
//            {
//                dayRemoved = true;

//                var parentForm = UIHelper.FindVisualParent<Window>(this);
//                parentForm.DialogResult = true;
//                parentForm.Close();
//            }
//        }

//        private void tsbPrevious_Click(object sender, EventArgs e)
//        {
//            previousNextEntry(CurrentDay, GetOperation.Previous, Strings.InfoPreviousEntryLimit);
//        }

//        private void tsbNext_Click(object sender, EventArgs e)
//        {
//            previousNextEntry(CurrentDay, GetOperation.Next, Strings.InfoNextEntryLimit);
//        }


//        private void previousNextEntry(TrainingDayDTO currentDay, GetOperation operationType, string limitMessage)
//        {
//            CancelEventArgs e= new CancelEventArgs();
//            onTrainingDayChanging(e);
//            if(e.Cancel)
//            {
//                return;
//            }
//            ParentWindow.RunAsynchronousOperation(delegate(OperationContext context)
//            {
//                WorkoutDayGetOperation operation = new WorkoutDayGetOperation();
//                operation.Operation = operationType;
//                operation.UserId = User.GlobalId;
//                if(Customer!=null)
//                {
//                    operation.CustomerId = Customer.GlobalId;
//                }
//                operation.WorkoutDateTime = currentDay.TrainingDate;
//                var day = ServiceManager.GetTrainingDay(operation);
//                context.CancellatioToken.ThrowIfCancellationRequested();
//                UIHelper.BeginInvoke(new Action(delegate
//                {
//                    if (day != null)
//                    {
//                        DateTime oldDate = CurrentDay.TrainingDate;
//                        CurrentDay = day;
//                        Fill(day, User,Customer,EntryObjectBuilder);
//                        onTrainingDayChanged(new TrainingDayChangedEventArgs(oldDate,CurrentDay.TrainingDate));
//                    }
//                    else
//                    {
//                        BAMessageBox.ShowInfo(limitMessage);//ApplicationStrings.InfoNextEntryLimit);
//                    }
//                }), Dispatcher);
//            },  delegate(OperationContext ctx)
//            {
//                tsbNext.IsEnabled = tsbPrevious.IsEnabled = ctx.State != OperationState.Started;
//            });

            
//        }

//        void btnDeleteEntry_Click(object sender, RoutedEventArgs e)
//        {
//            Button button = (Button) sender;
//            var tabItemViewModel=(TabItemViewModel) button.Tag;
//            var obj = tabItemViewModel.EntryObject;
//            if (BAMessageBox.AskYesNo(Strings.QAskForDeletingEntryObject, tabItemViewModel.Header) == MessageBoxResult.Yes)
//            {
//                day.Objects.Remove(obj);
//                Tabs.Remove(tabItemViewModel);
//                updateButtons();
//            }
//        }

//        private void tsAddMenu_Click(object sender, RoutedEventArgs e)
//        {
//            tbAddEntry.IsOpen = false;
//        }

  
//        private void tbsFilterAll_Click(object sender, RoutedEventArgs e)
//        {
//            refreshTabs(FilterType.All);
//            tsbFilter.IsOpen = false;
//        }

//        private void tsbFilterOnlyReservations_Click(object sender, RoutedEventArgs e)
//        {
//            refreshTabs(FilterType.OnlyReservations);
//            tsbFilter.IsOpen = false;
//        }

//        private void tsbFilterOnlyNotReservations_Click(object sender, RoutedEventArgs e)
//        {
//            refreshTabs(FilterType.OnlyNotReservations);
//            tsbFilter.IsOpen = false;
//        }

//        private void tbsShareToFacebook_Click(object sender, RoutedEventArgs e)
//        {
//            //todo:completed
//            //todo:add code where user hasnt' configured facebook account
//            if (BAMessageBox.AskYesNo(Strings.Question_usrTrainingDay_PublishEntryToFacebook) == MessageBoxResult.Yes)
//            {
//                if(string.IsNullOrEmpty(Settings1.Default.FacebookToken))
//                {
//                    FacebookLoginWindow dlg = new FacebookLoginWindow();
//                    if (dlg.ShowDialog() == false)
//                    {
//                        return;
//                    }
//                }
//                var selectedTab = (TabItemViewModel)xtraTabControl1.SelectedItem;
//                var obj = selectedTab.EntryObject;
//                SocialNetworkShare share = new SocialNetworkShare();
//                share.PostEntryObject(obj);    
//            }
            
//        }

//    }

//    


//}
