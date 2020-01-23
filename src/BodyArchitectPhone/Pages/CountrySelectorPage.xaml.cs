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
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.ViewModel;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BodyArchitect.WP7.Pages
{
    public interface ICountrySelector
    {
        Country Country
        {
            get; set;
        }
    }
    public partial class CountrySelectorPage 
    {
        public CountrySelectorPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            DataContext = new CountrySelectorViewModel();
        }

        private void LongListSelector_GroupViewOpened(object sender, GroupViewOpenedEventArgs e)
        {
            LongList.GroupViewOpen(e);
        }

        private void LongListSelector_GroupViewClosing(object sender, GroupViewClosingEventArgs e)
        {
            LongList.GroupViewClose(e);
        }

        private void LongList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] != null)
            {
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.Content is ICountrySelector && LongList.SelectedItem != null)
                (e.Content as ICountrySelector).Country = (Country)LongList.SelectedItem;
            base.OnNavigatedFrom(e);
        }
    }
}