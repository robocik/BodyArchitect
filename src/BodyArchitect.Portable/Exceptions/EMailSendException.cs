using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Shared
{
    public class EMailSendException : RethrowedException
    {
        public EMailSendException() { }
        public EMailSendException(string message) : base(message) { }
        public EMailSendException(string message, Exception inner) : base(message, inner) { }
    }

}
