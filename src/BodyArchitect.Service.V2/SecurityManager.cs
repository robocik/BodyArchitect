using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.Practices.EnterpriseLibrary.Caching.Expirations;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace BodyArchitect.Service.V2
{
    public class SecurityManager:IDisposable
    {
        object syncObject = new object();
        private int sessionTimeout;
        public const string AuthenticationCacheName = "AuthenticationCache";

        
        public ICacheManager Cache
        {
            get
            {
                return EnterpriseLibraryContainer.Current.GetInstance<ICacheManager>(AuthenticationCacheName);
            }
        }


        public bool IsLogged(Guid profileId)
        {
            lock (syncObject)
            {
                Cache myCache = (Cache)Cache.GetType().GetField("realCache", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(Cache);

                foreach (var Item in myCache.CurrentCacheState.Values)
                {
                    CacheItem CacheItem = (CacheItem)Item;
                    SecurityInfo val = (SecurityInfo)CacheItem.Value;
                    if (val.SessionData.Profile.GlobalId == profileId)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        
        public int LoggedClients
        {
            get { return Cache.Count; } //loggedClients.Count; }
        }
        
        /// <summary>
        /// Timeout in seconds
        /// </summary>
        public int SessionTimeout
        {
            get
            {
                if (sessionTimeout == 0)
                {
                    if (!int.TryParse(ConfigurationManager.AppSettings["SessionTimeout"], out sessionTimeout))
                    {
                        sessionTimeout = 1200;//in seconds = 20 minut
                    }
                }
                return sessionTimeout;
            }
            set { sessionTimeout = value; }
        }

        public SessionData CreateNewSession(ProfileDTO profile, ClientInformation clientInfo,LoginData loginData)
        {
            Version version = new Version(clientInfo.Version);
            if(version!=new Version(Const.ServiceVersion))
            {
                Log.WriteError("Client version:{0}, service version:{1}",clientInfo.Version,Const.ServiceVersion);
                throw new DatabaseVersionException("You have old version of the application. Please get the latest version and try again");
            }
            
            Token token = new Token(Guid.NewGuid());
            SessionData sessionData = new SessionData(token, profile, true);
            var securityInfo = new SecurityInfo(sessionData, loginData);
            securityInfo.ClientInformation = clientInfo;
            Cache.Add(token.SessionId.ToString(), securityInfo, CacheItemPriority.NotRemovable, null, new SlidingTime(TimeSpan.FromSeconds(SessionTimeout)));

            return sessionData;
        }

        public SecurityInfo EnsureAuthentication(IToken token)
        {
            lock (syncObject)
            {
                SecurityInfo securityInfo = (SecurityInfo)Cache.GetData(token.SessionId.ToString());
                if (securityInfo==null)
                {
                    throw new Portable.Exceptions.AuthenticationException("You must login first");
                }
                var tokenClass = token as Token;
                if (tokenClass != null && !string.IsNullOrEmpty(tokenClass.Language))
                {
                    //store always the current user language
                    securityInfo.SessionData.Token.Language = tokenClass.Language;
                }
                
                Log.WriteVerbose("Authenticated request: User:{0}",securityInfo.SessionData.Profile.UserName);
                return securityInfo;
            }
        }

        public void Dispose()
        {
            Cache.Flush();
        }

        public void Remove(Token token)
        {
            lock (syncObject)
            {
                Cache.Remove(token.SessionId.ToString());
            }
        }

        internal void Remove(ProfileDTO profile)
        {//remove all logged instance for this user (used for delete profile to logout all instances)
            lock (syncObject)
            {
                Cache myCache = (Cache)Cache.GetType().GetField("realCache", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(Cache);

                List<Guid> keysToRemove=new List<Guid>();
                foreach (var Item in myCache.CurrentCacheState.Values) 
                {
                    CacheItem CacheItem = (CacheItem)Item;
                    SecurityInfo val = (SecurityInfo) CacheItem.Value;
                    if (val.SessionData.Profile.GlobalId == profile.GlobalId)
                    {
                        keysToRemove.Add(val.SessionData.Token.SessionId);
                    }
                    // do Something with them
                }
                foreach (Guid guid in keysToRemove)
                {
                    Cache.Remove(guid.ToString());
                }
            }
        }

        public IList<SecurityInfo> GetForProfile(Guid profileId)
        {
            lock (syncObject)
            {
                Cache myCache = (Cache)Cache.GetType().GetField("realCache", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(Cache);

                List<SecurityInfo> list = new List<SecurityInfo>();
                foreach (var Item in myCache.CurrentCacheState.Values)
                {
                    CacheItem CacheItem = (CacheItem)Item;
                    SecurityInfo val = (SecurityInfo)CacheItem.Value;
                    if (val.SessionData.Profile.GlobalId == profileId)
                    {
                        list.Add(val);
                    }
                    // do Something with them
                }
                return list;
            }
        }
    }
}
