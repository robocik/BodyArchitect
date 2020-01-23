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
using System.Windows.Shapes;
using BodyArchitect.Client.Common;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.UI.Windows
{
    /// <summary>
    /// Interaction logic for ChangeAccountTypeWindow.xaml
    /// </summary>
    public partial class ChangeAccountTypeWindow
    {
        private bool _inPremium;

        public ChangeAccountTypeWindow(bool inPremium)
        {
            InitializeComponent();
            DataContext = this;
            InPremium = inPremium;
        }

        public bool InPremium
        {
            get { return _inPremium; }
            set
            {
                _inPremium = value;
                NotifyOfPropertyChange(()=>InPremium);
            }
        }


        private void btnShowDetails_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
