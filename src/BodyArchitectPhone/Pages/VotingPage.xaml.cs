using System;
using System.Collections.Generic;
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
using BodyArchitect.WP7.ViewModel;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using TombstoneHelper;
using ExtensionMethods = BodyArchitect.WP7.Controls.ExtensionMethods;

namespace BodyArchitect.WP7.Pages
{
    public partial class VotingPage 
    {
        private bool saved;
        public VotingPage()
        {
            InitializeComponent();
            buildApplicationBar();
        }

        public IRatingable Item { get; set; }

        void buildApplicationBar()
        {
            ApplicationBarIconButton button1 = new ApplicationBarIconButton(new Uri("/Toolkit.Content/ApplicationBar.Check.png", UriKind.Relative));
            button1.Click +=new EventHandler(btnVote_Click);
            button1.Text = ApplicationStrings.AppBarButton_RateIt;
            ApplicationBar.Buttons.Add(button1);
        }

        private void btnVote_Click(object sender, EventArgs e)
        {
            progressBar.ShowProgress(true,ApplicationStrings.VotingPage_ProgressSending);
            ExtensionMethods.BindFocusedTextBox();

            VoteObject objType = VoteObject.WorkoutPlan;
            if (Item is ExerciseLightDTO)
            {
                objType = VoteObject.Exercise;
            }
            else if (Item is SuplementDTO)
            {
                objType = VoteObject.Supplement;
            }
            else if (Item is SupplementCycleDefinitionDTO)
            {
                objType = VoteObject.SupplementCycleDefinition;
            }

            voteOnWorkoutPlan(objType);
        }

        private void voteOnWorkoutPlan(VoteObject objType)
        {
            var m =
                new ServiceManager<VoteCompletedEventArgs>(
                    delegate(BodyArchitectAccessServiceClient client1,
                             EventHandler<VoteCompletedEventArgs> operationCompleted)
                    {
                        VoteParams param = new VoteParams();
                        param.GlobalId = Item.GlobalId;
                        param.UserRating = Item.UserRating;
                        param.UserShortComment = Item.UserShortComment;
                        param.ObjectType = objType;

                        client1.VoteCompleted -= operationCompleted;
                        client1.VoteCompleted += operationCompleted;
                        //client1.VoteAsync(ApplicationState.Current.SessionData.Token, (WorkoutPlanDTO)Item);
                        client1.VoteAsync(ApplicationState.Current.SessionData.Token, param);
                    });

            m.OperationCompleted += (s, a) =>
            {
                progressBar.ShowProgress(false);
                if (a.Error != null)
                {
                    BAMessageBox.ShowError(ApplicationStrings.VotingPage_ErrSendRating);
                }
                else
                {
                    saved = true;
                    Item.Rating = a.Result.Result.Rating;
                    if (NavigationService.CanGoBack)
                    {
                        NavigationService.GoBack();
                    }
                }
            };

            if(!m.Run())
            {
                progressBar.ShowProgress(false);
                if (ApplicationState.Current.IsOffline)
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrOfflineMode);
                }
                else
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);
                }
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.Content is ExerciseViewPage && saved)
                (e.Content as ExerciseViewPage).votesControl.UpdateMyComment();

            if (e.Content is WorkoutPlanViewPage && saved)
                (e.Content as WorkoutPlanViewPage).votesControl.UpdateMyComment();
            //this.SaveState();
            ExtensionMethods.BindFocusedTextBox();
            State["Exercise"] = Item;
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //this.RestoreState();
            StateHelper helper = new StateHelper(State);
            Item = helper.GetValue("Exercise", Item);
            DataContext = new VotingViewModel(Item);
        }
    }
}