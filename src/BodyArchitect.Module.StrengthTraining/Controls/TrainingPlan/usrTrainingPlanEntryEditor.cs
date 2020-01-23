using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Controls;
using BodyArchitect.Module.StrengthTraining.Localization;
using BodyArchitect.Service.Model.TrainingPlans;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    public partial class usrTrainingPlanEntryEditor : DevExpress.XtraEditors.XtraUserControl
    {
        private TrainingPlanEntry entry;
        public event EventHandler<ParameterEventArgs<TrainingPlanEntry>> TrainingPlanChanged;


        public usrTrainingPlanEntryEditor()
        {
            InitializeComponent();
            fillSuperTips();
        }

        void fillSuperTips()
        {
            ControlHelper.AddSuperTip(this.txtComment, lblComment.Text, StrengthTrainingEntryStrings.TrainingPlanEntryEditor_CommentTXT);
            ControlHelper.AddSuperTip(this.exerciseLookUp1, lblExercise.Text, StrengthTrainingEntryStrings.TrainingPlanEntryEditor_ExerciseCMB);
            ControlHelper.AddSuperTip(this.txtRestSeconds, lblRestTime.Text, StrengthTrainingEntryStrings.TrainingPlanEntryEditor_RestTimeTXT);
        }

        public void Fill(TrainingPlanEntry entry)
        {
            this.entry = entry;
            if (entry != null)
            {
                exerciseLookUp1.EditValue = entry.ExerciseId;

                txtRestSeconds.Value = entry.RestSeconds;
                if (entry.Comment!=null)
                {
                    entry.Comment = entry.Comment.Replace("\n", "\r\n");
                }
                txtComment.Text = entry.Comment;
            }
        }

        private void exerciseLookUp1_EditValueChanged(object sender, EventArgs e)
        {
            entry.ExerciseId = (Guid)exerciseLookUp1.EditValue;
            if(TrainingPlanChanged!=null)
            {
                TrainingPlanChanged(this, new ParameterEventArgs<TrainingPlanEntry>(entry));
            }
        }

        private void txtRestTime_Validated(object sender, EventArgs e)
        {
            entry.RestSeconds = (int) txtRestSeconds.Value;
        }

        private void txtComment_Validated(object sender, EventArgs e)
        {
            entry.Comment = txtComment.Text;
        }
    }

    public class ParameterEventArgs<T>:EventArgs
    {
        private T entry;

        public ParameterEventArgs(T entry)
        {
            this.entry = entry;
        }

        public T Parameter
        {
            get { return entry; }
        }
    }
}
