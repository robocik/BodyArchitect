using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Service.V2.Model.Validators;

namespace BodyArchitect.Service.V2.Model
{
    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace)]
    [KnownType(typeof(MyPlaceDTO))]
    public class MyPlaceLightDTO : BAGlobalObject, IHasName
    {
 
        [DataMember]
        public bool NotForRecords { get; set; }

        [DataMember]
        public bool IsSystem { get; set; }

        [DataMember]
        public bool IsDefault { get; set; }

        [DataMember]
        [Required]
        public string Name { get; set; }

        [DataMember]
        public int Version { get; set; }

        [DataMember]
        public DateTime CreationDate { get; set; }

        [DataMember]
        public Guid ProfileId { get; set; }

        [DataMember]
        [ColorValidator]
        public string Color { get; set; }
    }
}
