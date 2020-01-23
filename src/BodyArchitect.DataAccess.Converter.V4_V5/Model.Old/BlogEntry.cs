using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model.Old
{
    public class BlogEntry : EntryObject
    {
        public BlogEntry()
        {
            Comments=new List<BlogComment>();
        }
        public static readonly Guid EntryTypeId = new Guid("4EC0C265-4B2E-473A-AD4C-4B482152B627");

        public virtual int BlogCommentsCount { get; private set; }

        public virtual DateTime? LastCommentDate { get; set; }

        public virtual bool AllowComments { get; set; }

        public virtual IList<BlogComment> Comments { get; private set; }
        public override Guid TypeId
        {
            get { return EntryTypeId; }
        }
    }
}
