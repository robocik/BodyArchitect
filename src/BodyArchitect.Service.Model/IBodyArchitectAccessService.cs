using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using BodyArchitect.Service.Model.Exceptions;
using BodyArchitect.Service.Model.TrainingPlans;
using BodyArchitect.Service.Model.WP7;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.Model
{
    public enum GetOperation
    {
        Current,
        First,
        Previous,
        Next,
        Last
    }

    public enum FavoriteOperation
    {
        Add,
        Remove
    }

    public enum InviteFriendOperation
    {
        Invite,
        Accept,
        Reject
    }

    public enum AccountOperationType
    {
        RestorePassword,
        SendActivationEmail
    }

    [ServiceContract(Namespace = Const.ServiceNamespaceRoot)]
    public interface IBodyArchitectAccessService
    {
        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        void ImportData([NotNullValidator]ImportDataStream data );

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        IList<EntryObjectDTO> GetMyTrainingEntries([NotNullValidator]Token token, [NotNullValidator]MyTrainingDTO myTraining);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        void DeleteProfile([NotNullValidator]Token token, [NotNullValidator]ProfileDTO profile);

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
        MapperResult MapExercises([NotNullValidator]Token token, [NotNullValidator]MapperData data);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        BlogCommentDTO BlogCommentOperation([NotNullValidator]Token token, [NotNullValidator] BlogCommentOperation arg);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        PagedResult<BlogCommentDTO> GetBlogComments([NotNullValidator]Token token, [NotNullValidator]BlogEntryDTO entry, [NotNullValidator]PartialRetrievingInfo info);

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
        void DeleteTrainingDay([NotNullValidator]Token token, [NotNullValidator] TrainingDayDTO dayDto);

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
        TrainingDayDTO SaveTrainingDay([NotNullValidator]Token token, [NotNullValidator] TrainingDayDTO day);

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
        PagedResult<SuplementDTO> GetSuplements([NotNullValidator]Token token, [NotNullValidator] PartialRetrievingInfo retrievingInfo);

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
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        IList<MyTrainingDTO> GetStartedTrainings([NotNullValidator]Token token, [NotNullValidator] Guid? typeId, [NotNullValidator] RetrievingInfo retrievingInfo);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(BAAuthenticationException))]
        [FaultContract(typeof(ValidationFault))]
        PagedResult<WorkoutPlanDTO> GetWorkoutPlans([NotNullValidator]Token token, [NotNullValidator]WorkoutPlanSearchCriteria searchCriteria, [NotNullValidator]PartialRetrievingInfo pagerInfo);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(BAAuthenticationException))]
        [FaultContract(typeof(ValidationFault))]
        WorkoutPlanDTO GetWorkoutPlan([NotNullValidator]Token token, [NotNullValidator]Guid planId, [NotNullValidator] RetrievingInfo retrievingInfo);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(BAAuthenticationException))]
        [FaultContract(typeof(ValidationFault))]
        WorkoutPlanDTO SaveWorkoutPlan([NotNullValidator]Token token, [NotNullValidator] WorkoutPlanDTO dto);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        WorkoutPlanDTO VoteWorkoutPlan([NotNullValidator]Token token, [NotNullValidator] WorkoutPlanDTO planDto);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        ExerciseDTO VoteExercise([NotNullValidator]Token token, [NotNullValidator] ExerciseDTO exercise);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        PagedResult<CommentEntryDTO> GetComments([NotNullValidator]Token token, [NotNullValidator]Guid globalId, [NotNullValidator]PartialRetrievingInfo retrievingInfo);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(BAAuthenticationException))]
        [FaultContract(typeof(ValidationFault))]
        void DeleteWorkoutPlan([NotNullValidator]Token token, [NotNullValidator] WorkoutPlanDTO dto);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(BAAuthenticationException))]
        [FaultContract(typeof(ValidationFault))]
        void DeleteExercise([NotNullValidator]Token token, [NotNullValidator] ExerciseDTO dto);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(BAAuthenticationException))]
        [FaultContract(typeof(ValidationFault))]
        void WorkoutPlanFavoritesOperation([NotNullValidator]Token token, [NotNullValidator] WorkoutPlanDTO planDto, [NotNullValidator] FavoriteOperation operation);
        
        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        WorkoutPlanDTO PublishWorkoutPlan([NotNullValidator]Token token, [NotNullValidator]WorkoutPlanDTO planDto);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(BAAuthenticationException))]
        ExerciseDTO PublishExercise([NotNullValidator]Token token, [NotNullValidator] ExerciseDTO exerciseDto);

        [OperationContract]
        [FaultContract(typeof(BAServiceException))]
        [FaultContract(typeof(BAAuthenticationException))]
        [FaultContract(typeof(ValidationFault))]
        PagedResult<UserSearchDTO> GetUsers([NotNullValidator]Token token, [NotNullValidator] UserSearchCriteria searchCriteria, [NotNullValidator] PartialRetrievingInfo pagerInfo);

        [OperationContract]
        string WP7Register(string deviceid, string uri, int profileId);

        [OperationContract]
        void WP7Unregister(string deviceid);

        [OperationContract]
        void WP7ClearCounter(string deviceid);

        [OperationContract]
        TrialStatusInfo WP7TrialStatus(string deviceId);
    }

    
}
