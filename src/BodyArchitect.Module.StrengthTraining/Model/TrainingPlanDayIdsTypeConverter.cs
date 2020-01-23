using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BodyArchitect.Module.StrengthTraining.Model.TrainingPlans
{
    public class TrainingPlanDayIdsTypeConverter : TypeConverter 
    {
        public override object ConvertTo(ITypeDescriptorContext context,
            System.Globalization.CultureInfo culture,
            object value,
            Type destinationType)
        {
            TrainingPlanHtmlExporter htmlExporter = (TrainingPlanHtmlExporter) context.Instance;
            var ids=htmlExporter.GetPrintDaysIds();

            StringBuilder builder = new StringBuilder();
            foreach (var dayId in ids)
            {

                builder.AppendFormat("{0}{1}", htmlExporter.TrainingPlan.GetDay(dayId).Name, TrainingPlanHtmlExporter.DaysSeparator);
            }
            return builder.ToString().TrimEnd(TrainingPlanHtmlExporter.DaysSeparator);
        }

    }
}
