using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF;
using ValidationResult = BodyArchitect.Portable.Exceptions.ValidationResult;

namespace BodyArchitect.Client.WCF
{
    public static class ValidationResultHelper
    {
        public static IList<ValidationResult> ToValidationResults(this ValidationFault fault)
        {
            List<ValidationResult> result = new List<ValidationResult>();
            foreach (ValidationDetail detail in fault.Details)
            {
                result.Add(new ValidationResult(detail.Message, detail.Key, detail.Tag));
            }
            return result;
        }

        public static IList<ValidationResult> ToBAResults(this ValidationResults results)
        {
            List<ValidationResult> result = new List<ValidationResult>();
            foreach (var detail in results)
            {
                result.Add(new ValidationResult(detail.Message, detail.Key, detail.Tag));
            }
            return result;
        }
    }
}
