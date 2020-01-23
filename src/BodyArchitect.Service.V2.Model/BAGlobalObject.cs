using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Shared;

namespace BodyArchitect.Service.V2.Model
{
    [DataContract(Namespace = Const.ServiceNamespace, IsReference = true)]
    [Serializable]
    [DebuggerDisplay("Id = {GlobalId}")]
    public class BAGlobalObject : IExtensibleDataObject
    {

        public override bool Equals(object obj)
        {
            var ex = obj as BAGlobalObject;
            if (((object)ex) == null)
            {
                return false;
            }
            if (ex.GlobalId == Guid.Empty)
            {
                return object.ReferenceEquals(ex, this);
            }
            return ex.GlobalId == GlobalId;
        }

        public override int GetHashCode()
        {
            return GlobalId.GetHashCode();
        }

        public static bool operator ==(BAGlobalObject a, BAGlobalObject b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }
            // Return true if the fields match:
            return a.Equals(b);
        }

        public static bool operator !=(BAGlobalObject a, BAGlobalObject b)
        {
            return !(a == b);
        }
        [NonSerialized]
        [DoNotSerialize]
        [DoNotChecksum]
        private object _tag;

        [DataMember]
        [SerializerId]
        [DoNotChecksum]
        [NotCloneable]
        public Guid GlobalId
        {
            get;
            set;
        }

        public bool IsNew
        {
            get
            {
                return GlobalId == Constants.UnsavedGlobalId;
            }
        }

        [DoNotSerialize]
        [DoNotChecksum]
        [NotCloneable]
        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        [NonSerialized]
        private ExtensionDataObject _extensionData;

        [DoNotSerialize]
        [NotCloneable]
        public ExtensionDataObject ExtensionData
        {
            get { return _extensionData; }
            set { _extensionData = value; }
        }
    }
}
