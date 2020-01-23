using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Security;
using System.Security.Authentication;
using System.ServiceModel;
using System.Text;
using System.Threading;
using BodyArchitect.Logger;
using BodyArchitect.Service.Model;
using BodyArchitect.Service.Model.Exceptions;
using BodyArchitect.Service.Model.TrainingPlans;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF;
using IBodyArchitectAccessService = BodyArchitect.WCF.Automatic.IBodyArchitectAccessService;

namespace BodyArchitect.WCF
{
    public class ServiceManager
    {
        public static Guid ClientInstanceId { get; set; }

        public static Token Token { get;
            set;
        }

        public static int GetPageSize(int defaultSize)
        {
            int size = -1;
            int.TryParse(ConfigurationManager.AppSettings["PageSize"], out size);
            return size > -1 ? size : defaultSize;
        }

        public static IBodyArchitectAccessService Instance
        {
            get { return new BodyArchitect.WCF.Automatic.BodyArchitectAccessServiceClient(); }
        }

        public static IList<EntryObjectDTO> GetMyTrainingEntries( MyTrainingDTO myTraining)
        {
            return exceptionHandling(delegate
                                         {
                                             return Instance.GetMyTrainingEntries(Token, myTraining);
                                         });
        }

        public static void AccountOperation(string userOrEmail,AccountOperationType type)
        {
            exceptionHandling(delegate
            {
                Instance.AccountOperation(userOrEmail, type);
            });
        }

        public static void DeleteProfile(ProfileDTO msg)
        {
            exceptionHandling(delegate
            {
                Instance.DeleteProfile(Token, msg);
            });
        }

        public static PagedResult<BlogCommentDTO> GetBlogComments( BlogEntryDTO entry, PartialRetrievingInfo info)
        {
            return exceptionHandling(delegate
            {
                info.PageSize = GetPageSize(info.PageSize);
                return Instance.GetBlogComments(Token, entry, info);
            });
        }
        public static MapperResult MapExercises(MapperData data)
        {
            return exceptionHandling(delegate
            {
                return Instance.MapExercises(Token, data);
            });
        }

        public static void SendMessage(MessageDTO msg)
        {
            exceptionHandling(delegate
            {
                Instance.SendMessage(Token, msg);
            });
        }

        public static BlogCommentDTO BlogCommentOperation(BlogCommentOperation arg)
        {
            return exceptionHandling(delegate
            {
                return Instance.BlogCommentOperation(Token, arg);
            });
        }

        public static void MessageOperation(MessageOperationParam arg)
        {
            exceptionHandling(delegate
            {
                Instance.MessageOperation(Token,arg);
            });
        }

        public static PictureInfoDTO UploadImage(PictureDTO picture)
        {
            return exceptionHandling(delegate
                                         {
                                             var dto = new Automatic.PictureDTO();
                                             dto.ImageStream = picture.ImageStream;
                                             dto.PictureId = picture.PictureId;
                                             dto.Hash = picture.Hash;
                                             dto.SessionId = picture.SessionId;
                                             var result = Instance.UploadImage(dto);

                                             PictureInfoDTO info = new PictureInfoDTO(picture);
                                             info.Hash = result.Hash;
                                             info.PictureId = result.PictureId;
                                             return info;
                                         });
        }
        public static SessionData Login(string username, string password)
        {
            return exceptionHandling(delegate
            {
                ClientInformation clientInfo = GetClientInformation();
                return Instance.Login(clientInfo,username, password);
            });
            
        }

        public static FriendInvitationDTO InviteFriendOperation( InviteFriendOperationData data)
        {
            return exceptionHandling(delegate
            {
                return Instance.InviteFriendOperation(Token, data);
            });
        }

        public static  ProfileInformationDTO GetProfileInformation(GetProfileInformationCriteria criteria)
        {
            return exceptionHandling(delegate
                                         {
                                             return Instance.GetProfileInformation(Token, criteria);
                                         });
        }

        public static void DeleteTrainingDay(TrainingDayDTO day)
        {
            exceptionHandling(delegate
                   {
                       Instance.DeleteTrainingDay(Token, day);
                   });
        }
        public static ClientInformation GetClientInformation()
        {
            ClientInformation clientInfo = new ClientInformation();
            clientInfo.ApplicationLanguage = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            clientInfo.ApplicationVersion = Constants.Version;
            clientInfo.Platform = PlatformType.Windows;
            clientInfo.PlatformVersion = Environment.OSVersion.VersionString;
            clientInfo.ClientInstanceId = ClientInstanceId;
            clientInfo.Version = Const.ServiceVersion;

            return clientInfo;
        }

        

        public class SessionDataChangedEventArgs:EventArgs
        {
            public SessionData SessionData { get; set; }
        }

        public static event EventHandler<SessionDataChangedEventArgs> LoginRequired;

        static void exceptionHandling(Action method)
        {
            exceptionHandling<ProfileDTO>(delegate
            {
                method();
                return null;
            });
        }

        static T exceptionHandling<T>(Func<T> method)
        {
            if (Token != null)
            {
                Token.Language = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            }
            try
            {
                bool retry = false;
                do
                {
                    try
                    {
                        return method();

                    }
                    catch (FaultException<BAAuthenticationException> ex)
                    {
                        retry = true;
                        ExceptionHandler.Default.Process(ex);
                        var arg = new SessionDataChangedEventArgs();
                        if (LoginRequired != null)
                        {
                            LoginRequired(null, arg);
                        }
                        if (arg.SessionData == null)
                        {
                            throw;

                        }
                    }
                } while (retry);

                return default(T);
            }
            catch (FaultException<ValidationFault> ex)
            {
                throw new ValidationException(ex.Detail.ToValidationResults());
            }
            catch (FaultException<BAAuthenticationException> baEx)
            {
                throw new AuthenticationException(baEx.Detail.Message);
            }
            catch (FaultException<BAServiceException> baEx)
            {
                switch (baEx.Detail.ErrorCode)
                {
                    case ErrorCode.ProfileIsNotActivatedException:
                        throw new ProfileIsNotActivatedException(baEx.Detail.Message);
                    case ErrorCode.UserDeletedException:
                        throw new UserDeletedException(baEx.Detail.Message);
                    case ErrorCode.MaintenanceException:
                        throw new MaintenanceException(baEx.Detail.Message);
                    case ErrorCode.ProfileDeletedException:
                        throw new ProfileDeletedException(baEx.Detail.Message);
                    case ErrorCode.ConsistencyException:
                        throw new ConsistencyException(baEx.Detail.Message);
                    case ErrorCode.SecurityException:
                        throw new SecurityException(baEx.Detail.Message);
                    case ErrorCode.EMailSendException:
                        throw new EMailSendException(baEx.Detail.Message);
                    case ErrorCode.UniqueException:
                        throw new UniqueException(baEx.Detail.Message);
                    case ErrorCode.CrossProfileOperation:
                        throw new CrossProfileOperationException(baEx.Detail.Message);
                    case ErrorCode.ValidationException:
                        throw new ValidationException(baEx.Detail.Message);
                    case ErrorCode.AuthenticationException:
                        throw new AuthenticationException(baEx.Detail.Message);
                    case ErrorCode.InvalidOperationException:
                        throw new InvalidOperationException(baEx.Detail.Message);
                    case ErrorCode.ArgumentNullException:
                        throw new ArgumentNullException(baEx.Detail.AdditionalData, baEx.Detail.Message);
                    case ErrorCode.ArgumentException:
                        throw new ArgumentException(baEx.Detail.Message);
                    case ErrorCode.DatabaseException:
                        throw new DatabaseException(baEx.Detail.Message);
                    case ErrorCode.NullReferenceException:
                        throw new NullReferenceException(baEx.Detail.Message);
                    case ErrorCode.DatabaseVersionException:
                        throw new DatabaseVersionException(baEx.Detail.Message);
                    case ErrorCode.OldDataException:
                        throw new OldDataException(baEx.Detail.Message);
                    case ErrorCode.CannotAcceptRejectInvitationDoesntExistException:
                        throw new CannotAcceptRejectInvitationDoesntExistException(baEx.Detail.Message);
                    case ErrorCode.ProfileAlreadyFriendException:
                        throw new ProfileAlreadyFriendException(baEx.Detail.Message);
                    case ErrorCode.ObjectIsFavoriteException:
                        throw new ObjectIsFavoriteException(baEx.Detail.Message);
                    case ErrorCode.ObjectIsNotFavoriteException:
                        throw new ObjectIsNotFavoriteException(baEx.Detail.Message);
                    case ErrorCode.ObjectNotFound:
                        throw new ObjectNotFoundException(baEx.Detail.Message);
                    case ErrorCode.UnauthorizedAccessException:
                        throw new UnauthorizedAccessException(baEx.Detail.Message);
                    case ErrorCode.TrainingIntegrityException:
                        throw new TrainingIntegrationException(baEx.Detail.Message);
                    case ErrorCode.FileNotFoundException:
                        throw new FileNotFoundException(baEx.Detail.Message);

                }
                throw new Exception(baEx.Detail.Message);
            }

        }

        public static SessionData CreateProfile(ProfileDTO newProfile)
        {
            return exceptionHandling(delegate
                                         {
                                             ClientInformation clientInfo = GetClientInformation();
                                             return Instance.CreateProfile(clientInfo,newProfile);
                                         });
           
            
        }

        public static PagedResult<ExerciseDTO> GetExercises(ExerciseSearchCriteria searchCriteria,PartialRetrievingInfo pageInfo)
        {
            return exceptionHandling(delegate
                                         {
                                             pageInfo.PageSize = GetPageSize(150);
                                             return Instance.GetExercises(Token, searchCriteria, pageInfo);
                                         });
        }

        public static PagedResult<SuplementDTO> GetSuplements(PartialRetrievingInfo pageInfo)
        {
            return exceptionHandling(delegate
                                         {
                                             return Instance.GetSuplements(Token,pageInfo);
                                         });
        }

        public static ProfileDTO UpdateProfile( ProfileUpdateData data)
        {
            return exceptionHandling(delegate
                                         {
                                             return Instance.UpdateProfile(Token, data);
                                         });

        }

        public static bool CheckProfileNameAvailability(string userName)
        {
            return exceptionHandling(delegate
                                         {
                                             return Instance.CheckProfileNameAvailability(userName);
                                         });
        }

        public static TrainingDayDTO SaveTrainingDay( TrainingDayDTO day)
        {
            return exceptionHandling(delegate
                                         {
                                             return Instance.SaveTrainingDay(Token, day);
                                         });
        }

        public static PagedResult<TrainingDayDTO> GetTrainingDays( WorkoutDaysSearchCriteria searchCriteria, PartialRetrievingInfo pageInfo)
        {
            return exceptionHandling(delegate
                                         {
                                             return Instance.GetTrainingDays(Token, searchCriteria, pageInfo);
                                         });
        }

        public static TrainingDayDTO GetTrainingDay(WorkoutDayGetOperation operation)
        {
            return exceptionHandling(delegate
                                         {
                                             return Instance.GetTrainingDay(Token, operation, new RetrievingInfo());
                                         });
        }

        public static PictureDTO GetImage(PictureInfoDTO pictureInfo)
        {
            return exceptionHandling(delegate
                                         {
                                             BodyArchitect.WCF.Automatic.PictureInfoDTO info =
                                                 new Automatic.PictureInfoDTO();
                                             info.Hash = pictureInfo.Hash;
                                             info.PictureId = pictureInfo.PictureId;
                                             info.SessionId = Token.SessionId;
                                             var result = Instance.GetImage(info);
                                             PictureDTO dto = new PictureDTO(pictureInfo);
                                             dto.ImageStream = result.ImageStream;
                                             dto.PictureId = result.PictureId;
                                             dto.Hash = result.Hash;
                                             return dto;
                                         });
        }

        public static void UserFavoritesOperation(UserDTO userDto, FavoriteOperation operation)
        {
            exceptionHandling(delegate
            {
                Instance.UserFavoritesOperation(Token, userDto, operation);
            });
        }


        public static ExerciseDTO SaveExercise( ExerciseDTO exerciseDto)
        {
            return exceptionHandling(delegate
                                         {
                                             return Instance.SaveExercise(Token, exerciseDto);
                                         });
        }

        public static SuplementDTO SaveSuplement( SuplementDTO suplementDto)
        {
            return exceptionHandling(delegate
                                         {
                                             return Instance.SaveSuplement(Token, suplementDto);
                                         });
        }

        public static IList<MyTrainingDTO> GetStartedTrainings(Guid? typeId)
        {
            return exceptionHandling(delegate
                                         {
                                             return Instance.GetStartedTrainings(Token, typeId, new RetrievingInfo());
                                         });
        }

        public static PagedResult<WorkoutPlanDTO> GetWorkoutPlans( WorkoutPlanSearchCriteria searchCriteria,PartialRetrievingInfo pagerInfo)
        {
            return exceptionHandling(delegate
                                         {
                                             pagerInfo.LongTexts = false;
                                             return Instance.GetWorkoutPlans(Token, searchCriteria, pagerInfo);
                                         });
        }

        public static WorkoutPlanDTO GetTrainingPlan( Guid planId)
        {
            return exceptionHandling(delegate
                                         {
                                             return Instance.GetWorkoutPlan(Token, planId, new RetrievingInfo());
                                         });
        }

        public static WorkoutPlanDTO SaveTrainingPlan( WorkoutPlanDTO dto)
        {
            return exceptionHandling(delegate
                                         {
                                             return Instance.SaveWorkoutPlan(Token, dto);
                                         });
        }

        public static void Logout(Token token)
        {
            try
            {
                Instance.Logout(Token);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex);
            }
        }

        public static WorkoutPlanDTO VoteWorkoutPlan(WorkoutPlanDTO dto)
        {
            return exceptionHandling(delegate
                                         {
                                             return Instance.VoteWorkoutPlan(Token, dto);
                                         });
        }

        public static ExerciseDTO VoteExercise( ExerciseDTO dto)
        {
            return exceptionHandling(delegate
                                         {
                                             return Instance.VoteExercise(Token, dto);
                                         });
        }

        public static PagedResult<CommentEntryDTO> GetComments( IRatingable ratingable, PartialRetrievingInfo retrievingInfo)
        {
            return exceptionHandling(delegate
                                         {
                                             retrievingInfo.PageSize = GetPageSize(retrievingInfo.PageSize);
                                             return Instance.GetComments(Token, ratingable.GlobalId, retrievingInfo);
                                         });
        }

        public static void DeleteExercise( ExerciseDTO dto)
        {
            exceptionHandling(delegate
                                         {
                                             Instance.DeleteExercise(Token, dto);
                                         });
        }

        public static void DeleteWorkoutPlan(WorkoutPlanDTO dto)
        {
            exceptionHandling(delegate
                                         {
                                             Instance.DeleteWorkoutPlan(Token, dto);
                                         });
        }

        public static void WorkoutPlanFavoritesOperation( WorkoutPlanDTO planDto, FavoriteOperation operation)
        {
            exceptionHandling(delegate
                                         {
                                             Instance.WorkoutPlanFavoritesOperation(Token, planDto, operation);
                                         });
        }

        public static ExerciseDTO PublishExercise( ExerciseDTO exerciseDto)
        {
            return exceptionHandling(delegate
                                         {
                                             return Instance.PublishExercise(Token, exerciseDto);
                                         });
        }

        public static WorkoutPlanDTO PublishWorkoutPlan( WorkoutPlanDTO planDto)
        {
            return exceptionHandling(delegate
                                         {
                                             return Instance.PublishWorkoutPlan(Token, planDto);
                                         });
        }

        public static PagedResult<UserSearchDTO> GetUsers( UserSearchCriteria searchCriteria, PartialRetrievingInfo pagerInfo)
        {
            return exceptionHandling(delegate
                                         {
                                             pagerInfo.PageSize = GetPageSize(40);
                                             return Instance.GetUsers(Token, searchCriteria, pagerInfo);
                                         });
        }
    }
}
