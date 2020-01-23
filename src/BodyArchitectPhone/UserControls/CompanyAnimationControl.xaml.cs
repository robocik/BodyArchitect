using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace BodyArchitect.WP7.UserControls
{
    public partial class CompanyAnimationControl : UserControl
    {
        public CompanyAnimationControl()
        {
            InitializeComponent();
        }



        public void BeginAnimation()
        {
            quasarAnimation.Begin();
            companyAnimation.Begin();
            devAnimation.Begin();
            createdAnimation.Begin();
        }
    }
}
