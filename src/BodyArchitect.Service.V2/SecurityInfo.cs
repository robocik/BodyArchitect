using System;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Service.V2
{
    public class SecurityInfo
    {
        private SessionData sessionData;
        private DateTime lastActivityTime;
        private LicenceInfoDTO licence;

        public SecurityInfo(SessionData sessionData, LoginData loginData)
        {
            this.sessionData = sessionData;
            this.LoginData = loginData;
            this.lastActivityTime = loginData.LoginDateTime;
        }

        public DateTime LastActivityTime
        {
            get { return lastActivityTime; }
            set { lastActivityTime = value; }
        }

        public LoginData LoginData { get; private set; }


        public SessionData SessionData
        {
            get { return sessionData; }
        }

        public ClientInformation ClientInformation
        {
            get; set; }

        public LicenceInfoDTO Licence
        {
            get { return licence; }
            set { licence = value; }
        }
    }
}
