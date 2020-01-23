using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model
{
    

    [Serializable]
    public class Activity : FMGlobalObject
    {
        public Activity()
        {
            CreationDate = DateTime.UtcNow;
        }

        public virtual string Color { get; set; }

        public virtual string Name { get; set; }

        public virtual Profile Profile { get; set; }

        //UTC
        public virtual DateTime CreationDate { get; set; }

        public virtual int Version { get; set; }

        public virtual int MaxPersons { get; set; }

        public virtual TimeSpan Duration { get; set; }

        public virtual decimal Price { get; set; }
    }
}
