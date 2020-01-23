using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using BodyArchitect.Common.Plugins;
using BodyArchitect.Controls.External;
using BodyArchitect.Common;
using BodyArchitect.Service.Model;
using BodyArchitect.Settings.Model;

namespace BodyArchitect.Controls.UserControls
{
    public class CalendarViewDayInfo:ICalendarViewDayInfo
    {
        private ICalendarDayContent DayContent
        {
            get
            {
                ICalendarDayContent dayContent;
                //if(dayContent==null)
                {
                    var options = UserContext.Settings.GuiState.CalendarOptions;
                    dayContent = PluginsManager.Instance.GetCalendarDayContent(options.CalendarTextType);

                    if (dayContent == null && PluginsManager.Instance.CalendarDayContents.Length > 0)
                    {
                        dayContent = PluginsManager.Instance.CalendarDayContents[0];
                    }
                }
                return dayContent;
            }
        }
        public DateItem AddDayInfo(MonthCalendar calendar, TrainingDayDTO day)
        {
            var options = UserContext.Settings.GuiState.CalendarOptions;
            var groupedEntries = day.SplitByType(!options.ShowMissingPlugins);
            DateItem item = new DateItem();
            Bitmap bitmap = new Bitmap(1, 1);
            bool showUnloaded = false;

            ICalendarDayContent dayContent = DayContent;
            foreach (KeyValuePair<Type, List<EntryObjectDTO>> groupedEntry in groupedEntries)
            {
                if (dayContent != null && dayContent.SupportedEntryType == groupedEntry.Key)
                {

                    item.Text = dayContent.GetDayInfoText(groupedEntry.Value);
                    item.BackColor1 = dayContent.GetBackColor(groupedEntry.Value);
                }

                var entryProvider = PluginsManager.Instance.GetEntryObjectProvider(groupedEntry.Key);
                if (entryProvider != null)
                {
                    bool show = true;
                    if (!options.ShowIcons.TryGetValue(entryProvider.GlobalId, out show) || show)
                    {
                        bitmap = combineImages(bitmap, entryProvider.ModuleImage);
                    }
                }
                else
                {
                    showUnloaded = true;
                }
            }


            if (showUnloaded)
            {
                bitmap = combineImages(bitmap, Icons.PluginMissing);
            }
            
            calendar.RemoveDateInfo(day.TrainingDate);
            item.Image = bitmap;
            item.Date = day.TrainingDate;
            item.Tag = day;
            calendar.AddDateInfo(item);
            return item;
        }

        public void PrepareData()
        {
            DayContent.PrepareData();
        }


        private static System.Drawing.Bitmap combineImages(Bitmap destination,Image imageToAdd)
        {
            int orgWidth = destination.Width;
            int width = destination.Width;
            int height = destination.Height;

            //update the size of the final bitmap
            width += imageToAdd.Width;
            height = imageToAdd.Height > height ? imageToAdd.Height : height;

            var finallImage = new System.Drawing.Bitmap(width, height);
            //get a graphics object from the image so we can draw on it
            using (System.Drawing.Graphics g = Graphics.FromImage(finallImage))
            {
                //set background color
                g.Clear(Color.Transparent);
                g.DrawImage(destination, new System.Drawing.Rectangle(0, 0, destination.Width, destination.Height));
                //go through each image and draw it on the final image
                int offset = orgWidth;
                g.DrawImage(imageToAdd, new System.Drawing.Rectangle(offset, 0, imageToAdd.Width, imageToAdd.Height));
            }

            return finallImage;
        }

    }

    
}
