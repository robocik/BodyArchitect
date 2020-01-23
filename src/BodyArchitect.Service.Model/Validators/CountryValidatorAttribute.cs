using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.Model.Validators
{
    [AttributeUsage(AttributeTargets.Property
        | AttributeTargets.Field
        | AttributeTargets.Method
        | AttributeTargets.Parameter,
        AllowMultiple = true,
        Inherited = false)]
    public sealed class CountryValidatorAttribute : ValueValidatorAttribute
    {
        /// <summary>
        /// Creates the <see cref="PersonIDValidator"/> described by the attribute object.
        /// </summary>
        /// <param name="targetType">The type of object that will be validated by the validator.</param>
        /// <remarks>This operation must be overriden by subclasses.</remarks>
        /// <returns>The created <see cref="PersonIDValidator"/>.</returns>
        protected override Validator DoCreateValidator(Type targetType)
        {
            return new CountryValidator(MessageTemplate, Negated);
        }
    }
}
