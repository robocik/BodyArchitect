using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model.Old
{
    public class RatingUserValue:FMObject
    {
        public virtual Guid RatedObjectId { get; set; }
        public virtual int ProfileId { get; set; }
        public virtual float Rating { get; set; }
        public virtual string ShortComment { get; set; }
        public virtual DateTime VotedDate { get; set; }
    }
}
