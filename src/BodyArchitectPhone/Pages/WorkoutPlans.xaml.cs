using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.ServiceModel;
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
using BodyArchitect.WP7.Controls.Animations;
using BodyArchitect.WP7.UserControls;
using BodyArchitect.WP7.ViewModel;
using Coding4Fun.Phone.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using TombstoneHelper;

namespace BodyArchitect.WP7.Pages
{
    public partial class WorkoutPlans : AnimatedBasePage
    {
        private WorkoutPlansViewModel viewModel;
        private WorkoutPlanSearchCriteria allCriteria;
        private ObservableCollection<WorkoutPlanViewModel> allPlans=new ObservableCollection<WorkoutPlanViewModel>();
        private int allPageIndex;
        private int allItemsCount;

        public WorkoutPlans()
        {
            InitializeComponent();
            AnimationContext = LayoutRoot;
        }

        protected override AnimatorHelperBase GetAnimation(AnimationType animationType, Uri toOrFrom)
        {
            ListBox list = lstMessages;
            if (pivot.SelectedIndex == 1)
            {
                list = lstFavorites;
            }
            else if (pivot.SelectedIndex == 2)
            {
                list = lstAllPlans;
            }
            if (animationType == AnimationType.NavigateForwardIn ||
                    animationType == AnimationType.NavigateBackwardIn)
                return null;
            else
                return new TurnstileFeatherBackwardOutAnimator() { ListBox = list, RootElement = LayoutRoot };
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            
            base.OnBackKeyPress(e);
            if (ModalDialog == null)
            {
                PagesState.Current.SearchedPlans = allPlans = null;
            }
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            lblOfflineMode.Visibility = ApplicationState.Current.IsOffline ? Visibility.Visible : Visibility.Collapsed;
            StateHelper helper = new StateHelper(State);
            pivot.SelectedIndex = helper.GetValue("PivotItem",0);
            allPageIndex = helper.GetValue("AllPageIndex", 0);
            allItemsCount = helper.GetValue("AllItemsCount", 0);
            allCriteria = helper.GetValue<WorkoutPlanSearchCriteria>("AllCriteria", null);
            //allPlans = helper.GetValue("AllPlans", new ObservableCollection<WorkoutPlanViewModel>());
            allPlans = PagesState.Current.SearchedPlans ?? new ObservableCollection<WorkoutPlanViewModel>();
            lstAllPlans.ItemsSource = null;
            lstAllPlans.ItemsSource = allPlans;

            buildApplicationBar();
            if (viewModel == null || !ApplicationState.Current.Cache.TrainingPlans.IsLoaded)
            {
                lstMessages.ItemsSource = null;
                lstFavorites.ItemsSource = null;
                viewModel = new WorkoutPlansViewModel();
                DataContext = viewModel;

                viewModel.WorkoutPlansLoaded += new EventHandler(_viewModel_PlansLoaded);
                progressBar.ShowProgress(true, ApplicationStrings.WorkoutPlans_ProgressRetrievingPlans, false);
            }

            viewModel.LoadPlans();

            this.RestoreState();  // <- second line
            
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            this.SaveState();

            WorkoutPlanViewPage page = e.Content as WorkoutPlanViewPage;
            if (page != null)
            {
                page.SelectedPlan = viewModel.SelectedPlan.Plan;
            }
            
            State["PivotItem"] = pivot.SelectedIndex;
            //State["AllPlans"] = allPlans;
            PagesState.Current.SearchedPlans = allPlans;
            State["AllPageIndex"] = allPageIndex;
            State["AllItemsCount"] = allItemsCount;
            State["AllCriteria"] = allCriteria;
        }

        void _viewModel_PlansLoaded(object sender, EventArgs e)
        {
            lstMessages.ItemsSource = viewModel.MyPlans;
            lstFavorites.ItemsSource = viewModel.FavoritePlans;
            progressBar.ShowProgress(false,"",false);
            updateApplicationBarButtons();
        }

        private void myPlans_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var list = (ListBox) sender;
            if (e.AddedItems.Count > 0)
            {
                viewModel.SelectedPlan = (WorkoutPlanViewModel)e.AddedItems[0];
                this.Navigate("/Pages/WorkoutPlanViewPage.xaml");
            }
            list.SelectedIndex = -1;
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            showFilterDlg();
        }

        private void btnMore_Click(object sender, EventArgs e)
        {
            retrievePlansPage(++allPageIndex);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            progressBar.ShowProgress(true, ApplicationStrings.WorkoutPlans_ProgressRetrievingPlans, false);
            viewModel.Refresh();
        }

        void retrievePlansPage(int pageIndex)
        {
            progressBar.ShowProgress(true, ApplicationStrings.WorkoutPlans_ProgressRetrievingPlans);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageIndex = pageIndex;
            var m = new ServiceManager<GetWorkoutPlansCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<GetWorkoutPlansCompletedEventArgs> operationCompleted)
            {
                lblNoSearchResult.Visibility = System.Windows.Visibility.Collapsed;
                client1.GetWorkoutPlansAsync(ApplicationState.Current.SessionData.Token, allCriteria, pageInfo);
                client1.GetWorkoutPlansCompleted -= operationCompleted;
                client1.GetWorkoutPlansCompleted += operationCompleted;

            });
            m.OperationCompleted += (s, a) =>
            {
                if (a.Error != null)
                {
                    progressBar.ShowProgress(false);
                    FaultException<ValidationFault> faultEx = a.Error as FaultException<ValidationFault>;
                    if (faultEx != null)
                    {
                        BAMessageBox.ShowError(faultEx.Detail.Details[0].Key + ":" + faultEx.Detail.Details[0].Message);
                        return;
                    }
                    BAMessageBox.ShowError(ApplicationStrings.ErrCannotRetrievePlans);
                    return;
                }
                allItemsCount = a.Result.Result.AllItemsCount;
                foreach (var planDto in a.Result.Result.Items)
                {
                    allPlans.Add(new WorkoutPlanViewModel(planDto));
                }
                if(allItemsCount==0)
                {
                    lblNoSearchResult.Visibility = System.Windows.Visibility.Visible;
                }
                progressBar.ShowProgress(false);
                updateApplicationBarButtons();

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

        private void showFilterDlg()
        {
            
            var ctrl = new WorkoutPlansFilterControl();
            ctrl.ShowPopup(messagePrompt =>
                               {
                                   messagePrompt.IsCancelVisible = true;
                                   messagePrompt.Completed += (a1, s1) =>
                                   {
                                       var commentCtrl = (WorkoutPlansFilterControl)((MessagePrompt)a1).Body;

                                       if (s1.PopUpResult == PopUpResult.Ok)
                                       {
                                           lstAllPlans.ItemsSource=allPlans=new ObservableCollection<WorkoutPlanViewModel>();
                                           allPageIndex = 0;
                                           allCriteria = commentCtrl.GetSearchCriteria();

                                           retrievePlansPage(allPageIndex);
                                       }
                                       messagePrompt = null;
                                   };
                               },this);
            
        }

        void buildApplicationBar()
        {
            ApplicationBar.Buttons.Clear();
            if (pivot.SelectedIndex == 2)
            {
                ApplicationBarIconButton button1 = new ApplicationBarIconButton(new Uri("/icons/appbar.download.rest.png", UriKind.Relative));
                button1.Click += new EventHandler(btnMore_Click);
                button1.Text = ApplicationStrings.MoreButton;
                ApplicationBar.Buttons.Add(button1);

                button1 = new ApplicationBarIconButton(new Uri("/icons/appbar.feature.search.rest.png", UriKind.Relative));
                button1.Click += new EventHandler(btnFilter_Click);
                button1.Text = ApplicationStrings.NewButton;
                ApplicationBar.Buttons.Add(button1);
            }
            else
            {
                ApplicationBarIconButton button1 = new ApplicationBarIconButton(new Uri("/icons/appbar.refresh.rest.png", UriKind.Relative));
                button1.Click += new EventHandler(btnRefresh_Click);
                button1.Text = ApplicationStrings.AppBarButton_Refresh;
                ApplicationBar.Buttons.Add(button1);
            }
            updateApplicationBarButtons();
        }

        private void updateApplicationBarButtons()
        {
            ApplicationBar.IsVisible = !ApplicationState.Current.IsOffline;

            if (pivot.SelectedIndex == 2 && !ApplicationState.Current.IsOffline)
            {
                var btn1 = (IApplicationBarIconButton)ApplicationBar.Buttons[0];
                btn1.IsEnabled = allCriteria != null && allPlans.Count < allItemsCount;
            }

        }

        private void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            buildApplicationBar();
        }

        private void Menu_Opened(object sender, RoutedEventArgs e)
        {
            LayoutRoot.IsHitTestVisible = false;
        }

        private void Menu_Closed(object sender, RoutedEventArgs e)
        {
            LayoutRoot.IsHitTestVisible = true;
        }

        private void mnuRemoveFromFavorites_Click(object sender, RoutedEventArgs e)
        {
            var item = (WorkoutPlanViewModel)(sender as FrameworkElement).DataContext;

            progressBar.ShowProgress(true);
            var m = new ServiceManager<WorkoutPlanOperationCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<WorkoutPlanOperationCompletedEventArgs> operationCompleted)
            {
                WorkoutPlanOperationParam param = new WorkoutPlanOperationParam();
                param.Operation = SupplementsCycleDefinitionOperation.RemoveFromFavorites;
                param.WorkoutPlanId = item.Plan.GlobalId;
                client1.WorkoutPlanOperationAsync(ApplicationState.Current.SessionData.Token,param);
                client1.WorkoutPlanOperationCompleted -= operationCompleted;
                client1.WorkoutPlanOperationCompleted += operationCompleted;
            });

            m.OperationCompleted += (s1, a1) =>
            {
                progressBar.ShowProgress(false);
                if (a1.Error != null)
                {

                    BAMessageBox.ShowError(ApplicationStrings.ErrCannotRemovePlanFromFavorites);
                    return;
                }
                else
                {
                    ApplicationState.Current.Cache.TrainingPlans.Items.Remove(item.Plan.GlobalId);
                    viewModel.RemoveFromFavorites(item);
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
    }
}