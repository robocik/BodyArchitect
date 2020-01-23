using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;

namespace BodyArchitectClientsManager
{
    internal interface IReport
    {
        void GenerateReport(Chart chartControl, ClientsInfo result);
        string ReportName { get; }
    }
}
