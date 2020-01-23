using System;
using System.Collections.Generic;
using System.Drawing;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.WCF.Plugins
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
