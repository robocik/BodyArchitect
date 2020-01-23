using System;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
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
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Service.V2.Model
{
    public partial class UserDTO
    {
        public bool HaveAccess(Privacy privacy)
        {
            if (privacy == BodyArchitect.Service.V2.Model.Privacy.Public || (privacy == BodyArchitect.Service.V2.Model.Privacy.FriendsOnly && IsFriend))
            {
                return true;
            }
            return false;
        }

        public bool IsFriend
        {
            get
            {
                return ApplicationState.Current.ProfileInfo.Friends.Where(x => x.GlobalId == GlobalId).Count() > 0;
            }
        }

        public bool IsInvited
        {
            get { return ApplicationState.Current.ProfileInfo.Invitations.Where(x => (x.Invited != null && x.Invited.GlobalId == GlobalId) || (x.Inviter != null && x.Inviter.GlobalId == GlobalId)).Count() > 0; }
        }

        public bool IsFavorite
        {
            get
            {
                return ApplicationState.Current.ProfileInfo.FavoriteUsers.Where(x => x.GlobalId == GlobalId).Count() > 0;
            }
        }

        public bool IsMe
        {
            get { return ApplicationState.Current != null && ApplicationState.Current.SessionData != null && ApplicationState.Current.SessionData.Profile.GlobalId == GlobalId; }
        }

        private ProfileInformationDTO _profileInfo;

        [DataMember]
        public ProfileInformationDTO ProfileInfo
        {
            get { return _profileInfo; }
            set { _profileInfo = value; }
        }
    }
}
