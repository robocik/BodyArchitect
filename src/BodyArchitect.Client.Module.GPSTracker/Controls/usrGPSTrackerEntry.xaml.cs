using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.GPSTracker.Resources;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Portable;
using BodyArchitect.Service.V2.Model;
using Microsoft.Maps.MapControl.WPF;
using Visiblox.Charts;

namespace BodyArchitect.Client.Module.GPSTracker.Controls
{
    public partial class usrGPSTrackerEntry : IWeakEventListener, IValidationControl
    {
        private GPSTrackerViewModel viewModel;

        public usrGPSTrackerEntry()
        {
            InitializeComponent();
            viewModel = new GPSTrackerViewModel();
            DataContext = viewModel;
            PropertyChangedEventManager.AddListener(UserContext.Current, this, string.Empty);
            updateMapMode();
        }

        public GPSTrackerEntryDTO GpsTrackerEntry
        {
            get { return (GPSTrackerEntryDTO)Entry; }
        }



        private usrLapsDetails LapDetails
        {
            get { return (usrLapsDetails)progressControl; }
        }

        public override bool HasProgressPane
        {
            get { return true; }
        }
        public override bool HasDetailsPane
        {
            get { return true; }
        }
        protected override UI.UserControls.usrEntryObjectDetailsBase CreateDetailsControl()
        {
            return new usrGPSTrackerEntryDetails();
        }

        protected override UI.UserControls.usrEntryObjectUserControl CreateProgressControl()
        {
            var laps= new usrLapsDetails(this);
            return laps;
        }

        
        protected override void FillImplementation(SaveTrainingDayResult originalEntry)
        {
            viewModel.Fill(GpsTrackerEntry);
            
            if (GpsTrackerEntry.HasCoordinates)
            {
                ThreadPool.QueueUserWorkItem(x =>
                {
                    Helper.EnsureThreadLocalized();
                    viewModel.RetrieveGpsCoordinates();
                    if (viewModel.GPSPoints == null || viewModel.GPSPoints.Count == 0)
                    {
                        return;
                    }
                    UIHelper.BeginInvoke(new Action(() =>
                        {
                            RefreshGui(true);
                        }), Dispatcher);

                });
            }
            else
            {
                viewModel.GpsCoordinatesStatus = GPSStrings.usrGPSTrackerEntry_InfoEntryWithoutGpsData;
            }
        }

        public void RefreshGui(bool centerMap)
        {
            viewModel.FillLaps();
            map.Fill(viewModel, centerMap);
            showReport();
        }

        private void Track_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var track = (TrackballBehaviour) sender;
            var pkt = track.CurrentPoints.FirstOrDefault();
            DataPoint<DateTime, decimal> timePoint = pkt as DataPoint<DateTime, decimal>;
            DataPoint<decimal, decimal> distancePoint = pkt as DataPoint<decimal, decimal>;
            GPSPointViewModel tag = null;
            if (timePoint != null)
            {
                tag = (GPSPointViewModel)timePoint.Tag;
            }
            if (distancePoint != null)
            {
                tag = (GPSPointViewModel)distancePoint.Tag;
            }
            if (tag != null)
            {
                ShowGpsPoint(tag.Point);
                fillCurrentPoint(tag);
            }
        }

        void fillCurrentPoint(GPSPointViewModel point)
        {
            tbCurrentAltitude.Text = string.Format("{0} {1}", point.DisplayAltitude, UIHelper.AltitudeType);
            tbCurrentSpeed.Text = string.Format("{0} {1}", point.DisplaySpeed, UIHelper.SpeedType);
            tbCurrentPace.Text = point.DisplayPace;
            tbCurrentTime.Text = point.DisplayDateTime;
            tbCurrentDistance.Text = string.Format("{0} {1}", point.DisplayDistance, UIHelper.DistanceType); 
        }

        private void reportType_CheckedChanged(object sender, RoutedEventArgs e)
        {

            showReport();
        }

        void showReport()
        {
            chart.Fill(viewModel.ShowReportByTime, viewModel.GPSPoints);
        }

        private void Chart_OnMouseLeave(object sender, MouseEventArgs e)
        {
            pnlCurrent.Visibility = Visibility.Collapsed;
        }

        private void Chart_OnMouseEnter(object sender, MouseEventArgs e)
        {
            pnlCurrent.Visibility = Visibility.Visible;
        }

        public void SelectLap(LapViewModel lapToSelect)
        {
            var lapPoints = viewModel.GetLapPoints(lapToSelect);
            map.SelectLap(lapPoints);
        }

        public void ShowGpsPoint(GPSPoint point)
        {
            map.PlaceCurrentPointDot(point);
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            //update licence here
            return true;
        }

        void updateMapMode()
        {
            viewModel.MapRoadMode =map.Mode==null ||  map.Mode.ToString() == "Microsoft.Maps.MapControl.WPF.RoadMode";
            viewModel.MapAerialMode = !viewModel.MapRoadMode;
        }

        private void rbtnMapRoadMode_Click(object sender, RoutedEventArgs e)
        {
            map.Mode = new RoadMode();
            updateMapMode();
        }

        private void rbtnMapAerialMode_Click(object sender, RoutedEventArgs e)
        {
            map.Mode = new AerialMode(true);
            updateMapMode();
        }

        private void btnWeather_Click(object sender, RoutedEventArgs e)
        {
            if (!UIHelper.EnsurePremiumLicence())
            {
                return;
            }
            ThreadPool.QueueUserWorkItem(delegate
                                             {
                                                 Helper.EnsureThreadLocalized();
                                                 viewModel.RetrieveWeather();
                                                 SetModifiedFlag();
                                             });
        }


        public bool ValidateControl()
        {
            if (GpsTrackerEntry.Exercise == null)
            {
                BAMessageBox.ShowError(GPSStrings.usrGPSTrackerEntry_ErrActivityNull);
                return false;
            }
            return true;
        }
    }
}
