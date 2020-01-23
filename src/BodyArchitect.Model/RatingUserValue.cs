using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model
{
    public class RatingUserValue:FMGlobalObject
    {
        public virtual Guid RatedObjectId { get; set; }
        public virtual Guid ProfileId { get; set; }
        public virtual float Rating { get; set; }
        public virtual string ShortComment { get; set; }
        public virtual DateTime VotedDate { get; set; }
        public virtual LoginData LoginData { get; set; }
    }
}
