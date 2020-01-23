using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor.Controls.Groups
{
    /// <summary>
    /// Interaction logic for usrCustomerGroupMembersEditor.xaml
    /// </summary>
    public partial class usrCustomerGroupMembersEditor
    {
        public usrCustomerGroupMembersEditor()
        {
            InitializeComponent();
        }

        public CustomerGroupDetailsViewModel CustomerGroup
        {
            get { return (CustomerGroupDetailsViewModel)GetValue(CustomerGroupProperty); }
            set
            {
                SetValue(CustomerGroupProperty, value);
            }
        }


        public static readonly DependencyProperty CustomerGroupProperty =
            DependencyProperty.Register("CustomerGroup", typeof(CustomerGroupDetailsViewModel), typeof(usrCustomerGroupMembersEditor), new UIPropertyMetadata(null));



        private void lstMembers_Drop(object sender, DragEventArgs e)
        {
            var customer = (GroupMemberItem)e.Data.GetData("GroupMemberItem");
            adddCustomerToGroup(customer);
        }

        private void lstMembers_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                removeSelectedMember();
            }
        }

        void adddCustomerToGroup(GroupMemberItem customer)
        {
            if (customer != null)
            {
                CustomerGroup.AddMember(customer.Customer);
                lstMembers.Refresh();
                lstAllCustomers.Refresh();
            }
        }

        private void removeSelectedMember()
        {
            var customer = lstMembers.SelectedCustomer;
            if (customer!=null)
            {
                CustomerGroup.DeleteMember(customer.Customer);
                lstMembers.Refresh();
                lstAllCustomers.Refresh();
            }
        }

        private void lstMembers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            removeSelectedMember();
        }

        private void lstAllCustomers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            adddCustomerToGroup(lstAllCustomers.SelectedCustomer);
        }
    }
}
