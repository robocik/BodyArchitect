using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Shared
{
    public class ObjectIsNotFavoriteException : RethrowedException
    {
        public ObjectIsNotFavoriteException() { }
        public ObjectIsNotFavoriteException(string message) : base(message) { }
        public ObjectIsNotFavoriteException(string message, Exception inner) : base(message, inner) { }
    }
}
