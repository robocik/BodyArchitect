using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Shared
{
    public class ProfileAlreadyFriendException : RethrowedException
    {
        public ProfileAlreadyFriendException() { }
        public ProfileAlreadyFriendException(string message) : base(message) { }
        public ProfileAlreadyFriendException(string message, Exception inner) : base(message, inner) { }
    }
}
