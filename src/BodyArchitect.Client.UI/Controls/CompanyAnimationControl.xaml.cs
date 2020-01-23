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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BodyArchitect.Client.Resources.Localization;

namespace BodyArchitect.Client.UI.Controls
{
    public partial class CompanyAnimationControl
    {
        public CompanyAnimationControl()
        {
            InitializeComponent();
        }

        public void BeginAnimation()
        {
            tbCreatedBy.Text = Strings.CompanyAnimationControl_CreatedBy;
            quasarAnimation.Begin();
            companyAnimation.Begin();
            devAnimation.Begin();
            createdAnimation.Begin();
        }
    }
}
