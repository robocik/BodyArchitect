using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Xml;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.Controls.Animations;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Visiblox.Charts;

namespace BodyArchitect.WP7.Pages
{
    public partial class ExercisesWeightReportPage
    {
        private ReportExerciseWeightCompletedEventArgs result;

        public ExercisesWeightReportPage()
        {
            InitializeComponent();
            AnimationContext = LayoutRoot;
            Exercises = new List<ExerciseLightDTO>();
        }

        public List<ExerciseLightDTO> Exercises { get; private set; }

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

            generateReport();
        }

        void generateReport()
        {
            if (result != null)
            {
                fillReportData(result);
                return;
            }
            progressBar.Visibility = Visibility.Visible;
            chart.Visibility = Visibility.Collapsed;
            txtMessage.Text = ApplicationStrings.MeasurementsReportPage_MsgGeneratingReport;
            var m = new ServiceManager<ReportExerciseWeightCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<ReportExerciseWeightCompletedEventArgs> operationCompleted)
            {
                var data = new ReportExerciseWeightParams();
                data.UserId = ApplicationState.Current.TrainingDay.TrainingDay.ProfileId;
                data.CustomerId = ApplicationState.Current.TrainingDay.TrainingDay.CustomerId;
                data.StartDate = DateTime.UtcNow.AddYears(-1);

                if (Exercises.Count == 0)
                {
                    if (Entry.MyPlace != null)
                    {
                        data.MyPlaces.Add(Entry.MyPlace.GlobalId);
                    }

                    foreach (var strengthTrainingItemDto in Entry.Entries)
                    {
                        if (strengthTrainingItemDto.Exercise.ExerciseType == ExerciseType.Cardio)
                        {
//skip cardio exercise because they not use kg - but time
                            continue;
                        }
                        if (Exercises.Where(x=>x.GlobalId==strengthTrainingItemDto.Exercise.GlobalId).Count()==0)
                        {
                            Exercises.Add(strengthTrainingItemDto.Exercise);
                        }
                        if (!data.DoneWays.Contains(strengthTrainingItemDto.DoneWay))
                        {
                            data.DoneWays.Add(strengthTrainingItemDto.DoneWay);
                        }
                    }
                }
                foreach (var exercise in Exercises)
                {
                    if (!data.Exercises.Contains(exercise.GlobalId))
                    {
                        data.Exercises.Add(exercise.GlobalId);
                    }
                }

                client1.ReportExerciseWeightCompleted -= operationCompleted;
                client1.ReportExerciseWeightCompleted += operationCompleted;
                client1.ReportExerciseWeightAsync(ApplicationState.Current.SessionData.Token, data);


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

            if (!m.Run())
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

        private void fillReportData(ReportExerciseWeightCompletedEventArgs a)
        {
            chart.Series.Clear();
            chart.ResetBarAndColumnCaches();

            chart.Title = ApplicationStrings.ExercisesWeightReportPage_ReportName;
            Dictionary<Guid, LineSeries> exSeries = new Dictionary<Guid, LineSeries>();
            foreach (var exercise in Exercises)
            {
                if (!exSeries.ContainsKey(exercise.GlobalId))
                {
                    LineSeries lineSeries = new LineSeries();
                    chart.Series.Add(lineSeries);
                    exSeries.Add(exercise.GlobalId, lineSeries);
                    lineSeries.DataSeries = new DataSeries<DateTime, decimal>(exercise.Name);
                }
            }

            getData(exSeries, a.Result);
            if(chart.Series.Count==0 || a.Result.Count==0)
            {
                txtMessage.Text = ApplicationStrings.MeasurementsReportPage_MsgEmptyReport;
                chart.Visibility = System.Windows.Visibility.Collapsed;
                return;

            }
            var xAxis = new DateTimeAxis();
            xAxis.ShowMinorTicks = false;
            xAxis.ShowMajorGridlines = false;
            chart.XAxis = xAxis;
            chart.XAxis.Title = ApplicationStrings.ExercisesWeightReportPage_XAxis_Title;
            var yAxis = new LinearAxis();
            yAxis.ShowMinorTicks = false;
            yAxis.ShowMajorGridlines = false;
            yAxis.LabelFormatString = "0'";
            if (ApplicationState.Current.ProfileInfo.Settings.WeightType == WeightType.Pounds)
            {
                yAxis.LabelFormatString += ApplicationStrings.Pound;
            }
            else
            {
                yAxis.LabelFormatString += ApplicationStrings.Kg;
            }
            
            chart.YAxis = yAxis;
            chart.YAxis.Title = ApplicationStrings.ExercisesWeightReportPage_YAxis_Title;

            var legend = chart.FindChild<Legend>();
            legend.IsHitTestVisible = true;
        }

        void getData(Dictionary<Guid, LineSeries> exSeries, IList<WeightExerciseReportResultItem> items)
        {
            foreach (var item in items)
            {
                var dataSeries = (DataSeries<DateTime, decimal>) exSeries[item.Exercise.GlobalId].DataSeries;
                var data = new DataPoint<DateTime, decimal>(item.DateTime, item.Weight.ToDisplayWeight());
                dataSeries.Add(data);
            }
        }

        //void getData(Chart chart, IList<WeightExerciseReportResultItem> items)
        //{
        //    Dictionary<Guid, LineSeries> exSeries = new Dictionary<Guid, LineSeries>();

        //    foreach (var item in items)
        //    {
        //        if (!exSeries.ContainsKey(item.Exercise.GlobalId))
        //        {
        //            LineSeries lineSeries = new LineSeries();
        //            chart.Series.Add(lineSeries);
        //            exSeries.Add(item.Exercise.GlobalId, lineSeries);
        //        }
        //        if (exSeries[item.Exercise.GlobalId].DataSeries == null)
        //        {
        //            exSeries[item.Exercise.GlobalId].DataSeries = new DataSeries<DateTime, decimal>(item.Exercise.Name);
        //        }
        //        var dataSeries = (DataSeries<DateTime, decimal>)exSeries[item.Exercise.GlobalId].DataSeries;
        //        var data = new DataPoint<DateTime, decimal>(item.DateTime, item.Weight.ToDisplayWeight());
        //        dataSeries.Add(data);
        //    }

        //}
    }
}