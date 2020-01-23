using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Module.StrengthTraining.Localization;
using BodyArchitect.Service.Model;
using BodyArchitect.Common.Reporting;
using BodyArchitect.Controls;
using BodyArchitect.Controls.Reporting;
using BodyArchitect.Settings;
using DevExpress.XtraEditors.Controls;
using LinqKit;

namespace BodyArchitect.Module.StrengthTraining.Reporting
{
    using TrainingPlanCondition = System.Linq.Expressions.Expression<System.Func<StrengthTrainingItemDTO, bool>>;
    using TrainingPlanConditionCombinator =
        System.Func<System.Linq.Expressions.Expression<System.Func<StrengthTrainingItemDTO, bool>>,
        System.Linq.Expressions.Expression<System.Func<StrengthTrainingItemDTO, bool>>,
        System.Linq.Expressions.Expression<System.Func<StrengthTrainingItemDTO, bool>>>;

    [Export(typeof(IReport))]
    public partial class usrWeightRepetitionsReport : usrBaseControl, IReport
    {
        public usrWeightRepetitionsReport()
        {
            InitializeComponent();

            foreach (var value in Enum.GetValues(typeof(SetType)))
            {
                CheckedListBoxItem item = new CheckedListBoxItem(value, EnumLocalizer.Default.Translate((SetType)value), CheckState.Checked);
                cmbSetTypes.Properties.Items.Add(item);
            }
        }

        List<ExerciseDTO> getSelectedExercises()
        {
            List<ExerciseDTO> list = new List<ExerciseDTO>();
            foreach (var item in listView1.CheckedItems)
            {
                list.Add((ExerciseDTO)((ListViewItem)item).Tag);
            }
            return list;
        }

        public void GenerateReport(Chart chart1,IList<TrainingDayDTO> days)
        {
            chart1.Series.Clear();
            var title = new Title(ReportName);
            chart1.Titles.Add(title);
            var exercises = getSelectedExercises();
            Dictionary<Guid, Series> exSeries = new Dictionary<Guid, Series>(exercises.Count);
            foreach (var exercise in exercises)
            {
                Series series = new Series(exercise.GetLocalizedName());
                series.ChartType = SeriesChartType.Line;
                series.IsValueShownAsLabel = ReportingOptions.Default.ShowValues;
                chart1.Series.Add(series);
                series.BorderWidth = 3;
                exSeries.Add(exercise.GlobalId, series);
            }
            getData(exSeries, days);
            var chartArea = chart1.ChartAreas[usrReporting.DefaultChartAreaName];

            chartArea.AxisX.Title = StrengthTrainingEntryStrings.Report_RepsWeight_AxisXTitle;

            chartArea.AxisY.Title = StrengthTrainingEntryStrings.Report_RepsWeight_AxisYTitle;


            // Enable X axis margin
            chartArea.AxisX.IsMarginVisible = true;

            // Show as 3D
            chartArea.Area3DStyle.Enable3D = false;
            chartArea.AxisY.IsStartedFromZero = false;

        }



        void getData(Dictionary<Guid, Series> exSeries,IList<TrainingDayDTO> days)
        {
            TrainingPlanCondition bigOr = m => false;
            foreach (var series in exSeries)
            {
                var guid = series.Key;
                bigOr = bigOr.Or(h => h.ExerciseId == guid);
            }

            var newWay =
                days.SelectMany(d => d.Objects.OfType<StrengthTrainingEntryDTO>().OrderBy(y => y.TrainingDay.TrainingDate).Where(
                        h => usrReportingEntryStatus1.UseAllEntries || h.ReportStatus == ReportStatus.ShowInReport)).
                        SelectMany(t => t.Entries.Where(y => bigOr.Invoke(y)));
            //MessageBox.Show("Must be implemented");
            //ISessionFactoryHolder holder = ActiveRecordMediator.GetSessionFactoryHolder();
            //ISession session = holder.CreateSession(typeof(StrengthTrainingItem));

            //var criteria = session.CreateCriteria(typeof(StrengthTrainingItem));
            //var bigOr = Expression.Disjunction();
            //criteria.CreateAlias("StrengthTrainingEntry", "e");
            //criteria.CreateAlias("e.TrainingDay", "td");

            //foreach (var series in exSeries)
            //{
            //    bigOr.Add(Expression.Eq("ExerciseId", series.Key));
            //}
            //if (usrDateRange1.DateFrom.HasValue)
            //{
            //    criteria.Add(Expression.Ge("td.TrainingDate", usrDateRange1.DateFrom.Value));
            //}
            //if (usrDateRange1.DateTo.HasValue)
            //{
            //    criteria.Add(Expression.Le("td.TrainingDate", usrDateRange1.DateTo.Value));
            //}

            //criteria.Add(bigOr);
            //criteria.Add(Expression.Eq("td.ProfileId", UserContext.SessionData.ProfileId));

            //if (!usrReportingEntryStatus1.UseAllEntries)
            //{
            //    criteria.Add(Expression.Eq("e.ReportStatus", ReportStatus.ShowInReport));
            //}


            //criteria.SetFetchMode("td.Entries", FetchMode.Lazy);
            //criteria.AddOrder(new Order("td.TrainingDate", true));
            //var list = criteria.List<StrengthTrainingItem>();
            Dictionary<Guid, Dictionary<int, float>> exercisesDict = new Dictionary<Guid, Dictionary<int, float>>();
            foreach (var item in newWay)
            {

                if (!exercisesDict.ContainsKey(item.ExerciseId))
                {
                    exercisesDict[item.ExerciseId]=new Dictionary<int, float>();
                }
                foreach (var set in item.Series)
                {
                    if(set.RepetitionNumber.HasValue && set.Weight.HasValue )
                    {
                        if (chkIncludeSetsWithoutBarrelWeight.Checked || !set.IsCiezarBezSztangi)
                        {
                            if (getSetTypeCheckState(set.SetType))
                            {
                                float weight = 0.0f;
                                exercisesDict[item.ExerciseId].TryGetValue(set.RepetitionNumber.Value, out weight);
                                exercisesDict[item.ExerciseId][set.RepetitionNumber.Value] = Math.Max(weight,set.Weight.Value);
                            }
                        }
                    }
                }
            }
            /*if (chkIncludeSetsWithoutBarrelWeight.Checked || !serie.IsCiezarBezSztangi)
                        {
                            if (getSetTypeCheckState(serie.SetType))
                            {
                                maxWeight = Math.Max(maxWeight, serie.Weight.Value);
                            }
                        }
             */
            foreach (var pair in exercisesDict)
            {
                var res = pair.Value.OrderBy(x => x.Key);
                foreach (KeyValuePair<int, float> valuePair in res)
                {
                    exSeries[pair.Key].Points.AddXY(valuePair.Key, valuePair.Value);
                }
                
            }

        }

        bool getSetTypeCheckState(SetType type)
        {
            foreach (CheckedListBoxItem item in cmbSetTypes.Properties.Items)
            {
                if ((SetType)item.Value == type)
                {
                    return item.CheckState == CheckState.Checked;
                }
            }
            return false;
        }

        public override string ToString()
        {
            return ReportName;
        }

        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            OnCanGenerateReportChanged();
        }

        public void Initialize()
        {
            listView1.Items.Clear();
            Dictionary<ExerciseType, ListViewGroup> groups = new Dictionary<ExerciseType, ListViewGroup>();

            foreach (var exercise in ObjectsReposidory.Exercises.Values)
            {
                ListViewItem item = new ListViewItem(exercise.GetLocalizedName());
                item.Tag = exercise;


                if (!groups.ContainsKey(exercise.ExerciseType))
                {
                    groups[exercise.ExerciseType] = new ListViewGroup(exercise.ExerciseType.ToString());
                }

                item.Group = groups[exercise.ExerciseType];
                listView1.Items.Add(item);
            }
            listView1.Groups.AddRange(groups.Values.ToArray());
        }

        public string ReportName
        {
            get { return StrengthTrainingResources.Report_WeightReps_Name; }
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
    }
}
