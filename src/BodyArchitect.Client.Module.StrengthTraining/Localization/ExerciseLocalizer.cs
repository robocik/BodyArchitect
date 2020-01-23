using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.StrengthTraining.Localization
{
    public static class ExerciseLocalizer
    {
        public static string GetLocalizedName(this ExerciseLightDTO exercise)
        {
            //string resKey = string.Format("{0}_Name", exercise.GlobalId.ToString().ToUpper());
            //return getLocalizedProperty(exercise.Name, resKey);
            return exercise.Name;
        }

        public static string GetLocalizedUrl(this ExerciseDTO exercise)
        {
            //string resKey = string.Format("{0}_Url", exercise.GlobalId.ToString().ToUpper());
            //return getLocalizedProperty(exercise.Url, resKey);
            return exercise.Url;
        }

        public static string GetLocalizedDescription(this ExerciseDTO exercise)
        {
            //string resKey = string.Format("{0}_Description", exercise.GlobalId.ToString().ToUpper());
            //return getLocalizedProperty(exercise.Description, resKey);
            return exercise.Description;
        }

        public static string GetLocalizedShortcut(this ExerciseLightDTO exercise)
        {
            //string resKey = string.Format("{0}_Shortcut", exercise.GlobalId.ToString().ToUpper());
            //return getLocalizedProperty(exercise.Shortcut, resKey);
            return exercise.Shortcut;
        }

        //private static string getLocalizedProperty(string defaultValue, string resKey)
        //{
        //    string translatedName = ExercisesStrings.ResourceManager.GetString(resKey);
        //    if (string.IsNullOrEmpty(translatedName))
        //    {
        //        translatedName = defaultValue;
        //    }
        //    return translatedName;
        //}
    }
}
