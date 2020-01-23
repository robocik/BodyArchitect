using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Shared
{
    public class ProfileDeletedException : UserDeletedException
    {
        public ProfileDeletedException() { }
        public ProfileDeletedException(string message) : base(message) { }
        public ProfileDeletedException(string message, Exception inner) : base(message, inner) { }
    }
}
