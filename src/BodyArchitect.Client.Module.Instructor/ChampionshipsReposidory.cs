using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.Instructor
{
    class ChampionshipsReposidory: ObjectsCacheBase<ChampionshipDTO>
    {
        private static ChampionshipsReposidory instance;
        public static ChampionshipsReposidory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ChampionshipsReposidory();
                }
                return instance;
            }
        }


        protected override PagedResult<ChampionshipDTO> GetItemsMethod(PartialRetrievingInfo pageInfo)
        {
            return ServiceManager.GetChampionships(new GetChampionshipsCriteria(), pageInfo);
        }
  
    }
}
