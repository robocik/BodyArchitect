using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model
{
    public enum FriendInvitationType
    {
        Invite,
        RejectInvitation,
        RejectFriendship
    }
    public class FriendInvitation
    {
        public virtual FriendInvitationType InvitationType { get; set; }
        public virtual Profile Inviter { get; set; }
        public virtual Profile Invited { get; set; }
        public virtual string Message { get; set; }
        public virtual DateTime CreateDate { get; set; }

        public override bool Equals(object obj)
        {
            if(obj==null || !(obj is FriendInvitation))
            {
                return false;
            }
            var inv = (FriendInvitation) obj;
            return Invited.GlobalId == inv.Invited.GlobalId && Inviter.GlobalId == inv.Inviter.GlobalId;
        }

        public override int GetHashCode()
        {
            return (Inviter.GlobalId.ToString() + Invited.GlobalId).GetHashCode();
        }
    }
}
