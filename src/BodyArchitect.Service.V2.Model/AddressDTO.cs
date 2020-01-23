using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Service.V2.Model.Localization;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.V2.Model
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    [Serializable]
    public class AddressDTO : BAGlobalObject
    {
        [DataMember]
        [StringLengthValidator(Constants.NameColumnLength, MessageTemplateResourceName = "AddressDTO_Country_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        public string Country { get; set; }

        [DataMember]
        [StringLengthValidator(10, MessageTemplateResourceName = "AddressDTO_PostalCode_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        public string PostalCode { get; set; }

        [DataMember]
        [StringLengthValidator(Constants.NameColumnLength, MessageTemplateResourceName = "AddressDTO_City_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        public string City { get; set; }

        [DataMember]
        [StringLengthValidator(200, MessageTemplateResourceName = "AddressDTO_Address1_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        public string Address1 { get; set; }

        [DataMember]
        [StringLengthValidator(200, MessageTemplateResourceName = "AddressDTO_Address2_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        public string Address2 { get; set; }
    }
}
