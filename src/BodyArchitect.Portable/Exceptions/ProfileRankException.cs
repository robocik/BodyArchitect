using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Shared
{
    public class ProfileRankException : RethrowedException
    {
        public ProfileRankException() { }
        public ProfileRankException(string message) : base(message) { }
        public ProfileRankException(string message, Exception inner) : base(message, inner) { }
    }
}
