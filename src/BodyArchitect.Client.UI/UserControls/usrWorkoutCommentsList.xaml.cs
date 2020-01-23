using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
using BodyArchitect.Client.Resources.Localization;

namespace BodyArchitect.Client.UI.UserControls
{
    /// <summary>
    /// Interaction logic for usrWorkoutCommentsList.xaml
    /// </summary>
    public partial class usrWorkoutCommentsList
    {

        public usrWorkoutCommentsList()
        {
            InitializeComponent();
            DataContext = this;
            Fill(null);
            Voted += usrRating1_Voted;
        }

        public static readonly RoutedEvent VotedEvent =
            usrRating.VotedEvent.AddOwner(typeof(usrWorkoutCommentsList));

        public event RoutedEventHandler Voted
        {
            add
            {
                AddHandler(VotedEvent, value);
            }
            remove
            {
                RemoveHandler(VotedEvent, value);
            }
        }

        [DefaultValue(true)]
        public bool AllowRedirectToDetails
        {
            get { return workoutPlanCommentListControl1.AllowRedirectToDetails; }
            set { workoutPlanCommentListControl1.AllowRedirectToDetails = value; }
        }

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

        protected override void LoginStatusChanged(LoginStatus newStatus)
        {
            base.LoginStatusChanged(newStatus);
            Fill(null);
        }

        protected override void FillResults(ObservableCollection<CommentEntryDTO> result)
        {
            workoutPlanCommentListControl1.Fill(Items);
            tableLayoutPanel1.SetVisible(workoutPlanCommentListControl1.Items.Count > 0);
            lblEmptyListMessage.SetVisible(workoutPlanCommentListControl1.Items.Count == 0);
        }

        protected override void ChangeOperationState(bool startLoginOperation)
        {
            progressIndicator1.IsRunning = plan != null && startLoginOperation;
        }

        public override void Fill(IRatingable plan)
        {
            base.Fill(plan);
            usrRating1.Fill(plan, CannotVote, CannotVoteMessage);
            fillCommentsList();
        }

        private void fillCommentsList()
        {
            ClearContent();
            
            lblEmptyListMessage.SetVisible(plan == null);
            workoutPlanCommentListControl1.SetVisible(plan != null);
            tableLayoutPanel1.SetVisible(plan != null);
            rowSplitter.SetVisible(plan != null);
            if (plan == null)
            {
                lblEmptyListMessage.Text = "usrWorkoutCommentsList_fillCommentsList_SelectItemFirst".TranslateStrings();
                return;
            }
            lblEmptyListMessage.Text = "usrWorkoutCommentsList_fillCommentsList_NoComments".TranslateStrings();
            if (plan != null)
            {
                DoSearch();
            }
        }



        private void btnGetMore_Click(object sender, EventArgs e)
        {
            MoreResults();
        }

        private void usrRating1_Voted(object sender, RoutedEventArgs e)
        {
            fillCommentsList();
        }
    }

    public abstract class WorkoutCommentsPagerList : PagerListUserControl<CommentEntryDTO>
    {
        protected IRatingable plan;

        public virtual void Fill(IRatingable plan)
        {
            this.plan = plan;
        }
        protected override void BeforeSearch(object param = null)
        {
            
        }

        protected override PagedResult<CommentEntryDTO> RetrieveItems(PartialRetrievingInfo pagerInfo)
        {
            if (plan != null)
            {
                return ServiceManager.GetComments(plan, pagerInfo);
            }
            return new PagedResult<CommentEntryDTO>(new List<CommentEntryDTO>(),0,0 );
        }
    }
}
