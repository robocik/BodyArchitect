using System;
using System.Collections.Generic;
using System.ServiceModel;
using AutoMapper;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Service.Model;
using BodyArchitect.Service.Model.WP7;
using BodyArchitect.Shared;
using NHibernate;
using System.Reflection;
// ADDED REFERENCE!!!
using BodyArchitect.Model;

namespace BodyArchitect.Service
{
    
    [NHibernateContext]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall,IncludeExceptionDetailInFaults=true)]
    public class BodyArchitectAccessService : IBodyArchitectAccessService
    {
               
        Service.V2.BodyArchitectAccessService service = new V2.BodyArchitectAccessService();
        public Helpers h = new Helpers();

        static BodyArchitectAccessService()
        {
            //AutoMapperConfiguration.Configure();
        }

        public void ImportData(ImportDataStream data)
        {
            throw new NotImplementedException();
        }

        public IList<EntryObjectDTO> GetMyTrainingEntries(Token token, MyTrainingDTO myTraining)
        {
            //var token2=Mapper.Map<Token, V2.Model.Token>(token);
            //var myTraining2=Mapper.Map<MyTrainingDTO, V2.Model.MyTrainingDTO>(myTraining);
            //var result = service.GetMyTrainingEntries(token2, myTraining2);

            //return Mapper.Map<IList<V2.Model.EntryObjectDTO>, EntryObjectDTO[]>(result);
            throw new NotImplementedException();
        }

        public void DeleteProfile(Token token, ProfileDTO profile)
        {
            throw new NotImplementedException();
        }

        public void AccountOperation(string userNameOrEmail, AccountOperationType operationType)
        {
            throw new NotImplementedException();
        }

        public void UserFavoritesOperation(Token token, UserDTO userDto, FavoriteOperation operation)
        {
            throw new NotImplementedException();
        }

        public MapperResult MapExercises(Token token, MapperData data)
        {
            throw new NotImplementedException();
        }

        public BlogCommentDTO BlogCommentOperation(Token token, BlogCommentOperation arg)
        {
            throw new NotImplementedException();
        }

        public PagedResult<BlogCommentDTO> GetBlogComments(Token token, BlogEntryDTO entry, PartialRetrievingInfo info)
        {
            throw new NotImplementedException();
        }

        public void MessageOperation(Token token, MessageOperationParam arg)
        {
            throw new NotImplementedException();
        }

        public void SendMessage(Token token, MessageDTO message)
        {
            throw new NotImplementedException();
        }

        public ProfileInformationDTO GetProfileInformation(Token token, GetProfileInformationCriteria criteria)
        {
            BodyArchitect.Service.V2.InternalBodyArchitectService service = new V2.InternalBodyArchitectService(NHibernateContext.Current().Session);
            V2.Model.Token v2token = new V2.Model.Token(token.SessionId, token.Language);
            V2.Model.GetProfileInformationCriteria v2Crit = new V2.Model.GetProfileInformationCriteria();
            //v2Crit.UserId
            int? tempId;
            Guid? tempGuid;
            if ((tempGuid = h.GetGuidFromInt(criteria)) == null)
                tempGuid = null;
            v2Crit.UserId = tempGuid;
            var res = service.GetProfileInformation(v2token, v2Crit);

            ProfileInformationDTO profile = new ProfileInformationDTO();
            profile.AboutInformation = res.AboutInformation;
            profile.Birthday = res.Birthday;

            foreach (V2.Model.UserSearchDTO u in res.FavoriteUsers)
            {
                UserSearchDTO a = new UserSearchDTO();
                a.CountryId = u.CountryId;
                //a.CreationDate = u.CreationDate;
                SetProperty(a, "CreationDate", u.CreationDate);
                a.Gender = (BodyArchitect.Service.Model.Gender)u.Gender;
                //a.Id = u.GlobalId;
                tempId = null;
                if ((tempId = h.GetIntFromGuid(u.GlobalId)) == null)
                    tempId = h.SetIntFromGuid(u);
                if (tempId != null)
                    a.Id = (int)tempId;
                else
                    throw new ArgumentException("Id not assigned to guid", u.GlobalId.ToString());

                //a.IsDeleted = u.IsDeleted;
                SetProperty(a, "IsDeleted", u.IsDeleted);
                //a.Picture = u.Picture;
                a.Picture = new PictureInfoDTO();
                if (u.Picture != null)
                {
                    a.Picture.Hash = u.Picture.Hash;
                    a.Picture.PictureId = u.Picture.PictureId;
                    a.Picture.SessionId = u.Picture.SessionId;
                }
                else
                    a.Picture = null;
                a.Privacy.BirthdayDate = (BodyArchitect.Service.Model.Privacy)u.Privacy.BirthdayDate;
                a.Privacy.CalendarView = (BodyArchitect.Service.Model.Privacy)u.Privacy.CalendarView;
                a.Privacy.Friends = (BodyArchitect.Service.Model.Privacy)u.Privacy.Friends;
                a.Privacy.Searchable = u.Privacy.Searchable;
                a.Privacy.Sizes = (BodyArchitect.Service.Model.Privacy)u.Privacy.Sizes;
                a.Statistics.A6WEntriesCount = u.Statistics.A6WEntriesCount;
                a.Statistics.A6WFullCyclesCount = u.Statistics.A6WFullCyclesCount;
                a.Statistics.BlogCommentsCount = u.Statistics.TrainingDayCommentsCount;
                a.Statistics.BlogEntriesCount = u.Statistics.BlogEntriesCount;
                a.Statistics.FollowersCount = u.Statistics.FollowersCount;
                a.Statistics.FriendsCount = u.Statistics.FriendsCount;
                //a.Statistics.Id
                if ((tempId = h.GetIntFromGuid(u.Statistics.GlobalId)) == null)
                    tempId = h.SetIntFromGuid(u.Statistics);
                if (tempId != null)
                    a.Id = (int)tempId;
                else
                    throw new ArgumentException("Id not assigned to guid", u.Statistics.GlobalId.ToString());
                //a.Statistics.IsNew
                a.Statistics.LastEntryDate = u.Statistics.LastEntryDate;
                a.Statistics.LastLoginDate = u.Statistics.LastLoginDate;
                a.Statistics.LoginsCount = u.Statistics.LoginsCount;
                a.Statistics.MyBlogCommentsCount = u.Statistics.MyTrainingDayCommentsCount;
                a.Statistics.SizeEntriesCount = u.Statistics.SizeEntriesCount;
                a.Statistics.StrengthTrainingEntriesCount = u.Statistics.StrengthTrainingEntriesCount;
                a.Statistics.SupplementEntriesCount = u.Statistics.SupplementEntriesCount;
                a.Statistics.Tag = u.Statistics.Tag;
                a.Statistics.TrainingDaysCount = u.Statistics.TrainingDaysCount;
                a.Statistics.VotingsCount = u.Statistics.VotingsCount;
                a.Statistics.WorkoutPlansCount = u.Statistics.WorkoutPlansCount;
                a.UserName = u.UserName;
                profile.FavoriteUsers.Add(a);
            }


            //profile.Friends = res.Friends;
            foreach (V2.Model.UserSearchDTO u in res.Friends)
            {
                UserSearchDTO a = new UserSearchDTO();
                a.CountryId = u.CountryId;
                //a.CreationDate = u.CreationDate;
                SetProperty(a, "CreationDate", u.CreationDate);
                a.Gender = (BodyArchitect.Service.Model.Gender)u.Gender;
                //a.Id = u.GlobalId;
                tempId = null;
                if ((tempId = h.GetIntFromGuid(u.GlobalId)) == null)
                    tempId = h.SetIntFromGuid(u);
                if (tempId != null)
                    a.Id = (int)tempId;
                else
                    throw new ArgumentException("Id not assigned to guid", u.GlobalId.ToString());

                //a.IsDeleted = u.IsDeleted;
                SetProperty(a, "IsDeleted", u.IsDeleted);
                //a.Picture = u.Picture;
                a.Picture = new PictureInfoDTO();
                if (u.Picture != null)
                {
                    a.Picture.Hash = u.Picture.Hash;
                    a.Picture.PictureId = u.Picture.PictureId;
                    a.Picture.SessionId = u.Picture.SessionId;
                }
                else
                    a.Picture = null;
                a.Privacy.BirthdayDate = (BodyArchitect.Service.Model.Privacy)u.Privacy.BirthdayDate;
                a.Privacy.CalendarView = (BodyArchitect.Service.Model.Privacy)u.Privacy.CalendarView;
                a.Privacy.Friends = (BodyArchitect.Service.Model.Privacy)u.Privacy.Friends;
                a.Privacy.Searchable = u.Privacy.Searchable;
                a.Privacy.Sizes = (BodyArchitect.Service.Model.Privacy)u.Privacy.Sizes;
                a.Statistics.A6WEntriesCount = u.Statistics.A6WEntriesCount;
                a.Statistics.A6WFullCyclesCount = u.Statistics.A6WFullCyclesCount;
                a.Statistics.BlogCommentsCount = u.Statistics.TrainingDayCommentsCount;
                a.Statistics.BlogEntriesCount = u.Statistics.BlogEntriesCount;
                a.Statistics.FollowersCount = u.Statistics.FollowersCount;
                a.Statistics.FriendsCount = u.Statistics.FriendsCount;
                //a.Statistics.Id
                if ((tempId = h.GetIntFromGuid(u.Statistics.GlobalId)) == null)
                    tempId = h.SetIntFromGuid(u.Statistics);
                if (tempId != null)
                    a.Id = (int)tempId;
                else
                    throw new ArgumentException("Id not assigned to guid", u.Statistics.GlobalId.ToString());
                //a.Statistics.IsNew
                a.Statistics.LastEntryDate = u.Statistics.LastEntryDate;
                a.Statistics.LastLoginDate = u.Statistics.LastLoginDate;
                a.Statistics.LoginsCount = u.Statistics.LoginsCount;
                a.Statistics.MyBlogCommentsCount = u.Statistics.MyTrainingDayCommentsCount;
                a.Statistics.SizeEntriesCount = u.Statistics.SizeEntriesCount;
                a.Statistics.StrengthTrainingEntriesCount = u.Statistics.StrengthTrainingEntriesCount;
                a.Statistics.SupplementEntriesCount = u.Statistics.SupplementEntriesCount;
                a.Statistics.Tag = u.Statistics.Tag;
                a.Statistics.TrainingDaysCount = u.Statistics.TrainingDaysCount;
                a.Statistics.VotingsCount = u.Statistics.VotingsCount;
                a.Statistics.WorkoutPlansCount = u.Statistics.WorkoutPlansCount;
                a.UserName = u.UserName;
                profile.FavoriteUsers.Add(a);
            }

            //profile.Invitations =  res.Invitations;
            foreach (V2.Model.FriendInvitationDTO c in res.Invitations)
            {
                Model.FriendInvitationDTO a = new FriendInvitationDTO();
                a.CreatedDateTime = c.CreatedDateTime;
                a.InvitationType = (BodyArchitect.Service.Model.InvitationType) c.InvitationType;
                a.Invited.CountryId = c.Invited.CountryId;
                //a.Invited.Id
                SetProperty(a.Invited, "CreationDate", c.Invited.CreationDate);
                a.Invited.Gender = (BodyArchitect.Service.Model.Gender)c.Invited.Gender;
                //a.Id = c.Invited.GlobalId;
                tempId = null;
                if ((tempId = h.GetIntFromGuid(c.Invited.GlobalId)) == null)
                    tempId = h.SetIntFromGuid(c.Invited);
                if (tempId != null)
                    a.Invited.Id = (int)tempId;
                else
                    throw new ArgumentException("Id not assigned to guid", c.Invited.GlobalId.ToString());

                //a.IsDeleted = c.Invited.IsDeleted;
                SetProperty(a.Invited, "IsDeleted", c.Invited.IsDeleted);
                //a.Picture = c.Invited.Picture;
                a.Invited.Picture = new PictureInfoDTO();
                if (c.Invited.Picture != null)
                {
                    a.Invited.Picture.Hash = c.Invited.Picture.Hash;
                    a.Invited.Picture.PictureId = c.Invited.Picture.PictureId;
                    a.Invited.Picture.SessionId = c.Invited.Picture.SessionId;
                }
                else
                    a.Invited.Picture = null;
                a.Invited.Privacy.BirthdayDate = (BodyArchitect.Service.Model.Privacy)c.Invited.Privacy.BirthdayDate;
                a.Invited.Privacy.CalendarView = (BodyArchitect.Service.Model.Privacy)c.Invited.Privacy.CalendarView;
                a.Invited.Privacy.Friends = (BodyArchitect.Service.Model.Privacy)c.Invited.Privacy.Friends;
                a.Invited.Privacy.Searchable = c.Invited.Privacy.Searchable;
                a.Invited.Privacy.Sizes = (BodyArchitect.Service.Model.Privacy)c.Invited.Privacy.Sizes;
                a.Invited.UserName = c.Invited.UserName;



                a.Inviter.CountryId = c.Inviter.CountryId;
                //a.Inviter.Id
                SetProperty(a.Inviter, "CreationDate", c.Inviter.CreationDate);
                a.Inviter.Gender = (BodyArchitect.Service.Model.Gender)c.Inviter.Gender;
                //a.Id = c.Inviter.GlobalId;
                tempId = null;
                if ((tempId = h.GetIntFromGuid(c.Inviter.GlobalId)) == null)
                    tempId = h.SetIntFromGuid(c.Inviter);
                if (tempId != null)
                    a.Inviter.Id = (int)tempId;
                else
                    throw new ArgumentException("Id not assigned to guid", c.Inviter.GlobalId.ToString());

                //a.IsDeleted = c.Inviter.IsDeleted;
                SetProperty(a.Inviter, "IsDeleted", c.Inviter.IsDeleted);
                //a.Picture = c.Inviter.Picture;
                a.Inviter.Picture = new PictureInfoDTO();
                if (c.Inviter.Picture != null)
                {
                    a.Inviter.Picture.Hash = c.Inviter.Picture.Hash;
                    a.Inviter.Picture.PictureId = c.Inviter.Picture.PictureId;
                    a.Inviter.Picture.SessionId = c.Inviter.Picture.SessionId;
                }
                else
                    a.Inviter.Picture = null;
                a.Inviter.Privacy.BirthdayDate = (BodyArchitect.Service.Model.Privacy)c.Inviter.Privacy.BirthdayDate;
                a.Inviter.Privacy.CalendarView = (BodyArchitect.Service.Model.Privacy)c.Inviter.Privacy.CalendarView;
                a.Inviter.Privacy.Friends = (BodyArchitect.Service.Model.Privacy)c.Inviter.Privacy.Friends;
                a.Inviter.Privacy.Searchable = c.Inviter.Privacy.Searchable;
                a.Inviter.Privacy.Sizes = (BodyArchitect.Service.Model.Privacy)c.Inviter.Privacy.Sizes;
                a.Inviter.UserName = c.Inviter.UserName;
                
                a.Message = c.Message;
                profile.Invitations.Add(a);
            }


            profile.IsActivated = res.IsActivated;
            profile.LastLogin = res.LastLogin;
            profile.Messages = null; //?
            profile.RetrievedDateTime = res.RetrievedDateTime;
            //profile.Settings = res.Settings;

            profile.Settings = new ProfileSettingsDTO();
            tempId = null;
            if ((tempId = h.GetIntFromGuid(res.Settings.GlobalId)) == null)
                tempId = h.SetIntFromGuid(res.Settings);
            if (tempId != null)
                profile.Settings.Id = (int)tempId;
            else
                throw new ArgumentException("Id not assigned to guid", res.Settings.GlobalId.ToString());
            //profile.Role = res.Profile.
            profile.Settings.AutomaticUpdateMeasurements = res.Settings.AutomaticUpdateMeasurements;
            //profile.Settings.Id = res.Profile.Settings.GlobalId;
            
            //SetProperty(profile.Settings, "IsNew", res.Profile.Settings.IsNew);
            //profile.Settings.IsNew = res.Profile.Settings.IsNew;
            //TODO:check notifications
            profile.Settings.NotificationBlogCommentAdded = res.Settings.NotificationBlogCommentAdded != BodyArchitect.Service.V2.Model.ProfileNotification.None;
            profile.Settings.NotificationExerciseVoted = res.Settings.NotificationVoted != BodyArchitect.Service.V2.Model.ProfileNotification.None; //??
            profile.Settings.NotificationFriendChangedCalendar = res.Settings.NotificationFriendChangedCalendar != BodyArchitect.Service.V2.Model.ProfileNotification.None;
            profile.Settings.NotificationWorkoutPlanVoted = res.Settings.NotificationVoted != BodyArchitect.Service.V2.Model.ProfileNotification.None; //??

            profile.User = new UserSearchDTO();
            profile.User.CountryId = res.User.CountryId;
            SetProperty(profile.User, "CreationDate", res.User.CreationDate);
            profile.User.Gender = (BodyArchitect.Service.Model.Gender)res.User.Gender;
            //a.Id = res.User.GlobalId;
            tempId = null;
            if ((tempId = h.GetIntFromGuid(res.User.GlobalId)) == null)
                tempId = h.SetIntFromGuid(res.User);
            if (tempId != null)
                profile.User.Id = (int)tempId;
            else
                throw new ArgumentException("Id not assigned to guid", res.User.GlobalId.ToString());

            //a.IsDeleted = res.User.IsDeleted;
            SetProperty(profile.User, "IsDeleted", res.User.IsDeleted);
            //a.Picture = res.User.Picture;
            profile.User.Picture = new PictureInfoDTO();
            if (res.User.Picture != null)
            {
                profile.User.Picture.Hash = res.User.Picture.Hash;
                profile.User.Picture.PictureId = res.User.Picture.PictureId;
                profile.User.Picture.SessionId = res.User.Picture.SessionId;
            }
            else
                profile.User.Picture = null;
            profile.User.Privacy.BirthdayDate = (BodyArchitect.Service.Model.Privacy)res.User.Privacy.BirthdayDate;
            profile.User.Privacy.CalendarView = (BodyArchitect.Service.Model.Privacy)res.User.Privacy.CalendarView;
            profile.User.Privacy.Friends = (BodyArchitect.Service.Model.Privacy)res.User.Privacy.Friends;
            profile.User.Privacy.Searchable = res.User.Privacy.Searchable;
            profile.User.Privacy.Sizes = (BodyArchitect.Service.Model.Privacy)res.User.Privacy.Sizes;
            profile.User.UserName = res.User.UserName;


            profile.Wymiary = new WymiaryDTO();
            if (res.Wymiary != null)
            {
                profile.Wymiary.DateTime = res.Wymiary.Time.DateTime;

                profile.Wymiary.Height = (int)res.Wymiary.Height;   //possible loss of data
                tempId = null;
                if ((tempId = h.GetIntFromGuid(res.Wymiary.GlobalId)) == null)
                    tempId = h.SetIntFromGuid(res.Wymiary);
                if (tempId != null)
                    profile.Wymiary.Id = (int)tempId;
                else
                    throw new ArgumentException("Id not assigned to guid", res.Wymiary.GlobalId.ToString());
                //profile.Wymiary.IsEmpty = res.Wymiary.IsEmpty; //READ ONLY
                profile.Wymiary.IsNaCzczo = false;  //res.Wymiary.????
                //profile.Wymiary.IsNew = res.Wymiary.IsNew;    //READ ONLY
                profile.Wymiary.Klatka = (float) res.Wymiary.Klatka;
                profile.Wymiary.LeftBiceps = (float)res.Wymiary.LeftBiceps;
                profile.Wymiary.LeftForearm = (float)res.Wymiary.LeftForearm;
                profile.Wymiary.LeftUdo = (float)res.Wymiary.LeftUdo;
                profile.Wymiary.Pas = (float)res.Wymiary.Pas;
                profile.Wymiary.RightBiceps = (float)res.Wymiary.RightBiceps;
                profile.Wymiary.RightForearm = (float)res.Wymiary.RightForearm;
                profile.Wymiary.RightUdo = (float)res.Wymiary.RightUdo;
                profile.Wymiary.Tag = res.Wymiary.Tag;
                profile.Wymiary.Weight = (float)res.Wymiary.Weight;
            }
            return profile;
            
        }

        public FriendInvitationDTO InviteFriendOperation(Token token, InviteFriendOperationData data)
        {
            throw new NotImplementedException();
        }

        public SessionData Login(ClientInformation clientInfo, string username, string password)
        {
            throw new NotImplementedException();
            //BodyArchitect.Service.V2.InternalBodyArchitectService service = new V2.InternalBodyArchitectService(NHibernateContext.Current().Session);
            //BodyArchitect.Service.V2.Model.ClientInformation v2ClientInfo = new V2.Model.ClientInformation();
            //v2ClientInfo.ApplicationLanguage = clientInfo.ApplicationLanguage;
            //v2ClientInfo.ApplicationVersion = clientInfo.ApplicationVersion;
            //v2ClientInfo.ClientInstanceId = clientInfo.ClientInstanceId;
            //v2ClientInfo.Platform = (BodyArchitect.Service.V2.Model.PlatformType)clientInfo.Platform;
            //v2ClientInfo.PlatformVersion = clientInfo.PlatformVersion;
            //v2ClientInfo.Version = clientInfo.Version;

            //var res = service.Login(v2ClientInfo, username, password);
            //if (res == null) return null;
            //ProfileDTO profile = new ProfileDTO();
            ////profile.Id = res.Profile.GlobalId;
            //int? tempId;
            //if ((tempId = h.GetIntFromGuid(res.Profile.GlobalId)) == null)
            //    tempId = h.SetIntFromGuid(res.Profile);
            //if (tempId != null)
            //    profile.Id = (int)tempId;
            //else
            //    throw new ArgumentException("Id not assigned to guid", res.Profile.GlobalId.ToString());



            //profile.Gender = (BodyArchitect.Service.Model.Gender)res.Profile.Gender;
            //profile.AboutInformation = res.Profile.AboutInformation;
            //profile.Birthday = res.Profile.Birthday;
            //profile.CountryId = res.Profile.CountryId;
            //SetProperty(profile, "CreationDate", res.Profile.CreationDate);
            ////profile.CreationDate = res.Profile.CreationDate; // protected set;
            //profile.Email = res.Profile.Email;
            //SetProperty(profile, "IsDeleted", res.Profile.IsDeleted);
            ////profile.IsDeleted = res.Profile.IsDeleted;
            //profile.Password = res.Profile.Password;

            ////profile.Picture = (BodyArchitect.Service.Model.PictureInfoDTO) res.Profile.Picture;
            //profile.Picture = new PictureInfoDTO();
            //if (res.Profile.Picture != null)
            //{
            //    profile.Picture.Hash = res.Profile.Picture.Hash;
            //    profile.Picture.PictureId = res.Profile.Picture.PictureId;
            //    profile.Picture.SessionId = res.Profile.Picture.SessionId;
            //}
            //else
            //    profile.Picture = null;

            //profile.PreviousClientInstanceId = res.Profile.PreviousClientInstanceId;
            //profile.Privacy.BirthdayDate = (BodyArchitect.Service.Model.Privacy)res.Profile.Privacy.BirthdayDate;
            //profile.Privacy.CalendarView = (BodyArchitect.Service.Model.Privacy)res.Profile.Privacy.CalendarView;
            //profile.Privacy.Friends = (BodyArchitect.Service.Model.Privacy)res.Profile.Privacy.Friends;
            //profile.Privacy.Searchable = res.Profile.Privacy.Searchable;
            //profile.Privacy.Sizes = (BodyArchitect.Service.Model.Privacy)res.Profile.Privacy.Sizes;

            ////todo:check
            //profile.Settings = new ProfileSettingsDTO();
            //tempId = null;
            //if ((tempId = h.GetIntFromGuid(res.Profile.Settings.GlobalId)) == null)
            //    tempId = h.SetIntFromGuid(res.Profile.Settings);
            //if (tempId != null)
            //    profile.Settings.Id = (int)tempId;
            //else
            //    throw new ArgumentException("Id not assigned to guid", res.Profile.Settings.GlobalId.ToString());
            ////profile.Role = res.Profile.
            //profile.Settings.AutomaticUpdateMeasurements = res.Profile.Settings.AutomaticUpdateMeasurements;
            ////profile.Settings.Id = res.Profile.Settings.GlobalId;
         


            ////SetProperty(profile.Settings, "IsNew", res.Profile.Settings.IsNew);
            ////profile.Settings.IsNew = res.Profile.Settings.IsNew;
            ////TODO:check notifications
            //profile.Settings.NotificationBlogCommentAdded = res.Profile.Settings.NotificationBlogCommentAdded!=BodyArchitect.Service.V2.Model.ProfileNotification.None;
            //profile.Settings.NotificationExerciseVoted = res.Profile.Settings.NotificationVoted!=BodyArchitect.Service.V2.Model.ProfileNotification.None; //??
            //profile.Settings.NotificationFriendChangedCalendar = res.Profile.Settings.NotificationFriendChangedCalendar != BodyArchitect.Service.V2.Model.ProfileNotification.None;
            //profile.Settings.NotificationWorkoutPlanVoted = res.Profile.Settings.NotificationVoted != BodyArchitect.Service.V2.Model.ProfileNotification.None; //??

            //profile.UserName = res.Profile.UserName;
            //SetProperty(profile, "Version", res.Profile.Version);
            ////profile.Version = res.Profile.Version;  //protected set;
            ////res.Profile.Tag?
            ////res.Profile.IsNew?
            ////res.Profile.ExtensionData??

            //Token token = new Token(res.Token.SessionId, res.Token.Language);
            ////token. = res.Token.ExtensionData;
            //SessionData sessionData = new SessionData(token, profile, res.IsConnected);

            //sessionData.LastLoginDate = res.LastLoginDate;
            ////sessionData. = res.ExtensionData;
            //return sessionData;
        }

        public void Logout(Token token)
        {
            throw new NotImplementedException();
        }

        public SessionData CreateProfile(ClientInformation clientInfo, ProfileDTO newProfile)
        {
            throw new NotImplementedException();
        }

        public ProfileDTO UpdateProfile(Token token, ProfileUpdateData data)
        {
            throw new NotImplementedException();
        }

        public void DeleteTrainingDay(Token token, TrainingDayDTO dayDto)
        {
            throw new NotImplementedException();
        }

        public PictureInfoDTO UploadImage(PictureDTO pictureDto)
        {
            throw new NotImplementedException();
        }

        public bool CheckProfileNameAvailability(string userName)
        {
            throw new NotImplementedException();
        }

        public TrainingDayDTO SaveTrainingDay(Token token, TrainingDayDTO day)
        {
            BodyArchitect.Service.V2.InternalBodyArchitectService service = new V2.InternalBodyArchitectService(NHibernateContext.Current().Session);

            //var test=service.SaveTrainingDay(token,);
            throw new NotImplementedException();

        }

        public PagedResult<TrainingDayDTO> GetTrainingDays(Token token, WorkoutDaysSearchCriteria searchCriteria, PartialRetrievingInfo retrievingInfo)
        {
            throw new NotImplementedException();
        }

        public TrainingDayDTO GetTrainingDay(Token token, WorkoutDayGetOperation operationParams, RetrievingInfo retrievingInfo)
        {
            throw new NotImplementedException();
        }

        public PictureDTO GetImage(PictureInfoDTO pictureInfo)
        {
            throw new NotImplementedException();
        }

        public PagedResult<ExerciseDTO> GetExercises(Token token, ExerciseSearchCriteria searchCriteria, PartialRetrievingInfo retrievingInfo)
        {
            BodyArchitect.Service.V2.InternalBodyArchitectService service = new V2.InternalBodyArchitectService(NHibernateContext.Current().Session);
            V2.Model.Token v2token = new V2.Model.Token(token.SessionId, token.Language);
            V2.Model.ExerciseSearchCriteria crit = new V2.Model.ExerciseSearchCriteria();
            V2.Model.PartialRetrievingInfo nfo = new V2.Model.PartialRetrievingInfo();
            crit = Mapper.Map<V2.Model.ExerciseSearchCriteria>(searchCriteria);
            nfo = Mapper.Map<V2.Model.PartialRetrievingInfo>(retrievingInfo);

            var res = service.GetExercises(v2token, crit, nfo);
            var ret = Mapper.Map<V2.Model.PagedResult<V2.Model.ExerciseDTO>, PagedResult<ExerciseDTO>>(res);
            return ret;
        }

        public PagedResult<SuplementDTO> GetSuplements(Token token, PartialRetrievingInfo retrievingInfo)
        {
            throw new NotImplementedException();
        }

        public ExerciseDTO SaveExercise(Token token, ExerciseDTO exercise)
        {
             throw new NotImplementedException();
        }

        public SuplementDTO SaveSuplement(Token token, SuplementDTO suplement)
        {
            throw new NotImplementedException();
        }

        public IList<MyTrainingDTO> GetStartedTrainings(Token token, Guid? typeId, RetrievingInfo retrievingInfo)
        {
            throw new NotImplementedException();
        }

        public PagedResult<WorkoutPlanDTO> GetWorkoutPlans(Token token, WorkoutPlanSearchCriteria searchCriteria, PartialRetrievingInfo pagerInfo)
        {
            throw new NotImplementedException();
        }

        public WorkoutPlanDTO GetWorkoutPlan(Token token, Guid planId, RetrievingInfo retrievingInfo)
        {
            throw new NotImplementedException();
        }

        public WorkoutPlanDTO SaveWorkoutPlan(Token token, WorkoutPlanDTO dto)
        {
            throw new NotImplementedException();
        }

        public WorkoutPlanDTO VoteWorkoutPlan(Token token, WorkoutPlanDTO planDto)
        {
            throw new NotImplementedException();
        }

        public ExerciseDTO VoteExercise(Token token, ExerciseDTO exercise)
        {
            throw new NotImplementedException();
        }

        public PagedResult<CommentEntryDTO> GetComments(Token token, Guid globalId, PartialRetrievingInfo retrievingInfo)
        {
            throw new NotImplementedException();
        }

        public void DeleteWorkoutPlan(Token token, WorkoutPlanDTO dto)
        {
            throw new NotImplementedException();
        }

        public void DeleteExercise(Token token, ExerciseDTO dto)
        {
            throw new NotImplementedException();
        }

        public void WorkoutPlanFavoritesOperation(Token token, WorkoutPlanDTO planDto, FavoriteOperation operation)
        {
            throw new NotImplementedException();
        }

        public WorkoutPlanDTO PublishWorkoutPlan(Token token, WorkoutPlanDTO planDto)
        {
            throw new NotImplementedException();
        }

        public ExerciseDTO PublishExercise(Token token, ExerciseDTO exerciseDto)
        {
            throw new NotImplementedException();
        }

        public PagedResult<UserSearchDTO> GetUsers(Token token, UserSearchCriteria searchCriteria, PartialRetrievingInfo pagerInfo)
        {
            throw new NotImplementedException();
        }

        public string WP7Register(string deviceid, string uri, int profileId)
        {
            throw new NotImplementedException();
        }

        public void WP7Unregister(string deviceid)
        {
            throw new NotImplementedException();
        }

        public void WP7ClearCounter(string deviceid)
        {
            throw new NotImplementedException();
        }

        public TrialStatusInfo WP7TrialStatus(string deviceId)
        {
            throw new NotImplementedException();
        }

        #region int <==> guid convertion methods

        /* int <==> guid conversion methods */

        

        #endregion

        public static void SetProperty(object obj, string fieldName, object value)
        {
            MemberInfo[] info = obj.GetType().GetMember(fieldName, BindingFlags.Instance | BindingFlags.SetField | BindingFlags.Public | BindingFlags.NonPublic);
            if (info.Length > 0)
            {
                var fieldInfo = (PropertyInfo)info[0];
                fieldInfo.SetValue(obj, value, null);
            }
            else
            {
                throw new ArgumentException("Wrong fieldName", fieldName);
            }
        }


    }

}

