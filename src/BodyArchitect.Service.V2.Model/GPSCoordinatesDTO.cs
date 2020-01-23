using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BodyArchitect.Service.V2.Model
{
    [MessageContract]
    public class GPSCoordinatesDTO
    {
        [MessageBodyMember]
        public Stream Stream { get; set; }
    }

    [MessageContract]
    public class GetGPSCoordinatesParam:IToken
    {
        [MessageHeader(MustUnderstand = true)]
        public Guid SessionId
        {
            get;
            set;
        }

        [MessageHeader(MustUnderstand = true)]
        public Guid GPSTrackerEntryId { get; set; }
    }
}
