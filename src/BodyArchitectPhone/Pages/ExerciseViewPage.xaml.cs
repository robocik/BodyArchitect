using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.Controls.Animations;
using BodyArchitect.WP7.Pages;
using BodyArchitect.WP7.UserControls;
using BodyArchitect.WP7.ViewModel;
using BodyArchitectCustom;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using TombstoneHelper;

namespace BodyArchitect.WP7
{
    public partial class ExerciseViewPage : AnimatedBasePage
    {
        private ExerciseViewModel viewModel;

        public ExerciseViewPage()
        {
            InitializeComponent();
            AnimationContext = LayoutRoot;
            DataContext = this;
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
            ApplicationBar.MenuItems.Clear();

            if (pivot.SelectedIndex == 0)
            {
                ApplicationBarIconButton button1 = new ApplicationBarIconButton(new Uri("/icons/appbar.add.rest.png", UriKind.Relative));
                button1.Click += new EventHandler(btnAddExercise_Click);
                button1.Text = ApplicationStrings.AppBarButton_Add;
                ApplicationBar.Buttons.Add(button1);
            }
            else
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

            if (SelectedExercise.ExerciseType != ExerciseType.Cardio)
            {
                var menu = new ApplicationBarMenuItem(ApplicationStrings.MeasurementsPage_ShowProgress);
                menu.Click += new EventHandler(mnuShowProgress_Click);
                ApplicationBar.MenuItems.Add(menu);
            }
            updateApplicationBar();
        }

        private void mnuShowProgress_Click(object sender, EventArgs e)
        {
            if (!UpgradeAccountControl.EnsureAccountType(ApplicationStrings.Feature_Premium_Reports, this))
            {
                return;
            }
            this.Navigate("/Pages/ExercisesWeightReportPage.xaml");
        }

        void updateApplicationBar()
        {
            if (pivot.SelectedIndex == 1)
            {
                ApplicationBarIconButton button1 = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
                button1.IsEnabled = votesControl.HasMore;
                button1 = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
                button1.IsEnabled = !viewModel.Exercise.IsMine;
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

        public ExerciseDTO SelectedExercise { get; private set; }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string strType;
            if (NavigationContext.QueryString.TryGetValue("ExerciseId", out strType))
            {
                var exerciseId = new Guid(strType);
                SelectedExercise = ApplicationState.Current.Cache.Exercises.GetItem(exerciseId);
            }
            votesControl.Restore(State);
            
            //this.RestoreState();  // <- second line
            
            base.OnNavigatedTo(e);
            
            viewModel=new ExerciseViewModel(SelectedExercise);

            DataContext = viewModel;
            buildApplicationBar();

            loadComments();

            StateHelper helper = new StateHelper(State);
            pivot.SelectedIndex = helper.GetValue("PivotItem", 0);
        }


        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.Content is IExerciseListInvoker && selectedExercise!=null)
                (e.Content as IExerciseListInvoker).SelectedExercise = selectedExercise;
            
            
            if (e.Content is VotingPage )
                (e.Content as VotingPage).Item = viewModel.Exercise;

            if (e.Content is ExercisesWeightReportPage)
            {
                (e.Content as ExercisesWeightReportPage).Exercises.Add(viewModel.Exercise);
            }
            
            //this.SaveState();
            State["PivotItem"]=pivot.SelectedIndex;
            votesControl.Save(State);
        }

        private ExerciseDTO selectedExercise;

        private void btnAddExercise_Click(object sender, EventArgs e)
        {
            selectedExercise = SelectedExercise;
            //NavigationService.Navigate(new Uri("/Pages/StrengthWorkoutPage.xaml",UriKind.Relative));
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void pivot_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            buildApplicationBar();
            loadComments();
        }

        private void loadComments()
        {
            if (pivot.SelectedIndex == 1 && (!votesControl.Loaded || votesControl.RefreshRequired))
            {
                progressBar.ShowProgress(true, ApplicationStrings.Votes_ProgressRetrieveComments);
                votesControl.Load(viewModel.Exercise);
            }
        }

        private void votesControl_CommentsLoaded(object sender, EventArgs e)
        {
            progressBar.ShowProgress(false);
            updateApplicationBar();
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.URL = (string)lnkUrl.Tag;
            task.Show();
        }

    }
}