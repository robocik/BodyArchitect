using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.GPSTracker.Resources;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.WCF;
using BodyArchitect.Portable;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.GPSTracker.Controls
{
    public class GPSTrackerViewModel:ViewModelBase
    {
        private GPSTrackerEntryDTO entry;
        bool isGpsCoordinatesInProgress;
        bool gpsCoordinatesRetrieved;
        private string gpsCoordinatesStatus;
        private IList<GPSPointViewModel> gpsPoints;
        private bool showReportByTime;
        private IList<LapViewModel> laps;
        private decimal lapLength;
        private bool mapAerialMode;
        private bool mapRoadMode;
        private IList<GPSPoint> points;
        private bool weatherRetrieving;
        private string weatherStatus;

        public GPSTrackerViewModel()
        {
            if (UserContext.Current.ProfileInformation.Settings.LengthType == LengthType.Inchs)
            {
                lapLength = 1609.347087886444m;//default lap length is 1 mile
            }
            else
            {
                lapLength = 1000;//default lap length is 1km    
            }
            WeatherStatus = GPSStrings.GPSTrackerViewModel_WeatherNotAvailable;
        }

        private IList<GPSPoint> Points
        {
            get { return points; }
        }

        public string WeatherStatus
        {
            get
            {
                
                return weatherStatus;
            }
            set
            {
                weatherStatus = value;
                NotifyOfPropertyChange(() => WeatherStatus);
            }
        }

        public bool IsGpsCoordinatesInProgress
        {
            get
            {
                
                return isGpsCoordinatesInProgress;
            }
            set
            {
                isGpsCoordinatesInProgress = value;
                NotifyOfPropertyChange(()=>IsGpsCoordinatesInProgress);
            }
        }

       

        public ImageSource WeatherImage
        {
            get
            {
                if (HasWeatherInfo)
                {
                    string imageUrl = WeatherIcon.GetIcon(entry.Weather.Condition);
                    return string.Format("pack://application:,,,/BodyArchitect.Client.Module.GPSTracker;component/Resources/Weather/{0}", imageUrl).ToBitmap();
                }
                return null;
            }
        }

        public decimal CurrentTemperature
        {
            get
            {
                if (HasWeatherInfo)
                {
                    return ((decimal)entry.Weather.Temperature.Value).ToDisplayTemperature();
                }
                return 0;
            }
        }

        public bool HasCalories
        {
            get { return entry.Calories.HasValue; }
        }

        public bool HasAvgSpeed
        {
            get { return entry.AvgSpeed.HasValue; }
        }

        public bool HasTotalAscent
        {
            get { return entry.TotalAscent.HasValue; }
        }

        public bool HasTotalDescent
        {
            get { return entry.TotalDescent.HasValue; }
        }

        public bool HasMaxSpeed
        {
            get { return entry.MaxSpeed.HasValue; }
        }

        public bool HasWeatherInfo
        {
            get { return entry.Weather != null && entry.Weather.Temperature.HasValue; }
        }

        
        public bool GPSCoordinatesRetrieved
        {
            get
            {

                return gpsCoordinatesRetrieved;
            }
            set
            {
                gpsCoordinatesRetrieved = value;
                NotifyOfPropertyChange(() => GPSCoordinatesRetrieved);
            }
        }

        public string GpsCoordinatesStatus
        {
            get { return gpsCoordinatesStatus; }
            set
            {
                gpsCoordinatesStatus = value;
                NotifyOfPropertyChange(() => IsGpsCoordinatesInProgress);
            }
        }

        public IList<GPSPointViewModel>  GPSPoints
        {
            get { return gpsPoints; }
            set
            {
                gpsPoints = value;
                NotifyOfPropertyChange(()=>GPSPoints);
            }
        }

        public bool ShowReportByTime
        {
            get { return showReportByTime; }
            set
            {
                showReportByTime = value;
                NotifyOfPropertyChange(()=>ShowReportByTime);
            }
        }

        
        public void FillLaps()
        {
            if (points == null)
            {
                return;
            }
            GPSPointsProcessor processor = new GPSPointsProcessor(points, LapLength);
            GPSPoints = processor.Points;
            Laps = processor.Laps;
        }

        public bool MapRoadMode
        {
            get { return mapRoadMode; }
            set
            {
                mapRoadMode = value;
                NotifyOfPropertyChange(() => MapRoadMode);
            }
        }

        public bool WeatherRetrieving
        {
            get { return weatherRetrieving; }
            set
            {
                weatherRetrieving = value;
                NotifyOfPropertyChange(() => WeatherRetrieving);
            }
        }

        public bool MapAerialMode
        {
            get { return mapAerialMode; }
            set
            {
                mapAerialMode = value;
                NotifyOfPropertyChange(() => MapAerialMode);
            }
        }

        public decimal LapLength
        {
            get { return lapLength; }
            set
            {
                lapLength = value;
                FillLaps();
                NotifyOfPropertyChange(()=>LapLength);
            }
        }

        public IList<GPSPointViewModel> RetrieveGpsCoordinates()
        {
            IsGpsCoordinatesInProgress = true;
            GPSCoordinatesRetrieved = false;

            GpsCoordinatesStatus = GPSStrings.GPSTrackerViewModel_RetrievingGpsCoordinates;
            try
            {
                points = ServiceManager.GetGPSCoordinates(entry.GlobalId);
                FillLaps();
                //Thread.Sleep(2000);
                GPSCoordinatesRetrieved = true;
                GpsCoordinatesStatus = string.Empty;
            }
            catch (Exception)
            {
                GpsCoordinatesStatus = GPSStrings.GPSTrackerViewModel_ErrRetrievingGpsCoordinates;
                GPSPoints= new List<GPSPointViewModel>();
                
            }
            finally
            {
                IsGpsCoordinatesInProgress = false;    
            }

            return GPSPoints;
        }
        
        public ExerciseLightDTO Exercise
        {
            get { return entry.Exercise; }
            set
            {
                entry.Exercise = value;
                NotifyOfPropertyChange(()=>Exercise);
            }
        }

        public TimeSpan Duration 
        {
            get { return TimeSpan.FromSeconds( (double) (entry.Duration.HasValue?entry.Duration.Value:0)); }
            set
            {
                entry.Duration = (decimal?) value.TotalSeconds;
                NotifyOfPropertyChange(()=>Duration);
            }
        }

        public decimal? Calories
        {
            get
            {
                if (entry.Calories.HasValue)
                {
                    return entry.Calories.Value;
                }
                return null;
            }
        }

        public decimal? Distance
        {
            get
            {
                if (entry.Distance.HasValue)
                {
                    return entry.Distance.Value.ToDisplayDistance();
                }
                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    entry.Distance = Helper.FromDisplayDistance(value.Value);
                    NotifyOfPropertyChange(() => Distance);
                }
            }
        }

        public decimal? MaxAltitude
        {
            get
            {
                if (entry.MaxAltitude.HasValue)
                {
                    return entry.MaxAltitude.Value.ToDisplayAltitude();
                }
                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    entry.MaxAltitude = Helper.FromDisplayAltitude(value.Value);
                    NotifyOfPropertyChange(() => MaxAltitude);
                }
            }
        }

        public decimal? MinAltitude
        {
            get
            {
                if (entry.MinAltitude.HasValue)
                {
                    return entry.MinAltitude.Value.ToDisplayAltitude();
                }
                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    entry.MinAltitude = Helper.FromDisplayAltitude(value.Value);
                    NotifyOfPropertyChange(() => MinAltitude);
                }
            }
        }

        public string DisplayAvgSpeed
        {
            get
            {
                if (entry.AvgSpeed.HasValue)
                {
                    return entry.AvgSpeed.Value.ToDisplaySpeed().ToString("0.#");
                }
                return null;
            }
        }

        public string DisplayTotalAscent
        {
            get
            {
                if (entry.TotalAscent.HasValue)
                {
                    return entry.TotalAscent.Value.ToDisplayAltitude().ToString("0.#");
                }
                return null;
            }
        }

        public string DisplayTotalDescent
        {
            get
            {
                if (entry.TotalDescent.HasValue)
                {
                    return entry.TotalDescent.Value.ToDisplayAltitude().ToString("0.#");
                }
                return null;
            }
        }

        public string DisplayMaxSpeed
        {
            get
            {
                if (entry.MaxSpeed.HasValue)
                {
                    return entry.MaxSpeed.Value.ToDisplaySpeed().ToString("0.#");
                }
                return null;
            }
        }

        public string DisplayAvgPace
        {
            get
            {
                if (entry.AvgSpeed.HasValue)
                {
                    return WilksFormula.PaceToString((float) entry.AvgSpeed.Value.ToDisplayPace(),true);
                }
                return null;
            }
        }

        public IList<LapViewModel> Laps
        {
            get { return laps; }
            private set
            {
                laps = value;
                NotifyOfPropertyChange(()=>Laps);
            }
        }


        public IList<GPSPointViewModel> GetLapPoints(LapViewModel lapToSelect)
        {
            if (lapToSelect == null)
            {
                return new List<GPSPointViewModel>();
            }
            var startIndex=gpsPoints.IndexOf(lapToSelect.StartPoint);
            var endIndex = gpsPoints.IndexOf(lapToSelect.EndPoint);
            return gpsPoints.Skip(startIndex).Take(endIndex - (startIndex-1)).ToList();
        }

        public void Fill(GPSTrackerEntryDTO gpsTrackerEntry)
        {
            this.entry = gpsTrackerEntry;
        }

        public void RetrieveWeather()
        {
            WeatherRetrieving = true;
            
            WeatherStatus = GPSStrings.GPSTrackerViewModel_RetrievingCurrentWeather;
            try
            {
                var weatherService = new WorldWeatherOnline();
                entry.Weather = weatherService.LoadWeather();
                WeatherStatus = "";
                NotifyOfPropertyChange(() => HasWeatherInfo);
                NotifyOfPropertyChange(() => WeatherImage);
                NotifyOfPropertyChange(() => CurrentTemperature);
            }
            catch (Exception)
            {
                WeatherStatus = GPSStrings.GPSTrackerViewModel_ErrRetrieveWeather;
            }
            finally
            {
                WeatherRetrieving = false;
            }
        }
    }
}
