using System;
using BodyArchitect.Shared;


namespace BodyArchitect.Model.Old
{
    [Serializable]
    public abstract class FMObject
    {

        virtual public int Id
        {
            get; set;
        }

        virtual public bool IsNew
        {
            get
            {
                return Id == Constants.UnsavedObjectId;
            }
        }
        public override bool Equals(object obj)
        {
            if (obj == null || Id==0)
            {
                return false;
            }
            FMObject o = (FMObject) obj;
            return o.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
