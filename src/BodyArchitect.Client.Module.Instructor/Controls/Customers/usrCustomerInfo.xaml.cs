using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Module.Instructor.ViewModel;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.Instructor.Controls.Customers
{
    //TODO: For now we don't use this functionality so I turned it off
    //TODO: Still must be translated to polish

    //[Export(typeof(IUserDetailControlBuilder))]
    public class usrCustomerInfoBuilder : IUserDetailControlBuilder
    {
        public IUserDetailControl Create()
        {
            return new usrCustomerInfo();
        }
    }

    public partial class usrCustomerInfo : IUserDetailControl
    {
        private usrCustomerInfoViewModel viewModel;

        public usrCustomerInfo()
        {
            InitializeComponent();
            viewModel = new usrCustomerInfoViewModel(this.ParentWindow);
            DataContext = viewModel;
            var binding = new Binding("CanSave");
            binding.Mode = BindingMode.OneWay;
            progressButtons.OkButton.SetBinding(Button.IsEnabledProperty, binding);
        }

        public void Fill(ProfileInformationDTO user, bool isActive)
        {
            viewModel.Fill(user, isActive);
        }

        public string Caption
        {
            get { return "usrCustomerInfo_Caption_CustomerDetails".TranslateInstructor(); }
        }

        public ImageSource SmallImage
        {
            get
            {
                return "pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Images/CustomerInfo.png".ToBitmap();
            }
        }

        public bool UpdateGui(ProfileInformationDTO user)
        {
            return user != null && !user.User.IsMe();
        }


        private void usrProgressIndicatorButtons_OkClick(object sender, CancellationSourceEventArgs e)
        {
            viewModel.SaveCustomer();
        }
        
        private void btnNewCustomer_Click(object sender, RoutedEventArgs e)
        {
            viewModel.NewCustomer();
        }

        private void btnStartEdit_Click(object sender, RoutedEventArgs e)
        {
            viewModel.EditMode = true;
        }

        private void btnCancelSaveCustomer_Click(object sender, EventArgs e)
        {
            viewModel.EditMode = false;
        }

        private void lblShowCustomer_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Controls/Customers/CustomersView.xaml"),()=>new PageContext() { Customer = viewModel .SelectedCustomer});
        }

        private void btnDisconnectCustomer_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Disconnect();
        }
    }
}
