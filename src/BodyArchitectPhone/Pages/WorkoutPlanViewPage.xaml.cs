using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls.Animations;
using BodyArchitect.WP7.UserControls;
using BodyArchitect.WP7.ViewModel;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using TombstoneHelper;

namespace BodyArchitect.WP7.Pages
{
    public partial class WorkoutPlanViewPage : AnimatedBasePage
    {
        private WorkoutPlanViewModel viewModel;

        public WorkoutPlanViewPage()
        {
            InitializeComponent();
            AnimationContext = LayoutRoot;
            buildApplicationBar();
        }

        protected override AnimatorHelperBase GetAnimation(AnimationType animationType, Uri toOrFrom)
        {
            if (animationType == AnimationType.NavigateForwardIn || animationType == AnimationType.NavigateBackwardIn)
                return new SlideUpAnimator() { RootElement = LayoutRoot };
            else
                return new SlideDownAnimator() { RootElement = LayoutRoot };
        }

        void buildApplicationBar()
        {
            ApplicationBar.Buttons.Clear();
            if(pivot.SelectedIndex==2)
            {
                ApplicationBarIconButton button1 = new ApplicationBarIconButton(new Uri("/icons/appbar.download.rest.png", UriKind.Relative));
                button1.Click += new EventHandler(btnMore_Click);
                button1.Text = ApplicationStrings.AppBarButton_More;
                ApplicationBar.Buttons.Add(button1);

                button1 = new ApplicationBarIconButton(new Uri("/icons/SMS.png", UriKind.Relative));
                button1.Click += new EventHandler(btnVote_Click);
                button1.Text = ApplicationStrings.AppBarButton_RateIt;
                ApplicationBar.Buttons.Add(button1);
            }
            else
            {
                ApplicationBarIconButton button1 = new ApplicationBarIconButton(new Uri("/icons/appbar.favs.addto.rest.png", UriKind.Relative));
                button1.Click += new EventHandler(btnAddToFavorites_Click);
                button1.Text = ApplicationStrings.AppBarButton_AddToFavorites;
                ApplicationBar.Buttons.Add(button1);
            }
        }

        void btnVote_Click(object sender, EventArgs e)
        {
            votesControl.Voting();
        }

        void btnMore_Click(object sender, EventArgs e)
        {
            progressBar.ShowProgress(true, ApplicationStrings.Votes_ProgressRetrieveComments);
            votesControl.LoadMore();
        }

        void updateApplicationBar()
        {
            if(ApplicationState.Current.IsOffline)
            {
                ApplicationBar.IsVisible = false;
                return;
            }
            ApplicationBar.IsVisible = true;
            //pivot.SelectedIndex!=3 && !SelectedPlan.IsMine && !SelectedPlan.IsFavorite;

            if (pivot.SelectedIndex == 2)
            {
                ApplicationBarIconButton button1 = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
                button1.IsEnabled = votesControl.HasMore;
                button1 = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
                button1.IsEnabled =  !viewModel.Plan.IsMine;
            }
            else if(pivot.SelectedIndex==3)
            {
                ApplicationBar.IsVisible = false;
            }
            else
            {
                ApplicationBarIconButton button1 = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
                button1.IsEnabled = !SelectedPlan.IsMine && !SelectedPlan.IsFavorite;
            }
        }

        
        public TrainingPlan SelectedPlan
        { 
            get { return ApplicationState.Current.CurrentTrainingPlan; }
            set
            {
                ApplicationState.Current.CurrentTrainingPlan = value;
            } }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //lblOfflineMode1.Visibility = ApplicationState.Current.IsOffline ? Visibility.Visible : Visibility.Collapsed;
            //lblOfflineMode.Visibility =  lblOfflineMode2.Visibility = ApplicationState.Current.IsOffline && !SelectedPlan.IsContentLoaded ? Visibility.Visible : Visibility.Collapsed;

            viewModel = new WorkoutPlanViewModel(SelectedPlan);
            viewModel.Loaded += (a, b) =>
                                    {
                                        progressBar.ShowProgress(false);
                                        updateApplicationBar();
                                    };
            DataContext = viewModel;

            StateHelper stateHelper = new StateHelper(this.State);
            var pivotItem = stateHelper.GetValue<int>("PivotSelectedTab", 0);
            if (pivotItem != 2)
            {//bug fixing. with 4 pivot items when we want to restore index 2 we have exception
                pivot.SelectedIndex = pivotItem;
            }
            this.RestoreState();  // <- second line
            votesControl.Restore(State);

            updateApplicationBar();
            loadComments();
            
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.Content is VotingPage)
                (e.Content as VotingPage).Item = viewModel.Plan;
            base.OnNavigatedFrom(e);
            
            this.SaveState();
            this.State["PivotSelectedTab"] = pivot.SelectedIndex;
            votesControl.Save(State);
        }


        private void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ApplicationState.Current.IsOffline)
            {
                return;
            }
            buildApplicationBar();
            //if(pivot.SelectedIndex>0)
            //{
            //    progressBar.ShowProgress(true, ApplicationStrings.WorkoutPlanViewPage_ProgressRetrievePlanContent);
            //    viewModel.Load();
            //}
            updateApplicationBar();
            loadComments();
        }

        private void loadComments()
        {
            if (pivot.SelectedIndex == 2 && (!votesControl.Loaded || votesControl.RefreshRequired))
            {
                progressBar.ShowProgress(true, ApplicationStrings.Votes_ProgressRetrieveComments);
                votesControl.Load(viewModel.Plan);
            }
        }

        private void votesControl_CommentsLoaded(object sender, EventArgs e)
        {
            progressBar.ShowProgress(false);
            updateApplicationBar();
        }

        private void btnAddToFavorites_Click(object sender, EventArgs e)
        {
           if (!UpgradeAccountControl.EnsureAccountType(ApplicationStrings.Feature_Premium_CreatingWorkoutPlans, this))
           {
               return;
           }
            progressBar.ShowProgress(true);
            var m = new ServiceManager<WorkoutPlanOperationCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<WorkoutPlanOperationCompletedEventArgs> operationCompleted)
                                                                                   {
                var param =new WorkoutPlanOperationParam();
                                                                                       param.WorkoutPlanId =SelectedPlan.GlobalId;
                                                                                       param.Operation =SupplementsCycleDefinitionOperation.AddToFavorites;
                client1.WorkoutPlanOperationAsync(ApplicationState.Current.SessionData.Token, param);
                client1.WorkoutPlanOperationCompleted -= operationCompleted;
                client1.WorkoutPlanOperationCompleted += operationCompleted;

            });

            m.OperationCompleted += (s1, a1) =>
            {
                progressBar.ShowProgress(false);
                FaultException<BAServiceException> serviceEx = a1.Error as FaultException<BAServiceException>;
                if (serviceEx != null && serviceEx.Detail.ErrorCode == ErrorCode.LicenceException)
                {
                    BAMessageBox.ShowInfo(ApplicationStrings.ErrLicence);
                    return;
                }
                if (a1.Error != null)
                {

                    BAMessageBox.ShowError(ApplicationStrings.ErrCannotAddPlanToFavorites);
                    return;
                }
                else
                {
                    ApplicationState.Current.Cache.TrainingPlans.Items.Add(SelectedPlan.GlobalId,SelectedPlan);
                }
                updateApplicationBar();

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

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.URL = (string)lnkUrl.Tag;
            task.Show();
        }

    }
}