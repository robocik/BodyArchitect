using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Service.Client.WP7
{
    public static class InstanceIdResolver
    {
        public static TrainingDayDTO FillInstaneId(this TrainingDayDTO newObj, TrainingDayDTO dayOld,bool fillOldGlobalId=false)
        {
            newObj.InstanceId = dayOld.InstanceId;
            if (fillOldGlobalId)
            {
                dayOld.GlobalId = newObj.GlobalId;
            }
            foreach (var newEntry in newObj.Objects)
            {
                //first find by GlobalId
                var oldEntry = dayOld.Objects.SingleOrDefault(x => !newEntry.IsNew && x.GlobalId == newEntry.GlobalId);
                if (oldEntry != null)
                {
                    InstanceIdResolver.fillOldGlobalId(newEntry, oldEntry, fillOldGlobalId);
                    continue;
                }
                //next we assume that there is only one type of entry in training day (e.g. one size entry or strength training entry)
                var oldEntriesWithSameType=dayOld.Objects.Where(x => x.GetType() == newEntry.GetType()).ToList();
                if (oldEntriesWithSameType.Count == 1 && oldEntriesWithSameType.Single().IsNew)
                {//we must check if this single entry is new (GlobalId = empty). If not we cannot attach InstanceId
                    InstanceIdResolver.fillOldGlobalId(newEntry, oldEntriesWithSameType.Single(),fillOldGlobalId);
                    continue;
                }

                //if there are more then one entry with the same type, we check if there is only one with IsNew status in old day. If yes then we can find out instanceid
                var oldIsNewEntries = oldEntriesWithSameType.Where(x => x.IsNew).ToList();
                if (oldEntriesWithSameType.Count > 1 && oldIsNewEntries.Count == 1)
                {
                    InstanceIdResolver.fillOldGlobalId(newEntry, oldIsNewEntries.Single(), fillOldGlobalId);
                    continue;
                }
            }
            return newObj;
        }

        private static void fillOldGlobalId(EntryObjectDTO newEntry, EntryObjectDTO oldEntry,bool fillOldGlobalId)
        {
            newEntry.InstanceId = oldEntry.InstanceId;
            if (fillOldGlobalId)
            {
                oldEntry.GlobalId = newEntry.GlobalId;
            }
        }
    }
}
