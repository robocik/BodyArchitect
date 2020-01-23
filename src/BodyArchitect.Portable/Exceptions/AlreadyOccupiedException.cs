using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Shared
{
    public class AlreadyOccupiedException : RethrowedException
    {
        public AlreadyOccupiedException() { }
        public AlreadyOccupiedException(string message) : base(message) { }
        public AlreadyOccupiedException(string message, Exception inner) : base(message, inner) { }
    }
}
