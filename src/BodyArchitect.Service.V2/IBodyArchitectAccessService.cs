using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Exceptions;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Service.V2.Model.Reports;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using BodyArchitect.Service.V2.Model.WP7;
using Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.V2
{
    [ServiceContract(Namespace = Const.ServiceNamespaceRoot)]
    public interface IBodyArchitectAccessService
    {
        
        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        GPSCoordinatesOperationResult GPSCoordinatesOperation(GPSOperationParam param);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        void ProfileOperation([NotNullValidator]Token token, [NotNullValidator]ProfileOperationParam param);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        void AccountOperation([NotNullValidator] string userNameOrEmail, [NotNullValidator]AccountOperationType operationType);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        void UserFavoritesOperation([NotNullValidator]Token token, [NotNullValidator] UserDTO userDto, FavoriteOperation operation);


        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        TrainingDayCommentDTO TrainingDayCommentOperation([NotNullValidator]Token token, [NotNullValidator] TrainingDayCommentOperationParam arg);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        PagedResult<TrainingDayCommentDTO> GetTrainingDayComments([NotNullValidator]Token token, [NotNullValidator]TrainingDayInfoDTO day, [NotNullValidator]PartialRetrievingInfo info);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        void MessageOperation([NotNullValidator]Token token, [NotNullValidator] MessageOperationParam arg);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        void SendMessage([NotNullValidator]Token token, [NotNullValidator]MessageDTO message);

        //[OperationContract]
        //[FaultContract(typeof(BAServiceException))]
        //[FaultContract(typeof(BAAuthenticationException))]
        //[FaultContract(typeof(ValidationFault))]
        //void ExerciseFavoritesOperation([NotNullValidator]Token token, [NotNullValidator] ExerciseDTO exerciseDto, [NotNullValidator]FavoriteOperation operation);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(BAAuthenticationException))]
        [FaultContract(typeof(ValidationFault))]
        ProfileInformationDTO GetProfileInformation([NotNullValidator]Token token, [NotNullValidator]GetProfileInformationCriteria criteria);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(BAAuthenticationException))]
        [FaultContract(typeof(ValidationFault))]
        FriendInvitationDTO InviteFriendOperation([NotNullValidator]Token token, [NotNullValidator]InviteFriendOperationData data);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(BAAuthenticationException))]
        [FaultContract(typeof(ValidationFault))]
        SessionData Login([NotNullValidator]ClientInformation clientInfo, [NotNullValidator] string username, [NotNullValidator]string password);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        void Logout([NotNullValidator]Token token);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(BAAuthenticationException))]
        SessionData CreateProfile([NotNullValidator]ClientInformation clientInfo, [NotNullValidator] ProfileDTO newProfile);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        [FaultContract(typeof(BAServiceException))]
        ProfileDTO UpdateProfile([NotNullValidator]Token token, [NotNullValidator]ProfileUpdateData data);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(BAAuthenticationException))]
        [FaultContract(typeof(ValidationFault))]
        void DeleteTrainingDay([NotNullValidator]Token token, [NotNullValidator] DeleteTrainingDayParam dayDto);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(BAAuthenticationException))]
        [FaultContract(typeof(ValidationFault))]
        PictureInfoDTO UploadImage([NotNullValidator]PictureDTO pictureDto);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(BAAuthenticationException))]
        [FaultContract(typeof(ValidationFault))]
        bool CheckProfileNameAvailability([NotNullValidator]string userName);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        SaveTrainingDayResult SaveTrainingDay([NotNullValidator]Token token, [NotNullValidator] TrainingDayDTO day);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        PagedResult<TrainingDayDTO> GetTrainingDays([NotNullValidator]Token token, [NotNullValidator] WorkoutDaysSearchCriteria searchCriteria, [NotNullValidator] PartialRetrievingInfo retrievingInfo);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        TrainingDayDTO GetTrainingDay([NotNullValidator]Token token, [NotNullValidator]WorkoutDayGetOperation operationParams, [NotNullValidator]RetrievingInfo retrievingInfo);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(BAAuthenticationException))]
        PictureDTO GetImage([NotNullValidator]PictureInfoDTO pictureInfo);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(BAAuthenticationException))]
        [FaultContract(typeof(ValidationFault))]
        PagedResult<ExerciseDTO> GetExercises([NotNullValidator]Token token, [NotNullValidator]ExerciseSearchCriteria searchCriteria, [NotNullValidator] PartialRetrievingInfo retrievingInfo);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(BAAuthenticationException))]
        [FaultContract(typeof(ValidationFault))]
        PagedResult<SuplementDTO> GetSuplements([NotNullValidator]Token token, [NotNullValidator]GetSupplementsParam param, [NotNullValidator] PartialRetrievingInfo retrievingInfo);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        ExerciseDTO SaveExercise([NotNullValidator]Token token, [NotNullValidator] ExerciseDTO exercise);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        SuplementDTO SaveSuplement([NotNullValidator]Token token, [NotNullValidator] SuplementDTO suplement);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(BAAuthenticationException))]
        [FaultContract(typeof(ValidationFault))]
        PagedResult<TrainingPlan> GetWorkoutPlans([NotNullValidator]Token token, [NotNullValidator]WorkoutPlanSearchCriteria searchCriteria, [NotNullValidator]PartialRetrievingInfo pagerInfo);


        //[OperationContract]
        //[FaultContract(typeof(BAServiceException))]
        //[FaultContract(typeof(BAAuthenticationException))]
        //[FaultContract(typeof(ValidationFault))]
        //void DeleteWorkoutPlan([NotNullValidator]Token token, [NotNullValidator] TrainingPlanInfo dto);

        //[OperationContract]
        //[FaultContract(typeof(BAServiceException))]
        //[FaultContract(typeof(BAAuthenticationException))]
        //[FaultContract(typeof(ValidationFault))]
        //TrainingPlan GetWorkoutPlan([NotNullValidator]Token token, [NotNullValidator]Guid planId, [NotNullValidator] RetrievingInfo retrievingInfo);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(BAAuthenticationException))]
        [FaultContract(typeof(ValidationFault))]
        TrainingPlan SaveWorkoutPlan([NotNullValidator]Token token, [NotNullValidator] TrainingPlan dto);

        //[OperationContract]
        //[FaultContract(typeof(BAServiceException))]
        //[FaultContract(typeof(ValidationFault))]
        //[FaultContract(typeof(BAAuthenticationException))]
        //WorkoutPlanDTO VoteWorkoutPlan([NotNullValidator]Token token, [NotNullValidator] WorkoutPlanDTO planDto);

        //[OperationContract]
        //[FaultContract(typeof(BAServiceException))]
        //[FaultContract(typeof(ValidationFault))]
        //[FaultContract(typeof(BAAuthenticationException))]
        //ExerciseDTO VoteExercise([NotNullValidator]Token token, [NotNullValidator] ExerciseDTO exercise);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        PagedResult<CommentEntryDTO> GetComments([NotNullValidator]Token token, [NotNullValidator]Guid globalId, [NotNullValidator]PartialRetrievingInfo retrievingInfo);

        
        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(BAAuthenticationException))]
        [FaultContract(typeof(ValidationFault))]
        TrainingPlan WorkoutPlanOperation([NotNullValidator]Token token, [NotNullValidator] WorkoutPlanOperationParam param);
   
        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(BAAuthenticationException))]
        [FaultContract(typeof(ValidationFault))]
        PagedResult<UserSearchDTO> GetUsers([NotNullValidator]Token token, [NotNullValidator] UserSearchCriteria searchCriteria, [NotNullValidator] PartialRetrievingInfo pagerInfo);

        [OperationContract]
        string WP7Register(string deviceid, string uri, Guid profileId);

        [OperationContract]
        void WP7Unregister(string deviceid);

        [OperationContract]
        void WP7ClearCounter(string deviceid);

        [OperationContract]
        TrialStatusInfo WP7TrialStatus(string deviceId);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        PagedResult<ActivityDTO> GetActivities([NotNullValidator] Token token,[NotNullValidator] PartialRetrievingInfo retrievingInfo);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(BAAuthenticationException))]
        [FaultContract(typeof(ValidationFault))]
        void DeleteActivity([NotNullValidator] Token token, [NotNullValidator] ActivityDTO activity);

        [OperationContract]
        [FaultContract(typeof (BAServiceException))]
        [FaultContract(typeof (BAAuthenticationException))]
        [FaultContract(typeof (ValidationFault))]
        ActivityDTO SaveActivity([NotNullValidator] Token token, [NotNullValidator] ActivityDTO activity);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(BAAuthenticationException))]
        [FaultContract(typeof(ValidationFault))]
        PagedResult<ScheduleEntryBaseDTO> GetScheduleEntries(Token token, GetScheduleEntriesParam getScheduleEntriesParam, PartialRetrievingInfo retrievingInfo);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(BAAuthenticationException))]
        [FaultContract(typeof(ValidationFault))]
        IList<ScheduleEntryBaseDTO> SaveScheduleEntriesRange([NotNullValidator]Token token, [NotNullValidator]SaveScheduleEntryRangeParam saveScheduleEntryRangeParam);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(BAAuthenticationException))]
        [FaultContract(typeof(ValidationFault))]
        CustomerDTO SaveCustomer([NotNullValidator]Token token, [NotNullValidator] CustomerDTO customerDto);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(BAAuthenticationException))]
        [FaultContract(typeof(ValidationFault))]
        void DeleteCustomer([NotNullValidator]Token token, [NotNullValidator]CustomerDTO customerDto);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        PagedResult<CustomerDTO> GetCustomers([NotNullValidator] Token token, [NotNullValidator]  CustomerSearchCriteria customerSearchCriteria, [NotNullValidator] PartialRetrievingInfo retrievingInfo);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        ReservationOperationResult ReservationsOperation([NotNullValidator]Token token, [NotNullValidator] ReservationsOperationParam param);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        PaymentBasketDTO PaymentBasketOperation([NotNullValidator]Token token, [NotNullValidator]PaymentBasketDTO koszyk);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        PagedResult<PaymentBasketDTO> GetPaymentBaskets([NotNullValidator]Token token, [NotNullValidator]GetPaymentBasketParam getPaymentBasketParam, [NotNullValidator]PartialRetrievingInfo retrievingInfo);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        CustomerGroupDTO SaveCustomerGroup([NotNullValidator]Token token, [NotNullValidator] CustomerGroupDTO customerGroup);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        PagedResult<CustomerGroupDTO> GetCustomerGroups([NotNullValidator]Token token, [NotNullValidator] CustomerGroupSearchCriteria criteria, [NotNullValidator] PartialRetrievingInfo pageInfo);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        void DeleteCustomerGroup([NotNullValidator]Token token, [NotNullValidator]CustomerGroupDTO group);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        PagedResult<ProductInfoDTO> GetProducts([NotNullValidator]Token token, [NotNullValidator]GetProductsParam getProductsParam, [NotNullValidator]PartialRetrievingInfo retrievingInfo);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        ReminderItemDTO SaveReminder([NotNullValidator]Token token, [NotNullValidator]ReminderItemDTO reminder);

        [OperationContract]
        [FaultContract(typeof (BAServiceException))]
        [FaultContract(typeof (ValidationFault))]
        [FaultContract(typeof (BAAuthenticationException))]
        PagedResult<ReminderItemDTO> GetReminders([NotNullValidator]Token token, [NotNullValidator]GetRemindersParam remindersParam, [NotNullValidator] PartialRetrievingInfo pageInfo);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        ReminderItemDTO ReminderOperation([NotNullValidator]Token token, [NotNullValidator] ReminderOperationParam remindersParam);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        PagedResult<SupplementCycleDefinitionDTO> GetSupplementsCycleDefinitions([NotNullValidator]Token token, [NotNullValidator] GetSupplementsCycleDefinitionsParam param, [NotNullValidator] PartialRetrievingInfo pageInfo);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        SupplementCycleDefinitionDTO SaveSupplementsCycleDefinition([NotNullValidator]Token token, [NotNullValidator]SupplementCycleDefinitionDTO definition);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        SupplementCycleDefinitionDTO SupplementsCycleDefinitionOperation([NotNullValidator]Token token, [NotNullValidator] SupplementsCycleDefinitionOperationParam param);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        VoteResult Vote([NotNullValidator]Token token, [NotNullValidator]VoteParams param);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        MyTrainingDTO MyTrainingOperation([NotNullValidator]Token token, [NotNullValidator]MyTrainingOperationParam param);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        PagedResult<MyTrainingDTO> GetMyTrainings([NotNullValidator]Token token, [NotNullValidator] GetMyTrainingsParam param, [NotNullValidator]PartialRetrievingInfo pageInfo);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        void ExerciseOperation([NotNullValidator]Token token, [NotNullValidator] ExerciseOperationParam param);

        [OperationContract]
        [FaultContract(typeof (BAServiceException))]
        [FaultContract(typeof (ValidationFault))]
        [FaultContract(typeof (BAAuthenticationException))]
        IList<WeightExerciseReportResultItem> ReportExerciseWeight([NotNullValidator]Token token, [NotNullValidator] ReportExerciseWeightParams param);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        IList<WeightReperitionReportResultItem> ReportWeightRepetitions([NotNullValidator]Token token, [NotNullValidator]ReportWeightRepetitionsParams param);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        IList<MeasurementsTimeReportResultItem> ReportMeasurementsTime([NotNullValidator]Token token, [NotNullValidator]ReportMeasurementsTimeParams param);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        PagedResult<MessageDTO> GetMessages([NotNullValidator]Token token, [NotNullValidator] GetMessagesCriteria param, [NotNullValidator]PartialRetrievingInfo pageInfo);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        void ImportLicence([NotNullValidator]Token token, [NotNullValidator] string licenceKey);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        MyPlaceDTO SaveMyPlace([NotNullValidator]Token token, [NotNullValidator]MyPlaceDTO myPlace);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        MyPlaceDTO MyPlaceOperation([NotNullValidator]Token token, [NotNullValidator]MyPlaceOperationParam param);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        PagedResult<MyPlaceDTO> GetMyPlaces([NotNullValidator]Token token, [NotNullValidator] MyPlaceSearchCriteria param, [NotNullValidator] PartialRetrievingInfo pageInfo);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        PagedResult<ExerciseRecordsReportResultItem> ReportExerciseRecords([NotNullValidator]Token token, [NotNullValidator] ExerciseRecordsParams param, [NotNullValidator]PartialRetrievingInfo pageInfo);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        FeaturedData GetFeaturedData([NotNullValidator]Token token, [NotNullValidator]GetFeaturedDataParam param);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        PagedResult<ChampionshipDTO> GetChampionships([NotNullValidator]Token token, [NotNullValidator] GetChampionshipsCriteria criteria, [NotNullValidator]PartialRetrievingInfo retrievingInfo);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        SaveChampionshipResult SaveChampionship([NotNullValidator]Token token, [NotNullValidator]ChampionshipDTO championship);


        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        GPSCoordinatesDTO GetGPSCoordinates(GetGPSCoordinatesParam param);

        
    }

    
}
