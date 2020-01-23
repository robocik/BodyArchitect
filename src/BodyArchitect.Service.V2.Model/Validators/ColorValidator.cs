using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.V2.Model.Validators
{
    [AttributeUsage(AttributeTargets.Property
        | AttributeTargets.Field
        | AttributeTargets.Method
        | AttributeTargets.Parameter,
        AllowMultiple = true,
        Inherited = false)]
    public sealed class ColorValidatorAttribute : ValueValidatorAttribute
    {
        protected override Validator DoCreateValidator(Type targetType)
        {
            return new ColorValidator(MessageTemplate, Negated);
        }
    }

    public class ColorValidator : ValueValidator
    {
        public ColorValidator()
            : this( null, false)
        { }


        public ColorValidator( bool negated)
            : this(null, negated)
        { }

        public ColorValidator( string messageTemplate)
            : this(messageTemplate, false)
        { }


        public ColorValidator(string messageTemplate, bool negated)
            : base(messageTemplate, null, negated)
        {
        }

        public override void DoValidate(object objectToValidate, object currentTarget, string key, ValidationResults validationResults)
        {
            bool isValid = false;
            if (objectToValidate != null)
            {
                // Get the object to validate
                var color = (string)objectToValidate;

                if (string.IsNullOrEmpty(color))
                {
                    isValid = true;
                }
                else
                {
                    try
                    {
                        color.FromColorString();
                        isValid = true;
                    }
                    catch (Exception)
                    {
                        isValid = false;
                    }
                    
                }
                
            }
            // If the negated property is false, and the id is not valid, log a validation error
            if (isValid == Negated)
            {
                LogValidationResult(validationResults, this.MessageTemplate, currentTarget, key);
            }
        }

        protected override string DefaultNonNegatedMessageTemplate
        {
            get { return "Invalid non color"; }
        }

        protected override string DefaultNegatedMessageTemplate
        {
            get { return "Invalid color"; }
        }
    }
}
