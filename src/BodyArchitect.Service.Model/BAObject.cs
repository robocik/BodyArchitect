using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using BodyArchitect.Shared;

namespace BodyArchitect.Service.Model
{
    [DataContract(Namespace = Const.ServiceNamespace,IsReference = true)]
    [Serializable]
    public class BAObject
    {
        private int id;
        [NonSerialized]
        private object _tag;

        [DataMember]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public bool IsNew
        {
            get
            {
                return id == Constants.UnsavedObjectId;
            }
        }

        
        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }
    }
}
