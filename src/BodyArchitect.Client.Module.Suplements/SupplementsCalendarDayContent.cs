using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.Suplements
{
    //[Export(typeof(ICalendarDayContent))]
    //public class SupplementsCalendarDayContent : ICalendarDayContent
    //{
    //    //public static readonly Guid ID = new Guid("A2A33E09-44C7-4FBC-A7C4-6D84F7B88D73");
    //    public static readonly Guid ID = new Guid("BD58BEAE-7E5F-424F-92A7-75CA132508FB");

    //    public Guid GlobalId
    //    {
    //        get { return ID; }
    //    }

    //    public Type SupportedEntryType
    //    {
    //        get { return typeof(SuplementsEntryDTO); }
    //    }

    //    public string GetDayInfoText(IEnumerable<EntryObjectDTO> entryObjects)
    //    {
    //        var supplementEntries = entryObjects.Cast<SuplementsEntryDTO>();
    //        StringBuilder builder = new StringBuilder();

    //        foreach (var sizeEntry in supplementEntries)
    //        {
    //            foreach (var item in sizeEntry.Items)
    //            {
    //                builder.AppendLine(string.Format("{0}: {1} {2}",item.Suplement.Name ,item.Dosage, EnumLocalizer.Default.Translate(item.DosageType)));
    //            }
    //        }

    //        return builder.ToString();
    //    }

    //    public Color GetBackColor(IEnumerable<EntryObjectDTO> entryObjects)
    //    {
    //        return Colors.LightGreen;
    //    }

    //    public string Name
    //    {
    //        get { return SuplementsEntryStrings.EntryTypeName; }
    //    }

    //    public ImageSource Image
    //    {
    //        get
    //        {
    //            return "pack://application:,,,/BodyArchitect.Client.Module.Suplements;component/Resources/Supplements.png".ToBitmap();
    //        }
    //    }

    //}
    [Export(typeof(ICalendarDayContextEx))]
    public class SupplementsCalendarDayContentEx : ICalendarDayContextEx
    {
        //public static readonly Guid ID = new Guid("A2A33E09-44C7-4FBC-A7C4-6D84F7B88D73");
        public static readonly Guid ID = new Guid("BD58BEAE-7E5F-424F-92A7-75CA132508FB");

        public Guid GlobalId
        {
            get { return ID; }
        }

        public string Name
        {
            get { return SuplementsEntryStrings.EntryTypeName; }
        }

        public ImageItem[] GetDayContents(TrainingDayDTO day)
        {
            List<ImageItem> items = new List<ImageItem>();
            foreach (var supple in day.Objects.OfType<SuplementsEntryDTO>())
            {
                StringBuilder builder = new StringBuilder();

                foreach (var suppleItem in supple.Items)
                {
                    string name = suppleItem.Suplement.Name;
                    if(!string.IsNullOrEmpty(suppleItem.Name))
                    {
                        name = suppleItem.Name;
                    }
                    builder.AppendLine(string.Format("{0}: {1} {2}", name, suppleItem.Dosage.ToString("0.##"), EnumLocalizer.Default.Translate(suppleItem.DosageType)));
                }
                ImageItem item = new ImageItem();
                item.BackBrush = EntryObjectColors.Supplements;
                item.Content = builder.ToString();
                item.Entry = supple;
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
                return "pack://application:,,,/BodyArchitect.Client.Module.Suplements;component/Resources/Supplements.png".ToBitmap();
            }
        }

    }
}
