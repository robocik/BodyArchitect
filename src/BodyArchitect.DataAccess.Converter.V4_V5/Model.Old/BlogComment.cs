using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model.Old
{
    public enum ContentType
    {
        Text,
        Html,
        Rtf
    }

    public class BlogComment:FMObject
    {
        public virtual Profile Profile { get; set; }

        public virtual string Comment { get; set; }

        public virtual DateTime DateTime { get; set; }

        public virtual ContentType CommentType { get; set; }

        public virtual BlogEntry BlogEntry { get; set; }
    }
}
