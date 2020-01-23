using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using BodyArchitect.Client.Common;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.Suplements
{
    public class SupplementsDefinitionsToStatusIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var exercise = (SupplementCycleDefinitionDTO)value;
            if (exercise==null)
            {
                return null;
            }
            if (exercise.IsFavorite())
            {
                return "Favorites16.png".ToResourceString();
            }
            else if (exercise.Status == PublishStatus.Published)
            {
                return "StatusPublic16.png".ToResourceString();
            }
            else 
            {
                return "StatusPrivate.png".ToResourceString();
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
