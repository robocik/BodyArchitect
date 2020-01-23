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
using Microsoft.Phone.Shell;

namespace BodyArchitect.WP7.Pages
{
    public partial class SupplementChooserPage 
    {
        private SupplementChooserViewModel _viewModel;

        public SupplementChooserPage()
        {
            InitializeComponent();
            buildApplicationBar();
        }

        private void LongListSelector_GroupViewOpened(object sender, GroupViewOpenedEventArgs e)
        {
            LongList.GroupViewOpen(e);
        }

        private void LongListSelector_GroupViewClosing(object sender, GroupViewClosingEventArgs e)
        {
            LongList.GroupViewClose(e);
        }

        void buildApplicationBar()
        {
            ApplicationBar.Buttons.Clear();
            ApplicationBar.MenuItems.Clear();


            ApplicationBarIconButton button1 = new ApplicationBarIconButton(new Uri("/icons/appbar.refresh.rest.png", UriKind.Relative));
            button1.Click += new EventHandler(btnRefresh_Click);
            button1.Text = ApplicationStrings.AppBarButton_Refresh;
            button1.IsEnabled = !ApplicationState.Current.IsOffline;
            ApplicationBar.Buttons.Add(button1);

            button1 = new ApplicationBarIconButton(new Uri("/icons/appbar.feature.settings.rest.png", UriKind.Relative));
            button1.Click += new EventHandler(btnSettings_Click);
            button1.Text = ApplicationStrings.AppBarButton_Settings;
            button1.IsEnabled = !ApplicationState.Current.IsOffline;
            ApplicationBar.Buttons.Add(button1);

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (_viewModel == null || !ApplicationState.Current.Cache.Supplements.IsLoaded)
            {
                LongList.ItemsSource = null;
                _viewModel = new SupplementChooserViewModel();
                DataContext = _viewModel;

                _viewModel.SupplementsLoaded += new EventHandler(_viewModel_SupplementsLoaded);
                progressBar.ShowProgress(true, ApplicationStrings.SupplementsPage_ProgressRetrieveSupplements);
            }

            _viewModel.LoadSupplements();
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.Content is SupplementsPage)
                (e.Content as SupplementsPage).SelectedSupplement = (SuplementDTO)LongList.SelectedItem;
            base.OnNavigatedFrom(e);
        }
        void _viewModel_SupplementsLoaded(object sender, EventArgs e)
        {
            LongList.ItemsSource = _viewModel.GroupedSupplements;
            progressBar.ShowProgress(false);
        }


        private void btnSettings_Click(object sender, EventArgs e)
        {
            this.Navigate("/Pages/SettingsPage.xaml");
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            progressBar.ShowProgress(true, ApplicationStrings.SupplementsPage_ProgressRetrieveSupplements);
            LongList.ItemsSource = null;
            _viewModel.Refresh();
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
    }
}