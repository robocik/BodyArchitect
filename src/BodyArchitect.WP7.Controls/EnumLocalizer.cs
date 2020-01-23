using System;
using System.Globalization;
using System.Net;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;

namespace BodyArchitect.WP7.Controls
{
    public class EnumLocalizer
    {
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
                if (defaultInstance == null || defaultInstance.cultureInfo != CultureInfo.CurrentUICulture)
                {
                    defaultInstance = new EnumLocalizer(new ResourceManager("BodyArchitect.Service.Client.WP7.ApplicationStrings", typeof(ApplicationStrings).Assembly));

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

        public string Translate<T>(T type)
        {
            string originalName = Enum.GetName(typeof(T), type);
            string name = ResourceManager.GetString(string.Format("{0}_{1}", typeof(T).Name, originalName));
            if (name == null)
            {
                name = originalName;
            }
            return name;
        }
    }
}
