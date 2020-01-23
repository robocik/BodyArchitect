using System;
using System.Windows;
using System.Windows.Controls;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.Pages;
using BodyArchitect.WP7.UserControls;
using BodyArchitect.WP7.ViewModel;
using BodyArchitectCustom;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BodyArchitect.WP7
{
    public partial class StrengthWorkoutItemPage : StrengthTrainingItemPageBase
    {
        public StrengthWorkoutItemPage()
        {
            InitializeComponent();
            SetControls(lstSets, lblNoSets, LayoutRoot, ctrlTimer);
        }

        public virtual void mnuDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteSet_Click(sender, e);
        }


        public virtual void Menu_Opened(object sender, RoutedEventArgs e)
        {
            Menu_Opened((ContextMenu)sender);
        }

        public virtual void Menu_Closed(object sender, RoutedEventArgs e)
        {
            Menu_Closed();
        }

        public virtual void lstSets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            base.lstSets_SelectionChanged(sender, e);
        }

        
    }
}