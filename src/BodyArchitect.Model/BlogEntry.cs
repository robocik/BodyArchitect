using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model
{
    public class BlogEntry : EntryObject
    {
        public static readonly Guid EntryTypeId = new Guid("4EC0C265-4B2E-473A-AD4C-4B482152B627");

        

        public override Guid TypeId
        {
            get { return EntryTypeId; }
        }
    }
}
