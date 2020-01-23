using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF;

namespace BodyArchitect.WCF
{
    public static class ValidationResultHelper
    {
        public static ValidationResults ToValidationResults(this ValidationFault fault)
        {
            ValidationResults result = new ValidationResults();
            foreach (ValidationDetail detail in fault.Details)
            {
                result.AddResult(new ValidationResult(detail.Message,null,detail.Key,detail.Tag,null));
            }
            return result;
        }
    }
}
