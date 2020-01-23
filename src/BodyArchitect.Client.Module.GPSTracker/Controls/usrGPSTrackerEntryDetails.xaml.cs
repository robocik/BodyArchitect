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
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.GPSTracker.Controls
{
    /// <summary>
    /// Interaction logic for usrGPSTrackerEntryDetails.xaml
    /// </summary>
    public partial class usrGPSTrackerEntryDetails
    {
        public usrGPSTrackerEntryDetails()
        {
            InitializeComponent();
        }

        private void usrReportStatus1_StatusesChanged(object sender, EventArgs e)
        {
            onObjectChanged();
        }

        private void txtComment_TextChanged(object sender, TextChangedEventArgs e)
        {
            onObjectChanged();
        }

        public override void Fill(EntryObjectDTO entry)
        {
            ctrlRemindMe.Entry = entry;
            usrApplicationName.Fill(entry);
            txtComment.Text = entry.Comment;
            usrReportStatus1.Fill(entry);
            ctrlMood.SelectedMood = ((GPSTrackerEntryDTO)entry).Mood;
            if (entry.RemindBefore == null && entry.TrainingDay.TrainingDate < DateTime.Now.Date)
            {//hide reminder box for old entries
                grRemindMe.SetVisible(false);
            }
        }

        public override void UpdateReadOnly(bool readOnly)
        {
            usrReportStatus1.ReadOnly = readOnly;
            usrReportStatus1.SetVisible(!readOnly);
            txtComment.IsReadOnly = readOnly;
            ctrlMood.ReadOnly = readOnly;
        }

        public override void UpdateEntryObject(EntryObjectDTO entry)
        {
            entry.Comment = txtComment.Text;
            usrReportStatus1.Save(entry);
            ((GPSTrackerEntryDTO) entry).Mood = ctrlMood.SelectedMood;
        }
    }
}
