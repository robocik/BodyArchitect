using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.UI.UserControls
{
    [Export(typeof(IUserDetailControlBuilder))]
    public class InvitationListControlBuilder : IUserDetailControlBuilder
    {
        public IUserDetailControl Create()
        {
            return new InvitationListControl();
        }
    }

    public partial class InvitationListControl : IUserDetailControl
    {
        private ProfileInformationDTO profileInfo;
        ObservableCollection<InvitationListItem> items = new ObservableCollection<InvitationListItem>();

        public InvitationListControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public ObservableCollection<InvitationListItem> Items
        {
            get { return items; }
        }

        public void Fill(ProfileInformationDTO user, bool isActive)
        {
            Items.Clear();
            profileInfo = user;
            if (user != null /*&& isActive*/)
            {
                foreach (var x in user.Invitations)
                {
                    Items.Add(new InvitationListItem(x));
                }
                
                NotifyOfPropertyChange(() => Caption);
            }
        }

        public string Caption
        {
            get
            {
                if (profileInfo != null)
                {
                    return string.Format(Strings.usrInvitationsView_Caption, profileInfo.Invitations.Count);
                }
                return string.Format(Strings.usrInvitationsView_Caption, 0);
            }
        }

        public ImageSource SmallImage
        {
            get
            {
                return "Invitation.png".ToResourceUrl().ToBitmap();
            }
        }

        public bool UpdateGui(ProfileInformationDTO user)
        {
            return user != null && user.User.IsMe() && profileInfo!=null && profileInfo.Invitations.Count>0;
        }


        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)e.OriginalSource;
            InvitationWindow dlg = new InvitationWindow();
            dlg.Fill((UserDTO)btn.Tag, InviteFriendOperation.Accept);
            dlg.ShowDialog();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)e.OriginalSource;
            InvitationWindow dlg = new InvitationWindow();
            dlg.Fill((UserDTO)btn.Tag, InviteFriendOperation.Reject);
            dlg.ShowDialog();
        }

        private void btnUserInfo_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)e.OriginalSource;
            UserDTO user = (UserDTO)btn.Tag;
            if (!user.IsDeleted)
            {
                MainWindow.Instance.ShowUserInformation(user);
            }
        }
    }

    public class InvitationListItem
    {
        private FriendInvitationDTO invitation;

        public InvitationListItem(FriendInvitationDTO invitation)
        {
            this.invitation = invitation;
        }

        public string OperationMessage
        {
            get
            {
                if (invitation.Invited != null)
                {
                    if (invitation.InvitationType == InvitationType.Invite)
                    {
                        return Strings.usrUserInvitationEntry_YouInvitedThisUser_Msg;
                    }

                }
                else
                {
                    if (invitation.InvitationType == InvitationType.Invite)
                    {
                        return Strings.usrUserInvitationEntry_YouAreInvitedByUser_Msg;
                    }
                }
                return null;
            }
        }

        public string OperationAcceptToolTip
        {
            get { return Strings.usrUserInvitationEntry_AcceptInvitation_ToolTip; }
        }

        public Visibility ButtonOkVisible
        {
            get
            {
                bool visible = invitation != null && invitation.Inviter != null && invitation.InvitationType == InvitationType.Invite;
                return visible ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public Visibility ButtonCancelVisible
        {
            get
            {
                bool visible = invitation != null && (invitation.Inviter == null || invitation.InvitationType == InvitationType.Invite || invitation.InvitationType == InvitationType.RejectFriendship);
                return visible ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public string OperationCancelToolTip
        {
            get
            {
                if (invitation.Invited != null)
                {
                    if (invitation.InvitationType == InvitationType.Invite)
                    {
                        return Strings.usrUserInvitationEntry_CancelInvitation_ToolTip;
                    }

                }
                else
                {
                    if (invitation.InvitationType == InvitationType.Invite)
                    {
                        return Strings.usrUserInvitationEntry_RejectInvitation_ToolTIp;
                    }
                }
                return string.Empty;
            }
        }

        public string Message
        {
            get { return !string.IsNullOrWhiteSpace(invitation.Message) ? string.Format("„{0}”", invitation.Message) : Strings.usrUserInvitationEntry_NoMessageText; }
        }

        public string Country
        {
            get { return Service.V2.Model.Country.GetCountry(User.CountryId).DisplayName; }
        }

        public UserDTO User
        {
            get
            {
                return invitation.Inviter ?? invitation.Invited;
            }
        }
    }
}
