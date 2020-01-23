using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model.Old
{
    public enum PlatformType
    {
        Windows,
        MacOS,
        Android,
        iPhone,
        WindowsMobile,
        WindowsPhone,
        Linux,
        Web,
        Other
    }

    public class LoginData:FMObject
    {
        public Guid ClientInstanceId { get; set; }

        public int ProfileId { get; set; }

        public PlatformType Platform { get; set; }

        public string PlatformVersion { get; set; }

        public string ApplicationLanguage { get; set; }

        public string ApplicationVersion { get; set; }

        /// <summary>
        /// UTC TIME
        /// </summary>
        public DateTime LoginDateTime { get; set; }
    }
}
