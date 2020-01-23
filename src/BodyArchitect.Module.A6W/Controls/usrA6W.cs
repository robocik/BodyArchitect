using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Logger;
using BodyArchitect.Service.Model;
using BodyArchitect.Shared;
using DevExpress.XtraEditors;
using BodyArchitect.Module.A6W.Localization;
using BodyArchitect.Common;
using BodyArchitect.Common.Controls;
using BodyArchitect.Controls;
using ControlHelper = BodyArchitect.Controls.ControlHelper;
namespace BodyArchitect.Module.A6W.Controls
{
    public partial class usrA6W : usrBaseControl, IEntryObjectControl, IValidationControl
    {
        private A6WEntryDTO entry;
        static readonly Color selectedDayColor = Color.LightBlue;
        private bool isReadOnly;
        private bool IsTrainingInProgress1;
        private Dictionary<int, A6WEntryDTO> entries;

        public usrA6W()
        {
            InitializeComponent();
            this.defaultToolTipController1.DefaultController.Active = UserContext.Settings.GuiState.ShowToolTips;
            fillSuperTips();
            //fillDaysCombo();
        }

        void fillSuperTips()
        {
            ControlHelper.AddSuperTip(this.defaultToolTipController1.DefaultController, this.lvA6W, string.Empty, A6WEntryStrings.A6W_LV);
            ControlHelper.AddSuperTip(this.rbCompleted, rbCompleted.Text, A6WEntryStrings.A6W_CompletedRB);
            ControlHelper.AddSuperTip(this.rbPartialCompleted, rbPartialCompleted.Text, A6WEntryStrings.A6W_PartialCompletedRB);
            ControlHelper.AddSuperTip(this.txtComment, grComment.Text, A6WEntryStrings.A6W_CommentTXT);
        }

        private void updateReadOnly(bool readOnly)
        {
            usrMyTrainingStatus1.ReadOnly = readOnly;
            panel1.Enabled = !readOnly;
            txtComment.Properties.ReadOnly = readOnly;
            usrReportStatus1.ReadOnly = readOnly;
            usrReportStatus1.Visible = !ReadOnly;
        }


        private void fillDaysCombo()
        {
            lvA6W.Items.Clear();
            IList<EntryObjectDTO> myEntries = new List<EntryObjectDTO>();
            if (!entry.MyTraining.IsNew)
            {
                myEntries = entry.MyTraining.Tag as IList<EntryObjectDTO>;
                if (myEntries == null)
                {
                    myEntries = ServiceManager.GetMyTrainingEntries(entry.MyTraining);
                }
                
            }

            if (myEntries == null)
            {
                return;
            }
            entries = myEntries.Cast<A6WEntryDTO>().ToDictionary(x => x.DayNumber);

            foreach (A6WDay day in A6WManager.Days)
            {
                ListViewItem item = new ListViewItem(new string[] { day.DayNumber.ToString(), day.SetNumber.ToString(), day.RepetitionNumber.ToString() });
                if (entries.ContainsKey(day.DayNumber))
                {
                    item.ImageKey = entries[day.DayNumber].Completed ? "Completed" : "PartialCompleted";
                }

                item.Tag = day;
                lvA6W.Items.Add(item);
            }
        }


        private A6WDay? GetSelectedA6wDay()
        {
            if(InvokeRequired)
            {
                return (A6WDay?)Invoke(new Func<A6WDay?>(GetSelectedA6wDay));
            }
            else
            {
                foreach (ListViewItem item in lvA6W.Items)
                {
                    if (item.BackColor == selectedDayColor)
                    {
                        return (A6WDay)item.Tag;
                    }
                }
                return null;
            }
        }

        public bool ReadOnly
        {
            get { return isReadOnly; }
            set 
            { 
                isReadOnly = value;
                updateReadOnly(isReadOnly);
            }
        }

        
        public bool IsTrainingInProgress
        {
            get { return IsTrainingInProgress1; }
            set 
            { 
                IsTrainingInProgress1 = value;
                updateReadOnlyForTrainingProgress(value);
            }
        }

        private void updateReadOnlyForTrainingProgress(bool readOnly)
        {
            usrMyTrainingStatus1.ReadOnly = readOnly || entries.Count == 0;
            panel1.Enabled = !readOnly;
            txtComment.Properties.ReadOnly = readOnly;
            usrReportStatus1.ReadOnly = readOnly;
        }

        public void Fill(EntryObjectDTO entry)
        {
            this.entry = (A6WEntryDTO)entry;
            usrReportStatus1.Fill(entry);
            if (this.entry.MyTraining.Id == Constants.UnsavedObjectId)
            {
                FMMessageBox.ShowInfo(A6WEntryStrings.StartNewTrainingText);
            }

            if (entry.Id != Constants.UnsavedObjectId)
            {
                rbCompleted.Checked = this.entry.Completed;
                rbPartialCompleted.Checked = !this.entry.Completed;
            }
            ReadOnly =ReadOnly || this.entry.MyTraining.TrainingEnd != TrainingEnd.NotEnded;
            txtComment.Text = entry.Comment;
            fillDaysCombo();
            usrMyTrainingStatus1.Fill(this.entry.MyTraining);
            usrA6WPartialCompleted1.Fill(this.entry);
            lvA6W.Items[this.entry.DayNumber - 1].BackColor = selectedDayColor;
        }

        public void UpdateEntryObject()
        {
            A6WDay? day = GetSelectedA6wDay();
            if (day == null)
            {
                throw new ArgumentNullException("You have to selected day number first");
            }
            entry.DayNumber = day.Value.DayNumber;
            entry.Completed = rbCompleted.Checked;
            entry.Comment = txtComment.Text;
            usrReportStatus1.Save(entry);
            if(!entry.Completed)
            {
                usrA6WPartialCompleted1.Save(entry);
            }
            else
            {
                entry.Set1 = entry.Set2 = entry.Set3 = null;
            }
            if (entry.DayNumber == A6WManager.LastDay.DayNumber && entry.MyTraining.TrainingEnd == TrainingEnd.NotEnded)
            {
                entry.MyTraining.Complete();
                FMMessageBox.ShowInfo(A6WEntryStrings.TrainingCompletedText);
            }
        }

        public void AfterSave(bool isWindowClosing)
        {
            if (!isWindowClosing)
            {//refresh the control only when user pressed Apply button (window is not closed)
                //Fill(this.entry);
            }
        }

        private void rbPartialCompleted_CheckedChanged(object sender, EventArgs e)
        {
            usrA6WPartialCompleted1.Enabled = rbPartialCompleted.Checked;
        }

        public bool ValidateControl()
        {
            if (entry.MyTraining.TrainingEnd == TrainingEnd.NotEnded && !rbPartialCompleted.Checked && !rbCompleted.Checked)
            {
                FMMessageBox.ShowError(A6WEntryStrings.ErrorA6WDayResultMissing);
                return false;
            }
            if (entry.MyTraining.TrainingEnd == TrainingEnd.NotEnded && rbPartialCompleted.Checked && !usrA6WPartialCompleted1.Validate(entry))
            {
                return false;
            }
            return true;
        }

        private void lblWWW_Click(object sender, EventArgs e)
        {
            
            try
            {
                System.Diagnostics.Process.Start("http://mybodyarchitect.tk/index.php/a6w-training");
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorOpenWebBrowser, ErrorWindow.MessageBox);
            }
        }

        
    }
}
