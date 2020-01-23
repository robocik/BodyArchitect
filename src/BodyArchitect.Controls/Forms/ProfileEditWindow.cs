using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Common;
using BodyArchitect.Controls.ProgressIndicator;
using BodyArchitect.Shared;
using DevExpress.XtraEditors;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Logger;


namespace BodyArchitect.Controls.Forms
{
    public partial class ProfileEditWindow : BaseWindow
    {
        public ProfileEditWindow()
        {
            InitializeComponent();
        }

        public bool ProfileDeleted { get; private set; }

        private void okButton1_Click(object sender, CancellationSourceEventArgs e)
        {
            if (!ValidateChildren())
            {
                return;
            }
            try
            {
                var savedProfile = usrProfileEdit1.SaveProfile();
                if (savedProfile != null)
                {
                    UserContext.SessionData.FillProfileData(savedProfile);
                    UserContext.RefreshUserData();
                    ThreadSafeClose(System.Windows.Forms.DialogResult.OK);
                }
            }
            catch (ValidationException validException)
            {
                this.SynchronizationContext.Send(delegate
                {
                    FMMessageBox.ShowValidationError(validException.Results);
                }, null);
            }
            catch (OldDataException validException)
            {
                this.SynchronizationContext.Send(delegate
                {
                    ExceptionHandler.Default.Process(validException, ApplicationStrings.ErrorOldDataModification, ErrorWindow.MessageBox);
                }, null);
            }
            catch (UniqueException uniqueEx)
            {
                this.SynchronizationContext.Send(delegate
                {
                    ExceptionHandler.Default.Process(uniqueEx, ApplicationStrings.ErrorCreateProfileUnique, ErrorWindow.MessageBox);
                }, null);
            }
            catch (Exception ex)
            {
                this.SynchronizationContext.Send(delegate
                {
                    ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorCreateProfile, ErrorWindow.EMailReport);
                }, null);

            }
        }

        public void UpdateProgressIndicator(OperationContext context)
        {
            usrProgressIndicatorButtons1.UpdateProgressIndicator(context);
        }


        private void ProfileEditWindow_Load(object sender, EventArgs e)
        {
            usrProfileEdit1.Fill(UserContext.CurrentProfile);
        }

        private void btnDeleteProfile_Click(object sender, EventArgs e)
        {
            if (FMMessageBox.AskWarningYesNo(ApplicationStrings.QDeleteProfile) == DialogResult.Yes)
            {
                PleaseWait.Run(delegate
                     {
                         ServiceManager.DeleteProfile(UserContext.CurrentProfile);
                         ProfileDeleted = true;
                         
                     });
                UserContext.Logout(skipLogoutOnServer:true);
                ThreadSafeClose();
            }
            
        }

    }
}
