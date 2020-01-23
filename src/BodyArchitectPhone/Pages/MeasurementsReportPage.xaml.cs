using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.Controls.Animations;
using Visiblox.Charts;

namespace BodyArchitect.WP7.Pages
{
    public partial class MeasurementsReportPage
    {
        private ReportMeasurementsTimeCompletedEventArgs result;

        public MeasurementsReportPage()
        {
            InitializeComponent();
            AnimationContext = LayoutRoot;
        }

        protected override AnimatorHelperBase GetAnimation(AnimationType animationType, Uri toOrFrom)
        {
            if (animationType == AnimationType.NavigateForwardIn || animationType == AnimationType.NavigateBackwardIn)
                return new SlideUpAnimator() { RootElement = LayoutRoot };
            else
                return new SlideDownAnimator() { RootElement = LayoutRoot };
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //ApplicationBar.IsVisible = !ApplicationState.Current.IsOffline;
            generateReport();
        }

        void generateReport()
        {
            if (result != null)
            {
                fillReportData(result);
                return;
            }
            progressBar.Visibility=Visibility.Visible;
            chart.Visibility = Visibility.Collapsed;
            txtMessage.Text = ApplicationStrings.MeasurementsReportPage_MsgGeneratingReport;
            var m = new ServiceManager<ReportMeasurementsTimeCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<ReportMeasurementsTimeCompletedEventArgs> operationCompleted)
            {
                ReportMeasurementsTimeParams data = new ReportMeasurementsTimeParams();
                data.UserId = ApplicationState.Current.TrainingDay.TrainingDay.ProfileId;
                data.CustomerId = ApplicationState.Current.TrainingDay.TrainingDay.CustomerId;
                data.StartDate = DateTime.UtcNow.AddYears(-1);

                client1.ReportMeasurementsTimeCompleted -= operationCompleted;
                client1.ReportMeasurementsTimeCompleted += operationCompleted;
                client1.ReportMeasurementsTimeAsync(ApplicationState.Current.SessionData.Token, data);
                

            });

            m.OperationCompleted += (s, a) =>
            {
                progressBar.Visibility = Visibility.Collapsed;
                FaultException<BAServiceException> faultEx = a.Error as FaultException<BAServiceException>;
                if (a.Error != null)
                {
                    txtMessage.Text = ApplicationStrings.MeasurementsReportPage_ErrGeneratingReport;
                    BAMessageBox.ShowError(ApplicationStrings.MeasurementsReportPage_ErrGeneratingReport);
                }
                else
                {
                    chart.Visibility = Visibility.Visible;
                    txtMessage.Text = "";
                    fillReportData(a.Result);
                }
            };

            if(!m.Run())
            {
                progressBar.Visibility = Visibility.Collapsed;
                if (ApplicationState.Current.IsOffline)
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrOfflineMode);
                }
                else
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);
                }
            }
        }

        private void fillReportData(ReportMeasurementsTimeCompletedEventArgs a)
        {
            chart.Series.Clear();
            chart.ResetBarAndColumnCaches();

            GenerateReport(chart, a.Result);
        }

        private List<PropertyInfo> getSelectedProperties()
        {
            List<PropertyInfo> list = new List<PropertyInfo>();
            var properties = typeof(WymiaryDTO).GetProperties();
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.PropertyType == typeof(decimal))
                {
                    var value = (decimal)propertyInfo.GetValue(Entry.Wymiary, null);
                    if (value > 0)
                    {
                        list.Add(propertyInfo);
                    }
                }
            }
            return list;
        }

        public void GenerateReport(Chart chart1, IEnumerable data)
        {
            chart1.Title = ApplicationStrings.MeasurementsReportPage_Report_Name;
            var exercises = getSelectedProperties();
            var exSeries = new Dictionary<PropertyInfo, LineSeries>(exercises.Count);
            foreach (var exercise in exercises)
            {
                var series = new LineSeries();
                chart1.Series.Add(series);
                exSeries.Add(exercise, series);

                string title = getLineTitle(exercise.Name);
                series.DataSeries = new DataSeries<DateTime, decimal>(title);
            }
            var list=(IList<MeasurementsTimeReportResultItem>)data;
            if (chart.Series.Count == 0 || getData(exSeries, list))
            {
                txtMessage.Text = ApplicationStrings.MeasurementsReportPage_MsgEmptyReport;
                chart.Visibility = System.Windows.Visibility.Collapsed;
                return;
            }

            var xAxis = new DateTimeAxis();
            xAxis.ShowMinorTicks = false;
            xAxis.ShowMajorGridlines = false;
            chart1.XAxis = xAxis;
            chart1.XAxis.Title = ApplicationStrings.MeasurementsReportPage_XAxis_Title;
            var yAxis = new LinearAxis();
            yAxis.ShowMinorTicks = false;
            yAxis.ShowMajorGridlines = false;
            chart1.YAxis = yAxis;
            chart1.YAxis.Title = ApplicationStrings.MeasurementsReportPage_YAxis_Title;

            var legend = chart.FindChild<Legend>();
            legend.IsHitTestVisible = true;
        }

        private string getLineTitle(string measurementName)
        {
            var title = "";
            if (measurementName == "Weight")
            {
                
                if (ApplicationState.Current.ProfileInfo.Settings.WeightType == WeightType.Pounds)
                {
                    title = ApplicationStrings.Pound;
                }
                else
                {
                    title = ApplicationStrings.Kg;
                }
            }
            else
            {
                if (ApplicationState.Current.ProfileInfo.Settings.LengthType == LengthType.Inchs)
                {
                    title = ApplicationStrings.Inch;
                }
                else
                {
                    title = ApplicationStrings.Cm;
                }
            }
            return string.Format("{0} ({1})", getTranslatedWymiar(measurementName), title);
        }

        object getValue(PropertyInfo info, WymiaryDTO obj)
        {
            return info.GetValue(obj, null);
        }

        string getTranslatedWymiar(string name)
        {
            switch (name)
            {
                case "Weight":
                    return ApplicationStrings.MeasurementsControl_Weight;
                case "Height":
                    return ApplicationStrings.MeasurementsControl_Height;
                case "Klatka":
                    return ApplicationStrings.MeasurementsControl_Chest;
                case "Pas":
                    return ApplicationStrings.MeasurementsControl_Abs;
                case "RightBiceps":
                    return ApplicationStrings.MeasurementsControl_RightBiceps;
                case "LeftBiceps":
                    return ApplicationStrings.MeasurementsControl_LeftBiceps;
                case "RightForearm":
                    return ApplicationStrings.MeasurementsControl_RightForearm;
                case "LeftForearm":
                    return ApplicationStrings.MeasurementsControl_LeftForearm;
                case "RightUdo":
                    return ApplicationStrings.MeasurementsControl_RightLeg;
                case "LeftUdo":
                    return ApplicationStrings.MeasurementsControl_LeftLeg;
            }
            return name;
        }

        bool getData(Dictionary<PropertyInfo, LineSeries> exSeries, IList<MeasurementsTimeReportResultItem> items)
        {
            bool isEmpty = true;
            foreach (var item in items)
            {
                foreach (var series in exSeries)
                {
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
                        isEmpty = false;
                    }
                }
            }
            return isEmpty;
        }
    }
}