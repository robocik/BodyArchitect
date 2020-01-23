using System;
using System.Collections.Generic;


namespace BodyArchitect.Model.Old
{
    public class DatabaseVersion:FMObject
    {
        protected string version;

        public DatabaseVersion(string version)
        {
            this.version = version;
        }

        public DatabaseVersion()
        {
        }

        
        public Version Version
        {
            get
            {
                if (string.IsNullOrEmpty(version))
                {
                    return new Version();
                }
                return new Version(version);
            }
            set
            {
                version = value.ToString();
            }
        }

        public override string ToString()
        {
            return version;
        }
    }
}
