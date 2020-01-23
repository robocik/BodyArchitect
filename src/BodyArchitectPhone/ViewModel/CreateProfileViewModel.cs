using System;
using System.Net;
using System.ServiceModel;
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
    public class CreateProfileViewModel:ViewModelBase
    {
        private int countryId;
        private Gender gender=Gender.Male;
        private DateTime _birthday = DateTime.Now;
        public string Username { get; set; }

        public string Email { get; set; }

        
        public DateTime Birthday
        {
            get { return _birthday; }
            set { _birthday = value; }
        }

        public bool IsMale
        {
            get { return gender == Gender.Male; }
            set
            {
                if (value)
                {
                    gender = Gender.Male;
                }
            }
        }

        public bool IsFemale
        {
            get { return gender == Gender.Female; }
            set
            {
                if (value)
                {
                    gender = Gender.Female;
                }
            }
        }

        public string DisplayCountry
        {
            get
            {
                var country = Country;
                if (country != null)
                {
                    return country.EnglishName;
                }
                return ApplicationStrings.ProfileViewModel_DisplayCountry_SelectCountry;
            }
            set { }
        }

        public Country Country
        {
            get { return Country.GetCountry(countryId); }
            set
            {
                if (value!=null && countryId != value.GeoId)
                {
                    countryId = value.GeoId;
                    NotifyPropertyChanged("Country");
                    NotifyPropertyChanged("DisplayCountry");
                }

            }
        }

        public string Password
        {
            get; set;
        }

        public string ConfirmPassword
        {
            get;
            set;
        }
    }
}
