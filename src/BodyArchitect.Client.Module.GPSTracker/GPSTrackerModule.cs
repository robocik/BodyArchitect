using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Media;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Module.GPSTracker.Controls;
using BodyArchitect.Client.Module.GPSTracker.Resources;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;
using Microsoft.Windows.Controls.Ribbon;

namespace BodyArchitect.Client.Module.GPSTracker
{
    [Export(typeof(IEntryObjectProvider))]
    [Export(typeof(IBodyArchitectModule))]
    public class GPSTrackerModule: IEntryObjectProvider, IBodyArchitectModule
    {
        public static readonly Guid ModuleId = new Guid("7240E588-75C5-4FE0-8688-5ED4D9532DD6");

        public Guid GlobalId
        {
            get { return ModuleId; }
        }

        public Type EntryObjectControl
        {
            get { return typeof (usrGPSTrackerEntry); }
        }

        public void Startup(EntryObjectLocalizationManager localizationManager, EntryObjectControlManager controlManager)
        {
            localizationManager.RegisterResourceManager(typeof(GPSTrackerEntryDTO), GPSStrings.ResourceManager);
        }

        public void AfterUserLogin()
        {
            
        }

        public ImageSource ModuleImage
        {
            get
            {
                return "pack://application:,,,/BodyArchitect.Client.Module.GPSTracker;component/Resources/GPSTracker32.png".ToBitmap();
            }
        }
        public void CreateRibbon(Ribbon ribbon)
        {
        }

        public Type EntryObjectType
        {
            get { return typeof(GPSTrackerEntryDTO); }
        }

        public ShareSocialContent ShareToSocial(EntryObjectDTO entryObj)
        {
            return null;
        }
    }
}
