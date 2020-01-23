using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls.PlansUI;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.Suplements.Controls
{
    /// <summary>
    /// Interaction logic for SupplementsCyclesList.xaml
    /// </summary>
    public partial class SupplementsCyclesList
    {
        public SupplementsCyclesList()
        {
            InitializeComponent();
            lvPlans.SelectedValuePath = "Cycle.GlobalId";
        }

        public bool ShowsGroups { get; set; }


        public void Fill(IEnumerable<SupplementCycleDefinitionDTO> plans, Guid? selectedItem)
        {
            var list = plans.Select(x => new SupplementCycleViewModel(x)).ToList();
            addFeaturedPlans(list);
            
            lvPlans.ItemsSource = list;
            lvPlans.SelectedValue = selectedItem;

            if (ShowsGroups)
            {
                CollectionView myView = (CollectionView)CollectionViewSource.GetDefaultView(lvPlans.ItemsSource);
                PropertyGroupDescription groupDescription = new PropertyGroupDescription("Group");
                myView.GroupDescriptions.Add(groupDescription);
                myView.SortDescriptions.Add(new SortDescription("FeaturedType", ListSortDirection.Ascending));
                myView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            }

        }

        void addFeaturedPlans(List<SupplementCycleViewModel> list)
        {//show groups is set to true in My plans tab (in search is false)
            if (ShowsGroups && FeaturedDataReposidory.Instance.IsLoaded)
            {
                foreach (var planInfo in FeaturedDataReposidory.Instance.Item.RandomSupplementsDefinitions)
                {
                    list.Add(new SupplementCycleViewModel(planInfo, FeaturedItem.Random));
                }
                foreach (var planInfo in FeaturedDataReposidory.Instance.Item.SupplementsDefinitions)
                {
                    list.Add(new SupplementCycleViewModel(planInfo, FeaturedItem.Latest));
                }

            }
        }

        public SupplementCycleViewModel SelectedCycleViewModel
        {
            get; set;
        }


        public event EventHandler SelectedCycleChanged;

        public SupplementCycleDefinitionDTO SelectedCycleDefinition
        {
            get { return (SupplementCycleDefinitionDTO)GetValue(SelectedCycleDefinitionProperty); }
            set
            {
                SetValue(SelectedCycleDefinitionProperty, value);
            }
        }


        public static readonly DependencyProperty SelectedCycleDefinitionProperty =
            DependencyProperty.Register("SelectedCycleDefinition", typeof(SupplementCycleDefinitionDTO), typeof(SupplementsCyclesList), new UIPropertyMetadata(null));



        private void lstPlans_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedCycleViewModel = lvPlans.SelectedItem != null ? ((SupplementCycleViewModel)lvPlans.SelectedItem) : null;
            SelectedCycleDefinition = lvPlans.SelectedItem != null ? ((SupplementCycleViewModel)lvPlans.SelectedItem).Cycle : null;
            if (SelectedCycleChanged != null)
            {
                SelectedCycleChanged(this, EventArgs.Empty);
            }
        }

        public void ClearContent()
        {
            lvPlans.ItemsSource = null;
        }

        private void lblUserName_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)e.OriginalSource;
            UserDTO user = (UserDTO)btn.Tag;
            if (!user.IsDeleted && !user.IsMe())
            {
                MainWindow.Instance.ShowUserInformation(user);
            }
        }

    }
}
