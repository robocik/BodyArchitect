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
using BodyArchitect.Client.Module.Instructor.ViewModel;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor.Controls.Customers
{
    /// <summary>
    /// Interaction logic for usrCustomersList.xaml
    /// </summary>
    public partial class usrCustomersList
    {
        public usrCustomersList()
        {
            InitializeComponent();
        }

        

        private bool FilterPredicate(object value)
        {
            var toCompare = QueryText!=null?QueryText.ToLower():string.Empty;
            var customer = (CustomerDTO) value;
            return customer.FirstName.SafeStartsWith(toCompare) ||
            customer.LastName.SafeStartsWith(toCompare) ||
            customer.Email.SafeStartsWith(toCompare);
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
            DependencyProperty.Register("SelectedCustomer", typeof(CustomerDTO), typeof(usrCustomersList), new UIPropertyMetadata(null));
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
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(usrCustomersList), new UIPropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            var ctrl = (usrCustomersList)d;
            if (e.OldValue != null)
            {
                ICollectionView view1 = CollectionViewSource.GetDefaultView(ctrl.lvCustomers.ItemsSource);
                view1.Filter -= ctrl.FilterPredicate;
                view1.SortDescriptions.Clear();
            }

            ctrl.lvCustomers.ItemsSource = (IEnumerable)e.NewValue;

            if (e.NewValue != null)
            {
                ICollectionView view2 = CollectionViewSource.GetDefaultView(ctrl.lvCustomers.ItemsSource);
                view2.SortDescriptions.Add(new SortDescription("FullName",ListSortDirection.Ascending));
                view2.Filter += ctrl.FilterPredicate;
            }

        }

        private string _queryText;

        #endregion

        public IEnumerable<CustomerDTO> SelectedCustomers
        {
            get { return lvCustomers.SelectedItems.Cast<CustomerDTO>(); }
        }

        public CustomerDTO GetClickedItem(MouseButtonEventArgs e)
        {
            return (CustomerDTO) lvCustomers.GetClickedItem(e);
        }

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
    }
}
