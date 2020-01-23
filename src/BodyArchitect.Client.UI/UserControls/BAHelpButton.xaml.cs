using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using BodyArchitect.Client.Common;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.UI.UserControls
{
    
    /// <summary>
    /// Interaction logic for BAHelpButton.xaml
    /// </summary>
    public partial class BAHelpButton : IWeakEventListener
    {
        public BAHelpButton()
        {
            InitializeComponent();
            PropertyChangedEventManager.AddListener(UserContext.Current, this, string.Empty);
            Header = EnumLocalizer.Default.GetGUIString("HelpButton");
            DataContext = this;
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            helpPopup.IsOpen = true;
        }

        public static DependencyProperty HeaderProperty =DependencyProperty.Register("Header",typeof(string),typeof(BAHelpButton));

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static DependencyProperty DescriptionProperty =DependencyProperty.Register("Description",typeof(string),typeof(BAHelpButton));

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static DependencyProperty ToolTipWidthProperty = DependencyProperty.Register("ToolTipWidth", typeof(double), typeof(BAHelpButton), new UIPropertyMetadata(200.0));

        public double ToolTipWidth
        {
            get { return (double)GetValue(ToolTipWidthProperty); }
            set { SetValue(ToolTipWidthProperty, value); }
        }

        public static DependencyProperty AccountTypeProperty = DependencyProperty.Register("AccountType", typeof(AccountType), typeof(BAHelpButton));

        public AccountType AccountType
        {
            get { return (AccountType)GetValue(AccountTypeProperty); }
            set { SetValue(AccountTypeProperty, value); }
        }

        public bool IsLicenceGood
        {
            get { return AccountType <= UserContext.Current.ProfileInformation.Licence.CurrentAccountType; }
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            NotifyOfPropertyChange(()=>IsLicenceGood);
            return true;
        }
    }
}
