using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using BodyArchitect.WCF;
using BodyArchitect.Common;
using BodyArchitect.Common.Plugins;
using BodyArchitect.Common.Reporting;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Controls.UserControls;
using BodyArchitect.Logger;
using BodyArchitect.Service.Model;
using BodyArchitect.Settings;

namespace BodyArchitect.Controls.Reporting
{
    public partial class usrReporting : usrBaseControl, IMainTabControl
    {
        public const string ReportingTemplatesFolder = "ReportingTemplates";
        public const string DefaultChartAreaName = "Default";
        private UserDTO user;

        public usrReporting()
        {
            InitializeComponent();
            chart1.AxisViewChanged += new EventHandler<ViewEventArgs>(chart1_AxisViewChanged);

            chart1.Serializer.IsResetWhenLoading = false;
            chart1.Serializer.IsTemplateMode = true;

            
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public UserDTO User
        {
            get
            {
                if (user != null)
                {
                    return user;
                }
                return UserContext.CurrentProfile;
            }
            set { user = value; }
        }

        private void prepareChartArea()
        {
            var chartArea = chart1.ChartAreas[DefaultChartAreaName];
            chartArea.CursorX.IsUserEnabled = true;
            chartArea.CursorX.IsUserSelectionEnabled = true;
            chartArea.AxisX.ScrollBar.IsPositionedInside = true;
            chartArea.CursorY.IsUserEnabled = true;
            chartArea.CursorY.IsUserSelectionEnabled = true;
            chartArea.AxisY.ScrollBar.IsPositionedInside = true;

            chartArea.AxisX.ScaleView.Zoomable = ReportingOptions.Default.AllowZoom;
            chartArea.AxisY.ScaleView.Zoomable = ReportingOptions.Default.AllowZoom;
        }

        void chart1_AxisViewChanged(object sender, ViewEventArgs e)
        {
            updateButtons(cancelGenerateReport != null);
        }

        public static string GetTemplatesFolderPath()
        {
            return Path.Combine(Application.StartupPath, ReportingTemplatesFolder);
        }
        private void tbReportSettings_Click(object sender, EventArgs e)
        {
            splitContainer1.Collapsed = !splitContainer1.Collapsed;
            updateButtons(cancelGenerateReport != null);
        }

        protected override void LoginStatusChanged(LoginStatus newStatus)
        {
            updateButtons(false);
            clearReportControl();
        }

        public void Fill()
        {
            if (UserContext.LoginStatus != LoginStatus.Logged)
            {
                return;
            }
            try
            {
                string path = Path.Combine(Application.StartupPath, string.Format(@"ReportingTemplates\{0}", ReportingOptions.Default.TemplateFile));
                if (!File.Exists(path))
                {
                    path = Path.Combine(Application.StartupPath, @"ReportingTemplates\SkyBlue.bart");
                }
                chart1.LoadTemplate(path);
                prepareChartArea();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex);
            }

            tcmbReportList.Items.Clear();
            tcmbReportList.Items.Add(ApplicationStrings.SelectReportText);
            foreach (var report in PluginsManager.Instance.Reports)
            {
                tcmbReportList.Items.Add(report);
            }
            tcmbReportList.SelectedIndex = 0;
            updateButtons(cancelGenerateReport != null);
        }

        public void RefreshView()
        {
            Fill();
        }


        private void tcmbReportList_SelectedIndexChanged(object sender, EventArgs e)
        {
            IReport report = tcmbReportList.SelectedItem as IReport;
            clearReportControl();
            if(report!=null)
            {
                
                report.CanGenerateReportChanged -= new EventHandler(report_CanGenerateReportChanged);
                report.CanGenerateReportChanged += new EventHandler(report_CanGenerateReportChanged);
                report.SettingsControl.Dock = DockStyle.Fill;
                splitContainer1.Panel2.Controls.Clear();
                splitContainer1.Panel2.Controls.Add(report.SettingsControl);
                report.Initialize();
                splitContainer1.Collapsed = false;
            }
            else
            {
                splitContainer1.Panel2.Controls.Clear();
                splitContainer1.Collapsed = true;
            }
            updateButtons(cancelGenerateReport != null);
        }

        private void clearReportControl()
        {
            chart1.ChartAreas.Clear();
            chart1.ChartAreas.Add(usrReporting.DefaultChartAreaName);
            prepareChartArea();
            resetZoom();
            chart1.Series.Clear();
            chart1.Titles.Clear();
        }

        void report_CanGenerateReportChanged(object sender, EventArgs e)
        {
            updateButtons(cancelGenerateReport != null);
        }

        private void tbSaveReport_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = ApplicationStrings.ReportSavingFilter;
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                ChartImageFormat format = getChartImageFormat(dlg.FileName);
                chart1.SaveImage(dlg.OpenFile(), format);
                dlg.Dispose();
            }
        }

        private ChartImageFormat getChartImageFormat(string filename)
        {
            ChartImageFormat format = ChartImageFormat.Png;
            switch (Path.GetExtension(filename))
            {
                case ".png":
                    format = ChartImageFormat.Png;
                    break;
                case ".jpg":
                    format = ChartImageFormat.Jpeg;
                    break;
                case ".bmp":
                    format = ChartImageFormat.Bmp;
                    break;
            }
            return format;
        }

        public IReport SelectedReport
        {
            get
            {
                if(tcmbReportList.SelectedIndex>0)
                {
                    return tcmbReportList.SelectedItem as IReport;
                }
                return null;
            }
        }

        private void AsyncOperationStateChange(OperationContext state)
        {
            bool startLoginOperation = state.State == OperationState.Started;
            updateButtons(startLoginOperation);
            if(!startLoginOperation)
            {
                cancelGenerateReport = null;
            }
        }

        private CancellationTokenSource cancelGenerateReport;

        private void tbGenerateReport_Click(object sender, EventArgs e)
        {
            cancelGenerateReport = null;
            var selectedReport = SelectedReport;
            if (selectedReport != null)
            {
                clearReportControl();
                cancelGenerateReport=ParentWindow.RunAsynchronousOperation(delegate(OperationContext context)
                {
                    try
                    {
                        DateTime start = DateTime.Now;
                        var criteria = selectedReport.GetWorkoutDaysCriteria();
                        criteria.UserId = User.Id;
                        PagedResultRetriever retriever = new PagedResultRetriever();
                        var res = retriever.GetAll(delegate(PartialRetrievingInfo pageInfo)
                        {
                            return ServiceManager.GetTrainingDays(criteria, pageInfo);
                        });
                        if (context.CancellatioToken.IsCancellationRequested)
                        {
                            return;
                        }
                        ParentWindow.SynchronizationContext.Send(delegate
                        {
                            selectedReport.GenerateReport(chart1, res);
                            DateTime end = DateTime.Now;
                            MainWindow.Instance.SetProgressStatus(false, ApplicationStrings.Reporting_GeneratingTimeStatus, (end - start).ToString());
                        }, null);
                    }
                    catch (Exception ex)
                    {
                        ParentWindow.TasksManager.SetException(ex);
                        ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorCannotGenerateReport,
                                                         ErrorWindow.MessageBox);
                    }
                }, AsyncOperationStateChange);
                
                
                
            }
        }

        void updateButtons(bool startOperation)
        {
            toolStrip1.Enabled = UserContext.LoginStatus == LoginStatus.Logged;
            tbCancelGenerateReport.Visible = startOperation;
            tbGenerateReport.Visible = !startOperation;
            tbGenerateReport.Enabled =!startOperation && SelectedReport != null && SelectedReport.CanGenerateReport;
            tbReportSettings.Checked =!startOperation && !splitContainer1.Collapsed;
            tbZoomReset.Enabled =!startOperation && chart1.ChartAreas[DefaultChartAreaName].AxisX.ScaleView.IsZoomed ||
                                  chart1.ChartAreas[DefaultChartAreaName].AxisY.ScaleView.IsZoomed;
            tbSaveReport.Enabled = !startOperation;
            tcmbReportList.Enabled = !startOperation;
        }

        private void tbZoomReset_Click(object sender, EventArgs e)
        {
            resetZoom();
        }

        private void resetZoom()
        {
            chart1.ChartAreas[DefaultChartAreaName].AxisX.ScaleView.ZoomReset(0);
            chart1.ChartAreas[DefaultChartAreaName].AxisY.ScaleView.ZoomReset(0);
            updateButtons(cancelGenerateReport != null);
        }

        private void splitContainer1_SplitGroupPanelCollapsed(object sender, DevExpress.XtraEditors.SplitGroupPanelCollapsedEventArgs e)
        {
            updateButtons(cancelGenerateReport!=null);
        }

        private void tbCancelGenerateReport_Click(object sender, EventArgs e)
        {
            if(cancelGenerateReport!=null)
            {
                ParentWindow.TasksManager.CancelTask(cancelGenerateReport);
            }
        }

    }
}
