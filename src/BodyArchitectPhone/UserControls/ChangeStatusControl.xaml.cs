using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.Pages;

namespace BodyArchitect.WP7.UserControls
{
    public partial class ChangeStatusControl
    {
        private ProgressStatus progressStatus;
        public event EventHandler OperationCompleted;

        public ChangeStatusControl()
        {
            InitializeComponent();
            
        }

        public void Fill(ProgressStatus progressStatus, string status, bool afterTombstome)
        {
            if (!afterTombstome)
            {
                if (status==null)
                {
                    status = string.Empty;
                }
                txtComment.Text = status;
            }

            this.progressStatus = progressStatus;
        }
        public string Comment
        {
            get { return txtComment.Text; }
        }

        public void Save(IDictionary<string, object> state)
        {
            state["AddBlogCommentControl_Text"] = txtComment.Text;
            state["AddBlogCommentControl_SelectionStart"] = txtComment.SelectionStart;
            state["AddBlogCommentControl_SelectionLength"] = txtComment.SelectionLength;
        }

        public void Restore(IDictionary<string, object> state)
        {
            StateHelper helper = new StateHelper(state);
            txtComment.Text = helper.GetValue<string>("AddBlogCommentControl_Text",string.Empty);
            txtComment.SelectionStart = helper.GetValue<int>("AddBlogCommentControl_SelectionStart", 0);
            txtComment.SelectionLength = helper.GetValue("AddBlogCommentControl_SelectionLength", 0);
        }

        public void Clear(IDictionary<string, object> state)
        {
            state.Remove("AddBlogCommentControl_Text");
            state.Remove("AddBlogCommentControl_SelectionStart");
            state.Remove("AddBlogCommentControl_SelectionLength");
        }

        public void ChangeStatus()
        {
            //progressStatus.ShowProgress(true, ApplicationStrings.ChangeStatusControl_ChangingStatusProgress);
            var m = new ServiceManager<AsyncCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<AsyncCompletedEventArgs> operationCompleted)
            {
                var param = new ProfileOperationParam();
                param.Operation = ProfileOperation.SetStatus;
                param.ProfileId = ApplicationState.Current.SessionData.Profile.GlobalId;
                param.Status = new ProfileStatusDTO() { Status = Comment };
                client1.ProfileOperationAsync(ApplicationState.Current.SessionData.Token, param);
                client1.ProfileOperationCompleted -= operationCompleted;
                client1.ProfileOperationCompleted += operationCompleted;

            });

            m.OperationCompleted += (s1, a1) =>
            {
                if (a1.Error != null)
                {
                    //progressStatus.ShowProgress(false);
                    BAMessageBox.ShowError(ApplicationStrings.ChangeStatusControl_ErrChangeStatus);
                    return;
                }
                else
                {
                    //progressStatus.ShowProgress(false);
                    if (OperationCompleted != null)
                    {
                        OperationCompleted(this, EventArgs.Empty);
                    }
                }

            };

            if (!m.Run())
            {
                progressStatus.ShowProgress(false);
                if (ApplicationState.Current.IsOffline)
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrOfflineMode);
                }
                else
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);
                }
                return;
            }
        }
    }
}
