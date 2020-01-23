using System.Windows;
using System.Windows.Data;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor.Controls.Finances
{
    /// <summary>
    /// Interaction logic for usrProductDetails.xaml
    /// </summary>
    public partial class usrProductDetails
    {
        public usrProductDetails()
        {
            InitializeComponent();
        }


        public bool ReadOnly
        {
            get { return true; }
        }

        public ProductInfoDTO Product
        {
            get { return (ProductInfoDTO)GetValue(ProductProperty); }
            set
            {
                SetValue(ProductProperty, value);
            }
        }


        public static readonly DependencyProperty ProductProperty =
            DependencyProperty.Register("Product", typeof(ProductInfoDTO), typeof(usrProductDetails), new UIPropertyMetadata(null, OnProductChanged));

        private static void OnProductChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (usrProductDetails)d;
            ctrl.DataContext = e.NewValue;
        }
    }
}
