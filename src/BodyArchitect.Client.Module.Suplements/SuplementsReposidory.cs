using System;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.Suplements
{
    public class SuplementsReposidory : ObjectsCacheBase<SuplementDTO>
    {
        private static SuplementsReposidory instance;

        public static SuplementsReposidory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SuplementsReposidory();
                }
                return instance;
            }
        }

        protected override PagedResult<SuplementDTO> GetItemsMethod(PartialRetrievingInfo pageInfo)
        {
            return ServiceManager.GetSuplements(pageInfo);
        }

        public override SuplementDTO GetItem(Guid id)
        {
            var item = base.GetItem(id);
            if (item == null)
            {
                return SuplementDTO.Removed;
            }
            return item;
        }
    }
}
