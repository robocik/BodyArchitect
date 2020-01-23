using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.UserControls;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor.Controls.Finances
{
    /// <summary>
    /// Interaction logic for usrProductsList.xaml
    /// </summary>
    public partial class usrProductsList
    {
        public usrProductsList()
        {
            InitializeComponent();
            DataContext = this;
            FillSearchCriterias();
            Loaded += new RoutedEventHandler(usrProductsList_Loaded);
        }

        void usrProductsList_Loaded(object sender, RoutedEventArgs e)
        {
            this.dataGrid1.SetTheme("BA");
        }

        protected override void FillResults(ObservableCollection<ProductInfoDTO> result)
        {
            dataGrid1.ItemsSource = result.Select(x=>new ProductInfoViewModel(x));
            
        }

        public void Fill()
        {
            DoSearch();
        }

        public void RefreshView()
        {
            Fill();
        }


        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            DoSearch();
        }

        private void btnMoreResults_Click(object sender, RoutedEventArgs e)
        {
            MoreResults();
        }

    }

    public abstract class ProductsPagerListUserControl : PagerListUserControl<ProductInfoDTO>
    {
        private CustomerDTO customerDTO;
        private GetProductsParam criteria;
        private DateTime? startTime;
        private DateTime? endTime;
        private ObservableCollection<CheckListItem<ProductsSortOrder>> sortOrders = new ObservableCollection<CheckListItem<ProductsSortOrder>>();
        private ObservableCollection<CheckListItem<PaymentCriteria>> payments = new ObservableCollection<CheckListItem<PaymentCriteria>>();
        private bool sortAscending;
        private ProductsSortOrder selectedSortOrder;
        private PaymentCriteria selectedPayment;

        public ObservableCollection<CheckListItem<PaymentCriteria>> Payments
        {
            get { return payments; }
        }

        public ObservableCollection<CheckListItem<ProductsSortOrder>> SortOrders
        {
            get { return sortOrders; }
        }

        public PaymentCriteria SelectedPayment
        {
            get { return selectedPayment; }
            set
            {
                selectedPayment = value;
                NotifyOfPropertyChange(() => SelectedPayment);
            }
        }

        public bool SortAscending
        {
            get { return sortAscending; }
            set
            {
                sortAscending = value;
                NotifyOfPropertyChange(() => SortAscending);
            }
        }

        public DateTime? StartTime
        {
            get { return startTime; }
            set
            {
                startTime = value;
                NotifyOfPropertyChange(()=>StartTime);
            }
        }
        public DateTime? EndTime
        {
            get { return endTime; }
            set
            {
                endTime = value;
                NotifyOfPropertyChange(() => EndTime);
            }
        }

        public CustomerDTO Customer
        {
            get { return customerDTO; }
            set { customerDTO = value; }
        }

        public ProductsSortOrder SelectedSortOrder
        {
            get { return selectedSortOrder; }
            set
            {
                selectedSortOrder = value;
                NotifyOfPropertyChange(() => SelectedSortOrder);
            }
        }

        protected override void BeforeSearch(object param = null)
        {
            criteria = new GetProductsParam();
            if (Customer != null)
            {
                criteria.CustomerId = Customer.GlobalId;
            }
            criteria.PaymentCriteria = SelectedPayment;
            criteria.SortOrder = SelectedSortOrder;
            criteria.SortAscending = SortAscending;
            criteria.StartTime = StartTime;
            criteria.EndTime = EndTime;
        }

        protected override PagedResult<ProductInfoDTO> RetrieveItems(PartialRetrievingInfo pagerInfo)
        {
            return ServiceManager.GetProducts(criteria, pagerInfo);
        }

        protected void FillSearchCriterias()
        {
            StartTime = EndTime = null;
            SortAscending = false;
            SearchStatus = string.Empty;
            SortOrders.Clear();
            Payments.Clear();

            foreach (PaymentCriteria test in Enum.GetValues(typeof(PaymentCriteria)))
            {
                var item = new CheckListItem<PaymentCriteria>(EnumLocalizer.Default.Translate(test), test);
                Payments.Add(item);
            }

            foreach (ProductsSortOrder test in Enum.GetValues(typeof(ProductsSortOrder)))
            {
                var item = new CheckListItem<ProductsSortOrder>(EnumLocalizer.Default.Translate(test), test);
                SortOrders.Add(item);
            }


            SelectedPayment = PaymentCriteria.Any;
            SelectedSortOrder = ProductsSortOrder.ByDate;
        }
    }

    public class ProductInfoViewModel
    {
        private ProductInfoDTO productInfo;

        public ProductInfoViewModel(ProductInfoDTO info)
        {
            this.productInfo = info;
        }

        public bool IsPaid
        {
            get { return ProductInfo.Product.IsPaid; }
        }

        public decimal Price
        {
            get { return ProductInfo.Product.Price; }
        }

        public string Name
        {
            get { return ProductInfo.Product.Name; }
        }
        
        public DateTime DateTime
        {
            get { return ProductInfo.Product.DateTime; }
        }

        public bool IsPresent
        {
            get
            {
                return ((ScheduleEntryReservationDTO)ProductInfo.Product).IsPresent;
            }
        }


        public ProductInfoDTO ProductInfo
        {
            get { return productInfo; }
        }
    }
}
