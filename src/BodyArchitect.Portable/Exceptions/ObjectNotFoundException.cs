using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Shared
{
    public class ObjectNotFoundException : RethrowedException
    {
        public ObjectNotFoundException() { }
        public ObjectNotFoundException(string message) : base(message) { }
        public ObjectNotFoundException(string message, Exception inner) : base(message, inner) { }
    }
}
