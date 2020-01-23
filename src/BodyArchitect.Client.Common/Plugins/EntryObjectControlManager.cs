using System;
using System.Collections.Generic;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Common.Plugins
{
    public class EntryObjectControlManager
    {
        Dictionary<Type, Type> entryObjectControls = new Dictionary<Type, Type>();

        static EntryObjectControlManager instance;

        public static EntryObjectControlManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EntryObjectControlManager();
                }
                return instance;
            }
        }

        public void RegisterControl<T>(Type controlType) where T : EntryObjectDTO
        {
            RegisterControl(typeof(T), controlType);
        }

        public void RegisterControl(Type entryObjectType,Type controlType)
        {
            if (!typeof(EntryObjectDTO).IsAssignableFrom(entryObjectType))
            {
                throw new ArgumentException("EntryObjecType not inherits from EntryObjectDTO class");
            }
            entryObjectControls.Add(entryObjectType, controlType);
        }

        public Dictionary<Type, Type> Controls
        {
            get
            {
                return entryObjectControls;
            }
        }
    }
}
