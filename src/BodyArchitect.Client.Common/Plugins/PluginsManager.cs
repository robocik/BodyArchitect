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


namespace BodyArchitect.Client.Common.Plugins
{
    public class PluginsManager
    {
        CompositionContainer container;
        static PluginsManager instance;
        Dictionary<Type, IEntryObjectProvider> plugins;
        Dictionary<string, IEntryObjectProvider> pluginsByEntryTypeFullname;
        IEntryObjectProvider[] entryProviders = new IEntryObjectProvider[0];
        IBodyArchitectModule[] modules = new IBodyArchitectModule[0];

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
        public IEntryObjectProvider[] EntryObjectExtensions
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

        [ImportMany]
        public IRibbonCreator[] RibbonCreators
        {
            get; 
            internal set;
        }

        [ImportMany]
        public IBodyArchitectModule[] Modules
        {
            get
            {
                return modules;
            }
            set
            {
                modules = value;
            }
        }

        public IList<IOptionsControl> GetOptionsControls() 
        {

            return OptionsControls.Select(v => v.Value).ToList();
        }

        [ImportMany]
        public IEnumerable<Lazy<IBAReportBuilder>> BAReports
        {
            get;
            internal set;
        }

        [ImportMany(RequiredCreationPolicy=CreationPolicy.NonShared)]
        IEnumerable<Lazy<IOptionsControl>> OptionsControls
        {
            get;
            set;
        }

        [ImportMany]
        public IEnumerable<IUserDetailControlBuilder> UserDetailControls
        {
            get;
            set;
        }


        [ImportMany]
        public IEnumerable<Lazy<ICalendarDayContextEx>> CalendarDayContentsEx
        {
            get;
            internal set;
        }

        
        
        public void LoadPlugins(string path)
        {
            var catalog = new DirectoryCatalog(path, "*.Module.*.dll");
            container = new CompositionContainer(catalog);
            container.ComposeParts(this);
            plugins = EntryObjectExtensions.ToDictionary(v => v.EntryObjectType);
            pluginsByEntryTypeFullname = EntryObjectExtensions.ToDictionary(v => v.EntryObjectType.FullName);
        }

        public void InitializePlugins()
        {
            foreach (var module in Modules)
            {
                module.Startup( EntryObjectLocalizationManager.Instance, EntryObjectControlManager.Instance);
            }
            foreach (var module in EntryObjectExtensions)
            {
                EntryObjectControlManager.Instance.RegisterControl(module.EntryObjectType, module.EntryObjectControl);
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
                    //instance.UserDetailControls=new IUserDetailControl[0];
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


        public ICalendarDayContextEx GetCalendarDayContentEx(Guid globalId)
        {
            foreach (var content in CalendarDayContentsEx)
            {
                if (content.Value.GlobalId == globalId)
                {
                    return content.Value;
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
