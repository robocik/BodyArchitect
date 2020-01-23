using System;
using System.Collections.Generic;
using BodyArchitect.Client.Resources.Localization;

namespace BodyArchitect.Client.UI.UserControls
{
    /// <summary>
    /// Interaction logic for usrReportingEntryStatus.xaml
    /// </summary>
    public partial class usrReportingEntryStatus
    {
        public usrReportingEntryStatus()
        {
            InitializeComponent();
            List<string> items = new List<string>();
            items.Add(Strings.usrReportingEntryStatus_OnlyForReportingEntries);
            items.Add(Strings.usrReportingEntryStatus_AllEntries);
            cmbEntryReportStatus.ItemsSource = items;
            cmbEntryReportStatus.SelectedIndex = 0;
        }

        public bool UseAllEntries
        {
            get { return cmbEntryReportStatus.SelectedIndex == 1; }
        }
    }
}
