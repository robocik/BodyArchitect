using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using BodyArchitect.WCF;
using BodyArchitect.Common;
using BodyArchitect.Common.Reporting;
using BodyArchitect.Controls;
using BodyArchitect.Controls.Reporting;
using BodyArchitect.Service.Model;
using BodyArchitect.Settings;
using BodyArchitect.Controls.Localization;

namespace BodyArchitect.Module.Size.Reporting
{
    [Export(typeof(IReport))]
    public partial class usrSizesTimeReport : usrBaseControl, IReport
    {
        public usrSizesTimeReport()
        {
            InitializeComponent();
            fillSuperTips();
        }

        void fillSuperTips()
        {
            ControlHelper.AddSuperTip(this.defaultToolTipController1.DefaultController, this.listView1, null, SizeEntryStrings.usrSizesTimeReport_ListView);
        }

        public void GenerateReport(Chart chart1, IList<TrainingDayDTO> days)
        {
            chart1.Series.Clear();
            var title = new Title(ReportName);
            chart1.Titles.Add(title);
            var exercises = getSelectedProperties();
            Dictionary<PropertyInfo, Series> exSeries = new Dictionary<PropertyInfo, Series>(exercises.Count);
            foreach (var exercise in exercises)
            {
                Series series = new Series(getTranslatedWymiar(exercise.Name));
                series.BorderWidth = 3;
                series.ChartType = SeriesChartType.Line ;
                series.IsValueShownAsLabel = ReportingOptions.Default.ShowValues;
                chart1.Series.Add(series);
                exSeries.Add(exercise, series);
                //getData(exercise.GlobalId, series);
            }
            getData(exSeries,days);
            var chartArea = chart1.ChartAreas[usrReporting.DefaultChartAreaName];
            
            chartArea.AxisX.Title = SizeEntryStrings.Report_SizeTime_AxisXTitle;

            chartArea.AxisY.Title = SizeEntryStrings.Report_SizeTime_AxisYTitle;

            // Enable X axis margin
            chartArea.AxisX.IsMarginVisible = true;

            // Show as 3D
            chartArea.Area3DStyle.Enable3D = false;
            chartArea.AxisY.IsStartedFromZero = false;
        }

        public void Initialize()
        {
            listView1.Items.Clear();

            listView1.AddItem(DomainModelStrings.Wymiary_Chest,"Klatka");
            listView1.AddItem(DomainModelStrings.Wymiary_Height, "Height");
            listView1.AddItem(DomainModelStrings.Wymiary_LeftBiceps, "LeftBiceps");
            listView1.AddItem(DomainModelStrings.Wymiary_LeftForearm, "LeftForearm");
            listView1.AddItem(DomainModelStrings.Wymiary_LeftUdo, "LeftUdo");
            listView1.AddItem(DomainModelStrings.Wymiary_Pas, "Pas");
            listView1.AddItem(DomainModelStrings.Wymiary_RightBiceps, "RightBiceps");
            listView1.AddItem(DomainModelStrings.Wymiary_RightForearm, "RightForearm");
            listView1.AddItem(DomainModelStrings.Wymiary_RightUdo, "RightUdo");
            listView1.AddItem(DomainModelStrings.Wymiary_Weight, "Weight");
        }

        string getTranslatedWymiar(string propertyName)
        {
            foreach (ListViewItem listViewItem in listView1.Items)
            {
                if((string)listViewItem.Tag==propertyName)
                {
                    return listViewItem.Text;
                }
            }
            return propertyName;
        }


        public string ReportName
        {
            get { return SizeEntryStrings.Report_SizesTime_Name; }
        }

        public Control SettingsControl
        {
            get { return this; }
        }

        public bool CanGenerateReport
        {
            get { return listView1.CheckedItems.Count > 0; }
        }

        public WorkoutDaysSearchCriteria GetWorkoutDaysCriteria()
        {
            var criteria = new WorkoutDaysSearchCriteria();
            criteria.StartDate = this.usrDateRange1.DateFrom;
            criteria.EndDate = this.usrDateRange1.DateTo;
            return criteria;
        }

        public event EventHandler CanGenerateReportChanged;

        protected virtual void OnCanGenerateReportChanged()
        {
            if (CanGenerateReportChanged != null)
            {
                CanGenerateReportChanged(this, EventArgs.Empty);
            }
        }

        private List<PropertyInfo> getSelectedProperties()
        {
            List<PropertyInfo> list = new List<PropertyInfo>();
            foreach (ListViewItem item in listView1.CheckedItems)
            {
                list.Add(typeof(WymiaryDTO).GetProperty(item.Tag.ToString()));
            }
            return list;
        }

        void getData(Dictionary<PropertyInfo, Series> exSeries,IList<TrainingDayDTO> days)
        {
            //var criteria = new WorkoutDaysSearchCriteria();
            //criteria.StartDate = this.usrDateRange1.DateFrom;
            //criteria.EndDate = this.usrDateRange1.DateTo;

            //var days = ServiceManager.GetTrainingDays(UserContext.Token, criteria, pageInfo);


            //PagedResultRetriever retriever = new PagedResultRetriever();
            //var res = retriever.GetAll(delegate(PartialRetrievingInfo pageInfo)
            //                               {
            //                                   return ServiceManager.GetTrainingDays(UserContext.Token, criteria,
            //                                                                         pageInfo);
            //                               });

            var newWay =
                days.SelectMany(
                    d =>d.Objects.OfType<SizeEntryDTO>().Where(
                        h => usrReportingEntryStatus1.UseAllEntries || h.ReportStatus == ReportStatus.ShowInReport)).
                    OrderBy(y=>y.TrainingDay.TrainingDate);
            //List<SizeEntryDTO> list = new List<SizeEntryDTO>();
            //foreach (var entry in result)
            //{
            //    list.AddRange(entry);
            //}
            //ISessionFactoryHolder holder = ActiveRecordMediator.GetSessionFactoryHolder();
            //ISession session = holder.CreateSession(typeof(SizeEntry));
            //var criteria = session.CreateCriteria(typeof(SizeEntry));

            //criteria.CreateAlias("TrainingDay", "td");

            //if (usrDateRange1.DateFrom.HasValue)
            //{
            //    criteria.Add(Expression.Ge("td.TrainingDate", usrDateRange1.DateFrom.Value));
            //}
            //if (usrDateRange1.DateTo.HasValue)
            //{
            //    criteria.Add(Expression.Le("td.TrainingDate", usrDateRange1.DateTo.Value));
            //}

            //criteria.Add(Expression.Eq("td.ProfileId", UserContext.SessionData.ProfileId));

            //criteria.SetFetchMode("td.Entries", FetchMode.Join);

            //criteria.AddOrder(new Order("td.TrainingDate", true));
            //if (!usrReportingEntryStatus1.UseAllEntries)
            //{
            //    criteria.Add(Expression.Eq("ReportStatus", ReportStatus.ShowInReport));
            //}
            //var list = criteria.List<SizeEntry>();

            foreach (var item in newWay)
            {
                foreach (var series in exSeries)
                {

                    float value = float.Parse(getValue(series.Key, item.Wymiary).ToString());
                    if (value > 0)
                    {
                        //we convert to decimal because when we used a float type than mschar had problems with precision
                        //it shows 81.099999999 instead of 81.1
                        series.Value.Points.AddXY(item.TrainingDay.TrainingDate, Convert.ToDecimal(value));
                    }
                }
            }

        }

        object getValue(PropertyInfo info, WymiaryDTO obj)
        {
            return info.GetValue(obj, null);
        }

        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            OnCanGenerateReportChanged();
        }

        public override string ToString()
        {
            return ReportName;
        }
    }
}
