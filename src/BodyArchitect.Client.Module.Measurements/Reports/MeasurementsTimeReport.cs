using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Controls;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Service.V2.Model.Reports;
using BodyArchitect.Settings.Model;
using Visiblox.Charts;
using System.Windows;

namespace BodyArchitect.Client.Module.Measurements.Reports
{
    [Export(typeof(IBAReportBuilder))]
    public class WeightRepetitionsReportBuilder : IBAReportBuilder
    {
        public string ReportName
        {
            get { return SizeEntryStrings.Report_SizesTime_Name; }
        }

        public IBAReport CreateReport()
        {
            return new MeasurementsTimeReport();
        }
    }

    public class MeasurementsTimeReport : IBAReport
    {
        private UserDTO user;
        private CustomerDTO customer;
        private MeasurementsTimeReportSettings settingsControl;

        public IEnumerable RetrieveReportData(object arg)
        {
            ReportMeasurementsTimeParams param = settingsControl.GetReportParameters();

            if (user != null)
            {
                param.UserId = user.GlobalId;
            }
            if (customer != null)
            {
                param.CustomerId = customer.GlobalId;
            }
            var list = ServiceManager.ReportMeasurementsTime(param);
            return list;
        }

        public void GenerateReport(Chart chart1, IEnumerable data)
        {
                chart1.Title = ReportName;
                var exercises = settingsControl.GetSelectedProperties();
                var exSeries = new Dictionary<PropertyInfo, LineSeries>(exercises.Count);
                foreach (var exercise in exercises)
                {
                    var series = new LineSeries();
                    series.LineStrokeThickness = 1.5;
                    chart1.Series.Add(series);
                    exSeries.Add(exercise, series);

                }
                getData(exSeries, (IList<MeasurementsTimeReportResultItem>)data);
                
                var xAxis = new DateTimeAxis();
                xAxis.ShowMinorTicks = false;
                xAxis.ShowMajorGridlines = false;
                chart1.XAxis = xAxis;
                chart1.XAxis.Title = SizeEntryStrings.Report_SizeTime_AxisXTitle;
                var yAxis = new LinearAxis();
                yAxis.ShowMinorTicks = false;
                yAxis.ShowMajorGridlines = false;
                chart1.YAxis = yAxis;
                chart1.YAxis.Title = SizeEntryStrings.Report_SizeTime_AxisYTitle;

                foreach (LineSeries series in chart1.Series)
                {
                    series.LegendItemTemplate = (ControlTemplate)Application.Current.Resources["CustomLegendItemTemplate"];
                }
        }

        private string getLineTitle(KeyValuePair<PropertyInfo, LineSeries> series)
        {
            var title = "";
            if (series.Key.Name == "Weight")
            {
                if (UserContext.Current.ProfileInformation.Settings.WeightType == WeightType.Pounds)
                {
                    title = Strings.WeightType_Pound;
                }
                else
                {
                    title = Strings.WeightType_Kg;
                }
            }
            else
            {
                if (UserContext.Current.ProfileInformation.Settings.LengthType == LengthType.Inchs)
                {
                    title = Strings.LengthType_Inch;
                }
                else
                {
                    title = Strings.LengthType_Cm;
                }
            }
            return string.Format("{0} ({1})", settingsControl.GetTranslatedWymiar(series.Key.Name), title);
        }



        object getValue(PropertyInfo info, WymiaryDTO obj)
        {
            return info.GetValue(obj, null);
        }

        public override string ToString()
        {
            return ReportName;
        }

        void getData(Dictionary<PropertyInfo, LineSeries> exSeries, IList<MeasurementsTimeReportResultItem> items)
        {
            foreach (var item in items)
            {
                foreach (var series in exSeries)
                {

                    if (exSeries[series.Key].DataSeries == null)
                    {
                        string title = getLineTitle(series);

                        exSeries[series.Key].DataSeries = new DataSeries<DateTime, decimal>(title);
                    }
                    decimal value = decimal.Parse(getValue(series.Key, item.Wymiary).ToString());
                    if (value > 0)
                    {
                        var dataSeries = (DataSeries<DateTime, decimal>)exSeries[series.Key].DataSeries;
                        decimal converterVal = 0;
                        if (series.Key.Name == "Weight")
                        {
                            converterVal = value.ToDisplayWeight();
                        }
                        else
                        {
                            converterVal = value.ToDisplayLength();
                        }
                        var data = new DataPoint<DateTime, decimal>(item.DateTime, converterVal);
                        dataSeries.Add(data);
                        
                    }
                }
            }
        }

        public void Initialize(UserDTO user, CustomerDTO customer)
        {
            this.user = user;
            this.customer = customer;
            settingsControl.Initialize(user,customer);
        }

        public string ReportName
        {
            get { return SizeEntryStrings.Report_SizesTime_Name; }
        }

        public Control SettingsControl
        {
            get
            {
                settingsControl = new MeasurementsTimeReportSettings();
                settingsControl.CanGenerateReportChanged += new EventHandler(settingsControl_CanGenerateReportChanged);
                return settingsControl;
            }
        }

        void settingsControl_CanGenerateReportChanged(object sender, EventArgs e)
        {
            if(CanGenerateReportChanged!=null)
            {
                CanGenerateReportChanged(this, e);
            }
        }

        public bool CanGenerateReport
        {
            get { return settingsControl.CanGenerateReport; }
        }

        public event EventHandler CanGenerateReportChanged;
        public void Close()
        {
            if (settingsControl != null)
            {
                settingsControl.CanGenerateReportChanged -= new EventHandler(settingsControl_CanGenerateReportChanged);
            }
        }
    }
}
