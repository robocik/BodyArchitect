using BodyArchitect.Client.Common;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor
{
    public class ActivitiesReposidory : ObjectsCacheBase<ActivityDTO>
    {
        private static ActivitiesReposidory instance;
        public static ActivitiesReposidory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ActivitiesReposidory();
                }
                return instance;
            }
        }


        protected override PagedResult<ActivityDTO> GetItemsMethod(PartialRetrievingInfo pageInfo)
        {
            return ServiceManager.GetActivities();
        }
    }
}
