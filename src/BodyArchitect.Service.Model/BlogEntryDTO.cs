using System;
using System.Runtime.Serialization;
using BodyArchitect.Shared;

namespace BodyArchitect.Service.Model
{
    [Serializable]
    [EntryObjectInstance(EntryObjectInstance.Single)]
    [EntryObjectOperationsAttribute(EntryObjectOperation.None)]
    [DataContract(Namespace = Const.ServiceNamespace,IsReference = true)]
    public class BlogEntryDTO : SpecificEntryObjectDTO
    {
        public BlogEntryDTO()
        {
            AllowComments = true;
        }

        [DataMember]
        public int BlogCommentsCount { get; private set; }

        [DataMember]
        public bool AllowComments { get; set; }

        [DataMember]
        public DateTime? LastCommentDate { get; set; }

        public override bool IsEmpty
        {
            get { return !AllowComments && string.IsNullOrEmpty(Comment); }
        }
    }
}
