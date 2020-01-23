using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;

namespace BodyArchitect.Service.V2.Localization
{
    static class SupplementLocalizer
    {
        public static string GetLocalizedName(this Suplement exercise)
        {
            string resKey = string.Format("{0}_Name", exercise.GlobalId.ToString().ToUpper());
            return getLocalizedProperty(exercise.Name, resKey);
        }

        public static string GetLocalizedUrl(this Suplement exercise)
        {
            string resKey = string.Format("{0}_Url", exercise.GlobalId.ToString().ToUpper());
            return getLocalizedProperty(exercise.Url, resKey);
        }


        private static string getLocalizedProperty(string defaultValue, string resKey)
        {
            string translatedName = SupplementsStrings.ResourceManager.GetString(resKey);
            if (string.IsNullOrEmpty(translatedName))
            {
                translatedName = defaultValue;
            }
            return translatedName;
        }
    }
}
