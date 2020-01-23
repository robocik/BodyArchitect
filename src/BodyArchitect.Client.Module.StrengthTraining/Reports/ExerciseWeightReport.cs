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
using BodyArchitect.Settings.Model;
using Visiblox.Charts;

namespace BodyArchitect.Client.Module.StrengthTraining.Reports
{
    [Export(typeof(IBAReportBuilder))]
    public class ExerciseWeightReportBuilder : IBAReportBuilder
    {
        public string ReportName
        {
            get { return StrengthTrainingEntryStrings.Report_ExerciseWeight_Name; }
        }

        public IBAReport CreateReport()
        {
            return new ExerciseWeightReport();
        }
    }
    
    public class ExerciseWeightReport : IBAReport
    {
        private UserDTO user;
        private CustomerDTO customer;
        private ExerciseWeightReportSettings settingsControl;

        public IEnumerable RetrieveReportData(object param=null)
        {
            ReportExerciseWeightParams arg = null;
            if (param == null)
            {
                arg = settingsControl.GetReportParameters();
                if (user != null)
                {
                    arg.UserId = user.GlobalId;
                }
                if (customer != null)
                {
                    arg.CustomerId = customer.GlobalId;
                }
            }
            else
            {
                arg = (ReportExerciseWeightParams) param;
            }

            var list = ServiceManager.ReportExerciseWeight(arg);
            return list;
        }

        public void GenerateReport(Chart chart1, IEnumerable data)
        {
            chart1.Title = ReportName;
            var exercises = settingsControl.GetSelectedExercises();
            Dictionary<Guid, LineSeries> exSeries = new Dictionary<Guid, LineSeries>(exercises.Count);
            foreach (var exercise in exercises)
            {
                if (!exSeries.ContainsKey(exercise.GlobalId))
                {
                    LineSeries lineSeries = new LineSeries();
                    lineSeries.LineStrokeThickness = 1.5;
                    chart1.Series.Add(lineSeries);
                    exSeries.Add(exercise.GlobalId, lineSeries);
                }
            }
            getData(exSeries, (IList<WeightExerciseReportResultItem>)data);
            var xAxis = new DateTimeAxis();
            xAxis.ShowMinorTicks = false;
            xAxis.ShowMajorGridlines = false;
            chart1.XAxis = xAxis;
            chart1.XAxis.Title = StrengthTrainingEntryStrings.Report_ExerciseWeight_AxisXTitle;
            var yAxis = new LinearAxis();
            yAxis.ShowMinorTicks = false;
            yAxis.ShowMajorGridlines = false;
            yAxis.LabelFormatString = "0'";
            if (UserContext.Current.ProfileInformation.Settings.WeightType == WeightType.Pounds)
            {
                yAxis.LabelFormatString += Strings.WeightType_Pound;
            }
            else
            {
                yAxis.LabelFormatString += Strings.WeightType_Kg;
            }
            chart1.YAxis = yAxis;
            chart1.YAxis.Title = StrengthTrainingEntryStrings.Report_ExerciseWeight_AxisYTitle;

            foreach (LineSeries series in chart1.Series)
            {
                series.LegendItemTemplate = (ControlTemplate)Application.Current.Resources["CustomLegendItemTemplate"];
            }
        }

        void getData(Dictionary<Guid, LineSeries> exSeries, IList<WeightExerciseReportResultItem> items)
        {
            foreach (var item in items)
            {
                if (exSeries[item.Exercise.GlobalId].DataSeries == null)
                {
                    exSeries[item.Exercise.GlobalId].DataSeries = new DataSeries<DateTime, decimal>(item.Exercise.Name);
                }
                var dataSeries = (DataSeries<DateTime, decimal>)exSeries[item.Exercise.GlobalId].DataSeries;
                var data = new DataPoint<DateTime, decimal>(item.DateTime, item.Weight.ToDisplayWeight());
                dataSeries.Add(data);
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
            get { return StrengthTrainingEntryStrings.Report_ExerciseWeight_Name; }
        }

        public Control SettingsControl
        {
            get
            {
                settingsControl = new ExerciseWeightReportSettings();
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
