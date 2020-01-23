using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Portable.Exceptions
{
    public class AuthenticationException:Exception
    {
        public AuthenticationException() { }
        public AuthenticationException(string message) : base(message) { }
        public AuthenticationException(string message, Exception inner) : base(message, inner) { }
    }
}
