using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Portable;
using BodyArchitect.Service.V2.Model;
using Microsoft.Maps.MapControl.WPF;
using Microsoft.Maps.MapControl.WPF.Overlays;

namespace BodyArchitect.Client.Module.GPSTracker.Controls
{
    public class BAMap : Map
    {
        private MapLayer selectedLapLayer;
        private MapLayer lapsLayer;


        public BAMap()
        {
            CredentialsProvider =
                new ApplicationIdCredentialsProvider("At_JDYOOWWeOlg8s03wdUsbgMwFg4Xh7VnYEpE2VyfGbeuP27MO83TVtvhUX-WDB");
            SizeChanged += BAMap_SizeChanged;
            ViewChangeEnd += BAMap_ViewChangeEnd;
        }

        void BAMap_ViewChangeEnd(object sender, MapEventArgs e)
        {
            updateLapsLayer();
        }

        void updateLapsLayer()
        {
            if (lapsLayer != null)
            {
                lapsLayer.Visibility = ZoomLevel > 13 ? Visibility.Visible : Visibility.Collapsed;    
            }
            
        }


        private LocationRect mapBounds;
        

        void BAMap_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            if (mapBounds != null)
            {
                SetView(mapBounds);
                mapBounds = null;
            }

            var mapScale = UIHelper.FindChild<Scale>(this).SingleOrDefault();
            if (mapScale != null)
            {
                if (UserContext.Current.ProfileInformation.Settings.LengthType == LengthType.Inchs)
                {
                    mapScale.DistanceUnit = DistanceUnit.MilesYards;
                }
                else
                {
                    mapScale.DistanceUnit = DistanceUnit.KilometersMeters;
                }
            }
        }

        public void Fill(GPSTrackerViewModel viewModel,bool centerMap=true)
        {
            Children.Clear();

            //Color routeColor = Colors.Blue;
            //SolidColorBrush routeBrush = new SolidColorBrush(routeColor);
            //var routeLine = new MapPolyline();
            //routeLine.Locations = new LocationCollection();
            //routeLine.Stroke = routeBrush;
            //routeLine.Opacity = 0.50;
            //routeLine.StrokeThickness = 5.0;

            
            //MapLayer myRouteLayer = new MapLayer();
            //Children.Add(myRouteLayer);
            
            //// Add the route line to the new layer.
            //myRouteLayer.Children.Add(routeLine);
            //foreach (var gpsPoint in points)
            //{
            //    routeLine.Locations.Add(gpsPoint.ToLocation());
            //}
            
            var myRouteLayer = new MapLayer();
            Children.Add(myRouteLayer);
            lapsLayer = new MapLayer();
            selectedLapLayer = new MapLayer();
            Children.Add(selectedLapLayer);
            Children.Add(lapsLayer);
            // Add the route line to the new layer.
            var routeLine = createMapLine(myRouteLayer);
            var firstLine = routeLine;
            for (int index = 0; index < viewModel.GPSPoints.Count; index++)
            {
                var gpsPoint = viewModel.GPSPoints[index];
                if (gpsPoint.Point.IsPoint())
                {
                    routeLine.Locations.Add(gpsPoint.ToLocation());
                }
                else if (routeLine.Locations != null && routeLine.Locations.Count > 0)
                {
                    routeLine = createMapLine(myRouteLayer);
                }
            }

            //add laps start-end points
            foreach (var lapViewModel in viewModel.Laps)
            {
                Pushpin pin = new Pushpin();
                pin.Content = lapViewModel.Nr;
                pin.Location = lapViewModel.EndPoint.ToLocation();
                pin.ToolTip = new usrLapPushpinInfo(lapViewModel);
                lapsLayer.Children.Add(pin);
            }

            var correctPoints = viewModel.GPSPoints.Where(x => x.Point.IsPoint());

            if (centerMap)
            {
                mapBounds = new LocationRect(
                    correctPoints.Max((p) => p.Point.Latitude),
                    correctPoints.Min((p) => p.Point.Longitude),
                    correctPoints.Min((p) => p.Point.Latitude),
                    correctPoints.Max((p) => p.Point.Longitude));
                //map.SetView(mapBounds);//here we got exception so moved this line to SizeChanged event
                Center = firstLine.Locations.FirstOrDefault();
            }
            updateLapsLayer();

            //add finish image (flag at the end of track)
            var startImg = new Image();
            startImg.Source ="pack://application:,,,/BodyArchitect.Client.Module.GPSTracker;component/Resources/Start16.png".ToBitmap();
            startImg.Width = 16;
            startImg.Height = 16;
            myRouteLayer.AddChild(startImg, correctPoints.Last().ToLocation());
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

        public  void PlaceDot(Location location, Color color)
        {
            Ellipse dot = new Ellipse();
            dot.Fill = new SolidColorBrush(color);
            double radius = 6.0;
            dot.Width = radius * 2;
            dot.Height = radius * 2;
            ToolTip tt = new ToolTip();
            tt.Content = "Location = " + location;
            dot.ToolTip = tt;
            Point p0 = LocationToViewportPoint(location);
            Point p1 = new Point(p0.X - radius, p0.Y - radius);
            Location loc = ViewportPointToLocation(p1);
            MapLayer.SetPosition(dot, loc);
            Children.Add(dot);
        }

        public void PlaceCurrentPointDot(GPSPoint point)
        {
            var oldPoint = Children.OfType<Ellipse>().FirstOrDefault();
            if (oldPoint != null)
            {//remove old point
                Children.Remove(oldPoint);
            }

            PlaceDot(new Location(point.Latitude, point.Longitude, point.Altitude), Colors.Orange);
        }

        public void SelectLap(ICollection<GPSPointViewModel> lapPoints)
        {
            selectedLapLayer.Children.Clear();
            if (lapPoints.Count>0)
            {
                var routeLine = createLapLine();

                foreach (var gpsPoint in lapPoints)
                {
                    if (gpsPoint.Point.IsPoint())
                    {
                        routeLine.Locations.Add(gpsPoint.ToLocation());
                    }
                    else if (routeLine.Locations != null && routeLine.Locations.Count > 0)
                    {
                        routeLine = createLapLine();
                    }
                }

            }
        }

        private MapPolyline createLapLine()
        {
            Color routeColor = Colors.GreenYellow;
            SolidColorBrush routeBrush = new SolidColorBrush(routeColor);
            var routeLine = new MapPolyline();
            routeLine.Locations = new LocationCollection();
            routeLine.Stroke = routeBrush;
            routeLine.StrokeThickness = 6.0;
            // Add the route line to the new layer.
            selectedLapLayer.Children.Add(routeLine);
            return routeLine;
        }
    }
}
