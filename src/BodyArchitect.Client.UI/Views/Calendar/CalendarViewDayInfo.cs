using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.UI.Controls.Calendar.Common;
using BodyArchitect.Client.UI.UserControls;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Settings;

namespace BodyArchitect.Client.UI.Views
{
    

    public class TrainingDayInfo : DateInfo
    {
        private bool isProcessing;

        public TrainingDayInfo()
        {
            Images=new List<ImageItem>();
        }

        public bool IsProcessing
        {
            get { return isProcessing; }
            set
            {
                isProcessing = value;
                NotifyOfPropertyChange(()=>IsProcessing);
            }
        }

        
        
        public List<ImageItem> Images { get; set; }
    }

    class CalendarViewDayInfo
    {
        public CalendarFilter FilterView { get; set;}

        //private IEnumerable<ICalendarDayContent> DayContents
        //{
        //    get
        //    {

        //        List<ICalendarDayContent> goodOrder = new List<ICalendarDayContent>();

        //        foreach (var goodOrderItem in GuiState.Default.CalendarOptions.DefaultEntries)
        //        {
        //            var dayContent1 = PluginsManager.Instance.GetCalendarDayContent(goodOrderItem.ModuleId);
        //            if (dayContent1 != null)
        //            {
        //                goodOrder.Add(dayContent1);
        //            }
        //        }
        //        return goodOrder;
        //    }
        //}


        public TrainingDayInfo AddDayInfo(TrainingDayDTO day, bool showTrainingDaySummary)
        {

            //var options = UserContext.Settings.GuiState.CalendarOptions;
            //var groupedEntries = day.SplitByType(!options.ShowMissingPlugins);
            TrainingDayInfo item = new TrainingDayInfo();


            foreach(var provider in PluginsManager.Instance.CalendarDayContentsEx)
            {
                if(!showTrainingDaySummary && provider.Value.GlobalId==new Guid("A18353CA-5767-46E5-AEC9-3E5C193E6E15"))
                    //training day summary
                {
                    continue;
                }
                var contents=provider.Value.GetDayContents(day);
                foreach (var imageItem in contents)
                {
                    if ((imageItem.Entry == null && FilterView != CalendarFilter.All) || imageItem.Entry !=null && ( FilterView == CalendarFilter.OnlyDone && imageItem.Entry.Status != EntryObjectStatus.Done
                    || FilterView == CalendarFilter.OnlyPlanned && imageItem.Entry.Status != EntryObjectStatus.Planned))
                    {
                        continue;
                    }
                    if (imageItem.Entry!=null && imageItem.Entry.Status == EntryObjectStatus.Planned)
                    {//for planned entries we must set some transparency
                        imageItem.BackBrush.Opacity = .4;
                    }
                    var optionItem = GuiState.Default.CalendarOptions.DefaultEntries.Where(x => x.ModuleId == provider.Value.GlobalId).SingleOrDefault();
                    imageItem.Order = optionItem == null ? int.MaxValue : optionItem.Order;
                    item.Images.Add(imageItem);
                }

            }

            //foreach (EntryObjectDTO groupedEntry in day.Objects)
            //{
            //    if (FilterView == CalendarFilter.OnlyDone && groupedEntry.Status != EntryObjectStatus.Done
            //        || FilterView == CalendarFilter.OnlyPlanned && groupedEntry.Status != EntryObjectStatus.Planned)
            //    {
            //        continue;
            //    }
            //    string toolTip = groupedEntry.GetEntryObjectText();
            //    var contentPresenter = PluginsManager.Instance.GetCalendarDayContents(groupedEntry.GetType());

            //    if (contentPresenter.Count > 0)
            //    {
            //        var presenter = contentPresenter[0];

            //        string content = presenter.GetDayInfoText(new[] { groupedEntry });
            //        var itemObj = new ImageItem()
            //        {
            //            Image = presenter.Image,
            //            ToolTip = toolTip,
            //            BackBrush = new SolidColorBrush(presenter.GetBackColor(new EntryObjectDTO[] { groupedEntry })),
            //            Content = content
            //        };
            //        if (groupedEntry.Status == EntryObjectStatus.Planned)
            //        {//for planned entries we must set some transparency
            //            itemObj.BackBrush.Opacity = .4;
            //        }

            //        var optionItem = GuiState.Default.CalendarOptions.DefaultEntries.Where(x => x.ModuleId == presenter.GlobalId).SingleOrDefault();
            //        itemObj.Order = optionItem == null ? int.MaxValue : optionItem.Order;
            //        item.Images.Add(itemObj);
            //    }
            //    else
            //    {
            //        BitmapImage source = "pack://application:,,,/BodyArchitect.Client.Resources;component/Images/PluginError.png".ToBitmap();
            //        item.Images.Add(new ImageItem() { Image = source, ToolTip = toolTip });
            //    }
            //}

            item.Date = day.TrainingDate;
            item.Tag = day;
            item.Images = item.Images.OrderBy(x => x.Order).ToList();
            return item.Images.Count > 0 ? item : null;
        }

        //public TrainingDayInfo AddDayInfo( TrainingDayDTO day)
        //{

        //    //var options = UserContext.Settings.GuiState.CalendarOptions;
        //    //var groupedEntries = day.SplitByType(!options.ShowMissingPlugins);
        //    TrainingDayInfo item = new TrainingDayInfo();


            
        //    foreach (EntryObjectDTO groupedEntry in day.Objects)
        //    {
        //        if(FilterView==CalendarFilter.OnlyDone && groupedEntry.Status!=EntryObjectStatus.Done
        //            || FilterView == CalendarFilter.OnlyPlanned && groupedEntry.Status != EntryObjectStatus.Planned)
        //        {
        //            continue;
        //        }
        //        string toolTip = groupedEntry.GetEntryObjectText();
        //        var contentPresenter=PluginsManager.Instance.GetCalendarDayContents(groupedEntry.GetType());
                
        //        if (contentPresenter.Count>0)
        //        {
        //            var presenter = contentPresenter[0];
                    
        //            string content = presenter.GetDayInfoText(new [] {groupedEntry});
        //            var itemObj = new ImageItem()
        //                              {
        //                                  Image = presenter.Image,
        //                                  ToolTip = toolTip,
        //                                  BackBrush = new SolidColorBrush(presenter.GetBackColor(new EntryObjectDTO[] {groupedEntry})),
        //                                  Content = content
        //                              };
        //            if(groupedEntry.Status==EntryObjectStatus.Planned)
        //            {//for planned entries we must set some transparency
        //                itemObj.BackBrush.Opacity = .4;
        //            }

        //            var optionItem = GuiState.Default.CalendarOptions.DefaultEntries.Where(x => x.ModuleId == presenter.GlobalId).SingleOrDefault();
        //            itemObj.Order = optionItem == null ? int.MaxValue : optionItem.Order;
        //            item.Images.Add(itemObj);
        //        }
        //        else
        //        {
        //            BitmapImage source = "pack://application:,,,/BodyArchitect.Client.Resources;component/Images/PluginError.png".ToBitmap();
        //            item.Images.Add(new ImageItem() { Image = source, ToolTip = toolTip });
        //        }
        //    }

        //    item.Date = day.TrainingDate;
        //    item.Tag = day;
        //    item.Images = item.Images.OrderBy(x => x.Order).ToList();
        //    return item.Images.Count>0?item:null;
        //}

    }
}
