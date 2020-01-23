using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Shared
{
    public class ProfileIsNotActivatedException : RethrowedException
    {
        public ProfileIsNotActivatedException() { }
        public ProfileIsNotActivatedException(string message) : base(message) { }
        public ProfileIsNotActivatedException(string message, Exception inner) : base(message, inner) { }
    }
}
