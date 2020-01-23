using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Shared
{
    public class OldDataException:Exception
    {
        public OldDataException() { }
        public OldDataException(string message) : base(message) { }
        public OldDataException(string message, Exception inner) : base(message, inner) { }
    }
}
