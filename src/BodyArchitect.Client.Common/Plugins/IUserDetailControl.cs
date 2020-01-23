using System.Windows.Media;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Common.Plugins
{
    public interface IUserDetailControlBuilder
    {
        IUserDetailControl Create();
    }

    public interface IUserDetailControl
    {
        void Fill(ProfileInformationDTO user,bool isActive);

        string Caption { get; }

        ImageSource SmallImage { get; }

        bool UpdateGui(ProfileInformationDTO user);
    }
}
