using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Module.A6W.Localization;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.A6W.Controls
{
    //[Export(typeof(ICalendarDayContent))]
    //public class A6WCalendarDayContent : ICalendarDayContent
    //{
    //    //public static readonly Guid ID = new Guid("CF0E9CAF-6724-4BFF-8A6D-6B9596665117");
    //    public static readonly Guid ID = new Guid("A7B7503A-FF26-414D-A8A5-242C137B426C");

    //    public Guid GlobalId
    //    {
    //        get { return ID; }
    //    }


    //    public Type SupportedEntryType
    //    {
    //        get { return typeof(A6WEntryDTO); }
    //    }

    //    public string GetDayInfoText(IEnumerable<EntryObjectDTO> entryObjects)
    //    {
    //        var entries = entryObjects.Cast<A6WEntryDTO>();
    //        StringBuilder builder = new StringBuilder();
    //        builder.AppendLine("A6W");
    //        foreach (var a6WEntry in entries)
    //        {
    //            builder.AppendLine(string.Format(A6WEntryStrings.CalendarDayText, a6WEntry.Day.DayNumber));
    //        }
    //        return builder.ToString();
    //    }

    //    public Color GetBackColor(IEnumerable<EntryObjectDTO> entryObjects)
    //    {
    //        return Colors.AntiqueWhite;
    //    }

    //    public string Name
    //    {
    //        get { return A6WEntryStrings.A6WCalendarDayContentName; }
    //    }

    //    public ImageSource Image
    //    {
    //        get
    //        {
    //            BitmapImage source = new BitmapImage(new Uri("pack://application:,,,/BodyArchitect.Client.Module.A6W;component/Images/A6W.png", UriKind.Absolute));
    //            return source;
    //        }
    //    }
    //}

    [Export(typeof(ICalendarDayContextEx))]
    public class A6WCalendarDayContent : ICalendarDayContextEx
    {
        //public static readonly Guid ID = new Guid("CF0E9CAF-6724-4BFF-8A6D-6B9596665117");
        public static readonly Guid ID = new Guid("A7B7503A-FF26-414D-A8A5-242C137B426C");

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
                builder.AppendLine(string.Format(A6WEntryStrings.CalendarDayText, a6WEntry.Day.DayNumber));
            }
            return builder.ToString();
        }

        public string Name
        {
            get { return A6WEntryStrings.A6WCalendarDayContentName; }
        }

        public ImageItem[] GetDayContents(TrainingDayDTO day)
        {
            List<ImageItem> items = new List<ImageItem>();
            foreach (var a6wEntry in day.Objects.OfType<A6WEntryDTO>())
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("A6W");
                builder.AppendLine(string.Format(A6WEntryStrings.CalendarDayText, a6wEntry.Day.DayNumber));
                ImageItem item = new ImageItem();
                item.BackBrush = EntryObjectColors.A6W;
                item.Content = builder.ToString();
                item.Entry = a6wEntry;
                item.ToolTip = Name;
                item.Image = Image;
                items.Add(item);
            }
            return items.ToArray();
        }

        public ImageSource Image
        {
            get
            {
                BitmapImage source = new BitmapImage(new Uri("pack://application:,,,/BodyArchitect.Client.Module.A6W;component/Images/A6W.png", UriKind.Absolute));
                return source;
            }
        }
    }
}
