using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Service.V2.Model.Localization;
using BodyArchitect.Service.V2.Model.Validators;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.V2.Model
{
    public enum Gender { NotSet, Male, Female };

    public enum AccountType
    {
        User,
        PremiumUser,
        Instructor,
        Administrator
    }

    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class ProfileDTO : UserDTO
    {
        public const string AdministratorName = "admin";
        public static ProfileDTO Empty = new ProfileDTO(DateTime.MinValue);


        public ProfileDTO()
        {
            Gender = Gender.Male;
        }

        private ProfileDTO(DateTime creationDate)
        {
            this.CreationDate = creationDate;
        }


        #region Persistent properties
        
        [DataMember]
        [Filter(FilterType.LongText)]
        [ValidatorComposition(CompositionType.Or)]
        [StringLengthValidator(0, Constants.CommentColumnLength, MessageTemplateResourceName = "ProfileDTO_AboutInformation_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        [NotNullValidator(Negated=true)]
        public string AboutInformation
        {
            get;
            set;
        }

        [DataMember]
        [RelativeDateTimeValidator(-150, DateTimeUnit.Year, 1, DateTimeUnit.Month, MessageTemplateResourceName = "ProfileDTO_Birthday", MessageTemplateResourceType = typeof(ValidationStrings))]
        public DateTime Birthday
        {
            get;
            set;
        }

        [DataMember]
        [NotNullValidator]
        [StringLengthValidator(1, Constants.NameColumnLength)]
        public string Password
        {
            get;
            set;
        }
        
        [DataMember]
        [EMailValidatorAttribute(MessageTemplateResourceName = "ProfileDTO_EMail", MessageTemplateResourceType = typeof(ValidationStrings))]
        [Required(ErrorMessageResourceName = "ProfileDTO_EMail", ErrorMessageResourceType = typeof(ValidationStrings))]
        public string Email { get; set; }
        
        

        [DataMember]
        public ProfileSettingsDTO Settings { get; set; }

        [DataMember]
        public Guid? PreviousClientInstanceId { get; set; }

        

        #endregion

    }
}
