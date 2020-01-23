using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Media;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.Blog
{
    //this class can be moved to Client.UI project but MEF cannot find it (there is a search pattern *.Module.*.dll
    [Export(typeof(ICalendarDayContextEx))]
    public class TrainingDayInfoDayContent : ICalendarDayContextEx
    {
        public string Name
        {
            get { return "TrainingDayInfoDayContent_Name_Summary".TranslateStrings(); }
        }

        public Guid GlobalId
        {
            get { return new Guid("A18353CA-5767-46E5-AEC9-3E5C193E6E15"); }
        }

        public ImageItem[] GetDayContents(TrainingDayDTO day)
        {
            StringBuilder builder = new StringBuilder();
            if (day.AllowComments)
            {
                string lastCommentDate = null;
                if (day.LastCommentDate.HasValue)
                {
                    lastCommentDate = day.LastCommentDate.Value.ToLocalTime().ToRelativeDate();
                }
                builder.AppendFormat("TrainingDayInfoDayContent_CommentsEnabled".TranslateStrings(), day.CommentsCount, lastCommentDate);
            }
            else
            {
                builder.AppendLine("TrainingDayInfoDayContent_CommentsDisabled".TranslateStrings());
            }

            ImageItem item = new ImageItem();
            item.BackBrush = new SolidColorBrush(Colors.Orange);
            item.Content = builder.ToString();
            item.Entry = null;
            item.ToolTip = Name;
            item.Image = Image;
            return new ImageItem[]{item};
        }

        public ImageSource Image
        {
            get { return "pack://application:,,,/BodyArchitect.Client.Resources;component/Images/Calendar16.png".ToBitmap(); }
        }
    }
}
