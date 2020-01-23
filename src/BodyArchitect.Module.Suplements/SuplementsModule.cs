using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Common.Localization;
using BodyArchitect.Common.Plugins;
using BodyArchitect.Service.Model;
using BodyArchitect.Settings;
using BodyArchitect.Module.Suplements.Controls;
using BodyArchitect.Module.Suplements.Model;

namespace BodyArchitect.Module.Suplements
{
    [Export(typeof(IEntryObjectProvider))]
    public class SuplementsModule : IEntryObjectProvider
    {
        public static readonly Guid ModuleId = new Guid("BD58BEAE-7E5F-424F-92A7-75CA132508FB");

        public Guid GlobalId
        {
            get { return ModuleId; }
        }


        public void Startup( EntryObjectLocalizationManager localizationManager, EntryObjectControlManager controlManager)
        {
            localizationManager.RegisterResourceManager(typeof(SuplementsEntryDTO), SuplementsEntryStrings.ResourceManager);
            controlManager.RegisterControl<SuplementsEntryDTO>(typeof(usrSuplements));
            ApplicationSettings.Register(SuplementsSettings.Default);
        }

        public Image ModuleImage
        {
            get { return SuplementsResources.SuplementsModule; }
        }

        public Type EntryObjectType
        {
            get { return typeof(SuplementsEntryDTO); }
        }

        public void CreateGui(IMainWindow mainWnd)
        {
            
        }

        public void PrepareNewEntryObject(SessionData sessionData, EntryObjectDTO obj, TrainingDayDTO day)
        {
            
        }

        public void CreateMainMenuItems(MenuStrip menu)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(SuplementsEntryStrings.SuplementTypesMenu);
            item.Image = SuplementsResources.SuplementsModule;
            item.Click += new EventHandler(item_Click);
            ((ToolStripMenuItem)menu.Items["mnuTools"]).DropDownItems.Add(item);
        }

        void item_Click(object sender, EventArgs e)
        {
            SuplementsViewWindow dlg = new SuplementsViewWindow();
            dlg.Fill();
            dlg.ShowDialog();
        }

        public void AfterSave(SessionData sessionData, TrainingDayDTO day)
        {
        }
    }
}
