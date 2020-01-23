using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Security;
using System.Security.Authentication;
using System.Threading;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.Web.Configuration;
using AutoMapper;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Portable.Exceptions;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Exceptions;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Service.V2.Model.Reports;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using BodyArchitect.Service.V2.Model.WP7;
using BodyArchitect.Service.V2.Services;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF;
using NHibernate;
using NHibernate.Exceptions;
using ObjectNotFoundException = BodyArchitect.Shared.ObjectNotFoundException;
using Profile = BodyArchitect.Model.Profile;
using SessionData = BodyArchitect.Service.V2.Model.SessionData;
using Token = BodyArchitect.Service.V2.Model.Token;
using TrainingPlan = BodyArchitect.Service.V2.Model.TrainingPlans.TrainingPlan;


namespace BodyArchitect.Service.V2
{
    
    [NHibernateContext]
    [ValidationBehavior]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall,IncludeExceptionDetailInFaults=true,ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class BodyArchitectAccessService : IBodyArchitectAccessService
    {
        public static void EnableLog(bool enable)
        {
            Log.EnableExceptionLog = enable;
            Log.EnableStandardLog = enable;
        }
        static BodyArchitectAccessService()
        {
            EnableLog(true);
        }

        private InternalBodyArchitectService service = new InternalBodyArchitectService(NHibernateContext.Current().Session);

        private InternalBodyArchitectService InternalService
        {
            get { return service; }
        }

        public int TestingDelay
        {
            get
            {
                int result = 0;
                if(WebConfigurationManager.AppSettings["TestingDelay"]!=null)
                {
                    int.TryParse(WebConfigurationManager.AppSettings["TestingDelay"], out result);
                }
                return result;
            }
        }

        void testSleep()
        {
            if (TestingDelay > 0)
            {
                Thread.Sleep(TestingDelay);
            }
        }

        public void ProfileOperation(Token token, ProfileOperationParam param)
        {
            exceptionHandling(token, () => InternalService.ProfileOperation(token, param));
        }

        public PagedResult<TrainingDayCommentDTO> GetTrainingDayComments(Token token, TrainingDayInfoDTO day, PartialRetrievingInfo info)
        {
            return exceptionHandling(token, () => InternalService.GetTrainingDayComments(token, day, info));
        }

        public void AccountOperation(string userNameOrEmail, AccountOperationType operationType)
        {
            exceptionHandling(null, () => InternalService.AccountOperation(userNameOrEmail, operationType));
        }


        public void UserFavoritesOperation(Token token, UserDTO userDto, FavoriteOperation operation)
        {
            exceptionHandling(token, () => InternalService.UserFavoritesOperation(token, userDto, operation));
        }

        public void MessageOperation(Token token, MessageOperationParam arg)
        {
            exceptionHandling(token, () => InternalService.MessageOperation(token, arg));
        }

        public TrainingDayCommentDTO TrainingDayCommentOperation(Token token, TrainingDayCommentOperationParam arg)
        {
            return exceptionHandling(token, () => InternalService.TrainingDayCommentOperation(token, arg));
        }

        public void SendMessage(Token token,MessageDTO message)
        {
            exceptionHandling(token, () => InternalService.SendMessage(token, message));
        }

        public FriendInvitationDTO InviteFriendOperation(Token token, InviteFriendOperationData data)
        {
            return exceptionHandling(token, () => InternalService.InviteFriendOperation(token, data));
        }

        public SessionData Login(ClientInformation clientInfo,string username, string password)
        {
            return exceptionHandling(null, () => InternalService.Login(clientInfo, username, password));
        }



        public void Logout(Token token)
        {
            exceptionHandling(token, () => InternalService.Logout(token));
        }

        void exceptionHandling(Token token,Action method)
        {
            exceptionHandling<Profile>(token,delegate
                                           {
                                               method();
                                               return null;
                                           });
        }
        T exceptionHandling<T>(Token token,Func<T> method)
        {
            testSleep();
            ExceptionHandler.Default.EmailFeaturesEnabled = false;
            try
            {
                localizeThread(token);
                return method();
            }
            catch (ProfileRankException ex)
            {
                BAServiceException baEx = new BAServiceException(ErrorCode.ProfileRankException, ex.Message);
                baEx.ErrorId = ExceptionHandler.Default.Process(ex);
                throw new FaultException<BAServiceException>(baEx, baEx.Message);
            }
            catch(MaintenanceException ex)
            {
                BAServiceException baEx = new BAServiceException(ErrorCode.MaintenanceException, ex.Message);
                baEx.ErrorId = ExceptionHandler.Default.Process(ex);
                throw new FaultException<BAServiceException>(baEx, baEx.Message);
            }
            catch (FileNotFoundException ex)
            {
                BAServiceException baEx = new BAServiceException(ErrorCode.FileNotFoundException, ex.Message);
                baEx.ErrorId = ExceptionHandler.Default.Process(ex);
                throw new FaultException<BAServiceException>(baEx, baEx.Message);
            }
                
            catch (ConsistencyException ex)
            {
                BAServiceException baEx = new BAServiceException(ErrorCode.ConsistencyException, ex.Message);
                baEx.ErrorId = ExceptionHandler.Default.Process(ex);
                throw new FaultException<BAServiceException>(baEx, baEx.Message);
            }
            catch (ProfileDeletedException ex)
            {
                BAServiceException baEx = new BAServiceException(ErrorCode.ProfileDeletedException, ex.Message);
                baEx.ErrorId = ExceptionHandler.Default.Process(ex);
                throw new FaultException<BAServiceException>(baEx, baEx.Message);
            }
            catch (UserDeletedException ex)
            {
                BAServiceException baEx = new BAServiceException(ErrorCode.UserDeletedException, ex.Message);
                baEx.ErrorId = ExceptionHandler.Default.Process(ex);
                throw new FaultException<BAServiceException>(baEx, baEx.Message);
            }
            
            catch (ProfileIsNotActivatedException ex)
            {
                BAServiceException baEx = new BAServiceException(ErrorCode.ProfileIsNotActivatedException, ex.Message);
                baEx.ErrorId = ExceptionHandler.Default.Process(ex);
                throw new FaultException<BAServiceException>(baEx, baEx.Message);
            }
            catch (SecurityException ex)
            {
                BAServiceException baEx = new BAServiceException(ErrorCode.SecurityException, ex.Message);
                baEx.ErrorId = ExceptionHandler.Default.Process(ex);
                throw new FaultException<BAServiceException>(baEx, baEx.Message);
            }
            catch (TrainingIntegrationException ex)
            {
                BAServiceException baEx = new BAServiceException(ErrorCode.TrainingIntegrityException, ex.Message);
                baEx.ErrorId = ExceptionHandler.Default.Process(ex);
                throw new FaultException<BAServiceException>(baEx, baEx.Message);
            }
            catch (DeleteConstraintException ex)
            {
                BAServiceException baEx = new BAServiceException(ErrorCode.DeleteConstraintException, ex.Message);
                baEx.ErrorId = ExceptionHandler.Default.Process(ex);
                throw new FaultException<BAServiceException>(baEx, baEx.Message);
            }
            catch (ObjectIsFavoriteException ex)
            {
                BAServiceException baEx = new BAServiceException(ErrorCode.ObjectIsFavoriteException, ex.Message);
                baEx.ErrorId=ExceptionHandler.Default.Process(ex);
                throw new FaultException<BAServiceException>(baEx, baEx.Message);
            }
            catch (AlreadyOccupiedException ex)
            {
                BAServiceException baEx = new BAServiceException(ErrorCode.AlreadyOccupied, ex.Message);
                baEx.ErrorId = ExceptionHandler.Default.Process(ex);
                throw new FaultException<BAServiceException>(baEx, baEx.Message);
            }
            catch (LicenceException ex)
            {
                BAServiceException baEx = new BAServiceException(ErrorCode.LicenceException, ex.Message);
                baEx.ErrorId = ExceptionHandler.Default.Process(ex);
                throw new FaultException<BAServiceException>(baEx, baEx.Message);
            }
            catch (ProductAlreadyPaidException ex)
            {
                BAServiceException baEx = new BAServiceException(ErrorCode.ProductAlreadyPaid, ex.Message);
                baEx.ErrorId = ExceptionHandler.Default.Process(ex);
                throw new FaultException<BAServiceException>(baEx, baEx.Message);
            }
            catch (ObjectIsNotFavoriteException ex)
            {
                BAServiceException baEx = new BAServiceException(ErrorCode.ObjectIsNotFavoriteException, ex.Message);
                baEx.ErrorId = ExceptionHandler.Default.Process(ex);
                throw new FaultException<BAServiceException>(baEx, baEx.Message);
            }
            catch (ProfileAlreadyFriendException ex)
            {
                BAServiceException baEx = new BAServiceException(ErrorCode.ProfileAlreadyFriendException, ex.Message);
                baEx.ErrorId = ExceptionHandler.Default.Process(ex);
                throw new FaultException<BAServiceException>(baEx, baEx.Message);
            }
            catch (CannotAcceptRejectInvitationDoesntExistException ex)
            {
                BAServiceException baEx = new BAServiceException(ErrorCode.CannotAcceptRejectInvitationDoesntExistException, ex.Message);
                baEx.ErrorId = ExceptionHandler.Default.Process(ex);
                throw new FaultException<BAServiceException>(baEx, baEx.Message);
            }
            catch (DatabaseException adoEx)
            {
                BAServiceException baEx = new BAServiceException(ErrorCode.DatabaseException, adoEx.Message);
                baEx.ErrorId = ExceptionHandler.Default.Process(adoEx);
                throw new FaultException<BAServiceException>(baEx, baEx.Message);
            }
            catch (GenericADOException adoEx)
            {
                BAServiceException baEx = new BAServiceException(ErrorCode.DatabaseException, adoEx.Message);
                baEx.ErrorId = ExceptionHandler.Default.Process(adoEx);
                throw new FaultException<BAServiceException>(baEx, baEx.Message);
            }
            catch (DatabaseVersionException adoEx)
            {
                BAServiceException baEx = new BAServiceException(ErrorCode.DatabaseVersionException, adoEx.Message);
                baEx.ErrorId = ExceptionHandler.Default.Process(adoEx);
                throw new FaultException<BAServiceException>(baEx, baEx.Message);
            }
            catch(ObjectNotFoundException ex)
            {
                BAServiceException baEx = new BAServiceException(ErrorCode.ObjectNotFound, ex.Message);
                baEx.ErrorId = ExceptionHandler.Default.Process(ex);
                throw new FaultException<BAServiceException>(baEx, baEx.Message);
            }
            catch (FaultException<BAServiceException>)
            {
                throw;
            }
            catch (Portable.Exceptions.AuthenticationException ex)
            {
                BAAuthenticationException baEx = new BAAuthenticationException( ex.Message);
                baEx.ErrorId = ExceptionHandler.Default.Process(ex);
                throw new FaultException<BAAuthenticationException>(baEx, baEx.Message);
            }
            catch (InvalidOperationException ex)
            {
                BAServiceException baEx = new BAServiceException(ErrorCode.InvalidOperationException, ex.Message);
                baEx.ErrorId = ExceptionHandler.Default.Process(ex);
                throw new FaultException<BAServiceException>(baEx, baEx.Message);
            }
            catch (ArgumentNullException ex)
            {
                BAServiceException baEx = new BAServiceException(ErrorCode.ArgumentNullException, ex.Message);
                baEx.ErrorId = ExceptionHandler.Default.Process(ex);
                baEx.AdditionalData = ex.ParamName;
                throw new FaultException<BAServiceException>(baEx, baEx.Message);
            }
            catch (EMailSendException ex)
            {
                BAServiceException baEx = new BAServiceException(ErrorCode.EMailSendException, ex.Message);
                baEx.ErrorId = ExceptionHandler.Default.Process(ex);
                throw new FaultException<BAServiceException>(baEx, baEx.Message);
            }
            catch (NullReferenceException ex)
            {
                BAServiceException baEx = new BAServiceException(ErrorCode.NullReferenceException, ex.Message);
                baEx.ErrorId = ExceptionHandler.Default.Process(ex);
                throw new FaultException<BAServiceException>(baEx, baEx.Message);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                BAServiceException baEx = new BAServiceException(ErrorCode.ArgumentOutOfRange, ex.Message);
                baEx.ErrorId = ExceptionHandler.Default.Process(ex);
                throw new FaultException<BAServiceException>(baEx, baEx.Message);
            }
            catch (UniqueException uniqEx)
            {
                BAServiceException baEx = new BAServiceException(ErrorCode.UniqueException, uniqEx.Message);
                baEx.ErrorId = ExceptionHandler.Default.Process(uniqEx);
                throw new FaultException<BAServiceException>(baEx, baEx.Message);
            }
            catch (CrossProfileOperationException ex)
            {
                BAServiceException baEx = new BAServiceException(ErrorCode.CrossProfileOperation, ex.Message);
                baEx.ErrorId = ExceptionHandler.Default.Process(ex);
                throw new FaultException<BAServiceException>(baEx, baEx.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                BAServiceException baEx = new BAServiceException(ErrorCode.UnauthorizedAccessException, ex.Message);
                baEx.ErrorId = ExceptionHandler.Default.Process(ex);
                throw new FaultException<BAServiceException>(baEx, baEx.Message);
            }
            catch (ValidationException ex)
            {
                ValidationFault fault = new ValidationFault();
                foreach (var result in ex.Results)
                {
                    ValidationDetail detail = new ValidationDetail(result.Message,result.Key,result.Tag);
                    fault.Add(detail);
                }
                
                throw new FaultException<ValidationFault>(fault, ex.Message);
            }
            catch (ArgumentException ex)
            {
                BAServiceException baEx = new BAServiceException(ErrorCode.ArgumentException, ex.Message);
                baEx.ErrorId = ExceptionHandler.Default.Process(ex);
                throw new FaultException<BAServiceException>(baEx, baEx.Message);
            }
            catch (StaleObjectStateException ex)
            {
                BAServiceException baEx = new BAServiceException(ErrorCode.OldDataException, "You are trying to change old data. In the database there is a newer version so please take the latest version and try again");
                baEx.ErrorId = ExceptionHandler.Default.Process(ex);
                throw new FaultException<BAServiceException>(baEx, baEx.Message);
            }
            catch (Exception ex)
            {
                BAServiceException baEx = new BAServiceException(ErrorCode.UnexpectedException, ex.Message);
                baEx.ErrorId = ExceptionHandler.Default.Process(ex);
                throw new FaultException<BAServiceException>(baEx, baEx.Message);
            }
        }

        public ProfileInformationDTO GetProfileInformation(Token token,GetProfileInformationCriteria criteria)
        {
            return exceptionHandling(token, () => InternalService.GetProfileInformation(token, criteria));
        }

        public SessionData CreateProfile(ClientInformation clientInfo,ProfileDTO newProfile)
        {
            return exceptionHandling(null, () => InternalService.CreateProfile(clientInfo, newProfile));
        }

        public ProfileDTO UpdateProfile(Token token, ProfileUpdateData data)
        {
            return exceptionHandling(token, () => InternalService.UpdateProfile(token, data));
        }

        public PictureDTO GetImage(PictureInfoDTO pictureInfo)
        {
            return exceptionHandling(null, () => InternalService.GetImage(pictureInfo));
        }

        public PagedResult<ExerciseDTO> GetExercises(Token token, ExerciseSearchCriteria searchCriteria, PartialRetrievingInfo retrievingInfo)
        {
            return exceptionHandling(token, () => InternalService.GetExercises(token, searchCriteria, retrievingInfo));
        }

        private void localizeThread(Token token)
        {
            if (token != null && !string.IsNullOrEmpty(token.Language))
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(token.Language);
            }
        }

        public PagedResult<SuplementDTO> GetSuplements(Token token,GetSupplementsParam param, PartialRetrievingInfo retrievingInfo)
        {
            return exceptionHandling(token, () => InternalService.GetSuplements(token, param, retrievingInfo));
        }

        public ExerciseDTO SaveExercise(Token token, ExerciseDTO exercise)
        {
            return exceptionHandling(token, () => InternalService.SaveExercise(token, exercise));
        }

        public SuplementDTO SaveSuplement(Token token, SuplementDTO suplement)
        {
            return exceptionHandling(token, () => InternalService.SaveSuplement(token, suplement));
        }
        
        public PictureInfoDTO UploadImage(PictureDTO pictureDto)
        {
            return exceptionHandling(null, () => InternalService.UploadImage(pictureDto));
        }


        public bool CheckProfileNameAvailability(string userName)
        {
            return exceptionHandling(null, () => InternalService.CheckProfileNameAvailability(userName));
        }

        public SaveTrainingDayResult SaveTrainingDay(Token token, TrainingDayDTO day)
        {
            return exceptionHandling(token, () => InternalService.SaveTrainingDay(token, day));
        }

        public PagedResult<TrainingDayDTO> GetTrainingDays(Token token, WorkoutDaysSearchCriteria searchCriteria, PartialRetrievingInfo retrievingInfo)
        {
            return exceptionHandling(token, () => InternalService.GetTrainingDays(token, searchCriteria, retrievingInfo));
        }
        
        public TrainingDayDTO GetTrainingDay(Token token, WorkoutDayGetOperation operationParams, RetrievingInfo retrievingInfo)
        {
            return exceptionHandling(token, () => InternalService.GetTrainingDay(token, operationParams, retrievingInfo));
        }
        
        //public TrainingPlan GetWorkoutPlan(Token token, Guid planId, RetrievingInfo retrievingInfo)
        //{
        //    return exceptionHandling(token, () => InternalService.GetWorkoutPlan(token, planId, retrievingInfo));
        //}

        public void DeleteTrainingDay(Token token, DeleteTrainingDayParam dayDto)
        {
            exceptionHandling(token, () => InternalService.DeleteTrainingDay(token, dayDto));
        }


        //public void DeleteWorkoutPlan(Token token, TrainingPlanInfo dto)
        //{
        //    exceptionHandling(token, () => InternalService.DeleteWorkoutPlan(token, dto));
        //}


        public TrainingPlan SaveWorkoutPlan(Token token, TrainingPlan dto)
        {
            return exceptionHandling(token, () => InternalService.SaveWorkoutPlan(token, dto));
        }

        public PagedResult<UserSearchDTO> GetUsers(Token token, UserSearchCriteria searchCriteria, PartialRetrievingInfo pagerInfo)
        {
            return exceptionHandling(token, () => InternalService.GetUsers(token, searchCriteria, pagerInfo));
        }

        public string WP7Register(string deviceid, string uri, Guid profileId)
        {
            return exceptionHandling(null, () => InternalService.WP7Register(deviceid, uri, profileId));
        }

        public TrialStatusInfo WP7TrialStatus(string deviceId)
        {
            return exceptionHandling(null, () => InternalService.WP7TrialStatus(deviceId));
        }

        public PagedResult<ActivityDTO> GetActivities(Token token, PartialRetrievingInfo retrievingInfo)
        {
            return exceptionHandling(token, () => InternalService.GetActivities(token, retrievingInfo));
        }

        public void DeleteActivity(Token token, ActivityDTO activity)
        {
            exceptionHandling(token, () => InternalService.DeleteActivity(token, activity));
        }

        public ActivityDTO SaveActivity(Token token, ActivityDTO activity)
        {
            return exceptionHandling(token, () => InternalService.SaveActivity(token, activity));
        }

        public void WP7Unregister(string deviceid)
        {
            exceptionHandling(null, () => InternalService.WP7Unregister(deviceid));
        }

        public void WP7ClearCounter(string deviceid)
        {
            exceptionHandling(null, () => InternalService.WP7ClearCounter(deviceid));
        }

        public PagedResult<TrainingPlan> GetWorkoutPlans(Token token, WorkoutPlanSearchCriteria searchCriteria, PartialRetrievingInfo pagerInfo)
        {
            return exceptionHandling(token, () => InternalService.GetWorkoutPlans(token, searchCriteria, pagerInfo));
        }


        public PagedResult<CommentEntryDTO> GetComments(Token token, Guid globalId, PartialRetrievingInfo retrievingInfo)
        {
            return exceptionHandling(token, () => InternalService.GetComments(token, globalId, retrievingInfo));
        }


        public TrainingPlan WorkoutPlanOperation(Token token, WorkoutPlanOperationParam param)
        {
            return exceptionHandling(token, () =>
                                         {
                                             return InternalService.WorkoutPlanOperation(token, param);
                                         });
        }


        public PagedResult<ScheduleEntryBaseDTO> GetScheduleEntries(Token token, GetScheduleEntriesParam getScheduleEntriesParam, PartialRetrievingInfo retrievingInfo)
        {
            return exceptionHandling(token, () => InternalService.GetScheduleEntries(token, getScheduleEntriesParam, retrievingInfo));
        }

        public IList<ScheduleEntryBaseDTO> SaveScheduleEntriesRange(Token token, SaveScheduleEntryRangeParam saveScheduleEntryRangeParam)
        {
            return exceptionHandling(token, () => InternalService.SaveScheduleEntriesRange(token, saveScheduleEntryRangeParam));
        }

        public CustomerDTO SaveCustomer(Token token, CustomerDTO customerDto)
        {
            return exceptionHandling(token, () => InternalService.SaveCustomer(token, customerDto));
        }

        public void DeleteCustomer(Token token, CustomerDTO customerDto)
        {
            exceptionHandling(token, () => InternalService.DeleteCustomer(token, customerDto));
        }

        public PagedResult<CustomerDTO> GetCustomers(Token token,CustomerSearchCriteria customerSearchCriteria, PartialRetrievingInfo retrievingInfo)
        {
            return exceptionHandling(token,() => InternalService.GetCustomers(token, customerSearchCriteria, retrievingInfo));
        }

        public ReservationOperationResult ReservationsOperation(Token token, ReservationsOperationParam param)
        {
            return exceptionHandling(token, () => InternalService.ReservationsOperation(token, param));
        }

        public PaymentBasketDTO PaymentBasketOperation(Token token, PaymentBasketDTO koszyk)
        {
            return exceptionHandling(token, () => InternalService.PaymentBasketOperation(token, koszyk));
        }

        public PagedResult<PaymentBasketDTO> GetPaymentBaskets(Token token, GetPaymentBasketParam getPaymentBasketParam, PartialRetrievingInfo retrievingInfo)
        {
            return exceptionHandling(token,() =>InternalService.GetPaymentBaskets(token, getPaymentBasketParam, retrievingInfo));
        }

        public CustomerGroupDTO SaveCustomerGroup(Token token, CustomerGroupDTO customerGroup)
        {
            return exceptionHandling(token, () => InternalService.SaveCustomerGroup(token, customerGroup));
        }

        public PagedResult<CustomerGroupDTO> GetCustomerGroups(Token token, CustomerGroupSearchCriteria criteria, PartialRetrievingInfo pageInfo)
        {
            return exceptionHandling(token, () => InternalService.GetCustomerGroups(token, criteria, pageInfo));
        }

        public void DeleteCustomerGroup(Token token, CustomerGroupDTO group)
        {
            exceptionHandling(token, () => InternalService.DeleteCustomerGroup(token, @group));
        }

        public PagedResult<ProductInfoDTO> GetProducts(Token token, GetProductsParam getProductsParam, PartialRetrievingInfo retrievingInfo)
        {
            return exceptionHandling(token, () => InternalService.GetProducts(token, getProductsParam, retrievingInfo));
        }

        public ReminderItemDTO SaveReminder(Token token, ReminderItemDTO reminder)
        {
            return exceptionHandling(token, () => InternalService.SaveReminder(token, reminder));
        }

        public PagedResult<ReminderItemDTO> GetReminders(Token token, GetRemindersParam remindersParam, PartialRetrievingInfo pageInfo)
        {
            return exceptionHandling(token, () => InternalService.GetReminders(token, remindersParam, pageInfo));
        }

        public ReminderItemDTO ReminderOperation(Token token, ReminderOperationParam remindersParam)
        {
            return exceptionHandling(token, () => InternalService.ReminderOperation(token, remindersParam));
        }

        public PagedResult<SupplementCycleDefinitionDTO> GetSupplementsCycleDefinitions(Token token, GetSupplementsCycleDefinitionsParam param, PartialRetrievingInfo pageInfo)
        {
            return exceptionHandling(token, () => InternalService.GetSupplementsCycleDefinitions(token, param, pageInfo));
        }

        public SupplementCycleDefinitionDTO SaveSupplementsCycleDefinition(Token token, SupplementCycleDefinitionDTO definition)
        {
            return exceptionHandling(token, () => InternalService.SaveSupplementsCycleDefinition(token, definition));
        }

        public SupplementCycleDefinitionDTO SupplementsCycleDefinitionOperation(Token token, SupplementsCycleDefinitionOperationParam definition)
        {
            return exceptionHandling(token, () => InternalService.SupplementsCycleDefinitionOperation(token, definition));
        }

        public VoteResult Vote(Token token, VoteParams param)
        {
            return exceptionHandling(token, () => InternalService.Vote(token, param));
        }

        public MyTrainingDTO MyTrainingOperation(Token token, MyTrainingOperationParam param)
        {
            return exceptionHandling(token, () => InternalService.MyTrainingOperation(token, param));
        }

        public PagedResult<MyTrainingDTO> GetMyTrainings(Token token, GetMyTrainingsParam param, PartialRetrievingInfo pageInfo)
        {
            return exceptionHandling(token, () => InternalService.GetMyTrainings(token, param, pageInfo));
        }

        public void ExerciseOperation(Token token, ExerciseOperationParam param)
        {
            exceptionHandling(token, () => InternalService.ExerciseOperation(token, param));
        }

        public IList<WeightExerciseReportResultItem> ReportExerciseWeight(Token token, ReportExerciseWeightParams param)
        {
            return exceptionHandling(token, () => InternalService.ReportExerciseWeight(token, param));
        }

        public IList<WeightReperitionReportResultItem> ReportWeightRepetitions(Token token, ReportWeightRepetitionsParams param)
        {
            return exceptionHandling(token, () => InternalService.ReportWeightRepetitions(token, param));
        }

        public IList<MeasurementsTimeReportResultItem> ReportMeasurementsTime(Token token, ReportMeasurementsTimeParams param)
        {
            return exceptionHandling(token, () => InternalService.ReportMeasurementsTime(token, param));
        }

        public PagedResult<MessageDTO> GetMessages(Token token, GetMessagesCriteria param, PartialRetrievingInfo pageInfo)
        {
            return exceptionHandling(token, () => InternalService.GetMessages(token, param, pageInfo));
        }

        public void ImportLicence(Token token, string licenceKey)
        {
            exceptionHandling(token, () => InternalService.ImportLicence(token, licenceKey));
        }

        public MyPlaceDTO SaveMyPlace(Token token, MyPlaceDTO myPlace)
        {
            return exceptionHandling(token, () => InternalService.SaveMyPlace(token, myPlace));
        }

        public MyPlaceDTO MyPlaceOperation(Token token, MyPlaceOperationParam param)
        {
            return exceptionHandling(token, () => InternalService.MyPlaceOperation(token, param));
        }

        public PagedResult<MyPlaceDTO> GetMyPlaces(Token token, MyPlaceSearchCriteria param, PartialRetrievingInfo pageInfo)
        {
            return exceptionHandling(token, () => InternalService.GetMyPlaces(token, param, pageInfo));
        }

        public PagedResult<ExerciseRecordsReportResultItem> ReportExerciseRecords(Token token, ExerciseRecordsParams param, PartialRetrievingInfo pageInfo)
        {
            return exceptionHandling(token, () => InternalService.ReportExerciseRecords(token, param,pageInfo));
        }

        public FeaturedData GetFeaturedData(Token token,GetFeaturedDataParam param)
        {
            return exceptionHandling(token, () => InternalService.GetFeaturedData(token,param));
        }

        public PagedResult<ChampionshipDTO> GetChampionships(Token token, GetChampionshipsCriteria criteria, PartialRetrievingInfo retrievingInfo)
        {
            return exceptionHandling(token, () => InternalService.GetChampionships(token, criteria, retrievingInfo));
        }

        public SaveChampionshipResult SaveChampionship(Token token, ChampionshipDTO championship)
        {
            return exceptionHandling(token, () => InternalService.SaveChampionship(token, championship));
        }

        public GPSCoordinatesDTO GetGPSCoordinates(GetGPSCoordinatesParam param)
        {
            return exceptionHandling(null, () => InternalService.GetGPSCoordinates(param));
        }


        public GPSCoordinatesOperationResult GPSCoordinatesOperation(GPSOperationParam param)
        {
            //int y = 0;
            //int yy = 45/y;
            return exceptionHandling(null, () => InternalService.GPSCoordinatesOperation(param));
        }
    }

}

