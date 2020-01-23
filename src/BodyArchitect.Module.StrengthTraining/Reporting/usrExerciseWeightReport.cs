using System;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Module.StrengthTraining.Localization;
using DevExpress.XtraEditors.Controls;
using LinqKit;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Data;
using System.Linq;
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
using BodyArchitect.Module.StrengthTraining.Model;


namespace BodyArchitect.Module.StrengthTraining.Reporting
{
    using TrainingPlanCondition = System.Linq.Expressions.Expression<System.Func<StrengthTrainingItemDTO, bool>>;
    using TrainingPlanConditionCombinator =
        System.Func<System.Linq.Expressions.Expression<System.Func<StrengthTrainingItemDTO, bool>>,
        System.Linq.Expressions.Expression<System.Func<StrengthTrainingItemDTO, bool>>,
        System.Linq.Expressions.Expression<System.Func<StrengthTrainingItemDTO, bool>>>;
    

    [Export(typeof(IReport))]
    public partial class usrExerciseWeightReport : usrBaseControl, IReport
    {
        public usrExerciseWeightReport()
        {
            InitializeComponent();
            fillSuperTips();
            
            foreach (var value in Enum.GetValues(typeof(SetType)))
            {
                CheckedListBoxItem item = new CheckedListBoxItem(value, EnumLocalizer.Default.Translate((SetType)value), CheckState.Checked);
                cmbSetTypes.Properties.Items.Add(item);
            }
        }

        void fillSuperTips()
        {
            ControlHelper.AddSuperTip(this.defaultToolTipController1.DefaultController, this.listView1, null, StrengthTrainingEntryStrings.usrExerciseWeightReport_ListView);
            ControlHelper.AddSuperTip(this.txtRepetitionFrom, null, StrengthTrainingEntryStrings.usrExerciseWeightReport_RepetitionsRange);
            ControlHelper.AddSuperTip(this.txtRepetitionsTo, null, StrengthTrainingEntryStrings.usrExerciseWeightReport_RepetitionsRange);
            ControlHelper.AddSuperTip(this.chkIncludeSetsWithoutBarrelWeight, chkIncludeSetsWithoutBarrelWeight.Text, StrengthTrainingEntryStrings.usrExerciseWeightReport_chkIncludeSetsWithoutBarrelWeight);
            //ControlHelper.AddSuperTip(this.chkIncludeSetsWithHelp, chkIncludeSetsWithHelp.Text, StrengthTrainingEntryStrings.usrExerciseWeightReport_chkIncludeSetsWithHelp);
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
            get { return StrengthTrainingEntryStrings.Report_ExerciseWeight_Name; }
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
            if(CanGenerateReportChanged!=null)
            {
                CanGenerateReportChanged(this, EventArgs.Empty);
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

        public void GenerateReport(Chart chart1, IList<TrainingDayDTO> days)
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
                //getData(exercise.GlobalId, series);
            }
            getData(exSeries,days);
            var chartArea = chart1.ChartAreas[usrReporting.DefaultChartAreaName ];

            chartArea.AxisX.Title = StrengthTrainingEntryStrings.Report_ExerciseWeight_AxisXTitle;

            chartArea.AxisY.Title = StrengthTrainingEntryStrings.Report_ExerciseWeight_AxisYTitle;

            // Enable X axis margin
            chartArea.AxisX.IsMarginVisible = true;

            // Show as 3D
            chartArea.Area3DStyle.Enable3D = false;
            chartArea.AxisY.IsStartedFromZero = false;

        }

        void getData(Dictionary<Guid, Series> exSeries, IList<TrainingDayDTO> days)
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
                        SelectMany(t => t.Entries.Where(y=>bigOr.Invoke(y)));



            foreach (var item in newWay)
            {
                double weight = getMaxWeight(item.Series, txtRepetitionFrom.Value, txtRepetitionsTo.Value);
                if (weight > 0)
                {
                    exSeries[item.ExerciseId].Points.AddXY(item.StrengthTrainingEntry.TrainingDay.TrainingDate, weight);
                }
            }

        }

        bool getSetTypeCheckState(SetType type)
        {
            foreach (CheckedListBoxItem item in cmbSetTypes.Properties.Items)
            {
                if((SetType)item.Value==type)
                {
                    return item.CheckState == CheckState.Checked;
                }
            }
            return false;
        }
        double getMaxWeight(IEnumerable<SerieDTO> series,decimal repsFrom,decimal repsTo)
        {
            double maxWeight = 0;
            foreach (var serie in series)
            {
                if (serie.Weight.HasValue)
                {
                    if ((serie.RepetitionNumber.HasValue && repsFrom > 0 && serie.RepetitionNumber >= repsFrom || repsFrom == 0) && (repsTo == 0 || serie.RepetitionNumber.HasValue && serie.RepetitionNumber <= repsTo))
                    {
                        if (chkIncludeSetsWithoutBarrelWeight.Checked || !serie.IsCiezarBezSztangi)
                        {
                            if (getSetTypeCheckState(serie.SetType))
                            {
                                maxWeight = Math.Max(maxWeight, serie.Weight.Value);
                            }
                        }
                    }
                }
            }
            return maxWeight;
        }

        public override string ToString()
        {
            return ReportName;
        }

        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            OnCanGenerateReportChanged();
        }
    }
}
