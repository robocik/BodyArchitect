using System;
using System.ServiceModel;
using System.Threading;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Common;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Controls.ProgressIndicator;
using BodyArchitect.Logger;
using BodyArchitect.Service.Model;
using BodyArchitect.Settings;
using BodyArchitect.Shared;
using SessionData = BodyArchitect.Service.Model.SessionData;


namespace BodyArchitect.Controls.Forms
{
    public partial class CreateProfileWindow : BaseWindow
    {
        public SessionData CreatedSessionData { get; private set; }

        public CreateProfileWindow()
        {
            InitializeComponent();

            this.usrProgressIndicatorButtons1.OkButton.Enabled = false;
        }


        private void usrCreateProfile1_ControlValidated(object sender, ControlValidatedEventArgs e)
        {
            this.usrProgressIndicatorButtons1.OkButton.Enabled = e.IsValid && chkAcceptLicence.Checked;
        }
        protected override void LoginStatusChanged(LoginStatus newStatus)
        {
            
        }

        private void okButton1_Click(object sender, CancellationSourceEventArgs e)
        {
            try
            {
                UserContext.Logout(LoginStatus.InProgress);
                ProfileDTO newProfile = new ProfileDTO();
                bool res = false;
                this.SynchronizationContext.Send(delegate
                                                     {
                                                         res = usrCreateProfile1.SaveProfile(newProfile);
                                                     }, null);
                if(!res)
                {
                    return;
                }
                
                newProfile.Privacy.CalendarView = Privacy.Public;
                newProfile.Privacy.Sizes = Privacy.Public;
                var sessionData = ServiceManager.CreateProfile(newProfile);
                if (sessionData!= null)
                {
                    UserContext.CreateUserContext(sessionData);
                    CreatedSessionData = sessionData;
                    
                    
                }
                ThreadSafeClose();
                
            }
            catch (EMailSendException ex)
            {
                TasksManager.SetException(ex);
                this.SynchronizationContext.Send(delegate
                {
                    ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrProblemWithSendingEmail, ErrorWindow.MessageBox);
                }, null);
            }
            catch (ValidationException validException)
            {
                TasksManager.SetException(validException);
                this.SynchronizationContext.Send(delegate
                {
                    FMMessageBox.ShowValidationError(validException.Results);
                }, null);
            }
            catch (UniqueException uniqueEx)
            {
                TasksManager.SetException(uniqueEx);
                this.SynchronizationContext.Send(delegate
                {
                    ExceptionHandler.Default.Process(uniqueEx, ApplicationStrings.ErrorCreateProfileUnique, ErrorWindow.MessageBox);
                }, null);
            }
        }


        private void usrProgressIndicatorButtons1_TaskProgressChanged(object sender, UserControls.TaskStateChangedEventArgs e)
        {
            bool startOperation = e.Context.State == OperationState.Started;
            baGroupControl1.Enabled = !startOperation;
        }

        private void lblShowTermsOfService_Click(object sender, EventArgs e)
        {
            ControlHelper.OpenUrl(ApplicationSettings.TermsOfServiceAddress);
        }

        private void lblShowPrivacyPolicy_Click(object sender, EventArgs e)
        {
            ControlHelper.OpenUrl(ApplicationSettings.PrivacyPolicyAddress);
        }

        private void chkAcceptLicence_CheckedChanged(object sender, EventArgs e)
        {
            this.usrProgressIndicatorButtons1.OkButton.Enabled = usrCreateProfile1 .ValidateData() && chkAcceptLicence.Checked;
        }

        private void CreateProfileWindow_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ControlHelper.OpenUrl(ControlHelper.TutorialCreateProfile);
        }
    }
}