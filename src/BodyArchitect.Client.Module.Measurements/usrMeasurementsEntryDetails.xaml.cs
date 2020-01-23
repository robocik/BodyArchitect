using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.Measurements.Reports;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Measurements
{
    /// <summary>
    /// Interaction logic for usrMeasurementsEntryDetails.xaml
    /// </summary>
    public partial class usrMeasurementsEntryDetails
    {
        public usrMeasurementsEntryDetails()
        {
            InitializeComponent();
        }

        public override void Fill(EntryObjectDTO entry)
        {
            usrReportStatus1.Fill(entry);
            usrApplicationName.Fill(entry);
            usrRemindMe.Entry = entry;
            txtComment.Text = entry.Comment;
            if (entry.RemindBefore == null && entry.TrainingDay.TrainingDate < DateTime.Now.Date)
            {//hide reminder box for old entries
                grRemindMe.SetVisible(false);
            }

            
        }


        

        public override void UpdateEntryObject(EntryObjectDTO sizeEntry)
        {
            usrReportStatus1.Save(sizeEntry);
            sizeEntry.Comment = txtComment.Text;
        }

        public override void UpdateReadOnly(bool readOnly)
        {
            txtComment.IsReadOnly = readOnly;
            this.usrReportStatus1.ReadOnly = readOnly;
            usrReportStatus1.SetVisible(!readOnly);
        }


        private void txtComment_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            onObjectChanged();
        }

        private void usrWymiaryEditor1_MeasurementChanged(object sender, EventArgs e)
        {
            onObjectChanged();
        }
    }
}
