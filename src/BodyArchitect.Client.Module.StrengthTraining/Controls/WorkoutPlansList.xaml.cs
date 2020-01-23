using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.StrengthTraining.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls.PlansUI;
using BodyArchitect.Client.UI.UserControls;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using BodyArchitect.Shared;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls
{
    /// <summary>
    /// Interaction logic for WorkoutPlansList.xaml
    /// </summary>
    public partial class WorkoutPlansList 
    {

        public WorkoutPlansList()
        {
            InitializeComponent();
            lvPlans.SelectedValuePath = "Plan.GlobalId";
        }

        public bool ShowsGroups { get; set; }

        private string _emptyListMessage;
        public string EmptyListMessage
        {
            get { return _emptyListMessage; }
            set
            {
                _emptyListMessage = value;
                NotifyOfPropertyChange(()=>EmptyListMessage);
            }
        }

        public void Fill(IList<TrainingPlan> plans,Guid? selectedPlan)
        {
            
            var list = plans.Select(x => new WorkoutPlanViewModel(x)).ToList();
            addFeaturedPlans(list);
            lvPlans.ItemsSource = list;
            lvPlans.SelectedValue = selectedPlan;
            
            
            if (ShowsGroups)
            {
                CollectionView myView = (CollectionView)CollectionViewSource.GetDefaultView(lvPlans.ItemsSource);
                PropertyGroupDescription groupDescription = new PropertyGroupDescription("Group");
                myView.GroupDescriptions.Add(groupDescription);
                myView.SortDescriptions.Add(new SortDescription("FeaturedType",ListSortDirection.Ascending));
                myView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            }

        }

        void addFeaturedPlans(List<WorkoutPlanViewModel> list )
        {//show groups is set to true in My plans tab (in search is false)
            if (ShowsGroups && FeaturedDataReposidory.Instance.IsLoaded)
            {
                foreach (var planInfo in FeaturedDataReposidory.Instance.Item.RandomTrainingPlans)
                {
                    list.Add(new WorkoutPlanViewModel(planInfo, FeaturedItem.Random));
                }
                foreach (var planInfo in FeaturedDataReposidory.Instance.Item.LatestTrainingPlans)
                {
                    list.Add(new WorkoutPlanViewModel(planInfo, FeaturedItem.Latest));
                }
                
            }
        }
        
        public event EventHandler SelectedPlanChanged;

        public TrainingPlan SelectedPlan
        {
            get
            {
                if (lvPlans.SelectedItem != null)
                {
                    return ((WorkoutPlanViewModel)lvPlans.SelectedItem).Plan;
                }
                return null;
            }
        }

        public WorkoutPlanViewModel SelectedPlanViewModel
        {
            get
            {
                if (lvPlans.SelectedItem != null)
                {
                    return (WorkoutPlanViewModel)lvPlans.SelectedItem;
                }
                return null;
            }
        }

        private void lstPlans_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedPlanChanged != null)
            {
                SelectedPlanChanged(this, EventArgs.Empty);
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
