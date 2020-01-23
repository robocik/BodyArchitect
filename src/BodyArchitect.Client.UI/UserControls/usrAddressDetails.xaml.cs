using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.UI.UserControls
{
    /// <summary>
    /// Interaction logic for usrAddressDetails.xaml
    /// </summary>
    public partial class usrAddressDetails
    {
        public usrAddressDetails()
        {
            InitializeComponent();
        }

        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set
            {
                SetValue(ReadOnlyProperty, value);
            }
        }


        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register("ReadOnly", typeof(bool), typeof(usrAddressDetails), new UIPropertyMetadata(false));

        public AddressDTO Address
        {
            get { return (AddressDTO)GetValue(AddressProperty); }
            set
            {
                SetValue(AddressProperty, value);
            }
        }

        public static readonly DependencyProperty AddressProperty =
            DependencyProperty.Register("Address", typeof(AddressDTO), typeof(usrAddressDetails), new UIPropertyMetadata(null));

    }
}
