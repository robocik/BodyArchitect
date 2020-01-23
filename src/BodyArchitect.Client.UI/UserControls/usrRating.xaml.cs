using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using BodyArchitect.Shared;

namespace BodyArchitect.Client.UI.UserControls
{
    /// <summary>
    /// Interaction logic for usrRating.xaml
    /// </summary>
    public partial class usrRating
    {
        private IRatingable ratingable;
        private bool cannotVote;
        //public event EventHandler<VoteEventArgs> Voted;

        public static readonly RoutedEvent VotedEvent = 
            EventManager.RegisterRoutedEvent( "Voted", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(usrRating));
 
        // .NET wrapper
        public event RoutedEventHandler Voted
        {
            add { AddHandler(VotedEvent, value); }
            remove { RemoveHandler(VotedEvent, value); }
        }
 
// Raise the routed event "selected"
//RaiseEvent(new RoutedEventArgs(MyCustomControl.SelectedEvent));

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
                    tableLayoutPanel2.SetVisible(!value);
                    lblCannotVoteMessage.SetVisible( value);
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
            txtShortComment.MaxLength = Constants.ShortCommentColumnLength;
            updateGui(false);
        }

        //protected virtual void OnVoted()
        //{
        //    if (Voted != null)
        //    {
        //        Voted(this, new VoteEventArgs(ratingable));
        //    }
        //}
        public void Fill(IRatingable ratingable, bool cannotVote, string cannotVoteMessage)
        {
            this.ratingable = ratingable;
            if (ratingable != null)
            {
                rbGlobalRating.RatingValue = ratingable.Rating;
                rbUserRating.RatingValue = ratingable.UserRating.HasValue ? ratingable.UserRating.Value : 0.0f;
                txtShortComment.Text = ratingable.UserShortComment;
            }
            CannotVote = cannotVote;
            CannotVoteMessage = cannotVoteMessage;
            updateGui(false);
        }

        void asyncOperationStateChange(OperationContext context)
        {
            bool startLoginOperation = context.State == OperationState.Started;
            progressIndicator1.IsRunning = startLoginOperation;
            if (!startLoginOperation)
            {
                Fill(ratingable, CannotVote, CannotVoteMessage);
            }
            updateGui(startLoginOperation);

        }

        void updateGui(bool startOperation)
        {
            this.btnRateIt.IsEnabled = ratingable != null && !startOperation;
            txtShortComment.IsEnabled = !startOperation;
            rbUserRating.IsHitTestVisible = ratingable != null && !startOperation;
            rbGlobalRating.IsHitTestVisible = false;
            progressIndicator1.SetVisible(ratingable != null && startOperation);
        }


        private void btnRateIt_Click(object sender, RoutedEventArgs e)
        {
            VoteParams param = new VoteParams();
            param.UserRating=ratingable.UserRating = (float?)rbUserRating.RatingValue;
            param.UserShortComment=ratingable.UserShortComment = txtShortComment.Text;
            param.GlobalId = ratingable.GlobalId;
            ParentWindow.RunAsynchronousOperation(delegate
            {

                if (ratingable is TrainingPlan)
                {
                    param.ObjectType = VoteObject.WorkoutPlan;
                }
                else if (ratingable is ExerciseDTO)
                {
                    param.ObjectType = VoteObject.Exercise;
                }
                else if (ratingable is SuplementDTO)
                {
                    param.ObjectType = VoteObject.Supplement;
                }
                else if (ratingable is SupplementCycleDefinitionDTO)
                {
                    param.ObjectType = VoteObject.SupplementCycleDefinition;
                }
                else
                {
#if DEBUG
                    throw new NotImplementedException();
#else
                    return;
#endif

                }
                ratingable.Rating = ServiceManager.Vote(param).Rating;
                UIHelper.BeginInvoke(delegate
                {
                    rbGlobalRating.RatingValue = ratingable.Rating;
                    RaiseEvent(new VoteEventArgs(VotedEvent, this, ratingable));
                },Dispatcher);

            }, asyncOperationStateChange);
        }
    }

    public class VoteEventArgs : RoutedEventArgs
    {
        private IRatingable ratingable;

        public VoteEventArgs(RoutedEvent routedEvent, object source, IRatingable ratingable)
            : base(routedEvent, source)
        {
            this.ratingable = ratingable;
        }

        public IRatingable Ratingable
        {
            get { return ratingable; }
        }
    }
}
