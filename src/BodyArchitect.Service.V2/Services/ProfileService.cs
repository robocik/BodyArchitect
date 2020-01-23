using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Portable.Exceptions;
using BodyArchitect.Service.V2.Localization;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Reports;
using BodyArchitect.Service.V2.Payments;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Criterion.Lambda;
using NHibernate.Transform;
using NHibernate.Type;
using AccountType = BodyArchitect.Model.AccountType;
using Gender = BodyArchitect.Model.Gender;
using ObjectNotFoundException = BodyArchitect.Shared.ObjectNotFoundException;
using PlatformType = BodyArchitect.Model.PlatformType;
using Privacy = BodyArchitect.Model.Privacy;
using Profile = BodyArchitect.Model.Profile;
using ProfileNotification = BodyArchitect.Model.ProfileNotification;
using ProfileSettings = BodyArchitect.Model.ProfileSettings;
using PublishStatus = BodyArchitect.Model.PublishStatus;

namespace BodyArchitect.Service.V2.Services
{
    public class ProfileService : MessageServiceBase
    {

        public ProfileService(ISession session, SecurityInfo securityInfo, ServiceConfiguration configuration, SecurityManager securityManager, IPushNotificationService pushNotificationService, IEMailService emailService)
            : base(session, securityInfo, configuration, pushNotificationService, emailService)
        {
            this.SecurityManager = securityManager;
        }


        public SecurityManager SecurityManager { get; private set; }

        public void ProfileOperation(Token token, ProfileOperationParam param)
        {
            Log.WriteWarning("ProfileOperation:Username={0}", SecurityInfo.SessionData.Profile.UserName);
            var session = Session;
            if (SecurityInfo.SessionData.Profile.GlobalId != param.ProfileId)
            {
                throw new CrossProfileOperationException("Cannot delete another profile");
            }
            using (var tx = session.BeginSaveTransaction())
            {
                var dbProfile = session.Load<Profile>(param.ProfileId);
                if (param.Operation == Model.ProfileOperation.Delete)
                {
                    deleteProfile(session, dbProfile);
                }
                else if (param.Operation == Model.ProfileOperation.AccountType)
                {
                    accountType(dbProfile, (AccountType)param.AccountType.Value);
                }
                else
                {
                    //change status
                    dbProfile.Statistics.Status = param.Status != null ? param.Status.Map<ProfileStatus>() : null;
                }

                if (param.Operation != Model.ProfileOperation.Delete)
                {
                    session.Update(dbProfile);
                }

                tx.Commit();

                if (param.Operation == Model.ProfileOperation.Delete)
                {
                    Log.WriteVerbose("Profile deleted");
                    if (dbProfile.Picture != null)
                    {
                        PictureService pictureService = new PictureService(Session, SecurityInfo, Configuration);
                        pictureService.DeletePicture(dbProfile.Picture);
                    }

                    Log.WriteVerbose("Remove token");
                    //logout after delete profile
                    SecurityManager.Remove(token);
                }
                else if (param.Operation == Model.ProfileOperation.AccountType)
                {
                    //update currently logged user in SecurityManager (do this for every client running because one user can be logged at the same time using many applications)
                    var newLicence = dbProfile.Licence.Map<LicenceInfoDTO>();
                    var securityInfos = SecurityManager.GetForProfile(dbProfile.GlobalId);
                    foreach (var info in securityInfos)
                    {
                        info.Licence = newLicence;
                        info.Licence.CurrentAccountType = SecurityInfo.Licence.AccountType;
                    }
                }
            }
        }

        private void accountType(Profile dbProfile, AccountType accountType)
        {
            Log.WriteVerbose("Change account type from {0} to {1}", dbProfile.Licence.AccountType, accountType);

            if (accountType == dbProfile.Licence.AccountType)
            {
                return;
            }
            if (accountType == AccountType.Administrator)
            {
                throw new InvalidOperationException("Cannot change account type to Administrator");
            }

            //first check if there is any promotion for paid accounts
            var currentAccount = ensurePromotion(dbProfile.Licence.AccountType);
            var newAccount = ensurePromotion(accountType);

            DateTime last = dbProfile.Licence.LastPointOperationDate;

            int accountDiff = accountType - dbProfile.Licence.AccountType;
            if (accountDiff < 0)
            {
                //downgrade (we must take additional bapoints for this)
                dbProfile.Licence.BAPoints -= Math.Abs(accountDiff) * Configuration.Payments.Kara;
                if (dbProfile.Licence.BAPoints < 0)
                {//if we decrease account type that user can always do this (even if there is no enough points)
                    dbProfile.Licence.BAPoints = 0;
                }
            }
            else
            {
                //in calculation when increase account we should take also promotions 
                int diffInDays = (int)(Configuration.TimerService.UtcNow.Date - last.Date).TotalDays + 1;
                //dbProfile.Licence.BAPoints -= diffInDays * (int)(getAccountTypeFactor(accountType) - getAccountTypeFactor(dbProfile.Licence.CurrentAccountType));
                var diffPoints = diffInDays * (newAccount.GetCurrentPoints(Configuration.TimerService.UtcNow) - currentAccount.GetCurrentPoints(Configuration.TimerService.UtcNow));
                if (diffPoints > 0)
                {
                    dbProfile.Licence.BAPoints -= diffPoints;
                }

            }


            if (dbProfile.Licence.BAPoints < 0)
            {
                throw new ConsistencyException("You have not enough ba points");
            }


            dbProfile.Licence.AccountType = accountType;
            dbProfile.Licence.LastPointOperationDate = Configuration.TimerService.UtcNow;

        }

        internal void deleteProfile(ISession session, Profile dbProfile)
        {
            Log.WriteVerbose("Removing friends");

            var deletedProfile = session.QueryOver<Profile>().Where(x => x.IsDeleted).SingleOrDefault();
            var profileId = dbProfile.GlobalId;
            var friendsToDelete = new List<Profile>(dbProfile.Friends);
            dbProfile.Friends.Clear();
            //send messages to friends);
            foreach (var friend in friendsToDelete)
            {
                Log.WriteVerbose("Removing user'{0}' from friends list", friend.UserName);
                //messageService.SendSystemMessage(null, dbProfile, friend,
                //                                 BodyArchitect.Model.MessageType.FriendProfileDeleted);


                NewSendMessageEx(friend.Settings.NotificationSocial, dbProfile, friend, "FriendProfileDeleted_Topic", "FriendProfileDeleted_Content", dbProfile.UserName, DateTime.Now);

                //now remove from friends
                friend.Friends.Remove(dbProfile);
                ProfileStatisticsUpdater.UpdateFriends(session, friend, dbProfile);
            }
            Log.WriteVerbose("Remaping votings...");
            //remap votings to deletedProfile
            foreach (var ratingUserValue in session.QueryOver<RatingUserValue>().Where(x => x.ProfileId == profileId).List())
            {
                ratingUserValue.ProfileId = deletedProfile.GlobalId;
                ratingUserValue.LoginData = null;
                session.Update(ratingUserValue);
            }

            Log.WriteVerbose("Remap TrainingDayComments...");

            TrainingDay _trainingDay = null;
            var comments = session.QueryOver<TrainingDayComment>()
                .JoinAlias(x => x.TrainingDay, () => _trainingDay).Where(x => _trainingDay.Profile != dbProfile && x.Profile == dbProfile).List();

            foreach (var ex in comments)
            {
                ex.Profile = deletedProfile;
                ex.LoginData = null;
            }

            //delete favorites for another users
            Profile _aliasProfile = null;
            var favotites = session.QueryOver<Profile>().JoinAlias(x => x.FavoriteUsers, () => _aliasProfile).Where(
                () => _aliasProfile.GlobalId == dbProfile.GlobalId).List();
            foreach (Profile favotite in favotites)
            {
                Log.WriteVerbose("Removing from favorites for user'{0}'", favotite.UserName);
                //messageService.SendSystemMessage(null, dbProfile, favotite,
                //                                 BodyArchitect.Model.MessageType.FavoriteProfileDeleted);
                NewSendMessageEx(favotite.Settings.NotificationSocial, dbProfile, favotite, "FavoriteProfileDeleted_Topic", "FavoriteProfileDeleted_Content", dbProfile.UserName, DateTime.Now);

                //now remove from friends
                favotite.FavoriteUsers.Remove(dbProfile);
            }

            //now update followers statistics
            var favortieUsers = new List<Profile>(dbProfile.FavoriteUsers);
            dbProfile.FavoriteUsers.Clear();
            foreach (var favoriteUser in favortieUsers)
            {
                ProfileStatisticsUpdater.UpdateFollowers(session, favoriteUser);
            }
            Log.WriteVerbose("Delete Messages...");
            var meesagesOfOtherUsers = session.QueryOver<Message>().Where(x => x.Sender == dbProfile).List();
            foreach (Message msg in meesagesOfOtherUsers)
            {
                //now remove from sender
                msg.Sender = deletedProfile;
            }

            var count = session.Delete("FROM Message WHERE Receiver=?", dbProfile.GlobalId, NHibernateUtil.Guid);
            session.Flush();
            Log.WriteVerbose("Previous operation row count={0}. Delete LoginData...", count);
            count = session.Delete("FROM LoginData WHERE ProfileId=?", dbProfile.GlobalId, NHibernateUtil.Guid);
            Log.WriteVerbose("Previous operation row count={0}. Delete BAPoints...", count);
            count = session.Delete("FROM BAPoints WHERE Profile=?", dbProfile.GlobalId, NHibernateUtil.Guid);
            Log.WriteVerbose("Previous operation row count={0}. Delete reminders...", count);
            count = session.Delete("FROM ReminderItem WHERE Profile=?", dbProfile.GlobalId, NHibernateUtil.Guid);
            //session.Delete("SELECT bc FROM TrainingDay td,BlogEntry be, TrainingDayComment bc WHERE bc.BlogEntry = be AND be.TrainingDay=td AND td.ProfileId=?", profile.GlobalId, NHibernateUtil.Guid);
            Log.WriteVerbose("Previous operation row count={0}. Delete training days...", count);
            count = session.Delete("FROM TrainingDay WHERE Profile=?", dbProfile.GlobalId, NHibernateUtil.Guid);


            Log.WriteVerbose("Previous operation row count={0}. Delete mytrainings...", count);
            count = session.Delete("FROM MyTraining WHERE Profile=?", dbProfile.GlobalId, NHibernateUtil.Guid);
            Log.WriteVerbose("Previous operation row count={0}.  Delete workout plans...", count);

            count = session.Delete("FROM TrainingPlan WHERE Profile=? AND Status=?",
                                       new object[] { dbProfile.GlobalId, BodyArchitect.Model.PublishStatus.Private },
                                       new IType[] { NHibernateUtil.Guid, NHibernateUtil.Int32 });
            Log.WriteVerbose("Previous operation row count={0}.  Delete supplements cycle definitions...", count);

            count = session.Delete("FROM SupplementCycleDefinition WHERE Profile=? AND Status=?",
                                       new object[] { dbProfile.GlobalId, BodyArchitect.Model.PublishStatus.Private },
                                       new IType[] { NHibernateUtil.Guid, NHibernateUtil.Int32 });
            Log.WriteVerbose("Previous operation row count={0}.Delete exercises...", count);

            TrainingPlanDay _day = null;
            TrainingPlanEntry _entry = null;
            Exercise _exercise = null;
            //get all exercises used in any workout plans and change the owner to deleted
            var ttt = session.QueryOver<TrainingPlan>()
                .JoinAlias(x => x.Days, () => _day)
                .JoinAlias(x => _day.Entries, () => _entry)
                .JoinAlias(x => _entry.Exercise, () => _exercise).Where(x => _exercise.Profile == dbProfile).Select(x => _entry.Exercise).List<Exercise>();

            foreach (var ex in ttt)
            {
                ex.Profile = deletedProfile;
            }

            ttt = session.QueryOver<StrengthTrainingItem>()
                .JoinAlias(x => x.Exercise, () => _exercise).Where(x => _exercise.Profile == dbProfile).Select(x => x.Exercise).List<Exercise>();

            foreach (var ex in ttt)
            {
                ex.Profile = deletedProfile;
            }

            count = session.Delete("FROM Exercise WHERE Profile=?", new object[] { dbProfile.GlobalId }, new IType[] { NHibernateUtil.Guid });
            

            Log.WriteVerbose("Remap published training plans...");
            var publicPlans = session.QueryOver<TrainingPlan>().Where(x => x.Status == PublishStatus.Published && x.Profile == dbProfile).List();
            foreach (var plan in publicPlans)
            {
                //now remove from public plans of this user
                plan.Profile = deletedProfile;
            }

            Log.WriteVerbose("Delete Remap published supplements cycle definitions...");
            var publicSupplementsCycleDefinitions = session.QueryOver<SupplementCycleDefinition>().Where(x => x.Status == PublishStatus.Published && x.Profile == dbProfile).List();
            foreach (var plan in publicSupplementsCycleDefinitions)
            {
                //now remove from public definition of this user
                plan.Profile = deletedProfile;
            }

            Log.WriteVerbose("Delete FriendInvitation...");
            count = session.Delete("FROM FriendInvitation WHERE Inviter=? OR Invited=?",
                                   new object[] { dbProfile.GlobalId, dbProfile.GlobalId },
                                   new IType[] { NHibernateUtil.Guid, NHibernateUtil.Guid });
            Log.WriteVerbose("Previous operation row count={0}. Delete lists...", count);

            //dbProfile.FavoriteWorkoutPlans.Clear();
            //dbProfile.FavoriteUsers.Clear();

            //session.Delete(dbProfile.Statistics);
            //session.Delete(dbProfile.DataInfo);
            //dbProfile.DataInfo = null;
            //dbProfile.Statistics = null;
            //session.Delete(dbProfile.Settings);
            //dbProfile.Settings = null;
            //dbProfile.IsDeleted = true;
            //if (dbProfile.Wymiary != null)
            //{
            //    Log.WriteInfo("Delete wymiary id={0}", dbProfile.Wymiary.GlobalId);
            //    session.Delete(dbProfile.Wymiary);
            //    dbProfile.Wymiary = null;
            //}

            session.Delete("FROM ExerciseProfileData WHERE Profile=?", dbProfile.GlobalId, NHibernateUtil.Guid);
            ///////INSTRUCTOR
            Log.WriteVerbose("Delete PaymentsBasket...");
            count = session.Delete("FROM PaymentBasket WHERE Profile=?", dbProfile.GlobalId, NHibernateUtil.Guid);
            Log.WriteVerbose("Previous operation row count={0}.Delete schedule entries...", count);
            count = session.Delete("FROM ScheduleEntry WHERE Profile=?", dbProfile.GlobalId, NHibernateUtil.Guid);
            Log.WriteVerbose("Previous operation row count={0}.Delete championships...", count);
            count = session.Delete("FROM Championship WHERE Profile=?", dbProfile.GlobalId, NHibernateUtil.Guid);
            Log.WriteVerbose("Previous operation row count={0}. Delete customers...", count);
            count = session.Delete("FROM Customer WHERE Profile=?", dbProfile.GlobalId, NHibernateUtil.Guid);
            Log.WriteVerbose("Previous operation row count={0}. Delete customer groups...", count);
            count = session.Delete("FROM CustomerGroup WHERE Profile=?", dbProfile.GlobalId, NHibernateUtil.Guid);
            Log.WriteVerbose("Previous operation row count={0}. Delete activities...", count);
            count = session.Delete("FROM Activity WHERE Profile=?", dbProfile.GlobalId, NHibernateUtil.Guid);
            Log.WriteVerbose("Previous operation row count={0}. Delete MyPlaces...", count);
            count = session.Delete("FROM MyPlace WHERE Profile=?", dbProfile.GlobalId, NHibernateUtil.Guid);
            Log.WriteVerbose("Previous operation row count={0}. Delete WP7PushNotification...", count);

            Log.WriteVerbose("Delete WP7PushNotification...");
            count = session.Delete("FROM WP7PushNotification WHERE ProfileId=?", dbProfile.GlobalId, NHibernateUtil.Guid);
            Log.WriteVerbose("Previous operation row count={0}. Delete WP7PushNotification...", count);
            session.Delete(dbProfile);

        }

        public SessionData CreateProfile(ClientInformation clientInfo, ProfileDTO newProfile)
        {
            Log.WriteWarning("CreateProfile: Username:{0}", newProfile.UserName);
            ensureMaintenanceMode();
            var session = Session;
            var settings = newProfile.Settings;

            var dbProfile = Mapper.Map<ProfileDTO, BodyArchitect.Model.Profile>(newProfile);
            LoginData loginData = null;
            using (var transactionScope = new TransactionManager(true))
            {
                var count = (from profile in session.QueryOver<BodyArchitect.Model.Profile>()
                             where profile.UserName == dbProfile.UserName
                             select profile).RowCount();
                if (count > 0)
                {
                    throw new UniqueException("Profile with the same UserName exists");
                }

                count = (from p in session.QueryOver<BodyArchitect.Model.Profile>()
                         where p.Email == dbProfile.Email
                         select p).RowCount();
                if (count > 0)
                {
                    throw new UniqueException("Profile with the same EMail exists");
                }
                if (dbProfile.Privacy == null)
                {
                    dbProfile.Privacy = new ProfilePrivacy();
                }
                dbProfile.Statistics = new ProfileStatistics();
                if (settings == null)
                {
                    dbProfile.Settings = new ProfileSettings();
                }
                else
                {
                    dbProfile.Settings = settings.Map<ProfileSettings>();
                }
                //by default calendar and measurements are public
                dbProfile.Privacy.CalendarView = BodyArchitect.Model.Privacy.Public;
                dbProfile.Privacy.Sizes = Privacy.Public;
                dbProfile.Privacy.Searchable = true;
                //every newly created account have Premium account for 30 days
                dbProfile.Licence.BAPoints = 30 * Configuration.Payments.GetPoints(Model.AccountType.PremiumUser).Points;
                dbProfile.Licence.AccountType = AccountType.PremiumUser;
                dbProfile.Licence.LastPointOperationDate = Configuration.TimerService.UtcNow;

                dbProfile.CreationDate = Configuration.TimerService.UtcNow;
                //id required to activation
                if (Configuration.RequireActivateNewProfile)
                {
                    dbProfile.ActivationId = Guid.NewGuid().ToString();
                }
                dbProfile.DataInfo = new DataInfo();
                session.Save(dbProfile.Statistics);
                session.Save(dbProfile);

                MyPlace myPlace = new MyPlace();
                myPlace.Profile = dbProfile;
                myPlace.IsDefault = true;
                myPlace.IsSystem = true;
                myPlace.Name = "Default";//TODO:Translate?
                myPlace.Color = System.Drawing.Color.LavenderBlush.ToColorString();
                session.Save(myPlace);

                //now send message to the user
                //MessageService messageService = new MessageService(session, SecurityInfo, Configuration, pushNotificationService);
                //messageService.SendSystemMessage(null, getAdministrator(), dbProfile, BodyArchitect.Model.MessageType.ProfileCreated);
                NewSendMessageEx(ProfileNotification.Message, getAdministrator(), dbProfile, "ProfileCreated_Topic", "ProfileCreated_Content");
                

                LoginAudit(session, dbProfile, clientInfo, out loginData);

                if (Configuration.RequireActivateNewProfile)
                {//we required activation so until the email is sent we cannot create the profle
                    AccountOperation(dbProfile.Email, AccountOperationType.SendActivationEmail);
                }


                transactionScope.CommitTransaction();
                Log.WriteVerbose("Profile created");
            }


            if (!Configuration.RequireActivateNewProfile)
            {//activation is not needed so we send only information email so if this throws exception we should continue anyway
                try
                {
                    //EmailService.SendEMail(dbProfile, "CreateProfileEMailSubject", "CreateProfileEMailMessage", DateTime.Now, dbProfile.UserName);
                    CultureInfo culture = dbProfile.GetProfileCulture();
                    var subject = string.Format(LocalizedStrings.ResourceManager.GetString("CreateProfileEMailSubject", culture), DateTime.Now, dbProfile.UserName);
                    var message = string.Format(LocalizedStrings.ResourceManager.GetString("CreateProfileEMailMessage", culture), DateTime.Now, dbProfile.UserName);
                    EmailService.NewSendEMail(dbProfile, subject, message);
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Default.Process(ex);
                }

            }

            if (Configuration.RequireActivateNewProfile)
            {//user needs to activate account so we will not create session data
                return null;
            }
            Log.WriteVerbose("Create session data...");
            var profileDTO = Mapper.Map<Profile, ProfileDTO>(dbProfile);
            var sessionData = SecurityManager.CreateNewSession(profileDTO, clientInfo, loginData);
            var securityInfo = SecurityManager.EnsureAuthentication(sessionData.Token);
            securityInfo.Licence = dbProfile.Licence.Map<LicenceInfoDTO>();
            securityInfo.Licence.CurrentAccountType = securityInfo.Licence.AccountType;
            return sessionData;
        }

        public void AccountOperation(string userNameOrEmail, AccountOperationType operationType)
        {
            Log.WriteWarning("RestorePassword: userNameOrEmail={0}", userNameOrEmail);

            var session = Session;
            using (var tx = session.BeginSaveTransaction())
            {
                var profile = session.QueryOver<Profile>().Where(x => x.UserName == userNameOrEmail || x.Email == userNameOrEmail).SingleOrDefault();

                if (profile != null)
                {
                    if (profile.IsDeleted)
                    {
                        throw new ProfileDeletedException("Profile has been deleted");
                    }
                    if (operationType == AccountOperationType.RestorePassword)
                    {
                        Log.WriteVerbose("Profile found");
                        string password = RandomPassword.Generate(7);
                        profile.Password = password.ToSHA1Hash();
                        session.Update(profile);
                        Log.WriteVerbose("Password changed");
                        //now send email with new password
                        NewSendMessageEx(ProfileNotification.Email, getAdministrator(), profile, "RestorePasswordEmailSubject", "RestorePasswordEmailMessage", Configuration.TimerService.UtcNow, password);
                        //EmailService.NewSendEMail(profile, "RestorePasswordEmailSubject", "RestorePasswordEmailMessage", DateTime.Now, password);
                    }
                    else
                    {
                        if (profile.IsActivated)
                        {
                            throw new ProfileIsNotActivatedException("Profile is already activated");
                        }
                        var path = Configuration.ServerUrl;
                        path = string.Format("{0}\\AcivateAccount.aspx?Id={1}", path.TrimEnd('/', '\\'),
                                             profile.ActivationId);
                        //EmailService.SendEMail(profile, "CreateProfileEMailSubject", "CreateProfileEMailMessage_ActivationMode",
                        //                       DateTime.Now, profile.UserName, path);
                        NewSendMessageEx(ProfileNotification.Email, getAdministrator(), profile, "CreateProfileEMailSubject", "CreateProfileEMailMessage_ActivationMode", Configuration.TimerService.UtcNow, profile.UserName, path);
                    }
                }
                else
                {
                    Log.WriteWarning("Profile not found");
                    throw new ObjectNotFoundException("Profile or email doesn't existi");
                }
                tx.Commit();
            }
            Log.WriteVerbose("RestorePassword end");
        }

        public ProfileDTO UpdateProfile(ProfileUpdateData data)
        {
            Log.WriteWarning("UpdateProfile:Username={0}", SecurityInfo.SessionData.Profile.UserName);


            if (data.Profile.GlobalId != SecurityInfo.SessionData.Profile.GlobalId)
            {
                throw new CrossProfileOperationException("Cannot modify profil for another user");
            }


            //var dbProfile = Mapper.Map<ProfileDTO, Profile>(data.Profile);
            //dbProfile.Wymiary = Mapper.Map<WymiaryDTO, Wymiary>(data.Wymiary);
            var session = Session;
            using (var tx = session.BeginSaveTransaction())
            {
                var originalProfile = session.QueryOver<Profile>().Where(x => x.GlobalId == data.Profile.GlobalId).Fetch(x => x.Settings).Eager.Fetch(x => x.Wymiary).Eager.SingleOrDefault();
                if (data.Profile.UserName != SecurityInfo.SessionData.Profile.UserName)
                {
                    throw new ValidationException("Cannot change UserName");
                }
                if (data.Profile.Email != SecurityInfo.SessionData.Profile.Email)
                {
                    throw new ValidationException("Cannot change EMail");
                }

                //if (data.Profile.AccountType != SecurityInfo.SessionData.Profile.AccountType)
                //{
                //    throw new ValidationException("Cannot change AccountType");
                //}
                if (originalProfile.Version != data.Profile.Version)
                {
                    throw new StaleObjectStateException("Profile", data.Profile.GlobalId);
                }
                //originalProfile.Birthday = data.Profile.Birthday;
                //originalProfile.Password = data.Profile.Password;
                //originalProfile.Privacy = Mapper.Map<ProfilePrivacyDTO, ProfilePrivacy>(data.Profile.Privacy);
                //originalProfile.Version= data.Profile.Version;
                //originalProfile.CountryId = data.Profile.CountryId;
                //originalProfile.AboutInformation = data.Profile.AboutInformation;
                //originalProfile.Picture = Mapper.Map<PictureInfoDTO, Picture>(data.Profile.Picture);
                //originalProfile.Gender = (Gender) data.Profile.Gender;
                //originalProfile.Wymiary = Mapper.Map<WymiaryDTO, Wymiary>(data.Wymiary);

                //remove old picture;
                //if (originalProfile.Picture != null && (data.Profile.Picture == null || (data.Profile.Picture.Hash != originalProfile.Picture.Hash && data.Profile.Picture.PictureId != originalProfile.Picture.PictureId)))
                //{
                //    PictureService pictureService = new PictureService(Session, SecurityInfo, Configuration);
                //    pictureService.DeletePicture(originalProfile.Picture);
                //}
                PictureService pictureService = new PictureService(Session, SecurityInfo, Configuration);
                pictureService.DeletePictureLogic(originalProfile.Picture, data.Profile.Picture);

                Mapper.Map<ProfileDTO, Profile>(data.Profile, originalProfile);
                Mapper.Map(data.Profile.Settings, originalProfile.Settings);

                if (!originalProfile.Settings.AutomaticUpdateMeasurements)
                {
                    if (originalProfile.Wymiary != null)
                    {
                        var originalId = originalProfile.Wymiary.GlobalId;
                        Log.WriteVerbose("Mapping existing wymiary: {0}", originalProfile.Wymiary.GlobalId);
                        Mapper.Map<WymiaryDTO, Wymiary>(data.Wymiary, originalProfile.Wymiary);
                        originalProfile.Wymiary.GlobalId = originalId;
                    }
                    else
                    {
                        Log.WriteVerbose("Mapping new wymiary...");
                        originalProfile.Wymiary = Mapper.Map<WymiaryDTO, Wymiary>(data.Wymiary);
                    }

                    if (originalProfile.Wymiary != null && originalProfile.Wymiary.IsEmpty)
                    {
                        Log.WriteVerbose("Wymiary is empty.");
                        if (!originalProfile.Wymiary.IsNew)
                        {
                            Log.WriteInfo("Delete wymiary from db");
                            session.Delete(originalProfile.Wymiary);
                        }
                        originalProfile.Wymiary = null;
                    }
                    else if (originalProfile.Wymiary != null)
                    {
                        Log.WriteInfo("Save wymiary in db");
                        session.SaveOrUpdate(originalProfile.Wymiary);
                    }
                }
                else
                {
                    MeasurementsAutomaticUpdater.Update(session, originalProfile, null,
                                                        Configuration.TimerService.UtcNow);
                }

                //session.Evict(originalProfile);
                session.Update(originalProfile);
                //var te=session.SaveOrUpdateCopy(originalProfile);
                tx.Commit();
                Log.WriteInfo("Profile updated");
                return Mapper.Map<Profile, ProfileDTO>(originalProfile);
            }


        }

        public SessionData Login(ClientInformation clientInfo, string username, string password)
        {
            Log.WriteInfo("Login: Username: {0}", username);
            ensureMaintenanceMode();
            var session = Session;

            using (var tx = session.BeginGetTransaction())
            {
                //var profileResult =
                //from profile in session.QueryOver<BodyArchitect.Model.Profile>()
                //where profile.UserName == username && profile.Password == password
                //select profile;

                var profileResult = session.QueryOver<BodyArchitect.Model.Profile>().Where(x => x.UserName == username && x.Password == password).Fetch(x => x.Settings).Eager.Fetch(x => x.Statistics).Eager;
                SessionData sessionData = null;

                var loggedProfile = profileResult.SingleOrDefault();
                if (loggedProfile != null)
                {

                    Log.WriteInfo("Profile found");
                    if (loggedProfile.IsDeleted)
                    {
                        throw new ProfileDeletedException("Cannot login to deleted profile");
                    }
                    if (!loggedProfile.IsActivated)
                    {
                        throw new ProfileIsNotActivatedException("You must activate your account first");
                    }
                    var loggedAsAccountType = ensureLicences(loggedProfile);

                    LoginData currentLoginData;
                    var lastLoginData = LoginAudit(session, loggedProfile, clientInfo, out currentLoginData);
                    ProfileDTO profileDTO = Mapper.Map<Profile, ProfileDTO>(loggedProfile);
                    sessionData = SecurityManager.CreateNewSession(profileDTO, clientInfo, currentLoginData);
                    var securityInfo = SecurityManager.EnsureAuthentication(sessionData.Token);
                    securityInfo.Licence = loggedProfile.Licence.Map<LicenceInfoDTO>();
                    securityInfo.Licence.CurrentAccountType = loggedAsAccountType;
                    if (lastLoginData != null)
                    {
                        sessionData.LastLoginDate = lastLoginData.LoginDateTime;
                    }

                }
                tx.Commit();
                if (sessionData != null)
                {//run maintenance task only when user is correctly logged in
                    Configuration.MethodInvoker.Invoke((x) =>
                    {
                        maintenanceTask(sessionData.Profile.GlobalId);
                    }, null);
                }
                return sessionData;
            }
        }

        private void maintenanceTask(Guid profileId)
        {
            var maintenanceSession = NHibernateFactory.OpenSession();
            ReminderService reminderService = new ReminderService(maintenanceSession, SecurityInfo, Configuration);
            reminderService.RemoveOldReminders(profileId);
            maintenanceSession.Flush();
        }

        private Model.AccountType ensureLicences(Profile loggedProfile)
        {
            Model.AccountType accountType = (Model.AccountType)loggedProfile.Licence.AccountType;
            if (loggedProfile.Licence.AccountType != AccountType.Administrator)
            {
                //first check if there is any promotion for paid accounts
                var accountTypeToSet = ensurePromotion(loggedProfile.Licence.AccountType);
                accountType = accountTypeToSet.AccountType;
                if (accountTypeToSet.PromotionStartDate.HasValue)
                {
                    calculatePointsToSubtrack(loggedProfile, accountTypeToSet.PromotionStartDate.Value.AddDays(-1), accountTypeToSet.Points);
                    loggedProfile.Licence.LastPointOperationDate = accountTypeToSet.PromotionStartDate.Value.Date;
                }
                calculatePointsToSubtrack(loggedProfile, Configuration.TimerService.UtcNow, accountTypeToSet.GetCurrentPoints(Configuration.TimerService.UtcNow));
                //DateTime last = Configuration.TimerService.UtcNow;
                //if (loggedProfile.Licence.LastPointOperationDate.HasValue)
                //{
                //    last = loggedProfile.Licence.LastPointOperationDate.Value;
                //}
                //int diffInDays = (int) (Configuration.TimerService.UtcNow.Date - last.Date).TotalDays;
                //loggedProfile.Licence.BAPoints -= diffInDays*(int)getAccountTypeFactor(loggedProfile.Licence.CurrentAccountType);
                if (loggedProfile.Licence.BAPoints < 0)
                {
                    loggedProfile.Licence.AccountType = AccountType.User;
                    loggedProfile.Licence.BAPoints = 0;
                    accountType = Model.AccountType.User;
                }

                loggedProfile.Licence.AccountType = AccountType.Instructor;
                accountType = Model.AccountType.Instructor;
            }
            loggedProfile.Licence.LastPointOperationDate = Configuration.TimerService.UtcNow.Date;
            return accountType;
        }

        void calculatePointsToSubtrack(Profile loggedProfile, DateTime tillDate, int points)
        {
            DateTime last = loggedProfile.Licence.LastPointOperationDate;
            int diffInDays = (int)(tillDate.Date - last.Date).TotalDays;
            if (diffInDays > 0)
            {
                loggedProfile.Licence.BAPoints -= diffInDays * points;
            }
        }

        /// <summary>
        /// Checks if there is any good promotion for this user and if yes the return the promotion. If user has higher account then promotion then we return his account
        /// </summary>
        /// <returns></returns>
        PaymentAccountType ensurePromotion(AccountType accountType)
        {
            var premiumCost = Configuration.Payments.GetPoints(Model.AccountType.PremiumUser);
            var instructorCost = Configuration.Payments.GetPoints(Model.AccountType.Instructor);
            var currentCost = Configuration.Payments.GetPoints((Model.AccountType)accountType);
            if ((instructorCost.PromotionStartDate == null || instructorCost.PromotionStartDate <= Configuration.TimerService.UtcNow.Date) && instructorCost.PromotionPoints <= currentCost.GetCurrentPoints(Configuration.TimerService.UtcNow) && accountType != AccountType.Administrator)
            {
                return instructorCost;
            }
            if ((premiumCost.PromotionStartDate == null || premiumCost.PromotionStartDate <= Configuration.TimerService.UtcNow.Date) && premiumCost.PromotionPoints <= currentCost.GetCurrentPoints(Configuration.TimerService.UtcNow) && accountType == AccountType.User || accountType == AccountType.PremiumUser)
            {
                return premiumCost;
            }
            return currentCost;
        }

        public ProfileInformationDTO GetProfileInformation(Token token, GetProfileInformationCriteria criteria)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            Log.WriteWarning("GetProfileInformation:Username={0}", securityInfo.SessionData.Profile.UserName);
            var session = Session;
            using (var tx = session.BeginGetTransaction())
            {
                var userId = securityInfo.SessionData.Profile.GlobalId;
                if (criteria.UserId.HasValue)
                {
                    userId = criteria.UserId.Value;
                }
                Log.WriteVerbose("UserId={0}", userId);
                var userDb = session.Load<Profile>(userId);
                var profileDb = session.Load<Profile>(securityInfo.SessionData.Profile.GlobalId);


                ProfileInformationDTO info = new ProfileInformationDTO();
                var profileQuery = session.QueryOver<Profile>().Fetch(x => x.Statistics).Eager.Fetch(x => x.Settings).Eager.Fetch(x => x.DataInfo).Eager;

                bool showSizes = userDb == profileDb || !ServiceHelper.IsPrivateSizes(userDb, profileDb);
                bool showCalendar = userDb == profileDb || !ServiceHelper.IsPrivateCalendar(userDb, profileDb);
                bool showFriends = userDb == profileDb || userDb.Privacy.Friends == Privacy.Public || (userDb.Privacy.Friends == Privacy.FriendsOnly && userDb.Friends.Contains(profileDb));
                bool showInvitations = userDb == profileDb;
                bool showFavoriteUsers = userDb == profileDb;
                bool showProfileActivationState = userDb == profileDb;
                bool showSettings = userDb == profileDb;
                bool showLicence = userDb == profileDb;
                bool isMe = userDb == profileDb;
                bool showBirthdayDate = userDb == profileDb || userDb.Privacy.BirthdayDate == Privacy.Public || (userDb.Privacy.BirthdayDate == Privacy.FriendsOnly && userDb.Friends.Contains(profileDb));
                Log.WriteVerbose("showSizes:{0},showFriends:{1},showInvitations:{2},showFavoritieUsers:{3},showBirthdayDate:{4}",
                    showSizes, showFriends, showInvitations, showFavoriteUsers, showBirthdayDate);
                if (showSizes)
                {
                    profileQuery = profileQuery.Fetch(x => x.Wymiary).Eager;
                }
                //many to many relations so egar loading doesn't work
                if (showFriends)
                {
                    profileQuery = profileQuery.Fetch(x => x.Friends).Eager.Fetch(x => x.Friends.First().Statistics).Eager;
                }
                if (showFavoriteUsers)
                {
                    profileQuery = profileQuery.Fetch(x => x.FavoriteUsers).Eager.Fetch(x => x.FavoriteUsers.First().Statistics).Eager;
                }

                profileQuery = profileQuery.Where(x => x.GlobalId == userId);
                var profileFuture = profileQuery.FutureValue();

                IEnumerable<ExerciseProfileData> records = null;
                if (showCalendar)
                {
                    //retrieve records
                    Exercise exercise = null;
                    var query = Session.QueryOver<ExerciseProfileData>().JoinAlias(x => x.Exercise, () => exercise).Fetch(x => x.Profile).Eager.Fetch(x => x.Exercise).Eager;
                    records = query.Where(x => x.Profile == userDb && x.Customer == null && exercise.UseInRecords).Future();
                }
                Log.WriteVerbose("Retrieving last login date...");
                //get last login date
                var lastLoginDataFuture = session.QueryOver<LoginData>().Where(x => x.ProfileId == userDb.GlobalId).OrderBy(
                    x => x.LoginDateTime).Desc.Take(1).FutureValue();

                var lastLoginData = lastLoginDataFuture.Value;
                var profile = profileFuture.Value;

                if (lastLoginData != null)
                {
                    info.LastLogin = lastLoginData.LoginDateTime;
                    Log.WriteVerbose("Last login date: {0}", info.LastLogin);
                }
                if (showLicence)
                {
                    info.Licence = profile.Licence.Map<LicenceInfoDTO>();
                    info.Licence.CurrentAccountType = securityInfo.Licence.CurrentAccountType;
                    info.Licence.Payments = Configuration.Payments;
                }

                Log.WriteVerbose("Profile retrieved");
                info.User = Mapper.Map<Profile, UserSearchDTO>(profile);
                if (isMe && SecurityInfo.Licence.AccountType == Model.AccountType.User)
                {//for free accounts we change privacy of calendar and size to public. (in automapper). But for myself we should return original privacy settings
                    info.User.Privacy = profile.Privacy.Map<ProfilePrivacyDTO>();
                }
                info.DataInfo = profile.DataInfo.Map<DataInfoDTO>();
                if (showSizes)
                {
                    info.Wymiary = Mapper.Map<Wymiary, WymiaryDTO>(profile.Wymiary);
                }
                if (showSettings)
                {
                    info.Settings = Mapper.Map<ProfileSettings, ProfileSettingsDTO>(profile.Settings);
                }
                if (showFriends)
                {
                    info.Friends = new List<UserSearchDTO>(Mapper.Map<ICollection<Profile>, UserSearchDTO[]>(profile.Friends.ToList()));
                }
                if (showFavoriteUsers)
                {
                    info.FavoriteUsers = new List<UserSearchDTO>(Mapper.Map<ICollection<Profile>, UserSearchDTO[]>(profile.FavoriteUsers));
                }
                if (showBirthdayDate)
                {
                    info.Birthday = profileDb.Birthday;
                }
                if (showProfileActivationState)
                {
                    info.IsActivated = string.IsNullOrEmpty(profileDb.ActivationId);
                }
                if (showCalendar)
                {
                    info.Records = records.Map<List<ExerciseRecordsReportResultItem>>();
                }
                info.AboutInformation = profile.AboutInformation;
                List<FriendInvitationDTO> invitationList = new List<FriendInvitationDTO>();
                if (showInvitations)
                {
                    Log.WriteVerbose("Retrieve invitations...");
                    var invitations = session.QueryOver<FriendInvitation>().Where(x =>
                            (x.Inviter == userDb && x.InvitationType != FriendInvitationType.RejectFriendship) ||
                            (x.Invited == userDb && x.InvitationType != FriendInvitationType.RejectInvitation)).List();
                    foreach (var invitation in invitations)
                    {
                        FriendInvitationDTO dto = ObjectsConverter.ConvertFriendInvitation(profileDb, invitation);
                        invitationList.Add(dto);
                    }
                    Log.WriteVerbose("Invitations retrieved");
                }

                info.Invitations = invitationList;
                info.RetrievedDateTime = Configuration.TimerService.UtcNow;


                tx.Commit();

                if (isMe)
                {
                    try
                    {
                        WP7Service wp7Service = new WP7Service(Session, SecurityInfo, Configuration, this.PushNotification);
                        wp7Service.WP7ClearCounter(userDb);
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.Default.Process(ex);
                    }

                }
                return info;
            }
        }

        public LoginData LoginAudit(ISession session, Profile loggedProfile, ClientInformation clientInfo, out  LoginData loginData)
        {
            var lastLoginData = session.QueryOver<LoginData>().Where(x => x.ProfileId == loggedProfile.GlobalId).OrderBy(x => x.LoginDateTime).Desc.Take(1).SingleOrDefault();
            loginData = new LoginData();
            loginData.ApplicationLanguage = clientInfo.ApplicationLanguage;
            loginData.ApplicationVersion = clientInfo.ApplicationVersion;
            loginData.ClientInstanceId = clientInfo.ClientInstanceId;
            loginData.ProfileId = loggedProfile.GlobalId;
            loginData.LoginDateTime = Configuration.TimerService.UtcNow;
            loginData.Platform = (PlatformType)clientInfo.Platform;
            loginData.PlatformVersion = clientInfo.PlatformVersion;
            loginData.AccountType = loggedProfile.Licence.AccountType;
            loginData.ApiKey = session.Load<APIKey>(Configuration.CurrentApiKey);
            Log.WriteVerbose("Add LoginData");
            session.Save(loginData);
            session.Flush();
            loggedProfile.Statistics.LastLoginDate = loginData.LoginDateTime;
            ProfileStatisticsUpdater.UpdateLogins(session, loggedProfile);
            return lastLoginData;
        }

        private Profile getAdministrator()
        {
            Log.WriteVerbose("GetAdministrator");
            return Session.QueryOver<Profile>().Where(x => x.UserName == ProfileDTO.AdministratorName).SingleOrDefault();
        }

        void ensureMaintenanceMode()
        {
            Log.WriteVerbose("Maintenance mode: {0}", Configuration.IsMaintenanceMode);
            if (Configuration.IsMaintenanceMode)
            {
                throw new MaintenanceException("Service is in maintenance mode. Please try later.");
            }
        }

        public PagedResult<UserSearchDTO> GetUsers(UserSearchCriteria searchCriteria, PartialRetrievingInfo pagerInfo)
        {
            Log.WriteWarning("GetUsers: Username={0}", SecurityInfo.SessionData.Profile.UserName);

            var session = Session;
            using (var tx = session.BeginTransaction())
            {
                //BodyArchitect.Model.TrainingPlan _planAlias = null;
                //result = result.JoinAlias(x => x.MyWorkoutPlans, () => _planAlias).Where(
                //    () => _planAlias.Status == PublishStatus.Published);
                ProfileStatistics statistics = null;
                var profile = session.Get<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
                var result = session.QueryOver<Profile>().JoinAlias(x => x.Statistics, () => statistics).Where(x => x.GlobalId != SecurityInfo.SessionData.Profile.GlobalId && x.IsDeleted == false);
                var friendsIds = profile.Friends.Select(x => x.GlobalId).ToList();

                //when we search by GlobalId the searchable parameter is skipped (we know exactly the profile name)
                if (searchCriteria.ProfileId.HasValue)
                {
                    result = result.Where(user => user.GlobalId == searchCriteria.ProfileId.Value);
                }
                else
                {
                    result = result.Where(x => x.Privacy.Searchable);
                }

                if (!string.IsNullOrWhiteSpace(searchCriteria.UserName))
                {
                    result = result.WhereRestrictionOn(x => x.UserName).IsLike(searchCriteria.UserName, MatchMode.Start);
                }

                IQueryOverOrderBuilder<Profile, Profile> orderBuilder;
                switch (searchCriteria.SortOrder)
                {
                    case UsersSortOrder.ByFriendsCount:
                        orderBuilder = result.OrderBy(x => statistics.FriendsCount);
                        break;
                    case UsersSortOrder.ByBlogCommentsCount:
                        orderBuilder = result.OrderBy(x => statistics.TrainingDayCommentsCount);
                        break;
                    case UsersSortOrder.ByLastLoginDate:
                        orderBuilder = result.OrderBy(x => statistics.LastLoginDate);
                        break;
                    case UsersSortOrder.ByVotingCount:
                        orderBuilder = result.OrderBy(x => statistics.VotingsCount);
                        break;
                    case UsersSortOrder.ByLastEntryDate:
                        orderBuilder = result.OrderBy(x => statistics.LastEntryDate);
                        break;
                    case UsersSortOrder.ByFollowersCount:
                        orderBuilder = result.OrderBy(x => statistics.FollowersCount);
                        break;
                    case UsersSortOrder.ByTrainingDaysCount:
                        orderBuilder = result.OrderBy(x => statistics.TrainingDaysCount);
                        break;
                    case UsersSortOrder.ByWorkoutPlansCount:
                        orderBuilder = result.OrderBy(x => statistics.WorkoutPlansCount);
                        break;
                    default:
                        orderBuilder = result.OrderBy(x => x.UserName);
                        break;
                }
                if (searchCriteria.SortAscending)
                {
                    result = orderBuilder.Asc;
                }
                else
                {
                    result = orderBuilder.Desc;
                }
                if (searchCriteria.Countries.Count > 0)
                {
                    var langOr = Restrictions.Disjunction();
                    foreach (var lang in searchCriteria.Countries)
                    {
                        langOr.Add<BodyArchitect.Model.Profile>(x => x.CountryId == lang);
                    }
                    result = result.And(langOr);

                }
                if (searchCriteria.Genders.Count > 0)
                {
                    var langOr = Restrictions.Disjunction();
                    foreach (var lang in searchCriteria.Genders)
                    {
                        langOr.Add<BodyArchitect.Model.Profile>(x => x.Gender == (Gender)lang);
                    }
                    result = result.And(langOr);

                }
                if (searchCriteria.Picture == PictureCriteria.OnlyWithPicture)
                {
                    result = result.And(x => x.Picture.PictureId != Guid.Empty);
                }
                else if (searchCriteria.Picture == PictureCriteria.OnlyWithoutPicture)
                {
                    result = result.And(x => x.Picture.PictureId == null || x.Picture.PictureId == Guid.Empty);
                }

                if (searchCriteria.WorkoutPlan == UserPlanCriteria.OnlyWithPlans)
                {
                    //result = result.WhereRestrictionOn(x => x.MyWorkoutPlans).IsNotEmpty();
                    BodyArchitect.Model.TrainingPlan _planAlias = null;
                    result = result.JoinAlias(x => x.MyWorkoutPlans, () => _planAlias).Where(
                        () => _planAlias.Status == PublishStatus.Published);
                }

                if (searchCriteria.SupplementsDefinition == UserPlanCriteria.OnlyWithPlans)
                {
                    //result = result.WhereRestrictionOn(x => x.MyWorkoutPlans).IsNotEmpty();
                    BodyArchitect.Model.SupplementCycleDefinition _defAlias = null;
                    result = result.JoinAlias(x => x.MySupplementsDefinitions, () => _defAlias).Where(
                        () => _defAlias.Status == PublishStatus.Published);
                }

                //privacy criteria
                if (searchCriteria.AccessCalendar == PrivacyCriteria.Accessible)
                {
                    result = result.And(x => x.Privacy.CalendarView == Privacy.Public || x.Licence.AccountType == AccountType.User || (x.Privacy.CalendarView == Privacy.FriendsOnly && x.GlobalId.IsIn(friendsIds)));
                }
                else if (searchCriteria.AccessCalendar == PrivacyCriteria.NotAccessible)
                {
                    result = result.And(x => x.Licence.AccountType != AccountType.User && (x.Privacy.CalendarView == Privacy.Private || (x.Privacy.CalendarView == Privacy.FriendsOnly && !x.GlobalId.IsIn(friendsIds))));
                }

                if (searchCriteria.AccessSizes == PrivacyCriteria.Accessible)
                {
                    result = result.And(x => x.Privacy.Sizes == Privacy.Public || x.Licence.AccountType == AccountType.User || (x.Privacy.Sizes == Privacy.FriendsOnly && x.GlobalId.IsIn(friendsIds)));
                }
                else if (searchCriteria.AccessSizes == PrivacyCriteria.NotAccessible)
                {
                    result = result.And(x => x.Licence.AccountType != AccountType.User && (x.Privacy.Sizes == Privacy.Private || (x.Privacy.Sizes == Privacy.FriendsOnly && !x.GlobalId.IsIn(friendsIds))));
                }

                if (searchCriteria.AccessFriends == PrivacyCriteria.Accessible)
                {
                    result = result.And(x => x.Privacy.Friends == Privacy.Public || (x.Privacy.Friends == Privacy.FriendsOnly && x.GlobalId.IsIn(friendsIds)));
                }
                else if (searchCriteria.AccessFriends == PrivacyCriteria.NotAccessible)
                {
                    result = result.And(x => x.Privacy.Friends == Privacy.Private || (x.Privacy.Friends == Privacy.FriendsOnly && !x.GlobalId.IsIn(friendsIds)));
                }

                if (searchCriteria.UserSearchGroups.Count > 0)
                {
                    var favoritesIds = profile.FavoriteUsers.Select(x => x.GlobalId).ToList();

                    var bigOr = Restrictions.Disjunction();
                    if (searchCriteria.UserSearchGroups.IndexOf(UserSearchGroup.Favorites) > -1)
                    {
                        if (profile.FavoriteUsers.Count > 0)
                        {
                            bigOr.Add<Profile>(x => x.GlobalId.IsIn((ICollection)favoritesIds));
                        }
                    }
                    if (searchCriteria.UserSearchGroups.IndexOf(UserSearchGroup.Friends) > -1)
                    {
                        if (profile.Friends.Count > 0)
                        {
                            bigOr.Add(Restrictions.On<Profile>(x => x.GlobalId).IsIn((ICollection)friendsIds));
                        }
                    }
                    if (searchCriteria.UserSearchGroups.IndexOf(UserSearchGroup.Others) > -1)
                    {
                        var tmpAnd = Restrictions.Conjunction();
                        if (profile.Friends.Count > 0)
                        {
                            tmpAnd.Add(Restrictions.On<Profile>(x => x.GlobalId).Not.IsIn((ICollection)friendsIds));
                        }
                        if (profile.FavoriteUsers.Count > 0)
                        {
                            tmpAnd.Add(Restrictions.On<Profile>(x => x.GlobalId).Not.IsIn((ICollection)favoritesIds));
                        }
                        bigOr.Add(tmpAnd);
                    }

                    result = result.And(bigOr);
                }
                tx.Commit();
                var res = result.ToPagedResults<UserSearchDTO, Profile>(pagerInfo);

                return res;
            }


        }

        public bool CheckProfileNameAvailability(string username)
        {
            if (username == null || username.Length < 3)
            {
                return false;
            }
            //RegexValidator("^[a-zA-Z_][0-9a-zA-Z_]*$"
            var validator = new RegexValidator("^[a-zA-Z_][0-9a-zA-Z_]*$");
            var result=validator.Validate(username);
            if (!result.IsValid)
            {
                return false;
            }
            var session = Session;
            var profileResult = session.QueryOver<BodyArchitect.Model.Profile>().Where(x => x.UserName == username);//.Fetch(x=>x.Wymiary).Lazy;
            var loggedProfile = profileResult.SingleOrDefault();
            return loggedProfile == null;
        }

        public void UserFavoritesOperation(UserDTO userDto, FavoriteOperation operation)
        {

            Log.WriteWarning("UserFavoritesOperation: Username={0},userDto.GlobalId={1}", SecurityInfo.SessionData.Profile.UserName, userDto.GlobalId);

            var session = Session;
            if (SecurityInfo.SessionData.Profile.GlobalId == userDto.GlobalId)
            {
                throw new InvalidOperationException("You cannot add yourself to the favorites");
            }
            using (var tx = session.BeginSaveTransaction())
            {
                var profileDb = session.QueryOver<Profile>().Fetch(x => x.FavoriteUsers).Eager.Where(x => x.GlobalId == SecurityInfo.SessionData.Profile.GlobalId).SingleOrDefault();
                var userDb = session.Load<Profile>(userDto.GlobalId);
                if (operation == FavoriteOperation.Add)
                {
                    if (profileDb.FavoriteUsers.Contains(userDb))
                    {
                        throw new ObjectIsFavoriteException("User is in favorites list already");
                    }
                    profileDb.FavoriteUsers.Add(userDb);
                }
                else
                {
                    if (profileDb.FavoriteUsers.Contains(userDb))
                    {
                        profileDb.FavoriteUsers.Remove(userDb);
                    }
                    else
                    {
                        throw new ObjectIsNotFavoriteException("User is not in favorites list");
                    }
                }
                session.Update(profileDb);
                ProfileStatisticsUpdater.UpdateFollowers(session, userDb);
                tx.Commit();
            }
        }
    }
}
