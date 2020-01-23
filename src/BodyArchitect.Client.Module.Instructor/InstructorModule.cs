using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.SchedulerEngine;
using BodyArchitect.Client.UI.Views;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Service.V2.Model;
using Microsoft.Windows.Controls.Ribbon;

namespace BodyArchitect.Client.Module.Instructor
{
    [Export(typeof(IBodyArchitectModule))]
    public class InstructorModule : IBodyArchitectModule
    {
        //public static readonly Guid ModuleId = new Guid("6C7EE638-6ACE-4F9D-A1D5-63DB79A5ED61");
        //public Guid GlobalId
        //{
        //    get { return ModuleId; }
        //}


        public void Startup(EntryObjectLocalizationManager localizationManager, EntryObjectControlManager controlManager)
        {
            ResourceDictionary rd = new ResourceDictionary();
            rd.Source = new Uri(@"pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Themes/Generic.xaml");
            Application.Current.Resources.MergedDictionaries.Add(rd);
            
        }

        public void AfterUserLogin()
        {
            ReminderItemsReposidory.Instance.CollectionChanged -= ReminderItemsReposidory_OnCollectionChanged;
            ReminderItemsReposidory.Instance.CollectionChanged += ReminderItemsReposidory_OnCollectionChanged;
            ReminderItemsReposidory.Instance.ClearCache();
        }

        public ImageSource ModuleImage
        {
            get
            {
                return "pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Images/BodyInstructor16.png".ToBitmap();
            }
        }

       void createBodyInstructorRibbon(Ribbon ribbon)
       {
           var mainRibbonTab=EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Instructor:InstructorStrings:InstructorModule_createBodyInstructorRibbon_TabName");
           if (ribbon.Items.Cast<RibbonTab>().Where(x => x.Name == mainRibbonTab).Count() > 0)
           {
               return;
           }
           RibbonTab tab = new RibbonTab();
           tab.Name = mainRibbonTab;
           tab.Header = "ModuleName".TranslateInstructor();
           ribbon.Items.Add(tab);

           RibbonGroup ribbonGroup = new RibbonGroup();

           ribbonGroup.Header = EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Instructor:InstructorStrings:InstructorModule_createBodyInstructorRibbon_RibbonHeader_Catalogs");
           tab.Items.Add(ribbonGroup);

           RibbonGroup customersGroup = new RibbonGroup();
           customersGroup.Header = EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Instructor:InstructorStrings:InstructorModule_createBodyInstructorRibbon_CustomerHeader_Customers");
           tab.Items.Add(customersGroup);

           var button = new RibbonButton();
           button.Label = EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Instructor:InstructorStrings:InstructorModule_createBodyInstructorRibbon_ButtonLabel_Activities"); 
           button.LargeImageSource = "pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Images/Activity32.png".ToBitmap();
           button.SmallImageSource = "pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Images/Activity16.png".ToBitmap();
           button.Click += new System.Windows.RoutedEventHandler(button_Click);
           ribbonGroup.Items.Add(button);

           var rbtnScheduleEntries = new RibbonButton();
           rbtnScheduleEntries.Label = EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Instructor:InstructorStrings:InstructorModule_createBodyInstructorRibbon_ScheduleEntries_Label_Sheduler");
           rbtnScheduleEntries.LargeImageSource = "pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Images/ScheduleEntries.png".ToBitmap();
           rbtnScheduleEntries.Click += new RoutedEventHandler(rbtnScheduleEntries_Click);
           ribbonGroup.Items.Add(rbtnScheduleEntries);

           var rbtnCustomers = new RibbonButton();
           rbtnCustomers.Label = EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Instructor:InstructorStrings:InstructorModule_createBodyInstructorRibbon_Customers_Label_Customers");
           rbtnCustomers.LargeImageSource = "pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Images/Customer32.png".ToBitmap();
           rbtnCustomers.SmallImageSource = "pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Images/Customer16.png".ToBitmap();
           rbtnCustomers.Click += new RoutedEventHandler(rbtnCustomers_Click);
           customersGroup.Items.Add(rbtnCustomers);

           var rbtnCustomerGroups = new RibbonButton();
           rbtnCustomerGroups.Label = EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Instructor:InstructorStrings:InstructorModule_createBodyInstructorRibbon_CustomerGroups_Label_Groups");
           rbtnCustomerGroups.LargeImageSource = "pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Images/CustomerGroup32.png".ToBitmap();
           rbtnCustomerGroups.SmallImageSource = "pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Images/CustomerGroup16.png".ToBitmap();
           rbtnCustomerGroups.Click += new RoutedEventHandler(rbtnCustomerGroups_Click);
           customersGroup.Items.Add(rbtnCustomerGroups);

           var rbtnChampionships = new RibbonButton();
           rbtnChampionships.Label = EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Instructor:InstructorStrings:ChampionshipsView_Header"); 
           rbtnChampionships.LargeImageSource = "pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Images/Championship32.png".ToBitmap();
           rbtnChampionships.SmallImageSource = "pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Images/Championship16.png".ToBitmap();
           rbtnChampionships.Click += new RoutedEventHandler(rbtnChampionships_Click);
           customersGroup.Items.Add(rbtnChampionships);

           var rbtnReminders = new RibbonButton();
           rbtnReminders.Label = EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Instructor:InstructorStrings:InstructorModule_createBodyInstructorRibbon_Reminders_Label_Reminders");
           rbtnReminders.LargeImageSource = "pack://application:,,,/BodyArchitect.Client.Resources;component/Images/Reminder32.png".ToBitmap();
           rbtnReminders.SmallImageSource = "pack://application:,,,/BodyArchitect.Client.Resources;component/Images/Reminder16.png".ToBitmap();
           rbtnReminders.Click += new RoutedEventHandler(rbtnReminders_Click);
           customersGroup.Items.Add(rbtnReminders);
       }

       void rbtnChampionships_Click(object sender, RoutedEventArgs e)
       {
           if (!UIHelper.EnsureInstructorLicence())
           {
               return;
           }
           MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Controls/ChampionshipsView.xaml"));
       }

        public void CreateRibbon(Ribbon ribbon)
        {
            createBodyInstructorRibbon(ribbon);
        }

        void rbtnReminders_Click(object sender, RoutedEventArgs e)
        {
            if(!UIHelper.EnsurePremiumLicence())
            {
                return;
            }
            MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Reminders/Controls/RemindersView.xaml"));
        }

        void rbtnCustomerGroups_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Controls/Groups/CustomerGroupsView.xaml"));
        }

        void rbtnCustomers_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Controls/Customers/CustomersView.xaml"));
        }

        void rbtnScheduleEntries_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Controls/ScheduleEntriesView.xaml"));
        }

        void button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Controls/ActivitiesView.xaml"));
        }

        private void ReminderItemsReposidory_OnCollectionChanged(object sender,NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                ReminderBuilder.Fill(false, e.NewItems.Cast<ReminderItemDTO>().ToArray());
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                ReminderBuilder.Fill(true, e.NewItems.Cast<ReminderItemDTO>().ToArray());
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var toRemove = e.OldItems.Cast<ReminderItemDTO>().ToArray();
                Scheduler.Remove(toRemove.Select(x => x.GlobalId.ToString()).ToArray());
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                Scheduler.RemoveAll();
            }
        }
    }
}
