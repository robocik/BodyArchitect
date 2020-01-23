using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Shared
{
    public enum FilterType
    {
        Image,
        LongText
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class FilterAttribute:Attribute
    {
        public FilterAttribute(FilterType filterType)
        {
            FilterType = filterType;
        }

        public FilterType FilterType { get; set; }
    }
}
