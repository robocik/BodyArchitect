using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Service.V2.Model
{
    public enum GetOperation
    {
        Current,
        First,
        Previous,
        Next,
        Last
    }

    public enum FavoriteOperation
    {
        Add,
        Remove
    }

    public enum InviteFriendOperation
    {
        Invite,
        Accept,
        Reject
    }

    public enum AccountOperationType
    {
        RestorePassword,
        SendActivationEmail
    }
}
