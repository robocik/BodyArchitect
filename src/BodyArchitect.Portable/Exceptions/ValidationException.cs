using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Shared;

namespace BodyArchitect.Portable.Exceptions
{
    public class ValidationResult
    {
        public ValidationResult(string message, string key, string tag)
        {
            Message = message;
            Key = key;
            Tag = tag;
        }

        public string Message { get; private set; }

        public string Key { get; private set; }

        public string Tag { get; private set; }
    }

    public class ValidationException:RethrowedException
    {
        private ICollection<ValidationResult> result;

        public ValidationException() { }
        public ValidationException(ICollection<ValidationResult> result)
        {
            this.result = result;
        }
        public ValidationException(string message) : base(message) { }
        public ValidationException(string message, Exception inner) : base(message, inner) { }

        public ICollection<ValidationResult> Results
        {
            get { return result; }
        }
    }
}
