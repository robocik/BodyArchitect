using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using Orchard.BodyArchitect.WCF;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Logging;
using Orchard.Mvc;
using Orchard.Security;
using Orchard.Services;
using Orchard.Users.Models;

namespace Orchand.BodyArchitect
{
    [OrchardSuppressDependency("Orchard.Security.Providers.FormsAuthenticationService")]
    public class BodyArchitectAuthenticationService: IAuthenticationService {
        private readonly IClock _clock;
        private readonly IContentManager _contentManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IUser _signedInUser;
        private bool _isAuthenticated = false;

        public BodyArchitectAuthenticationService(IClock clock, IContentManager contentManager, IHttpContextAccessor httpContextAccessor)
        {
            _clock = clock;
            _contentManager = contentManager;
            _httpContextAccessor = httpContextAccessor;

            Logger = NullLogger.Instance;
            
            ExpirationTimeSpan = TimeSpan.FromDays(30);
        }

        public ILogger Logger { get; set; }

        public TimeSpan ExpirationTimeSpan { get; set; }

        public void SignIn(IUser user, bool createPersistentCookie) {
            var now = _clock.UtcNow.ToLocalTime();
            var userData = Convert.ToString(user.Id);
            var ticket = new FormsAuthenticationTicket(
                1 /*version*/,
                user.UserName,
                now,
                now.Add(ExpirationTimeSpan),
                createPersistentCookie,
                userData,
                FormsAuthentication.FormsCookiePath);

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            cookie.HttpOnly = true;
            cookie.Secure = FormsAuthentication.RequireSSL;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            if (FormsAuthentication.CookieDomain != null) {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }

            if (createPersistentCookie) {
                cookie.Expires = ticket.Expiration;
            }

            var httpContext = _httpContextAccessor.Current();
            httpContext.Response.Cookies.Add(cookie);

            _isAuthenticated = true;
            _signedInUser = user;
        }

        public void SignOut() {
            _signedInUser = null;
            _isAuthenticated = false;
            FormsAuthentication.SignOut();
            var httpContext = _httpContextAccessor.Current();
            if (httpContext.Session["SessionData"] != null) {
                var token = ((SessionData)httpContext.Session["SessionData"]).Token;
                httpContext.Session["SessionData"] = null;
                //TODO:This can be run in separate thread
                BodyArchitectAccessServiceClient client = new BodyArchitectAccessServiceClient("Full");
                client.Logout(token);
            }
        }

        public void SetAuthenticatedUserForRequest(IUser user) {
            _signedInUser = user;
            _isAuthenticated = true;
        }

        public IUser GetAuthenticatedUser() {
            if (_signedInUser != null || _isAuthenticated)
                return _signedInUser;

            var httpContext = _httpContextAccessor.Current();
            if (httpContext == null || !httpContext.Request.IsAuthenticated || !(httpContext.User.Identity is FormsIdentity)) {
                return null;
            }

            
            var formsIdentity = (FormsIdentity)httpContext.User.Identity;
            var userData = formsIdentity.Ticket.UserData;

            int userId;
            if (!int.TryParse(userData, out userId))
            {
                Logger.Fatal("User id not a parsable integer");
                return null;
            }

            _isAuthenticated = true;
            _signedInUser = _contentManager.Get(userId).As<IUser>();
            var userPart = (UserPart)_signedInUser;
            if (_signedInUser == null)
            {
                _signedInUser = null;
                _isAuthenticated = false;
                return null;
            }
            
            if (httpContext.Session["SessionData"] == null)
            {
                if (string.IsNullOrEmpty(userPart.Record.Password)) {
                    _signedInUser = null;
                    _isAuthenticated = false;
                    return null;
                }
                try {
                    BodyArchitectAccessServiceClient client = new BodyArchitectAccessServiceClient("Full");
                    ClientInformation clientInfo = new ClientInformation();
                    clientInfo.ApplicationLanguage = "pl";
                    clientInfo.ApplicationVersion = "5.0.0.0";
                    clientInfo.ClientInstanceId = Guid.NewGuid();
                    clientInfo.Platform = PlatformType.Web;
                    clientInfo.PlatformVersion = "temp";
                    clientInfo.Version = "4.5.0.0";
                    var sessionData = client.Login(clientInfo, _signedInUser.UserName, userPart.Record.Password);
                    if (sessionData == null)
                    {
                        _signedInUser = null;
                        _isAuthenticated = false;
                        userPart.Record.Password = null;
                        return null;
                    }
                    var profileInfo = client.GetProfileInformation(sessionData.Token, new GetProfileInformationCriteria());
                    httpContext.Session["ProfileInformation"] = profileInfo;
                    httpContext.Session["SessionData"] = sessionData;
                }
                catch (Exception ex)
                {
                    _signedInUser = null;
                    _isAuthenticated = false;
                    userPart.Record.Password = null;
                    Logger.Fatal("Cannot login to the BodyArchitectService",ex);
                    return null;
                }
                
            }

            //string tokenId = splitedUserData.Length > 1 ? splitedUserData[1] : Guid.NewGuid().ToString();
            var baUser = new BAUser(userPart, ((SessionData)httpContext.Session["SessionData"]).Token);
            return baUser;
        }
    }
}
