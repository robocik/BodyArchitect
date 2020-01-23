using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;

namespace BodyArchitect.Common.Localization
{
    public class EntryObjectLocalizationManager
    {
        Dictionary<Type, ResourceManager> resourcesManager = new Dictionary<Type, ResourceManager>();
        static EntryObjectLocalizationManager instance;

        private EntryObjectLocalizationManager()
        {
        }

        public void RegisterResourceManager(Type type, ResourceManager manager)
        {
            resourcesManager.Add(type, manager);
        }

        public static EntryObjectLocalizationManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EntryObjectLocalizationManager();
                }
                return instance;
            }
        }

        public string GetString(Type type,string key)
        {
            if (!resourcesManager.ContainsKey(type))
            {
                throw new InvalidOperationException("There is no resource manager for type: "+type.FullName);
            }
            return resourcesManager[type].GetString(key);
        }
    }
}
