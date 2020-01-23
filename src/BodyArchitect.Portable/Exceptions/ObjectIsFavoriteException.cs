using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Shared
{
    public class ObjectIsFavoriteException : RethrowedException
    {
        public ObjectIsFavoriteException() { }
        public ObjectIsFavoriteException(string message) : base(message) { }
        public ObjectIsFavoriteException(string message, Exception inner) : base(message, inner) { }
    }
}
