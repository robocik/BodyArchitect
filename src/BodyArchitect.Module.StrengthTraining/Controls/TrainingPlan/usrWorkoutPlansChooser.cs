using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Controls;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Module.StrengthTraining.Localization;
using BodyArchitect.Service.Model;
using BodyArchitect.Service.Model.TrainingPlans;
using BodyArchitect.Shared;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using ControlHelper = BodyArchitect.Controls.ControlHelper;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    public partial class usrWorkoutPlansChooser : usrBaseControl
    {
        private StrengthTrainingEntryDTO strengthEntry;
        private bool suppressEventForTrainingPlan;

        public event EventHandler SelectedPlanDayChanged;

        public usrWorkoutPlansChooser()
        {
            InitializeComponent();

            luTrainingPlans.Properties.Columns.Add(new LookUpColumnInfo("Name", StrengthTrainingEntryStrings.WorkoutPlanLU_Name));
            luTrainingPlans.Properties.Columns.Add(new LookUpColumnInfo("TrainingType", StrengthTrainingEntryStrings.WorkoutPlanLU_Type));
            luTrainingPlans.Properties.Columns.Add(new LookUpColumnInfo("DayName", StrengthTrainingEntryStrings.WorkoutPlanLU_DayName));
            luTrainingPlans.Properties.Columns.Add(new LookUpColumnInfo("Author", StrengthTrainingEntryStrings.WorkoutPlanLU_Author));
            luTrainingPlans.Properties.ValueMember = "GlobalId";
        }

        public void Fill(StrengthTrainingEntryDTO strengthEntry)
        {
            this.strengthEntry = strengthEntry;
            setTrainingPlans(fillWorkoutPlans(false));
        }
        private bool queryingPopup = false;

        private void luTrainingPlans_QueryPopUp(object sender, CancelEventArgs e)
        {
            if (queryingPopup)
            {
                queryingPopup = false;
                return;
            }
            queryingPopup = true;
            ParentWindow.RunAsynchronousOperation(delegate
            {
                List<TrainingPlanItem> plans = fillWorkoutPlans(true);
                ParentWindow.SynchronizationContext.Send(delegate
                {
                    luTrainingPlans.Properties.DataSource = plans;
                    luTrainingPlans.Enabled = true;
                    luTrainingPlans.ShowPopup();

                }, null);
            }, updateGui);
            e.Cancel = true;
        }

        public TrainingPlanDay SelectedPlanDay
        {
            get
            {
                if (luTrainingPlans.EditValue != null)
                {
                    var plans = (List<TrainingPlanItem>)luTrainingPlans.Properties.DataSource;
                    foreach (var trainingPlanItem in plans)
                    {
                        if (trainingPlanItem.GlobalId == (Guid)luTrainingPlans.EditValue)
                        {
                            return trainingPlanItem.TrainingPlanDay;
                        }

                    }
                }
                return null;
            }
        }

        public WorkoutPlanDTO SelectedPlan
        {
            get
            {
                if (luTrainingPlans.EditValue != null)
                {
                    var plans = (List<TrainingPlanItem>)luTrainingPlans.Properties.DataSource;
                    foreach (var trainingPlanItem in plans)
                    {
                        if (trainingPlanItem.GlobalId == (Guid)luTrainingPlans.EditValue)
                        {
                            Guid id = trainingPlanItem.TrainingPlanDay != null
                                          ? trainingPlanItem.TrainingPlanDay.TrainingPlan.GlobalId
                                          : trainingPlanItem.GlobalId;
                            return ObjectsReposidory.GetWorkoutPlan(id);
                        }

                    }
                }
                return null;
            }
        }

        private void setTrainingPlans(IList<TrainingPlanItem> plans)
        {
            luTrainingPlans.Properties.DataSource = plans;
            if (strengthEntry.TrainingPlanItemId.HasValue)
            {
                suppressEventForTrainingPlan = true;
                if (plans.Where(x => x.GlobalId == strengthEntry.TrainingPlanItemId.Value).SingleOrDefault() != null)
                {
                    luTrainingPlans.EditValue = strengthEntry.TrainingPlanItemId.Value;
                }
                else
                {
                    luTrainingPlans.EditValue = strengthEntry.TrainingPlanId.Value;
                }

                suppressEventForTrainingPlan = false;
            }

            //if (strengthEntry.Id > Constants.UnsavedObjectId)
            //{
            //    luTrainingPlans.Enabled = false;
            //}

            //btnShowTrainingPlan.Visible = luTrainingPlans.EditValue != null && (SelectedPlanDay != null || SelectedPlan != null);
            updateGui(false);
        }

        List<TrainingPlanItem> fillWorkoutPlans(bool forceFill)
        {
            List<TrainingPlanItem> plans = new List<TrainingPlanItem>();
            var trainingPlans = ObjectsReposidory.WorkoutPlans;
            foreach (var trainingPlanDto in trainingPlans)
            {
                if (trainingPlanDto.IsContentLoaded || forceFill)
                {
                    var trainingPlan = trainingPlanDto.ToTrainingPlan();

                    foreach (var planDay in trainingPlan.Days)
                    {
                        plans.Add(new TrainingPlanItem(trainingPlan.Name, trainingPlanDto.Profile.UserName, trainingPlan.TrainingType, planDay.Name,
                                                       planDay.GlobalId, planDay));
                    }
                }
                else
                {
                    plans.Add(new TrainingPlanItem(trainingPlanDto.Name, trainingPlanDto.Profile.UserName, trainingPlanDto.TrainingType, StrengthTrainingEntryStrings.usrStrengthTraining_fillWorkoutPlans_NotLoaded,
                                                       trainingPlanDto.GlobalId, null));
                }
            }
            return plans;
        }

        private void btnShowTrainingPlan_Click(object sender, EventArgs e)
        {
            if (SelectedPlan == null)
            {
                return;
            }
            ControlHelper.RunWithExceptionHandling(delegate
                                                       {
                                                           SelectedPlan.Open();
                                                           setTrainingPlans(fillWorkoutPlans(false));
                                                       });
            
        }

        private void luTrainingPlans_EditValueChanged(object sender, EventArgs e)
        {
            if (suppressEventForTrainingPlan)
            {
                return;
            }
            if (SelectedPlanDayChanged != null)
            {
                SelectedPlanDayChanged(this, EventArgs.Empty);
            }
            //Guid trainingPlanId = (Guid)luTrainingPlans.EditValue;
            //List<TrainingPlanItem> plans = (List<TrainingPlanItem>)luTrainingPlans.Properties.DataSource;
            //TrainingPlanDay planDay = null;
            //foreach (TrainingPlanItem trainingPlanItem in plans)
            //{
            //    if (trainingPlanId == trainingPlanItem.GlobalId)
            //    {
            //        planDay = trainingPlanItem.TrainingPlanDay;
                    
            //        break;
            //    }
            //}
            //TrainingBuilder builder = new TrainingBuilder();
            //builder.FillRepetitionNumber = Options.StrengthTraining.Default.FillRepetitionNumberFromPlan;
            //builder.FillTrainingFromPlan(planDay, strengthEntry);
            //usrTrainingDaySourceGrid1.Fill(strengthEntry, SelectedPlanDay, ReadOnly);
            //btnShowTrainingPlan.Visible = luTrainingPlans.EditValue != null;
            //luTrainingPlans.Enabled = strengthEntry.Id == Constants.UnsavedObjectId;
            updateGui(false);
        }

        void updateGui(OperationContext context)
        {
            bool start = context.State == OperationState.Started;
            UseWaitCursor = start;
            updateGui(start);
        }

        private void updateGui(bool start)
        {
            if(InvokeRequired)
            {
                BeginInvoke(new Action<bool>(updateGui), start);
            }
            else
            {
                luTrainingPlans.Enabled = strengthEntry.Id == Constants.UnsavedObjectId && !start;
                btnShowTrainingPlan.Visible = luTrainingPlans.EditValue != null && !start && (SelectedPlanDay != null || SelectedPlan != null);
                btnRefresh.Enabled = !start;
                progressIndicator1.Visible = start;
                if (start)
                {
                    progressIndicator1.Start();
                }
                else
                {
                    progressIndicator1.Stop();

                }
            }
            
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ParentWindow.RunAsynchronousOperation(delegate
            {
                ObjectsReposidory.ClearWorkoutPlansCache();
                ObjectsReposidory.EnsurePlansLoaded();

                var plans = fillWorkoutPlans(true);
                //List<TrainingPlanItem> plans = fillWorkoutPlans(true);
                ParentWindow.SynchronizationContext.Send(delegate
                {
                    setTrainingPlans(plans);
                    // luTrainingPlans.ShowPopup();

                }, null);
            }, updateGui);
        }
    }

    class TrainingPlanItem
    {
        public TrainingPlanItem(string name,string author, TrainingType trainingType, string dayName, Guid globalId, TrainingPlanDay trainingPlan)
        {
            Name = name;
            TrainingType = trainingType;
            DayName = dayName;
            GlobalId = globalId;
            TrainingPlanDay = trainingPlan;
            Author = author;
        }

        public string Name { get; set; }
        public TrainingType TrainingType { get; set; }
        public string DayName { get; set; }
        public Guid GlobalId { get; set; }
        public TrainingPlanDay TrainingPlanDay { get; set; }
        public string Author { get; set; }

        public override string ToString()
        {
            return string.Format("{0}-{1}", Name, DayName);
        }
    }
}
