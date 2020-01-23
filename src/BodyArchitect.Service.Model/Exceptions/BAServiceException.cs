using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation;

namespace BodyArchitect.Service.Model.Exceptions
{
    public enum ErrorCode
    {
        UnexpectedException,
        UniqueException,
        ValidationException,
        CrossProfileOperation,
        DatabaseException,
        ArgumentNullException,
        NullReferenceException,
        InvalidOperationException,
        AuthenticationException,
        OldDataException,
        ProfileAlreadyFriendException,
        CannotAcceptRejectInvitationDoesntExistException,
        ObjectIsFavoriteException,
        ObjectIsNotFavoriteException,
        ObjectNotFound,
        EMailSendException,
        SecurityException,
        ConsistencyException,
        MaintenanceException,
        UnauthorizedAccessException,
        DatabaseVersionException,
        ArgumentException,
        UserDeletedException,
        ProfileIsNotActivatedException,
        FileNotFoundException,
        ProfileDeletedException,
        TrainingIntegrityException
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    public class BAServiceException
    {
        public BAServiceException(ErrorCode errorCode, string message):this()
        {
            ErrorCode = errorCode;
            Message = message;
        }

        public BAServiceException()
        {
        }

        [DataMember]
        public Guid ErrorId { get; set; }

        [DataMember]
        public ErrorCode ErrorCode;
        [DataMember]
        public string Message;
        [DataMember]
        public string AdditionalData;
    }
}
