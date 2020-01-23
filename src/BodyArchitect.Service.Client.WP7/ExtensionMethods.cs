using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Linq;
using BodyArchitect.Portable;
using BodyArchitect.Portable.Exceptions;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Service.Client.WP7
{
    

    public static class ExtensionMethods
    {
        public static IList<ValidationResult> ToValidationResults(this ValidationFault fault)
        {
            List<ValidationResult> result = new List<ValidationResult>();
            foreach (ValidationDetail detail in fault.Details)
            {
                result.Add(new ValidationResult(detail.Message, detail.Key, detail.Tag));
            }
            return result;
        }

        public static decimal ToDisplayWeight(this decimal kg)
        {
            if (ApplicationState.Current.ProfileInfo.Settings.WeightType == Service.V2.Model.WeightType.Pounds)
            {
                kg = kg / 0.454m;
            }
            return kg;
        }

        public static decimal ToDisplayPace(this decimal mPerSec)
        {
            return (decimal)WilksFormula.SpeedToPace((float)mPerSec, ApplicationState.Current.ProfileInfo.Settings.LengthType == Service.V2.Model.LengthType.Inchs);
        }

        public static decimal ToDisplaySpeed(this decimal mPerSec)
        {
            var kmPerHour = mPerSec*3.6m;
            if (ApplicationState.Current.ProfileInfo.Settings.LengthType == Service.V2.Model.LengthType.Inchs)
            {
                kmPerHour = kmPerHour*0.621371m;
            }
            return kmPerHour;
        }

        public static decimal ToDisplayLength(this decimal cm)
        {
            if (ApplicationState.Current.ProfileInfo.Settings.LengthType == Service.V2.Model.LengthType.Inchs)
            {
                cm = cm / 2.54m;
            }
            return cm;
        }

        public static decimal ToDisplayAltitude(this decimal m)
        {
            if (ApplicationState.Current.ProfileInfo.Settings.LengthType == Service.V2.Model.LengthType.Inchs)
            {
                m = m*3.2808m;
            }
            return m;
        }

        public static TimeSpan ToDisplayDuration(this decimal seconds)
        {
            return TimeSpan.FromSeconds((int)seconds);
        }

        public static decimal ToDisplayDistance(this decimal m)
        {
            var km = m/1000;//to kilometers
            if (ApplicationState.Current.ProfileInfo.Settings.LengthType == Service.V2.Model.LengthType.Inchs)
            {
                km = km * 0.621371m;
            }
            return km;
        }

        public static decimal FromDisplayDistance(this decimal miles)
        {
            if (ApplicationState.Current.ProfileInfo.Settings.LengthType == Service.V2.Model.LengthType.Inchs)
            {
                miles = miles * 1.609344m;//to km
            }
            return miles*1000;//to meters
        }

        public static bool IsFuture(this DateTime date)
        {
            return date > DateTime.Now.Date;
        }

        public static T Copy<T>(this T oSource,bool ignoreId=false)
        {
            
            //T oClone;
            //DataContractSerializer dcs = new DataContractSerializer(typeof(T));

            //using (MemoryStream ms = new MemoryStream())
            //{
            //    dcs.WriteObject(ms, oSource);
            //    ms.Position = 0;
            //    oClone = (T)dcs.ReadObject(ms);
            //}
            
            //return oClone;
            //BUG FII
            SilverlightSerializer.Reset();
            var tmp = SilverlightSerializer.IgnoreIds;
            SilverlightSerializer.IgnoreIds = ignoreId;
            var c=SilverlightSerializer.Serialize(oSource);
            var res= (T)SilverlightSerializer.Deserialize(c);
            SilverlightSerializer.IgnoreIds = tmp;


            return res;
        }


        public static DateTime MonthDate(this DateTime date)
        {
            return new DateTime(date.Year, date.Month,1);
        }

        public static bool IsModified(this object currentCopy,object original)
        {
            string orgSum=SilverlightSerializer.GetChecksum(original);
            string copySum = SilverlightSerializer.GetChecksum(currentCopy);
            return orgSum != copySum;
        }
        
    }
}
