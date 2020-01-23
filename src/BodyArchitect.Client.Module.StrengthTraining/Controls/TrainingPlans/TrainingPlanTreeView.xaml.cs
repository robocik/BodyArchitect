using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.StrengthTraining.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls.TrainingPlans
{
    /// <summary>
    /// Interaction logic for TrainingPlanTreeView.xaml
    /// </summary>
    public partial class TrainingPlanTreeView
    {
        public event EventHandler SelectedItemChanged;

        private TrainingPlanViewModel viewModel;

        public TrainingPlanTreeView()
        {
            InitializeComponent();
            updateToolbar();
        }

        public void FillDetailsTree(TrainingPlanViewModel viewModel)
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
        }

        private void tbMoveUp_Click(object sender, RoutedEventArgs e)
        {
            var selItem = (IRepositionableChild)tvDetails.SelectedItem;
            IRepositionableChild selectedDay = selItem;
            if (selectedDay != null && ((TrainingPlanTreeItemViewModel)selectedDay).CanMoveUp)
            {
                selectedDay.RepositionableParent.RepositionEntry(selectedDay.Position, selectedDay.Position - 1);
                viewModel.SetModifiedFlag();
                updateToolbar();
            }
        }

        private void tbMoveDown_Click(object sender, RoutedEventArgs e)
        {
            var node = (IRepositionableChild)tvDetails.SelectedItem;
            IRepositionableChild selectedDay = node;
            if (selectedDay != null && ((TrainingPlanTreeItemViewModel)selectedDay).CanMoveDown)
            {
                selectedDay.RepositionableParent.RepositionEntry(selectedDay.Position, selectedDay.Position + 1);
                viewModel.SetModifiedFlag();
                updateToolbar();
            }
        }

        private void tbNewDay_Click(object sender, RoutedEventArgs e)
        {
            InputWindow wnd = new InputWindow(true);
            wnd.Value = string.Format(StrengthTrainingEntryStrings.TrainingPlanNewDayName,viewModel.Days.Count + 1);
            if (wnd.ShowDialog() == true)
            {
                viewModel.AddDays(wnd.Value);
            }
        }

        
        public T GetSelected<T>() where T : class
        {
            if(viewModel==null)
            {
                return null;
            }
            var item = viewModel.SelectedItem;
            return item as T;
        }

        private void tbEditDay_Click(object sender, RoutedEventArgs e)
        {
            var day=GetSelected<TrainingPlanDayViewModel>();
            if ( day!= null)
            {
                InputWindow wnd = new InputWindow(true);
                wnd.Value = day.Header;
                if (wnd.ShowDialog() == true)
                {
                    day.Header = wnd.Value;
                }
            }
        }

        private void tbDeleteDay_Click(object sender, RoutedEventArgs e)
        {
            if (GetSelected<TrainingPlanDayViewModel>() == null)
            {
                return;
            }
            if (BAMessageBox.AskYesNo(StrengthTrainingEntryStrings.QDeleteTrainingDay) == MessageBoxResult.Yes)
            {
                viewModel.DeleteDay(GetSelected<TrainingPlanDayViewModel>());
            }
        }

        private void tbNewEntry_Click(object sender, RoutedEventArgs e)
        {
            var day = GetSelected<TrainingPlanDayViewModel>();
            if (day != null)
            {
                viewModel.AddEntry(day);
            }
        }

        private void tbDeleteEntry_Click(object sender, RoutedEventArgs e)
        {
            var entry = GetSelected<TrainingPlanEntryViewModel>();
            if (entry == null)
            {
                return;
            }
            if (BAMessageBox.AskYesNo(StrengthTrainingEntryStrings.QDeleteTrainingPlanEntry) == MessageBoxResult.Yes)
            {
                viewModel.DeleteEntry(entry);
            }
        }

        private void tbNewSet_Click(object sender, RoutedEventArgs e)
        {
            var entry = GetSelected<TrainingPlanEntryViewModel>();
            if (entry == null)
            {
                return;
            }
            viewModel.AddSet(entry);
        }

        void updateToolbar()
        {
            if (viewModel != null)
            {
                viewModel.UpdateToolbar();
            }
        }

        private void tbDeleteSet_Click(object sender, RoutedEventArgs e)
        {
            var set = GetSelected<TrainingPlanSetViewModel>();
            if (set == null)
            {
                return;
            }

            if (BAMessageBox.AskYesNo(StrengthTrainingEntryStrings.QAskDeleteSet) == MessageBoxResult.Yes)
            {
                viewModel.DeleteSet(set);
            }
        }

        private void tvDetails_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item=tvDetails.SelectedItem;
            viewModel.SelectedItem = item as TrainingPlanTreeItemViewModel;
            updateToolbar();
            if(SelectedItemChanged!=null)
            {
                SelectedItemChanged(this, EventArgs.Empty);
            }
        }
    }

    
}
