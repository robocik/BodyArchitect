using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using BodyArchitect.Shared;

namespace BodyArchitect.Model
{
    [Serializable]
    [DebuggerDisplay("Id = {GlobalId}")]
    public class FMGlobalObject
    {

        virtual public Guid GlobalId
        {
            get;
            set;
        }

        virtual public bool IsNew
        {
            get
            {
                return GlobalId == Constants.UnsavedGlobalId;
            }
        }

        public override bool Equals(object obj)
        {
            var ex = obj as FMGlobalObject;
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
    }
}
