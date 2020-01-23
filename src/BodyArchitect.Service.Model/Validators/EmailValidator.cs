using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.Model.Validators
{
    public class EMailValidator : RegexValidator
    {
        public const string EMailPattern = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";

        public EMailValidator()
            : this(null, false)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="PersonIDValidator"/>.</para>
        /// </summary>
        /// <param name="negated">True if the validator must negate the result of the validation.</param>
        public EMailValidator(bool negated)
            : this(null, negated)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="PersonIDValidator"/>.</para>
        /// </summary>
        /// <param name="coutry">Country Code to validate by it</param>
        /// <param name="messageTemplate">The message template to use when logging results.</param>
        public EMailValidator(string messageTemplate)
            : this(messageTemplate, false)
        { }

        // <summary>
        /// <para>Initializes a new instance of the <see cref="PersonIDValidator"/>.</para>
        /// </summary>
        /// <param name="messageTemplate">The message template to use when logging results.</param>
        /// <param name="negated">True if the validator must negate the result of the validation</param>
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
