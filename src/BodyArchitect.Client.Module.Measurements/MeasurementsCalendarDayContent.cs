using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Settings.Model;

namespace BodyArchitect.Client.Module.Measurements
{
    [Export(typeof(ICalendarDayContextEx))]
    public class MeasurementsCalendarDayContent : ICalendarDayContextEx
    {
        //public static readonly Guid ID = new Guid("33B76948-6F93-4281-AE7D-F2E9141588C1");
        public static readonly Guid ID = new Guid("083F5171-0D6C-46B4-B72A-155DB3C0270A");

        #region Implementation of ICalendarDayContent

        public Guid GlobalId
        {
            get { return ID; }
        }

        void addLine(decimal value, string label, StringBuilder builder, string unitType)
        {
            if (value > 0)
            {
                builder.AppendLine(string.Format("{0} {1:#.##}{2}", label.Replace("_", ""), value, unitType));
            }
        }


        public string Name
        {
            get { return SizeEntryStrings.EntryTypeName; }
        }

        public ImageItem[] GetDayContents(TrainingDayDTO day)
        {
            List<ImageItem> items = new List<ImageItem>();
            foreach (var size in day.Objects.OfType<SizeEntryDTO>())
            {
                string weightUnitType = UIHelper.WeightType;
                string lengthUnitType = UIHelper.LengthType;

                StringBuilder builder = new StringBuilder();
                if (size.Wymiary != null)
                {
                    addLine(size.Wymiary.RightBiceps.ToDisplayLength(), "usrWymiaryEditor_RightBicepsLabel".TranslateGUI(), builder, lengthUnitType);
                    addLine(size.Wymiary.LeftBiceps.ToDisplayLength(), "usrWymiaryEditor_LeftBicepsLabel".TranslateGUI(), builder, lengthUnitType);
                    addLine(size.Wymiary.Klatka.ToDisplayLength(), "usrWymiaryEditor_KlatkaLabel".TranslateGUI(),builder, lengthUnitType);
                    addLine(size.Wymiary.RightForearm.ToDisplayLength(), "usrWymiaryEditor_RightForearmLabel".TranslateGUI(), builder, lengthUnitType);
                    addLine(size.Wymiary.LeftForearm.ToDisplayLength(), "usrWymiaryEditor_LeftForearmsLabel".TranslateGUI(), builder, lengthUnitType);
                    addLine(size.Wymiary.RightUdo.ToDisplayLength(), "usrWymiaryEditor_RightUdoLabel".TranslateGUI(), builder, lengthUnitType);
                    addLine(size.Wymiary.LeftUdo.ToDisplayLength(), "usrWymiaryEditor_LeftUdoLabel".TranslateGUI(), builder, lengthUnitType);
                    addLine(size.Wymiary.Pas.ToDisplayLength(), "usrWymiaryEditor_PasLabel".TranslateGUI(), builder, lengthUnitType);
                    addLine(size.Wymiary.Weight.ToDisplayWeight(), "usrWymiaryEditor_WeightLabel".TranslateGUI(), builder, weightUnitType);
                }
                ImageItem item = new ImageItem();
                item.BackBrush = EntryObjectColors.Measurements;
                item.Content = builder.ToString();
                item.Entry = size;
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
                return "pack://application:,,,/BodyArchitect.Client.Module.Measurements;component/Resources/Measurements.png".ToBitmap();
            }
        }

        #endregion
    }
}
