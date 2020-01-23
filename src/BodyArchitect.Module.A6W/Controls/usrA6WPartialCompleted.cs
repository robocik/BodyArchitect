using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Service.Model;
using DevExpress.XtraEditors;
using BodyArchitect.Module.A6W.Localization;
using BodyArchitect.Controls;
using ControlHelper = BodyArchitect.Controls.ControlHelper;

namespace BodyArchitect.Module.A6W.Controls
{
    public partial class usrA6WPartialCompleted : DevExpress.XtraEditors.XtraUserControl
    {
        public usrA6WPartialCompleted()
        {
            InitializeComponent();
            fillSuperTips();
        }

        void fillSuperTips()
        {
            ControlHelper.AddSuperTip(this.txtSet1, lblSet1.Text, A6WEntryStrings.A6W_PartialCompleteSet);
            ControlHelper.AddSuperTip(this.txtSet2, lblSet2.Text, A6WEntryStrings.A6W_PartialCompleteSet);
            ControlHelper.AddSuperTip(this.txtSet3, lblSet3.Text, A6WEntryStrings.A6W_PartialCompleteSet);
        }

        void showSetControls(int dayNumber)
        {
            A6WDay day = A6WManager.Days[dayNumber - 1];
            if(day.SetNumber<2)
            {
                lblSet2.Visible = false;
                txtSet2.Visible = false;
            }
            if (day.SetNumber < 3)
            {
                lblSet3.Visible = false;
                txtSet3.Visible = false;
            }
        }
        public void Fill(A6WEntryDTO entry)
        {
            if (entry.Set1.HasValue)
            {
                txtSet1.Value = entry.Set1.Value;
            }
            if (entry.Set2.HasValue)
            {
                txtSet2.Value = entry.Set2.Value;
            }
            if (entry.Set3.HasValue)
            {
                txtSet3.Value = entry.Set3.Value;
            }
            showSetControls(entry.DayNumber);
        }

        public void Save(A6WEntryDTO entry)
        {
            if(txtSet1.Visible && txtSet1.Value>0)
            {
                entry.Set1 = (int)txtSet1.Value;
            }
            else
            {
                entry.Set1 = null;
            }
            if (txtSet2.Visible && txtSet2.Value > 0)
            {
                entry.Set2 = (int)txtSet2.Value;
            }
            else
            {
                entry.Set2 = null;
            }
            if (txtSet3.Visible && txtSet3.Value > 0)
            {
                entry.Set3 = (int)txtSet3.Value;
            }
            else
            {
                entry.Set3 = null;
            }
        }

        public bool Validate(A6WEntryDTO entry)
        {
            A6WDay day = A6WManager.Days[entry.DayNumber - 1];
            if(day.RepetitionNumber<txtSet1.Value)
            {
                FMMessageBox.ShowError(A6WEntryStrings.ErrorSetTooManyRepetitions, 1, entry.DayNumber, day.RepetitionNumber);
                return false;
            }
            if (day.RepetitionNumber < txtSet2.Value)
            {
                FMMessageBox.ShowError(A6WEntryStrings.ErrorSetTooManyRepetitions, 2, entry.DayNumber, day.RepetitionNumber);
                return false;
            }
            if (day.RepetitionNumber < txtSet3.Value)
            {
                FMMessageBox.ShowError(A6WEntryStrings.ErrorSetTooManyRepetitions, 3, entry.DayNumber, day.RepetitionNumber);
                return false;
            }
            return true;
        }
    }
}
