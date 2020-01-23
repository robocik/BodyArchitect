

using System.Windows.Media;

namespace BodyArchitect.Client.Common.Plugins
{
    public interface IOptionsControl
    {
        void Save();
        void Fill();
        bool RestartRequired { get; }
        string Title { get; }
        ImageSource Image { get; }
    }
}
