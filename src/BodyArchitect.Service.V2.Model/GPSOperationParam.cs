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
    public class GPSCoordinatesOperationResult
    {
        [MessageBodyMember]
        public GPSTrackerEntryDTO GPSTrackerEntry { get; set; }
    }

    [DataContract]
    public class GPSOperationData
    {
        [DataMember]
        public GPSCoordinatesOperationType Operation { get; set; }

        [DataMember]
        public Guid GPSTrackerEntryId { get; set; }
    }

    [MessageContract()]
    [Serializable]
    public class GPSOperationParam:IToken
    {
        public GPSOperationParam()
        {
            Params = new GPSOperationData();
        }

        [MessageHeader(MustUnderstand = true)]
        public GPSOperationData Params { get; set; }

        [MessageHeader(MustUnderstand = true)]
        public Guid SessionId{get;set;}



        [MessageBodyMember(Order = 1)]
        public Stream CoordinatesStream { get; set; }
    }
}
