using System;
using System.Collections.Generic;
using System.Windows.Media;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Common.Plugins
{
    public class ImageItem
    {
        public ImageSource Image { get; set; }

        public string ToolTip { get; set; }

        public string Content { get; set; }

        public Brush BackBrush { get; set; }

        public int Order { get; set; }

        public EntryObjectDTO Entry { get; set; }
    }

    public interface  ICalendarDayContextEx
    {
        string Name { get; }

        Guid GlobalId { get; }

        ImageItem[] GetDayContents(TrainingDayDTO day);

        ImageSource Image
        {
            get;
        }
    }
}
