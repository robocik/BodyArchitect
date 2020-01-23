using System;
using System.Collections;
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
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor.Controls.Groups
{
    /// <summary>
    /// Interaction logic for usrGroupMembersListBox.xaml
    /// </summary>
    public partial class usrGroupMembersListBox
    {
        
        public usrGroupMembersListBox()
        {
            InitializeComponent();
            Connect(lvCustomers);
        }

        #region Dependency Properties

        public GroupMemberItem SelectedCustomer
        {
            get { return (GroupMemberItem)GetValue(SelectedCustomerProperty); }
            set
            {
                SetValue(SelectedCustomerProperty, value);
            }
        }


        public static readonly DependencyProperty SelectedCustomerProperty =
            DependencyProperty.Register("SelectedCustomer", typeof(GroupMemberItem), typeof(usrGroupMembersListBox), new UIPropertyMetadata(null));
        /// <summary> 
        ///Gets or sets a collection used to generate the content of the ComboBox 
        /// </summary> 
        public IEnumerable<GroupMemberItem> ItemsSource
        {
            get { return (IEnumerable<GroupMemberItem>)GetValue(ItemsSourceProperty); }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(usrGroupMembersListBox), new UIPropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            var ctrl = (usrGroupMembersListBox)d;
            if (e.OldValue != null)
            {
                ICollectionView view1 = CollectionViewSource.GetDefaultView(ctrl.lvCustomers.ItemsSource);
                view1.Filter -= ctrl.FilterPredicate;
            }

            ctrl.lvCustomers.ItemsSource = (IEnumerable<GroupMemberItem>)e.NewValue;

            if (e.NewValue != null)
            {
                ICollectionView view2 = CollectionViewSource.GetDefaultView(ctrl.lvCustomers.ItemsSource);
                view2.Filter += ctrl.FilterPredicate;
            }

        }


        public IEnumerable<GroupMemberItem> FilterItemsSource
        {
            get { return (IEnumerable<GroupMemberItem>)GetValue(FilterItemsSourceProperty); }
            set
            {
                SetValue(FilterItemsSourceProperty, value);
            }
        }

        public static readonly DependencyProperty FilterItemsSourceProperty =
            DependencyProperty.Register("FilterItemsSource", typeof(IEnumerable<GroupMemberItem>), typeof(usrGroupMembersListBox), new UIPropertyMetadata(null));


        private string _queryText;

        #endregion


        public string QueryText
        {
            get { return _queryText; }
            set
            {
                if (_queryText != value)
                {
                    _queryText = value;
                    Refresh();
                }

            }
        }

        public void Refresh()
        {
            lvCustomers.Refresh();
        }

        private bool FilterPredicate(object value)
        {
            var toCompare = QueryText != null ? QueryText.ToLower() : string.Empty;
            var customer = ((GroupMemberItem)value).Customer;

            if (FilterItemsSource != null && FilterItemsSource.Any(x => x.Customer.GlobalId == customer.GlobalId))
            {
                return false;
            }
            return customer.FirstName.SafeStartsWith(toCompare) ||
            customer.LastName.SafeStartsWith(toCompare) ||
            customer.Email.SafeStartsWith(toCompare);
        }

        private void lvCustomers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = (GroupMemberItem)lvCustomers.GetClickedItem(e);
            if (item != null)
            {
                lvCustomers.SelectedItem = item;
            }
        }
    }

    public class GroupMemberItem:ViewModelBase
    {
        private CustomerDTO customer;

        public GroupMemberItem(CustomerDTO customer)
        {
            this.customer = customer;
        }

        public CustomerDTO Customer{get { return customer; }}

        private bool _isError;
        public bool IsError
        {
            get { return _isError; }
            set
            {
                _isError = value;
                NotifyOfPropertyChange(()=>IsError);
            }
        }

        private bool _isEnabled=true;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                NotifyOfPropertyChange(() => IsEnabled);
            }
        }

        private string _errorTooltip;
        public string ErrorToolTip
        {
            get { return _errorTooltip; }
            set
            {
                _errorTooltip = value;
                NotifyOfPropertyChange(() => ErrorToolTip);
            }
        }
    }
}
