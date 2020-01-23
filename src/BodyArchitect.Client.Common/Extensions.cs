using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Windows.Media;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;

namespace BodyArchitect.Client.Common
{
    public static class Extensions
    {
        public static bool CanBeManuallyAdded(this Type obj)
        {
            var attributes = obj.GetCustomAttributes(typeof(EntryObjectInstanceAttribute), true);
            if (attributes.Length > 0 && ((EntryObjectInstanceAttribute)attributes[0]).Instance == EntryObjectInstance.None)
            {
                return false;
            }
            return true;
        }
        public static bool IsEmpty(this BAGlobalObject obj)
        {
            if (obj == null || obj.GlobalId == Guid.Empty)
            {
                return true;
            }
            return false;
        }

        public static MemberInfo GetMemberInfo(this Expression expression)
        {
            var lambda = (LambdaExpression)expression;

            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else
                memberExpression = (MemberExpression)lambda.Body;

            return memberExpression.Member;
        }

        //public static DateTime StartOfWeek(this DateTime dt)
        //{
        //    System.Globalization.CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
        //    DayOfWeek fdow = ci.DateTimeFormat.FirstDayOfWeek;
        //    return dt.AddDays(-(dt.DayOfWeek - fdow));
        //}

        public static DateTime StartOfWeek(this DateTime dt)
        {
            System.Globalization.CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
            DayOfWeek startOfWeek = ci.DateTimeFormat.FirstDayOfWeek;
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }

        public static Color ToMediaColor(this System.Drawing.Color color)
        {
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static string GetKnownColorName(this Color clr)
        {
            Color clrKnownColor;

            //Use reflection to get all known colors
            Type ColorType = typeof(System.Windows.Media.Colors);
            PropertyInfo[] arrPiColors = ColorType.GetProperties(BindingFlags.Public | BindingFlags.Static);

            //Iterate over all known colors, convert each to a <Color> and then compare
            //that color to the passed color.
            foreach (PropertyInfo pi in arrPiColors)
            {
                clrKnownColor = (Color)pi.GetValue(null, null);
                if (clrKnownColor == clr) return pi.Name;
            }

            return string.Empty;
        }


        public static System.Drawing.Color ToDrawingColor(this Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

    }
}
