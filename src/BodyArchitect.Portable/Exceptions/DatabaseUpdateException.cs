using System;

namespace BodyArchitect.Shared
{
    public class DatabaseUpdateException: RethrowedException
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public DatabaseUpdateException() { }
        public DatabaseUpdateException(string message) : base(message) { }
        public DatabaseUpdateException(string message, Exception inner) : base(message, inner) { }
    }
}
