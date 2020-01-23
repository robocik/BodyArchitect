using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Module.StrengthTraining.Localization;
using BodyArchitect.Service.Model;
using BodyArchitect.Service.Model.TrainingPlans;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    public class ExercisesListView:ListView
    {
        private ColumnHeader colName;
        private ColumnHeader colMuscle;
        private ListViewGroup publishedExercised;
        private ColumnHeader colId;
        private ListViewGroup privateExercises;

        public ExercisesListView()
        {
            InitializeComponent();

            publishedExercised = new ListViewGroup("published",StrengthTrainingEntryStrings.ExercisesPublishedGroup);
            privateExercises = new ListViewGroup("private",StrengthTrainingEntryStrings.ExercisesPrivateGroup);
            Groups.Add(publishedExercised);
            Groups.Add(privateExercises);
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ListViewGroupCollection Groups
        {
            get { return base.Groups; }
        }

        public void Fill(TrainingPlan trainingPlan)
        {
            Items.Clear();
            foreach (var planDay in trainingPlan.Days)
            {
                foreach (var planEntry in planDay.Entries)
                {
                    var exercise = ObjectsReposidory.GetExercise(planEntry.ExerciseId);
                    ListViewItem item = new ListViewItem(new string[] { exercise.GetLocalizedName(), EnumLocalizer.Default.Translate(exercise.ExerciseType), exercise.GlobalId.ToString() });
                    item.Group = exercise.Status == PublishStatus.Published ? publishedExercised : privateExercises;
                    item.Tag = exercise;
                    Items.Add(item);
                }
            }
        }

        #region InitializeComponent

        private void InitializeComponent()
        {
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colMuscle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // colName
            // 
            this.colName.Text = "Name";
            this.colName.Width = 100;
            // 
            // colMuscle
            // 
            this.colMuscle.Text = "Muscle";
            // 
            // colId
            // 
            this.colId.Text = "Id";
            // 
            // ExercisesListView
            // 
            this.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colMuscle,
            this.colId});
            this.FullRowSelect = true;
            this.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.MultiSelect = false;
            this.View = System.Windows.Forms.View.Details;
            this.ResumeLayout(false);

        }

        #endregion
    }
}
