using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Common.Plugins;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Service.Model;

namespace BodyArchitect.Controls.UserControls
{
    [Export(typeof(IUserDetailControl))]
    public partial class usrUserSizes : UserControl, IUserDetailControl
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
            usrWymiaryEditor1.Visible = user!=null;
        }

        public string Caption
        {
            get { return ApplicationStrings.usrUserSizes_Caption; }
        }

        public Image SmallImage
        {
            get { return Icons.Measurements; }
        }

        public bool UpdateGui(ProfileInformationDTO user)
        {
            return user!=null && ( user.User.IsMe() || user.User.HaveAccess(user.User.Privacy.Sizes) );
        }
    }
}
