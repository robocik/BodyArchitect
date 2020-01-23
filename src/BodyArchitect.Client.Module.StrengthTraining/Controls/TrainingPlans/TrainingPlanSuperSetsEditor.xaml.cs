using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BodyArchitect.Client.Module.StrengthTraining.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls.TrainingPlans
{
    /// <summary>
    /// Interaction logic for TrainingPlanSuperSetsEditor.xaml
    /// </summary>
    public partial class TrainingPlanSuperSetsEditor
    {
        private TrainingPlan plan;
        

        public TrainingPlanSuperSetsEditor()
        {
            InitializeComponent();
        }


        public void Fill(TrainingPlan plan)
        {
            this.plan = plan;
            GridView gridView = (GridView) lvExercises.View;
            gridView.Columns.Clear();
            GridViewColumn colExercise = new GridViewColumn();
            colExercise.Header = StrengthTrainingEntryStrings.ExercisesTrainingPlanListView_ExerciseColumn;
            Binding binding = new Binding("ExerciseName");
            colExercise.DisplayMemberBinding = binding;
            colExercise.Width = 200;
            gridView.Columns.Add(colExercise);
            GridViewColumn colExerciseType = new GridViewColumn();
            colExerciseType.DisplayMemberBinding = new Binding("ExerciseType");
            colExerciseType.Header = StrengthTrainingEntryStrings.ExercisesTrainingPlanListView_MuscleColumn;
            gridView.Columns.Add(colExerciseType);
            for (int i = 0; i < plan.GetMaximumSeriesCount(); i++)
            {
                GridViewColumn colSet = new GridViewColumn();
                colSet.DisplayMemberBinding = new Binding(i.ToString());
                colSet.Header = string.Format(StrengthTrainingEntryStrings.ExercisesTrainingPlanListView_SetColumn,i+1);
                gridView.Columns.Add(colSet);
            }

            SuperSetViewManager superSetManager = new SuperSetViewManager();
            List<TrainingPlanSetEditorItemViewModel> items = new List<TrainingPlanSetEditorItemViewModel>();
            for (int i = 0; i < plan.Days.Count; i++)
            {
                TrainingPlanDay day = plan.Days[i];
                foreach (var planEntry in day.Entries)
                {
                    var itemViewModel = new TrainingPlanSetEditorItemViewModel(planEntry);
                    if (!string.IsNullOrEmpty(planEntry.GroupName))
                    {
                        var color = superSetManager.GetSuperSetColor(planEntry.GroupName);
                        itemViewModel.Background = new SolidColorBrush(color);
                    }
                    
                    items.Add(itemViewModel);

                }

            }

            lvExercises.ItemsSource = items;

            CollectionView myView = (CollectionView)CollectionViewSource.GetDefaultView(lvExercises.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Day");
            myView.GroupDescriptions.Add(groupDescription);
        }


        bool containsSuperSet()
        {
            foreach (TrainingPlanSetEditorItemViewModel selectedItem in lvExercises.SelectedItems)
            {
                //TrainingPlanEntry entry = selectedItem.Entry;
                //var superSet = entry.Day.GetSuperSet(entry);
                //if (superSet != null)
                //{
                //    return true;
                
                //}
                if(!string.IsNullOrEmpty(selectedItem.Entry.GroupName))
                {
                    return true;
                }
            }
            return false;
        }



        private void btnJoin_Click(object sender, RoutedEventArgs e)
        {
            TrainingPlanDay day = null;
            foreach (TrainingPlanSetEditorItemViewModel selectedItem in lvExercises.SelectedItems)
            {
                TrainingPlanEntry entry = selectedItem.Entry;
                if (day != null && entry.Day != day)
                {
                    BAMessageBox.ShowError(StrengthTrainingEntryStrings.ErrorCannotCreateSuperSetDifferentDays);
                    return;
                }
                day = entry.Day;
            }

            //foreach (TrainingPlanSetEditorItemViewModel selectedItem in lvExercises.SelectedItems)
            //{
            //    TrainingPlanEntry entry = selectedItem.Entry;
            //    var superSet = day.GetSuperSet(entry);
            //    if (superSet != null)
            //    {
            //        superSet.SuperSets.Remove(entry);
            //    }
            //}
            var superSetGroup = Guid.NewGuid();//TODO:Change this to some better name (at least in display)
            var selectedEntries=lvExercises.SelectedItems.Cast<TrainingPlanSetEditorItemViewModel>().Select(x => x.Entry);
            foreach (var trainingPlanEntry in selectedEntries)
            {
                trainingPlanEntry.GroupName = superSetGroup.ToString();
            }

            //SuperSet newSet = new SuperSet();
            //foreach (TrainingPlanSetEditorItemViewModel selectedItem in lvExercises.SelectedItems)
            //{
            //    TrainingPlanEntry entry = selectedItem.Entry;
            //    newSet.SuperSets.Add(entry);
            //}
            //day.SuperSets.Add(newSet);

            //clear empty supersets or with only one exercise
            TrainingPlanChecker checker = new TrainingPlanChecker();
            checker.Process(plan);

            Fill(plan);
        }

        private void btnSplit_Click(object sender, RoutedEventArgs e)
        {
            //foreach (TrainingPlanSetEditorItemViewModel selectedItem in lvExercises.SelectedItems)
            //{
            //    TrainingPlanEntry entry = selectedItem.Entry;
            //    var superSet = entry.Day.GetSuperSet(entry);
            //    if (superSet != null)
            //    {
            //        superSet.SuperSets.Remove(entry);
            //    }
            //}
            var selectedEntries = lvExercises.SelectedItems.Cast<TrainingPlanSetEditorItemViewModel>().Select(x => x.Entry);
            foreach (var trainingPlanEntry in selectedEntries)
            {
                trainingPlanEntry.GroupName = null;
            }

            //clear empty supersets or with only one exercise
            TrainingPlanChecker checker = new TrainingPlanChecker();
            checker.Process(plan);
            Fill(plan);
        }

        private void lvExercises_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnRemoveFromSuperset.IsEnabled = containsSuperSet();

            btnJoinAsSuperset.IsEnabled = lvExercises.SelectedItems.Count > 1;
        }

        private void ContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            e.Handled = lvExercises.SelectedItems.Count == 0;
            mnuRemoveSuperSet.IsEnabled = containsSuperSet();
            mnuJoinSuperSet.IsEnabled = lvExercises.SelectedItems.Count > 1;
        }

    }
}
