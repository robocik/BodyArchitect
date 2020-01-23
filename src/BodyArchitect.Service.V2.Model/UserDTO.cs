using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Service.V2.Model.Localization;
using BodyArchitect.Service.V2.Model.Validators;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.V2.Model
{
    [KnownType(typeof(ProfileDTO))]
    [KnownType(typeof(UserSearchDTO))]
    [DataContract(Namespace = Const.ServiceNamespace)]
    [Serializable]
    [DebuggerDisplay("UserName = {UserName}")]
    public class UserDTO:BAGlobalObject,IHasPicture
    {
        public const int PictureMaxWidth = 400;
        public const int PictureMaxHeight = 400;

        public UserDTO()
        {
            Privacy=new ProfilePrivacyDTO();
        }

        [DataMember]
        [SerializerId]
        public int Version { get; set; }

        [DataMember]
        public PictureInfoDTO Picture { get; set; }

        [DataMember]
        public Gender Gender { get; set; }

        private string _userName;

        [DataMember]
        [NotNullValidator(MessageTemplateResourceName = "UserDTO_UserName_NotNull", MessageTemplateResourceType = typeof(ValidationStrings))]
        [StringLengthValidator(3, Constants.NameColumnLength, MessageTemplateResourceName = "UserDTO_UserName_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        [RegexValidator("^[a-zA-Z_][0-9a-zA-Z_]*$", MessageTemplateResourceName = "UserDTO_UserName_SpecialCharacters", MessageTemplateResourceType = typeof(ValidationStrings))]
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        [DataMember]
        [NotNullValidator]
        public ProfilePrivacyDTO Privacy { get; set; }

        [DataMember]
        public DateTime CreationDate
        {
            get; set;
        }

        [DataMember]
        public bool IsDeleted
        {
            get;
            protected set;
        }

        

        private int _countryId;

        [DataMember]
        [CountryValidator(MessageTemplateResourceName = "UserDTO_CountryId", MessageTemplateResourceType = typeof(ValidationStrings))]
        public int CountryId
        {
            get { return _countryId; }
            set { _countryId = value; }
        }
    }
}
