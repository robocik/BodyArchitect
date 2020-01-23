using System;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.ViewModel;
using Microsoft.Phone.Controls;

namespace BodyArchitect.WP7.Pages
{
    public partial class SupplementItemPage 
    {
        private SupplementItemViewModel viewModel;

        public SupplementItemPage()
        {
            InitializeComponent();
        }

        public SuplementItemDTO SelectedItem { get; set; }

        
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            StateHelper stateHelper = new StateHelper(this.State);
            var item = stateHelper.GetValue<Guid>("SelectedItemId", Guid.Empty);
            if (item != Guid.Empty && (SelectedItem == null || SelectedItem.InstanceId == item))
            {
                //SelectedItem = ApplicationState.Current.TrainingDay.TrainingDay.Supplements.GetItem(item);
                SelectedItem = Entry.GetItem(item);

            }

            viewModel = new SupplementItemViewModel(SelectedItem);
            DataContext = viewModel;
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            this.State["SelectedItemId"] = SelectedItem.InstanceId;
        }
    }
}