using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Service.Model
{
    public static class ModelHelper
    {
        public static void SplitByType(TrainingDayDTO day, Dictionary<Type, List<EntryObjectDTO>> entriesGroup, bool loadedOnly)
        {
            foreach (var entry in day.Objects)
            {
                if (!loadedOnly || entry.IsLoaded)
                {
                    Type type = entry.GetType();
                    if (!entriesGroup.ContainsKey(type))
                    {
                        entriesGroup.Add(type, new List<EntryObjectDTO>());
                    }
                    entriesGroup[type].Add(entry);
                }
            }
        }

        public static Dictionary<Type, List<EntryObjectDTO>> SplitByType(this TrainingDayDTO day, bool loadedOnly)
        {
            Dictionary<Type, List<EntryObjectDTO>> dict = new Dictionary<Type, List<EntryObjectDTO>>();
            SplitByType(day, dict, loadedOnly);
            return dict;
        }
    }
}
