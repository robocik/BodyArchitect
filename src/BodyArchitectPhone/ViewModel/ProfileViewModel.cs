using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;

namespace BodyArchitect.WP7.ViewModel
{
    public class ProfileViewModel:ViewModelBase
    {
        private ProfileInformationDTO profile;

        public ProfileViewModel(ProfileInformationDTO profile)
        {
            this.profile = profile;
        }

        public ProfileInformationDTO Profile
        {
            get { return profile; }
        }

        public Privacy CalendarPrivacy
        {
            get
            {
                if (!ApplicationState.Current.IsPremium)
                {
                    return Privacy.Public;
                }
                return profile.User.Privacy.CalendarView;
            }
            set
            { 
                profile.User.Privacy.CalendarView = value;
                NotifyPropertyChanged("CalendarPrivacy");
            }
        }

        public Privacy MeasurementsPrivacy
        {
            get
            {
                if(!ApplicationState.Current.IsPremium)
                {
                    return Privacy.Public;
                }
                return profile.User.Privacy.Sizes;
            }
            set
            {
                profile.User.Privacy.Sizes = value;
                NotifyPropertyChanged("MeasurementsPrivacy");
            }
        }

        private string _password1;
        public string Password1
        {
            get { return _password1; }
            set { _password1 = value; }
        }

        public string Password2 { get; set; }


        public string EMail
        {
            get { return ApplicationState.Current.SessionData.Profile.Email; }
        }

        public DateTime Birthday
        {
            get { return profile.Birthday.Value; }
            set { profile.Birthday=value; }
        }

        public int LengthType
        {
            get { return (int) profile.Settings.LengthType; }
            set { profile.Settings.LengthType = (LengthType) value; }
        }

        public int WeightType
        {
            get { return (int)profile.Settings.WeightType; }
            set { profile.Settings.WeightType = (WeightType)value; }
        }

        public bool AllowComments
        {
            get { return profile.Settings.AllowTrainingDayComments; }
            set
            {
                if (profile.Settings.AllowTrainingDayComments != value)
                {
                    profile.Settings.AllowTrainingDayComments = value;
                    NotifyPropertyChanged("AllowComments");
                }
            }
        }

        public bool AutomaticUpdate
        {
            get { return profile.Settings.AutomaticUpdateMeasurements; }
            set
            {
                if(profile.Settings.AutomaticUpdateMeasurements!=value)
                {
                    profile.Settings.AutomaticUpdateMeasurements = value;
                    NotifyPropertyChanged("AutomaticUpdate");
                }
            }
        }
        public bool IsMale
        {
            get { return profile.User.Gender == Gender.Male; }
            set
            {
                if(value)
                {
                    profile.User.Gender = Gender.Male;
                }
            }
        }

        public bool IsFemale
        {
            get { return profile.User.Gender == Gender.Female; }
            set
            {
                if (value)
                {
                    profile.User.Gender = Gender.Female;
                }
            }
        }

        public string DisplayCountry
        {
            get
            {
                var country = Country;
                if(country!=null)
                {
                    return country.EnglishName;
                }
                return ApplicationStrings.ProfileViewModel_DisplayCountry_SelectCountry;
            }
        }

        public bool ShowPrivacyHelp
        {
            get { return !ApplicationState.Current.IsPremium; }
        }

        public Country Country
        {
            get {return  Country.GetCountry(profile.User.CountryId); }
            set
            {
                if (profile.User.CountryId!=value.GeoId)
                {
                    profile.User.CountryId = value.GeoId;
                    NotifyPropertyChanged("Country");
                    NotifyPropertyChanged("DisplayCountry");
                }
                
            }
        }
    }
}
