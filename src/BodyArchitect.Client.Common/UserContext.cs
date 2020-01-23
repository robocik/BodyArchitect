using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Deployment.Application;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Settings;
using BodyArchitect.Shared;

namespace BodyArchitect.Client.Common
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
    public class UserContextData : INotifyPropertyChanged
    {
        string tempUserName;
        private string tempPassword;
        private ApplicationSettings settings = new ApplicationSettings();
        private SessionData sessionData;
        public event EventHandler<LoginStatusEventArgs> LoginStatusChanged;
        public event EventHandler ProfileInformationChanged;

        private LoginStatus loginStatus;
        private ProfileInformationDTO profileInfo;

        public event PropertyChangedEventHandler PropertyChanged;

        internal UserContextData()
        {
            ServiceManager.LoginRequired += new EventHandler<ServiceManager.SessionDataChangedEventArgs>(ServiceManager_LoginRequired);
        }

        public ProfileDTO CurrentProfile
        {
            get { return sessionData != null ? sessionData.Profile : null; }
        }

        public FeaturedData FeaturedData { get; set; }

        public ApplicationSettings Settings
        {
            get { return settings; }
        }

        public SessionData SessionData
        {
            get { return sessionData; }
            internal set { sessionData = value; }
        }

        public bool IsConnected
        {
            get { return SessionData != null && SessionData.IsConnected; }
        }

        public LoginStatus LoginStatus
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

        public Token Token
        {
            get { return SessionData != null ? SessionData.Token : null; }
        }

        public ProfileInformationDTO ProfileInformation
        {
            get
            {
                return profileInfo;
            }
        }

        void ServiceManager_LoginRequired(object sender, ServiceManager.SessionDataChangedEventArgs e)
        {
            var data = loginImplementation(tempUserName, tempPassword, false);
            e.SessionData = data;
        }

        public void RefreshUserData()
        {
            refreshUserDataImplementation(true);
        }

        internal void refreshUserDataImplementation(bool raiseEvent)
        {
            profileInfo = ServiceManager.GetProfileInformation(new GetProfileInformationCriteria());
            SessionData.Profile.Picture = profileInfo.User.Picture;
            sessionData.Profile.Privacy = profileInfo.User.Privacy;
            sessionData.Profile.CountryId = profileInfo.User.CountryId;
            sessionData.Profile.Gender = profileInfo.User.Gender;
            sessionData.Profile.Settings = profileInfo.Settings;
            //TODO:here we must refresh the Version but UserDTO doesn't have it
            sessionData.Profile.Version = profileInfo.User.Version;
            if (raiseEvent)
            {
                if (ProfileInformationChanged != null)
                {
                    ProfileInformationChanged(null, EventArgs.Empty);
                }
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ProfileInformation"));
                }
            }
        }

        SessionData loginImplementation(string user, string password, bool saveCredentials)
        {
            Log.WriteInfo("Login on user: {0}", user);
            var sessionData = ServiceManager.Login(user, password);
            if (sessionData == null)
            {
                Log.WriteInfo("Login failed");
                tempPassword = null;
                tempUserName = null;
                LoginStatus = LoginStatus.LoginFailed;
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
        public SessionData Login(string user, string password, bool saveCredentials, bool passwordHashed = false)
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

        public void Logout(LoginStatus newStatus = LoginStatus.NotLogged, bool resetAutologon = true, bool skipLogoutOnServer = false)
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
            Settings1.Default.FacebookToken = null;
            sessionData = null;
            profileInfo = null;
            tempPassword = null;
            tempUserName = null;
            LoginStatus = newStatus;
            FeaturedData = null;

        }


        void onLoginChanged(LoginStatus status)
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

        
    }
    //[DebuggerStepThrough]
    public static class UserContext
    {
        private static UserContextData current;


        public static UserContextData Current
        {
            get
            {
                if(current==null)
                {
                    current = new UserContextData();
                }
                return current;
            }
        }

        public static bool IsMine( ExerciseLightDTO item)
        {
            if (Current.LoginStatus != LoginStatus.Logged || item == null || item.ProfileId == null || Current.CurrentProfile.GlobalId != item.ProfileId)
            {
                return false;
            }
            return true;
        }
        
        public static bool IsMine(this IBelongToUser item)
        {
            if (Current.LoginStatus != LoginStatus.Logged || item == null || item.Profile == null || Current.CurrentProfile.GlobalId != item.Profile.GlobalId)
            {
                return false;
            }
            return true;
        }

        public static bool IsPremium
        {
            get { return Current.ProfileInformation.Licence.CurrentAccountType != AccountType.User; }
        }

        public static bool IsInstructor
        {
            get { return Current.ProfileInformation.Licence.CurrentAccountType == AccountType.Instructor || Current.ProfileInformation.Licence.CurrentAccountType == AccountType.Administrator; }
        }

        
        public static bool IsFavorite(this UserDTO user)
        {
            return Current.ProfileInformation != null && user != null && Current.ProfileInformation.FavoriteUsers.Where(x => x.GlobalId == user.GlobalId).Count() > 0;
        }

        public static bool IsMe(this UserDTO user)
        {
            return Current.LoginStatus == LoginStatus.Logged && user != null && user.GlobalId == Current.CurrentProfile.GlobalId;
        }

        public static bool IsInInvitation(this UserDTO user)
        {
            return Current.LoginStatus == LoginStatus.Logged && Current.ProfileInformation != null && user != null && Current.ProfileInformation.Invitations.Where(x => (x.Invited != null && x.Invited.GlobalId == user.GlobalId) || (x.Inviter != null && x.Inviter.GlobalId == user.GlobalId)).Count() > 0;
        }

        public static bool IsInvited(this UserDTO user)
        {
            return Current.LoginStatus == LoginStatus.Logged && Current.ProfileInformation != null && user != null && Current.ProfileInformation.Invitations.Where(x => (x.Invited != null && x.Invited.GlobalId == user.GlobalId)).Count() > 0;
        }

        public static bool IsInviter(this UserDTO user)
        {
            return Current.LoginStatus == LoginStatus.Logged && Current.ProfileInformation != null && user != null && Current.ProfileInformation.Invitations.Where(x => (x.Inviter != null && x.Inviter.GlobalId == user.GlobalId)).Count() > 0;
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
            return Current.LoginStatus == LoginStatus.Logged && Current.ProfileInformation != null && user != null && Current.ProfileInformation.Friends.Where(x => x.GlobalId == user.GlobalId).Count() > 0;
        }

        public static void CreateUserContext(SessionData data)
        {
            
            Current.SessionData = data;
            ServiceManager.Token = Current.SessionData.Token;
            Current.refreshUserDataImplementation(false);
            Current.LoginStatus = LoginStatus.Logged;

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

        

    }
}
