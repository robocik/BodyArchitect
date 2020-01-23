using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Service.V2.Payments
{
    public class PaymentsManager
    {
        public PaymentsHolder Load(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            var xml = reader.ReadToEnd();
            return Load(xml);
        }


        public PaymentsHolder Load(string xml)
        {
            PaymentsHolder payments = new PaymentsHolder();

            var xmlElement=XElement.Parse(xml);
            var temp = (from c in xmlElement.Descendants("Accounts").Descendants("AccountType")
                             select new PaymentAccountType(){
                                 AccountType =(AccountType) Enum.Parse(typeof(AccountType),c.Attribute("Name").Value),
                                Points =int.Parse(c.Attribute("Points").Value),
                                 PromotionPoints = c.Attribute("PromotionPoints") != null ? int.Parse(c.Attribute("PromotionPoints").Value) : (int?)null,
                                 PromotionStartDate = c.Attribute("PromotionStartDate") != null ? DateTime.Parse(c.Attribute("PromotionStartDate").Value) : (DateTime?)null
                             });

            payments.Kara = int.Parse((from c in xmlElement.Descendants("Accounts") select c.Attribute("Kara").Value).Single());
            payments.AccountTypes = temp.ToDictionary(x => x.AccountType);

            var temp1 = (from c in xmlElement.Descendants("Payments").Descendants("Payment")
                    select new { OffertName = c.Attribute("Name"), Points = c.Attribute("Points") });

            payments.ItemsToBuy = temp1.ToDictionary(x => x.OffertName.Value, b => int.Parse(b.Points.Value));
            return payments;
        }

        public PaymentsHolder Load(IDictionary<AccountType, PaymentAccountType> accountTypes, IDictionary<string, int> itemsToBuy, int kara)
        {
            PaymentsHolder paymentsHolder = new PaymentsHolder();
            paymentsHolder.AccountTypes = accountTypes;
            paymentsHolder.ItemsToBuy = itemsToBuy;
            paymentsHolder.Kara = kara;
            return paymentsHolder;
        }

        
    }
}
