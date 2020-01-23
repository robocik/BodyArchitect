using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI.SchedulerEngine;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.UI.SchedulerEngine
{

    public class ReminderItemsReposidory : ObjectsCacheBase<ReminderItemDTO>
    {
        private static ReminderItemsReposidory instance;
        public static ReminderItemsReposidory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ReminderItemsReposidory();
                }
                return instance;
            }
        }


        protected override PagedResult<ReminderItemDTO> GetItemsMethod(PartialRetrievingInfo pageInfo)
        {
            GetRemindersParam param = new GetRemindersParam();
            return ServiceManager.GetReminders(param, pageInfo);
        }

        

        
    }
}
