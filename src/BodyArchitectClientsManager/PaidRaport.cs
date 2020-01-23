using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;

namespace BodyArchitectClientsManager
{
    class PaidRaport:IReport
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

            var cos =
                result.Clients.Where(x => x.Version.StartsWith("Full") && x.Version != "Full 1.0.0").OrderBy(x => x.Date)
                    .GroupBy(x => x.Name);

            Dictionary<DateTime, int> dict = new Dictionary<DateTime, int>();
            DateTime date =new DateTime(2011,8,17);
            for (int i =0; i < (DateTime.Now - date).TotalDays; i++)
            {
                dict.Add(date.AddDays(i).Date,0);
            }
            
            foreach (var pair in cos)
            {
                var item= pair.First();
                if(!dict.ContainsKey(item.Date.Date))
                {
                    dict.Add(item.Date.Date,0);
                }
                dict[item.Date.Date]++;
            }

            foreach (var i in dict)
            {
                series.Points.AddXY(i.Key, i.Value);
            }
        }

        public string ReportName
        {
            get { return "Platne"; }
        }

        public override string ToString()
        {
            return ReportName;
        }
    }
}
