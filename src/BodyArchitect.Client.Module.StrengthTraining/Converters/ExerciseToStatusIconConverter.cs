using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.StrengthTraining.Controls;
using BodyArchitect.Client.Module.StrengthTraining.Model;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.StrengthTraining.Converters
{
    class ExerciseToStatusIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var exercise = (ExerciseLightDTO) value;
            if (exercise.IsFavorite())
            {
                return "Favorites16.png".ToResourceString();
            }
            else if (exercise.ProfileId == null)
            {
                return "Global.png".ToResourceString();
            }
            else if (UserContext.IsMine(exercise))
            {
                return "StatusPrivate.png".ToResourceString();
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
