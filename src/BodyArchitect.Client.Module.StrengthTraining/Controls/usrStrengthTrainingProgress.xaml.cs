using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.StrengthTraining.Reports;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Reports;
using Visiblox.Charts;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls
{
    /// <summary>
    /// Interaction logic for usrStrengthTrainingProgress.xaml
    /// </summary>
    public partial class usrStrengthTrainingProgress
    {
        public usrStrengthTrainingProgress()
        {
            InitializeComponent();
        }

        void generateProgress(StrengthTrainingEntryDTO entry)
        {
            progressIndicator1.IsRunning = true;
            tbLicenceMsg.Text = Strings.ProgressReport_Generating;
            chart.SetVisible(!progressIndicator1.IsRunning);
            var reportBuilder = new ExerciseWeightReportBuilder();
            var report = reportBuilder.CreateReport();

            var settings = (ExerciseWeightReportSettings)report.SettingsControl;
            settings.usrDateRange1.DateFrom = DateTime.UtcNow.AddYears(-1);


            chart.Series.Clear();
            chart.ResetBarAndColumnCaches();
            chart.LegendPosition = LegendPosition.OutsideBottomLeft;
            ReportExerciseWeightParams param = new ReportExerciseWeightParams();
            param.UserId = entry.TrainingDay.ProfileId;
            if (entry.TrainingDay.CustomerId.HasValue)
            {
                param.CustomerId = entry.TrainingDay.CustomerId.Value;
            }
            if (entry.MyPlace != null)
            {
                param.MyPlaces.Add(entry.MyPlace.GlobalId);
            }
            param.StartDate = DateTime.UtcNow.AddYears(-1);

            foreach (var strengthTrainingItemDto in entry.Entries)
            {
                if (strengthTrainingItemDto.Exercise.ExerciseType == ExerciseType.Cardio)
                {//skip cardio exercise because they not use kg - but time
                    continue;
                }
                if (!param.Exercises.Contains(strengthTrainingItemDto.Exercise.GlobalId))
                {
                    param.Exercises.Add(strengthTrainingItemDto.Exercise.GlobalId);
                }
                if (!param.DoneWays.Contains(strengthTrainingItemDto.DoneWay))
                {
                    param.DoneWays.Add(strengthTrainingItemDto.DoneWay);
                }
                settings.Items.Add(new CheckListItem<ExerciseLightDTO>(strengthTrainingItemDto.Exercise.Name, strengthTrainingItemDto.Exercise) { IsChecked = true });
            }

            ThreadPool.QueueUserWorkItem(new WaitCallback((h) =>
            {
                try
                {
                    Helper.EnsureThreadLocalized();
                    var data = report.RetrieveReportData(param);
                    Helper.Delay(2000);
                    UIHelper.BeginInvoke(() =>
                    {

                        report.GenerateReport(chart, data);

                        progressIndicator1.IsRunning = false;
                        chart.SetVisible(!progressIndicator1.IsRunning);
                        tbLicenceMsg.Text = "";

                    }, Dispatcher);
                }
                catch (Exception)
                {
                    UIHelper.BeginInvoke(() =>
                    {
                        progressIndicator1.IsRunning = false;
                        chart.SetVisible(!progressIndicator1.IsRunning);
                        tbLicenceMsg.Text = Strings.ProgressReport_Error;
                    }, Dispatcher);
                }
                
            }));

        }

        public override void Fill(EntryObjectDTO entry)
        {
            if (!UserContext.IsPremium)
            {
                chart.SetVisible(false);
                tbLicenceMsg.Text = Strings.ProgressReport_FreeLicenceMsg;
            }
            else
            {
                chart.SetVisible(true);
                generateProgress((StrengthTrainingEntryDTO)entry);
            }
        }
    }
}
