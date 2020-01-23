using System;
using System.Collections.Generic;
using System.Linq;
using BodyArchitect.Client.Common;
using BodyArchitect.Shared;

namespace BodyArchitect.Service.V2.Model
{
    public class SupplementsCycleRepetiter
    {
        public SupplementCycleDefinitionDTO Preapre(SupplementCycleDefinitionDTO definition, int totalWeeks)
        {
            var clone = definition.StandardClone();
            var normalTotalWeeks = definition.GetTotalWeeks();
            if (normalTotalWeeks < totalWeeks)
            {
                int index = 0;
                //this cycle will take more weeks so we must repetit the correct weeks
                var repetitedWeeks = definition.Weeks.Where(x => x.IsRepetitable).OrderBy(x => x.CycleWeekStart).ToList();
                if(repetitedWeeks.Count==0)
                {
                    throw new TrainingIntegrationException("This plan doesn't support longer cycles");
                }
                for (int i = 0; i < totalWeeks - normalTotalWeeks; i++)
                {
                    var week = repetitedWeeks[index % repetitedWeeks.Count].StandardClone();
                    week.GlobalId = Guid.NewGuid();
                    int diff = week.CycleWeekEnd - week.CycleWeekStart;

                    week.CycleWeekStart = normalTotalWeeks + i+1;
                    week.CycleWeekEnd = week.CycleWeekStart + diff;
                    i += diff;
                    index++;
                    clone.Weeks.Add(week);
                }
            }

            return clone;
        }
    }
}
