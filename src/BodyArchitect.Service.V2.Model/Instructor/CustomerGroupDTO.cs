using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Service.V2.Model.Localization;
using BodyArchitect.Service.V2.Model.Validators;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.V2.Model.Instructor
{
    public enum CustomerGroupRestrictedType
    {
        /// <summary>
        /// One customer can belong to many different groups
        /// </summary>
        None,
        /// <summary>
        /// One customer which belong to Partially restricted group can belong to many Not restricted group only
        /// </summary>
        Partially,
        /// <summary>
        /// One customer which belong to Fully restricted group cannot belong to any other groups
        /// </summary>
        Full
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    [Serializable]
    public class CustomerGroupDTO : BAGlobalObject, IHasName
    {
        public CustomerGroupDTO()
        {
            Customers=new List<CustomerDTO>();
        }

        [Required]
        [DataMember]
        [StringLengthValidator(Constants.NameColumnLength, MessageTemplateResourceName = "CustomerGroupDTO_Name_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        public string Name { get; set; }

        [DataMember]
        [ColorValidator]
        public string Color { get; set; }

        [DataMember]
        [NotNullValidator]
        [ObjectCollectionValidator(typeof(CustomerDTO))]
        public List<CustomerDTO> Customers { get; set; }

        [DataMember]
        [RangeAttribute(0, 10000)]
        public int MaxPersons { get; set; }

        [DataMember]
        public Guid? DefaultActivityId { get; set; }
               
        //UTC  
        [DataMember]
        public DateTime CreationDate { get; set; }

        [DataMember]
        public Guid ProfileId { get; set; }

        [DataMember]
        public CustomerGroupRestrictedType RestrictedType { get; set; }

        [DataMember]
        public int Version { get; set; }
    }
}
