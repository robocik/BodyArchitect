using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Controls;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Module.StrengthTraining.Localization;
using BodyArchitect.Service.Model;
using BodyArchitect.Service.Model.TrainingPlans;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    public partial class usrTrainingPlanSerieEditor : DevExpress.XtraEditors.XtraUserControl
    {
        public event EventHandler<ParameterEventArgs<TrainingPlanSerie>> TrainingPlanSetChanged;
        private TrainingPlanSerie set;

        public usrTrainingPlanSerieEditor()
        {
            InitializeComponent();
            fillSuperTips();

            foreach (var value in Enum.GetValues(typeof(SetType)))
            {
                ComboBoxItem item = new ComboBoxItem(value,EnumLocalizer.Default.Translate((SetType)value));
                cmbRepetitionsType.Properties.Items.Add(item);
            }
        }

        void fillSuperTips()
        {
            ControlHelper.AddSuperTip(this.txtComment, lblComment.Text, StrengthTrainingEntryStrings.SerieInfo_CommentTE);
            ControlHelper.AddSuperTip(this.cmbRepetitionsType, lblRepetitionsType.Text, StrengthTrainingEntryStrings.SetEditor_RepetitionType);
            ControlHelper.AddSuperTip(this.txtRepetitionsRange, lblRepetitionsRange.Text, StrengthTrainingEntryStrings.SetEditor_RepetitionsRange);
            ControlHelper.AddSuperTip(this.cmbDropSet, lblDropSet.Text, StrengthTrainingEntryStrings.SerieInfo_DropSetCMB);
        }

        public void Fill(TrainingPlanSerie set)
        {
            this.set = set;
            if(set!=null)
            {
                cmbRepetitionsType.SelectedIndex = (int)set.RepetitionsType;
                cmbDropSet.SelectedIndex = (int)set.DropSet;
                txtRepetitionsRange.EditValue = set.ToStringRepetitionsRange();

                if (set.Comment != null)
                {
                    set.Comment = set.Comment.Replace("\n", "\r\n");
                }
                txtComment.Text = set.Comment;
            }
        }

        private void cmbRepetitionsType_SelectedIndexChanged(object sender, EventArgs e)
        {
            set.RepetitionsType = (TrainingPlanSerieRepetitions)cmbRepetitionsType.SelectedIndex;
            trainingPlanSerieChanged();
            //txtRepetitionsRange.Enabled = cmbRepetitionsType.SelectedIndex == 0;
            //lblRepetitionsRange.Enabled = cmbRepetitionsType.SelectedIndex == 0;
        }

        private void trainingPlanSerieChanged()
        {
            if(TrainingPlanSetChanged!=null)
            {
                TrainingPlanSetChanged(this, new ParameterEventArgs<TrainingPlanSerie>(set));
            }
        }

        private void txtRepetitionsRange_Validated(object sender, EventArgs e)
        {
            set.FromString(txtRepetitionsRange.Text);
            trainingPlanSerieChanged();
            
        }

        private void txtComment_Validated(object sender, EventArgs e)
        {
            set.Comment = txtComment.Text;
        }

        private void cmbDropSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            set.DropSet = (DropSetType)cmbDropSet.SelectedIndex;
            trainingPlanSerieChanged();
        }

    }
}
