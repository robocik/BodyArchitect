using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Client.Resources.Localization;

namespace BodyArchitect.Client.UI.Views
{
    [Serializable]
    public class DashboardPageContext:PageContext
    {
        [NonSerialized]
        private Type _controlToShow;

        
        public Type ControlToShow
        {
            get { return _controlToShow; }
            set { _controlToShow = value; }
        }
    }
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : IWeakEventListener
    {
        public DashboardView()
        {
            InitializeComponent();
            PropertyChangedEventManager.AddListener(UserContext.Current, this, string.Empty);
        }
        
        protected override void LoginStatusChanged(LoginStatus newStatus)
        {
            if (newStatus != LoginStatus.Logged)
            {
                usrUserInformation1.ClearContent();
            }
        }


        public override void RefreshView()
        {
            MessagesReposidory.Instance.ClearCache();
            ParentWindow.RunAsynchronousOperation(delegate
            {
                UserContext.Current.RefreshUserData();
                //we don't need to invoke fill method because there is an event and fill will be called automatically
            });
        }

        public override Uri HeaderIcon
        {
            get { return "Dashboard.png".ToResourceUrl(); }
        }


        public override void Fill()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action(Fill));
            }
            else
            {
                Header = Strings.DashBoardView_Fill_Header_Dashboard;
                if (UserContext.Current.IsConnected && UserContext.Current.LoginStatus == LoginStatus.Logged)
                {
                    usrUserInformation1.Fill(UserContext.Current.CurrentProfile);
                    usrUserInformation1.ShowUserDetails(DashboardContext!=null?DashboardContext.ControlToShow:null);
                    //kick the featured data 
                    FeaturedDataReposidory.Instance.BeginEnsure();
                }
                else
                {
                    usrUserInformation1.ClearContent();

                }
            }
        }


        public DashboardPageContext DashboardContext
        {
            get { return (DashboardPageContext) PageContext; }
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            UIHelper.BeginInvoke(Fill, Dispatcher);
            return true;
        }
    }
}
