using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Controls;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Module.StrengthTraining.Localization;
using BodyArchitect.Service.Model;
using BodyArchitect.Shared;
using DevExpress.XtraEditors;


namespace BodyArchitect.Module.StrengthTraining.Controls
{
    public partial class usrWorkoutCommentsList : usrBaseControl
    {
        private CancellationTokenSource currentTask;
        private IRatingable plan;
        private int currentPage = 0;
        PagedResult<CommentEntryDTO> commentsListPack;
        private ObservableCollection<CommentEntryDTO> comments = new ObservableCollection<CommentEntryDTO>();

        public usrWorkoutCommentsList()
        {
            InitializeComponent();
            lblStatus.Text = "";

            changeOperationState(false);
            AllowRedirectToDetails = true;
            workoutPlanCommentListControl1.Fill(comments);
        }

        [DefaultValue(true)]
        public bool AllowRedirectToDetails { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool CannotVote
        {
            get { return usrRating1.CannotVote; }
            set { usrRating1.CannotVote = value; }
        }

        [Localizable(true)]
        public string CannotVoteMessage
        {
            get { return usrRating1.CannotVoteMessage; }
            set { usrRating1.CannotVoteMessage = value; }
        }

        void asyncOperationStateChange(OperationContext context)
        {
            bool startLoginOperation = context.State == OperationState.Started;

            changeOperationState(startLoginOperation);
        }

        protected override void LoginStatusChanged(LoginStatus newStatus)
        {
            Fill(null);
        }

        void changeOperationState(bool startLoginOperation)
        {
            if (startLoginOperation)
            {
                progressIndicator1.Start();
            }
            else
            {
                progressIndicator1.Stop();
            }
            btnGetMore.Enabled = plan!=null && !startLoginOperation && (commentsListPack!=null && workoutPlanCommentListControl1.Items.Count < commentsListPack.AllItemsCount);
            progressIndicator1.Visible = plan!=null && startLoginOperation;
        }
        
        public void Fill(IRatingable plan)
        {
            this.plan = plan;
            usrRating1.Fill(plan,CannotVote,CannotVoteMessage);
            fillCommentsList();
        }

        private void fillCommentsList()
        {
            comments.Clear();
            currentPage = 0;
            commentsListPack = null;
            
            lblStatus.Text = "";
            if (currentTask!=null)
            {
                currentTask.Cancel();
                changeOperationState(false);
                currentTask = null;
            }

            tableLayoutPanel1.Visible = plan != null;
            splitContainerControl1.PanelVisibility = plan != null? SplitPanelVisibility.Both: SplitPanelVisibility.Panel1;

            if (plan == null)
            {
                lblEmptyListMessage.Text = StrengthTrainingEntryStrings.usrWorkoutCommentsList_SelectWorkoutPlanFirst_Msg;
                return;
            }
            btnGetMore.Enabled = true;
            lblEmptyListMessage.Text = StrengthTrainingEntryStrings.usrWorkoutCommentsList_NoComments_Msg;
            fillCommentsPage(0);
        }

        
        private void fillCommentsPage(int pageIndex)
        {
            currentTask=ParentWindow.TasksManager.RunAsynchronousOperation(delegate(OperationContext ctx)
                          {
                              
                              PartialRetrievingInfo info = new PartialRetrievingInfo();
                              info.PageIndex = pageIndex;
                              commentsListPack = ServiceManager.GetComments(plan, info);
                              if (ctx.CancellatioToken.IsCancellationRequested)
                              {
                                  return;
                              }
                              ParentWindow.SynchronizationContext.Send(delegate
                                      {
                                          foreach (var item in commentsListPack.Items)
                                          {
                                              comments.Add(item);
                                          }
                                          lblStatus.Text = string.Format(ApplicationStrings.PartialLoadedStatus, workoutPlanCommentListControl1.Items.Count, commentsListPack.AllItemsCount);
                                          tableLayoutPanel1.Visible = workoutPlanCommentListControl1.Items.Count > 0;
                                          
                                      },null);


                          },  asyncOperationStateChange,null,false);
        }
        
        private void btnGetMore_Click(object sender, EventArgs e)
        {
            if (plan != null)
            {
                currentPage++;
                fillCommentsPage(currentPage);
            }
        }

        private void usrRating1_Voted(object sender, BodyArchitect.Controls.Rating.VoteEventArgs e)
        {
            fillCommentsList();
        }
    }
}
