using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Settings;
using BodyArchitect.Settings.Model;
using BodyArchitect.Shared;

namespace BodyArchitect.Client.UI.UserControls
{
    /// <summary>
    /// Interaction logic for usrCalendarOptions.xaml
    /// </summary>
    public partial class usrCalendarOptions : IOptionsControl
    {
        private ObservableCollection<ModuleSettingViewModel> modules = new ObservableCollection<ModuleSettingViewModel>();
        private bool canUp;
        private bool canDown;


        public usrCalendarOptions()
        {
            InitializeComponent();
            DataContext = this;
        }

        #region Implementation of IOptionsControl

        public void Save()
        {
            CalendarOptions options = new CalendarOptions();
            UserContext.Current.Settings.GuiState.ShowRelativeDates = chkShowRelativeDates.IsChecked.Value;

            saveList(options);
            UserContext.Current.Settings.GuiState.CalendarOptions = options;
            UserContext.Current.Settings.GuiState.SaveGUI = chkSaveGUI.IsChecked.Value;
            GuiState.Default.SmartEditingDataGrid = chkSmartGridEdit.IsChecked.Value;
        }

        void saveList(CalendarOptions options)
        {
            options.DefaultEntries.Clear();
            for (int index = 0; index < lvModulesSettings.Items.Count; index++)
            {
                ModuleSettingViewModel item = Modules[index];
//Guid moduleGlobalId = item.Value;
                //options.DefaultEntries[moduleGlobalId].IsDefault = item.IsDefault;
                item.Order = index;
                options.DefaultEntries.Add(item.Item);
            }
        }

        public void Fill()
        {
            CalendarOptions options = UserContext.Current.Settings.GuiState.CalendarOptions;

            fillModulesList(options);
            chkSaveGUI.IsChecked = UserContext.Current.Settings.GuiState.SaveGUI;
            chkShowRelativeDates.IsChecked = UserContext.Current.Settings.GuiState.ShowRelativeDates;
            chkSmartGridEdit.IsChecked = GuiState.Default.SmartEditingDataGrid;
        }

        void fillModulesList(CalendarOptions options)
        {

            //for (int i = 0; i < PluginsManager.Instance.EntryObjectExtensions.Length; i++)
            //{
            //    var module = PluginsManager.Instance.EntryObjectExtensions[i];
            //    var optionItem=options.DefaultEntries.Where(x => x.ModuleId == module.GlobalId).SingleOrDefault();
            //    if(optionItem!=null)
            //    {
            //        var item = new ModuleSettingViewModel(module,optionItem);
            //        item.IsDefault = options.GetDefaultEntry(module.GlobalId);
            //        lvModulesSettings.Items.Add(item);
            //    }
                
            //}
            var temp=new ObservableCollection<ModuleSettingViewModel>();
            
            for (int i = 0; i < PluginsManager.Instance.EntryObjectExtensions.Length; i++)
            {
                var module = PluginsManager.Instance.EntryObjectExtensions[i];
                
                var optionItem = options.DefaultEntries.Where(x => x.ModuleId == module.GlobalId).SingleOrDefault();
                if (optionItem == null)
                {
                    optionItem = new CalendarOptionsItem(module.GlobalId, false);
                }
                

                ModuleSettingViewModel item = new ModuleSettingViewModel(optionItem);
                item.Value = module.GlobalId;
                
                item.Text = EntryObjectLocalizationManager.Instance.GetString(module.EntryObjectType, EnumLocalizer.EntryObjectName);
                item.Image = module.ModuleImage;
                item.IsDefault = options.GetDefaultEntry(module.GlobalId);
                item.CanBeDefault = canBeDefault(module.EntryObjectType);
                temp.Add(item);
            }

            for (int i = 0; i < PluginsManager.Instance.CalendarDayContentsEx.Count(); i++)
            {
                var module = PluginsManager.Instance.CalendarDayContentsEx.ElementAt(i);
                
                var optionItem = options.DefaultEntries.Where(x => x.ModuleId == module.Value.GlobalId).SingleOrDefault();
                
                if (optionItem == null)
                {
                    optionItem = new CalendarOptionsItem(module.Value.GlobalId, false);
                }

                ModuleSettingViewModel item = temp.Where(x => x.Value == module.Value.GlobalId).SingleOrDefault();

                if (item == null)
                {
                    item = new ModuleSettingViewModel( optionItem);
                    item.Value = module.Value.GlobalId;
                    item.Text = module.Value.Name;
                    item.Image = module.Value.Image;
                    item.CanBeDefault = false;
                    temp.Add(item);
                }
                item.Order = options.DefaultEntries.IndexOf(optionItem);
                
            }
            Modules = new ObservableCollection<ModuleSettingViewModel>(temp.OrderBy(x => x.Order));
            //foreach (var calendarOptionsItem in options.DefaultEntries)
            //{
            //    var module = PluginsManager.Instance.GetEntryObjectProvider(calendarOptionsItem.ModuleId);
            //    var item = new ModuleSettingViewModel(module, calendarOptionsItem);
            //    item.IsDefault = options.GetDefaultEntry(module.GlobalId);
            //    lvModulesSettings.Items.Add(item);
            //}
        }

        bool canBeDefault(Type entryType)
        {
            var instanceAttrib = entryType.GetCustomAttributes(typeof(EntryObjectInstanceAttribute),false);
            if(instanceAttrib.Length>0)
            {
                return ((EntryObjectInstanceAttribute) instanceAttrib[0]).Instance != EntryObjectInstance.None;
            }
            return false;
        }

        public bool RestartRequired
        {
            get { return false; }
        }

        public string Title
        {
            get { return Strings.OptionsCalendarTitle; }
        }

        public ImageSource Image
        {
            get
            {
                return "Calendar.png".ToResourceUrl().ToBitmap();
            }
        }

        public ObservableCollection<ModuleSettingViewModel> Modules
        {
            get { return modules; }
            set
            {
                modules = value;
                NotifyOfPropertyChange(()=>Modules);
            }
        }

        public bool CanUp
        {
            get { return canUp; }
            set
            {
                canUp = value;
                NotifyOfPropertyChange(()=>CanUp);
            }
        }

        public bool CanDown
        {
            get { return canDown; }
            set
            {
                canDown = value;
                NotifyOfPropertyChange(() => CanDown);
            }
        }

        #endregion

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            if (lvModulesSettings.SelectedIndex>0)
            {
                Modules.Move(lvModulesSettings.SelectedIndex, lvModulesSettings.SelectedIndex - 1);
            }
            updateMoveButtons();
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            if (Modules.Count-1 > lvModulesSettings.SelectedIndex)
            {
                Modules.Move(lvModulesSettings.SelectedIndex, lvModulesSettings.SelectedIndex + 1);    
            }
            updateMoveButtons();
        }


        void updateMoveButtons()
        {
            CanDown = Modules.Count - 1 > lvModulesSettings.SelectedIndex;
            CanUp = lvModulesSettings.SelectedIndex > 0;
        }
        private void lvModulesSettings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateMoveButtons();
        }
    }

    public class ModuleSettingViewModel:ImageSourceListItem<Guid>
    {
        private CalendarOptionsItem item;

        public ModuleSettingViewModel(CalendarOptionsItem item)
        {
            this.item = item;
            ShowIcon = true;
            CanBeDefault = true;
        }

        public int Order
        {
            get { return item.Order; }
            set { item.Order = value; }
        }

        public CalendarOptionsItem Item { get { return item; } }

        public bool ShowIcon { get; set; }

        public bool CanBeDefault { get; set; }

        public bool? IsDefault { get { return item.IsDefault; } set { item.IsDefault = value; } }
    }
}
