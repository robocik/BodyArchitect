using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Common.Plugins;
using BodyArchitect.Controls;
using BodyArchitect.Module.Blog.Localization;
using BodyArchitect.Module.Blog.Options;
using BodyArchitect.Service.Model;

namespace BodyArchitect.Module.Blog
{
    [Export(typeof(ICalendarDayContent))]
    public class BlogCalendarDayContent : ICalendarDayContent
    {
        public static readonly Guid ID = new Guid("2C0513B7-2BAD-4A40-A8E5-9592EEF3CAB8");

        public Guid GlobalId
        {
            get { return ID; }
        }

        public Type SupportedEntryType
        {
            get { return typeof(BlogEntryDTO); }
        }

        public string GetDayInfoText(IEnumerable<EntryObjectDTO> entryObjects)
        {
            var blogEntries = entryObjects.Cast<BlogEntryDTO>();
            StringBuilder builder = new StringBuilder();
            foreach (var blogEntry in blogEntries)
            {
                if (blogEntry.AllowComments)
                {
                    builder.AppendFormat(BlogEntryStrings.BlogCalendarDayContent_GetDayInfoText_CommentsEnabled, blogEntry.BlogCommentsCount, blogEntry.LastCommentDate);
                }
                else
                {
                    builder.AppendLine(BlogEntryStrings.BlogCalendarDayContent_GetDayInfoText_CommentsDisabled);
                }
            }
            return builder.ToString();
        }


        public Color GetBackColor(IEnumerable<EntryObjectDTO> entryObjects)
        {
            BlogEntryDTO blog = (BlogEntryDTO)entryObjects.Single();
            DateTime lastVisit = BlogSettings.Default.GetVisitDate(blog.TrainingDay.TrainingDate,UserContext.CurrentProfile.Id, blog.TrainingDay.ProfileId);
            if (blog.LastCommentDate.HasValue && lastVisit < blog.LastCommentDate)
            {
                return Color.DarkOrange;
            }
            else
            {
                return Color.DarkKhaki;
            }
        }

        public string Name
        {
            get { return BlogEntryStrings.BlogCalendarDayContent_Name; }
        }

        public Image Image
        {
            get { return BlogResources.BlogModule; }
        }

        public void PrepareData()
        {
        }
    }
}
