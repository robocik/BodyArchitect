using System;

namespace BodyArchitect.Shared
{
    public class LicenceException : RethrowedException
    {
        public LicenceException() { }
        public LicenceException(string message) : base(message) { }
        public LicenceException(string message, Exception inner) : base(message, inner) { }
    }
}
