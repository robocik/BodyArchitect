using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Settings;

namespace BodyArchitect.Client.WCF
{
    public enum LoginStatus
    {
        NotLogged,
        InProgress,
        Logged,
        LoginFailed
    }
    public class LoginStatusEventArgs : EventArgs
    {
        private LoginStatus status;

        public LoginStatusEventArgs(LoginStatus status)
        {
            this.status = status;
        }

        public LoginStatus Status
        {
            get { return status; }
        }
    }
    //[DebuggerStepThrough]
    public static class UserContext
    {
        private static ApplicationSettings settings = new ApplicationSettings();
        private static SessionData sessionData;
        public static event EventHandler<LoginStatusEventArgs> LoginStatusChanged;
        public static event EventHandler ProfileInformationChanged;

        private static LoginStatus loginStatus;
        private static ProfileInformationDTO profileInfo;

        static UserContext()
        {
            ServiceManager.LoginRequired += new EventHandler<ServiceManager.SessionDataChangedEventArgs>(ServiceManager_LoginRequired);
        }

        static string tempUserName;
        private static string tempPassword;

        static void ServiceManager_LoginRequired(object sender, ServiceManager.SessionDataChangedEventArgs e)
        {
            var data = loginImplementation(tempUserName, tempPassword, false);
            e.SessionData = data;
        }
        public static bool IsMine(this IBelongToUser item)
        {
            if (LoginStatus != LoginStatus.Logged || item == null || item.Profile == null || CurrentProfile.Id != item.Profile.Id)
            {
                return false;
            }
            return true;
        }

        public static void RefreshUserData()
        {
            refreshUserDataImplementation(true);
        }

        static void refreshUserDataImplementation(bool raiseEvent)
        {
            profileInfo = ServiceManager.GetProfileInformation(new GetProfileInformationCriteria());
            SessionData.Profile.Picture = profileInfo.User.Picture;
            sessionData.Profile.Privacy = profileInfo.User.Privacy;
            sessionData.Profile.CountryId = profileInfo.User.CountryId;
            sessionData.Profile.Gender = profileInfo.User.Gender;
            sessionData.Profile.Settings = profileInfo.Settings;
            //TODO:here we must refresh the Version but UserDTO doesn't have it
            //sessionData.Profile.Version=profileInfo.User.Version;
            if (raiseEvent && ProfileInformationChanged != null)
            {
                ProfileInformationChanged(null, EventArgs.Empty);
            }
        }

        static void onLoginChanged(LoginStatus status)
        {
            if (LoginStatusChanged != null)
            {
                foreach (var del in LoginStatusChanged.GetInvocationList())
                {
                    EventHandler<LoginStatusEventArgs> handler = (EventHandler<LoginStatusEventArgs>)del;
                    handler.BeginInvoke(null, new LoginStatusEventArgs(status), null, null);
                }
                //LoginStatusChanged(null, new LoginStatusEventArgs(status));
            }
        }
        public static bool IsFavorite(this UserDTO user)
        {
            return ProfileInformation != null && user != null && ProfileInformation.FavoriteUsers.Where(x => x.Id == user.Id).Count() > 0;
        }

        public static bool IsMe(this UserDTO user)
        {
            return LoginStatus == LoginStatus.Logged && user != null && user.Id == CurrentProfile.Id;
        }

        public static bool IsInInvitation(this UserDTO user)
        {
            return LoginStatus == LoginStatus.Logged && ProfileInformation != null && user != null && ProfileInformation.Invitations.Where(x => (x.Invited != null && x.Invited.Id == user.Id) || (x.Inviter != null && x.Inviter.Id == user.Id)).Count() > 0;
        }

        public static bool IsInvited(this UserDTO user)
        {
            return LoginStatus == LoginStatus.Logged && ProfileInformation != null && user != null && ProfileInformation.Invitations.Where(x => (x.Invited != null && x.Invited.Id == user.Id)).Count() > 0;
        }

        public static bool IsInviter(this UserDTO user)
        {
            return LoginStatus == LoginStatus.Logged && ProfileInformation != null && user != null && ProfileInformation.Invitations.Where(x => (x.Inviter != null && x.Inviter.Id == user.Id)).Count() > 0;
        }

        public static bool HaveAccess(this UserDTO user, Privacy privacy)
        {
            if (privacy == Privacy.Public || (privacy == Privacy.FriendsOnly && user.IsFriend()))
            {
                return true;
            }
            return false;
        }
        public static bool IsFriend(this UserDTO user)
        {
            return LoginStatus == LoginStatus.Logged && ProfileInformation != null && user != null && ProfileInformation.Friends.Where(x => x.Id == user.Id).Count() > 0;
        }

        public static void CreateUserContext(SessionData data)
        {
            sessionData = data;
            ServiceManager.Token = sessionData.Token;
            refreshUserDataImplementation(false);
            LoginStatus = LoginStatus.Logged;

        }

        static SessionData loginImplementation(string user, string password, bool saveCredentials)
        {
            Log.WriteInfo("Login on user: {0}", user);
            var sessionData = ServiceManager.Login(user, password);
            if (sessionData == null)
            {
                Log.WriteInfo("Login failed");
                tempPassword = null;
                tempUserName = null;
                UserContext.LoginStatus = LoginStatus.LoginFailed;
                return sessionData;
            }
            else
            {
                Log.WriteInfo("Login ok");
                tempPassword = password;
                tempUserName = user;
                if (saveCredentials)
                {
                    Settings1.Default.AutoLoginUserName = user;
                    Settings1.Default.AutoLoginPassword = password;
                    Settings1.Default.Save();
                }
            }
            UserContext.CreateUserContext(sessionData);
            return sessionData;
        }
        public static SessionData Login(string user, string password, bool saveCredentials, bool passwordHashed = false)
        {
            if (LoginStatus == LoginStatus.Logged)
            {
                try
                {
                    ServiceManager.Logout(Token);
                }
                catch (Exception)
                {
                }

                profileInfo = null;
                LoginStatus = LoginStatus.InProgress;
            }
            if (!passwordHashed)
            {
                password = password.ToSHA1Hash();
            }
            return loginImplementation(user, password, saveCredentials);
        }

        public static void Logout(LoginStatus newStatus = LoginStatus.NotLogged, bool resetAutologon = true, bool skipLogoutOnServer = false)
        {
            if (!skipLogoutOnServer && LoginStatus == LoginStatus.Logged)
            {
                //save the token instance because logout is run in separate thread and this logout method set 
                //null to sessionData. Basically without this token in ServiceManager.Logout is alwas null so in 
                //most cases logout on the server is not working
                var token = Token;
                ThreadPool.QueueUserWorkItem(delegate
                                                 {
                                                     try
                                                     {
                                                         ServiceManager.Logout(token);
                                                     }
                                                     catch (Exception ex)
                                                     {
                                                         ExceptionHandler.Default.Process(ex);
                                                     }

                                                 });

            }
            if (resetAutologon)
            {
                Settings1.Default.AutoLoginUserName = null;
                Settings1.Default.AutoLoginPassword = null;
                Settings1.Default.Save();
            }

            sessionData = null;
            profileInfo = null;
            tempPassword = null;
            tempUserName = null;
            LoginStatus = newStatus;

        }
        public static bool IsFirstRun
        {
            get
            {
                if (ApplicationDeployment.IsNetworkDeployed)
                {
                    return ApplicationDeployment.CurrentDeployment.IsFirstRun;
                }
                return false;
            }
        }

        public static ProfileDTO CurrentProfile
        {
            get { return sessionData != null ? sessionData.Profile : null; }
        }

        public static ApplicationSettings Settings
        {
            get { return settings; }
        }

        public static SessionData SessionData
        {
            get { return sessionData; }
        }

        public static bool IsConnected
        {
            get { return SessionData != null && SessionData.IsConnected; }
        }

        public static LoginStatus LoginStatus
        {
            get { return loginStatus; }
            set
            {
                if (loginStatus != value)
                {
                    loginStatus = value;
                    onLoginChanged(loginStatus);
                }
            }
        }

        public static Token Token
        {
            get { return SessionData!=null?SessionData.Token:null; }
        }

        public static ProfileInformationDTO ProfileInformation
        {
            get
            {
                return profileInfo;
            }
        }
    }
}
