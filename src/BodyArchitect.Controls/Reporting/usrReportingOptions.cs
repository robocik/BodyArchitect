using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using BodyArchitect.Common.Plugins;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Settings;

namespace BodyArchitect.Controls.Reporting
{
    public partial class usrReportingOptions : XtraUserControl, IOptionsControl
    {
        private bool restartRequired;

        public usrReportingOptions()
        {
            InitializeComponent();
            fillSuperTips();
        }

        void fillSuperTips()
        {
            ControlHelper.AddSuperTip(this.cmbTemplates, lblTemplates.Text, SuperTips.OptionReportingTemplates);
            ControlHelper.AddSuperTip(this.chkAllowZoom, chkAllowZoom.Text, SuperTips.OptionReportingAllowZoom);
            ControlHelper.AddSuperTip(this.chkShowValues, chkShowValues.Text, SuperTips.OptionReportingShowValues);
        }

        public void Fill()
        {
            chkAllowZoom.Checked = ReportingOptions.Default.AllowZoom;
            chkShowValues.Checked = ReportingOptions.Default.ShowValues;
            
            cmbTemplates.Properties.Items.Clear();
            cmbTemplates.Properties.Items.Add(new ComboBoxItem(string.Empty,ApplicationStrings.ReportingOptions_NoTemplate));
            string[] filenames=Directory.GetFiles(usrReporting.GetTemplatesFolderPath(), "*.bart");
            foreach (var filename in filenames)
            {
                cmbTemplates.Properties.Items.Add(new ComboBoxItem(Path.GetFileName(filename), Path.GetFileNameWithoutExtension(filename)));
            }

            cmbTemplates.SelectValueString(ReportingOptions.Default.TemplateFile);
            cmbWeightType.SelectedIndex = Settings1.Default.WeightType;
            cmbLengthType.SelectedIndex = Settings1.Default.LengthType;
        }

        public bool RestartRequired
        {
            get { return restartRequired; }
            private set { restartRequired = value; }
        }

        public string Title
        {
            get { return ApplicationStrings.OptionsReportingTitle; }
        }

        public Image Image
        {
            get { return Icons.Reports; }
        }

        public void Save()
        {
            string oldTemplate = ReportingOptions.Default.TemplateFile;
            //if (ReportingOptions.Default.TemplateFile==string.Empty)
            //{
            //    oldTemplate = null;
            //}
            ReportingOptions.Default.AllowZoom = chkAllowZoom.Checked;
            ReportingOptions.Default.ShowValues = chkShowValues.Checked;
            Settings1.Default.LengthType = cmbLengthType.SelectedIndex;
            Settings1.Default.WeightType = cmbWeightType.SelectedIndex;

            if (cmbTemplates.SelectedItem != null)
            {
                ReportingOptions.Default.TemplateFile = (string) ((ComboBoxItem) cmbTemplates.SelectedItem).Tag;
            }
            else
            {
                ReportingOptions.Default.TemplateFile = null;
            }
            ReportingOptions.Default.Save();
            //show restart required only when you changed template file to null
            RestartRequired = ReportingOptions.Default.TemplateFile == string.Empty && oldTemplate != ReportingOptions.Default.TemplateFile;
        }
    }
}
