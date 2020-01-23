using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Controls;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Service.Model;
using BodyArchitect.Service.Model.TrainingPlans;
using BodyArchitect.Shared;
using DevExpress.XtraEditors.Controls;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    public partial class usrWorkoutPlansSearch : usrBaseControl
    {
        
        private WorkoutPlanSearchCriteria criteria;

        public event EventHandler SelectedPlanChanged;
        public event EventHandler<WorkoutPlanEventArgs> DeletePlanRequest;
        public event EventHandler<WorkoutPlanEventArgs> OpenPlanRequest;


        public usrWorkoutPlansSearch()
        {
            InitializeComponent();
            fillSearchControls();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            btnSearch.Enabled = UserContext.LoginStatus == LoginStatus.Logged;
        }

        private void fillSearchControls()
        {
            cmbWorkoutPlanType.Properties.Items.Clear();
            cmbSortOrder.Properties.Items.Clear();
            cmbWorkoutPlanDifficult.Properties.Items.Clear();
            cmbLanguages.Properties.Items.Clear();
            cmbPurposes.Properties.Items.Clear();
            cmbDays.Properties.Items.Clear();

            for (int i = 1; i < 8; i++)
            {
                CheckedListBoxItem item = new CheckedListBoxItem(i, i.ToString());
                cmbDays.Properties.Items.Add(item);
            }
            foreach (var test in Enum.GetValues(typeof(WorkoutPlanPurpose)))
            {
                CheckedListBoxItem item = new CheckedListBoxItem(test, EnumLocalizer.Default.Translate((WorkoutPlanPurpose)test));
                cmbPurposes.Properties.Items.Add(item);
            }

            foreach (var type in Enum.GetValues(typeof(TrainingType)))
            {
                CheckedListBoxItem item = new CheckedListBoxItem(type);
                cmbWorkoutPlanType.Properties.Items.Add(item);
            }

            foreach (var type in Enum.GetValues(typeof(TrainingPlanDifficult)))
            {
                CheckedListBoxItem item = new CheckedListBoxItem(type, EnumLocalizer.Default.Translate((TrainingPlanDifficult)type));
                cmbWorkoutPlanDifficult.Properties.Items.Add(item);
            }

            foreach (var type in Enum.GetValues(typeof(WorkoutPlanSearchOrder)))
            {
                BodyArchitect.Controls.ComboBoxItem item = new BodyArchitect.Controls.ComboBoxItem(type, EnumLocalizer.Default.Translate((WorkoutPlanSearchOrder)type));
                cmbSortOrder.Properties.Items.Add(item);
            }
            cmbSortOrder.SelectedIndex = 0;
           

            foreach (var test in Language.Languages)
            {
                CheckedListBoxItem item = new CheckedListBoxItem(test, test.DisplayName);
                cmbLanguages.Properties.Items.Add(item);
            }
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
            if(DeletePlanRequest!=null)
            {
                DeletePlanRequest(this,new WorkoutPlanEventArgs(plan));
            }
        }

        

        protected override void LoginStatusChanged(LoginStatus newStatus)
        {
            btnSearch.Enabled = newStatus == LoginStatus.Logged;
            Clear();
            changeOperationState(false);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (UserContext.LoginStatus != LoginStatus.Logged)
            {
                throw new AuthenticationException("You must be logged");
            }
            usrWorkoutPlansPagerListView1.Clear();
            ParentWindow.TasksManager.TaskStateChanged += new EventHandler<BodyArchitect.Controls.UserControls.TaskStateChangedEventArgs>(TasksManager_TaskStateChanged);

            criteria = WorkoutPlanSearchCriteria.CreateFindAllCriteria();
            foreach (CheckedListBoxItem value in cmbDays.Properties.Items)
            {
                if (value.CheckState == CheckState.Checked)
                {
                    criteria.Days.Add((int)value.Value);
                }
            }
            foreach(CheckedListBoxItem value in cmbWorkoutPlanDifficult.Properties.Items)
            {
                if(value.CheckState==CheckState.Checked)
                {
                    criteria.Difficults.Add((TrainingPlanDifficult)value.Value);
                }
            }

            foreach (CheckedListBoxItem value in cmbPurposes.Properties.Items)
            {
                if (value.CheckState == CheckState.Checked)
                {
                    criteria.Purposes.Add((WorkoutPlanPurpose)value.Value);
                }
            }

            foreach (CheckedListBoxItem value in cmbLanguages.Properties.Items)
            {
                if (value.CheckState == CheckState.Checked)
                {
                    criteria.Languages.Add(((Language)value.Value).Shortcut);
                }
            }

            foreach (CheckedListBoxItem value in cmbWorkoutPlanType.Properties.Items)
            {
                if (value.CheckState == CheckState.Checked)
                {
                    criteria.WorkoutPlanType.Add((TrainingType)value.Value);
                }
            }
            criteria.SortOrder =(WorkoutPlanSearchOrder) cmbSortOrder.SelectedIndex;
            usrWorkoutPlansPagerListView1.Fill(criteria);

        }

        void TasksManager_TaskStateChanged(object sender, BodyArchitect.Controls.UserControls.TaskStateChangedEventArgs e)
        {
            changeOperationState(e.Context.State == OperationState.Started);
        }

 
        void changeOperationState(bool startLoginOperation)
        {
            ParentWindow.SynchronizationContext.Send(delegate
                                                         {
                                                             btnSearch.Enabled = UserContext.LoginStatus ==LoginStatus.Logged &&!startLoginOperation;
                                                         },null);

        }

        public WorkoutPlanDTO SelectedPlan
        {
            get { return usrWorkoutPlansPagerListView1.SelectedPlan; }
        }
        

        public void Clear()
        {
            usrWorkoutPlansPagerListView1.Clear();
        }

        private void usrWorkoutPlansPagerListView1_OpenPlanRequest(object sender, WorkoutPlanEventArgs e)
        {
            OnOpenPlanRequest(e.WorkoutPlan);
        }

        private void usrWorkoutPlansPagerListView1_SelectedPlanChanged(object sender, EventArgs e)
        {
            if(SelectedPlanChanged!=null)
            {
                SelectedPlanChanged(this, e);
            }
        }

        private void usrWorkoutPlansPagerListView1_DeletePlanRequest(object sender, WorkoutPlanEventArgs e)
        {
            OnDeletePlanRequest(e.WorkoutPlan);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            fillSearchControls();
        }

        
    }
}
