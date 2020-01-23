using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BodyArchitect.Client.Module.A6W.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using Visiblox.Charts;

namespace BodyArchitect.Client.Module.A6W.Controls
{
    /// <summary>
    /// Interaction logic for usrA6WDetails.xaml
    /// </summary>
    public partial class usrA6WDetails
    {
        private usrA6W a6wControl;

        public usrA6WDetails()
        {
            InitializeComponent();
        }

        public void SetA6WControl(usrA6W a6w)
        {
            this.a6wControl = a6w;
        }

        private void usrReportStatus1_StatusesChanged(object sender, EventArgs e)
        {
            onObjectChanged();
        }

        public override void UpdateReadOnly(bool readOnly)
        {
            usrMyTrainingStatus1.ReadOnly = readOnly;
            panel1.IsEnabled = !readOnly;
            txtComment.IsReadOnly = readOnly;
            usrReportStatus1.ReadOnly = readOnly;
            usrReportStatus1.SetVisible(!readOnly);
        }

        public override bool ValidateControl(EntryObjectDTO entryDto)
        {
            A6WEntryDTO entry = (A6WEntryDTO)entryDto;
            if (entry.Status == EntryObjectStatus.Done && entry.MyTraining.TrainingEnd == TrainingEnd.NotEnded && !rbPartialCompleted.IsChecked.Value && !rbCompleted.IsChecked.Value)
            {

                BAMessageBox.ShowErrorEx(A6WEntryStrings.ErrorA6WDayResultMissing, A6WEntryStrings.EntryTypeName);
                return false;
            }
            if (entry.MyTraining.TrainingEnd == TrainingEnd.NotEnded && rbPartialCompleted.IsChecked.Value && !usrA6WPartialCompleted1.Validate(entry))
            {
                return false;
            }
            return true;
        }
        public override void UpdateEntryObject(EntryObjectDTO entryDto)
        {
            A6WDay? day = a6wControl.GetSelectedA6wDay();
            if (day == null)
            {
                return;
            }
            A6WEntryDTO entry = (A6WEntryDTO) entryDto;
            entry.Day = day.Value;
            entry.Completed = rbCompleted.IsChecked.Value;
            entry.Comment = txtComment.Text;
            usrReportStatus1.Save(entry);
            if (entry.Status == EntryObjectStatus.Planned)
            {
                entry.Completed = false;
            }
            else
            {
                if (!entry.Completed)
                {
                    usrA6WPartialCompleted1.Save(entry);
                }
                else
                {
                    entry.Set1 = entry.Set2 = entry.Set3 = null;
                }
            }

            if (entry.Day.DayNumber == A6WManager.LastDay.DayNumber && entry.MyTraining.TrainingEnd == TrainingEnd.NotEnded)
            {
                entry.MyTraining.Complete();
                BAMessageBox.ShowInfo(EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.A6W:A6WEntryStrings:TrainingCompletedText"));
            }
        }


        public override void Fill(EntryObjectDTO entryDto)
        {
            A6WEntryDTO entry = (A6WEntryDTO)entryDto;
            usrApplicationName.Fill(entry);
            usrReportStatus1.Fill(entry);

            usrA6WPartialCompleted1.Fill(entry);
            if (entry.Status == EntryObjectStatus.Done)
            {
                rbPartialCompleted.IsChecked = !entry.Completed;
                rbCompleted.IsChecked = entry.Completed;
            }
            if (entry.RemindBefore == null && entry.TrainingDay.TrainingDate < DateTime.Now.Date)
            {//hide reminder box for old entries
                grRemindMe.SetVisible(false);
            }
            usrReminder.Entry = entry;
            txtComment.Text = entry.Comment;

            usrMyTrainingStatus1.Fill(a6wControl.MyTraining);
        }


        private void rbCompleted_Checked(object sender, RoutedEventArgs e)
        {
            usrA6WPartialCompleted1.IsEnabled = rbPartialCompleted.IsChecked.Value;

            a6wControl.A6WEntry.Completed = rbCompleted.IsChecked.Value;
            if (!a6wControl.A6WEntry.Completed)
            {
                usrA6WPartialCompleted1.Save(a6wControl.A6WEntry);
            }
            else
            {
                a6wControl.A6WEntry.Set1 = a6wControl.A6WEntry.Set2 = a6wControl.A6WEntry.Set3 = null;
            }
            usrReportStatus1.SelectedStatus = EntryObjectStatus.Done;
            onObjectChanged();
        }
    }
}
