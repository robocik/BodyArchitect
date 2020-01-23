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
using BodyArchitect.Shared;

namespace BodyArchitect.Client.UI.Windows
{
    /// <summary>
    /// Interaction logic for ProfileEditWindow.xaml
    /// </summary>
    public partial class ProfileEditWindow
    {
        public ProfileEditWindow()
        {
            InitializeComponent();
            Loaded += ProfileEditWindow_Load;
        }

        public bool ProfileDeleted { get; private set; }

        private void okButton1_Click(object sender, CancellationSourceEventArgs e)
        {
            //if (!ValidateChildren())
            //{
            //    return;
            //}
            try
            {
                ProfileDTO savedProfile = usrProfileEdit1.SaveProfile();
                
                if (savedProfile != null)
                {
                    UserContext.Current.SessionData.FillProfileData(savedProfile);
                    UserContext.Current.RefreshUserData();
                    ThreadSafeClose(true);
                }
            }
            catch (ValidationException validException)
            {
                UIHelper.BeginInvoke(new Action(delegate
                {
                    BAMessageBox.ShowValidationError(validException.Results);
                }), Dispatcher);
            }
            catch (OldDataException validException)
            {
                UIHelper.BeginInvoke(new Action(delegate
                {
                    ExceptionHandler.Default.Process(validException, Strings.ErrorOldDataModification, ErrorWindow.MessageBox);
                }), Dispatcher);
            }
            catch (UniqueException uniqueEx)
            {
                UIHelper.BeginInvoke(() =>ExceptionHandler.Default.Process(uniqueEx, Strings.ErrorCreateProfileUnique,ErrorWindow.MessageBox), Dispatcher);

            }
            catch (Exception ex)
            {
                UIHelper.BeginInvoke(new Action(delegate
                {
                    ExceptionHandler.Default.Process(ex, Strings.ErrorCreateProfile, ErrorWindow.EMailReport);
                }), Dispatcher);
            }
        }

        public void UpdateProgressIndicator(OperationContext context)
        {
            bool startLoginOperation = context.State == OperationState.Started;
            btnDeleteProfile.IsEnabled = !startLoginOperation;
            usrProgressIndicatorButtons1.UpdateProgressIndicator(context);
            usrProfileEdit1.IsEnabled = !startLoginOperation;
        }


        private void ProfileEditWindow_Load(object sender, EventArgs e)
        {
            usrProfileEdit1.Fill(UserContext.Current.CurrentProfile);
        }

        private void btnDeleteProfile_Click(object sender, EventArgs e)
        {
            if (BAMessageBox.AskWarningYesNo(Strings.QDeleteProfile) == MessageBoxResult.Yes)
            {
                PleaseWait.Run(delegate
                    {
                        var profileOperation = new ProfileOperationParam();
                        profileOperation.ProfileId = UserContext.Current.CurrentProfile.GlobalId;
                        profileOperation.Operation = ProfileOperation.Delete;
                        ServiceManager.ProfileOperation(profileOperation);
                    ProfileDeleted = true;

                });
                UserContext.Current.Logout(skipLogoutOnServer: true);
            }

        }

        private void usrProgressIndicatorButtons1_TaskProgressChanged(object sender, TaskStateChangedEventArgs e)
        {
            UpdateProgressIndicator(e.Context);
        }
    }
}
