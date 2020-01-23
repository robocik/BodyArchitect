using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using BodyArchitect.Client.UI;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.Suplements
{
    public class WorkoutPlanPurposeToLocalizedStringConverter:IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            WorkoutPlanPurpose exerciseType = (WorkoutPlanPurpose)value;
            return EnumLocalizer.Default.Translate(exerciseType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
