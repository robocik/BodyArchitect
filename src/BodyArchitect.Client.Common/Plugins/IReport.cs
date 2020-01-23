using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using Chart = Visiblox.Charts.Chart;

namespace BodyArchitect.Client.Common.Plugins
{
    public interface IBAReportBuilder
    {
        string ReportName { get; }
        IBAReport CreateReport();
    }

    public interface IBAReport
    {
        void GenerateReport(Chart chartControl, IEnumerable data);
        IEnumerable RetrieveReportData(object param);
        void Initialize(UserDTO user, CustomerDTO customer);
        string ReportName { get; }
        Control SettingsControl { get; }
        bool CanGenerateReport { get; }
        event EventHandler CanGenerateReportChanged;
        void Close();
    }
}
