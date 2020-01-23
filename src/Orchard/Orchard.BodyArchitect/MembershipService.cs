using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Orchard;
using Orchard.BodyArchitect.Models;
using Orchard.BodyArchitect.WCF;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Security;
using Orchard.Users.Models;

namespace Orchand.BodyArchitect
{
    [UsedImplicitly]
    [OrchardSuppressDependency("Orchard.Users.Services.MembershipService")]
    public class MembershipService : IMembershipService
    {
        private readonly IOrchardServices _orchardServices;

        public MembershipService(IOrchardServices orchardServices) {
            _orchardServices = orchardServices;
        }

        public MembershipSettings GetSettings() {
            var settings = new MembershipSettings();
            // accepting defaults
            return settings;
        }

        public IUser CreateUser(CreateUserParams createUserParams)
        {
            BACreateUserParams param =(BACreateUserParams) createUserParams;
            ClientInformation clientInfo = new ClientInformation();
            clientInfo.ApplicationLanguage = "pl";
            clientInfo.ApplicationVersion = "5.0.0.0";
            clientInfo.ClientInstanceId = Guid.NewGuid();
            clientInfo.Platform = PlatformType.Web;
            clientInfo.PlatformVersion = "temp";
            clientInfo.Version = "4.5.0.0";

            try 
            {
                BodyArchitectAccessServiceClient client = new BodyArchitectAccessServiceClient("Full");
                ProfileDTO newProfile = new ProfileDTO();
                newProfile.Email = param.Email;
                newProfile.UserName = param.Username;
                newProfile.Password = param.Password.ToSHA1Hash();
                newProfile.CountryId = param.CountryId;
                newProfile.Birthday = DateTime.Now.AddYears(-20);
                newProfile.Privacy = new ProfilePrivacyDTO();
                var result = client.CreateProfile(clientInfo, newProfile);
                return ensureUserExistsInDb(result);
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }

        public IUser GetUser(string username) {
            throw new NotImplementedException();
        }

        public IUser ValidateUser(string userNameOrEmail, string password)
        {
            BodyArchitectAccessServiceClient client = new BodyArchitectAccessServiceClient("Full");
            ClientInformation clientInfo = new ClientInformation();
            clientInfo.ApplicationLanguage = "pl";
            clientInfo.ApplicationVersion = "5.0.0.0";
            clientInfo.ClientInstanceId = Guid.NewGuid();
            clientInfo.Platform = PlatformType.Web;
            clientInfo.PlatformVersion = "temp";
            clientInfo.Version = "4.5.0.0";
            var sessionData = client.Login(clientInfo, userNameOrEmail, password.ToSHA1Hash());
            return ensureUserExistsInDb(sessionData);
        }

        private IUser ensureUserExistsInDb(SessionData sessionData) {
            if (sessionData == null)
            {
                return null;
            }
            var user = _orchardServices.ContentManager.Query<UserPart, UserPartRecord>().Where(u => u.NormalizedUserName == sessionData.Profile.UserName).List().FirstOrDefault();
            if (user == null) {
                user = _orchardServices.ContentManager.New<UserPart>("User");
                user.UserName = sessionData.Profile.UserName;
                user.NormalizedUserName = user.UserName.ToLowerInvariant();
                user.Email = sessionData.Profile.Email;
                user.Record.Password = sessionData.Profile.Password;

                user.Record.RegistrationStatus = UserStatus.Approved;
                user.Record.EmailStatus = UserStatus.Approved;
                user.Record.RegistrationStatus = UserStatus.Approved;
                user.Record.EmailStatus = UserStatus.Approved;

                _orchardServices.ContentManager.Create(user);
                //user.Record.HashAlgorithm = "SHA1";
                //SetPassword(user.Record, createUserParams.Password);
            }
            else {
                user.Record.Password = sessionData.Profile.Password;
                _orchardServices.ContentManager.Flush();
            }
           
            var baUser = new BAUser(user, sessionData.Token);
            return baUser;
        }

        public void SetPassword(IUser user, string password) {
            throw new NotImplementedException();
        }
    }
}
