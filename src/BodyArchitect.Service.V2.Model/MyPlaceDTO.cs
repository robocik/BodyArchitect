using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model
{
    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class MyPlaceDTO : MyPlaceLightDTO
    {
        [DataMember]
        public AddressDTO Address
        {
            get;
            set;
        }
        
    }
}
