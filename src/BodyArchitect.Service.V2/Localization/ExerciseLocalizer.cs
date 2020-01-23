using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Localization;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Module.StrengthTraining.Localization
{
    public static class ExerciseLocalizer
    {
        public static string GetLocalizedName(this Exercise exercise)
        {
            string resKey = string.Format("{0}_Name", exercise.GlobalId.ToString().ToUpper());
            return getLocalizedProperty(exercise.Name, resKey);
        }

        public static string GetLocalizedUrl(this Exercise exercise)
        {
            string resKey = string.Format("{0}_Url", exercise.GlobalId.ToString().ToUpper());
            return getLocalizedProperty(exercise.Url, resKey);
        }

        public static string GetLocalizedDescription(this Exercise exercise)
        {
            string resKey = string.Format("{0}_Description", exercise.GlobalId.ToString().ToUpper());
            return getLocalizedProperty(exercise.Description, resKey);
        }

        public static string GetLocalizedShortcut(this Exercise exercise)
        {
            string resKey = string.Format("{0}_Shortcut", exercise.GlobalId.ToString().ToUpper());
            return getLocalizedProperty(exercise.Shortcut, resKey);
        }

        private static string getLocalizedProperty(string defaultValue, string resKey)
        {
            string translatedName = ExercisesStrings.ResourceManager.GetString(resKey);
            if (string.IsNullOrEmpty(translatedName))
            {
                translatedName = defaultValue;
            }
            return translatedName;
        }
    }
}
