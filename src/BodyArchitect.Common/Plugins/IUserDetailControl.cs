using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using BodyArchitect.Service.Model;

namespace BodyArchitect.Common.Plugins
{
    public interface IUserDetailControl
    {
        void Fill(ProfileInformationDTO user,bool isActive);

        string Caption { get; }

        Image SmallImage { get; }

        bool UpdateGui(ProfileInformationDTO user);
    }
}
