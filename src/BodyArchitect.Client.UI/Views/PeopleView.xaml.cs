using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI.UserControls;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.UI.Views
{
    /// <summary>
    /// Interaction logic for PeopleView.xaml
    /// </summary>
    public partial class PeopleView
    {
        public PeopleView()
        {
            InitializeComponent();
        }

        public override void Fill()
        {
            Header = "PeopleView_Fill_Header_People".TranslateStrings();
            usrPeople.Fill();
            if (PageContext != null && PageContext.User != null)
            {
                usrPeople.Fill(PageContext.User.GlobalId);
            }
            
        }

        public override void RefreshView()
        {
            usrPeople.Fill();
        }


        public override Uri HeaderIcon
        {
            get { return "People.png".ToResourceUrl(); }
        }
    }

    
}
