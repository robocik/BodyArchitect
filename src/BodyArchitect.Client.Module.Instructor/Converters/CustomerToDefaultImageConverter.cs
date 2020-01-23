using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor.Converters
{
    class CustomerToDefaultImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var customer = (CustomerDTO)value;
            if (customer != null && customer.IsVirtual)
            {
                return "pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Images/CustomerVirtual.png".ToBitmap();
            }
            return CachedImage.DefaultProfile;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    
    }
}
