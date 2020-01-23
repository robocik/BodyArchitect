using System;
using System.Collections.Generic;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI.SchedulerEngine;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.Instructor.Reminders
{
    public class NotificationsReposidory : ObjectsCacheBase<NotifyObject>
    {
        private static NotificationsReposidory instance;
        public static NotificationsReposidory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NotificationsReposidory();
                }
                return instance;
            }
        }

        protected override PagedResult<NotifyObject> GetItemsMethod(PartialRetrievingInfo pageInfo)
        {
            return new PagedResult<NotifyObject>(new List<NotifyObject>(),0,0 );
        }
    }
}
