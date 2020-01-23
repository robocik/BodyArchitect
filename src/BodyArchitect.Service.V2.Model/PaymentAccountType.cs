using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    [Serializable]
    public class PaymentsHolder
    {
        private IDictionary<AccountType, PaymentAccountType> accountTypes;
        private IDictionary<string, int> itemsToBuy;

        public PaymentsHolder()
        {
            accountTypes = new Dictionary<AccountType, PaymentAccountType>();
            itemsToBuy = new Dictionary<string, int>();
        }

        [DataMember]
        public int Kara { get; set; }

        [DataMember]
        public IDictionary<AccountType, PaymentAccountType> AccountTypes
        {
            get { return accountTypes; }
            set { accountTypes = value; }
        }

        [DataMember]
        public IDictionary<string, int> ItemsToBuy
        {
            get { return itemsToBuy; }
            set { itemsToBuy = value; }
        }


        public PaymentAccountType GetPoints(AccountType accountType)
        {
            if (AccountTypes.ContainsKey(accountType))
            {
                return AccountTypes[accountType];
            }
            return new PaymentAccountType() { AccountType = accountType };
        }

        public int GetPointsForOffert(string offertName)
        {
            if (ItemsToBuy.ContainsKey(offertName))
            {
                return ItemsToBuy[offertName];
            }
            return 0;
        }
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    [Serializable]
    public class PaymentAccountType
    {
        [DataMember]
        public AccountType AccountType { get; set; }

        [DataMember]
        public int Points { get; set; }

        [DataMember]
        public int? PromotionPoints { get; set; }

        [DataMember]
        public DateTime? PromotionStartDate { get; set; }

        public int GetCurrentPoints(DateTime utcNow)
        {
            if ((PromotionStartDate == null || PromotionStartDate.Value <= utcNow) && PromotionPoints.HasValue)
            {
                return PromotionPoints.Value;
            }
            return Points;
        }
    }
}
