using System;
using System.Collections.Generic;
using System.Drawing;
using BodyArchitect.Service.Model;

namespace BodyArchitect.Common.Plugins
{
    public interface ICalendarDayContent
    {
        Guid GlobalId { get; }

        Type SupportedEntryType { get; }

        string GetDayInfoText(IEnumerable<EntryObjectDTO> entryObjects);

        Color GetBackColor(IEnumerable<EntryObjectDTO> entryObjects);

        string Name { get; }

        Image Image
        {
            get;
        }

        void PrepareData();
    }
}
