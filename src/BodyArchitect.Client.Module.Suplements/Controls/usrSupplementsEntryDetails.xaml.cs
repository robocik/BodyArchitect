using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.Suplements.Controls
{
    /// <summary>
    /// Interaction logic for usrSupplementsEntryDetails.xaml
    /// </summary>
    public partial class usrSupplementsEntryDetails
    {

        public usrSupplementsEntryDetails()
        {
            InitializeComponent();
        }


        public override void UpdateEntryObject(EntryObjectDTO entry)
        {
            entry.Comment = txtComment.Text;
            usrReportStatus1.Save(entry);
        }

        public override void UpdateReadOnly(bool readOnly)
        {
            usrReportStatus1.ReadOnly = readOnly;
            usrReportStatus1.SetVisible(!readOnly);
            txtComment.IsReadOnly = readOnly;
        }

        public override void Fill(EntryObjectDTO entry)
        {
            ctrlRemindMe.Entry = entry;
            usrApplicationName.Fill(entry);
            txtComment.Text = entry.Comment;
            usrReportStatus1.Fill(entry);
            if (entry.RemindBefore == null && entry.TrainingDay.TrainingDate < DateTime.Now.Date)
            {//hide reminder box for old entries
                grRemindMe.SetVisible(false);
            }
        }


        private void usrReportStatus1_StatusesChanged(object sender, EventArgs e)
        {
            onObjectChanged();
        }

        private void txtComment_TextChanged(object sender, TextChangedEventArgs e)
        {
            onObjectChanged();
        }
    }
}
