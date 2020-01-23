using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Shared
{
    public class UserDeletedException : RethrowedException
    {
        public UserDeletedException() { }
        public UserDeletedException(string message) : base(message) { }
        public UserDeletedException(string message, Exception inner) : base(message, inner) { }
    }
}
