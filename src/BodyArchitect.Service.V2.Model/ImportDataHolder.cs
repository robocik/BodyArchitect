using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.V2.Model
{
    [MessageContract]
    [Serializable]
    public class ImportDataStream
    {
        [MessageBodyMember(Order = 1)]
        public Stream ImageStream { get; set; }
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    [Serializable]
    public class ImportDataHolder
    {
        [DataMember]
        //[NotNullValidator]
        public string Schema { get; set; }

        [DataMember]
        //[NotNullValidator]
       public string XmlData { get; set; }
        //public DataSet DataSet { get; set; }

        [DataMember]
        [NotNullValidator]
        public ProfileDTO Profile { get; set; }

        [DataMember]
        public ClientInformation ClientInformation { get; set; }

        [DataMember]
        public int WymiaryId { get; set; }
    }
}
