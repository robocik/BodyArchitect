using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Authentication;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using BodyArchitect.Client.WCF.Automatic;
using BodyArchitect.Logger;
using BodyArchitect.Portable;
using BodyArchitect.Portable.Exceptions;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Service.V2.Model.Reports;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF;
using Newtonsoft.Json;
using AccountOperationType = BodyArchitect.Service.V2.Model.AccountOperationType;
using ActivityDTO = BodyArchitect.Service.V2.Model.Instructor.ActivityDTO;
using BAAuthenticationException = BodyArchitect.Service.V2.Model.Exceptions.BAAuthenticationException;
using BAServiceException = BodyArchitect.Service.V2.Model.Exceptions.BAServiceException;
using BlogEntryDTO = BodyArchitect.Service.V2.Model.BlogEntryDTO;
using ClientInformation = BodyArchitect.Service.V2.Model.ClientInformation;
using CommentEntryDTO = BodyArchitect.Service.V2.Model.CommentEntryDTO;
using CustomerDTO = BodyArchitect.Service.V2.Model.Instructor.CustomerDTO;
using CustomerGroupDTO = BodyArchitect.Service.V2.Model.Instructor.CustomerGroupDTO;
using CustomerGroupSearchCriteria = BodyArchitect.Service.V2.Model.Instructor.CustomerGroupSearchCriteria;
using CustomerSearchCriteria = BodyArchitect.Service.V2.Model.Instructor.CustomerSearchCriteria;
using DeleteTrainingDayParam = BodyArchitect.Service.V2.Model.DeleteTrainingDayParam;
using ErrorCode = BodyArchitect.Service.V2.Model.Exceptions.ErrorCode;
using ExerciseDTO = BodyArchitect.Service.V2.Model.ExerciseDTO;
using ExerciseOperationParam = BodyArchitect.Service.V2.Model.ExerciseOperationParam;
using ExerciseSearchCriteria = BodyArchitect.Service.V2.Model.ExerciseSearchCriteria;
using FavoriteOperation = BodyArchitect.Service.V2.Model.FavoriteOperation;
using FriendInvitationDTO = BodyArchitect.Service.V2.Model.FriendInvitationDTO;
using GetMyTrainingsParam = BodyArchitect.Service.V2.Model.GetMyTrainingsParam;
using GetPaymentBasketParam = BodyArchitect.Service.V2.Model.Instructor.GetPaymentBasketParam;
using GetProductsParam = BodyArchitect.Service.V2.Model.Instructor.GetProductsParam;
using GetProfileInformationCriteria = BodyArchitect.Service.V2.Model.GetProfileInformationCriteria;
using GetRemindersParam = BodyArchitect.Service.V2.Model.GetRemindersParam;
using GetScheduleEntriesParam = BodyArchitect.Service.V2.Model.Instructor.GetScheduleEntriesParam;
using GetSupplementsCycleDefinitionsParam = BodyArchitect.Service.V2.Model.GetSupplementsCycleDefinitionsParam;
using GetSupplementsParam = BodyArchitect.Service.V2.Model.GetSupplementsParam;
using InviteFriendOperationData = BodyArchitect.Service.V2.Model.InviteFriendOperationData;
using MessageDTO = BodyArchitect.Service.V2.Model.MessageDTO;
using MessageOperationParam = BodyArchitect.Service.V2.Model.MessageOperationParam;
using MyTrainingDTO = BodyArchitect.Service.V2.Model.MyTrainingDTO;
using MyTrainingOperationParam = BodyArchitect.Service.V2.Model.MyTrainingOperationParam;
using PartialRetrievingInfo = BodyArchitect.Service.V2.Model.PartialRetrievingInfo;
using PaymentBasketDTO = BodyArchitect.Service.V2.Model.Instructor.PaymentBasketDTO;
using PictureDTO = BodyArchitect.Service.V2.Model.PictureDTO;
using PictureInfoDTO = BodyArchitect.Service.V2.Model.PictureInfoDTO;
using PlatformType = BodyArchitect.Service.V2.Model.PlatformType;
using ProductInfoDTO = BodyArchitect.Service.V2.Model.Instructor.ProductInfoDTO;
using ProfileDTO = BodyArchitect.Service.V2.Model.ProfileDTO;
using ProfileInformationDTO = BodyArchitect.Service.V2.Model.ProfileInformationDTO;
using ProfileUpdateData = BodyArchitect.Service.V2.Model.ProfileUpdateData;
using ReminderItemDTO = BodyArchitect.Service.V2.Model.ReminderItemDTO;
using ReminderOperationParam = BodyArchitect.Service.V2.Model.ReminderOperationParam;
using ReservationOperationResult = BodyArchitect.Service.V2.Model.Instructor.ReservationOperationResult;
using ReservationsOperationParam = BodyArchitect.Service.V2.Model.Instructor.ReservationsOperationParam;
using RetrievingInfo = BodyArchitect.Service.V2.Model.RetrievingInfo;
using SaveScheduleEntryRangeParam = BodyArchitect.Service.V2.Model.Instructor.SaveScheduleEntryRangeParam;
using ScheduleEntryDTO = BodyArchitect.Service.V2.Model.Instructor.ScheduleEntryDTO;
using SessionData = BodyArchitect.Service.V2.Model.SessionData;
using SuplementDTO = BodyArchitect.Service.V2.Model.SuplementDTO;
using SupplementCycleDefinitionDTO = BodyArchitect.Service.V2.Model.SupplementCycleDefinitionDTO;
using SupplementsCycleDefinitionOperationParam = BodyArchitect.Service.V2.Model.SupplementsCycleDefinitionOperationParam;
using Token = BodyArchitect.Service.V2.Model.Token;
using TrainingDayDTO = BodyArchitect.Service.V2.Model.TrainingDayDTO;
using TrainingPlan = BodyArchitect.Service.V2.Model.TrainingPlans.TrainingPlan;
using UserDTO = BodyArchitect.Service.V2.Model.UserDTO;
using UserSearchCriteria = BodyArchitect.Service.V2.Model.UserSearchCriteria;
using UserSearchDTO = BodyArchitect.Service.V2.Model.UserSearchDTO;
using VoteParams = BodyArchitect.Service.V2.Model.VoteParams;
using VoteResult = BodyArchitect.Service.V2.Model.VoteResult;




namespace BodyArchitect.Client.WCF
{
    public class ServiceManager
    {
        private static string serverAddress;

        static string getEndpoint()
        {
            string endpoint = "production";
#if DEBUG
            endpoint = "test";
#endif
            return endpoint;
        }

        public static string ServerAddress
        {
            get
            {
                if(serverAddress==null)
                {

                    var service = new BodyArchitect.Client.WCF.Automatic.BodyArchitectAccessServiceClient(getEndpoint());
                    //get address and remove the last part (we want to only main address to the server)
                    serverAddress = service.Endpoint.Address.Uri.ToString().Replace("V2/BodyArchitect.svc","");

                }
                return serverAddress;
            }
        }
        public static Guid ClientInstanceId { get; set; }

        public static Token Token { get;
            set;
        }

        public static int GetPageSize(int defaultSize)
        {
            int size = -1;
            if(int.TryParse(ConfigurationManager.AppSettings["PageSize"], out size))
            {
                return size;
            }
            return defaultSize;
        }

        public static IBodyArchitectAccessService Instance
        {
            get
            {
                var service = new BodyArchitect.Client.WCF.Automatic.BodyArchitectAccessServiceClient(getEndpoint());
                return service;
            }
        }

        public static void AccountOperation(string userOrEmail,AccountOperationType type)
        {
            exceptionHandling(() => Instance.AccountOperation(userOrEmail, type));
        }

        public static void ProfileOperation(ProfileOperationParam msg)
        {
            exceptionHandling(() => Instance.ProfileOperation(Token, msg));
        }

        public static PagedResult<TrainingDayCommentDTO> GetBlogComments( TrainingDayInfoDTO day, PartialRetrievingInfo info)
        {
            return exceptionHandling(delegate
            {
                info.PageSize = GetPageSize(info.PageSize);
                return Instance.GetTrainingDayComments(Token, day, info);
            });
        }

        public static void SendMessage(MessageDTO msg)
        {
            exceptionHandling(() => Instance.SendMessage(Token, msg));
        }

        public static TrainingDayCommentDTO TrainingDayCommentOperation(TrainingDayCommentOperationParam arg)
        {
            return exceptionHandling(delegate
            {
                return Instance.TrainingDayCommentOperation(Token, arg);
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

        public static void DeleteTrainingDay(DeleteTrainingDayParam param)
        {
            exceptionHandling(delegate
                   {
                       Instance.DeleteTrainingDay(Token, param);
                   });
        }
        public static ClientInformation GetClientInformation()
        {
            ClientInformation clientInfo = new ClientInformation();
            clientInfo.ApplicationLanguage = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            clientInfo.ApplicationVersion = Portable.Constants.Version;
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
                throw new Portable.Exceptions.AuthenticationException(baEx.Detail.Message);
            }
            catch (FaultException<BAServiceException> baEx)
            {
                switch (baEx.Detail.ErrorCode)
                {
                    case ErrorCode.ProfileRankException:
                        throw new ProfileRankException(baEx.Detail.Message);
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
                    case ErrorCode.DeleteConstraintException:
                        throw new DeleteConstraintException(baEx.Detail.Message);
                    case ErrorCode.UniqueException:
                        throw new UniqueException(baEx.Detail.Message);
                    case ErrorCode.CrossProfileOperation:
                        throw new CrossProfileOperationException(baEx.Detail.Message);
                    case ErrorCode.ValidationException:
                        throw new ValidationException(baEx.Detail.Message);
                    case ErrorCode.AuthenticationException:
                        throw new Portable.Exceptions.AuthenticationException(baEx.Detail.Message);
                    case ErrorCode.InvalidOperationException:
                        throw new InvalidOperationException(baEx.Detail.Message);
                    case ErrorCode.ArgumentNullException:
                        throw new ArgumentNullException(baEx.Detail.AdditionalData, baEx.Detail.Message);
                    case ErrorCode.ArgumentOutOfRange:
                        throw new ArgumentOutOfRangeException(baEx.Detail.AdditionalData, baEx.Detail.Message);
                    case ErrorCode.ProductAlreadyPaid:
                        throw new ProductAlreadyPaidException( baEx.Detail.Message);
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
                    case ErrorCode.AlreadyOccupied:
                        throw new AlreadyOccupiedException(baEx.Detail.Message);
                    case ErrorCode.LicenceException:
                        throw new LicenceException(baEx.Detail.Message);

                }
                throw new Exception(baEx.Detail.Message);
            }

        }

        public static SessionData CreateProfile(ProfileDTO newProfile)
        {
            return exceptionHandling(delegate
                                         {
                                             ClientInformation clientInfo = GetClientInformation();
                                             return Instance.CreateProfile(clientInfo, newProfile);
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
                                            var param = new GetSupplementsParam();
                                            param.SortOrder = SearchSortOrder.Name;
                                            return Instance.GetSuplements(Token, param, pageInfo);
                                         });
        }

        public static ProfileDTO UpdateProfile( ProfileUpdateData data)
        {
            return exceptionHandling(() => Instance.UpdateProfile(Token, data));

        }

        public static bool CheckProfileNameAvailability(string userName)
        {
            return exceptionHandling(() => Instance.CheckProfileNameAvailability(userName));
        }

        public static SaveTrainingDayResult SaveTrainingDay( TrainingDayDTO day)
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
                                             BodyArchitect.Client.WCF.Automatic.PictureInfoDTO info =
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

        public static PagedResult<TrainingPlan> GetWorkoutPlans(WorkoutPlanSearchCriteria searchCriteria, PartialRetrievingInfo pagerInfo)
        {
            return exceptionHandling(delegate
                                         {
                                             pagerInfo.LongTexts = false;
                                             return Instance.GetWorkoutPlans(Token, searchCriteria, pagerInfo);
                                         });
        }

        //public static TrainingPlan GetTrainingPlan(Guid planId)
        //{
        //    return exceptionHandling(delegate
        //                                 {
        //                                     return Instance.GetWorkoutPlan(Token, planId, new RetrievingInfo());
        //                                 });
        //}

        public static TrainingPlan SaveTrainingPlan(TrainingPlan dto)
        {
            return exceptionHandling(() => Instance.SaveWorkoutPlan(Token, dto));
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

        public static VoteResult Vote(VoteParams param)
        {
            return exceptionHandling(() => Instance.Vote(Token, param));
        }


        public static PagedResult<CommentEntryDTO> GetComments( IRatingable ratingable, PartialRetrievingInfo retrievingInfo)
        {
            return exceptionHandling(delegate
                                         {
                                             retrievingInfo.PageSize = GetPageSize(retrievingInfo.PageSize);
                                             return Instance.GetComments(Token, ratingable.GlobalId, retrievingInfo);
                                         });
        }



        public static TrainingPlan WorkoutPlanOperation(WorkoutPlanOperationParam param)
        {
            return exceptionHandling(() => Instance.WorkoutPlanOperation(Token, param));
        }


        public static PagedResult<UserSearchDTO> GetUsers( UserSearchCriteria searchCriteria, PartialRetrievingInfo pagerInfo)
        {
            return exceptionHandling(() =>
                                         {
                                             pagerInfo.PageSize = GetPageSize(40);
                                             return Instance.GetUsers(Token, searchCriteria, pagerInfo);
                                         });
        }

        public static PagedResult<ActivityDTO> GetActivities()
        {
            return exceptionHandling(delegate
            {
                PartialRetrievingInfo pagerInfo = new PartialRetrievingInfo();
                pagerInfo.PageSize = -1;
                return Instance.GetActivities(Token, pagerInfo);
            });
        }

        public static ActivityDTO SaveActivity(ActivityDTO activity)
        {
            return exceptionHandling(delegate
            {
                return Instance.SaveActivity(Token, activity);
            });
        }

        public static void DeleteActivity(ActivityDTO activity)
        {
            exceptionHandling(delegate
            {
                Instance.DeleteActivity(Token, activity);
            });
        }

        public static PagedResult<ScheduleEntryBaseDTO> GetScheduleEntries(GetScheduleEntriesParam getScheduleEntriesParam, PartialRetrievingInfo retrievingInfo)
        {
            return exceptionHandling(delegate
            {
                retrievingInfo.PageSize = GetPageSize(retrievingInfo.PageSize);
                return Instance.GetScheduleEntries(Token, getScheduleEntriesParam, retrievingInfo);
            });
        }

        public static IList<ScheduleEntryBaseDTO> SaveScheduleEntriesRange(SaveScheduleEntryRangeParam scheduleEntriesRange)
        {
            return exceptionHandling(delegate
            {
                return Instance.SaveScheduleEntriesRange(Token, scheduleEntriesRange);
            });
        }

        public static PagedResult<CustomerDTO> GetCustomers(CustomerSearchCriteria customerSearchCriteria)
        {
            return exceptionHandling(delegate
            {
                PartialRetrievingInfo pagerInfo = new PartialRetrievingInfo();
                pagerInfo.PageSize = -1;
                return Instance.GetCustomers(Token, customerSearchCriteria, pagerInfo);
            });
        }

        public static CustomerDTO SaveCustomer(CustomerDTO customer)
        {
            return exceptionHandling(delegate
            {
                return Instance.SaveCustomer(Token, customer);
            });
        }

        public static void DeleteCustomer(CustomerDTO customer)
        {
            exceptionHandling(delegate
            {
                Instance.DeleteCustomer(Token, customer);
            });
        }

        public static ReservationOperationResult ReservationsOperation(ReservationsOperationParam param)
        {
            return exceptionHandling(() => Instance.ReservationsOperation(Token, param));
        }

        public static PaymentBasketDTO PaymentBasketOperation( PaymentBasketDTO koszyk)
        {
            return exceptionHandling(() => Instance.PaymentBasketOperation(Token, koszyk));
        }

        public static PagedResult<PaymentBasketDTO> GetPaymentBaskets( GetPaymentBasketParam getPaymentBasketParam, PartialRetrievingInfo retrievingInfo)
        {
            return exceptionHandling(() => Instance.GetPaymentBaskets(Token, getPaymentBasketParam, retrievingInfo));
        }

        public static CustomerGroupDTO SaveCustomerGroup( CustomerGroupDTO customerGroup)
        {
            return exceptionHandling(() => Instance.SaveCustomerGroup(Token, customerGroup));
        }

        public static PagedResult<CustomerGroupDTO> GetCustomerGroups( CustomerGroupSearchCriteria criteria, PartialRetrievingInfo pageInfo)
        {
            return exceptionHandling(() => Instance.GetCustomerGroups(Token, criteria, pageInfo));
        }

        public static void DeleteCustomerGroup(CustomerGroupDTO group)
        {
            exceptionHandling(() => Instance.DeleteCustomerGroup(Token, @group));
        }

        public static PagedResult<ProductInfoDTO> GetProducts(GetProductsParam criteria, PartialRetrievingInfo pageInfo)
        {
            return exceptionHandling(() => Instance.GetProducts(Token, criteria, pageInfo));
        }

        public static ReminderItemDTO SaveReminder( ReminderItemDTO reminder)
        {
            return exceptionHandling(() => Instance.SaveReminder(Token, reminder));
        }

        public static ReminderItemDTO ReminderOperation(ReminderOperationParam param)
        {
            return exceptionHandling(() => Instance.ReminderOperation(Token, param));
        }

        public static PagedResult<ReminderItemDTO> GetReminders(GetRemindersParam remindersParam, PartialRetrievingInfo pageInfo)
        {
            return exceptionHandling(() => Instance.GetReminders(Token, remindersParam, pageInfo));
        }

        public static PagedResult<SupplementCycleDefinitionDTO> GetSupplementsCycleDefinitions( GetSupplementsCycleDefinitionsParam param, PartialRetrievingInfo pageInfo)
        {
            return exceptionHandling(() => Instance.GetSupplementsCycleDefinitions(Token, param, pageInfo));
        }

        public static SupplementCycleDefinitionDTO SaveSupplementsCycleDefinition(SupplementCycleDefinitionDTO definition)
        {
            return exceptionHandling(() => Instance.SaveSupplementsCycleDefinition(Token, definition));
        }

        public static SupplementCycleDefinitionDTO SupplementsCycleDefinitionOperation(SupplementsCycleDefinitionOperationParam definition)
        {
            return exceptionHandling(() => Instance.SupplementsCycleDefinitionOperation(Token, definition));
        }

        public static MyTrainingDTO MyTrainingOperation(MyTrainingOperationParam param)
        {
            return exceptionHandling(() => Instance.MyTrainingOperation(Token, param));
        }

        public static PagedResult<MyTrainingDTO> GetMyTrainings(GetMyTrainingsParam param, PartialRetrievingInfo pageInfo)
        {
            return exceptionHandling(() => Instance.GetMyTrainings(Token, param, pageInfo));
        }

        public static void ExerciseOperation(ExerciseOperationParam param)
        {
            exceptionHandling(() => Instance.ExerciseOperation(Token, param));
        }

        public static IList<WeightExerciseReportResultItem> ReportExerciseWeight(ReportExerciseWeightParams param)
        {
            return exceptionHandling(() => Instance.ReportExerciseWeight(Token, param));
        }

        public static IList<WeightReperitionReportResultItem> ReportWeightRepetitions(ReportWeightRepetitionsParams param)
        {
            return exceptionHandling(() => Instance.ReportWeightRepetitions(Token, param));
        }

        public static IList<MeasurementsTimeReportResultItem> ReportMeasurementsTime(ReportMeasurementsTimeParams param)
        {
            return exceptionHandling(() => Instance.ReportMeasurementsTime(Token, param));
        }

        public static PagedResult<MessageDTO> GetMessages(GetMessagesCriteria param, PartialRetrievingInfo pageInfo)
        {
            return exceptionHandling(() => Instance.GetMessages(Token, param, pageInfo));
        }

        public static void ImportLicence(string licenceKey)
        {
            exceptionHandling(() => Instance.ImportLicence(Token, licenceKey));
        }

        public static PagedResult<MyPlaceDTO> GetMyPlaces(MyPlaceSearchCriteria param, PartialRetrievingInfo pageInfo)
        {
            return exceptionHandling(() => Instance.GetMyPlaces(Token, param, pageInfo));
        }

        public static MyPlaceDTO SaveMyPlace(MyPlaceDTO myPlace)
        {
            return exceptionHandling(() => Instance.SaveMyPlace(Token, myPlace));
        }

        public static MyPlaceDTO MyPlaceOperation(MyPlaceOperationParam param)
        {
            return exceptionHandling(() => Instance.MyPlaceOperation(Token, param));
        }

        public static PagedResult<ExerciseRecordsReportResultItem> ReportExerciseRecords(ExerciseRecordsParams param, PartialRetrievingInfo pageInfo)
        {
            return exceptionHandling(() => Instance.ReportExerciseRecords(Token, param,pageInfo));
        }

        public static FeaturedData GetFeaturedData()
        {
            return exceptionHandling(() => Instance.GetFeaturedData(Token,new GetFeaturedDataParam()));
        }

        public static PagedResult<ChampionshipDTO> GetChampionships(GetChampionshipsCriteria criteria, PartialRetrievingInfo retrievingInfo)
        {
            return exceptionHandling(() => Instance.GetChampionships(Token, criteria, retrievingInfo));
        }

        public static SaveChampionshipResult SaveChampionship( ChampionshipDTO championship)
        {
            return exceptionHandling(() => Instance.SaveChampionship(Token, championship));
        }

        public static IList<GPSPoint> GetGPSCoordinates(Guid gpsTrackerEntryId)
        {
            var param = new Automatic.GetGPSCoordinatesParam()
                            {
                                GPSTrackerEntryId =gpsTrackerEntryId,
                                SessionId=Token.SessionId
                            };
            var result = exceptionHandling(() => Instance.GetGPSCoordinates(param));
            var unzipped = result.Stream.FromZip();
            var json = UTF8Encoding.UTF8.GetString(unzipped, 0, unzipped.Length);
            var points = JsonConvert.DeserializeObject<List<GPSPoint>>(json);
            return points.OrderBy(x=>x.Duration).ToList();
        }
    }
}
