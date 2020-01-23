using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
using BodyArchitect.WP7.Controls;

namespace BodyArchitect.WP7.ViewModel
{
    public class CountrySelectorViewModel : ViewModelBase
    {
        private ObservableCollection<InGroup<Country>> _countries;
        private static readonly string Groups = "#abcdefghijklmnopqrstuvwxyz";

        public CountrySelectorViewModel()
        {
            GroupedCountries=new ObservableCollection<InGroup<Country>>();

            var groups = new Dictionary<string, InGroup<Country>>();

            foreach (char c in Groups)
            {
                var group = new InGroup<Country>(c.ToString());
                GroupedCountries.Add(group);
                groups[c.ToString()] = group;
            }



            foreach (var country in Country.GetCountries())
            {
                groups[GetFirstNameKey(country)].Add(country);
            }
        }

        public static string GetFirstNameKey(Country country)
        {
            char key = char.ToLower(country.EnglishName[0]);

            if (key < 'a' || key > 'z')
            {
                key = '#';
            }

            return key.ToString();
        }

        public ObservableCollection<InGroup<Country>> GroupedCountries
        {
            get
            {
                return _countries;
            }

            private set
            {
                _countries = value;
                NotifyPropertyChanged("GroupedCountries");
            }
        }
    }
}
