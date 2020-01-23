using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BodyArchitect.Common.Plugins;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Service.Model;
using Image = System.Drawing.Image;

namespace BodyArchitect.Controls.WPF
{
    [Export(typeof(IUserDetailControl))]
    public partial class InvitationListControl : UserControl, IUserDetailControl
    {
        private ProfileInformationDTO profileInfo;

        public InvitationListControl()
        {
            InitializeComponent();
        }

        public void Fill(ProfileInformationDTO user, bool isActive)
        {
            profileInfo = user;
            if (user != null && isActive)
            {
                lstInvitations.ItemsSource = user.Invitations.Select(x=>new InvitationListItem(x));
            }
            
        }

        public string Caption
        {
            get
            {
                if (profileInfo != null)
                {
                    return string.Format(ApplicationStrings.usrInvitationsView_Caption, profileInfo.Invitations.Count);
                }
                return string.Format(ApplicationStrings.usrInvitationsView_Caption, 0);
            }
        }

        public Image SmallImage
        {
            get { return Icons.Invitation; }
        }

        public bool UpdateGui(ProfileInformationDTO user)
        {
            return user != null && user.User.IsMe();
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

    public class InvitationListHost:ElementHost,IUserDetailControl
    {
        public InvitationListControl InvitationList
        {
            get { return (InvitationListControl) Child; }
        }
        public void Fill(ProfileInformationDTO user, bool isActive)
        {
            InvitationList.Fill(user,isActive);
        }

        public string Caption
        {
            get { return InvitationList.Caption; }
        }

        public Image SmallImage
        {
            get { return InvitationList.SmallImage; }
        }

        public bool UpdateGui(ProfileInformationDTO user)
        {
            return InvitationList.UpdateGui(user);
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
                        return ApplicationStrings.usrUserInvitationEntry_YouInvitedThisUser_Msg;
                    }

                }
                else
                {
                    if (invitation.InvitationType == InvitationType.Invite)
                    {
                        return ApplicationStrings.usrUserInvitationEntry_YouAreInvitedByUser_Msg;
                    }
                }
                return null;
            }
        }

        public string OperationAcceptToolTip
        {
            get { return ApplicationStrings.usrUserInvitationEntry_AcceptInvitation_ToolTip; }
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
                        return ApplicationStrings.usrUserInvitationEntry_CancelInvitation_ToolTip;
                    }

                }
                else
                {
                    if (invitation.InvitationType == InvitationType.Invite)
                    {
                        return ApplicationStrings.usrUserInvitationEntry_RejectInvitation_ToolTIp;
                    }
                }
                return string.Empty;
            }
        }

        public string Message
        {
            get { return !string.IsNullOrWhiteSpace(invitation.Message) ? string.Format("„{0}”", invitation.Message) : ApplicationStrings.usrUserInvitationEntry_NoMessageText; }
        }

        public string Country
        {
            get { return Service.Model.Country.GetCountry(User.CountryId).DisplayName; }
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
