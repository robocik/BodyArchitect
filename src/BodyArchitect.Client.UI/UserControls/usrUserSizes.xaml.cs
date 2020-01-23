using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.UI.UserControls
{
    [Export(typeof(IUserDetailControlBuilder))]
    public class usrUserSizesBuilder : IUserDetailControlBuilder
    {
        public IUserDetailControl Create()
        {
            return new usrUserSizes();
        }
    }

    public partial class usrUserSizes : IUserDetailControl
    {
        public usrUserSizes()
        {
            InitializeComponent();
        }

        public void Fill(ProfileInformationDTO user, bool isActive)
        {
            if (user != null)
            {
                usrWymiaryEditor1.Fill(user.Wymiary);
            }
            usrWymiaryEditor1.SetVisible(user != null);
        }

        public string Caption
        {
            get { return Strings.usrUserSizes_Caption; }
        }

        public ImageSource SmallImage
        {
            get
            {
                BitmapImage source = "Measurements.png".ToResourceUrl().ToBitmap();
                return source;
            }
        }

        public bool UpdateGui(ProfileInformationDTO user)
        {
            return user != null && (user.User.IsMe() || user.User.HaveAccess(user.User.Privacy.Sizes));
        }
    }
}
