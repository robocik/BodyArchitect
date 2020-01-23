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
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.ViewModel;
using Microsoft.Phone.Controls;

namespace BodyArchitect.WP7.Pages
{
    public partial class CardioStrengthTrainingItemPage : StrengthTrainingItemPageBase
    {
        public CardioStrengthTrainingItemPage()
        {
            InitializeComponent();
            SetControls(lstSets, lblNoSets, LayoutRoot, ctrlTimer);
        }


        protected override void NavigateToSetPage()
        {
            this.Navigate("/Pages/CardioSetPage.xaml");
        }

        public virtual void mnuDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteSet_Click(sender,e);
        }

        public virtual void Menu_Opened(object sender, RoutedEventArgs e)
        {
            Menu_Opened((ContextMenu)sender);
        }

        public virtual void Menu_Closed(object sender, RoutedEventArgs e)
        {
            Menu_Closed();
        }

        public new virtual void lstSets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            base.lstSets_SelectionChanged(sender, e);
        }

    }
}