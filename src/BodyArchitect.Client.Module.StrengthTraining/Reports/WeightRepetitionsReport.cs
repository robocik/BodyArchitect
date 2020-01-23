using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Module.StrengthTraining.Localization;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Service.V2.Model.Reports;
using Visiblox.Charts;
using SelectionMode = Visiblox.Charts.SelectionMode;

namespace BodyArchitect.Client.Module.StrengthTraining.Reports
{
    [Export(typeof(IBAReportBuilder))]
    public class WeightRepetitionsReportBuilder : IBAReportBuilder
    {
        public string ReportName
        {
            get { return StrengthTrainingEntryStrings.Report_WeightReps_Name; }
        }

        public IBAReport CreateReport()
        {
            return new WeightRepetitionsReport();
        }
    }

    public class WeightRepetitionsReport : IBAReport
    {
        private WeightRepetitionsReportSettings settingsControl;
        private UserDTO user;
        CustomerDTO customer;


        public IEnumerable RetrieveReportData(object arg)
        {
            ReportWeightRepetitionsParams param = null;
            param = settingsControl.GetReportParameters();
            if (user != null)
            {
                param.UserId = user.GlobalId;
            }
            if (customer != null)
            {
                param.CustomerId = customer.GlobalId;
            }
            var list = ServiceManager.ReportWeightRepetitions(param);

            return list;
        }

        public void GenerateReport(Chart chart1, IEnumerable data)
        {
            chart1.Title = ReportName;
            var exercises = settingsControl.GetSelectedExercises();
            var exSeries = new Dictionary<Guid, ColumnSeries>(exercises.Count);
            foreach (var exercise in exercises)
            {
                var lineSeries = new ColumnSeries();
                lineSeries.SelectionMode = SelectionMode.Series;
                chart1.Series.Add(lineSeries);
                exSeries.Add(exercise.GlobalId, lineSeries);
            }
            getData(exSeries, (IList<WeightReperitionReportResultItem>)data);
            var xAxis = new CategoryAxis();
            xAxis.ShowLabels = true;
            xAxis.ShowMinorTicks = false;
            xAxis.ShowMajorGridlines = false;
            chart1.XAxis = xAxis;
            chart1.XAxis.Title = StrengthTrainingEntryStrings.Report_ExerciseWeight_AxisXTitle;
            var yAxis = new LinearAxis();
            yAxis.ShowMinorTicks = false;
            yAxis.LabelFormatString = "0'";
            if (UserContext.Current.ProfileInformation.Settings.WeightType == WeightType.Pounds)
            {
                yAxis.LabelFormatString += Strings.WeightType_Pound;
            }
            else
            {
                yAxis.LabelFormatString += Strings.WeightType_Kg;
            }

            yAxis.ShowMajorGridlines = false;
            chart1.YAxis = yAxis;
            chart1.YAxis.Title = StrengthTrainingEntryStrings.Report_ExerciseWeight_AxisYTitle;
            foreach (LineSeries series in chart1.Series)
            {
                series.LegendItemTemplate = (ControlTemplate)Application.Current.Resources["CustomLegendItemTemplate"];
            }
        }

        void getData(Dictionary<Guid, ColumnSeries> exSeries, IList<WeightReperitionReportResultItem> items)
        {
            foreach (var item in items)
            {
                try
                {
                    if (exSeries[item.Exercise.GlobalId].DataSeries == null)
                    {
                        exSeries[item.Exercise.GlobalId].DataSeries = new DataSeries<int, double>(item.Exercise.Name);
                    }
                    var dataSeries = (DataSeries<int, double>)exSeries[item.Exercise.GlobalId].DataSeries;
                    var data = new DataPoint<int, double>((int)item.Repetitions, (double)item.Weight.ToDisplayWeight());
                    dataSeries.Add(data);
                }//try catch is because strange bug with conversion in case when we change the report to this
                catch (Exception)
                {

                }
                
            }

        }

        public void Initialize(UserDTO user, CustomerDTO customer)
        {
            this.user = user;
            this.customer = customer;
            settingsControl.Initialize(user, customer);
        }

        public string ReportName
        {
            get { return StrengthTrainingEntryStrings.Report_WeightReps_Name; }
        }

        public Control SettingsControl
        {
            get
            {
                settingsControl = new WeightRepetitionsReportSettings();
                settingsControl.CanGenerateReportChanged += new EventHandler(settingsControl_CanGenerateReportChanged);
                return settingsControl;
            }
        }

        void settingsControl_CanGenerateReportChanged(object sender, EventArgs e)
        {
            if (CanGenerateReportChanged != null)
            {
                CanGenerateReportChanged(sender, e);
            }
        }

        public bool CanGenerateReport
        {
            get { return settingsControl.CanGenerateReport; }
        }

        public event EventHandler CanGenerateReportChanged;

        public override string ToString()
        {
            return ReportName;
        }
        public void Close()
        {
            if (settingsControl != null)
            {
                settingsControl.CanGenerateReportChanged -= new EventHandler(settingsControl_CanGenerateReportChanged);
            }
        }
    }
}
