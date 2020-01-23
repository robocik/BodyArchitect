using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Service.Model;
using DevExpress.XtraEditors;
using BodyArchitect.Controls.Localization;

namespace BodyArchitect.Controls.UserControls
{
    public partial class usrMyTrainingStatus : DevExpress.XtraEditors.XtraUserControl
    {
        private MyTrainingDTO myTraining;
        private bool readOnly;

        public usrMyTrainingStatus()
        {
            InitializeComponent();
            fillSuperTips();
        }

        private void fillSuperTips()
        {
            ControlHelper.AddSuperTip(dtaStartDate, lblStartDate.Text, SuperTips.usrMyTrainingStatus_StartDate);
            ControlHelper.AddSuperTip(dtaEndDate, lblEndDate .Text, SuperTips.usrMyTrainingStatus_EndDate);
            ControlHelper.AddSuperTip(btnAbortMyTraining, btnAbortMyTraining.Text, SuperTips.usrMyTrainingStatus_Abort);
        }

        private Color getStatusColor(TrainingEnd value)
        {
            if(value==TrainingEnd.Break)
            {
                return Color.Red;
            }
            else if(value==TrainingEnd.Complete)
            {
                return Color.Green;
            }
            return Color.Blue;
        }

        public void Fill(MyTrainingDTO myTraining)
        {
            this.myTraining = myTraining;
            lblTrainingState.Text = EnumLocalizer.Default.Translate(myTraining.TrainingEnd);
            lblTrainingState.ForeColor = getStatusColor(myTraining.TrainingEnd);
            if (myTraining.PercentageCompleted.HasValue)
            {
                lblPercentageResult.Text = string.Format(ApplicationStrings.CompletePercentageResult, myTraining.PercentageCompleted.Value);
                lblPercentageResult.Visible = true;
            }
            else
            {
                lblPercentageResult.Visible = false;
            }
            dtaStartDate.DateTime = myTraining.StartDate;
            if(myTraining.EndDate.HasValue)
            {
                lblEndDate.Visible = true;
                dtaEndDate.Visible = true;
                btnAbortMyTraining.Visible = false;
                dtaEndDate.DateTime = myTraining.EndDate.Value;
            }
            else
            {
                lblEndDate.Visible = false;
                dtaEndDate.Visible = false;
                btnAbortMyTraining.Visible = true;
            }

        }

        private void btnAbortMyTraining_Click(object sender, EventArgs e)
        {
            myTraining.Abort();
            ReadOnly = true;
        }

        public bool ReadOnly
        {
            get { return readOnly; }
            set
            {
                readOnly=value;
                btnAbortMyTraining.Enabled = !readOnly && (myTraining==null || myTraining.TrainingEnd == TrainingEnd.NotEnded);
            }
        }
    }
}
