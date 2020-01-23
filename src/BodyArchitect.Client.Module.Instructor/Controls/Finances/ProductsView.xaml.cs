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

namespace BodyArchitect.Client.Module.Instructor.Controls.Finances
{
    /// <summary>
    /// Interaction logic for ProductsView.xaml
    /// </summary>
    public partial class ProductsView
    {
        public ProductsView()
        {
            InitializeComponent();
        }

        public override void Fill()
        {
            Header = "ProductsView_Fill_Header_Products".TranslateInstructor() + Customer.FullName;
            
            usrProductsList.Customer = Customer;
            usrProductsList.Fill();
        }

        public override void RefreshView()
        {
            usrProductsList.RefreshView();
        }


        public override Uri HeaderIcon
        {
            get { return new Uri("pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Images/Finance.png", UriKind.Absolute); }
        }
    }
}
