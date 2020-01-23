using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Common.Localization;
using BodyArchitect.Common.Plugins;
using System.ComponentModel.Composition;
using BodyArchitect.Module.Blog.Controls;
using BodyArchitect.Module.Blog.Localization;
using BodyArchitect.Module.Blog.Options;
using BodyArchitect.Service.Model;
using BodyArchitect.Settings;

namespace BodyArchitect.Module.Blog
{
    [Export(typeof(IEntryObjectProvider))]
    public class BlogModule : IEntryObjectProvider
    {
        public static readonly Guid ModuleId = new Guid("29E5BFEB-D8D0-434C-BF37-B22F18E23E9A");

        public Guid GlobalId
        {
            get { return ModuleId; }
        }

        public void Startup(EntryObjectLocalizationManager localizationManager, EntryObjectControlManager controlManager)
        {
            localizationManager.RegisterResourceManager(typeof(BlogEntryDTO), BlogEntryStrings.ResourceManager);
            controlManager.RegisterControl<BlogEntryDTO>(typeof(usrBlog));
            ApplicationSettings.Register(BlogSettings.Default);
        }

        public Image ModuleImage
        {
            get { return BlogResources.BlogModule; }
        }

        public Type EntryObjectType
        {
            get { return typeof(BlogEntryDTO); }
        }

        public void CreateGui(IMainWindow mainWnd)
        {

        }

        public void PrepareNewEntryObject(SessionData sessionData, EntryObjectDTO obj, TrainingDayDTO day)
        {
            
        }

        public void CreateMainMenuItems(MenuStrip menu)
        {

        }

        public void AfterSave(SessionData sessionData, TrainingDayDTO day)
        {

        }
    }
}
