using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Common.Controls;
using BodyArchitect.Controls;
using BodyArchitect.Controls.UserControls;
using BodyArchitect.Module.Blog.Options;
using BodyArchitect.Service.Model;
using BodyArchitect.Shared;
using DevExpress.XtraEditors;

namespace BodyArchitect.Module.Blog.Controls
{
    public partial class usrBlog : usrBaseControl, IEntryObjectControl
    {
        private BlogEntryDTO blogEntry;
        private bool filling;

        public usrBlog()
        {
            InitializeComponent();
            
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            var parent = this.GetParentControl<usrAddTrainingDay>();
            if (parent != null)
            {
                parent.TrainingDayChanged += new EventHandler<TrainingDayChangedEventArgs>(parent_TrainingDayChanged);
            }
        }
        void parent_TrainingDayChanged(object sender, TrainingDayChangedEventArgs e)
        {
            BlogSettings.Default.SetVisitDate(e.OldDate, UserContext.CurrentProfile.Id, blogEntry.TrainingDay.ProfileId, DateTime.Now);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (UserContext.CurrentProfile != null)
            {
                BlogSettings.Default.SetVisitDate(blogEntry.TrainingDay.TrainingDate,
                                                  UserContext.CurrentProfile.Id, blogEntry.TrainingDay.ProfileId,
                                                  DateTime.Now);
            }
            base.OnHandleDestroyed(e);
        }

        public void Fill(EntryObjectDTO entry)
        {
            filling = true;
            usrHtmlEditor1.ClearContent();
            blogEntry = (BlogEntryDTO) entry;
            usrBlogComments1.Fill(blogEntry);
            usrHtmlEditor1.SetHtml(blogEntry.Comment);
            cmbAllowComments.SelectedIndex = blogEntry.AllowComments ? 0 : 1;
            updateReadOnly();
            filling = false;
        }

        public void UpdateEntryObject()
        {
            blogEntry.AllowComments = cmbAllowComments.SelectedIndex == 0;
            blogEntry.Comment = usrHtmlEditor1.GetHtml();
        }

        void updateReadOnly()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(updateReadOnly));
            }
            else
            {
                usrHtmlEditor1.ReadOnly = ReadOnly;
                grAllowComments.Visible = !ReadOnly;
                splitContainerControl1.Collapsed = blogEntry.Id == Constants.UnsavedObjectId || !blogEntry.AllowComments;
            }
            
        }

        public void AfterSave(bool isWindowClosing)
        {
            updateReadOnly();
        }

        public bool ReadOnly
        {
            get; set;
        }

        private void splitContainerControl1_SplitGroupPanelCollapsing(object sender, SplitGroupPanelCollapsingEventArgs e)
        {
            if (!filling && blogEntry != null)
            {
                e.Cancel =blogEntry.IsNew || !blogEntry.AllowComments;
            }
        }

    }
}
