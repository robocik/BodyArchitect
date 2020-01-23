using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;

namespace BodyArchitectClientsManager
{
    class FirstRaport : IReport
    {
        public void GenerateReport(Chart chart1, ClientsInfo result)
        {
            chart1.Series.Clear();
            var title = new Title(ReportName);
            chart1.Titles.Add(title);

            Series series = new Series();
            series.ChartType = SeriesChartType.Line;
            series.IsValueShownAsLabel = true;
            chart1.Series.Add(series);

            foreach (var pair in splitByDate(result))
            {
                series.Points.AddXY(pair.Key, pair.Value);
            }
        }

        private Dictionary<DateTime,int> splitByDate(ClientsInfo result)
        {
            Dictionary<DateTime,int> res= new Dictionary<DateTime, int>();
            foreach (var client in result.Clients)
            {
                if(!res.ContainsKey(client.Date.Date))
                {
                    res.Add(client.Date.Date,0);
                }
                res[client.Date.Date] = res[client.Date.Date]+1;
            }
            return res;
        }

        public string ReportName
        {
            get { return "First"; }
        }

        public override string ToString()
        {
            return ReportName;
        }
    }
}
