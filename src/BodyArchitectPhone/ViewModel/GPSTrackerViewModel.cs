using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Portable;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.UserControls;
using BodyArchitect.WP7.Utils;

namespace BodyArchitect.WP7.ViewModel
{
    public class GPSTrackerViewModel:ViewModelBase
    {
        private GPSTrackerEntryDTO entry;
        private WorldWeatherOnline weatherService;

        public GPSTrackerViewModel(GPSTrackerEntryDTO entry)
        {
            this.entry = entry;

            var tdi = ApplicationState.Current.TrainingDay;
            var gpsBag = tdi.GetGpsCoordinates(entry);
            if (gpsBag != null)
            {
                Points = gpsBag.Points;
            }
            //tdi.SetGpsCoordinates(entry, Points);
            
        }

        //public IList<GPSPoint> Points
        //{
        //    get { return ApplicationState.Current.CurrentBrowsingTrainingDays.GetGpsCoordinates(entry).Points; }
        //}
        public IList<GPSPoint> Points { get; set; }

        public ImageSource GpsSignal
        {
            get { return _gpsSignal; }
            set
            {
                if (_gpsSignal != value)
                {
                    _gpsSignal = value;
                    NotifyPropertyChanged("GpsSignal");
                }

            }
        }

        public bool EditMode
        {
            get { return Entry.TrainingDay.IsMine && Entry.Status != EntryObjectStatus.System; }
        }

        public static decimal? CalculateCalories(decimal met, decimal? duration, IPerson person)
        {
            if (duration == null)
            {//we don't have time of exercising then we cannot calculate calories
                return null;
            }
            DateTime? birthday = null;
            Gender gender = Gender.Male;
            decimal weight = 0;
            decimal height = 0;

            if (person.Birthday != null)
            {
                birthday = person.Birthday;
            }
            gender = person.Gender;
            if (person.Wymiary != null)
            {
                weight = person.Wymiary.Weight;
                height = person.Wymiary.Height;
            }
            if (gender == Gender.NotSet)
            {
                gender = Gender.Male;
            }

            if (weight == 0)
            {//set default weight if not defined
                if (gender == Gender.Female)
                {
                    weight = 70;
                }
                else
                {
                    weight = 100;
                }
            }
            if (gender == Gender.Male)
            {
                if (height == 0)
                {
                    height = 177.4M;
                }
            }
            else
            {//female
                if (height == 0)
                {
                    height = 163M;
                }
            }
            if (birthday == null)
            {//if birthday is not set then assume 30 years old
                birthday = DateTime.UtcNow.AddYears(-30).AddDays(-1);
            }

            int age = birthday.Value.GetAge();
            return WilksFormula.CalculateCaloriesUsingMET(gender == Gender.Male, met, TimeSpan.FromSeconds((double)duration), age, weight, height);
        }

        public void CalculateCaloriesBurned(GPSTrackerEntryDTO gpsEntry,decimal duration, IPerson person)
        {
            gpsEntry.Calories = CalculateCalories(gpsEntry.Exercise.Met, duration, person);
        }

        
        public string TrainingDate
        {
            get { return Entry.TrainingDay.TrainingDate.ToLongDateString(); }
        }

        public GPSTrackerEntryDTO Entry
        {
            get { return entry; }
        }

        public bool HasWeather
        {
            get { return Entry.Weather!=null && Entry.Weather.Condition != WeatherCondition.NotSet; }
        }

        public bool HasAvgSpeed
        {
            get { return Entry.AvgSpeed.HasValue && Entry.AvgSpeed>0; }
        }

        public bool HasMaxSpeed
        {
            get { return Entry.MaxSpeed.HasValue && Entry.MaxSpeed > 0; }
        }

        public bool HasDistance
        {
            get { return Entry.Distance.HasValue && Entry.Distance>0; }
        }

        public bool HasMaxAlt
        {
            get { return Entry.MaxAltitude.HasValue; }
        }

        public bool HasMinAlt
        {
            get { return Entry.MinAltitude.HasValue; }
        }

        public bool HasCalories
        {
            get { return Entry.Calories.HasValue; }
        }

        public bool HasDuration
        {
            get { return Entry.Duration.HasValue && Entry.Duration.Value>0; }
        }

        private bool weatherRetrievingStarted = false;
        private ImageSource _gpsSignal;

        public void RetrieveWeather(GPSPoint location)
        {
            if (ApplicationState.Current.IsOffline || !ApplicationState.Current.IsPremium)
            {//weather conditions are retrieved only for premium or instructor accounts. also skip weather retrieving in offline mode
                return;
            }
            bool hasWeather = Entry.Weather != null && Entry.Weather.Temperature != null;
            if (weatherRetrievingStarted || hasWeather)
            {//run retrieving weather only once
                return;
            }
            weatherRetrievingStarted = true;
            //retrieve weather for user current location
            weatherService = new WorldWeatherOnline();
            weatherService.WeatherLoaded += weatherService_WeatherLoaded;
            weatherService.BeginLoadWeather(location);
        }

        void weatherService_WeatherLoaded(object sender, WeatherRetrievedEventArgs e)
        {
            weatherService.WeatherLoaded -= weatherService_WeatherLoaded;
            weatherService = null;
            Entry.Weather = e.Weather;
        }

        
    }
}
