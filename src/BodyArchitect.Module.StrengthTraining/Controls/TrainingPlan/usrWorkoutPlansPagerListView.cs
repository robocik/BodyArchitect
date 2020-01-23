using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Controls;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Service.Model;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    public partial class usrWorkoutPlansPagerListView : usrBaseControl
    {
        private PartialRetrievingInfo pagerInfo;
        private CancellationTokenSource currentTask;
        private PagedResult<WorkoutPlanDTO> result;
        public event EventHandler SelectedPlanChanged;
        public event EventHandler<WorkoutPlanEventArgs> DeletePlanRequest;
        public event EventHandler<WorkoutPlanEventArgs> OpenPlanRequest;
        private WorkoutPlanSearchCriteria criteria;

        public usrWorkoutPlansPagerListView()
        {
            InitializeComponent();
            lblStatus.Text = "";
        }

        public void Clear()
        {
            workoutPlansListView1.ClearContent();
            pagerInfo = null;
            btnMoreResults.Enabled = false;
            lblStatus.Text = "";
            result = null;
            if(currentTask!=null && !currentTask.IsCancellationRequested)
            {
                currentTask.Cancel();
                currentTask = null;
            }
        }

        private void btnMoreResults_Click(object sender, EventArgs e)
        {
            fillPage(pagerInfo.PageIndex + 1);
        }

        public void Fill(WorkoutPlanSearchCriteria criteria)
        {
            if (currentTask != null)
            {
                currentTask.Cancel();
                changeOperationState(false);
                currentTask = null;
            }
            this.criteria = criteria;
            pagerInfo = new PartialRetrievingInfo();
            fillPage(0);
        }

        private void fillPage(int pageIndex)
        {
            currentTask = ParentWindow.TasksManager.RunAsynchronousOperation(delegate(OperationContext ctx)
            {
                if (ctx.CancellatioToken==null || ctx.CancellatioToken.IsCancellationRequested || pagerInfo == null)
                {
                    return;
                }
                pagerInfo.PageIndex = pageIndex;
                result = ServiceManager.GetWorkoutPlans( criteria, pagerInfo);
                if (ctx.CancellatioToken.IsCancellationRequested)
                {
                    return;
                }
                ParentWindow.SynchronizationContext.Send(delegate
                {
                    if (result==null || ctx.CancellatioToken.IsCancellationRequested)
                    {
                        return;
                    }
                    workoutPlansListView1.Fill(result.Items);
                    lblStatus.Text = string.Format(ApplicationStrings.PartialLoadedStatus, workoutPlansListView1.Items.Count, result.AllItemsCount);
                }, null);


            }, asyncOperationStateChange, null, false);
        }

        void asyncOperationStateChange(OperationContext context)
        {
            bool startLoginOperation = context.State == OperationState.Started;

            changeOperationState(startLoginOperation);
        }

        void changeOperationState(bool startLoginOperation)
        {
            if (startLoginOperation)
            {
                progressIndicator1.Start();
            }
            else
            {
                progressIndicator1.Stop();
            }
            //btnSearch.Enabled = UserContext.LoginStatus == LoginStatus.Logged && !startLoginOperation;
            btnMoreResults.Enabled =UserContext.LoginStatus== LoginStatus.Logged && !startLoginOperation && (result != null && workoutPlansListView1.Items.Count < result.AllItemsCount);
            progressIndicator1.Visible = startLoginOperation;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public WorkoutPlanDTO SelectedPlan
        {
            get { return workoutPlansListView1.SelectedPlan; }
        }

        protected virtual void OnOpenPlanRequest(WorkoutPlanDTO plan)
        {
            if (OpenPlanRequest != null)
            {
                OpenPlanRequest(this, new WorkoutPlanEventArgs(plan));
            }
        }

        protected virtual void OnDeletePlanRequest(WorkoutPlanDTO plan)
        {
            if (DeletePlanRequest != null)
            {
                DeletePlanRequest(this, new WorkoutPlanEventArgs(plan));
            }
        }


        private void workoutPlansListView1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (SelectedPlanChanged != null)
            {
                SelectedPlanChanged(sender, e);
            }
        }

        private void workoutPlansListView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (SelectedPlan != null)
            {
                OnOpenPlanRequest(SelectedPlan);
            }
        }

        private void workoutPlansListView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (SelectedPlan != null)
            {
                OnDeletePlanRequest(SelectedPlan);
            }
        }
    }
}
