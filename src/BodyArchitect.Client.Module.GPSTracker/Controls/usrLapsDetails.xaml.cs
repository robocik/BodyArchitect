using System.Collections.ObjectModel;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.GPSTracker.Resources;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Portable;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.GPSTracker.Controls
{
    public partial class usrLapsDetails
    {
        ObservableCollection<ListItem<decimal>> scales = new ObservableCollection<ListItem<decimal>>();
        private LapViewModel selectedLap;
        private usrGPSTrackerEntry parentCtrl;
        private GPSPointViewModel selectedPoint;

        public usrLapsDetails(usrGPSTrackerEntry parentCtrl)
        {
            InitializeComponent();
            this.parentCtrl = parentCtrl;
            DataContext = this;

            if (UserContext.Current.ProfileInformation.Settings.LengthType == LengthType.Inchs)
            {
                scales.Add(new ListItem<decimal>("0.5 mi", 804.6735439432222m));
                scales.Add(new ListItem<decimal>("1.0 mi", 1609.347087886444m));
                scales.Add(new ListItem<decimal>("2.0 mi", 3218.694175772889m));
                scales.Add(new ListItem<decimal>("5.0 mi", 8046.735439432222m));
                scales.Add(new ListItem<decimal>("10.0 mi", 16093.47087886444m));
            }
            else
            {
                scales.Add(new ListItem<decimal>("0.5 km", 500));
                scales.Add(new ListItem<decimal>("1.0 km", 1000));
                scales.Add(new ListItem<decimal>("2.0 km", 2000));
                scales.Add(new ListItem<decimal>("5.0 km", 5000));
                scales.Add(new ListItem<decimal>("10.0 km", 10000));    
            }
            
        }

        public string LapTimeHeader
        {
            get { return GPSStrings.usrLapsDetails_Header_LapTime; }
        }

        public string TotalTimeHeader
        {
            get { return GPSStrings.usrLapsDetails_Header_TotalTime; }
        }

        public string DistanceHeader
        {
            get { return string.Format(GPSStrings.usrLapsDetails_Header_Distance, UIHelper.DistanceType); }
        }

        public string SpeedHeader
        {
            get { return string.Format(GPSStrings.usrLapsDetails_Header_Speed, UIHelper.SpeedType); }
        }

        public string PaceHeader
        {
            get { return string.Format(GPSStrings.usrLapsDetails_Header_Pace,UIHelper.PaceType); }
        }

        public GPSPointViewModel SelectedPoint
        {
            get { return selectedPoint; }
            set
            {
                selectedPoint = value;
                if (selectedPoint != null)
                {
                    parentCtrl.ShowGpsPoint(selectedPoint.Point);
                }
            }
        }
        public GPSTrackerViewModel ViewModel
        {
            get { return (GPSTrackerViewModel) parentCtrl.DataContext; }
        }

        public override void Fill(Service.V2.Model.EntryObjectDTO entry)
        {
            
        }

        public decimal LapLength
        {
            get { return ViewModel.LapLength; }
            set
            {
                ViewModel.LapLength = value;
                parentCtrl.RefreshGui(false);
                NotifyOfPropertyChange(() => LapLength);
            }
        }

        public ObservableCollection<ListItem<decimal>> Scales
        {
            get { return scales; }
        }

        public LapViewModel SelectedLap
        {
            get { return selectedLap; }
            set
            {
                selectedLap = value;
                parentCtrl.SelectLap(value);
            }
        }
    }
}
