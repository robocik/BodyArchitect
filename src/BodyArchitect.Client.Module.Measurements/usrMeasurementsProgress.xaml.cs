using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.Measurements.Reports;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Measurements
{
    /// <summary>
    /// Interaction logic for usrMeasurementsProgress.xaml
    /// </summary>
    public partial class usrMeasurementsProgress
    {
        public usrMeasurementsProgress()
        {
            InitializeComponent();
        }

        void generateProgress(SizeEntryDTO sizeEntry)
        {
            progressIndicator1.IsRunning = true;
            chart.SetVisible(!progressIndicator1.IsRunning);
            tbLicenceMsg.Text = Strings.ProgressReport_Generating;
            WeightRepetitionsReportBuilder reportBuilder = new WeightRepetitionsReportBuilder();
            var report = reportBuilder.CreateReport();

            var settings = (MeasurementsTimeReportSettings)report.SettingsControl;
            settings.usrDateRange1.DateFrom = DateTime.UtcNow.AddYears(-1);
            //create customer and user object only as a mock to put correct ProfileId and CustomerId to retrieve data method
            CustomerDTO customer = null;
            if (sizeEntry.TrainingDay.CustomerId.HasValue)
            {
                customer = new CustomerDTO();
                customer.GlobalId = sizeEntry.TrainingDay.CustomerId.Value;
            }
            report.Initialize(new UserDTO() { GlobalId = sizeEntry.TrainingDay.ProfileId }, customer);
            var properties = typeof(WymiaryDTO).GetProperties();
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.PropertyType == typeof(decimal))
                {
                    var value = (decimal)propertyInfo.GetValue(sizeEntry.Wymiary, null);
                    if (value > 0)
                    {
                        var item = settings.Items.Where(x => x.Value == propertyInfo.Name).Single();
                        item.IsChecked = true;
                    }
                }
            }

            chart.Series.Clear();
            chart.ResetBarAndColumnCaches();

            //StringBuilder stringBuilder = new StringBuilder();

            //XmlWriterSettings xmlSettings = new XmlWriterSettings();
            //xmlSettings.Indent = true;

            //using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, xmlSettings))
            //{
            //    XamlWriter.Save(chart.Template, xmlWriter);
            //}

            //Console.WriteLine(stringBuilder.ToString());

            ThreadPool.QueueUserWorkItem(new WaitCallback((h) =>
            {
                try
                {
                    Helper.EnsureThreadLocalized();
                    var data = report.RetrieveReportData(null);
                    Helper.Delay(2000);
                    UIHelper.BeginInvoke(() =>
                    {

                        report.GenerateReport(chart, data);

                        progressIndicator1.IsRunning = false;
                        chart.SetVisible(!progressIndicator1.IsRunning);
                        tbLicenceMsg.Text = "";
                    }, Dispatcher);
                }
                catch (Exception)
                {
                    UIHelper.BeginInvoke(() =>
                                             {
                                                 progressIndicator1.IsRunning = false;
                                                 chart.SetVisible(!progressIndicator1.IsRunning);
                                                 tbLicenceMsg.Text = Strings.ProgressReport_Error;
                                             }, Dispatcher);
                }

                
            }));

        }

        public override void Fill(EntryObjectDTO entry)
        {
            if(!UserContext.IsPremium)
            {
                chart.SetVisible(false);
                tbLicenceMsg.Text = Strings.ProgressReport_FreeLicenceMsg;
            }
            else
            {
                chart.SetVisible(true);
                generateProgress((SizeEntryDTO)entry);
            }
            
        }
    }
}
