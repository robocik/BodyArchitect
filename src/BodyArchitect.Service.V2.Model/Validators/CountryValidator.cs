using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BodyArchitect.Service.V2.Model.Localization;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.V2.Model.Validators
{
    public class CountryValidator : ValueValidator
    {
        public CountryValidator()
            : this( null, false)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="PersonIDValidator"/>.</para>
        /// </summary>
        /// <param name="negated">True if the validator must negate the result of the validation.</param>
        public CountryValidator( bool negated)
            : this(null, negated)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="PersonIDValidator"/>.</para>
        /// </summary>
        /// <param name="coutry">Country Code to validate by it</param>
        /// <param name="messageTemplate">The message template to use when logging results.</param>
        public CountryValidator( string messageTemplate)
            : this(messageTemplate, false)
        { }

        // <summary>
        /// <para>Initializes a new instance of the <see cref="PersonIDValidator"/>.</para>
        /// </summary>
        /// <param name="messageTemplate">The message template to use when logging results.</param>
        /// <param name="negated">True if the validator must negate the result of the validation</param>
        public CountryValidator(string messageTemplate, bool negated)
            : base(messageTemplate, null, negated)
        {
        }

        /// <summary>
        /// Gets the Default Message Template when the PersonIDValidator validator is non negated.
        /// </summary>
        protected override string DefaultNonNegatedMessageTemplate
        {
            get { return "Invalid nonneg"; }
        }

        /// <summary>
        /// Gets the Default Message Template when the PersonIDValidator validator is negated.
        /// </summary>
        protected override string DefaultNegatedMessageTemplate
        {
            get { return "Invalid neg"; }
        }

        /// <summary>
        /// Implements the validation logic for the receiver.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <param name="currentTarget">The object on the behalf of which the validation is performed.</param>
        /// <param name="key">The key that identifies the source of <paramref name="objectToValidate"/>.</param>
        /// <param name="validationResults">The validation results to which the outcome of the validation should be stored.</param>
        public override void DoValidate(object objectToValidate, object currentTarget, string key, Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults validationResults)
        {
            bool isValid = false;
            if (objectToValidate != null)
            {
                // Get the object to validate
                var country = (int) objectToValidate;

                var res = Country.GetCountry(country);
                isValid = res != null;
            }
            // If the negated property is false, and the id is not valid, log a validation error
            if (isValid == Negated)
            {
                LogValidationResult(validationResults, this.MessageTemplate, currentTarget, key);
            }

        }

    }
}
