using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;

namespace BodyArchitect.Service.V2
{
    static class FilterEngine
    {
        public static void Filter(IEnumerable list,RetrievingInfo retrievingInfo)
        {
            foreach (var item in list)
            {
                var properties = item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo propertyInfo in properties)
                {
                    var attributes = propertyInfo.GetCustomAttributes(typeof(FilterAttribute), true);
                    if (attributes.Length > 0)
                    {
                        var filterAttr = (FilterAttribute)attributes[0];
                        if((filterAttr.FilterType==FilterType.Image && !retrievingInfo.Images)
                            || (filterAttr.FilterType==FilterType.LongText && !retrievingInfo.LongTexts))
                        {
                            propertyInfo.SetValue(item,null,null);
                        }
                    }    
                }
                
            }
        }


    }
}
