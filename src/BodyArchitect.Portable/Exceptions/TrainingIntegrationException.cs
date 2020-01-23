using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Shared
{
    public class TrainingIntegrationException : RethrowedException
    {
        public TrainingIntegrationException() { }

        public TrainingIntegrationException(string message) : base(message) { }
        public TrainingIntegrationException(string message, Exception inner) : base(message, inner) { }

    }
}
