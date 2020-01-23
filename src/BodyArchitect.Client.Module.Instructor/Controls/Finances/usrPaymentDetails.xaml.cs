using System.Windows;
using System.Windows.Controls;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor.Controls.Finances
{
    /// <summary>
    /// Interaction logic for usrPaymentDetails.xaml
    /// </summary>
    public partial class usrPaymentDetails
    {
        public usrPaymentDetails()
        {
            InitializeComponent();
        }

        public bool ReadOnly
        {
            get { return true; }
        }

        public PaymentBaseDTO Payment
        {
            get { return (PaymentBaseDTO)GetValue(PaymentProperty); }
            set
            {
                SetValue(PaymentProperty, value);
            }
        }


        public static readonly DependencyProperty PaymentProperty =
            DependencyProperty.Register("Payment", typeof(PaymentBaseDTO), typeof(usrPaymentDetails), new UIPropertyMetadata(null));


    }
}
