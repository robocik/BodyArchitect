using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.V2.Model.Instructor
{
    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace, IsReference = true)]
    [KnownType(typeof(ScheduleEntryReservationDTO))]
    public class ProductDTO:BAGlobalObject
    {

        [DataMember]
        public Guid CustomerId { get; set; }

        [DataMember]
        public bool IsPaid { get; set; }

        [DataMember]
        public decimal Price { get; set; }

        [DataMember]
        [Required]
        [StringLengthValidator(Constants.NameColumnLength)]
        public string Name { get; set; }

        [DataMember]
        public int Version { get; set; }

        //UTC
        [DataMember]
        public DateTime DateTime { get; set; }
    }
}
