using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model.Old
{
    public enum FriendInvitationType
    {
        Invitation,
        RejectInvitation,
        RejectFriendship
    }
    public class FriendInvitation
    {

        public FriendInvitationType InvitationType { get; set; }
        public Profile Inviter { get; set; }
        public Profile Invited { get; set; }
        public string Message { get; set; }
        public DateTime CreateDate { get; set; }

        public override bool Equals(object obj)
        {
            if(obj==null || !(obj is FriendInvitation))
            {
                return false;
            }
            var inv = (FriendInvitation) obj;
            return Invited.Id == inv.Invited.Id && Inviter.Id == inv.Inviter.Id;
        }

        public override int GetHashCode()
        {
            return (Inviter.Id.ToString() + Invited.Id.ToString()).GetHashCode();
        }
    }
}
