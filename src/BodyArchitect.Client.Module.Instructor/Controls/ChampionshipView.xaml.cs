using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using AvalonDock.Layout;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.Instructor.ViewModel;
using BodyArchitect.Client.Module.StrengthTraining;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Cache;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Views;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;

namespace BodyArchitect.Client.Module.Instructor.Controls
{
    /// <summary>
    /// Interaction logic for ChampionshipView.xaml
    /// </summary>
    public partial class ChampionshipView : IHasFloatingPane
    {
        ObservableCollection<ChampionshipEntryViewModel> items = new ObservableCollection<ChampionshipEntryViewModel>();
        
        ObservableCollection<Common.ListItem<ChampionshipGridGroupMode>> gridGroupModes = new ObservableCollection<ListItem<ChampionshipGridGroupMode>>();
        internal usrChampionshipTeamsPane teamsControl;

        private bool canSave;
        private ChampionshipDTO championship;
        private ChampionshipDTO original;
        private ChampionshipGridGroupMode _selectedGridGroup;
        private bool canEditGroup;
        private bool canDeleteGroup;
        private ListItem<ChampionshipGroupDTO> _selectedGroup;
        ObservableCollection<ListItem<ChampionshipGroupDTO>> groups = new ObservableCollection<ListItem<ChampionshipGroupDTO>>();
        private bool _hasResults;

        public ChampionshipView()
        {
            InitializeComponent();
            DataContext = this;

            gridGroupModes.Add(new ListItem<ChampionshipGridGroupMode>(InstructorStrings.ChampionshipView_Grouping_None, ChampionshipGridGroupMode.None));
            gridGroupModes.Add(new ListItem<ChampionshipGridGroupMode>(InstructorStrings.ChampionshipView_Grouping_ByGender, ChampionshipGridGroupMode.Gender));
            gridGroupModes.Add(new ListItem<ChampionshipGridGroupMode>(InstructorStrings.ChampionshipView_Grouping_ByWeight, ChampionshipGridGroupMode.ByWeightCategories));
            gridGroupModes.Add(new ListItem<ChampionshipGridGroupMode>(InstructorStrings.ChampionshipView_Grouping_ByTeam, ChampionshipGridGroupMode.ByGroup));

            MainWindow.Instance.EnsureAnchorable(InstructorStrings.ChampionshipView_Group_Teams, "pack://application:,,,/BodyArchitect.Client.Resources;component/Images/Comments16.png", "ChampionshipTeamsPane", AnchorableShowStrategy.Right);
        }

        #region Properties

        public ObservableCollection<ListItem<ChampionshipGroupDTO>> Groups
        {
            get { return groups; }
        }


        public ListItem<ChampionshipGroupDTO> SelectedGroup
        {
            get { return _selectedGroup; }
            set
            {

                _selectedGroup = value;
                UpdateToolbar();
                NotifyOfPropertyChange(() => SelectedGroup);
            }
        }

        public bool CanEditGroup
        {
            get { return canEditGroup; }
            set
            {
                canEditGroup = value;
                NotifyOfPropertyChange(()=>CanEditGroup);
            }
        }

        public bool CanDeleteGroup
        {
            get { return canDeleteGroup; }
            set
            {
                canDeleteGroup = value;
                NotifyOfPropertyChange(() => CanDeleteGroup);
            }
        }

        public ChampionshipGridGroupMode SelectedGridGroup
        {
            get { return _selectedGridGroup; }
            set
            {

                if (_selectedGridGroup != value)
                {
                    _selectedGridGroup = value;
                    grid.GroupItemsBy(value);
                }
                NotifyOfPropertyChange(() => SelectedGridGroup);
            }
        }


        public ObservableCollection<Common.ListItem<ChampionshipGridGroupMode>> GridGroupModes
        {
            get { return gridGroupModes; }
        }

        public ObservableCollection<ChampionshipEntryViewModel> Items
        {
            get { return items; }
        }

        public bool HasResults
        {
            get { return _hasResults; }
            set
            {
                _hasResults = value;
                NotifyOfPropertyChange(() => HasResults);
            }
        }
        

        public bool CanSave
        {
            get { return canSave; }
            set
            {
                canSave = value;
                NotifyOfPropertyChange(()=>CanSave);
            }
        }

        public ChampionshipPageContext ChampionshipPageContext
        {
            get { return (ChampionshipPageContext)PageContext; }
        }

        #endregion

        public Control GetContentForPane(string paneId)
        {
            if (paneId == "ChampionshipTeamsPane")
            {
                if (teamsControl == null)
                {
                    teamsControl = new usrChampionshipTeamsPane();
                    teamsControl.Fill(this);
                    UpdateToolbar();
                }
                
                return teamsControl;
            }
            return null;
        }

        public void SetModifiedFlag()
        {
            var val = Championship.IsModified(original);
            IsModified = val || Championship.IsNew;
            NotifyOfPropertyChange(() => Header);
            NotifyOfPropertyChange(() => IsModified);
        }

        public override void Fill()
        {
            Header = string.Format(InstructorStrings.ChampionshipView_Header,ChampionshipPageContext.Championship.Name);
            IsInProgress = true;
            updateButtons(false);
            ParentWindow.RunAsynchronousOperation(x =>
                                                      {
                                                          var champWait=ChampionshipsReposidory.Instance.BeginEnsure();
                                                          var custWait=CustomersReposidory.Instance.BeginEnsure();
                                                          var exercisesWait = ExercisesReposidory.Instance.BeginEnsure();
                                                          WaitHandle.WaitAll(new WaitHandle[] { custWait, champWait, exercisesWait });
                                                          UIHelper.Invoke(() =>
                                                                              {
                                                                                  championship =ChampionshipsReposidory.Instance.GetItem(((BAGlobalObject)ChampionshipPageContext.Championship).GlobalId);
                                                                                  fillImplementation();
                                                                                  IsInProgress = false;
                                                                                  
                                                                              },Dispatcher);
                                                          
                                                      });
        }

        private void fillImplementation()
        {
            original = Championship;
            championship = Championship.StandardClone();
            fillChampionship(Championship);
            fillResults(Championship);
            grid.BuildColumns(Championship.ChampionshipType);
                                                                                  
            updateReadOnly();
            updateButtons(true);
        }

        void updateReadOnly()
        {
            grid.IsReadOnly = false;
        }

        void updateButtons(bool enabled)
        {
            CanSave = enabled;
        }
        

        void fillResults(ChampionshipDTO item)
        {

            List<ChampionshipResultsViewModel> items = new List<ChampionshipResultsViewModel>();
            foreach (IGrouping<ChampionshipCategoryDTO, ChampionshipResultItemDTO> dto in item.Results.GroupBy(x=>x.Category).OrderBy(x=>x.Key.Category))
            {
                items.Add(new ChampionshipResultsViewModel(dto));
            }
            HasResults = item.Results.Count > 0;
            lstResults.ItemsSource = items;
        }
        void fillChampionship(ChampionshipDTO item)
        {

            groups.Clear();
            Groups.Add(new ListItem<ChampionshipGroupDTO>("", null));
            foreach (var championshipGroupDto in item.Groups)
            {
                Groups.Add(new ListItem<ChampionshipGroupDTO>(championshipGroupDto.Name, championshipGroupDto));
            }

            Items.Clear();
            foreach (var reservation in item.Reservations)
            {
                var customer = CustomersReposidory.Instance.GetItem(reservation.CustomerId);
                ChampionshipEntryViewModel itemViewModel = new ChampionshipEntryViewModel(customer, item,this);

                Items.Add(itemViewModel);
            }
            
            if (teamsControl != null)
            {
                teamsControl.Fill(this);
            }
        }

        public bool ShowGroups
        {
            get { return teamsControl != null; }
        }

        public override void RefreshView()
        {
            ChampionshipsReposidory.Instance.ClearCache();
            Fill();
        }

        public override Uri HeaderIcon
        {
            get { return new Uri(@"pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Images/Championship16.png"); }
        }

        public ChampionshipDTO Championship
        {
            get { return championship; }
        }

        ChampionshipDTO updateChampionship()
        {
            Championship.Entries.Clear();
            Championship.Groups.Clear();
            foreach (var viewModel in Items)
            {
                Championship.Entries.Add(viewModel.Exercise1.Entry);
                if (Championship.ChampionshipType == ChampionshipType.Trojboj)
                {
                    Championship.Entries.Add(viewModel.Exercise2.Entry);
                    Championship.Entries.Add(viewModel.Exercise3.Entry);
                }
            }

            foreach (var item in Groups)
            {
                if (item.Value != null)
                {
                    Championship.Groups.Add(item.Value);
                }
            }

            if (teamsControl != null)
            {
                Championship.Comment = teamsControl.Comment;
            }
            return Championship;
        }

        private void displayNotificationAboutRecords(SaveChampionshipResult saveResult)
        {
            if (saveResult == null)
            {
                return;
            }
            foreach (var item in Items)
            {
                item.Exercise1.IsExercise1Try1Record = false;
                item.Exercise1.IsExercise1Try2Record = false;
                item.Exercise1.IsExercise1Try1Record = false;
                item.Exercise2.IsExercise1Try1Record = false;
                item.Exercise2.IsExercise1Try2Record = false;
                item.Exercise2.IsExercise1Try1Record = false;
                item.Exercise3.IsExercise1Try1Record = false;
                item.Exercise3.IsExercise1Try2Record = false;
                item.Exercise3.IsExercise1Try1Record = false;
            }
            
            foreach (var currentRecord in saveResult.NewRecords)
            {
                //RecordNotifyObject notifyObject = new RecordNotifyObject(currentRecord);
                //MainWindow.Instance.ShowNotification(notifyObject);
                var item=Items.Where(x =>x.Customer.GlobalId ==currentRecord.StrengthTrainingItem.StrengthTrainingEntry.TrainingDay.CustomerId).SingleOrDefault();
                if (item.Exercise1.Entry.Exercise.GlobalId == currentRecord.StrengthTrainingItem.Exercise.GlobalId)
                {
                    if (item.Exercise1.Exercise1Try1Weight == currentRecord.Weight && item.Exercise1.IsExercise1Try1Ok)
                    {
                        item.Exercise1.IsExercise1Try1Record = true;    
                    }
                    else if (item.Exercise1.Exercise1Try2Weight == currentRecord.Weight && item.Exercise1.IsExercise1Try2Ok)
                    {
                        item.Exercise1.IsExercise1Try2Record = true;
                    }
                    else if (item.Exercise1.Exercise1Try3Weight == currentRecord.Weight && item.Exercise1.IsExercise1Try3Ok)
                    {
                        item.Exercise1.IsExercise1Try3Record = true;
                    }
                }
                if (item.Exercise2.Entry.Exercise.GlobalId == currentRecord.StrengthTrainingItem.Exercise.GlobalId)
                {
                    if (item.Exercise2.Exercise1Try1Weight == currentRecord.Weight && item.Exercise2.IsExercise1Try1Ok)
                    {
                        item.Exercise2.IsExercise1Try1Record = true;
                    }
                    else if (item.Exercise2.Exercise1Try2Weight == currentRecord.Weight && item.Exercise2.IsExercise1Try2Ok)
                    {
                        item.Exercise2.IsExercise1Try2Record = true;
                    }
                    else if (item.Exercise2.Exercise1Try3Weight == currentRecord.Weight && item.Exercise2.IsExercise1Try3Ok)
                    {
                        item.Exercise2.IsExercise1Try3Record = true;
                    }
                }
                if (item.Exercise3.Entry.Exercise.GlobalId == currentRecord.StrengthTrainingItem.Exercise.GlobalId)
                {
                    if (item.Exercise3.Exercise1Try1Weight == currentRecord.Weight && item.Exercise3.IsExercise1Try1Ok)
                    {
                        item.Exercise3.IsExercise1Try1Record = true;
                    }
                    else if (item.Exercise3.Exercise1Try2Weight == currentRecord.Weight && item.Exercise3.IsExercise1Try2Ok)
                    {
                        item.Exercise3.IsExercise1Try2Record = true;
                    }
                    else if (item.Exercise3.Exercise1Try3Weight == currentRecord.Weight && item.Exercise3.IsExercise1Try3Ok)
                    {
                        item.Exercise3.IsExercise1Try3Record = true;
                    }
                }
            }
        }

        private void btnSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Keyboard.Focus(this);
            IsInProgress = true;
            
            PleaseWait.Run(x =>
                               {
                try
                {
                    championship = updateChampionship();
                    var result=ServiceManager.SaveChampionship(Championship);
                    championship = result.Championship;
                    ChampionshipsReposidory.Instance.Update(Championship);
                    UIHelper.Invoke(() =>
                                        {
                                            fillImplementation();
                                            displayNotificationAboutRecords(result);
                                        }, Dispatcher);
                    
                }
                catch(ArgumentException ex)
                {
                    UIHelper.Invoke(() =>
                    {
                        this.ParentWindow.SetException(ex);
                        ExceptionHandler.Default.Process(ex, InstructorStrings.ChampionshipView_ErrCustomerWithoutGender, ErrorWindow.MessageBox);
                    }, Dispatcher);
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
                    UIHelper.Invoke(() =>
                    {
                        this.ParentWindow.SetException(ex);
                        ExceptionHandler.Default.Process(ex, Strings.ErrorOldTrainingDay, ErrorWindow.MessageBox);
                    }, Dispatcher);

                }
                catch (Exception ex)
                {
                    UIHelper.Invoke(() =>
                    {
                        this.ParentWindow.SetException(ex);
                        ExceptionHandler.Default.Process(ex,InstructorStrings.ChampionshipView_ErrSaveUnhandled, ErrorWindow.MessageBox);
                    }, Dispatcher);

                }
                finally
                {
                    UIHelper.Invoke(delegate
                                        {
                                            IsInProgress = false;
                                        }, Dispatcher);
                }
            });
        }

        private void rbtnNewGroup_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            InputWindow dlg = new InputWindow(true);
            if(dlg.ShowDialog()==true)
            {
                var group = new ChampionshipGroupDTO();
                group.Name = dlg.Value;
                Groups.Add(new ListItem<ChampionshipGroupDTO>(group.Name, group));
                updateChampionship();
                SetModifiedFlag();
            }
        }

        internal void UpdateToolbar()
        {
            if (teamsControl != null)
            {
                CanEditGroup = SelectedGroup != null;
                CanDeleteGroup = SelectedGroup != null;
            }
            NotifyOfPropertyChange(() => ShowGroups);
        }

        private void rbtnEditGroup_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            InputWindow dlg = new InputWindow(true);
            dlg.Value = SelectedGroup.Value.Name;
            if (dlg.ShowDialog() == true)
            {
                SelectedGroup.Value.Name = dlg.Value;
                SelectedGroup.Text = dlg.Value;
                updateChampionship();
                SetModifiedFlag();
            }
        }

        private void rbtnDeleteGroup_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (BAMessageBox.AskYesNo(InstructorStrings.ChampionshipView_QDeleteGroup) == MessageBoxResult.Yes)
            {
                Groups.Remove(SelectedGroup);
                foreach (var item in Items)
                {
                    if (item.SelectedGroup == SelectedGroup)
                    {
                        item.SelectedGroup = null;
                    }
                }
                updateChampionship();
                SetModifiedFlag();
            }
        }


        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.UI;component/Views/HtmlPreviewView.xaml"), () =>
            {
                return new HtmlPreviewPageContext(new ChampionshipHtmlExporter(Championship));
            });
        }

        private void grid_CurrentCellChanged(object sender, EventArgs e)
        {
            SetModifiedFlag();
        }

        private void tbsShowTeams_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.EnsureVisible("ChampionshipTeamsPane");
        }
        //to mialo byc po to zeby moc scrollować na wynikach (teraz listview przechwytuje scrola) ale to nie dziala
        //http://josheinstein.com/blog/index.php/2010/08/wpf-nested-scrollviewer-listbox-scrolling/
        //private void UIElement_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        //{
        //    e.Handled = true;

        //    var e2 = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
        //    e2.RoutedEvent = UIElement.MouseWheelEvent;
        //}
        
    }

    [Serializable]
    public class ChampionshipPageContext : PageContext
    {
        private IHasName championship;

        public ChampionshipPageContext(IHasName championship)
        {
            this.championship = championship;
        }


        public IHasName Championship
        {
            get { return championship; }
            set { championship = value; }
        }
    }

    public class ChampionshipResultTemplateSelector:DataTemplateSelector
    {
        public DataTemplate GroupTemplate { get; set; }
        public DataTemplate CustomerTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            ChampionshipResultsViewModel viewModel = (ChampionshipResultsViewModel) item;
            if(viewModel.Item.Category==ChampionshipWinningCategories.Druzynowa)
            {
                return GroupTemplate;    
            }
            var template = CustomerTemplate;
            return template;
        }
    }
}
