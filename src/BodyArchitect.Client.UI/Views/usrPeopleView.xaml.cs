using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Authentication;
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
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.UI.Views
{
    /// <summary>
    /// Interaction logic for usrPeopleView.xaml
    /// </summary>
    public partial class usrPeopleView:IWeakEventListener
    {
        public usrPeopleView()
        {
            InitializeComponent();
            DataContext = this;
            
            lstUserBrowser.SelectedUserChanged += new EventHandler(listViewEx1_SelectedIndexChanged);
            lstMyContacts.SelectedUserChanged += new EventHandler(listViewEx1_SelectedIndexChanged);
            lstMyContacts.ShowGroups = true;
            PropertyChangedEventManager.AddListener(UserContext.Current, this, string.Empty);
        }

        private bool SelectFirstElement { get; set; }


        public void Fill()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action(Fill));
            }
            else
            {
                SearchEnabled = UserContext.Current.LoginStatus == LoginStatus.Logged;
                SearchStatus = string.Empty;
                lstMyContacts.ClearContent();
                if (UserContext.Current.LoginStatus == LoginStatus.Logged)
                {
                    //do not duplicate user when it is in friends and favorites 
                    Dictionary<Guid, UserSearchDTO> users = UserContext.Current.ProfileInformation.Friends.ToDictionary(x => x.GlobalId);
                    foreach (var user in UserContext.Current.ProfileInformation.FavoriteUsers)
                    {
                        if (!users.ContainsKey(user.GlobalId))
                        {
                            users.Add(user.GlobalId, user);
                        }
                    }
                    lstMyContacts.Fill(users.Values);
                }
            }

        }


        public void RefreshView()
        {
            UserContext.Current.RefreshUserData();
            Fill();
        }


        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (UserContext.Current.LoginStatus != LoginStatus.Logged)
            {
                throw new Portable.Exceptions.AuthenticationException("You must be logged");
            }
            DoSearch();
        }

        protected override void FillResults(ObservableCollection<UserSearchDTO> result)
        {
            xtraTabControl1.SelectedItem = tpSearch;
            lstUserBrowser.Fill(result);
            if (SelectFirstElement && lstUserBrowser.Items.Count > 0)
            {
                lstUserBrowser.SelectedIndex = 0;
                SelectFirstElement = false;
            }
        }

        protected override void BeforeSearch(object param=null)
        {
            xtraTabControl1.SelectedItem = tpSearch;
            base.BeforeSearch(param);
        }


        public override void ClearContent()
        {
            lstUserBrowser.ClearContent();
            usrUserInformation1.ClearContent();
            base.ClearContent();
        }
        
        private void btnMoreResults_Click(object sender, EventArgs e)
        {
            MoreResults();
        }

        private void listViewEx1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedUser != null)
            {
                usrUserInformation1.Fill(SelectedUser);
            }
        }

        

        internal void Fill(Guid profileId)
        {
            FillSearchCriterias();
            SelectFirstElement = true;
            DoSearch(profileId);
        }

        public UserDTO SelectedUser
        {
            get
            {
                if (xtraTabControl1.SelectedItem == tpMyContacts)
                {
                    return lstMyContacts.SelectedUser;
                }
                return lstUserBrowser.SelectedUser;
            }
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            UIHelper.BeginInvoke(Fill, Dispatcher);
            return true;
        }
    }
}
