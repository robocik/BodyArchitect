using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Module.StrengthTraining.Localization;
using BodyArchitect.Service.Model;
using BodyArchitect.Service.Model.TrainingPlans;
using BodyArchitect.Shared;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using BodyArchitect.Common;
using BodyArchitect.Controls;
using BodyArchitect.Common.Controls;
using BodyArchitect.Module.StrengthTraining.Model;
using ControlHelper = BodyArchitect.Controls.ControlHelper;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    public partial class usrStrengthTraining : usrBaseControl, IEntryObjectControl
    {
        StrengthTrainingEntryDTO strengthEntry;
        

        public usrStrengthTraining()
        {
            InitializeComponent();
            fillSuperTips();
            
        }

       
        void fillSuperTips()
        {
            ControlHelper.AddSuperTip(this.txtComment, grInfo.Text, StrengthTrainingEntryStrings.usrStrengthTraining_Tip_CommentTE);
            ControlHelper.AddSuperTip(this.teStart, lblStart.Text, StrengthTrainingEntryStrings.usrStrengthTraining_Tip_StartTimeTE);
            ControlHelper.AddSuperTip(this.teEndTime, lblEndTime.Text, StrengthTrainingEntryStrings.usrStrengthTraining_Tip_EndTimeTE);
        }

        public void Fill(EntryObjectDTO entry)
        {
            strengthEntry = (StrengthTrainingEntryDTO)entry;
            txtComment.Text = strengthEntry.Comment;
            teStart.Time = strengthEntry.StartTime.HasValue ? strengthEntry.StartTime.Value : DateHelper.NullDateTime;
            teEndTime.Time = strengthEntry.EndTime.HasValue ? strengthEntry.EndTime.Value : DateHelper.NullDateTime;
            usrIntensity1.Intensity = strengthEntry.Intensity;

            usrReportStatus1.Fill(entry);


            usrWorkoutPlansChooser1.Fill(strengthEntry);
            usrTrainingDaySourceGrid1.Fill(strengthEntry, usrWorkoutPlansChooser1.SelectedPlanDay,ReadOnly);

            updateReadOnlyMode();
        }

        

        
        public void UpdateEntryObject()
        {
            usrReportStatus1.Save(strengthEntry);
            var entries = usrTrainingDaySourceGrid1.GetTrainingDayEntries();
            strengthEntry.Entries = new List<StrengthTrainingItemDTO>();
            foreach (StrengthTrainingItemDTO dayEntry in entries)
            {
                strengthEntry.AddEntry(dayEntry);
            }
            strengthEntry.Comment = txtComment.Text;
            strengthEntry.Intensity=usrIntensity1.Intensity;
            strengthEntry.StartTime = DateHelper.GetCorrectDateTime(teStart.Time);
            strengthEntry.EndTime = DateHelper.GetCorrectDateTime(teEndTime.Time);
        }

        public void AfterSave(bool isWindowClosing)
        {
            //luTrainingPlans.Enabled = strengthEntry.Id == Constants.UnsavedObjectId;
            if (!isWindowClosing)
            {
                usrWorkoutPlansChooser1.Fill(strengthEntry);
            }
        }

        void updateReadOnlyMode()
        {
            bool readOnly = ReadOnly;
            txtComment.Properties.ReadOnly = readOnly;
            this.usrReportStatus1.ReadOnly = readOnly;
            usrReportStatus1.Visible = !ReadOnly;
            this.teStart.Properties.ReadOnly = readOnly;
            this.teEndTime.Properties.ReadOnly = readOnly;
            this.usrIntensity1.ReadOnly = readOnly;
        }

        [DefaultValue(false)]
        public bool ReadOnly
        {
            get; set;
        }

        private void usrWorkoutPlansChooser1_SelectedPlanDayChanged(object sender, EventArgs e)
        {
            TrainingBuilder builder = new TrainingBuilder();
            builder.FillRepetitionNumber = Options.StrengthTraining.Default.FillRepetitionNumberFromPlan;
            builder.FillTrainingFromPlan(usrWorkoutPlansChooser1.SelectedPlanDay, strengthEntry);
            usrTrainingDaySourceGrid1.Fill(strengthEntry, usrWorkoutPlansChooser1.SelectedPlanDay, ReadOnly);
        }

        

    }

    
}
