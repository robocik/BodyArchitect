using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Module.A6W.Controls;
using BodyArchitect.Client.Module.A6W.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Views.Calendar;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using Microsoft.Windows.Controls.Ribbon;

namespace BodyArchitect.Client.Module.A6W
{
    [Export(typeof(IEntryObjectProvider))]
    [Export(typeof(IBodyArchitectModule))]
    public class A6WModule : IEntryObjectProvider, IBodyArchitectModule
    {
        #region Implementation of IEntryObjectProvider

        public static readonly Guid ModuleId = new Guid("A7B7503A-FF26-414D-A8A5-242C137B426C");

        public Guid GlobalId
        {
            get { return ModuleId; }
        }

        public Type EntryObjectControl
        {
            get { return typeof (usrA6W); }
        }

        public void Startup(EntryObjectLocalizationManager localizationManager, EntryObjectControlManager controlManager)
        {
            localizationManager.RegisterResourceManager(typeof(A6WEntryDTO), A6WEntryStrings.ResourceManager);
            //controlManager.RegisterControl<A6WEntryDTO>(typeof(usrA6W));
        }

        public void AfterUserLogin()
        {
            
        }

        public ShareSocialContent ShareToSocial(EntryObjectDTO entryObj)
        {
            A6WEntryDTO a6W = (A6WEntryDTO) entryObj;
            ShareSocialContent content = new ShareSocialContent();
            content.Caption = EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.A6W:A6WEntryStrings:A6WModule_ShareToSocial_Caption");
            content.Name = EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.A6W:A6WEntryStrings:A6WModule_ShareToSocial_Name"); 
            var daysToFinish=A6WManager.Days.Count - a6W.Day.DayNumber;
            if(daysToFinish==0)
            {
                content.Description = string.Format(EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.A6W:A6WEntryStrings:A6WModule_ShareToSocial_Finished_Description"), a6W.Day.DayNumber, a6W.TrainingDay.TrainingDate.ToShortDateString());
            }
            else
            {
                content.Description = string.Format(EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.A6W:A6WEntryStrings:A6WModule_ShareToSocial_NotFinished_Description"), a6W.Day.DayNumber, a6W.TrainingDay.TrainingDate.ToShortDateString(), daysToFinish);    
            }
            
            return content;
        }

        public ImageSource ModuleImage
        {
            get { return "pack://application:,,,/BodyArchitect.Client.Module.A6W;component/Images/A6W.png".ToBitmap(); }
        }

        public Type EntryObjectType
        {
            get { return typeof(A6WEntryDTO); }
        }

        public void CreateRibbon(Ribbon ribbon)
        {
            if (ribbon.Items.Cast<RibbonTab>().Where(x => x.Name == "Calendar").Count() > 0)
            {
                return;
            }
        }

        #endregion
    }

    [Export(typeof(IRibbonCreator))]
    public class A6WCalendarRibbonGroup:IRibbonCreator
    {

        #region Implementation of IRibbonCreator

        public void CreateRibbonGroup(RibbonTab ribbonTab)
        {
            if (ribbonTab.Uid == "Calendar" && ribbonTab.Items.Cast<RibbonGroup>().Where(x=>x.Name=="A6W").Count()==0
                && MainWindow.Instance.CurrentView != null && (MainWindow.Instance.CurrentView.User==null || MainWindow.Instance.CurrentView.User.GlobalId == UserContext.Current.CurrentProfile.GlobalId))
            {
                RibbonGroup ribbonGroup = new RibbonGroup();
                ribbonGroup.Name = "A6W";
                ribbonGroup.Header = EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.A6W:A6WEntryStrings:A6WModule_CreateRibbonGroup_Header");
                ribbonTab.Items.Add(ribbonGroup);

                var button = new RibbonButton();
                button.Label = EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.A6W:A6WEntryStrings:A6WModule_CreateRibbonGroup_Label");
                button.LargeImageSource ="pack://application:,,,/BodyArchitect.Client.Module.A6W;component/Images/A6W.png".ToBitmap();
                button.Click += new System.Windows.RoutedEventHandler(button_Click);
                ribbonGroup.Items.Add(button);
            }
        }

        void button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var view=(NewCalendarView) MainWindow.Instance.CurrentView;
            StartA6WCycleWindow dlg = new StartA6WCycleWindow(view.SelectedDate);
            dlg.Customer = view.Customer;
            if(dlg.ShowDialog()==true)
            {
                view.Fill();
            }
            
        }

        #endregion
    }
}
