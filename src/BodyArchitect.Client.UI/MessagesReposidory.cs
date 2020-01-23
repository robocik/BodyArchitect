using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.UI
{
    public class MessagesReposidory : ObjectsCacheBase<MessageDTO>
    {
        private static MessagesReposidory instance;
        public static MessagesReposidory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MessagesReposidory();
                }
                return instance;
            }
        }


        protected override PagedResult<MessageDTO> GetItemsMethod(PartialRetrievingInfo pageInfo)
        {
            var search = new GetMessagesCriteria();
            return ServiceManager.GetMessages(search, pageInfo);
        }
    }
}
