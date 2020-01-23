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
using BodyArchitect.Common.Plugins;
using BodyArchitect.Controls.Cache;
using BodyArchitect.Logger;
using BodyArchitect.Module.A6W;
using BodyArchitect.Module.Blog;
using BodyArchitect.Module.Size;
using BodyArchitect.Module.StrengthTraining;
using BodyArchitect.Module.Suplements;
using BodyArchitect.Service.Model;
using BodyArchitect.Settings.Model;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;

namespace BodyArchitect.Controls.Forms
{
    public partial class ProfileConfigurationWizard : BaseWindow
    {

        public ProfileConfigurationWizard()
        {
            InitializeComponent();
            xtraTabControl1.ShowTabHeader = DefaultBoolean.False;
            xtraTabPage1.PageVisible = false;
            chkAutoUpdateMeasurements.Checked = false;
            chkSupplementsEntry.Checked = UserContext.Settings.GuiState.CalendarOptions.GetDefaultEntry(SuplementsModule.ModuleId);
        }


        private void simpleButton1_Click(object sender, EventArgs e)
        {
            showTabPage(xtraTabPage1);
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            showTabPage(xtraTabPage2);
        }

        private void showTabPage(XtraTabPage page)
        {
            if(InvokeRequired)
            {
                Invoke(new Action<XtraTabPage>(showTabPage),page);
            }
            else
            {
                xtraTabControl1.SelectedTabPage = page;
                xtraTabPage1.PageVisible = false;
                xtraTabPage2.PageVisible = false;
                xtraTabPage3.PageVisible = false;

                page.PageVisible = true;
            }
            
        }

        private void chkBlogger_CheckedChanged(object sender, EventArgs e)
        {
            btnNext.Enabled = true;
        }

        private void btnPrevious2_Click(object sender, EventArgs e)
        {
            showTabPage(xtraTabPage1);
        }

        private void btnNext2_Click(object sender, EventArgs e)
        {
            RunAsynchronousOperation(delegate
            {
                baPictureEdit1.Fill(UserContext.CurrentProfile);
                showTabPage(xtraTabPage3);
            }, updateProgressIndicator);
            
        }

        private void updateProgressIndicator(OperationContext context)
        {
            bool start = context.State == OperationState.Started;
            progressIndicator1.Visible = start;
            
            if(start)
            {
                progressIndicator1.Start();
            }
            else
            {
                progressIndicator1.Stop();
            }
        }

        private void usrProgressIndicatorButtons1_OkClick(object sender, ProgressIndicator.CancellationSourceEventArgs e)
        {
            var copyProfile=UserContext.CurrentProfile.Clone();
            var picture = baPictureEdit1.Save(copyProfile);

            if (picture != null && (copyProfile.Picture == null || picture.Hash != copyProfile.Picture.Hash))
            {
                var info = ServiceManager.UploadImage(picture);
                copyProfile.Picture = info;
                PicturesCache.Instance.AddToCache(picture.ToPictureCacheItem());
            }
            ProfileUpdateData data = new ProfileUpdateData();
            data.Profile = copyProfile;

            if (chkBlogger.Checked || chkPublic.Checked)
            {
                data.Profile.Privacy.CalendarView = Privacy.Public;
                data.Profile.Privacy.Sizes = Privacy.Public;
            }
            else
            {
                data.Profile.Privacy.CalendarView = Privacy.Private;
                data.Profile.Privacy.Sizes = Privacy.Private;
            }

            data.Profile.Settings.AutomaticUpdateMeasurements = chkAutoUpdateMeasurements.Checked;
            data.Wymiary = UserContext.ProfileInformation.Wymiary;
            var res = ServiceManager.UpdateProfile(data);
            UserContext.SessionData.FillProfileData(res);
            UserContext.RefreshUserData();

            if (UserContext.Settings.GuiState.CalendarOptions == null)
            {
                UserContext.Settings.GuiState.CalendarOptions = new CalendarOptions();
            }
            UserContext.Settings.GuiState.CalendarOptions.DefaultEntries.Clear();
            if (chkBlogger.Checked)
            {
                UserContext.Settings.GuiState.CalendarOptions.CalendarTextType = BlogCalendarDayContent.ID;
                UserContext.Settings.GuiState.CalendarOptions.DefaultEntries[BlogModule.ModuleId] = true;
                UserContext.Settings.GuiState.CalendarOptions.DefaultEntries[StrengthTrainingModule.ModuleId] = true;
                UserContext.Settings.GuiState.CalendarOptions.DefaultEntries[SizeModule.ModuleId] = true;
                
            }
            else
            {
                UserContext.Settings.GuiState.CalendarOptions.CalendarTextType = StrengthTrainingCalendarDayContent.ID;
                UserContext.Settings.GuiState.CalendarOptions.DefaultEntries[StrengthTrainingModule.ModuleId] = true;
                UserContext.Settings.GuiState.CalendarOptions.DefaultEntries[SizeModule.ModuleId] = true;
            }

            UserContext.Settings.GuiState.CalendarOptions.DefaultEntries[SuplementsModule.ModuleId] = chkSupplementsEntry.Checked;

            //mark current profile to not show this wizard again
            UserContext.Settings.SetProfileConfigurationWizardShowed(UserContext.CurrentProfile.Id, true);
            UserContext.Settings.Save();
            ThreadSafeClose();
        }

        private void usrProgressIndicatorButtons1_TaskProgressChanged(object sender, UserControls.TaskStateChangedEventArgs e)
        {
            bool start = e.Context.State == OperationState.Started;
            panel3.Enabled = !start;
            btnPrevious2.Enabled = !start;
        }

        private void cancelButton1_Click(object sender, EventArgs e)
        {
            //mark current profile to not show this wizard again
            UserContext.Settings.SetProfileConfigurationWizardShowed(UserContext.CurrentProfile.Id, true);
        }

        private void ProfileConfigurationWizard_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            ControlHelper.OpenUrl(ControlHelper.TutorialCreateProfile);
        }

    }
}
