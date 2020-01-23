using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Service.Model.Localization;
using BodyArchitect.Service.Model.Validators;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.Model
{
    public enum PlatformType
    {
        Windows,
        MacOS,
        Android,
        iPhone,
        WindowsMobile,
        WindowsPhone,
        Linux,
        Web,
        Other
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    public class ClientInformation
    {
        [DataMember]
        [NotNullValidator(MessageTemplateResourceName = "ClientInformation_Version_NotNull", MessageTemplateResourceType = typeof(ValidationStrings))]
        [StringLengthValidator(1, 20, MessageTemplateResourceName = "ClientInformation_Version_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        public string Version { get; set; }

        [DataMember]
        public Guid ClientInstanceId { get; set; }

        [DataMember]
        public PlatformType Platform { get; set; }

        [DataMember]
        public string PlatformVersion { get; set; }

        [DataMember]
        [LanguageValidator(MessageTemplateResourceName = "ClientInformation_ApplicationLanguage", MessageTemplateResourceType = typeof(ValidationStrings))]
        public string ApplicationLanguage { get; set; }

        [DataMember]
        public string ApplicationVersion { get; set; }
    }
}
