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
using BodyArchitect.Client.Module.Instructor.ViewModel;
using BodyArchitect.Client.Module.Suplements;
using BodyArchitect.Client.Module.Suplements.Controls;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Cache;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor.Controls.Customers
{
    /// <summary>
    /// Interaction logic for CustomersView.xaml
    /// </summary>
    public partial class CustomersView
    {
        private CustomersViewModel viewModel;

        public CustomersView()
        {
            InitializeComponent();
            viewModel = new CustomersViewModel(this.ParentWindow);

            var binding = new Binding("IsInProgress");
            binding.Mode = BindingMode.OneWay;
            SetBinding(IsInProgressProperty, binding);

            DataContext = viewModel;
        }

        public override AccountType AccountType
        {
            get { return Service.V2.Model.AccountType.Instructor; }
        }

        public override void Fill()
        {
            Header = "CustomersView_Fill_Header_Customers".TranslateInstructor();
            
            viewModel.Fill();

            if (PageContext != null && PageContext.Customer != null)
            {
                viewModel.SelectedItem = PageContext.Customer;
            }
        }

        public override void RefreshView()
        {
            CustomersReposidory.Instance.ClearCache();
            Fill();
        }


        public override Uri HeaderIcon
        {
            get
            {
                return new Uri("pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Images/Customer16.png", UriKind.Absolute);
            }
        }
        
        private void rbtnNew_Click(object sender, RoutedEventArgs e)
        {
            viewModel.NewCustomer();
            //lvCustomers.Refresh();
        }

        private void rbtnEdit_Click(object sender, RoutedEventArgs e)
        {
            viewModel.EditSelectedItem();
            //lvCustomers.Refresh();
        }

        private void rbtnDelete_Click(object sender, RoutedEventArgs e)
        {
            viewModel.DeleteSelectedActivity();
            //lvCustomers.Refresh();
        }

        private void rbtnShowCalendar_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ShowCalendar();
        }

        private void rbtnReporting_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ShowReports();
        }


        private void rbtnShowProductsList_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ShowProducts();
        }

        private void lvCustomers_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Delete)
            {
                viewModel.DeleteSelectedActivity();
            }
        }

        private void lvCustomers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = (CustomerDTO)lvCustomers.GetClickedItem(e);
            if (item != null)
            {
                viewModel.SelectedItem = item;
                viewModel.EditSelectedItem();
            }
        }

        private void rbtnStartCycle_Click(object sender, RoutedEventArgs e)
        {
            viewModel.StartSupplementsCycle();
        }

        private void rbtnMyTrainings_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ShowMyTrainings();
        }
    }
}
