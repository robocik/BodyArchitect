using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Controls;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Module.StrengthTraining.Localization;
using BodyArchitect.Service.Model.TrainingPlans;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    public partial class TrainingPlanSuperSetsEditor : BaseWindow
    {
        private TrainingPlan plan;

        public TrainingPlanSuperSetsEditor()
        {
            InitializeComponent();
        }


        public void Fill(TrainingPlan plan)
        {
            this.plan = plan;
            exercisesTrainingPlanListView1.Fill(plan);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = exercisesTrainingPlanListView1.SelectedIndices.Count == 0;
            mnuRemoveSuperSet.Enabled = containsSuperSet();
            mnuJoinSuperSet.Enabled = exercisesTrainingPlanListView1.SelectedItems.Count > 1;
        }

        bool containsSuperSet()
        {
            foreach (ListViewItem selectedItem in exercisesTrainingPlanListView1.SelectedItems)
            {
                TrainingPlanEntry entry = (TrainingPlanEntry)selectedItem.Tag;
                var superSet = entry.Day.GetSuperSet(entry);
                if (superSet != null)
                {
                    return true;
                }
            }
            return false;
        }

        private void mnuJoinSuperSet_Click(object sender, EventArgs e)
        {
            TrainingPlanDay day=null;
            foreach (ListViewItem selectedItem in exercisesTrainingPlanListView1.SelectedItems)
            {
                TrainingPlanEntry entry = (TrainingPlanEntry)selectedItem.Tag;
                if(day!=null && entry.Day!=day)
                {
                    FMMessageBox.ShowError(StrengthTrainingEntryStrings.ErrorCannotCreateSuperSetDifferentDays);
                    return;
                }
                day = entry.Day;
            }

            foreach (ListViewItem selectedItem in exercisesTrainingPlanListView1.SelectedItems)
            {
                TrainingPlanEntry entry = (TrainingPlanEntry)selectedItem.Tag;
                var superSet=day.GetSuperSet(entry);
                if(superSet!=null)
                {
                    superSet.SuperSets.Remove(entry);
                }
            }

            SuperSet newSet = new SuperSet();
            foreach (ListViewItem selectedItem in exercisesTrainingPlanListView1.SelectedItems)
            {
                TrainingPlanEntry entry = (TrainingPlanEntry) selectedItem.Tag;
                newSet.SuperSets.Add(entry);
            }
            day.SuperSets.Add(newSet);

            //clear empty supersets or with only one exercise
            TrainingPlanChecker checker = new TrainingPlanChecker();
            checker.Process(plan);

            exercisesTrainingPlanListView1.Fill(plan);
        }

        private void mnuRemoveSuperSet_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem selectedItem in exercisesTrainingPlanListView1.SelectedItems)
            {
                TrainingPlanEntry entry = (TrainingPlanEntry)selectedItem.Tag;
                var superSet = entry.Day.GetSuperSet(entry);
                if (superSet != null)
                {
                    superSet.SuperSets.Remove(entry);
                }
            }

            //clear empty supersets or with only one exercise
            TrainingPlanChecker checker = new TrainingPlanChecker();
            checker.Process(plan);
            exercisesTrainingPlanListView1.Fill(plan);
        }

        private void exercisesTrainingPlanListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnRemoveFromSuperset.Enabled = containsSuperSet();
            btnJoinAsSuperset.Enabled = exercisesTrainingPlanListView1.SelectedItems.Count > 1;
        }
    }
}
