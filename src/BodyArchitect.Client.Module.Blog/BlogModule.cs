using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Module.Blog.Controls;
using BodyArchitect.Client.Module.Blog.Localization;
using BodyArchitect.Client.Module.Blog.Options;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Settings;
using Microsoft.Windows.Controls.Ribbon;

namespace BodyArchitect.Client.Module.Blog
{
    [Export(typeof(IEntryObjectProvider))]
    [Export(typeof(IBodyArchitectModule))]
    public class BlogModule : IEntryObjectProvider, IBodyArchitectModule
    {
        public static readonly Guid ModuleId = new Guid("29E5BFEB-D8D0-434C-BF37-B22F18E23E9A");

        public Guid GlobalId
        {
            get { return ModuleId; }
        }

        public void Startup(EntryObjectLocalizationManager localizationManager, EntryObjectControlManager controlManager)
        {
            localizationManager.RegisterResourceManager(typeof(BlogEntryDTO), BlogEntryStrings.ResourceManager);
            //controlManager.RegisterControl<BlogEntryDTO>(typeof(usrBlog));
            ApplicationSettings.Register(BlogSettings.Default);
        }

        public void AfterUserLogin()
        {
            
        }

        public Type EntryObjectControl
        {
            get { return typeof(usrBlog); }
        }

        public ImageSource ModuleImage
        {
            get
            {
                return "pack://application:,,,/BodyArchitect.Client.Module.Blog;component/Images/Blog.png".ToBitmap();
            }
        }

        public ShareSocialContent ShareToSocial(EntryObjectDTO entryObj)
        {
            ShareSocialContent content = new ShareSocialContent();
            content.Caption = BlogEntryStrings.BlogModule_ShareToSocial_Caption;
            content.Name = BlogEntryStrings.BlogModule_ShareToSocial_Name;
            StringBuilder descriptionBuilder = new StringBuilder();
            descriptionBuilder.AppendFormat(BlogEntryStrings.BlogModule_ShareToSocial_Description, entryObj.TrainingDay.TrainingDate.ToShortDateString());
            content.Description = descriptionBuilder.ToString();
            return content;
        }

        public Type EntryObjectType
        {
            get { return typeof(BlogEntryDTO); }
        }


        public void CreateRibbon(Ribbon ribbon)
        {
        }
    }
}
