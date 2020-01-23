using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Service.Model.Localization;
using BodyArchitect.Service.Model.Validators;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.Model
{
    [KnownType(typeof(ProfileDTO))]
    [KnownType(typeof(UserSearchDTO))]
    [DataContract(Namespace = Const.ServiceNamespace)]
    [Serializable]
    [DebuggerDisplay("UserName = {UserName}")]
    public class UserDTO:IHasPicture
    {
        public const int PictureMaxWidth = 150;
        public const int PictureMaxHeight = 150;

        public UserDTO()
        {
            Privacy=new ProfilePrivacyDTO();
        }

        [DataMember]
        public PictureInfoDTO Picture { get; set; }

        [DataMember]
        public Gender Gender { get; set; }

        [DataMember]
        [NotNullValidator(MessageTemplateResourceName = "UserDTO_UserName_NotNull", MessageTemplateResourceType = typeof(ValidationStrings))]
        [StringLengthValidator(3, Constants.NameColumnLength, MessageTemplateResourceName = "UserDTO_UserName_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        [RegexValidator("^[a-zA-Z_][0-9a-zA-Z_]*$", MessageTemplateResourceName = "UserDTO_UserName_SpecialCharacters", MessageTemplateResourceType = typeof(ValidationStrings))]
        public string UserName
        {
            get;
            set;
        }

        [DataMember]
        [NotNullValidator]
        public ProfilePrivacyDTO Privacy { get; set; }

        [DataMember]
        public DateTime CreationDate
        {
            get; protected set;
        }

        [DataMember]
        public bool IsDeleted
        {
            get;
            protected set;
        }

        

        [DataMember]
        public int Id { get; set; }

        

        [DataMember]
        [CountryValidator(MessageTemplateResourceName = "UserDTO_CountryId", MessageTemplateResourceType = typeof(ValidationStrings))]
        public int CountryId { get; set; }
    }
}
