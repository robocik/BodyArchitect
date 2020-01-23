using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Portable
{
    public static class DateHelper
    {
        public static readonly DateTime NullDateTime = DateTime.MinValue;
        
        public static DateTime? GetCorrectDateTime(DateTime time)
        {
            if (time == NullDateTime)
            {
                return null;
            }
            //if we have only time value (0001-01-01 15:24) then we have to set also date time because without this database will throw exception
            if (time.Date == NullDateTime.Date)
            {
                time = DateTime.Now.Date.Add(time.TimeOfDay);
            }
            return time;
        }

        /// <summary>
        /// Get the first day of the month for
        /// any full date submitted
        /// </summary>
        /// <param name="dtDate"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfMonth(DateTime dtDate)
        {
            // set return value to the first day of the month
            // for any date passed in to the method
            // create a datetime variable set to the passed in date
            DateTime dtFrom = dtDate;
            // remove all of the days in the month
            // except the first day and set the
            // variable to hold that date
            dtFrom = dtFrom.AddDays(-(dtFrom.Day - 1));
            // return the first day of the month
            return dtFrom;
        }

        public static decimal RoundToNearestHalf(this decimal value)
        {
            return Math.Round(value * 2) / 2;
        }
        public static bool HasBirthdayToday(this DateTime? birthday)
        {
            if (birthday.HasValue)
            {
                DateTime now = DateTime.UtcNow;
                return birthday.Value.Day == now.Day && birthday.Value.Month == now.Month;
            }
            return false;
        }

        public static int GetAge(this DateTime birthday)
        {
            DateTime now = DateTime.Today;
            int age = now.Year - birthday.Year;
            if (birthday > now.AddYears(-age))
            {
                age--;
            }
            return age;
        }

        /// <summary>
        /// Get the first day of the month for a
        /// month passed by it's integer value
        /// </summary>
        /// <param name="iMonth"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfMonth(int iMonth)
        {
            // set return value to the last day of the month
            // for any date passed in to the method
            // create a datetime variable set to the passed in date
            DateTime dtFrom = new DateTime(DateTime.Now.Year, iMonth, 1);
            
            // remove all of the days in the month
            // except the first day and set the
            // variable to hold that date
            dtFrom = dtFrom.AddDays(-(dtFrom.Day - 1));
            
            // return the first day of the month
            return dtFrom;

        }
        

        /// <summary>
        /// Get the last day of the month for any
        /// full date
        /// </summary>
        /// <param name="dtDate"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfMonth(DateTime dtDate)
        {
            // set return value to the last day of the month
            // for any date passed in to the method

            // create a datetime variable set to the passed in date

            DateTime dtTo = dtDate;
            
            // overshoot the date by a month
            dtTo = dtTo.AddMonths(1);

            // remove all of the days in the next month
            // to get bumped down to the last day of the
            // previous month
            dtTo = dtTo.AddDays(-(dtTo.Day));
            
            // return the last day of the month
            return dtTo;

        }
        
        /// <summary>
        /// Get the last day of a month expressed by it's
        /// integer value
        /// </summary>
        /// <param name="iMonth"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfMonth(int iMonth)
        {
            // set return value to the last day of the month
            // for any date passed in to the method
            
            // create a datetime variable set to the passed in date
            DateTime dtTo = new DateTime(DateTime.Now.Year, iMonth, 1);
            
            // overshoot the date by a month
            dtTo = dtTo.AddMonths(1);
            
            // remove all of the days in the next month
            // to get bumped down to the last day of the
            // previous month
            dtTo = dtTo.AddDays(-(dtTo.Day));
            
            // return the last day of the month
            return dtTo;
        }

        public static DateTime MonthDate(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static int MonthDifference(this DateTime lValue, DateTime rValue)
        {
            return (lValue.Month - rValue.Month) + 12 * (lValue.Year - rValue.Year);
        }

        public static DateTime MoveToNewDate(DateTime time, DateTime newDate)
        {
            TimeSpan timeOnly = time - time.Date;
            return newDate.Date.Add(timeOnly);
        }
    }
}
