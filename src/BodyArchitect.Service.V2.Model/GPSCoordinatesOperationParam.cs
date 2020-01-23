using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Portable;

namespace BodyArchitect.Service.V2.Model
{
    public enum GPSCoordinatesOperationType
    {
        UpdateCoordinates,
        UpdateCoordinatesWithCorrection,
        DeleteCoordinates
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    public class GPSCoordinatesOperationParam
    {
        public GPSCoordinatesOperationParam()
        {
            Coordinates = new List<GPSPoint>();
        }

        [DataMember]
        public IList<GPSPoint> Coordinates { get; set; }

        [DataMember]
        public Guid GPSTrackerEntryId { get; set; }

        [DataMember]
        public GPSCoordinatesOperationType Operation { get; set; }
    }
}
