using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Views;
using BodyArchitect.Client.UI.Views.Calendar;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;

namespace BodyArchitect.Client.UI.UserControls
{
    /// <summary>
    /// Interaction logic for usrUserInformation.xaml
    /// </summary>
    public partial class usrUserInformation
    {
        private ProfileInformationDTO user;
        private AccordionItem navMessages;
        private AccordionItem navNewInvitations;
        private AccordionItem navSizes;
        private AccordionItem navUser;
        private CancellationTokenSource currentTask;

        public usrUserInformation()
        {
            InitializeComponent();
            DataContext = this;
            navUser = addDetailControl(new usrUserInfo());
            navSizes = addDetailControl(new usrUserSizes());
            //InvitationListHost wpfHost = new InvitationListHost();
            //wpfHost.Child = new InvitationListControl();
            //wpfHost.Dock = DockStyle.Fill;
            navNewInvitations = addDetailControl(new InvitationListControl());
            navMessages = addDetailControl(new usrMessagesBoard());
            //this.SuspendLayout();
            
            //naviBar1.ActiveBand = navUser;
            accordionCtrl.Items.Add(navUser);
            accordionCtrl.Items.Add(navSizes);
            accordionCtrl.Items.Add(navNewInvitations);
            accordionCtrl.Items.Add(navMessages);
            foreach (var detailControl in PluginsManager.Instance.UserDetailControls)
            {
                var item = addDetailControl(detailControl.Create());
                accordionCtrl.Items.Add(item);
            }
            updateButtons(false);
        }


        #region Ribbon

        private bool _showSocialGroup;
        public bool ShowSocialGroup
        {
            get { return _showSocialGroup; }
            set
            {
                _showSocialGroup = value;
                NotifyOfPropertyChange(() => ShowSocialGroup);
            }
        }

        private bool _showUserTab;
        public bool ShowUserTab
        {
            get { return _showUserTab; }
            set
            {
                _showUserTab = value;
                NotifyOfPropertyChange(() => ShowUserTab);
            }
        }

        private bool _showEditProfile;
        public bool ShowEditProfile
        {
            get { return _showEditProfile; }
            set
            {
                _showEditProfile = value;
                NotifyOfPropertyChange(()=>ShowEditProfile);
            }
        }

        private bool _showInvite;
        public bool ShowInvite
        {
            get { return _showInvite; }
            set
            {
                _showInvite = value;
                NotifyOfPropertyChange(() => ShowInvite);
            }
        }

        private bool _showAccept;
        public bool ShowAccept
        {
            get { return _showAccept; }
            set
            {
                _showAccept = value;
                NotifyOfPropertyChange(() => ShowAccept);
            }
        }

        private bool _showReject;
        public bool ShowReject
        {
            get { return _showReject; }
            set
            {
                _showReject = value;
                NotifyOfPropertyChange(() => ShowReject);
            }
        }

        private bool _showCalendar;
        public bool ShowCalendar
        {
            get { return _showCalendar; }
            set
            {
                _showCalendar = value;
                NotifyOfPropertyChange(() => ShowCalendar);
            }
        }

        private bool _showReports;
        public bool ShowReports
        {
            get { return _showReports; }
            set
            {
                _showReports = value;
                NotifyOfPropertyChange(() => ShowReports);
            }
        }

        private bool _showAddToFavorites;
        public bool ShowAddToFavorites
        {
            get { return _showAddToFavorites; }
            set
            {
                _showAddToFavorites = value;
                NotifyOfPropertyChange(() => ShowAddToFavorites);
            }
        }

        private bool _showRemoveFromFavorites;
        public bool ShowRemoveFromFavorites
        {
            get { return _showRemoveFromFavorites; }
            set
            {
                _showRemoveFromFavorites = value;
                NotifyOfPropertyChange(() => ShowRemoveFromFavorites);
            }
        }

        private bool _showSendMessage;
        public bool ShowSendMessage
        {
            get { return _showSendMessage; }
            set
            {
                _showSendMessage = value;
                NotifyOfPropertyChange(() => ShowSendMessage);
            }
        }

        #endregion

        public void ShowInvitations()
        {
            //naviBar1.ActiveBand = navInvitations;
            accordionCtrl.SelectedItem = navNewInvitations;
        }

        public void ShowUserInfo()
        {
            //naviBar1.ActiveBand = navInvitations;
            accordionCtrl.SelectedItem = navUser;
        }

        public void ShowMessages()
        {
            accordionCtrl.SelectedItem = navMessages;
        }


        public void ShowUserDetails(Type userCtrl)
        {
            if (userCtrl != null)
            {
                var item = accordionCtrl.Items.Cast<AccordionItem>().Where(x => x.Content.GetType()== userCtrl).SingleOrDefault();
                accordionCtrl.SelectedItem = item;
            }
            else
            {
                ShowUserInfo();
            }
        }

        private AccordionItem addDetailControl(IUserDetailControl detailControl)
        {
            AccordionItem band = new AccordionItem();
            band.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            band.VerticalContentAlignment = VerticalAlignment.Stretch;

            //Bug fixing: MEF tworzy singletony i po pierwszym dodaniu kontroki jako dziecko parenta przy drugim mamy wyjątek dlatego trzeba odłączyc od starego
            FrameworkElement element = detailControl as FrameworkElement;
            if (element != null && element.Parent!=null)
            {
                ((ContentControl) element.Parent).Content = null;
                element.Tag = band;
            }
            //band.Text = detailControl.Caption;
            
            band.Tag = detailControl;
            band.DataContext = detailControl;
            band.Content = detailControl;
            
            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;
            Image img = new Image();
            img.Source = detailControl.SmallImage;
            img.Width = img.Height = 16;
            panel.Children.Add(img);
            TextBlock textBlock = new TextBlock();
            Binding result = new Binding("Caption");
            result.Source = detailControl;
            textBlock.SetBinding(TextBlock.TextProperty, result);

            textBlock.Style = (Style) this.FindResource("accordionHeaderText");
            panel.Children.Add(textBlock);
            band.Header = panel;

            //band.LargeImage = detailControl.SmallImage;
            //band.SmallImage = detailControl.SmallImage;
            //Control ctrl = (Control)detailControl;
            //ctrl.Dock = DockStyle.Fill;
            //band.ClientArea.Controls.Add(ctrl);
            //naviBar1.Controls.Add(band);))
            return band;
        }


        public void Fill(UserDTO user)
        {
            if (currentTask != null)
            {
                currentTask.Cancel();
                currentTask = null;
                updateButtons(false);
            }
            this.ClearContent();
            if (accordionCtrl.SelectedItem == null)
            {
                ShowUserInfo();
                
            }
            showProgress(true);
            currentTask = ParentWindow.RunAsynchronousOperation(delegate(OperationContext ctx)
            {
                GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
                criteria.UserId = user.GlobalId;
                ProfileInformationDTO usr;
                if (user.IsMe())
                {
                    usr = UserContext.Current.ProfileInformation;
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
                UIHelper.BeginInvoke(new Action(delegate
                {
                    foreach (AccordionItem band in accordionCtrl.Items)
                    {
                        if (ctx.CancellatioToken.IsCancellationRequested)
                        {
                            return;
                        }
                        if (band.Content!=null)
                        {
                            IUserDetailControl ctrl = band.Content as IUserDetailControl;
                            if (ctrl != null)
                            {
                                ctrl.Fill(this.user, accordionCtrl.SelectedItem == band);
                             }
                        }
                    }
                    updateButtons(false);
                    showProgress(false);
                }), Dispatcher);


            }, asyncOperationStateChange, null);


        }

        void asyncOperationStateChange(OperationContext context)
        {
            bool startLoginOperation = context.State == OperationState.Started;



            updateButtons(startLoginOperation);
        }

        private void showProgress(bool startLoginOperation)
        {
            var usrUser = (usrUserInfo) navUser.Content;
            usrUser.ShowProgress(startLoginOperation);
        }

        public IUserDetailControl ActiveDetailControl
        {
            get
            {
                if (accordionCtrl.SelectedItem != null)
                {
                    return (IUserDetailControl)((AccordionItem)accordionCtrl.SelectedItem).Tag;
                }
                return null;
            }
        }
        private void naviBar1_ActiveBandChanged(object sender, SelectionChangedEventArgs e)
        {
            if (accordionCtrl.SelectedItem != null)
            {
                var ctrl = (IUserDetailControl)((AccordionItem)accordionCtrl.SelectedItem).Content;
                ctrl.Fill(user, true);
            }
        }

        public void ClearContent()
        {
            user = null;
            if (currentTask != null)
            {
                currentTask.Cancel();
                currentTask = null;
            }
            showProgress(false);
            //foreach (NaviBand band in naviBar1.Bands)
            //{
            //    if (band.ClientArea.Controls.Count > 0)
            //    {
            //        IUserDetailControl ctrl = band.ClientArea.Controls[0] as IUserDetailControl;
            //        if (ctrl != null)
            //        {
            //            ctrl.Fill(null, naviBar1.ActiveBand == band);
            //        }
            //    }
            //}
            foreach (AccordionItem band in accordionCtrl.Items)
            {
                //if (band.Controls.Count > 0)
                {
                    IUserDetailControl ctrl = band.Content as IUserDetailControl;
                    if (ctrl != null)
                    {
                        ctrl.Fill(null, accordionCtrl.SelectedItem == band);
                    }
                }
            }

            updateButtons(false);
        }

        void updateButtons(bool operationStarted)
        {
            bool logged = !operationStarted && user != null && UserContext.Current.LoginStatus == LoginStatus.Logged;
            ShowEditProfile = logged && user.User.IsMe();
            ShowInvite = logged && !user.User.IsMe() && !user.User.IsFriend() && !user.User.IsInInvitation() &&!user.User.IsDeleted;
            ShowReject = logged && (user.User.IsFriend() || user.User.IsInInvitation());
            ShowAccept = logged && user.User.IsInviter();
            ShowCalendar = logged && !user.User.IsMe() && user.User.HaveAccess(user.User.Privacy.CalendarView);
            ShowReports = ShowCalendar;
            ShowAddToFavorites = logged && !user.User.IsMe() && !user.User.IsFriend() && !user.User.IsFavorite() &&!user.User.IsDeleted;
            ShowRemoveFromFavorites = logged && !user.User.IsMe() && !user.User.IsFriend() && user.User.IsFavorite() && !user.User.IsDeleted;
            ShowSocialGroup = logged && !user.User.IsMe();
            ShowUserTab = logged;
            ShowSendMessage = logged && !user.User.IsMe() && !user.User.IsDeleted;
            
            foreach (AccordionItem band in accordionCtrl.Items)
            {
                //if (band.Controls.Count > 0)
                {
                    IUserDetailControl ctrl = band.Content as IUserDetailControl;
                    if (ctrl != null)
                    {
                        band.SetVisible(ctrl.UpdateGui(user));
                    }
                }
            }
        }

        private void btnInviteAFriend_Click(object sender, EventArgs e)
        {
            InvitationWindow dlg = new InvitationWindow();
            dlg.Fill(user.User, InviteFriendOperation.Invite);
            if (dlg.ShowDialog() ==true)
            {
                updateButtons(false);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            //ParentWindow.RunAsynchronousOperation(delegate
            //{
            //    UserContext.RefreshUserData();
            //},
            //  delegate(OperationContext context)
            //  {
            //      btnRefresh.Enabled = context.State != OperationState.Started;
            //  });

        }

        private void btnRejectFriendship_Click(object sender, EventArgs e)
        {
            InvitationWindow dlg = new InvitationWindow();
            dlg.Fill(user.User, InviteFriendOperation.Reject);
            if (dlg.ShowDialog() ==true)
            {
                updateButtons(false);
            }
        }

        private void btnOpenWorkoutsLog_Click(object sender, EventArgs e)
        {
            //var userCalendarView = new NewCalendarView();
            //userCalendarView.User = user.User;
            //MainWindow.Instance.ShowView(userCalendarView);

            MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.UI;component/Views/Calendar/NewCalendarView.xaml?UserId=" + user.User.GlobalId), () => new CalendarPageContext(user.User, null));
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            //var view = new NewReportingView();
            //view.User = user.User;
            //MainWindow.Instance.ShowView( view);
            MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.UI;component/Views/NewReportingView.xaml?UserId=" + user.User.GlobalId), () => new PageContext(user.User, null));
        }

        private void btnAcceptFriend_Click(object sender, EventArgs e)
        {
            InvitationWindow dlg = new InvitationWindow();
            dlg.Fill(user.User, InviteFriendOperation.Accept);
            if (dlg.ShowDialog() ==true)
            {
                updateButtons(false);
            }
        }

        private void btnEditProfile_Click(object sender, EventArgs e)
        {
            try
            {
                ProfileEditWindow dlg = new ProfileEditWindow();
                if (dlg.ShowDialog() == true)
                {
                    //Text = string.Format(ApplicationStrings.MainWindowTitle, Constants.ApplicationName, UserContext.CurrentProfile.UserName);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex, Strings.ErrorLoadProfile, ErrorWindow.EMailReport);
            }
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            SendMessageWindow dlg = new SendMessageWindow();
            MessageDTO msg = new MessageDTO();
            msg.Receiver = user.User;
            msg.Sender = UserContext.Current.CurrentProfile;
            dlg.Fill(msg);
            dlg.ShowDialog();
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

                    UserContext.Current.RefreshUserData();
                });
                updateButtons(false);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex, Strings.ErrorCannotAddUserToFavorites, ErrorWindow.EMailReport);
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

                    UserContext.Current.RefreshUserData();
                });
                updateButtons(false);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex, Strings.ErrorCannotRemoveUserFromFavorites, ErrorWindow.EMailReport);
            }
        }

        private void RibbonButton_Click(object sender, RoutedEventArgs e)
        {

        }

    }

}
