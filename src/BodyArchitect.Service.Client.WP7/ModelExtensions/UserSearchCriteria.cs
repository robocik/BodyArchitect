using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace BodyArchitect.Service.V2.Model
{
    public partial class UserSearchCriteria
    {
        public UserSearchCriteria()
        {
            Countries=new List<int>();
            this.Genders=new List<Gender>();
            this.UserSearchGroups=new List<UserSearchGroup>();
        }
    }
}
