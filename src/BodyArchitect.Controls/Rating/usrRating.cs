using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Service.Model;
using BodyArchitect.Shared;
using DevExpress.XtraEditors;

namespace BodyArchitect.Controls.Rating
{
    public partial class usrRating : usrBaseControl
    {
        private IRatingable ratingable;
        private bool cannotVote;
        public event EventHandler<VoteEventArgs> Voted;


        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool CannotVote
        {
            get { return cannotVote; }
            set
            {
                if (cannotVote != value)
                {
                    cannotVote = value;
                    tableLayoutPanel2.Visible = !value;
                    lblCannotVoteMessage.Visible = value;
                }
            }
        }

        [Localizable(true)]
        public string CannotVoteMessage
        {
            get { return lblCannotVoteMessage.Text; }
            set { lblCannotVoteMessage.Text = value; }
        }

        public usrRating()
        {
            InitializeComponent();
            txtShortComment.Properties.MaxLength = Constants.ShortCommentColumnLength;
            updateGui(false);
        }

        protected virtual void OnVoted()
        {
            if(Voted!=null)
            {
                Voted(this, new VoteEventArgs(ratingable));
            }
        }
        public void Fill(IRatingable ratingable,bool cannotVote,string cannotVoteMessage)
        {
            this.ratingable = ratingable;
            if (ratingable != null)
            {
                rbGlobalRating.Rate = ratingable.Rating;
                rbUserRating.Rate = ratingable.UserRating.HasValue ? ratingable.UserRating.Value : 0.0f;
                txtShortComment.Text = ratingable.UserShortComment;
            }
            CannotVote = cannotVote;
            CannotVoteMessage = cannotVoteMessage;
            updateGui(false);
        }

        void asyncOperationStateChange(OperationContext context)
        {
            bool startLoginOperation = context.State == OperationState.Started;

            if (startLoginOperation)
            {
                progressIndicator1.Start();
            }
            else
            {
                progressIndicator1.Stop();
                Fill(ratingable, CannotVote, CannotVoteMessage);
            }
            updateGui(startLoginOperation);

        }

        void updateGui(bool startOperation)
        {
            this.btnRateIt.Enabled = ratingable!=null && !startOperation;
            rbUserRating.ReadOnly =ratingable==null || startOperation;
            progressIndicator1.Visible =ratingable!=null &&  startOperation;
        }

        private void btnRateIt_Click(object sender, EventArgs e)
        {
            ratingable.UserRating = rbUserRating.Rate;
            ratingable.UserShortComment=txtShortComment.Text;
            ParentWindow.TasksManager.RunAsynchronousOperation(delegate
                                                                   {
                          IRatingable tmp;
                          if (ratingable is WorkoutPlanDTO)
                          {
                              tmp = ServiceManager.VoteWorkoutPlan((WorkoutPlanDTO) ratingable);
                          }else if(ratingable is ExerciseDTO)
                          {
                              tmp = ServiceManager.VoteExercise((ExerciseDTO)ratingable);
                          }
                          else
                          {
                              throw new NotImplementedException();
                          }
                          ratingable.Rating = tmp.Rating;
                          ratingable.UserRating = tmp.UserRating;
                          ratingable.UserShortComment =tmp.UserShortComment;
                          ParentWindow.SynchronizationContext.Send(delegate
                                                                       {
                                                                           rbGlobalRating.Rate = tmp.Rating;
                                                                           OnVoted();
                                                                       },null);
                          
                     },  asyncOperationStateChange);

        }
    }

    public class VoteEventArgs:EventArgs
    {
        private IRatingable ratingable;

        public VoteEventArgs(IRatingable ratingable)
        {
            this.ratingable = ratingable;
        }

        public IRatingable Ratingable
        {
            get { return ratingable; }
        }
    }
}
