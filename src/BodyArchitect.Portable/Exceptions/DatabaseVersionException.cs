﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Shared
{
    public class DatabaseVersionException: RethrowedException
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public DatabaseVersionException() { }
        public DatabaseVersionException(string message) : base(message) { }
        public DatabaseVersionException(string message, Exception inner) : base(message, inner) { }
    }
}
