using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class BodyInstructorPage
    {
        private ObservableCollection<CustomerViewModel> customers = new ObservableCollection<CustomerViewModel>();

        public BodyInstructorPage()
        {
            InitializeComponent();
            AnimationContext = LayoutRoot;
        }

        public CustomerDTO SelectedCustomer { get; set; }

        protected override AnimatorHelperBase GetAnimation(AnimationType animationType, Uri toOrFrom)
        {
            ListBox list = lstCustomers;
            if (animationType == AnimationType.NavigateForwardIn ||
                    animationType == AnimationType.NavigateBackwardIn)
                return null;
            else
                return new TurnstileFeatherBackwardOutAnimator() { ListBox = list, RootElement = LayoutRoot };
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            DataContext = this;
            //buildApplicationBar();
            StateHelper helper = new StateHelper(State);
            pivot.SelectedIndex = helper.GetValue("PivotItem", 0);
            loadCustomers();
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            var page = e.Content as CustomerInfoPage;
            if (page != null)
            {
                page.SelectedCustomer = SelectedCustomer;
            }

            base.OnNavigatedFrom(e);
            State["PivotItem"] = pivot.SelectedIndex;
        }

        public ObservableCollection<CustomerViewModel> Customers
        {
            get { return customers; }
        }

        private void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void lstCustomers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var list = (ListBox)sender;
            if (e.AddedItems.Count > 0)
            {
                SelectedCustomer = ((CustomerViewModel) e.AddedItems[0]).Customer;
                this.Navigate("/Pages/CustomerInfoPage.xaml");
            }
            list.SelectedIndex = -1;
        }

        public bool HasCustomers
        {
            get { return customers.Count > 0; }
        }

        void loadCustomers()
        {
            if (!ApplicationState.Current.Cache.Customers.IsLoaded)
            {
                ApplicationState.Current.Cache.Customers.Loaded+=onCustomersLoaded;
                progressBar.ShowProgress(true,ApplicationStrings.BodyInstructorPage_RetrievingCustomers);
                ApplicationState.Current.Cache.Customers.Load();
            }
            else
            {
                fillCustomers();
            }
        }

        void fillCustomers()
        {
            customers.Clear();
            foreach (var customer in ApplicationState.Current.Cache.Customers.Items.Values)
            {
                customers.Add(new CustomerViewModel(customer));
            }
            NotifyPropertyChanged("HasCustomers");
        }

        private void onCustomersLoaded(object sender, EventArgs e)
        {
            ApplicationState.Current.Cache.Customers.Loaded -= onCustomersLoaded;
            if(ApplicationState.Current.Cache.Customers.IsLoaded)
            {
                fillCustomers();
            }
            progressBar.ShowProgress(false);
        }

        private void GoToWebSite_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri("http://bodyarchitectonline.com");
            task.Show();
        }
    }
}