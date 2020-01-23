using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Shared
{
    public class DeleteConstraintException : RethrowedException
    {
        public DeleteConstraintException() { }
        public DeleteConstraintException(string message) : base(message) { }
        public DeleteConstraintException(string message, Exception inner) : base(message, inner) { }
    }
}
