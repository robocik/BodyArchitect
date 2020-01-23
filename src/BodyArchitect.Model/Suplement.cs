using System;
using BodyArchitect.Shared;


namespace BodyArchitect.Model
{
    [Serializable]
    public class Suplement : FMGlobalObject, ICommentable, ISortable
    {
        public virtual Profile Profile { get; set; }

        public virtual string Name { get; set; }

        public virtual string Comment { get; set; }

        public virtual string Url { get; set; }

        public virtual float Rating
        {
            get;
            set;
        }

        //UTC
        public virtual DateTime CreationDate { get; set; }

        public virtual bool IsProduct { get; set; }

        public virtual bool CanBeIllegal { get; set; }
    }
}
