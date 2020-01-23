using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.GPSTracker.Resources;
using BodyArchitect.Client.UI.Controls;
using Visiblox.Charts;

namespace BodyArchitect.Client.Module.GPSTracker.Controls
{
    class GpsTrackerChart:BAChart
    {
        public void Fill(bool showByTime, IList<GPSPointViewModel> points)
        {
            Series.Clear();
            ResetBarAndColumnCaches();

            if (points == null || points.Count == 0)
            {
                return;
            }

            var yAxis = new LinearAxis();
            yAxis.ShowMinorTicks = false;
            yAxis.ShowMajorGridlines = false;
            yAxis.LabelFormatString = "0'";
            YAxis = yAxis;
            YAxis.Title = UIHelper.SpeedType;

            var ySecondaryAxis = new LinearAxis();
            ySecondaryAxis.ShowMinorTicks = false;
            ySecondaryAxis.ShowMajorGridlines = false;
            ySecondaryAxis.LabelFormatString = "0'";
            SecondaryYAxis = ySecondaryAxis;
            SecondaryYAxis.Title = UIHelper.AltitudeType;

            SplineSeries speedSeries = new SplineSeries();
            speedSeries.IsVisibleChanged += (s, e) =>
            {
                (YAxis as LinearAxis).Visibility = speedSeries.Visibility;
            };
            speedSeries.LineStrokeThickness = 1.5;
            Series.Add(speedSeries);
            

            LineSeries altitudeSeries = new LineSeries();
            altitudeSeries.IsVisibleChanged += (s, e) =>
            {
                (SecondaryYAxis as LinearAxis).Visibility = altitudeSeries.Visibility;
            };
            altitudeSeries.LineStrokeThickness = 1.5;

            Series.Add(altitudeSeries);

            //altitudeSeries.AreaFill=new SolidColorBrush(Colors.DarkGray){Opacity = 0.5};
            altitudeSeries.ShowArea = true;
            altitudeSeries.YAxis = SecondaryYAxis;

            if (showByTime)
            {
                fillChart(points, speedSeries, altitudeSeries);
            }
            else
            {
                fillChartByDistance(points, speedSeries, altitudeSeries);
            }

            foreach (LineSeries series in Series)
            {
                series.LegendItemTemplate = (ControlTemplate)Application.Current.Resources["CustomLegendItemTemplate"];
            }
            XAxis.AdoptZoomAsRange();
        }
        private void fillChart(IList<GPSPointViewModel> points, SplineSeries speedSeries, LineSeries altitudeSeries)
        {

            var xAxis = new DateTimeAxis();
            xAxis.ShowMinorTicks = false;
            xAxis.ShowMajorGridlines = false;
            XAxis = xAxis;
            xAxis.LabelFormatString = "HH:mm:ss";
            XAxis.Title = GPSStrings.GpsTrackerChart_XAxis_Time;

            var speedData = new DataSeries<DateTime, decimal>(GPSStrings.GpsTrackerChart_YAxis_Speed);
            speedSeries.DataSeries = speedData;
            var altitudeData = new DataSeries<DateTime, decimal>(GPSStrings.GpsTrackerChart_YAxis_Altitude);
            altitudeSeries.DataSeries = altitudeData;

            var date = new DateTime(2000, 1, 1,0,0,0,DateTimeKind.Local);
            for (int index = 0; index < points.Count; index++)
            {
                var gpsPoint1 = points[index];
                if (gpsPoint1.Point.IsPoint())
                {
                    var time = TimeSpan.FromSeconds(gpsPoint1.Point.Duration);
                    var timeDateTime = date + time;
                    var data = new DataPoint<DateTime, decimal>(timeDateTime,
                                                                ((decimal) gpsPoint1.Point.Altitude).ToDisplayAltitude());
                    data.Tag = gpsPoint1;
                    altitudeData.Add(data);

                    var speed = new DataPoint<DateTime, decimal>(timeDateTime,
                                                                 ((decimal) gpsPoint1.Point.Speed).ToDisplaySpeed());
                    speed.Tag = gpsPoint1;
                    speedData.Add(speed);
                }
            }
        }

        private void fillChartByDistance(IList<GPSPointViewModel> points, LineSeries speedSeries, LineSeries altitudeSeries)
        {

            var xAxis = new LinearAxis();
            xAxis.ShowMinorTicks = false;
            xAxis.ShowMajorGridlines = false;
            XAxis = xAxis;
            XAxis.Title = GPSStrings.GpsTrackerChart_XAxis_Distance;

            var speedData = new DataSeries<decimal, decimal>(GPSStrings.GpsTrackerChart_YAxis_Speed);
            speedSeries.DataSeries = speedData;
            var altitudeData = new DataSeries<decimal, decimal>(GPSStrings.GpsTrackerChart_YAxis_Altitude);
            altitudeSeries.DataSeries = altitudeData;

            for (int index = 0; index < points.Count; index++)
            {
                var gpsPoint1 = points[index];
                if (gpsPoint1.Point.IsPoint())
                {
                    var displayDistance = gpsPoint1.Distance.ToDisplayDistance();
                    var data = new DataPoint<decimal, decimal>(displayDistance,
                                                               ((decimal) gpsPoint1.Point.Altitude).ToDisplayAltitude());
                    data.Tag = gpsPoint1;
                    altitudeData.Add(data);
                    var speed = new DataPoint<decimal, decimal>(displayDistance,
                                                                ((decimal) gpsPoint1.Point.Speed).ToDisplaySpeed());
                    speed.Tag = gpsPoint1;
                    speedData.Add(speed);
                }
            }


        }

        public void FillWithCorrection(bool showReportByTime, IList<GPSPointViewModel> gpsPoints)
        {
               Fill(showReportByTime, gpsPoints);
        }
    }
}
