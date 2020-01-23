using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.Controls.Model;
using BodyArchitect.WP7.ViewModel;
using LengthType = BodyArchitect.Service.V2.Model.LengthType;
using WeightType = BodyArchitect.Service.V2.Model.WeightType;

namespace BodyArchitect.WP7
{
    public class SetsToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            StringBuilder builder = new StringBuilder();
            ObservableCollection<SetViewModel> col = (ObservableCollection<SetViewModel>)value;
            foreach (var setViewModel in col)
            {
                builder.AppendFormat("{0}   ", setViewModel.Set.GetDisplayText(false));
            }
            return builder.ToString().Trim();
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SizesChangeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var col = (decimal)value;
            
            return col==0?Visibility.Collapsed:Visibility.Visible;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SizesChangeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            string format = "#.##";
            if(parameter!=null)
            {
                format = "#";
            }
            var col = (decimal)value;
            string str = "(+0)";
            if (col < 0)
            {
                str = col.ToString(format);
            }
            if (col > 0)
            {
                str = "+" + col.ToString(format);
            }
            return str;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SizesChangeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var col = (decimal)value;
            Brush brush = new SolidColorBrush(Colors.Gray);
            if(col<0)
            {
                brush= new SolidColorBrush(Colors.Red);
            }
            if(col>0)
            {
                brush = new SolidColorBrush(Colors.Green);
            }
            return brush;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SuperSetToBrushConverter : IValueConverter
    {
        SuperSetViewManager viewManager = new SuperSetViewManager();
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var item = (StrengthTrainingItemDTO) value;
            if(string.IsNullOrEmpty(item.SuperSetGroup))
            {
                
                return (Brush)Application.Current.Resources["CustomSubtleBrush"];
            }
            return new SolidColorBrush(viewManager.GetSuperSetColor(item.SuperSetGroup));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class MergeStateToBrushConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            MergeState state = (MergeState) value;
            if(state==MergeState.None)
            {
                return new SolidColorBrush(Colors.Gray);
            }
            else if(state==MergeState.Processing)
            {
                return new SolidColorBrush(Colors.Green);
            }
            else if(state==MergeState.Error)
            {
                return new SolidColorBrush(Colors.Red);
            }
            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class TrainingPlanSuperSetToBrushConverter : IValueConverter
    {
        SuperSetViewManager viewManager = new SuperSetViewManager();
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var entry = (TrainingPlanEntry)value;
            //var superSet = entry.Day.GetSuperSet(entry);
            //if (superSet == null)
            //{

            //    return (Brush)Application.Current.Resources["CustomSubtleBrush"];
            //}
            //return new SolidColorBrush(viewManager.GetSuperSetColor(superSet.SuperSetId.ToString()));
            if (string.IsNullOrEmpty(entry.GroupName))
            {

                return (Brush)Application.Current.Resources["CustomSubtleBrush"];
            }
            return new SolidColorBrush(viewManager.GetSuperSetColor(entry.GroupName));

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class NegationConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var b = (bool)value;
            return !b;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class GroupToBrushValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var group = value as IInGroup;
            object result = null;

            if (group != null)
            {
                if (!group.HasItems)
                {
                    result = (SolidColorBrush)Application.Current.Resources["CustomChromeBrush"];
                }
                else
                {
                    result = (SolidColorBrush)Application.Current.Resources["CustomAccentFullBrush"];
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ListToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var col = (ICollection) value;
            return col.Count>0 ? Visibility.Visible : Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EmptyListToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var col = (ICollection)value;
            return col.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    

    public class MessagePriorityToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)(MessagePriority)value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (MessagePriority)(int)value;
        }
    }

    public class IntensitytToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)(Intensity)value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Intensity)(int)value;
        }
    }

    public class EntryStatusToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((EntryObjectStatus)value) == EntryObjectStatus.Done;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value ? EntryObjectStatus.Done : EntryObjectStatus.Planned);
        }
    }

    public class ReportStatusToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((ReportStatus)value)==ReportStatus.ShowInReport;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value?ReportStatus.ShowInReport:ReportStatus.SkipInReport);
        }
    }

    public class DosageTypeToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)(DosageType)value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (DosageType)(int)value;
        }
    }

    public class TimeTypeToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)(TimeType)value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (TimeType)(int)value;
        }
    }

    public class PrivacyToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)(Privacy)value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Privacy)(int)value;
        }
    }

    public class DropSetToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)(DropSetType)value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (DropSetType)(int)value;
        }
    }

    public class SetTypeToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)(SetType)value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (SetType)(int)value;
        }
    }

    public class MergeActionToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)(MergeAction)value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (MergeAction)(int)value;
        }
    }


    public class FloatToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string format = (string)parameter;
            var val = (float)value;
            return val.ToString(format);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class UtcDateTimeToLocalRelativeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (DateTime)value;
            return val.ToLocalTime().ToRelativeDate();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DecimalToStringIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = System.Convert.ToDecimal(value);
            //return string.Format("{0:F1}",val.ToDisplayWeight());
            return val.ToDisplayWeight().ToString("#");
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = (string)value;
            int weight = 0;
            int.TryParse(val, out weight);

            return (decimal)weight;
        }
    }

    public class DecimalToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = System.Convert.ToDecimal(value);
            //return string.Format("{0:F1}",val.ToDisplayWeight());
            return val.ToDisplayWeight().ToString("#.##");
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = (string)value;
            decimal weight = 0;

            var nfi = new CultureInfo(CultureInfo.CurrentCulture.Name);
            nfi.NumberFormat.NumberDecimalSeparator = ".";
            //float.TryParse(val, out weight, nfi);

            decimal.TryParse(val, NumberStyles.Float, nfi, out weight);

            return weight;
        }
    }

    public class WeightToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = System.Convert.ToDecimal(value);
            //return string.Format("{0:F1}",val.ToDisplayWeight());
            return val.ToDisplayWeight().ToString("#.##");
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = (string) value;
            float weight = 0;

            var nfi = new CultureInfo(CultureInfo.CurrentCulture.Name);
            nfi.NumberFormat.NumberDecimalSeparator = ".";
            //float.TryParse(val, out weight, nfi);

            float.TryParse(val,NumberStyles.Float,nfi.NumberFormat,out weight);

            if (ApplicationState.Current.SessionData.Profile.Settings.WeightType == WeightType.Pounds)
            {
                weight= weight*0.454f;
            }
            return weight;
        }
    }

    public class LengthFloatToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (decimal)value;
            return val.ToDisplayLength().ToString("#.##");
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = (string)value;
            float weight = 0;

            var nfi = new CultureInfo(CultureInfo.CurrentCulture.Name);
            nfi.NumberFormat.NumberDecimalSeparator = ".";

            float.TryParse(val,NumberStyles.Float,nfi.NumberFormat, out weight);
            if (ApplicationState.Current.SessionData.Profile.Settings.LengthType == LengthType.Inchs)
            {
                weight = weight * 2.54f;
            }
            return weight;
        }
    }

    public class LengthIntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (decimal)value;
            return val.ToDisplayLength().ToString("#");
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = (string)value;
            float weight = 0;
            float.TryParse(val, out weight);
            if (ApplicationState.Current.SessionData.Profile.Settings.LengthType == LengthType.Inchs)
            {
                weight = weight * 2.54f ;
            }
            return (int)weight;
        }
    }

    public class TextAlignmentToHorizontalAlignment : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (TextAlignment)value;
            switch (val)
            {
                case TextAlignment.Center:
                    return HorizontalAlignment.Center;
                case TextAlignment.Right:
                    return HorizontalAlignment.Right;
                case TextAlignment.Left:
                    return HorizontalAlignment.Left;
                    
            }
            return HorizontalAlignment.Stretch;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (HorizontalAlignment)value;
            switch (val)
            {
                case HorizontalAlignment.Center:
                    return TextAlignment.Center;
                case HorizontalAlignment.Right:
                    return TextAlignment.Right;
                case HorizontalAlignment.Left:
                    return TextAlignment.Left;

            }
            return TextAlignment.Justify;
        }
    }
}
