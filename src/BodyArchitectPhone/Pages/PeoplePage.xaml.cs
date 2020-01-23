using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Microsoft.Phone.Tasks;

namespace BodyArchitect.WP7.Pages
{
    public partial class PeoplePage : AnimatedBasePage
    {
        private PeopleViewModel viewModel;
        private ObservableCollection<UserViewModel> allUsers = new ObservableCollection<UserViewModel>();
        private int allPageIndex;
        private int allItemsCount;
        private UserSearchCriteria allCriteria;

        public PeoplePage()
        {
            InitializeComponent();
            AnimationContext = LayoutRoot;
        }

        protected override AnimatorHelperBase GetAnimation(AnimationType animationType, Uri toOrFrom)
        {
            ListBox list = lstFriends;
            if(pivot.SelectedIndex==1)
            {
                list = lstFavorites;
            }
            else if(pivot.SelectedIndex==2)
            {
                list = lstUsers;
            }
            if (animationType == AnimationType.NavigateForwardIn ||
                    animationType == AnimationType.NavigateBackwardIn)
                return null;
            else
                return new TurnstileFeatherBackwardOutAnimator() { ListBox = list, RootElement = LayoutRoot };
        }

        void buildApplicationBar()
        {
            ApplicationBar.Buttons.Clear();
            if (pivot.SelectedIndex == 2)
            {
                ApplicationBarIconButton button1 = new ApplicationBarIconButton(new Uri("/icons/appbar.download.rest.png", UriKind.Relative));
                button1.Click += new EventHandler(btnMore_Click);
                button1.Text = ApplicationStrings.AppBarButton_More;
                ApplicationBar.Buttons.Add(button1);
                
                button1 = new ApplicationBarIconButton(new Uri("/icons/appbar.feature.search.rest.png", UriKind.Relative));
                button1.Click += new EventHandler(btnFilter_Click);
                button1.Text = ApplicationStrings.AppBarButton_Search;
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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            progressBar.ShowProgress(true, ApplicationStrings.Login_ProgressRetrieveProfile);
            var m = new ServiceManager<GetProfileInformationCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<GetProfileInformationCompletedEventArgs> operationCompleted)
            {
                GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
                client1.GetProfileInformationAsync(ApplicationState.Current.SessionData.Token, criteria);
                client1.GetProfileInformationCompleted -= operationCompleted;
                client1.GetProfileInformationCompleted += operationCompleted;

            });
            m.OperationCompleted += (s, a) =>
            {
                progressBar.ShowProgress(false);
                if (a.Result.Result != null)
                {
                    ApplicationState.Current.ProfileInfo = a.Result.Result;
                    viewModel = new PeopleViewModel();
                    DataContext = viewModel;
                }
                else
                {
                    BAMessageBox.ShowError(ApplicationStrings.MyProfileControl_CantRetrieveProfileInfo_ErrMsg);
                }
            };

            if (!m.Run())
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

        private void updateApplicationBarButtons()
        {
            ApplicationBar.IsVisible = !ApplicationState.Current.IsOffline;

            if (pivot.SelectedIndex == 2 && !ApplicationState.Current.IsOffline)
            {
                var btn1 = (IApplicationBarIconButton)ApplicationBar.Buttons[0];
                btn1.IsEnabled = allCriteria != null && allUsers.Count < allItemsCount;
            }

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            lblOfflineMode.Visibility = ApplicationState.Current.IsOffline ? Visibility.Visible : Visibility.Collapsed;
            //if (viewModel == null)
            {
                viewModel = new PeopleViewModel();
                DataContext = viewModel;
            }
            buildApplicationBar();
            StateHelper helper = new StateHelper(State);
            pivot.SelectedIndex = helper.GetValue("PivotItem", 0);
            allPageIndex = helper.GetValue("AllPageIndex", 0);
            allItemsCount = helper.GetValue("AllItemsCount", 0);
            allCriteria = helper.GetValue<UserSearchCriteria>("AllCriteria", null);
            allUsers = helper.GetValue("AllUsers", new ObservableCollection<UserViewModel>());
            lstUsers.ItemsSource = allUsers;
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            ProfileInfoPage page = e.Content as ProfileInfoPage;
            if (page != null)
            {
                page.SelectedUser = viewModel.SelectedUser.User;
            }

            base.OnNavigatedFrom(e);
            State["PivotItem"] = pivot.SelectedIndex;
            State["AllUsers"] = allUsers;
            State["AllPageIndex"] = allPageIndex;
            State["AllItemsCount"] = allItemsCount;
            State["AllCriteria"] = allCriteria;
        }


        private void lstFriends_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var list = (ListBox)sender;
            if (e.AddedItems.Count > 0)
            {
                viewModel.SelectedUser = (UserViewModel)e.AddedItems[0];
                this.Navigate("/Pages/ProfileInfoPage.xaml");
            }
            list.SelectedIndex = -1;
        }

        private void showFilterDlg()
        {
            //var messagePrompt = new MessagePrompt();
            var ctrl = new UsersFilterControl();
            ctrl.ShowPopup(messagePrompt =>
                               {
                                   messagePrompt.IsCancelVisible = true;
                                   messagePrompt.Completed += (a1, s1) =>
                                   {
                                       var commentCtrl =
                                           (UsersFilterControl) ((MessagePrompt) a1).Body;

                                       if (s1.PopUpResult == PopUpResult.Ok)
                                       {
                                           allUsers=new ObservableCollection<UserViewModel>();
                                           allPageIndex = 0;
                                           allCriteria = commentCtrl.GetSearchCriteria();

                                           retrievePlansPage(allPageIndex);
                                       }
                                       messagePrompt = null;
                                   };
                               },this);
            //messagePrompt.Body = ctrl;
            //messagePrompt.IsCancelVisible = true;
            //messagePrompt.IsAppBarVisible = true;
            //messagePrompt.Completed += (a1, s1) =>
            //{
            //    var commentCtrl = (UsersFilterControl)((MessagePrompt)a1).Body;

            //    if (s1.PopUpResult == PopUpResult.Ok)
            //    {
            //        allUsers.Clear();
            //        allPageIndex = 0;
            //        allCriteria = commentCtrl.GetSearchCriteria();

            //        retrievePlansPage(allPageIndex);
            //    }
            //    messagePrompt = null;
            //};
            //messagePrompt.Show();
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            showFilterDlg();
        }

        private void btnMore_Click(object sender, EventArgs e)
        {
            retrievePlansPage(++allPageIndex);
        }

        void retrievePlansPage(int pageIndex)
        {
            progressBar.ShowProgress(true, ApplicationStrings.PeoplePage_ProgressRetrieveUsers);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageIndex = pageIndex;
            var m = new ServiceManager<GetUsersCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<GetUsersCompletedEventArgs> operationCompleted)
            {
                client1.GetUsersCompleted -= operationCompleted;
                client1.GetUsersCompleted += operationCompleted;
                client1.GetUsersAsync(ApplicationState.Current.SessionData.Token, allCriteria, pageInfo);
                

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
                    BAMessageBox.ShowError(ApplicationStrings.PeoplePage_ErrRetrieveUsers);
                    return;
                }
                allItemsCount = a.Result.Result.AllItemsCount;
                foreach (var planDto in a.Result.Result.Items)
                {
                    allUsers.Add(new UserViewModel(planDto));
                }
                progressBar.ShowProgress(false,"",false);
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

        private void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            buildApplicationBar();
        }
    }
}