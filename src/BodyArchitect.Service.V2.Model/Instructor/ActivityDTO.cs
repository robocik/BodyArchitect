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
    [DataContract(Namespace = Const.ServiceNamespace)]
    [Serializable]
    public class ActivityDTO : BAGlobalObject, IHasName
    {
        public ActivityDTO()
        {
            Duration = TimeSpan.FromHours(1);
        }

        [DataMember]
        [ColorValidator]
        public string Color { get; set; }

        [DataMember]
        [RangeAttribute(0, 10000)]
        public int MaxPersons { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceName = "ActivityDTO_Name_Required", ErrorMessageResourceType = typeof(ValidationStrings))]
        [StringLengthValidator(Constants.NameColumnLength, MessageTemplateResourceName = "ActivityDTO_Name_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        public string Name { get; set; }

        [DataMember]
        public Guid ProfileId { get; set; }

        //UTC
        [DataMember]
        public DateTime CreationDate { get; set; }

        [DataMember]
        [DoNotChecksum]
        [SerializerId]
        public int Version { get; set; }

        [DataMember]
        public TimeSpan Duration { get; set; }

        [DataMember]
        public decimal Price { get; set; }
    }
}
