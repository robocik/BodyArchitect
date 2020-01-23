using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model
{
    [Serializable]
    public class MyPlace:FMGlobalObject
    {
        public MyPlace()
        {
            Entries = new HashSet<StrengthTrainingEntry>();
        }

        public virtual Address Address { get; set; }

        public virtual bool IsDefault { get; set; }

        public virtual bool IsSystem { get; set; }

        public virtual bool NotForRecords { get; set; }

        public virtual string Name { get; set; }

        public virtual int Version { get; set; }

        public virtual DateTime CreationDate { get; set; }

        public virtual Profile Profile { get; set; }

        public virtual ICollection<StrengthTrainingEntry> Entries { get; set; }

        public virtual string Color { get; set; }
    }
}
