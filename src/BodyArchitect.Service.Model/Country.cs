using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BodyArchitect.Service.Model
{
    public class Country
    {
        public int GeoId { get; set; }
        public RegionInfo RegionInfo { get; set; }
        public string EnglishName
        {
            get { return RegionInfo.EnglishName; }
        }


        public string NativeName
        {
            get { return RegionInfo.NativeName; }
        }
        public Country(RegionInfo regionInfo)
        {
            GeoId = regionInfo.GeoId;
            RegionInfo = regionInfo;
        }

        public  static Country GetByTwoLetters(string region)
        {
            var res = (from c in Countries where c.RegionInfo.TwoLetterISORegionName == region select c);
            return res.SingleOrDefault();
        }

        public string DisplayName
        {
            get { return RegionInfo.DisplayName; }
        }
        static List<Country>  countriesList;

        public static  Country GetCountry(int geoId)
        {
            var res = (from c in Countries where c.GeoId == geoId select c) ;
            return res.FirstOrDefault();
        }

        public static List<Country> Countries
        {
            get
            {
                if(countriesList==null)
                {
                    countriesList = GetCountryList();
                }
                return countriesList;
            }
        }

        public static List<Country> GetCountryList()
        {
            //create a new Generic list to hold the country names returned
            Dictionary<string, Country> cultureList = new Dictionary<string, Country>();

            //create an array of CultureInfo to hold all the cultures found, these include the users local cluture, and all the
            //cultures installed with the .Net Framework
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);
            

            //loop through all the cultures found
            foreach (CultureInfo culture in cultures)
            {
                if (!culture.IsNeutralCulture && CultureInfo.InvariantCulture.LCID != culture.LCID)
                {
                    try
                    {
                        //pass the current culture's Locale ID (http://msdn.microsoft.com/en-us/library/0h88fahh.aspx)
                        //to the RegionInfo contructor to gain access to the information for that culture
                        RegionInfo region = new RegionInfo(culture.LCID);

                        //make sure out generic list doesnt already
                        //contain this country
                        if (!(cultureList.ContainsKey(region.EnglishName)))
                            //not there so add the EnglishName (http://msdn.microsoft.com/en-us/library/system.globalization.regioninfo.englishname.aspx)
                            //value to our generic list
                            cultureList.Add(region.EnglishName, new Country(region));
                    }
                    catch (Exception)
                    {
                    }
                    
                }
            }
            var list= new List<Country>(cultureList.Values);
            return list.OrderBy(t=>t.DisplayName).ToList();
        }
    }
}
