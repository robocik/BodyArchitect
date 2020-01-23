using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using BodyArchitect.Controls.Localization;

namespace BodyArchitect.Controls.Reporting
{
    public partial class usrReportingEntryStatus : DevExpress.XtraEditors.XtraUserControl
    {
        
        public usrReportingEntryStatus()
        {
            InitializeComponent();
            cmbEntryReportStatus.SelectedIndex = 0;
            fillSuperTips();
        }

        void fillSuperTips()
        {
            ControlHelper.AddSuperTip(cmbEntryReportStatus, grEntryReportStatus.Text, SuperTips.usrReportingEntryStatus_ReportStatusCmb);
        }

        public bool UseAllEntries
        {
            get { return cmbEntryReportStatus.SelectedIndex == 1; }
        }
    }
}
