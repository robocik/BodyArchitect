using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.UI.Converters
{
    public class MessagePriorityToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var priority = (MessagePriority)value;

            string imageName = "PriorityNormal.png";
            switch (priority)
            {
                case MessagePriority.System:
                    imageName = "PrioritySystem.png";
                    break;
                case MessagePriority.High:
                    imageName = "PriorityHigh.png";
                    break;
                case MessagePriority.Low:
                    imageName = "PriorityLow.png";
                    break;
            }
            BitmapImage source = imageName.ToResourceUrl().ToBitmap();
            return source; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
