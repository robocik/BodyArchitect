using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using BodyArchitect.Service.V2.Model.Localization;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.V2.Model
{
    public interface IToken
    {
        Guid SessionId { get; }
    }
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class Token : IToken, IExtensibleDataObject
    {
        public Token(Guid sessionId)
        {
            SessionId = sessionId;
        }

        public Token(Guid sessionId,string language)
        {
            SessionId = sessionId;
            Language = language;
        }

        public ExtensionDataObject ExtensionData
        {
            get;
            set;
        }

        [DataMember]
        public Guid SessionId
        {
            get;
            private set;
        }

        [DataMember]
        [ValidatorComposition(CompositionType.Or)]
        [StringLengthValidator(0, 3)]
        [NotNullValidator(Negated = true)]
        public string Language { get; set; }
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    public class SessionData : IExtensibleDataObject
    {
        public SessionData(Token token, ProfileDTO profile, bool isConnected)
        {
            Token = token;
            IsConnected = isConnected;
            Profile = profile;
        }

        public ExtensionDataObject ExtensionData
        {
            get;
            set;
        }

        [DataMember]
        public Token Token
        {
            get;
            set;
        }

        [DataMember]
        public ProfileDTO Profile { get; set; }

        [DataMember]
        public DateTime? LastLoginDate { get; set; }

        [DataMember]
        public bool IsConnected
        {
            get;
            private set;
        }

        public void FillProfileData(ProfileDTO newProfile)
        {
            if (Profile != null && Profile.GlobalId != newProfile.GlobalId)
            {
                throw new ArgumentException("Cannot set different profile");
            }
            Profile = newProfile;
        }
    }
}
