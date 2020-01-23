using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Media;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Module.GPSTracker.Resources;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.GPSTracker
{
    [Export(typeof(ICalendarDayContextEx))]
    public class GPSTrackerCalendarDayContent : ICalendarDayContextEx
    {
        //public static readonly Guid ID = new Guid("33B76948-6F93-4281-AE7D-F2E9141588C1");
        public static readonly Guid ID = new Guid("7240E588-75C5-4FE0-8688-5ED4D9532DD6");

        #region Implementation of ICalendarDayContent

        public Guid GlobalId
        {
            get { return ID; }
        }


        public string Name
        {
            get { return GPSStrings.EntryTypeName; }
        }

        public ImageItem[] GetDayContents(TrainingDayDTO day)
        {
            List<ImageItem> items = new List<ImageItem>();
            foreach (var gpsEntry in day.Objects.OfType<GPSTrackerEntryDTO>())
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine(gpsEntry.Exercise.Name);
                if (gpsEntry.Distance.HasValue)
                {//
                    builder.AppendFormat(GPSStrings.GPSTrackerCalendarDayContent_Distance, gpsEntry.Distance.Value.ToDisplayDistance(), UIHelper.DistanceType);
                }
                if (gpsEntry.Duration.HasValue)
                {
                    builder.AppendFormat(GPSStrings.GPSTrackerCalendarDayContent_Duration, gpsEntry.Duration.Value.ToDisplayDuration());
                }
                ImageItem item = new ImageItem();
                item.BackBrush = EntryObjectColors.GPSTracker;
                item.Content = builder.ToString();
                item.Entry = gpsEntry;
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
                return "pack://application:,,,/BodyArchitect.Client.Module.GPSTracker;component/Resources/GPSTracker16.png".ToBitmap();
            }
        }

        #endregion
    }
}
