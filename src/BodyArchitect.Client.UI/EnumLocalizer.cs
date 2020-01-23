using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.Globalization;
using System.Windows;
using BodyArchitect.Client.Resources.Localization;
using WPFLocalizeExtension.Engine;
using WPFLocalizeExtension.Extensions;

namespace BodyArchitect.Client.UI
{
    public static class Localizer
    {
        public static string Translate(this string key)
        {
            return EnumLocalizer.Default.GetUIString(key);
        }

        public static string TranslateGUI(this string key)
        {
            return EnumLocalizer.Default.GetGUIString(key);
        }

        public static string TranslateStrings(this string key)
        {
            return EnumLocalizer.Default.GetStringsString(key);
        }
    }

    public class EnumLocalizer
    {
        public const string EntryObjectName = "EntryTypeName";
        public const string EntryObjectShortDescription = "EntryTypeShortDescription";

        private ResourceManager resourceManager;
        private CultureInfo cultureInfo;
        private static EnumLocalizer defaultInstance = null;

        public EnumLocalizer(ResourceManager resManager)
        {
            resourceManager = resManager;
            cultureInfo = CultureInfo.CurrentUICulture;
        }

        public static EnumLocalizer Default
        {
            get
            {
                if(defaultInstance==null  || defaultInstance.cultureInfo!=CultureInfo.CurrentUICulture)
                {
                    defaultInstance = new EnumLocalizer(new ResourceManager("BodyArchitect.Client.Resources.Localization.Strings", typeof(Strings).Assembly));
                    
                }
                return defaultInstance;
            }
        }
        public ResourceManager ResourceManager
        {
            get
            {
                return resourceManager;
            }
        }

        public string GetUIString(string key)
        {
            //var locExtension = new LocTextExtension(key);
            var splitedKey=key.Split(':');
            if (splitedKey.Length == 3 && LocalizeDictionary.Instance.ResourceKeyExists(splitedKey[0], splitedKey[1], splitedKey[2]))
            {
                var uiString = (string)LocalizeDictionary.Instance.GetLocalizedObject(splitedKey[0], splitedKey[1], splitedKey[2], LocalizeDictionary.Instance.Culture);
                //locExtension.ResolveLocalizedValue(out uiString);
                return uiString;    
            }
#if DEBUG
            throw new ArgumentException("Missing localization: " + key);
#else
            return key;
#endif

        }

        //public bool ResolveLocalizedValue<TValue>(out TValue resolvedValue, CultureInfo targetCulture, DependencyObject target)
        //{
        //    // define the default value of the resolved value
        //    resolvedValue = default(TValue);

        //    // get the localized object from the dictionary
        //    string resKey = targetCulture.Name + ":" + typeof(TValue).Name + ":" + this.Key;

        //    if (ResourceBuffer.ContainsKey(resKey))
        //    {
        //        resolvedValue = (TValue)ResourceBuffer[resKey];
        //    }
        //    else
        //    {
        //        object localizedObject = LocalizeDictionary.Instance.GetLocalizedObject(this.Key, target, targetCulture);

        //        if (localizedObject == null)
        //            return false;

        //        object result = this.Converter.Convert(localizedObject, typeof(TValue), this.ConverterParameter, targetCulture);

        //        if (result is TValue)
        //        {
        //            resolvedValue = (TValue)result;
        //            ResourceBuffer.Add(resKey, resolvedValue);
        //        }
        //    }

        //    if (resolvedValue != null)
        //        return true;

        //    return false;
        //}

        public string GetGUIString(string key)
        {
            return GetUIString("BodyArchitect.Client.Resources:Strings:" + key);
        }

        public string GetStringsString(string key)
        {
            return GetUIString("BodyArchitect.Client.Resources:Strings:" + key);
        }

        public string Translate<T>(T type)
        {
            string originalName = Enum.GetName(typeof (T), type);
            string name = GetUIString(string.Format("BodyArchitect.Client.Resources:Strings:{0}_{1}", typeof(T).Name, originalName));
            if(name==null)
            {
                name = originalName;
            }
            return name;
        }
    }
}
