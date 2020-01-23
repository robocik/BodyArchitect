using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Shared
{
    public class CannotAcceptRejectInvitationDoesntExistException : RethrowedException
    {
        public CannotAcceptRejectInvitationDoesntExistException() { }
        public CannotAcceptRejectInvitationDoesntExistException(string message) : base(message) { }
        public CannotAcceptRejectInvitationDoesntExistException(string message, Exception inner) : base(message, inner) { }
    }
}
