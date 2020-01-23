using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using BodyArchitect.Service.Model;

namespace BodyArchitect.Common.Reporting
{
    public interface IReport
    {
        void GenerateReport(Chart chartControl,IList<TrainingDayDTO> days);
        void Initialize();
        string ReportName { get; }
        Control SettingsControl { get; }
        bool CanGenerateReport { get; }
        WorkoutDaysSearchCriteria GetWorkoutDaysCriteria();
        event EventHandler  CanGenerateReportChanged;
    }
}
