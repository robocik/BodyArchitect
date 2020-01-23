using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Portable;
using BodyArchitect.Service.V2.Model.Localization;
using BodyArchitect.Service.V2.Model.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.V2.Model.Instructor
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    [Serializable]
    public class CustomerDTO : BAGlobalObject, IHasPicture, IRemindable
    {
        [Required]
        [DataMember]
        [StringLengthValidator(Shared.Constants.NameColumnLength, MessageTemplateResourceName = "CustomerDTO_FirstName_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        public string FirstName { get; set; }

        [DataMember]
        [ValidatorComposition(CompositionType.Or)]
        [StringLengthValidator(Shared.Constants.NameColumnLength, MessageTemplateResourceName = "CustomerDTO_LastName_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        [NotNullValidator(Negated = true)]
        public string LastName { get; set; }

        [DataMember]
        public bool IsVirtual { get; set; }

        [DataMember]
        public Guid ProfileId { get; set; }

        [DataMember]
        public PictureInfoDTO Picture { get; set; }

        //[DataMember]
        //public bool HasBirthdayReminder { get; set; }

        //UTC
        [DataMember]
        public DateTime CreationDate { get; set; }

        public bool HasBirhtdayToday
        {
            get { return Birthday.HasBirthdayToday(); }
        }

        [DataMember]
        public WymiaryDTO Wymiary { get; set; }

        [DataMember]
        public CustomerSettingsDTO Settings { get; set; }

        //UTC
        [DataMember]
        public DateTime? Birthday { get; set; }

        [DataMember]
        public int Version { get; set; }

        [DataMember]
        public UserDTO ConnectedAccount { get; set; }

        [DataMember]
        [IgnoreNulls]
        //[ValidatorComposition(CompositionType.And)]
        //[StringLengthValidator(0,200)]
        [EMailValidator(MessageTemplateResourceName = "CustomerDTO_EMail", MessageTemplateResourceType = typeof(ValidationStrings))]
        public string Email { get; set; }

        [DataMember]
        [ValidatorComposition(CompositionType.Or)]
        [StringLengthValidator(30, MessageTemplateResourceName = "CustomerDTO_PhoneNumber_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        [NotNullValidator(Negated = true)]
        public string PhoneNumber { get; set; }

        [DataMember]
        public Gender Gender { get; set; }

        public string FullName
        {
            get { return string.Format("{0} {1}",LastName,FirstName); }
        }

        [DataMember]
        public TimeSpan? RemindBefore
        {
            get;
            set;
        }

        [DataMember]
        public AddressDTO Address
        {
            get; set;
        }
    }
}
