using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Common.Plugins;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Service.Model;
using DevExpress.XtraEditors.Controls;

namespace BodyArchitect.Controls.UserControls
{
    public partial class usrProfilesBrowser : usrBaseControl, IMainTabControl
    {
        private PartialRetrievingInfo pagerInfo;
        private CancellationTokenSource currentTask;
        private PagedResult<UserSearchDTO> result;
        private UserSearchCriteria criteria;

        private List<UserSearchDTO> users = null;

        public usrProfilesBrowser()
        {
            InitializeComponent();
            fillSearchCriterias();
            lstUserBrowser.SelectedUserChanged += new EventHandler(listViewEx1_SelectedIndexChanged);
            lstMyContacts.SelectedUserChanged += new EventHandler(listViewEx1_SelectedIndexChanged);
            lstMyContacts.ShowGroups = true;
        }

        void fillSearchCriterias()
        {
            chkSortOrder.Checked = false;
            cmbCountries.Properties.Items.Clear();
            cmbSearchGroups.Properties.Items.Clear();
            cmbGenders.Properties.Items.Clear();
            cmbAccessCalendar.Properties.Items.Clear();
            cmbAccessSizes.Properties.Items.Clear();
            cmbPhoto.Properties.Items.Clear();
            cmbWorkoutPlans.Properties.Items.Clear();
            lblStatus.Text = string.Empty;
            cmbSortOrder.Properties.Items.Clear();

            foreach (var test in Country.Countries)
            {
                CheckedListBoxItem item = new CheckedListBoxItem(test, test.DisplayName);
                cmbCountries.Properties.Items.Add(item);
            }

            foreach (UserSearchGroup test in Enum.GetValues(typeof(UserSearchGroup)))
            {
                CheckedListBoxItem item = new CheckedListBoxItem(test, EnumLocalizer.Default.Translate(test));
                cmbSearchGroups.Properties.Items.Add(item);
            }

            foreach (Gender test in Enum.GetValues(typeof(Gender)))
            {
                CheckedListBoxItem item = new CheckedListBoxItem(test, EnumLocalizer.Default.Translate(test));
                cmbGenders.Properties.Items.Add(item);
            }

            foreach (PrivacyCriteria test in Enum.GetValues(typeof(PrivacyCriteria)))
            {
                BodyArchitect.Controls.ComboBoxItem item = new BodyArchitect.Controls.ComboBoxItem(test, EnumLocalizer.Default.Translate(test));
                cmbAccessCalendar.Properties.Items.Add(item);
            }
            foreach (PrivacyCriteria test in Enum.GetValues(typeof(PrivacyCriteria)))
            {
                BodyArchitect.Controls.ComboBoxItem item = new BodyArchitect.Controls.ComboBoxItem(test, EnumLocalizer.Default.Translate(test));
                cmbAccessSizes.Properties.Items.Add(item);
            }

            foreach (UsersSortOrder test in Enum.GetValues(typeof(UsersSortOrder)))
            {
                BodyArchitect.Controls.ComboBoxItem item = new BodyArchitect.Controls.ComboBoxItem(test, EnumLocalizer.Default.Translate(test));
                cmbSortOrder.Properties.Items.Add(item);
            }

            foreach (PictureCriteria test in Enum.GetValues(typeof(PictureCriteria)))
            {
                BodyArchitect.Controls.ComboBoxItem item = new BodyArchitect.Controls.ComboBoxItem(test, EnumLocalizer.Default.Translate(test));
                cmbPhoto.Properties.Items.Add(item);
            }
            foreach (UserWorkoutPlanCriteria test in Enum.GetValues(typeof(UserWorkoutPlanCriteria)))
            {
                BodyArchitect.Controls.ComboBoxItem item = new BodyArchitect.Controls.ComboBoxItem(test, EnumLocalizer.Default.Translate(test));
                cmbWorkoutPlans.Properties.Items.Add(item);
            }

            cmbAccessSizes.SelectedIndex = 0;
            cmbAccessCalendar.SelectedIndex = 0;
            cmbPhoto.SelectedIndex = 0;
            cmbWorkoutPlans.SelectedIndex = 0;
            cmbSortOrder.SelectedIndex = 0;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            UserContext.ProfileInformationChanged += new EventHandler(UserContext_ProfileInformationChanged);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            UserContext.ProfileInformationChanged -= new EventHandler(UserContext_ProfileInformationChanged);
            base.OnHandleDestroyed(e);
        }

        void UserContext_ProfileInformationChanged(object sender, EventArgs e)
        {
            Fill();
        }

        public void Fill()
        {
            if(InvokeRequired)
            {
                BeginInvoke(new Action(Fill));
            }
            else
            {
                btnSearch.Enabled = UserContext.LoginStatus == LoginStatus.Logged;
                lblStatus.Text = string.Empty;
                lstMyContacts.ClearContent();
                if (UserContext.LoginStatus == LoginStatus.Logged)
                {
                    //do not duplicate user when it is in friends and favorites 
                    Dictionary<int, UserSearchDTO> users = UserContext.ProfileInformation.Friends.ToDictionary(x=>x.Id);
                    foreach (var user in UserContext.ProfileInformation.FavoriteUsers)
                    {
                        if(!users.ContainsKey(user.Id))
                        {
                            users.Add(user.Id,user);
                        }
                    }
                    lstMyContacts.Fill(users.Values);
                }
            }
            
        }


        public void RefreshView()
        {
            UserContext.RefreshUserData();
            Fill();
        }


        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (UserContext.LoginStatus != LoginStatus.Logged)
            {
                throw new AuthenticationException("You must be logged");
            }
            doSearch();
        }

        private void doSearch(bool selectFirstElement=false)
        {
            ClearContent();
            pagerInfo = new PartialRetrievingInfo();
            users=new List<UserSearchDTO>();
            criteria = new UserSearchCriteria();

            foreach (CheckedListBoxItem value in cmbCountries.Properties.Items)
            {
                if (value.CheckState == CheckState.Checked)
                {
                    criteria.Countries.Add(((Country)value.Value).GeoId);
                }
            }

            foreach (CheckedListBoxItem value in cmbGenders.Properties.Items)
            {
                if (value.CheckState == CheckState.Checked)
                {
                    criteria.Genders.Add((Gender)value.Value);
                }
            }
            criteria.SortAscending = chkSortOrder.Checked;
            criteria.AccessCalendar = (PrivacyCriteria)cmbAccessCalendar.SelectedIndex;
            criteria.AccessSizes = (PrivacyCriteria)cmbAccessSizes.SelectedIndex;
            criteria.Picture = (PictureCriteria)cmbPhoto.SelectedIndex;
            criteria.WorkoutPlan = (UserWorkoutPlanCriteria)cmbWorkoutPlans.SelectedIndex;
            criteria.SortOrder = (UsersSortOrder) cmbSortOrder.SelectedIndex;

            foreach (CheckedListBoxItem value in cmbSearchGroups.Properties.Items)
            {
                if (value.CheckState == CheckState.Checked)
                {
                    criteria.UserSearchGroups.Add((UserSearchGroup)value.Value);
                }
            }

            criteria.UserName = txtUserName.Text;
            fillPage(0,selectFirstElement);
        }

        public void ClearContent()
        {
            lstUserBrowser.ClearContent();
            usrUserInformation1.ClearContent();
            lblStatus.Text = string.Empty;
            users = null;
            if (currentTask != null && !currentTask.IsCancellationRequested)
            {
                currentTask.Cancel();
                currentTask = null;
            }
        }

        private void fillPage(int pageIndex,bool selectFirstElement)
        {
            currentTask = ParentWindow.TasksManager.RunAsynchronousOperation(delegate(OperationContext ctx)
              {
                  if (ctx.CancellatioToken == null || ctx.CancellatioToken.IsCancellationRequested || pagerInfo == null)
                  {
                      return;
                  }
                pagerInfo.PageIndex = pageIndex;
                result = ServiceManager.GetUsers( criteria, pagerInfo);
                users.AddRange(result.Items);
                ctx.CancellatioToken.ThrowIfCancellationRequested();
                ParentWindow.SynchronizationContext.Send(delegate
                {
                    lstUserBrowser.Fill(users);
                    if (selectFirstElement && lstUserBrowser.Items.Count > 0)
                    {
                        lstUserBrowser.SelectedIndex=0;
                    }
                    lblStatus.Text = string.Format(ApplicationStrings.PartialLoadedStatus, lstUserBrowser.Items.Count, result.AllItemsCount);
                }, null);


            }, asyncOperationStateChange, null, false);
        }

        void asyncOperationStateChange(OperationContext context)
        {
            bool startLoginOperation = context.State == OperationState.Started;

            changeOperationState(startLoginOperation);
        }

        void changeOperationState(bool startLoginOperation)
        {
            if (startLoginOperation)
            {
                progressIndicator1.Start();
            }
            else
            {
                progressIndicator1.Stop();
            }
            btnSearch.Enabled = UserContext.LoginStatus == LoginStatus.Logged && !startLoginOperation;
            btnMoreResults.Enabled = UserContext.LoginStatus == LoginStatus.Logged && !startLoginOperation && (result != null && lstUserBrowser.Items.Count < result.AllItemsCount);
            progressIndicator1.Visible = startLoginOperation;
        }

        protected override void LoginStatusChanged(LoginStatus newStatus)
        {
            btnSearch.Enabled = newStatus == LoginStatus.Logged;
            btnMoreResults.Enabled = false;
            ClearContent();
        }

        private void btnMoreResults_Click(object sender, EventArgs e)
        {
            fillPage(pagerInfo.PageIndex + 1,false);
        }

        private void listViewEx1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedUser != null)
            {
                usrUserInformation1.Fill(SelectedUser);
            }
        }

        public UserDTO SelectedUser
        {
            get
            {
                if(xtraTabControl1.SelectedTabPage==tpMyContacts)
                {
                    return lstMyContacts.SelectedUser;
                }
                return lstUserBrowser.SelectedUser;
            }
        }


        internal void Fill(UserDTO user)
        {
            Fill(user.UserName);
        }

        internal void Fill(string userName)
        {
            xtraTabControl1.SelectedTabPage = tpBrowser;
            fillSearchCriterias();
            txtUserName.Text = userName;
            doSearch(true);
        }
    }
}
