using System;
using System.Collections.Generic;
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
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor.Controls.Groups
{
    /// <summary>
    /// Interaction logic for CustomerGroupsView.xaml
    /// </summary>
    public partial class CustomerGroupsView
    {
        private CustomerGroupsViewModel viewModel;

        public CustomerGroupsView()
        {
            InitializeComponent();
        }

        public override void Fill()
        {
            Header = "CustomerGroupsView_Fill_Header_Groups".TranslateInstructor();
            viewModel = new CustomerGroupsViewModel(this.ParentWindow);
            var binding = new Binding("IsInProgress");
            binding.Mode = BindingMode.OneWay;
            SetBinding(IsInProgressProperty, binding);

            DataContext = viewModel;
            viewModel.Fill();
        }

        public override AccountType AccountType
        {
            get { return Service.V2.Model.AccountType.Instructor; }
        }

        public override void RefreshView()
        {
            CustomerGroupsReposidory.Instance.ClearCache();
            viewModel.Fill();
        }


        public override Uri HeaderIcon
        {
            get
            {
                return new Uri("pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Images/CustomerGroup16.png", UriKind.Absolute);

            }
        }

        private void rbtnNew_Click(object sender, RoutedEventArgs e)
        {
            viewModel.NewGroup();
        }

        private void rbtnEdit_Click(object sender, RoutedEventArgs e)
        {
            viewModel.EditSelectedItem();
        }

        private void rbtnDelete_Click(object sender, RoutedEventArgs e)
        {
            viewModel.DeleteSelectedGroup();
        }

        private void lsItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = (CustomerGroupDTO)lsItems.GetClickedItem(e);
            if (item != null)
            {
                lsItems.SelectedItem = item;
                viewModel.EditSelectedItem();
            }
        }

        private void lsItems_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Delete)
            {
                viewModel.DeleteSelectedGroup();
            }
        }
    }
}
