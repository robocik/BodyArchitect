using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Service.Model.Localization;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.Model
{
    public interface IRetrievedDateTime
    {
        DateTime RetrievedDateTime { get; set; }
    }
    [DataContract(Namespace = Const.ServiceNamespace)]
    [Serializable]
    public class ProfileInformationDTO : IRetrievedDateTime
    {
        public ProfileInformationDTO()
        {
            Friends = new List<UserSearchDTO>();
            FavoriteUsers = new List<UserSearchDTO>();
            Invitations=new List<FriendInvitationDTO>();
            Messages=new List<MessageDTO>();
        }

        [DataMember]
        [ValidatorComposition(CompositionType.Or)]
        [StringLengthValidator(0, Constants.CommentColumnLength, MessageTemplateResourceName = "ProfileDTO_AboutInformation_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        [NotNullValidator(Negated = true)]
        [Filter(FilterType.LongText)]
        public string AboutInformation
        {
            get;
            set;
        }

        [DataMember]
        [RelativeDateTimeValidator(-150, DateTimeUnit.Year, 1, DateTimeUnit.Month, MessageTemplateResourceName = "ProfileDTO_Birthday", MessageTemplateResourceType = typeof(ValidationStrings))]
        public DateTime? Birthday
        {
            get;
            set;
        }

        [DataMember]
        public bool IsActivated { get; set; }

        [DataMember]
        [NotNullValidator]
        public List<MessageDTO> Messages { get; set; }

        [DataMember]
        [NotNullValidator]
        public List<UserSearchDTO> Friends { get; set; }

        [DataMember]
        [NotNullValidator]
        public List<UserSearchDTO> FavoriteUsers { get; set; }

        [DataMember]
        [NotNullValidator]
        public List<FriendInvitationDTO> Invitations { get; set; }

        [DataMember]
        [NotNullValidator]
        public UserSearchDTO User { get; set; }

        [DataMember]
        [ObjectValidator]
        public WymiaryDTO Wymiary
        {
            get;
            set;
        }

        [DataMember]
        [ObjectValidator]
        public ProfileSettingsDTO Settings { get; set; }

        [DataMember]
        public DateTime RetrievedDateTime
        {
            get; set; 
        }

        [DataMember]
        public DateTime? LastLogin
        {
            get;
            set;
        }
    }

}
