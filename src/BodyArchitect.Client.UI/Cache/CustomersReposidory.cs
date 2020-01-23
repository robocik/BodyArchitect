using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.UI.Cache
{
    public class CustomersReposidory: ObjectsCacheBase<CustomerDTO>
    {
        private static CustomersReposidory instance;
        public static CustomersReposidory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CustomersReposidory();
                }
                return instance;
            }
        }


        protected override PagedResult<CustomerDTO> GetItemsMethod(PartialRetrievingInfo pageInfo)
        {
            return ServiceManager.GetCustomers(new CustomerSearchCriteria());
        }
    }
    
}
