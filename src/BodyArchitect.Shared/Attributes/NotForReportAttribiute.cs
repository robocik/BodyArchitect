using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Shared
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NotForReportAttribiute:Attribute
    {
    }
}
