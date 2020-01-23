using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace BodyArchitect.Service.V2.Model
{
    public partial class TrainingPlanSerie
    {
        public const string Separator = "-";

        public string ToStringRepetitionsRange()
        {
            string format = "{0}-{1}";
            if (RepetitionNumberMin == RepetitionNumberMax)
            {
                format = "{0}";
            }
            else if (RepetitionNumberMin == null)
            {
                format = "-{1}";
            }
            else if (RepetitionNumberMax == null)
            {
                format = "{0}-";
            }
            return string.Format(format, RepetitionNumberMin, RepetitionNumberMax);
        }



        public void FromString(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                RepetitionNumberMax = RepetitionNumberMin = null;
                //RepetitionsType = TrainingPlanSerieRepetitions.Normalna;
                return;
            }
            int? repMin, repMax;
            int index = text.IndexOf(Separator);
            if (index > -1)
            {
                int tempMin;

                if (int.TryParse(text.Substring(0, index), out tempMin))
                {
                    repMin = tempMin;
                }
                else
                {
                    repMin = null;
                }
                string maxString = text.Substring(index + 1, text.Length - index - 1);
                int tempMax;
                if (int.TryParse(maxString, out tempMax))
                {
                    repMax = tempMax;
                }
                else
                {
                    repMax = null;
                }
                if (repMin != null && repMax != null && repMin.Value > repMax.Value)
                {
                    throw new ArgumentException();
                }
            }
            else
            {
                repMin = repMax = int.Parse(text);
            }
            RepetitionNumberMax = repMax;
            RepetitionNumberMin = repMin;
        }
    }
}
