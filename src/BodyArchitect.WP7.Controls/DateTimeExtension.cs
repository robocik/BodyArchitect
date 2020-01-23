using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;

namespace BodyArchitect.WP7.Controls
{
    public static class DateTimeExtension
    {
        public static bool IsToday(this DateTime dateTime)
        {
            return dateTime.Date == DateTime.Today;
        }
        static readonly Dictionary<double, Func<TimeSpan, string>> relativeDateMap;

        static readonly Dictionary<double, Func<TimeSpan, string>> calendarDateMap;

        static DateTimeExtension()
        {
            relativeDateMap = new Dictionary<double, Func<TimeSpan, string>>
            {
                { 0.75, x => ApplicationStrings.DateTimeExtension_LessThanMinute },
                { 1.5, x => ApplicationStrings.DateTimeExtension_OneMinute },
                { 45, x => String.Format(ApplicationStrings.DateTimeExtension_XMinutes,Math.Round( Math.Abs(x.TotalMinutes))) },
                { 90, x => ApplicationStrings.DateTimeExtension_OneHour },
                { 60 * 24, x => String.Format(ApplicationStrings.DateTimeExtension_XHours, Math.Round(Math.Abs(x.TotalHours))) }, // 
                { 60 * 48, x => ApplicationStrings.DateTimeExtension_OneDay },
                { 60 * 24 * 30, x => String.Format(ApplicationStrings.DateTimeExtension_XDays, Math.Floor(Math.Abs(x.TotalDays))) },
                { 60 * 24 * 60, x => ApplicationStrings.DateTimeExtension_OneMonth },
                { 60 * 24 * 365, x => String.Format(ApplicationStrings.DateTimeExtension_XMonths, Math.Floor(Math.Abs(x.TotalDays / 30))) },
                { 60 * 24 * 365 * 2, x => ApplicationStrings.DateTimeExtension_OneYear },
                { Double.MaxValue,  x =>x.ToString() }
            };

            calendarDateMap = new Dictionary<double, Func<TimeSpan, string>>
            {
                { 60 * 24, x => ApplicationStrings.DateTimeExtension_Today },
                { 60 * 48, x => ApplicationStrings.DateTimeExtension_Yesterday },
                { 60 * 24 * 30, x => String.Format(ApplicationStrings.DateTimeExtension_XDaysAgo, Math.Floor(Math.Abs(x.TotalDays))) },
                { 60 * 24 * 60, x => ApplicationStrings.DateTimeExtension_OneMonthAgo },
                { 60 * 24 * 365, x => String.Format(ApplicationStrings.DateTimeExtension_XMonthsAgo, Math.Floor(Math.Abs(x.TotalDays / 30))) },
                { 60 * 24 * 365 * 2, x => ApplicationStrings.DateTimeExtension_OneYearAgo },
                { Double.MaxValue, x =>x.ToString() }
            };
        }

        public static string ToRelativeDate(this DateTime input)
        {
            TimeSpan diff = DateTime.Now.Subtract(input);
            if (diff > TimeSpan.FromDays(365 * 2))
            {
                return input.ToString();
            }
            double totalMinutes = diff.TotalMinutes;

            string suffix = ApplicationStrings.DateTimeExtension_AgoPostfix;
            if (!suffix.StartsWith(" "))
            {
                suffix = " " + suffix;
            }
            if (totalMinutes < 0.0)
            {
                totalMinutes = Math.Abs(totalMinutes);
                suffix = ApplicationStrings.DateTimeExtension_FromNowSuffix;
            }

            return relativeDateMap.First(n => totalMinutes < n.Key).Value(diff) + suffix;
        }

        public static string ToCalendarDate(this DateTime input)
        {
            TimeSpan diff = DateTime.Now.Date.Subtract(input.Date);
            double totalMinutes = diff.TotalMinutes;
            return calendarDateMap.First(n => totalMinutes < n.Key).Value(diff);
        }
    }
}
