using System;
using System.Collections.Generic;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.WCF.Plugins
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
            entryObjectControls.Add(typeof(T), controlType);
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
