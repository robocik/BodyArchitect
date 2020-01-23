using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model
{
    public class APIKey
    {
        public virtual Guid ApiKey { get; set; }

        public virtual string ApplicationName { get; set; }

        public virtual string EMail { get; set; }

        public virtual PlatformType Platform { get; set; }

        //UTC
        public virtual DateTime RegisterDateTime { get; set; }
    }
}
