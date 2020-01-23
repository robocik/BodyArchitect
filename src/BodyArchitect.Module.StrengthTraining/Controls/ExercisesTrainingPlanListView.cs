using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Controls;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Module.StrengthTraining.Controls.SourceGridExtension;
using BodyArchitect.Module.StrengthTraining.Localization;
using BodyArchitect.Service.Model;
using BodyArchitect.Service.Model.TrainingPlans;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    public class ExercisesTrainingPlanListView:ListView
    {
        public ExercisesTrainingPlanListView()
        {
            UseAlternateRows = true;
            View = View.Details;
            FullRowSelect = true;
        }

        [DefaultValue(true)]
        public bool UseAlternateRows { get; set; }

        public void Fill(TrainingPlan plan)
        {
            SuperSetViewManager superSetManager = new SuperSetViewManager();

            Clear();
            Columns.Add("colExercise", StrengthTrainingEntryStrings.ExercisesTrainingPlanListView_ExerciseColumn, 200);
            Columns.Add("colExerciseType", StrengthTrainingEntryStrings.ExercisesTrainingPlanListView_MuscleColumn);
            for (int i = 0; i < plan.GetMaximumSeriesCount(); i++)
            {
                int setNumber = i + 1;
                Columns.Add("colSet" + setNumber, string.Format(StrengthTrainingEntryStrings.ExercisesTrainingPlanListView_SetColumn, setNumber));
            }
            for (int i = 0; i < plan.Days.Count; i++)
            {
                TrainingPlanDay day = plan.Days[i];
                ListViewGroup group = new ListViewGroup(string.IsNullOrEmpty(day.Name) ? StrengthTrainingEntryStrings.ExercisesTrainingPlanListView_EmptyDayName : day.Name);
                Groups.Add(group);

                bool alt = false;
                foreach (var planEntry in day.Entries)
                {
                    ExerciseDTO exercise = ObjectsReposidory.GetExercise(planEntry.ExerciseId);

                    ListViewItem item = new ListViewItem(new string[] { exercise.GetLocalizedName(), EnumLocalizer.Default.Translate(exercise.ExerciseType) });
                    if (alt && UseAlternateRows)
                    {
                        item.BackColor = Color.LightGray;
                    }
                    alt = !alt;
                    //get special color for superset entry
                    var superSet = day.GetSuperSet(planEntry);
                    if (superSet != null)
                    {
                        item.BackColor = superSetManager.GetSuperSetColor(superSet.SuperSetId.ToString());
                    }
                    item.Group = group;
                    Items.Add(item);
                    item.Tag = planEntry;
                    for (int j = 0; j < planEntry.Sets.Count; j++)
                    {
                        ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem();
                        item.SubItems.Add(subItem);
                        subItem.Text = planEntry.Sets[j].GetDisplayText();
                        subItem.Tag = planEntry.Sets[j];
                    }

                }

            }
        }
    }
}
