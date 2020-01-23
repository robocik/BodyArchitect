using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Shared
{
    public class CrossProfileOperationException : RethrowedException
    {
        public CrossProfileOperationException() { }
        public CrossProfileOperationException(string message) : base(message) { }
        public CrossProfileOperationException(string message, Exception inner) : base(message, inner) { }
    }
}
