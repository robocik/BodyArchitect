using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model
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
        Other,
        Metro
    }

    public class LoginData:FMGlobalObject
    {
        public virtual Guid ClientInstanceId { get; set; }

        public virtual Guid ProfileId { get; set; }

        public virtual PlatformType Platform { get; set; }

        public virtual string PlatformVersion { get; set; }

        public virtual string ApplicationLanguage { get; set; }

        public virtual string ApplicationVersion { get; set; }

        public virtual AccountType AccountType { get; set; }
        
        public virtual APIKey ApiKey { get; set; }

        /// <summary>
        /// UTC TIME
        /// </summary>
        public virtual DateTime LoginDateTime { get; set; }
    }
}
