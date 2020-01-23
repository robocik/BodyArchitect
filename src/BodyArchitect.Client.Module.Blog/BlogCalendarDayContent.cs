using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Media;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Module.Blog.Localization;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Converters;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.Blog
{
    //[Export(typeof(ICalendarDayContent))]
    //public class BlogCalendarDayContent : ICalendarDayContent
    //{
    //    //public static readonly Guid ID = new Guid("2C0513B7-2BAD-4A40-A8E5-9592EEF3CAB8");
    //    public static readonly Guid ID = new Guid("29E5BFEB-D8D0-434C-BF37-B22F18E23E9A");

    //    public Guid GlobalId
    //    {
    //        get { return ID; }
    //    }

    //    public Type SupportedEntryType
    //    {
    //        get { return typeof(BlogEntryDTO); }
    //    }

    //    public string GetDayInfoText(IEnumerable<EntryObjectDTO> entryObjects)
    //    {
    //        //var blogEntries = entryObjects.Cast<BlogEntryDTO>();
    //        StringBuilder builder = new StringBuilder();
    //        //foreach (var blogEntry in blogEntries)
    //        //{
    //        //    if (blogEntry.AllowComments)
    //        //    {
    //        //        string lastCommentDate = null;
    //        //        if (blogEntry.LastCommentDate.HasValue)
    //        //        {
    //        //            lastCommentDate=blogEntry.LastCommentDate.Value.ToLocalTime().ToRelativeDate();
    //        //        }
    //        //        builder.AppendFormat(BlogEntryStrings.BlogCalendarDayContent_GetDayInfoText_CommentsEnabled, blogEntry.TrainingDayCommentsCount, lastCommentDate);
    //        //    }
    //        //    else
    //        //    {
    //        //        builder.AppendLine(BlogEntryStrings.BlogCalendarDayContent_GetDayInfoText_CommentsDisabled);
    //        //    }
    //        //}
    //        return builder.ToString();
    //    }


    //    public Color GetBackColor(IEnumerable<EntryObjectDTO> entryObjects)
    //    {
    //        //BlogEntryDTO blog = (BlogEntryDTO)entryObjects.Single();
    //        //DateTime lastVisit = BlogSettings.Default.GetVisitDate(blog.TrainingDay.TrainingDate, UserContext.CurrentProfile.Id, blog.TrainingDay.ProfileId);
    //        //if (blog.LastCommentDate.HasValue && lastVisit < blog.LastCommentDate)
    //        {
    //            return Colors.DarkOrange;
    //        }
    //        //else
    //        //{
    //        //    return Colors.DarkKhaki;
    //        //}
    //    }

    //    public string Name
    //    {
    //        get { return BlogEntryStrings.BlogCalendarDayContent_Name; }
    //    }

    //    public ImageSource Image
    //    {
    //        get
    //        {
    //            return "pack://application:,,,/BodyArchitect.Client.Module.Blog;component/Images/Blog.png".ToBitmap();
                
    //        }
    //    }
    //}

    [Export(typeof(ICalendarDayContextEx))]
    public class BlogCalendarDayContent : ICalendarDayContextEx
    {
        //public static readonly Guid ID = new Guid("2C0513B7-2BAD-4A40-A8E5-9592EEF3CAB8");
        public static readonly Guid ID = new Guid("29E5BFEB-D8D0-434C-BF37-B22F18E23E9A");

        public Guid GlobalId
        {
            get { return ID; }
        }

        public string Name
        {
            get { return BlogEntryStrings.BlogCalendarDayContent_Name; }
        }

        public ImageItem[] GetDayContents(TrainingDayDTO day)
        {
            HtmlToTextConverter htmlConverter = new HtmlToTextConverter();
            List<ImageItem> items = new List<ImageItem>();
            foreach (var blog in day.Objects.OfType<BlogEntryDTO>())
            {
                ImageItem item = new ImageItem();
                item.BackBrush = EntryObjectColors.Blog;
                item.Content = (string) htmlConverter.Convert(blog.Comment,typeof(string),null,CultureInfo.CurrentCulture);
                item.Entry = blog;
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
                return "pack://application:,,,/BodyArchitect.Client.Module.Blog;component/Images/Blog.png".ToBitmap();

            }
        }
    }
}
