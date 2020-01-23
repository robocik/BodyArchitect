using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model
{
    public class IdConvertion
    {
        public virtual int IntId { get; set; }
        public virtual string Type { get; set; }
        public virtual Guid GuidId { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var t = obj as IdConvertion;
            if (t == null)
                return false;
            if (IntId == t.IntId && Type == t.Type)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            return (IntId + "|" + Type).GetHashCode();
        }  

    }
}
