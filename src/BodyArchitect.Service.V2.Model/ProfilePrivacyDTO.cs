using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Shared;

namespace BodyArchitect.Service.V2.Model
{
    public enum Privacy
    {
        Private,
        FriendsOnly,
        Public
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    [Serializable]
    public class ProfilePrivacyDTO
    {
        public ProfilePrivacyDTO()
        {
            Searchable = true;
        }

        [DataMember]
        public Privacy CalendarView { get; set; }

        [DataMember]
        public Privacy Sizes { get; set; }

        [DataMember]
        public Privacy Friends { get; set; }

        [DataMember]
        public Privacy BirthdayDate { get; set; }

        [DataMember]
        public bool Searchable { get; set; }
    }
}
