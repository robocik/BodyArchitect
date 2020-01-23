using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    [Serializable]
    public class LicenceInfoDTO
    {

        /// <summary>
        /// Account type for currently logged user. The difference with AccountType is that this property can have different values because of promotions
        /// </summary>
        [DataMember]
        public AccountType CurrentAccountType { get; set; }

        /// <summary>
        /// Account type saved in the db
        /// </summary>
        [DataMember]
        public AccountType AccountType { get; set; }

        [DataMember]
        public int BAPoints { get; set; }

        [DataMember]
        public PaymentsHolder Payments { get; set; }
        
        public bool IsPremium
        {
            get { return CurrentAccountType != AccountType.User; }
        }

        public bool IsInstructor
        {
            get { return CurrentAccountType == AccountType.Instructor || CurrentAccountType == AccountType.Administrator; }
        }
    }
}
