using System;

namespace BodyArchitect.Shared
{
    public class DatabaseException : RethrowedException
    {
        public DatabaseException() { }
        public DatabaseException(string message) : base(message) { }
        public DatabaseException(string message, Exception inner) : base(message, inner) { }
    }
}
