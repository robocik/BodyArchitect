using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using BodyArchitect.Model;

namespace BodyArchitect.DataAccess.Converter.V4_V5
{
    public static class Helper
    {
        static float max = 1000000;//(float)decimal.MaxValue;
        static float min = -1000000;//(float)decimal.MinValue;

        public static SupplementCycleDosage CreateDosage(decimal dosageValue, string name,Suplement supplement, SupplementCycleDayRepetitions repetitions = SupplementCycleDayRepetitions.OnceAWeek, DosageType dosageType = DosageType.MiliGrams, TimeType timeType = TimeType.NotSet)
        {
            var dosage = new SupplementCycleDosage();
            dosage.Dosage = dosageValue;
            dosage.Name = name;
            dosage.DosageType = dosageType;
            dosage.Repetitions = repetitions;
            dosage.Supplement = supplement;
            dosage.TimeType = timeType;
            return dosage;
        }


        public static bool IsInDbFormat(this decimal val)
        {
            return Decimal.Round(val, 2) == val;
        }
        public static decimal ToSafe(this float value)
        {
            if (value > (float)max || value < (float)min)
            {
                value = 0;
            }
            return (decimal)value;
        }


        public static decimal ToSafe(this double value)
        {
            if (value > (double)max || value < (double)min)
            {
                value = 0;
            }
            return (decimal)value;
        }
        public static decimal ToSafe(this int value)
        {
            if (value > (double)max || value < (double)min)
            {
                value = 0;
            }
            return (decimal)value;
        }

        public static Stream GetResource(string resourceName)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            Stream stream = assembly.GetManifestResourceStream(resourceName);
            return stream;
        }
    }
}
