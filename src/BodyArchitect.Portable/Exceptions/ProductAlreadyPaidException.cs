using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Shared
{
    public class ProductAlreadyPaidException : RethrowedException
    {
        public ProductAlreadyPaidException() { }
        public ProductAlreadyPaidException(string message) : base(message) { }
        public ProductAlreadyPaidException(string message, Exception inner) : base(message, inner) { }
    }
}
