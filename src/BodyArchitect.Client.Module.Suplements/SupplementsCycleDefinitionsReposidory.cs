using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.Suplements
{

    class SupplementsCycleDefinitionsReposidory : ObjectsCacheBase<SupplementCycleDefinitionDTO>
    {
        private static SupplementsCycleDefinitionsReposidory instance;

        public static SupplementsCycleDefinitionsReposidory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SupplementsCycleDefinitionsReposidory();
                }
                return instance;
            }
        }

        

        protected override PagedResult<SupplementCycleDefinitionDTO> GetItemsMethod(PartialRetrievingInfo pageInfo)
        {
            var param = new GetSupplementsCycleDefinitionsParam();
            param.UserId = UserContext.Current.CurrentProfile.GlobalId;
            param.SearchGroups.Add(WorkoutPlanSearchCriteriaGroup.Mine);
            param.SearchGroups.Add(WorkoutPlanSearchCriteriaGroup.Favorites);
            return ServiceManager.GetSupplementsCycleDefinitions(param,pageInfo);
        }
    }
}
