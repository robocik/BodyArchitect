using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using System.Reflection;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Settings;


namespace BodyArchitect.Client.WCF.Plugins
{
    public class PluginsManager
    {
        CompositionContainer container;
        static PluginsManager instance;
        Dictionary<Type, IEntryObjectProvider> plugins;
        Dictionary<string, IEntryObjectProvider> pluginsByEntryTypeFullname;
        IEntryObjectProvider[] entryProviders = new IEntryObjectProvider[0];
        private bool isReady = false;
        public event EventHandler InitializeCompleted;

        private void onInitializeCompleted()
        {
            if(InitializeCompleted!=null)
            {
                InitializeCompleted(this, EventArgs.Empty);
            }
        }

        [ImportMany]
        public IEntryObjectProvider[] Modules
        {
            get
            {
                return entryProviders;
            }
            set
            {
                entryProviders = value;
            }
        }

        public IList<IOptionsControl> GetOptionsControls() 
        {

            return OptionsControls.Select(v => v.Value).ToList();
        }


        [ImportMany(RequiredCreationPolicy=CreationPolicy.NonShared)]
        IEnumerable<Lazy<IOptionsControl>> OptionsControls
        {
            get;
            set;
        }

        [ImportMany]
        public IUserDetailControl[] UserDetailControls
        {
            get;
            set;
        }


        [ImportMany]
        public ICalendarDayContent[] CalendarDayContents
        {
            get;
            internal set;
        }
        
        public void LoadPlugins(string path)
        {
            var catalog = new DirectoryCatalog(path, "*.Module.*.dll");
            container = new CompositionContainer(catalog);
            container.ComposeParts(this);
            plugins = Modules.ToDictionary(v => v.EntryObjectType);
            pluginsByEntryTypeFullname = Modules.ToDictionary(v => v.EntryObjectType.FullName);
        }

        public void InitializePlugins()
        {
            foreach (var module in Modules)
            {
                module.Startup( /*EntryObjectLocalizationManager.Instance*/ EntryObjectControlManager.Instance);
            }
            IsReady = true;
            ApplicationSettings.Upgrade();

        }

        public static PluginsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PluginsManager();
                    //for design mode only
                    instance.UserDetailControls=new IUserDetailControl[0];
                }
                return instance;
            }
        }

        public bool IsReady
        {
            get { return isReady; }
            private set
            {
                isReady = value;
                onInitializeCompleted();
            }
        }

        public IList<ICalendarDayContent> GetCalendarDayContents(Type supportedEntryType)
        {
            List<ICalendarDayContent> list = new List<ICalendarDayContent>();
            foreach (var content in CalendarDayContents)
            {
                if (content.SupportedEntryType == supportedEntryType)
                {
                    list.Add(content);
                }
            }
            return list;
        }

        public ICalendarDayContent GetCalendarDayContent(Guid globalId)
        {
            foreach (var content in CalendarDayContents)
            {
                if(content.GlobalId==globalId)
                {
                    return content;
                }
            }
            return null;
        }

        public IEntryObjectProvider GetEntryObjectProvider(Guid globalId)
        {
            return (from entry in plugins where entry.Value.GlobalId == globalId select entry.Value).SingleOrDefault();
        }
        public IEntryObjectProvider GetEntryObjectProvider(Type type)
        {
            if (plugins.ContainsKey(type))
            {
                return plugins[type];
            }
            return null;
        }

        public IEntryObjectProvider GetEntryObjectProvider(string entryTypeFullname)
        {
            if (pluginsByEntryTypeFullname.ContainsKey(entryTypeFullname))
            {
                return pluginsByEntryTypeFullname[entryTypeFullname];
            }
            return null;
        }

        public IEntryObjectProvider GetEntryObjectProvider<T>() where T : EntryObjectDTO
        {
            return GetEntryObjectProvider(typeof (T));
        }
    }
}
