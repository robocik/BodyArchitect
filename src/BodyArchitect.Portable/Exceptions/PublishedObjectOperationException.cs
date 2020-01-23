using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Shared
{
    public class PublishedObjectOperationException : Exception
    {
        public PublishedObjectOperationException() { }
        public PublishedObjectOperationException(string message) : base(message) { }
        public PublishedObjectOperationException(string message, Exception inner) : base(message, inner) { }
    }
}
