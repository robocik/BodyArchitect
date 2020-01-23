using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using BodyArchitect.Client.Common;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.UI.UserControls
{
    /// <summary>
    /// Interaction logic for usrReportStatus.xaml
    /// </summary>
    public partial class usrReportStatus 
    {
        private bool readOnly;
        private ReportStatus selectedReportStatus;
        private EntryObjectStatus selectedStatus;
        private List<ListItem<ReportStatus>> _reportStatuses = new List<ListItem<ReportStatus>>();
        private List<ListItem<EntryObjectStatus>> _statuses = new List<ListItem<EntryObjectStatus>>();
        public event EventHandler StatusesChanged;

        public usrReportStatus()
        {
            InitializeComponent();
            DataContext = this;
            var enums = Enum.GetValues(typeof(ReportStatus));
            foreach (ReportStatus status in enums)
            {
                ReportStatuses.Add(new ListItem<ReportStatus>(EnumLocalizer.Default.Translate(status),status));
            }

            Statuses.Add(new ListItem<EntryObjectStatus>(EnumLocalizer.Default.Translate(EntryObjectStatus.Done), EntryObjectStatus.Done));
            Statuses.Add(new ListItem<EntryObjectStatus>(EnumLocalizer.Default.Translate(EntryObjectStatus.Planned), EntryObjectStatus.Planned));
        }

        void onStatusesChanged()
        {
            if (StatusesChanged != null)
            {
                StatusesChanged(this, EventArgs.Empty);
            }
        }

        public bool ReadOnly
        {
            get
            {
                return readOnly;
            }
            set
            {
                readOnly = value;
                cmbReportStatus.IsEnabled= !readOnly;
                cmbStatus.IsEnabled = !readOnly;
            }
        }

        public List<ListItem<EntryObjectStatus>> Statuses
        {
            get { return _statuses; }
        }

        public EntryObjectStatus SelectedStatus
        {
            get { return selectedStatus; }
            set
            {
                if (selectedStatus != value)
                {
                    selectedStatus = value;
                    NotifyOfPropertyChange(() => SelectedStatus);
                    onStatusesChanged();
                }
            }
        }

        public List<ListItem<ReportStatus>> ReportStatuses
        {
            get { return _reportStatuses; }
        }

        public ReportStatus SelectedReportStatus
        {
            get { return selectedReportStatus; }
            set
            {
                if (selectedReportStatus != value)
                {
                    selectedReportStatus = value;
                    NotifyOfPropertyChange(() => SelectedReportStatus);
                    onStatusesChanged();
                }
            }
        }


        public void Fill(EntryObjectDTO entryObject)
        {
            SelectedReportStatus = entryObject.ReportStatus;
            SelectedStatus = entryObject.Status;
        }

        public void Save(EntryObjectDTO entryObject)
        {
            entryObject.ReportStatus = SelectedReportStatus;// (ReportStatus)((ComboBoxItem)cmbReportStatus.SelectedItem).Tag;
            entryObject.Status = SelectedStatus;
        }
    }
}
