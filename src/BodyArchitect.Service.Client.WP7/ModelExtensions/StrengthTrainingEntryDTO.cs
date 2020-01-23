using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace BodyArchitect.Service.V2.Model
{
    public partial class StrengthTrainingEntryDTO:SpecificEntryObjectDTO
    {
        public StrengthTrainingEntryDTO()
        {
            Entries=new List<StrengthTrainingItemDTO>();
        }

        public StrengthTrainingItemDTO GetStrengthTrainingItem(Guid instanceId)
        {
            return (from e in Entries where e.InstanceId == instanceId select e).Single();
        }

        public SerieDTO GetSet(Guid instanceId)
        {
            return Entries.SelectMany(itemDto => itemDto.Series).FirstOrDefault(serieDto => serieDto.InstanceId == instanceId);
        }

        public object GetItem(Guid instanceId)
        {
            foreach (var entry in Entries)
            {
                if(entry.InstanceId==instanceId)
                {
                    return  entry;
                }
                foreach (var series in entry.Series)
                {
                    if(series.InstanceId==instanceId)
                    {
                        return series;
                    }
                }
            }
            return null;
        }

        public void AddEntry(StrengthTrainingItemDTO item)
        {
            Entries.Add(item);
            item.StrengthTrainingEntry = this;
        }
    }
}
