using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor
{
    class CustomerGroupsReposidory : ObjectsCacheBase<CustomerGroupDTO>
    {
        private static CustomerGroupsReposidory instance;
        public static CustomerGroupsReposidory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CustomerGroupsReposidory();
                }
                return instance;
            }
        }


        protected override PagedResult<CustomerGroupDTO> GetItemsMethod(PartialRetrievingInfo pageInfo)
        {
            return ServiceManager.GetCustomerGroups(new CustomerGroupSearchCriteria(), pageInfo);
        }
    }
}
