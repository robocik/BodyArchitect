using System;
using System.Collections.Generic;
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
using System.Xml.Linq;

namespace BodyArchitect.Service.Client.WP7
{
    public class Country
    {
        private static IList<Country> countries;

        public Country()
        {
            
        }
        internal Country (int geoId,string englishName,string nativeName)
        {
            GeoId = geoId;
            EnglishName = englishName;
            NativeName = nativeName;
        }

        public int GeoId { get; set; }

        public string EnglishName { get; set; }

        public string NativeName { get; set; }

        public static IList<Country> GetCountries()
        {
            if (countries == null)
            {
                XDocument document = XDocument.Load("Resources\\countries.xml");
                var res = from e in document.Element("Countries").Elements()
                          select
                              new Country(int.Parse(e.Element("GeoId").Value), e.Element("EnglishName").Value,
                                          e.Element("NativeName").Value);
                countries= res.ToList();
            }
            return countries;
        }

        public static Country GetCountry(int countryId)
        {
            //there are two countries with the same id
            //124
            //201
            //because of this I use FirstOrDefault instead SingleOrDefault
            return (from c in GetCountries() where c.GeoId == countryId  select  c).FirstOrDefault();
        }
    }
}
