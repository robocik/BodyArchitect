using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Navigation;
using BodyArchitect.Portable;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls.Animations;
using BodyArchitect.WP7.Utils;
using Microsoft.Phone.Controls.Maps;

namespace BodyArchitect.WP7.Pages
{
    public partial class MapPage 
    {
        public MapPage()
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

        public GPSTrackerEntryDTO SelectedItem { get; set; }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var pointsBag = ApplicationState.Current.TrainingDay.GetGpsCoordinates(SelectedItem);
            if (pointsBag == null)
            {
                await downloadGpsCoordinates();
            }
            else
            {
                drawTrackOnMap(pointsBag.Points);
                fillLaps(pointsBag.Points);
            }
        }

        async Task downloadGpsCoordinates()
        {
            progressBar.ShowProgress(true, ApplicationStrings.MapPage_DownloadingGpsCoordinates);
            try
            {
                List<GPSPoint> points = await BAService.GetGPSCoordinatesAsync(SelectedItem.GlobalId);
                ApplicationState.Current.TrainingDay.SetGpsCoordinates(SelectedItem, points);
                if (
                    ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays.ContainsKey(
                        SelectedItem.TrainingDay.TrainingDate))
                {
                    ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays[
                        SelectedItem.TrainingDay.TrainingDate].SetGpsCoordinates(SelectedItem, points);
                }
                drawTrackOnMap(points);
                fillLaps(points);
            }
            catch (Exception)
            {
                BAMessageBox.ShowError(ApplicationStrings.MapPage_ErrDownloadingGpsCoordinates);
            }
            finally
            {
                progressBar.ShowProgress(false);
            }
        }

        void fillLaps(IList<GPSPoint> points)
        {
            var lapLength = ApplicationState.Current.ProfileInfo.Settings.LengthType == LengthType.Cm ? 1000 : 1609.344;
            GPSPointsProcessor processor = new GPSPointsProcessor(points, (decimal) lapLength);
            lstLaps.ItemsSource = processor.Laps;
        }

        void drawTrackOnMap(IList<GPSPoint>  points)
        {
            if (points.Count == 0)
            {
                return;
            }
            MapLayer myRouteLayer = new MapLayer();
            mapBing.Children.Add(myRouteLayer);
            // Add the route line to the new layer.
            var routeLine = createMapLine(myRouteLayer);
            foreach (var gpsPoint in points)
            {
                if (gpsPoint.IsPoint())
                {
                    var coordinate = new GeoCoordinate(gpsPoint.Latitude, gpsPoint.Longitude, gpsPoint.Altitude);

                    routeLine.Locations.Add(coordinate);
                }
                else if (routeLine.Locations!=null && routeLine.Locations.Count>0)
                {
                    routeLine = createMapLine(myRouteLayer);
                }
            }

            var correctPoints = points.Where(x => x.IsPoint());
            var bounds = new LocationRect(
                    correctPoints.Max((p) => p.Latitude),
                    correctPoints.Min((p) => p.Longitude),
                    correctPoints.Min((p) => p.Latitude),
                    correctPoints.Max((p) => p.Longitude));
            mapBing.SetView(bounds);
            mapBing.Center = routeLine.Locations.FirstOrDefault();
        }

        MapPolyline createMapLine(MapLayer layer)
        {
            Color routeColor = Colors.Blue;
            SolidColorBrush routeBrush = new SolidColorBrush(routeColor);

            var routeLine = new MapPolyline();
            routeLine.Locations = new LocationCollection();
            routeLine.Stroke = routeBrush;
            routeLine.Opacity = 0.50;
            routeLine.StrokeThickness = 5.0;
            layer.Children.Add(routeLine);
            return routeLine;
        }
    }
}