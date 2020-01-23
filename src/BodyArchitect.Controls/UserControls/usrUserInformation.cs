using System;

using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using BodyArchitect.WCF;
using BodyArchitect.Common.Plugins;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Controls.Reporting;
using BodyArchitect.Controls.WPF;
using BodyArchitect.Logger;
using BodyArchitect.Service.Model;
using BodyArchitect.Shared;
using DevExpress.Utils;
using Guifreaks.Navisuite;

namespace BodyArchitect.Controls.UserControls
{
    public partial class usrUserInformation : usrBaseControl
    {
        private ProfileInformationDTO user;
        private NaviBand navMessages;
        private NaviBand navNewInvitations;
        private NaviBand navSizes;
        private NaviBand navUser;
        private CancellationTokenSource currentTask;

        public usrUserInformation()
        {
            InitializeComponent();

            navUser = addDetailControl(new usrUserInfo());
            navSizes=addDetailControl(new usrUserSizes());
            InvitationListHost wpfHost = new InvitationListHost();
            wpfHost.Child = new InvitationListControl();
            wpfHost.Dock = DockStyle.Fill;
            navNewInvitations = addDetailControl(wpfHost);
            navMessages=addDetailControl(new usrMessagesBoard());
            this.SuspendLayout();
            foreach (var detailControl in PluginsManager.Instance.UserDetailControls)
            {
                addDetailControl(detailControl);
            }
            naviBar1.ActiveBand = navUser;

            updateButtons(false);
            this.ResumeLayout();
            
            
        }

        public void ShowInvitations()
        {
            //naviBar1.ActiveBand = navInvitations;
            naviBar1.ActiveBand = navNewInvitations;
        }

        public void ShowMessages()
        {
            naviBar1.ActiveBand = navMessages;
        }

        private NaviBand addDetailControl(IUserDetailControl detailControl)
        {
            NaviBand band = new NaviBand();

            band.Text = detailControl.Caption;
            band.Tag = detailControl;
            band.LargeImage = detailControl.SmallImage;
            band.SmallImage = detailControl.SmallImage;
            Control ctrl = (Control) detailControl;
            ctrl.Dock = DockStyle.Fill;
            band.ClientArea.Controls.Add(ctrl);
            naviBar1.Controls.Add(band);
            return band;
        }

        
        public void Fill(UserDTO user)
        {
            if(currentTask!=null)
            {
                currentTask.Cancel();
                currentTask = null;
                updateButtons(false);
            }
            this.ClearContent();
            showProgress(true);
            currentTask = ParentWindow.TasksManager.RunAsynchronousOperation(delegate(OperationContext ctx)
            {
                GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
                criteria.UserId = user.Id;
                ProfileInformationDTO usr;
                if (user.IsMe())
                {
                    usr = UserContext.ProfileInformation;
                }
                else
                {
                    usr = ServiceManager.GetProfileInformation(criteria);
                }
                
                if (ctx.CancellatioToken.IsCancellationRequested)
                {
                    return;
                }
                this.user = usr;
                ParentWindow.SynchronizationContext.Send(delegate
                {
                    foreach (NaviBand band in naviBar1.Bands)
                    {
                        if (ctx.CancellatioToken.IsCancellationRequested)
                        {
                            return;
                        }
                        if (band.ClientArea.Controls.Count > 0)
                        {
                            IUserDetailControl ctrl = band.ClientArea.Controls[0] as IUserDetailControl;
                            if (ctrl != null)
                            {
                                ctrl.Fill(this.user, naviBar1.ActiveBand == band);
                                band.Text = ctrl.Caption;
                            }
                        }
                    }
                    //updateButtons();
                    //without these caption of navi bar was not refreshed in some cases
                    naviBar1.Refresh();
                    naviBar1.Update();
                    showProgress(false);
                }, null);


            }, asyncOperationStateChange, null, false);
            
            
        }

        void asyncOperationStateChange(OperationContext context)
        {
            bool startLoginOperation = context.State == OperationState.Started;

            

            updateButtons(startLoginOperation);
        }

        private void showProgress(bool startLoginOperation)
        {
            var usrUser = (usrUserInfo)navUser.ClientArea.Controls[0];
            usrUser.ShowProgress(startLoginOperation);
        }

        public IUserDetailControl ActiveDetailControl
        {
            get
            {
                if (naviBar1.ActiveBand != null)
                {
                    return (IUserDetailControl)naviBar1.ActiveBand.Tag;
                }
                return null;
            }
        }
        private void naviBar1_ActiveBandChanged(object sender, EventArgs e)
        {
            if(naviBar1.ActiveBand!=null)
            {
                IUserDetailControl ctrl = (IUserDetailControl)naviBar1.ActiveBand.Tag;
                ctrl.Fill(user,true);
            }
        }

        public void ClearContent()
        {
            user = null;
            if(currentTask!=null)
            {
                currentTask.Cancel();
                currentTask = null;
            }
            showProgress(false);
            foreach (NaviBand band in naviBar1.Bands)
            {
                if (band.ClientArea.Controls.Count > 0)
                {
                    IUserDetailControl ctrl = band.ClientArea.Controls[0] as IUserDetailControl;
                    if (ctrl != null)
                    {
                        ctrl.Fill(null, naviBar1.ActiveBand == band);
                    }
                }
            }
            
            updateButtons(false);
        }

        void updateButtons(bool operationStarted)
        {
            bool logged =!operationStarted && user != null && UserContext.LoginStatus == LoginStatus.Logged;
            btnEditProfile.Visible = logged && user.User.IsMe();
            btnInviteAFriend.Visible = logged && !user.User.IsMe() && !user.User.IsFriend() && !user.User.IsInInvitation() && !user.User.IsDeleted;
            btnRefresh.Visible = logged && user.User.IsMe();
            btnRejectFriendship.Visible = logged && (user.User.IsFriend() || user.User.IsInInvitation());
            btnReports.Visible = btnOpenWorkoutsLog.Visible = logged && !user.User.IsMe() && user.User.HaveAccess(user.User.Privacy.CalendarView);
            btnAcceptFriend.Visible = logged && user.User.IsInviter();
            btnSendMessage.Visible = logged && !user.User.IsMe() && !user.User.IsDeleted;
            btnAddToFavorites.Visible = logged && !user.User.IsMe() && !user.User.IsFriend() && !user.User.IsFavorite() && !user.User.IsDeleted;
            btnRemoveFromFavorites.Visible = logged && !user.User.IsMe() && !user.User.IsFriend() && user.User.IsFavorite() && !user.User.IsDeleted;
            //flowLayoutPanel1.Visible = !operationStarted;

            

            foreach (NaviBand band in naviBar1.Bands)
            {
                if (band.ClientArea.Controls.Count > 0)
                {
                    IUserDetailControl ctrl = band.ClientArea.Controls[0] as IUserDetailControl;
                    if (ctrl != null)
                    {
                        band.Visible = ctrl.UpdateGui(user);
                    }
                }
            }
        }

        private void btnInviteAFriend_Click(object sender, EventArgs e)
        {
            InvitationWindow dlg = new InvitationWindow();
            dlg.Fill(user.User, InviteFriendOperation.Invite);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                updateButtons(false);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ParentWindow.RunAsynchronousOperation(delegate
            {
                UserContext.RefreshUserData();
            },
              delegate(OperationContext context)
              {
                  btnRefresh.Enabled = context.State != OperationState.Started;
              });

        }

        private void btnRejectFriendship_Click(object sender, EventArgs e)
        {
            InvitationWindow dlg = new InvitationWindow();
            dlg.Fill(user.User, InviteFriendOperation.Reject);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                updateButtons(false);
            }
        }

        private void btnOpenWorkoutsLog_Click(object sender, EventArgs e)
        {
            var userCalendarView = new usrCalendarView();
            userCalendarView.User = user.User;

            var tab = ControlHelper.MainWindow.AddTabPage(userCalendarView, string.Format(ApplicationStrings.CalendarViewForUser_TabText, user.User.UserName), null, true);
            tab.ShowCloseButton = DefaultBoolean.True;
            userCalendarView.Fill();
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            var reporting = new usrReporting();
            reporting.User = user.User;
            var tab = ControlHelper.MainWindow.AddTabPage(reporting, string.Format(ApplicationStrings.ReportsForUser_TabText, user.User.UserName), null, true);
            tab.ShowCloseButton = DefaultBoolean.True;
            reporting.Fill();
        }

        private void btnAcceptFriend_Click(object sender, EventArgs e)
        {
            InvitationWindow dlg = new InvitationWindow();
            dlg.Fill(user.User, InviteFriendOperation.Accept);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                updateButtons(false);
            }
        }

        private void btnEditProfile_Click(object sender, EventArgs e)
        {
            try
            {
                ProfileEditWindow dlg = new ProfileEditWindow();
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //Text = string.Format(ApplicationStrings.MainWindowTitle, Constants.ApplicationName, UserContext.CurrentProfile.UserName);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorLoadProfile, ErrorWindow.EMailReport);
            }
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            SendMessageWindow dlg = new SendMessageWindow();
            MessageDTO msg = new MessageDTO();
            msg.Receiver = user.User;
            msg.Sender = UserContext.CurrentProfile;
            dlg.Fill(msg);
            dlg.ShowDialog(this);
        }

        private void btnAddToFavorites_Click(object sender, EventArgs e)
        {
            try
            {
                PleaseWait.Run(delegate
                {
                    try
                    {
                        ServiceManager.UserFavoritesOperation(user.User, FavoriteOperation.Add);
                    }
                    catch (ObjectIsFavoriteException ex)
                    {//probably second istance remove this profile from favorites
                        ExceptionHandler.Default.Process(ex);
                    }
                    
                    UserContext.RefreshUserData();
                });
                updateButtons(false);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorCannotAddUserToFavorites, ErrorWindow.EMailReport);
            }

        }

        private void btnRemoveFromFavorites_Click(object sender, EventArgs e)
        {
            try
            {
                PleaseWait.Run(delegate
                {
                    try
                    {
                        ServiceManager.UserFavoritesOperation(this.user.User, FavoriteOperation.Remove);
                    }
                    catch (ObjectIsNotFavoriteException ex)
                    {//probably second istance remove this profile from favorites
                        ExceptionHandler.Default.Process(ex);
                    }
                    
                    UserContext.RefreshUserData();
                });
                updateButtons(false);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorCannotRemoveUserFromFavorites, ErrorWindow.EMailReport);
            }
        }
    }
}
