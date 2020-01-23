using System;
using System.Drawing;
using System.Runtime.Serialization;
using BodyArchitect.Shared;

namespace BodyArchitect.Service.V2.Model
{
    [Serializable]
    [EntryObjectInstance(EntryObjectInstance.Single)]
    [EntryObjectOperationsAttribute(EntryObjectOperation.None)]
    [DataContract(Namespace = Const.ServiceNamespace,IsReference = true)]
    public class BlogEntryDTO : SpecificEntryObjectDTO
    {
        public override bool IsEmpty
        {
            get { return string.IsNullOrEmpty(Comment); }
        }
    }
}
