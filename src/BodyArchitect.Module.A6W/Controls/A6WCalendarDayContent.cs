using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Text;
using BodyArchitect.Module.A6W.Localization;
using BodyArchitect.Common.Plugins;
using BodyArchitect.Service.Model;

namespace BodyArchitect.Module.A6W.Controls
{
    [Export(typeof(ICalendarDayContent))]
    public class A6WCalendarDayContent : ICalendarDayContent
    {
        public static readonly Guid ID = new Guid("CF0E9CAF-6724-4BFF-8A6D-6B9596665117");

        public Guid GlobalId
        {
            get { return ID; }
        }


        public Type SupportedEntryType
        {
            get { return typeof(A6WEntryDTO); }
        }

        public string GetDayInfoText(IEnumerable<EntryObjectDTO> entryObjects)
        {
            var entries = entryObjects.Cast<A6WEntryDTO>();
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("A6W");
            foreach (var a6WEntry in entries)
            {
                builder.AppendLine(string.Format(A6WEntryStrings.CalendarDayText, a6WEntry.DayNumber));
            }
            return builder.ToString();
        }

        public Color GetBackColor(IEnumerable<EntryObjectDTO> entryObjects)
        {
            return Color.LightGreen;
        }

        public string Name
        {
            get { return A6WEntryStrings.A6WCalendarDayContentName; }
        }

        public Image Image
        {
            get { return A6WResources.A6WModule; }
        }

        public void PrepareData()
        {
            
        }
    }
}
