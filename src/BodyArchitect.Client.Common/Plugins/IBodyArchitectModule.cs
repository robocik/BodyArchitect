using System.Windows.Media;
using Microsoft.Windows.Controls.Ribbon;

namespace BodyArchitect.Client.Common.Plugins
{
    public interface IBodyArchitectModule
    {
        void Startup(EntryObjectLocalizationManager localizationManager, EntryObjectControlManager controlManager);

        void AfterUserLogin();

        ImageSource ModuleImage
        {
            get;
        }

        void CreateRibbon(Ribbon ribbon);
    }
}
