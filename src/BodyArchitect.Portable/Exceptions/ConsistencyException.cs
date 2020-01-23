using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Shared
{
    public class ConsistencyException : RethrowedException
    {
        public ConsistencyException() { }
        public ConsistencyException(string message) : base(message) { }
        public ConsistencyException(string message, Exception inner) : base(message, inner) { }
    }
}
