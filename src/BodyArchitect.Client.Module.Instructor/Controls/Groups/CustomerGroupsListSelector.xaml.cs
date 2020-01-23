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
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor.Controls.Groups
{
    /// <summary>
    /// Interaction logic for CustomerGroupsListSelector.xaml
    /// </summary>
    public partial class CustomerGroupsListSelector
    {
        public CustomerGroupsListSelector()
        {
            InitializeComponent();
            lsItems.DataContext = this;
            Connect(lsItems);

        }

        #region Appointments

        public static readonly DependencyProperty CustomerGroupsProperty =
            DependencyProperty.Register("CustomerGroups", typeof(IEnumerable<CustomerGroupDTO>), typeof(CustomerGroupsListSelector),
            new FrameworkPropertyMetadata(null));

        public IEnumerable<CustomerGroupDTO> CustomerGroups
        {
            get { return (IEnumerable<CustomerGroupDTO>)GetValue(CustomerGroupsProperty); }
            set { SetValue(CustomerGroupsProperty, value); }
        }


        #endregion   
    }
}
