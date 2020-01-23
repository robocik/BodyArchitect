using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Module.Suplements.Controls;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Settings;
using Microsoft.Windows.Controls.Ribbon;

namespace BodyArchitect.Client.Module.Suplements
{
    [Export(typeof(IEntryObjectProvider))]
    [Export(typeof(IBodyArchitectModule))]
    public class SuplementsModule : IEntryObjectProvider, IBodyArchitectModule
    {
        public static readonly Guid ModuleId = new Guid("BD58BEAE-7E5F-424F-92A7-75CA132508FB");

        public Guid GlobalId
        {
            get { return ModuleId; }
        }


        public void Startup(EntryObjectLocalizationManager localizationManager, EntryObjectControlManager controlManager)
        {
            localizationManager.RegisterResourceManager(typeof(SuplementsEntryDTO), SuplementsEntryStrings.ResourceManager);
            //controlManager.RegisterControl<SuplementsEntryDTO>(typeof(usrSupplementsEntry));
            ApplicationSettings.Register(SuplementsSettings.Default);
        }

        public void AfterUserLogin()
        {
            
        }

        public ImageSource ModuleImage
        {
            get
            {
                return "pack://application:,,,/BodyArchitect.Client.Module.Suplements;component/Resources/Supplements.png".ToBitmap();
            }
        }

        public Type EntryObjectControl
        {
            get { return typeof(usrSupplementsEntry); }
        }

        public Type EntryObjectType
        {
            get { return typeof(SuplementsEntryDTO); }
        }


        public ShareSocialContent ShareToSocial(EntryObjectDTO entryObj)
        {
            var supple = (SuplementsEntryDTO)entryObj;
            ShareSocialContent content = new ShareSocialContent();
            content.Caption = EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Suplements:SuplementsEntryStrings:SupplementsModule_ShareToSocial_Caption");
            content.Name = EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Suplements:SuplementsEntryStrings:SupplementsModule_ShareToSocial_Name");
            StringBuilder descriptionBuilder = new StringBuilder();
            descriptionBuilder.AppendFormat(EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Suplements:SuplementsEntryStrings:SupplementsModule_ShareToSocial_Date"), entryObj.TrainingDay.TrainingDate.ToShortDateString());
            descriptionBuilder.AppendFormat("<br/>");
            foreach (var item in supple.Items)
            {
                if (item.Suplement != null)
                {
                    descriptionBuilder.AppendFormat(EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Suplements:SuplementsEntryStrings:SupplementsModule_ShareToSocial_Dosage"), item.Suplement.Name, item.Name.Trim(), item.Dosage, EnumLocalizer.Default.Translate(item.DosageType));
                }
            }
            content.Description = descriptionBuilder.ToString();
            return content;
        }


        void item_Click(object sender, EventArgs e)
        {
            MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.Module.Suplements;component/Controls/SupplementsView.xaml"));

        }

        void btnSupplementsCycles_Click(object sender, EventArgs e)
        {
            MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.Module.Suplements;component/Controls/SupplementsCyclesView.xaml"));
        }

        public void CreateRibbon(Ribbon ribbon)
        {
            if (MainWindow.Instance.RibbonHomeTab!= null
                && MainWindow.Instance.RibbonHomeTab.Items.Cast<RibbonGroup>().Where(x => x.Name == "Supplements").Count() == 0)
            {
                RibbonGroup ribbonGroup=new RibbonGroup();
                ribbonGroup.Name = "Supplements";
                ribbonGroup.Header = EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Suplements:SuplementsEntryStrings:SupplementsModule_CreateRibbon_Header_Supplements");
                MainWindow.Instance.RibbonHomeTab.Items.Add(ribbonGroup);

                var button = new RibbonButton();
                button.Label = EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Suplements:SuplementsEntryStrings:SupplementsModule_CreateRibbon_Label_Cycles");
                button.LargeImageSource = "pack://application:,,,/BodyArchitect.Client.Module.Suplements;component/Resources/SupplementsCycle.png".ToBitmap();
                button.Click += new System.Windows.RoutedEventHandler(btnSupplementsCycles_Click);
                ribbonGroup.Items.Add(button);

                button = new RibbonButton();
                button.Label = EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Suplements:SuplementsEntryStrings:SuplementTypesMenu");
                button.SmallImageSource = ModuleImage;
                button.Click += item_Click;
                ribbonGroup.Items.Add(button);

                
            }
        }
    }
}
