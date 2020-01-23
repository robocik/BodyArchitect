using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Portable.Exceptions;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Settings;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Client.UI.Windows
{
    /// <summary>
    /// Interaction logic for CreateProfileWindow.xaml
    /// </summary>
    public partial class CreateProfileWindow
    {
        public CreateProfileWindow()
        {
            InitializeComponent();
            this.usrProgressIndicatorButtons1.OkButton.IsEnabled = false;
            var newProfile = new ProfileDTO() {Birthday = DateTime.Now};
            newProfile.Settings = new ProfileSettingsDTO();
            usrCreateProfile1.Fill(newProfile);
        }

        public SessionData CreatedSessionData { get; private set; }

        private void btnShowTermsOfService_Click(object sender, RoutedEventArgs e)
        {
            Helper.OpenUrl(ApplicationSettings.TermsOfServiceAddress);
        }

        private void btnShowPrivacyPolicy_Click(object sender, RoutedEventArgs e)
        {
            Helper.OpenUrl(ApplicationSettings.PrivacyPolicyAddress);
        }

        protected override void LoginStatusChanged(LoginStatus newStatus)
        {

        }

        private void usrProgressIndicatorButtons1_TaskProgressChanged(object sender, TaskStateChangedEventArgs e)
        {
            bool startOperation = e.Context.State == OperationState.Started;
            grUserInfo.IsEnabled = !startOperation;
        }

        private void usrProgressIndicatorButtons_OkClick(object sender, Controls.CancellationSourceEventArgs e)
        {
            try
            {
                
                UserContext.Current.Logout(LoginStatus.InProgress);
                ProfileDTO newProfile = usrCreateProfile1.Profile;
                bool res = false;
            this.SynchronizationContext.Send(delegate
                                                 {
                                                     res = usrCreateProfile1.SaveProfile(newProfile);

                                                     var validator = new ObjectValidator(typeof (ProfileDTO));
                                                     var result = validator.Validate(newProfile);

                                                     if (!result.IsValid)
                                                     {
                                                         BAMessageBox.ShowValidationError(result.ToBAResults());
                                                         res=false;
                                                     }
                                                 },null);
                if (!res)
                {
                    return;
                }

                newProfile.Privacy.CalendarView = Privacy.Public;
                newProfile.Privacy.Sizes = Privacy.Public;
                var sessionData = ServiceManager.CreateProfile(newProfile);
                if (sessionData != null)
                {
                    UserContext.CreateUserContext(sessionData);
                    CreatedSessionData = sessionData;


                }
                ThreadSafeClose(true);

            }
            catch (EMailSendException ex)
            {
                TasksManager.SetException(ex);
                this.SynchronizationContext.Send(delegate
                {
                    ExceptionHandler.Default.Process(ex, Strings.ErrProblemWithSendingEmail, ErrorWindow.MessageBox);
                }, null);
            }
            catch (ValidationException validException)
            {
                TasksManager.SetException(validException);
                this.SynchronizationContext.Send(delegate
                {
                    BAMessageBox.ShowValidationError(validException.Results);
                }, null);
            }
            catch (UniqueException uniqueEx)
            {
                TasksManager.SetException(uniqueEx);
                this.SynchronizationContext.Send(delegate
                {
                    ExceptionHandler.Default.Process(uniqueEx, Strings.ErrorCreateProfileUnique, ErrorWindow.MessageBox);
                }, null);
            }
        }

        private void chkAcceptLicence_CheckedChanged(object sender, EventArgs e)
        {
            this.usrProgressIndicatorButtons1.OkButton.IsEnabled = usrCreateProfile1.ValidateData() && chkAcceptLicence.IsChecked.Value;
        }

        private void usrCreateProfile1_ControlValidated(object sender, ControlValidatedEventArgs e)
        {
            this.usrProgressIndicatorButtons1.OkButton.IsEnabled = e.IsValid && chkAcceptLicence.IsChecked.Value;
        }
    }
}
