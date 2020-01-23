using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BodyArchitect.Service.V2.Model
{
    public class Language
    {
        public string Shortcut { get; set; }
        public CultureInfo CultureInfo { get; set; }

        public string EnglishName
        {
            get { return CultureInfo.EnglishName; }
        }


        public string NativeName
        {
            get { return CultureInfo.NativeName; }
        }
        public Language(CultureInfo regionInfo)
        {
            Shortcut = regionInfo.TwoLetterISOLanguageName;
            CultureInfo = regionInfo;
        }

        static List<Language> countriesList;

        public static Language GetLanguage(string shortcut)
        {
            var res = (from c in Languages where StringComparer.CurrentCultureIgnoreCase.Compare(c.Shortcut ,shortcut)==0 select c);
            return res.SingleOrDefault();
        }

        public static List<Language> Languages
        {
            get
            {
                if (countriesList == null)
                {
                    countriesList = GetLanguagesList();
                }
                return countriesList;
            }
        }

        public string DisplayName
        {
            get { return CultureInfo.DisplayName; }
        }

        public static List<Language> GetLanguagesList()
        {
            //create a new Generic list to hold the country names returned
            Dictionary<string, Language> cultureList = new Dictionary<string, Language>();

            //create an array of CultureInfo to hold all the cultures found, these include the users local cluture, and all the
            //cultures installed with the .Net Framework
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.NeutralCultures );


            //loop through all the cultures found
            foreach (CultureInfo culture in cultures)
            {
                if (culture .IsNeutralCulture && !cultureList.ContainsKey(culture.TwoLetterISOLanguageName))
                {
                    cultureList.Add(culture.TwoLetterISOLanguageName, new Language(culture));
                }
            }
            var list = new List<Language>(cultureList.Values);
            return list.OrderBy(t => t.DisplayName).ToList();
        }
    }
}
