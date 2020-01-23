using System;
using System.Windows.Forms;
using BodyArchitect.Common;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Logger;
using BodyArchitect.Service.Model;
using BodyArchitect.Shared;


namespace BodyArchitect.Controls.Forms
{
    public partial class AddTrainingDay : BaseWindow
    {
        public AddTrainingDay()
        {
            InitializeComponent();
            btnOK.DialogResult = System.Windows.Forms.DialogResult.None;

            fillSuperTips();

        }

        public bool FillRequired { get; private set; }

        private bool ReadOnly { get; set; }

        public TrainingDayDTO CurrentDay
        {
            get
            {
                return usrAddTrainingDay1.CurrentDay;
            }
        }

        private void fillSuperTips()
        {
            ControlHelper.AddSuperTip(btnApply, btnApply.Text, SuperTips.ButtonApply);
        }

        public void Fill(TrainingDayDTO day,UserDTO user)
        {
            ReadOnly = user != null && user.Id != UserContext.CurrentProfile.Id;
            if(!ReadOnly)
            {
                day = day.Clone();
            }
            usrAddTrainingDay1.Fill(day, user);
            
            updateReadOnlyButtons();

        }

        void updateReadOnlyButtons()
        {
            btnOK.Visible = !ReadOnly;
            btnApply.Visible = !ReadOnly;
            if (ReadOnly)
            {
                btnCancel.Text = ApplicationStrings.CloseButton;
            }
        }

        private void AsyncOperationStateChange(OperationContext context)
        {
            bool started = context.State == OperationState.Started;

            btnOK.Enabled = !started;
            btnCancel.Enabled = !started;
            btnApply.Enabled = !started;
            progressIndicator1.Visible = started;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {

            RunAsynchronousOperation(delegate(OperationContext context)
                                         {
                                             try
                                             {
                                                 if (usrAddTrainingDay1.SaveTrainingDay(true))
                                                 {
                                                     FillRequired = true;
                                                     ThreadSafeClose(System.Windows.Forms.DialogResult.OK);
                                                 }
                                             }
                                             catch (TrainingIntegrationException ex)
                                             {
                                                 TasksManager.SetException(ex);
                                                 this.SynchronizationContext.Send(delegate
                                                 {
                                                     ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorTrainingIntegrity, ErrorWindow.MessageBox);
                                                 }, null);
                                             }
                                             
                                         },AsyncOperationStateChange);
        }


        public bool DayRemoved
        {
            get
            {
                return usrAddTrainingDay1.DayRemoved;
            }
        }

        private void AddTrainingDay_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (UserContext.LoginStatus == LoginStatus.Logged && !ReadOnly && DialogResult == DialogResult.Cancel && FMMessageBox.AskYesNo(ApplicationStrings.QCloseTrainingDay) == System.Windows.Forms.DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            RunAsynchronousOperation(delegate(OperationContext context)
                                         {
                                             try
                                             {
                                                 if (usrAddTrainingDay1.SaveTrainingDay(false))
                                                 {
                                                     FillRequired = true;
                                                 }
                                             }
                                             catch (TrainingIntegrationException ex)
                                             {
                                                 TasksManager.SetException(ex);
                                                 this.SynchronizationContext.Send(delegate
                                                     {
                                                         ExceptionHandler.Default.Process(ex,ApplicationStrings.ErrorTrainingIntegrity,ErrorWindow.MessageBox);
                                                     },null);
                                             }
                                             
                                         },  AsyncOperationStateChange);
        }

        private void lblShowTutorial_Click(object sender, EventArgs e)
        {
            ControlHelper.OpenUrl(ControlHelper.TutorialWorkoutLog);
        }
    }
}
