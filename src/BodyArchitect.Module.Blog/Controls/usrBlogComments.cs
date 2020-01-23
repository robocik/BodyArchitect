using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Controls;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Service.Model;
using BodyArchitect.Settings;
using BodyArchitect.Shared;
using DevExpress.XtraEditors;
using Button = System.Windows.Controls.Button;

namespace BodyArchitect.Module.Blog.Controls
{
    public partial class usrBlogComments : usrBaseControl
    {
        private BlogEntryDTO blogEntry;

        public usrBlogComments()
        {
            InitializeComponent();
        }

        public void Fill(BlogEntryDTO blogEntry)
        {
            this.blogEntry = blogEntry;
            if (blogEntry == null || blogEntry.Id == Constants.UnsavedObjectId)
            {
                return;
            }
            blogCommentsList1.Fill(blogEntry);
        }

        void asyncOperationStateChange(OperationContext context)
        {
            bool startLoginOperation = context.State == OperationState.Started;
            btnSend.Enabled = !startLoginOperation && !string.IsNullOrWhiteSpace(txtComment.Text); 
            progressIndicator1.Visible = startLoginOperation;
            if (startLoginOperation)
            {
                progressIndicator1.Start();
            }
            else
            {
                progressIndicator1.Stop();
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            BlogCommentOperation arg = new BlogCommentOperation();
            arg.BlogEntryId = blogEntry.Id;
            arg.BlogComment=new BlogCommentDTO();
            arg.BlogComment.Comment = txtComment.Text;
            arg.BlogComment.CommentType = ContentType.Text;
            arg.BlogComment.Profile = UserContext.CurrentProfile;
            
            ParentWindow.TasksManager.RunAsynchronousOperation(delegate
            {
                ServiceManager.BlogCommentOperation(arg);
                ParentWindow.SynchronizationContext.Send(delegate
                                                             {
                                                                 Fill(blogEntry);
                                                                 txtComment.Text = string.Empty;
                                                             },null);

            }, asyncOperationStateChange);
        }

        private void txtComment_EditValueChanged(object sender, EventArgs e)
        {
            btnSend.Enabled = !string.IsNullOrWhiteSpace(txtComment.Text);
        }
    }
}
