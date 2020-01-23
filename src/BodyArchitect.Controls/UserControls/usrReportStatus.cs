using System;
using BodyArchitect.Service.Model;
using DevExpress.XtraEditors;
using BodyArchitect.Controls.Localization;

namespace BodyArchitect.Controls.UserControls
{
    public partial class usrReportStatus : DevExpress.XtraEditors.XtraUserControl
    {
        private bool readOnly;

        public usrReportStatus()
        {
            InitializeComponent();
            fillSuperTips();

            var enums = Enum.GetValues(typeof(ReportStatus));
            foreach (var status in enums)
            {
                cmbReportStatus.Properties.Items.Add(new ComboBoxItem(status,EnumLocalizer.Default.Translate((ReportStatus)status)));
            }
            
        }

        public bool ReadOnly
        {
            get {
                return readOnly;
            }
            set {
                readOnly = value;
                cmbReportStatus.Properties.ReadOnly = readOnly;
            }
        }

        void fillSuperTips()
        {
            ControlHelper.AddSuperTip(cmbReportStatus, grReportStatus.Text, SuperTips.usrReportStatus_ReportStatusCmb);
        }

        public void Fill(EntryObjectDTO entryObject)
        {
            cmbReportStatus.SelectValue(entryObject.ReportStatus);
        }

        public void Save(EntryObjectDTO entryObject)
        {
            entryObject.ReportStatus = (ReportStatus)((ComboBoxItem)cmbReportStatus.SelectedItem).Tag;
        }
    }
}
