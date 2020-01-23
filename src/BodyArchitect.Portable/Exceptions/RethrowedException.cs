using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Shared
{
    public class RethrowedException : Exception
    {
        private string originalStackTrace;

        public RethrowedException() { }
        public RethrowedException(Exception originalException):base(originalException.Message,originalException)
        {
            originalStackTrace = originalException.StackTrace;
        }
        public RethrowedException(string message) : base(message) { }
        public RethrowedException(string message, Exception inner) : base(message, inner) { }

        public string OriginalStackTrace
        {
            get { return originalStackTrace; }
            set { originalStackTrace = value; }
        }
    }
}
