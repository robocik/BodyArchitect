using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;

namespace BodyArchitect.WP7.ViewModel
{
    public class InvitationItemViewModel:ViewModelBase
    {
        private FriendInvitationDTO invitation;

        public InvitationItemViewModel(FriendInvitationDTO invitation)
        {
            this.invitation = invitation;
        }

        public PictureInfoDTO Picture
        {
            get
            {
                var pic = invitation.Inviter ?? invitation.Invited;
                return pic.Picture ?? PictureInfoDTO.Empty;
            }
        }

        public string UserName
        {
            get
            {
                return User.UserName;
            }
        }

        public UserDTO User
        {
            get
            {
                var pic = invitation.Inviter ?? invitation.Invited;
                return pic;
            }
        }

        public bool ButtonOkVisible
        {
            get
            {
                bool visible = invitation != null && invitation.Inviter != null && invitation.InvitationType == InvitationType.Invite;
                return visible;
            }
        }

        public bool ButtonCancelVisible
        {
            get
            {
                bool visible = invitation != null && (invitation.Inviter == null || invitation.InvitationType == InvitationType.Invite || invitation.InvitationType == InvitationType.RejectFriendship);
                return visible;
            }
        }

        public string OperationMessage
        {
            get
            {
                if (invitation.Invited != null)
                {
                    if (invitation.InvitationType == InvitationType.Invite)
                    {
                        return ApplicationStrings.InvitationItemViewModel_YouInvitedThisUser_Msg;
                    }

                }
                else
                {
                    if (invitation.InvitationType == InvitationType.Invite)
                    {
                        return ApplicationStrings.InvitationItemViewModel_YouAreInvitedByUser_Msg;
                    }
                }
                return null;
            }
        }

        public string Message
        {
            get
            {
                if (!string.IsNullOrEmpty(invitation.Message))
                {
                    return string.Format("„{0}”", invitation.Message);
                }
                return ApplicationStrings.InvitationItemViewModel_NoMessage;
            }
        }

        public DateTime CreatedDateTime
        {
            get { return invitation.CreatedDateTime.ToLocalTime(); }
        }

        public FriendInvitationDTO Invitation
        {
            get { return invitation; }
        }
    }
}
