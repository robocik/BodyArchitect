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
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor.Controls.Customers
{
    /// <summary>
    /// Interaction logic for usrCustomersListBox.xaml
    /// </summary>
    public partial class usrCustomersListBox
    {
        public usrCustomersListBox()
        {
            InitializeComponent();
            Connect(lvCustomers);
        }

        #region Dependency Properties

        public CustomerDTO SelectedCustomer
        {
            get { return (CustomerDTO)GetValue(SelectedCustomerProperty); }
            set
            {
                SetValue(SelectedCustomerProperty, value);
            }
        }


        public static readonly DependencyProperty SelectedCustomerProperty =
            DependencyProperty.Register("SelectedCustomer", typeof(CustomerDTO), typeof(usrCustomersListBox), new UIPropertyMetadata(null));
        /// <summary> 
        ///Gets or sets a collection used to generate the content of the ComboBox 
        /// </summary> 
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(usrCustomersListBox), new UIPropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            var ctrl = (usrCustomersListBox)d;
            if (e.OldValue != null)
            {
                ICollectionView view1 = CollectionViewSource.GetDefaultView(ctrl.lvCustomers.ItemsSource);
                view1.Filter -= ctrl.FilterPredicate;
            }

            ctrl.lvCustomers.ItemsSource = (IEnumerable) e.NewValue;

            if (e.NewValue != null)
            {
                ICollectionView view2 = CollectionViewSource.GetDefaultView(ctrl.lvCustomers.ItemsSource);
                view2.Filter += ctrl.FilterPredicate;
            }

        }


        public IEnumerable<CustomerDTO> FilterItemsSource
        {
            get { return (IEnumerable<CustomerDTO>)GetValue(FilterItemsSourceProperty); }
            set
            {
                SetValue(FilterItemsSourceProperty, value);
            }
        }

        public static readonly DependencyProperty FilterItemsSourceProperty =
            DependencyProperty.Register("FilterItemsSource", typeof(IEnumerable<CustomerDTO>), typeof(usrCustomersListBox), new UIPropertyMetadata(null));


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
            var customer = (CustomerDTO)value;

            if (FilterItemsSource != null && FilterItemsSource.Any(x => x.GlobalId == customer.GlobalId))
            {
                return false;
            }
            return customer.FirstName.SafeStartsWith(toCompare) ||
            customer.LastName.SafeStartsWith(toCompare) ||
            customer.Email.SafeStartsWith(toCompare);
        }

        private void lvCustomers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = (ActivityDTO)lvCustomers.GetClickedItem(e);
            if (item != null)
            {
                lvCustomers.SelectedItem = item;
            }
        }
    }
}
