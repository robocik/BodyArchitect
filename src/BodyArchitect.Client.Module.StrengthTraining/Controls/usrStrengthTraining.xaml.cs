using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using AvalonDock.Layout;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Module.StrengthTraining.Localization;
using BodyArchitect.Client.Module.StrengthTraining.Reports;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Service.V2.Model.Reports;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using BodyArchitect.Service.V2.Model.Validators;
using BodyArchitect.Shared;
using Visiblox.Charts;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls
{
    public partial class usrStrengthTraining : IWeakEventListener,IValidationControl
    {
        private StrengthTrainingViewModel viewModel;

        private StrengthTrainingEntryDTO StrengthTrainingEntry
        {
            get { return (StrengthTrainingEntryDTO) Entry; }
        }
        
        private usrStrengthTrainingDetails StrengthDetails
        {
            get { return (usrStrengthTrainingDetails) detailsControl; }
        }

        public override bool HasDetailsPane
        {
            get { return true; }
        }

        public override bool HasProgressPane
        {
            get { return true; }
        }

        public usrStrengthTraining()
        {
            InitializeComponent();
            PropertyChangedEventManager.AddListener(UserContext.Current, this, string.Empty);
            viewModel = new StrengthTrainingViewModel(UserContext.Current.Settings.SerieNumberComboBoxSelectedItem, ReadOnly);
            viewModel.ShowRestColumns = grid.ShowRestColumns;
            viewModel.ShowExerciseTypeColumn = grid.ShowExerciseTypeColumn;
            DataContext = viewModel;
        }

        protected override UI.UserControls.usrEntryObjectUserControl CreateProgressControl()
        {
            return new usrStrengthTrainingProgress();
        }

        protected override UI.UserControls.usrEntryObjectDetailsBase CreateDetailsControl()
        {
            //TODO:sprawdzic czy ta kontrolka nie wisi (memory leaks) bo nie odpinamy eventu
            var ctrl= new usrStrengthTrainingDetails();
            ctrl.usrWorkoutPlansChooser1.SelectedPlanDayChanged += usrWorkoutPlansChooser1_SelectedPlanDayChanged;
            ctrl.UpdateLicence();
            ctrl.UpdateReadOnly(ReadOnly);
            return ctrl;
        }


        void updateLicence()
        {
            if (detailsControl != null)
            {
                ((usrStrengthTrainingDetails)detailsControl).UpdateLicence();
            }
            viewModel.LicenceChanged();
        }

        private void usrWorkoutPlansChooser1_SelectedPlanDayChanged(object sender, EventArgs e)
        {
            //TODO:This event is raised twice every change - this could be improved later:)
            if (StrengthDetails.usrWorkoutPlansChooser1.SelectedPlanDay != null)
            {
                viewModel.TrainingPlanDay = StrengthDetails.usrWorkoutPlansChooser1.SelectedPlanDay;
            }

            if (StrengthDetails.usrWorkoutPlansChooser1.SelectedPlanDay == null || StrengthTrainingEntry.TrainingPlanItemId == StrengthDetails.usrWorkoutPlansChooser1.SelectedPlanDay.GlobalId)
            {
                updateToolbar();
                return;
            }
            TrainingBuilder builder = new TrainingBuilder();
            builder.FillRepetitionNumber = StrengthTraining.Default.FillRepetitionNumberFromPlan;
            builder.FillTrainingFromPlan(StrengthDetails.usrWorkoutPlansChooser1.SelectedPlanDay, StrengthTrainingEntry);
            viewModel.UpdateTrainingPlan(true);
            SetModifiedFlag();
            updateToolbar();
        }

        protected override void FillImplementation(SaveTrainingDayResult originalEntry)
        {
            ExercisesReposidory.Instance.BeginEnsure();//start fetching exercises
            viewModel.IsReadOnly = ReadOnly;
            viewModel.Fill(this.StrengthTrainingEntry);
            viewModel.SelectedGroup = GridGroupMode.None;


            DataContext = viewModel;
            changeSetNumbers();
            grid.Fill(this.StrengthTrainingEntry, viewModel);



            updateReadOnly();
            updateToolbar();

            displayNotificationAboutRecords(originalEntry);
        }

   
        private void displayNotificationAboutRecords(SaveTrainingDayResult saveResult)
        {
            if (saveResult==null)
            {
                return;
            }
            foreach (var currentRecord in saveResult.NewRecords)
            {
                RecordNotifyObject notifyObject = new RecordNotifyObject(currentRecord);
                MainWindow.Instance.ShowNotification(notifyObject);
            }
        }

        void updateReadOnly()
        {
            updateLicence();
            grid.IsReadOnly = ReadOnly;
            bool readOnly = ReadOnly;
            grid.IsReadOnly = readOnly;
            if (detailsControl != null)
            {
                detailsControl.UpdateReadOnly(ReadOnly);
            }
            viewModel.IsReadOnly = readOnly;
        }


        void changeSetNumbers()
        {
            if (StrengthTrainingEntry != null)
            {
                int serieCount = viewModel.SetNumber + 1;
                int trainingDayCount = StrengthTrainingEntry.GetMaximumSeriesCount();
                if (serieCount < trainingDayCount)
                {
                    BAMessageBox.ShowError(Strings.ErrorSerieColumnCount);
                    viewModel.SetNumber = trainingDayCount - 1;
                    return;
                }
            }
            UserContext.Current.Settings.SerieNumberComboBoxSelectedItem = viewModel.SetNumber;
            grid.BuildGridColumns(viewModel.SetNumber + 1);
        }

        protected override void UpdateEntryObjectImplementation()
        {
            viewModel.GetStrengthTrainingEntry();
        }


        public override void AfterSave(bool isWindowClosing)
        {
            if (!isWindowClosing && StrengthDetails!=null)
            {
                StrengthDetails.usrWorkoutPlansChooser1.Fill(StrengthTrainingEntry);
            }
        }


        //private void cmbSets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (entry != null)
        //    {
        //        int serieCount = cmbSets.SelectedIndex+1;
        //        int trainingDayCount = entry.GetMaximumSeriesCount();
        //        if (serieCount < trainingDayCount)
        //        {
        //            BAMessageBox.ShowError(Strings.ErrorSerieColumnCount);
        //            cmbSets.SelectedIndex = trainingDayCount - 1;
        //            return;
        //        }
        //    }
        //    UserContext.Settings.SerieNumberComboBoxSelectedItem = this.cmbSets.SelectedIndex;
        //    grid.BuildGridColumns(cmbSets.SelectedIndex + 1);
        //}
        

        

        private void btnDeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedEntries();
        }

        public void DeleteSelectedEntries()
        {
            var rows = grid.SelectedItems;
            if (ReadOnly || BAMessageBox.AskYesNo(StrengthTrainingEntryStrings.QAskDeleteExerciseRow) == MessageBoxResult.No)
            {
                return;
            }
            for (int j = rows.Count - 1; j >= 0; j--)
            {
                var row = (StrengthTrainingItemViewModel)rows[j];
                if (!row.IsNew)
                {
                    viewModel.Items.Remove(row);
                }
            }
            SetModifiedFlag();
        }


        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button) sender;
            StrengthTrainingItemViewModel item = (StrengthTrainingItemViewModel) btn.Tag;
            if (item != null && !item.IsNew)
            {
                grid.SelectedItem = item;
                DeleteSelectedEntries();
            }
        }


        private void grid_KeyDown(object sender, KeyEventArgs e)
        {
            var cell=e.OriginalSource as DataGridCell;//we are not editing the value (in textbox for example)
            if (e.Key == Key.Delete && cell != null )
            {
                deleteSelectedSet();
            }
        }

        private void deleteSelectedSet()
        {
            var viewModel = (StrengthTrainingItemViewModel) grid.CurrentCell.Item;
            int setNumber = grid.GetCurrentSetIndex();
            if (setNumber >= 0)
            {
                viewModel.RemoveSet(setNumber);
                SetModifiedFlag();
            }
        }

        

        public bool CanDelete
        {
            get
            {
                bool canDelete = false;
                var rows = grid.SelectedItems;
                for (int j = rows.Count - 1; j >= 0; j--)
                {
                    var row = (StrengthTrainingItemViewModel)rows[j];
                    if (!row.IsNew)
                    {
                        canDelete = true;
                        break;
                    }
                }

                return canDelete && !ReadOnly;
            }
        }

        void updateToolbar()
        {
            bool isExistingItemSelected = CanDelete;
            viewModel.CanMoveUp = isExistingItemSelected &&grid.GroupMode==GridGroupMode.None;
            viewModel.CanMoveDown = isExistingItemSelected && grid.GroupMode == GridGroupMode.None;
            //CanShowPlan = entry.TrainingPlanItemId.HasValue && usrWorkoutPlansChooser1.SelectedPlanDay != null;
            viewModel.CanShowPlan = StrengthTrainingEntry.TrainingPlanItemId.HasValue;
            viewModel.CanJoinSets = isExistingItemSelected;
            viewModel.CanSplitSets = isExistingItemSelected;
            viewModel.CanDeleteEntry = isExistingItemSelected;

            var currentSetCell = grid.GetSetCellForCurrentRest();
            //viewModel.CanStartTimer = isExistingItemSelected && !ReadOnly && currentSetCell != null && (currentSetCell.Set != null || grid.IsNewSetSelected());
            viewModel.CanStartTimer = isExistingItemSelected && !ReadOnly && (currentSetCell != null )  && (currentSetCell.Set != null || grid.IsNewSetSelected());
            
            viewModel.ShowDeleteEntry = !ReadOnly;
            viewModel.ShowSetNumbers = !ReadOnly;
            viewModel.ShowMoveUp = !ReadOnly;
            viewModel.ShowMoveDown = !ReadOnly;
            viewModel.ShowSuperSet = !ReadOnly;
            viewModel.IsTimerVisible = wnd.Visibility == Visibility.Visible;
            //btnDeleteSelected.SetVisible(!ReadOnly);
            //cmbSets.IsEnabled = !ReadOnly;
            //tbMoveUp.SetVisible(!ReadOnly);
            //tbMoveDown.SetVisible(!ReadOnly);
            //tbSuperSet.SetVisible(!ReadOnly);

        }

        private void grid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            SetModifiedFlag();
            updateToolbar();
        }

        private void grid_CurrentCellChanged(object sender, EventArgs e)
        {
            grid.CommitEdit();
            SetModifiedFlag();
        }

        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //DataGridCell cell = sender as DataGridCell;
            //BADataGrid.GridColumnFastEdit(cell, e);
        }

        private void DataGridCell_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;
            BADataGrid.GridColumnFastEdit(cell, e);
        }

        private void timer_Closed(object sender, EventArgs e)
        {
            if(wnd.DialogResult.HasValue && wnd.DialogResult.Value)
            {
                try
                {
                    grid.Focus();
                    grid.SelectedCells.Clear();
                    grid.SelectedCells.Add(wnd.Cell);
                    grid.CurrentCell = wnd.Cell;

                    var setViewModel = grid.GetSetCellForCurrentRest();
                    var setCell = setViewModel;
                    setCell.EnsureSetExists();

                    if (grid.IsRestColumnSelected())
                    {
                        setCell.Set.EndTime = wnd.StartTime;
                        var item = setCell.Set.StrengthTrainingItem;

                        var index = item.Series.IndexOf(setCell.Set) + 1;
                        var nextSet = setCell.Parent.GetSetViewModel(index);
                        nextSet.EnsureSetExists();

                        var newIndex = item.Series.IndexOf(nextSet.Set);
                        if (newIndex == -1)
                        {
                            item.AddSerie(nextSet.Set);
                        }
                        nextSet.Set.StartTime = wnd.EndTime;
                        setCell.Parent.UpdateReadOnly();
                        setCell.Parent.UpdateSetsDisplay();

                        var cell = grid.GetCell(grid.GetSelectedRow(), grid.CurrentCell.Column.DisplayIndex + 1);
                        if (cell != null)
                        {
                            cell.IsSelected = true;
                            BADataGrid.GridColumnFastEdit(cell, new RoutedEventArgs());
                        }
                    }
                    else
                    {

                        setCell.Set.StartTime = wnd.StartTime;
                        setCell.Set.EndTime = wnd.EndTime;
                        if (setCell.Set.StrengthTrainingItem.Exercise.ExerciseType == ExerciseType.Cardio)
                        {
                            setCell.Set.Weight = (int)(setCell.Set.EndTime.Value - setCell.Set.StartTime.Value).TotalSeconds;
                        }
                        setCell.UpdateDisplay();
                        setCell.Parent.UpdateReadOnly();
                        var cell = grid.GetCell(grid.GetSelectedRow(), grid.CurrentCell.Column.DisplayIndex);
                        BADataGrid.GridColumnFastEdit(cell, new RoutedEventArgs());
                    }

                }
                catch (Exception ex)
                {
                    ExceptionHandler.Default.Process(ex, "ErrTimerGridChanged".TranslateStrength(),ErrorWindow.MessageBox);
                }
                finally
                {
                    updateToolbar();
                }
                
            }
        }

        private void rbtnStartTimer_Click(object sender, RoutedEventArgs e)
        {
            if (!viewModel.CanStartTimer  || !UIHelper.EnsurePremiumLicence())
            {
                return;
            }
            wnd.Cell = grid.CurrentCell;
            wnd.Show();
            updateToolbar();
        }

        private void tbMoveUp_Click(object sender, RoutedEventArgs e)
        {
            grid.MoveOneUp();
            SetModifiedFlag();
        }
        
        private void tbMoveDown_Click(object sender, RoutedEventArgs e)
        {
            grid.MoveOneDown();
            SetModifiedFlag();
        }

        private void tbAddSuperSet_Click(object sender, RoutedEventArgs e)
        {
            grid.AddSupersets();
            SetModifiedFlag();
        }

        private void tbRemoveSuperSet_Click(object sender, RoutedEventArgs e)
        {
            grid.RemoveSupersets();
            SetModifiedFlag();
        }


        //private void cmbGroupBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (grid!=null)
        //    {
        //        grid.GroupItemsBy((GridGroupMode)cmbGroupBy.SelectedIndex);    
        //    }
            
        //}
        

        private void mnuAdditionalInfo_Click(object sender, RoutedEventArgs e)
        {
            var setViewModel = grid.GetCurrentSetCell();
            setViewModel.EnsureSetExists();
            SetInfoWindow wnd = new SetInfoWindow(setViewModel.Set, ReadOnly);
            if (wnd.ShowDialog() ==true)
            {
                setViewModel.RefreshToolTip();
                setViewModel.UpdateDisplay();
                SetModifiedFlag();
            }
        }

        private void ContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var item = grid.CurrentCell.Item as StrengthTrainingItemViewModel;
            var selectedSet = grid.GetCurrentSetCell();
            if (item == null || item.IsNew || selectedSet == null || selectedSet.Set==null)
            {
                e.Handled = true;
                return;
            }
            grid.EnsureNotEditMode();
        }

        

        private void tbShowPlan_Click(object sender, RoutedEventArgs e)
        {
            //if (!WorkoutPlansReposidory.Instance.IsLoaded)
            //{//if we don't have plans retrieved then do this first
            //    Func<bool, List<TrainingPlanItem>> fillMethod = usrWorkoutPlansChooser1.FillWorkoutPlans;
            //    PleaseWait.Run(x =>
            //                       {
            //                           fillMethod(true);
            //                           UIHelper.BeginInvoke(() =>
            //                                                    {
            //                                                        usrWorkoutPlansChooser1.Fill(entry);
            //                                                        viewModel.ShowPlanItems = viewModel.IsPlanShowed;
            //                                                    }, Dispatcher);
            //                       }, true);
            //    return;
            //}
            //viewModel.ShowPlanItems = viewModel.IsPlanShowed;
        }


        private void cmbSets_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            changeSetNumbers();
        }

        private void cmbGroups_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (grid != null)
            {
                grid.GroupMode = viewModel.SelectedGroup;
            }
        }

        

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            UIHelper.BeginInvoke(updateLicence,Dispatcher);
            return true;
        }

        public bool ValidateControl()
        {
            var incorrectSets = SerieValidator.GetIncorrectSets(StrengthTrainingEntry);

            if (incorrectSets.Count > 0)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("usrStrengthTraining_MsgIncorrectSetsValidation".TranslateStrength());
                foreach (var incorrectSet in incorrectSets.GroupBy(x => x.StrengthTrainingItem.Exercise))
                {
                    builder.Append(incorrectSet.Key.GetLocalizedName() + ": ");
                    foreach (var serieDto in incorrectSet)
                    {
                        builder.Append(serieDto.GetDisplayText(WorkoutPlanOperationHelper.SetDisplayMode.Medium) + ", ");
                    }
                    builder.AppendLine();
                }
                if (BAMessageBox.AskWarningYesNo(builder.ToString()) == MessageBoxResult.No)
                {
                    return false;
                }
            }
            return true;
        }

        private void mnuDeleteSet_Click(object sender, RoutedEventArgs e)
        {
            deleteSelectedSet();
        }

        private void rbtnShowRestColumns_Click(object sender, RoutedEventArgs e)
        {
            grid.ShowRestColumns = viewModel.ShowRestColumns;
            
        }

        private void rbtnShowExerciseTypeColumn_Click(object sender, RoutedEventArgs e)
        {
            grid.ShowExerciseTypeColumn = viewModel.ShowExerciseTypeColumn;
        }

        private void btnSetRestTime_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            StrengthTrainingItemViewModel item = (StrengthTrainingItemViewModel)btn.Tag;
            if (item != null && !item.IsNew)
            {
                SetRestTimeWindow dlg = new SetRestTimeWindow();
                if (dlg.ShowDialog() == true)
                {
                    item.Item.SetRestTime(dlg.TimeSpan);
                    item.UpdateSetsDisplay();
                }
            }
        }
    }
}
