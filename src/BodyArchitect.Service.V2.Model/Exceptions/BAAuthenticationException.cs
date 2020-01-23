using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Shared;

namespace BodyArchitect.Service.V2.Model.Exceptions
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class BAAuthenticationException
    {
        public BAAuthenticationException(string message)
        {
            Message = message;
        }
        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public Guid ErrorId { get; set; }
    }
}
