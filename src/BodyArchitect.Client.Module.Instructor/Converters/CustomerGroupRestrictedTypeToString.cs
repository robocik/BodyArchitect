using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor.Converters
{
    class CustomerGroupRestrictedTypeToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CustomerGroupRestrictedType type = (CustomerGroupRestrictedType) value;
            if(type==CustomerGroupRestrictedType.Full)
            {
                return "usrCustomerGroupDetails_Content_Fully".TranslateInstructor();
            }
            if (type == CustomerGroupRestrictedType.Partially)
            {
                return "usrCustomerGroupDetails_Content_Partially".TranslateInstructor();
            }
            return "usrCustomerGroupDetails_Content_None".TranslateInstructor();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
