using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.StrengthTraining.Reports;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Portable;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Reports;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using BodyArchitect.Shared;
using Visiblox.Charts;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls
{
    /// <summary>
    /// Interaction logic for usrStrengthTrainingDetails.xaml
    /// </summary>
    public partial class usrStrengthTrainingDetails
    {
        public usrStrengthTrainingDetails()
        {
            InitializeComponent();
        }
        
        public override void UpdateEntryObject(EntryObjectDTO entryDto)
        {
            StrengthTrainingEntryDTO entry = (StrengthTrainingEntryDTO)entryDto;
            usrReportStatus1.Save(entry);
            entry.Comment = txtComment.Text;
            entry.Mood = ctrlMood.SelectedMood;
            entry.Intensity = usrIntensity1.Intensity;
            entry.StartTime = DateHelper.GetCorrectDateTime(tpStart.Value.Value);
            entry.EndTime = DateHelper.GetCorrectDateTime(tpEnd.Value.Value);
            if (cmbMyPlaces.SelectedValue != null)
            {
                entry.MyPlace = (MyPlaceLightDTO)cmbMyPlaces.SelectedItem;
            }
        }

        public override void UpdateReadOnly(bool readOnly)
        {
            txtComment.IsReadOnly = readOnly;
            this.usrReportStatus1.ReadOnly = readOnly;
            usrReportStatus1.SetVisible(!readOnly);
            this.tpStart.IsEnabled = !readOnly;
            this.tpEnd.IsEnabled = !readOnly;
            this.usrIntensity1.ReadOnly = readOnly;
            ctrlMood.ReadOnly = readOnly;
            usrReminder.IsEnabled = !readOnly;
        }

        public void UpdateLicence()
        {
            cmbMyPlaces.IsEnabled = !this.usrReportStatus1.ReadOnly && UserContext.IsPremium;
        }

        

        private void tpStart_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            onObjectChanged();
        }

        private void usrIntensity1_IntensityChanged(object sender, EventArgs e)
        {
            onObjectChanged();
        }

        private void txtComment_TextChanged(object sender, TextChangedEventArgs e)
        {
            onObjectChanged();
        }

        private void cmbMyPlaces_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            onObjectChanged();
        }

        public override void Fill(EntryObjectDTO entryDto)
        {
            StrengthTrainingEntryDTO entry = (StrengthTrainingEntryDTO) entryDto;
            txtComment.Text = entry.Comment;
            ctrlMood.SelectedMood = entry.Mood;
            tpStart.Value = entry.StartTime.HasValue ? entry.StartTime.Value : BAHelper.NullDateTime;
            tpEnd.Value = entry.EndTime.HasValue ? entry.EndTime.Value : BAHelper.NullDateTime;
            usrIntensity1.Intensity = entry.Intensity;
            usrReminder.Entry = entryDto;
            if (entry.MyPlace != null)
            {
                cmbMyPlaces.SelectedValue = entry.MyPlace.GlobalId;
                cmbMyPlaces.Text = entry.MyPlace.Name;
            }
            usrWorkoutPlansChooser1.Fill(entry);
            usrReportStatus1.Fill(entry);
            usrApplicationName.Fill(entry);

            if (entry.RemindBefore == null && entry.TrainingDay.TrainingDate < DateTime.Now.Date)
            {//hide reminder box for old entries
                grRemindMe.SetVisible(false);
            }
        }
    }
}
