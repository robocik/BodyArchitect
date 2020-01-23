using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Logger;
using BodyArchitect.Shared;
using Microsoft.Win32;

namespace BodyArchitect.Client.UI.Views
{
    //TODO: Ta kontrolka moze wisiec w pamięci ze względu na to że trzyma ustawiena raportu (kontrolkę) a ona ma event handler do statycznego raportu (nie wiem czy to jeszcze aktualne - był refactor w raportach)
    public partial class NewReportingView
    {
        ObservableCollection<ImageListItem<IBAReportBuilder>> reports = new ObservableCollection<ImageListItem<IBAReportBuilder>>();
        private IBAReport selectedReport;
        private CancellationTokenSource cancelGenerateReport;

        public NewReportingView()
        {
            InitializeComponent();
            DataContext = this;
        }

        #region Ribbon properties

        private bool cancelGenerateVisible;
        private bool generateVisible;
        private bool generateEnabled;
        bool settingsChecked;
        private bool zoomResetEnabled;
        private bool saveReportEnabled;
        private bool reportListEnabled;

        public bool ReportListEnabled
        {
            get { return reportListEnabled; }
            set
            {
                reportListEnabled = value;
                NotifyOfPropertyChange(() => ReportListEnabled);
            }
        }

        public bool SaveReportEnabled
        {
            get { return saveReportEnabled; }
            set
            {
                saveReportEnabled = value;
                NotifyOfPropertyChange(() => SaveReportEnabled);
            }
        }

        public bool CancelGenerateVisible
        {
            get { return cancelGenerateVisible; }
            set
            {
                cancelGenerateVisible = value;
                NotifyOfPropertyChange(() => CancelGenerateVisible);
            }
        }

        public bool GenerateVisible
        {
            get { return generateVisible; }
            set
            {
                generateVisible = value;
                NotifyOfPropertyChange(() => GenerateVisible);
            }
        }

        public bool GenerateEnabled
        {
            get { return generateEnabled; }
            set
            {
                generateEnabled = value;
                NotifyOfPropertyChange(() => GenerateEnabled);
            }
        }

        public bool SettingsChecked
        {
            get { return settingsChecked; }
            set
            {
                settingsChecked = value;
                NotifyOfPropertyChange(() => SettingsChecked);
            }
        }

        public bool ZoomResetEnabled
        {
            get { return zoomResetEnabled; }
            set
            {
                zoomResetEnabled = value;
                NotifyOfPropertyChange(() => ZoomResetEnabled);
            }
        }

        public IBAReport SelectedReport
        {
            get { return selectedReport; }
            set
            {
                if (selectedReport != value && value != null)
                {
                    if (selectedReport != null && selectedReport!=null)
                    {
                        selectedReport.Close();
                    }
                    selectedReport = value;

                    changeReport();
                    NotifyOfPropertyChange(() => SelectedReport);
                }
            }
        }

        public IList<ImageListItem<IBAReportBuilder>> Reports
        {
            get { return reports; }
        }


        #endregion


        public override Service.V2.Model.AccountType AccountType
        {
            get { return Service.V2.Model.AccountType.PremiumUser; }
        }

        public override Uri HeaderIcon
        {
            get { return "Reports16.png".ToResourceUrl(); }
        }


        void report_CanGenerateReportChanged(object sender, EventArgs e)
        {
            updateButtons(false);
        }

        public override void Fill()
        {
            setHeader();
            reports.Clear();
            foreach (var report in PluginsManager.Instance.BAReports)
            {
                reports.Add(new ImageListItem<IBAReportBuilder>(report.Value.ReportName, "ReportTypes\\Chart_Graph.png".ToResourceString(), report.Value));
            }
            updateButtons(false);
        }

        private void setHeader()
        {
            string title = "NewReportingView_setHeader_Reports".TranslateStrings();
            if (Customer != null)
            {
                title += Customer.FullName;
            }
            else
            {
                title += User.UserName;
            }
            Header = title; 
        }

        private void btnSelectReportItem_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.Wait;
            try
            {
                var menuItem = (MenuItem)sender;
                var report = (ImageListItem<IBAReportBuilder>)menuItem.DataContext;
                SelectedReport = report.Value.CreateReport();
            }
            finally
            {
                Cursor = Cursors.Arrow;
            }

        }

        void changeReport()
        {
            var report = SelectedReport;
            chart.Series.Clear();
            chart.ResetBarAndColumnCaches();

            //clearReportControl();
            if (report != null)
            {

                report.CanGenerateReportChanged -= new EventHandler(report_CanGenerateReportChanged);
                report.CanGenerateReportChanged += new EventHandler(report_CanGenerateReportChanged);

                pnlSettings.Children.Clear();
                pnlSettings.Children.Add(report.SettingsControl);
                var user = PageContext != null ? PageContext.User:UserContext.Current.SessionData.Profile;
                var customer = PageContext != null && PageContext.Customer!=null ?PageContext.Customer: null;
                report.Initialize(user, customer);
                splitter.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                pnlSettings.Children.Clear();
                splitter.Visibility = System.Windows.Visibility.Collapsed;
            }
            updateButtons(cancelGenerateReport!=null);
        }

        void updateButtons(bool startOperation)
        {
            CancelGenerateVisible = startOperation;
            GenerateVisible = !startOperation;
            GenerateEnabled = !startOperation && SelectedReport != null && SelectedReport != null && SelectedReport.CanGenerateReport;
            SettingsChecked = !startOperation && splitter.Visibility == System.Windows.Visibility.Visible;
            ZoomResetEnabled = !startOperation;
            SaveReportEnabled = !startOperation;
            ReportListEnabled = !startOperation;
        }

        public override void RefreshView()
        {
            Fill();
        }

        private void tbSaveReport_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "NewReportingView_tbSaveReport_Click_ExportedChart".TranslateStrings();
            dlg.Filter = "PNG File|*.png";

            if (dlg.ShowDialog() == true)
            {
                // Save the current layout transform before clearing it
                Transform transform = chart.LayoutTransform;
                chart.LayoutTransform = null;

                int width = (int)chart.ActualWidth;
                int height = (int)chart.ActualHeight;

                // Note: You only really need to do this bit if the chart isn't part of the UI (ie: a chart you create in memory)
                Size size = new Size(width, height);
                chart.Measure(size);
                chart.Arrange(new Rect(size));
                chart.UpdateLayout();

                // Create a render bitmap and push the surface to it
                RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
                rtb.Render(chart);

                // Note: Change the encoder and frame for different image types
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));

                // Save the image
                Stream stream = dlg.OpenFile();
                encoder.Save(stream);
                stream.Flush();
                stream.Close();

                // Restore previously saved layout transform
                chart.LayoutTransform = transform;
            }
        }

        private void tbZoomReset_Click(object sender, EventArgs e)
        {
            if (chart.XAxis != null)
            {
                chart.XAxis.Zoom.Scale = 1;
                chart.YAxis.Zoom.Scale = 1;
                chart.XAxis.Zoom.Offset = 0;
                chart.YAxis.Zoom.Offset =0;
            }
        }

        private void tbCancelGenerateReport_Click(object sender, EventArgs e)
        {
            if (cancelGenerateReport != null)
            {
                ParentWindow.CancelTask(cancelGenerateReport);
            }
        }

        private void tbGenerateReport_Click(object sender, EventArgs e)
        {
            if(!UIHelper.EnsurePremiumLicence())
            {
                return;
            }
            cancelGenerateReport = null;
            var selectedReport = SelectedReport;
            if (selectedReport != null)
            {
                chart.Series.Clear();
                var parentWnd = ParentWindow;
                //clearReportControl();
                cancelGenerateReport=ParentWindow.RunAsynchronousOperation(delegate(OperationContext context)
                {
                    try
                    {
                        DateTime start = DateTime.Now;
                        var data=selectedReport.RetrieveReportData(null);
                        if (context.CancellatioToken.IsCancellationRequested)
                        {
                            return;
                        }

                        UIHelper.BeginInvoke(new Action(delegate
                        {
                            selectedReport.GenerateReport(chart, data);
                            DateTime end = DateTime.Now;
                            MainWindow.Instance.SetProgressStatus(false, "Reporting_GeneratingTimeStatus".TranslateStrings(), (end - start).ToString());
                        }), Dispatcher);
                    }
                    catch (LicenceException ex)
                    {
                        parentWnd.SetException(ex);
                        UIHelper.Invoke(() => ExceptionHandler.Default.Process(ex, "ErrorLicence".TranslateStrings(), ErrorWindow.MessageBox), Dispatcher);
                    }
                    catch (Exception ex)
                    {
                        parentWnd.SetException(ex);
                        UIHelper.Invoke(() => ExceptionHandler.Default.Process(ex, "ErrorCannotGenerateReport".TranslateStrings(), ErrorWindow.MessageBox), Dispatcher);
                    }
                }, AsyncOperationStateChange);
            }
        }

        private void AsyncOperationStateChange(OperationContext state)
        {
            bool startLoginOperation = state.State == OperationState.Started;
            updateButtons(startLoginOperation);
            if (!startLoginOperation)
            {
                cancelGenerateReport = null;
            }
        }

        private void tbReportSettings_Click(object sender, EventArgs e)
        {
            splitter.SetVisible(splitter.Visibility == System.Windows.Visibility.Collapsed);
            updateButtons(cancelGenerateReport != null);
        }
    }

}
