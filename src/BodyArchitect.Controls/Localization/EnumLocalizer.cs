using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.Globalization;

namespace BodyArchitect.Controls.Localization
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
                if(defaultInstance==null  || defaultInstance.cultureInfo!=CultureInfo.CurrentUICulture)
                {
                    defaultInstance=new EnumLocalizer(new ResourceManager("BodyArchitect.Controls.Localization.DomainModelStrings",typeof (DomainModelStrings).Assembly));
                    
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
            string originalName = Enum.GetName(typeof (T), type);
            string name = ResourceManager.GetString(string.Format("{0}_{1}", typeof(T).Name, originalName));
            if(name==null)
            {
                name = originalName;
            }
            return name;
        }
    }
}
