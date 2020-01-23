using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Shared
{
    public class MaintenanceException : RethrowedException
    {
        public MaintenanceException() { }
        public MaintenanceException(string message) : base(message) { }
        public MaintenanceException(string message, Exception inner) : base(message, inner) { }
    }
}
