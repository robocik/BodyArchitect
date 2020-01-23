using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.V2.Model.Validators
{
    public class EMailValidator : RegexValidator
    {
        public const string EMailPattern = @"^$|\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";

        public EMailValidator()
            : this(null, false)
        {
            
        }


        public EMailValidator(bool negated)
            : this(null, negated)
        { }

        public EMailValidator(string messageTemplate)
            : this(messageTemplate, false)
        { }

        public EMailValidator(string messageTemplate, bool negated)
            : base(EMailPattern,messageTemplate,  negated)
        {
        }

        /// <summary>
        /// Gets the Default Message Template when the PersonIDValidator validator is non negated.
        /// </summary>
        protected override string DefaultNonNegatedMessageTemplate
        {
            get { return "Invalid email address"; }
        }

        /// <summary>
        /// Gets the Default Message Template when the PersonIDValidator validator is negated.
        /// </summary>
        protected override string DefaultNegatedMessageTemplate
        {
            get { return "Invalid email address"; }
        }


    }
}
