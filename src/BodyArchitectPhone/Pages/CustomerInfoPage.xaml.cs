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
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.Controls.Animations;
using BodyArchitect.WP7.ViewModel;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace BodyArchitect.WP7.Pages
{
    public partial class CustomerInfoPage
    {
        private CustomerViewModel viewModel;

        public CustomerInfoPage()
        {
            InitializeComponent();
            AnimationContext = LayoutRoot;
        }

        public CustomerDTO SelectedCustomer
        {
            get { return ApplicationState.Current.CurrentViewCustomer; }
            set
            {
                ApplicationState.Current.CurrentViewCustomer = value;
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            ApplicationState.Current.CurrentViewCustomer = null;
            ApplicationState.Current.CurrentBrowsingTrainingDays = null;
            base.OnBackKeyPress(e);
        }

        protected override AnimatorHelperBase GetAnimation(AnimationType animationType, Uri toOrFrom)
        {
            if (animationType == AnimationType.NavigateForwardIn || animationType == AnimationType.NavigateBackwardIn)
                return new SlideUpAnimator() { RootElement = LayoutRoot };
            else
                return new SlideDownAnimator() { RootElement = LayoutRoot };
        }

        private void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnShowCalendar_Click(object sender, RoutedEventArgs e)
        {
            this.Navigate("/Pages/CalendarPage.xaml");
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            State["PivotSelectedTab"] = pivot.SelectedIndex;
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            //updateOfflineModeGui();
            viewModel = new CustomerViewModel(SelectedCustomer);
            //viewModel.OperationCompleted += new EventHandler(viewModel_OperationCompleted);
            DataContext = viewModel;
            sizesCtrl.Fill(viewModel.Wymiary, null);
            StateHelper stateHelper = new StateHelper(this.State);
            var pivotItem = stateHelper.GetValue<int>("PivotSelectedTab", 0);
            pivot.SelectedIndex = pivotItem;

            base.OnNavigatedTo(e);
        }

        private void btnSendEmail_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.To = SelectedCustomer.Email;

            emailComposeTask.Show();
        }

        private void btnCallPhone_Click(object sender, RoutedEventArgs e)
        {
            PhoneCallTask phoneCallTask = new PhoneCallTask();

            phoneCallTask.PhoneNumber = SelectedCustomer.PhoneNumber;
            phoneCallTask.DisplayName = SelectedCustomer.FullName;

            phoneCallTask.Show();
        }
    }
}