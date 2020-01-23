using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI.Cache;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Controls.PlansUI;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Service.V2.Model.Reports;
using BodyArchitect.Service.V2.Model.TrainingPlans;

namespace BodyArchitect.Client.UI.Views
{
    /// <summary>
    /// Interaction logic for FeaturedItemsView.xaml
    /// </summary>
    public partial class FeaturedItemsView : IWeakEventListener
    {
        ObservableCollection<PlanViewModel> latestSupplementsDefinitions = new ObservableCollection<PlanViewModel>();
        ObservableCollection<PlanViewModel> randomSupplementsDefinitions = new ObservableCollection<PlanViewModel>();
        ObservableCollection<PlanViewModel> latestTrainingPlans = new ObservableCollection<PlanViewModel>();
        ObservableCollection<PlanViewModel> randomTrainingPlans = new ObservableCollection<PlanViewModel>();
        ObservableCollection<FeaturedEntryObjectDTO> latestBlogs = new ObservableCollection<FeaturedEntryObjectDTO>();
        ObservableCollection<FeaturedEntryObjectDTO> latestStrengthTrainings = new ObservableCollection<FeaturedEntryObjectDTO>();
        ObservableCollection<ExerciseRecordViewModel> records = new ObservableCollection<ExerciseRecordViewModel>();
        
        public FeaturedItemsView()
        {
            InitializeComponent();
            DataContext = this;
            Header = "FeaturedItemsView_Header".TranslateGUI();
        }

        public override void Fill()
        {
            IsInProgress = true;
            ParentWindow.RunAsynchronousOperation(delegate(OperationContext context)
            {
                FeaturedDataReposidory.Instance.EnsureLoaded();

                UIHelper.BeginInvoke(() =>
                {
                    latestSupplementsDefinitions.Clear();
                    randomSupplementsDefinitions.Clear();
                    latestTrainingPlans.Clear();
                    randomTrainingPlans.Clear();
                    latestBlogs.Clear();
                    latestStrengthTrainings.Clear();
                    records.Clear();
                    foreach (var definitionDto in FeaturedDataReposidory.Instance.Item.SupplementsDefinitions)
                    {
                        latestSupplementsDefinitions.Add(new PlanViewModel(definitionDto,FeaturedItem.Latest));
                    }
                    foreach (var definitionDto in FeaturedDataReposidory.Instance.Item.RandomSupplementsDefinitions)
                    {
                        randomSupplementsDefinitions.Add(new PlanViewModel(definitionDto, FeaturedItem.Random));
                    }
                    foreach (var definitionDto in FeaturedDataReposidory.Instance.Item.LatestTrainingPlans)
                    {
                        latestTrainingPlans.Add(new PlanViewModel(definitionDto, FeaturedItem.Latest));
                    }
                    foreach (var definitionDto in FeaturedDataReposidory.Instance.Item.RandomTrainingPlans)
                    {
                        randomTrainingPlans.Add(new PlanViewModel(definitionDto, FeaturedItem.Random));
                    }
                    foreach (var entry in FeaturedDataReposidory.Instance.Item.LatestBlogs)
                    {
                        latestBlogs.Add(entry);
                    }
                    foreach (var entry in FeaturedDataReposidory.Instance.Item.LatestStrengthTrainings)
                    {
                        latestStrengthTrainings.Add(entry);
                    }
                    foreach (var record in FeaturedDataReposidory.Instance.Item.Records)
                    {
                        records.Add(new ExerciseRecordViewModel(record,null,0));
                    }
                    IsInProgress = false;
                }, Dispatcher);
            });
        }

        public override void RefreshView()
        {
            FeaturedDataReposidory.Instance.ClearCache();
            Fill();
        }

        public override Uri HeaderIcon
        {
            get { return "Featured16.png".ToResourceUrl(); }
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            UIHelper.BeginInvoke(Fill, Dispatcher);
            return true;
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

        public ICollection<ExerciseRecordViewModel> Records
        {
            get { return records; }
        }

        public ICollection<FeaturedEntryObjectDTO> LatestBlogs
        {
            get { return latestBlogs; }
        }

        public ICollection<FeaturedEntryObjectDTO> LatestStrengthTrainings
        {
            get { return latestStrengthTrainings; }
        }

        public ICollection<PlanViewModel> RandomTrainingPlans
        {
            get { return randomTrainingPlans; }
        }

        public ICollection<PlanViewModel> LatestTrainingPlans
        {
            get { return latestTrainingPlans; }
        }


        public ICollection<PlanViewModel> RandomSupplementsDefinitions
        {
            get { return randomSupplementsDefinitions; }
        }

        public ICollection<PlanViewModel> LatestSupplementsDefinitions
        {
            get { return latestSupplementsDefinitions; }
        }

        private void btnGoToPlan_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)e.OriginalSource;
            goToPlan(btn.Tag);
        }

        private static void goToPlan(object tag)
        {
            var supple =tag as SupplementCycleDefinitionDTO;
            var plan = tag as TrainingPlan;

            if (supple != null)
            {
                MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.Module.Suplements;component/Controls/SupplementsCyclesView.xaml"), () => new PageContext() { SelectedItem = supple.GlobalId });
            }
            else if (plan != null)
            {
                MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.Module.StrengthTraining;component/Controls/TrainingPlansView.xaml"),()=>new PageContext() {SelectedItem = plan.GlobalId});
            }
        }

        private void plan_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var btn = (Control)sender;
            goToPlan(btn.Tag);
        }

        private void record_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var btn = (ContentControl)sender;
            ExerciseRecordsReportResultItem item = (ExerciseRecordsReportResultItem)btn.Tag;
            goToRecordTrainingDay(item);
        }

        void goToRecordTrainingDay(ExerciseRecordsReportResultItem item)
        {
            MainWindow.Instance.ShowTrainingDayReadOnly(item.TrainingDate, item.User, item.CustomerId);
        }

        private void btnRecordTrainingDay_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            ExerciseRecordsReportResultItem item = (ExerciseRecordsReportResultItem)btn.Tag;
            goToRecordTrainingDay(item);
        }

        private void btnShowExerciseRecords_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            ExerciseRecordsReportResultItem item = (ExerciseRecordsReportResultItem)btn.Tag;

            MainWindow.Instance.ShowPage(
                new Uri("pack://application:,,,/BodyArchitect.Client.Module.StrengthTraining;component/Controls/ExercisesView.xaml"),
                () => new PageContext()
                {
                    SelectedItem = item.Exercise.GlobalId,
                    DisplayMode = 2
                });
        }

        private void btnGoToTrainingDay_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            var item = (FeaturedEntryObjectDTO)btn.Tag;
            MainWindow.Instance.ShowTrainingDayReadOnly(item.DateTime, item.User, null);
        }

        private void featuredItem_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var btn = (ContentControl)sender;
            var item = (FeaturedEntryObjectDTO)btn.Tag;
            MainWindow.Instance.ShowTrainingDayReadOnly(item.DateTime, item.User, null);
        }
    }


    public class ExerciseRecordViewModel : ViewModelBase
    {
        private ExerciseRecordsReportResultItem item;
        private PagedResult<ExerciseRecordsReportResultItem> result;
        private int pageSize;

        public ExerciseRecordViewModel(ExerciseRecordsReportResultItem item, PagedResult<ExerciseRecordsReportResultItem> result, int pageSize)
        {
            this.pageSize = pageSize;
            this.item = item;
            this.result = result;
        }

        public bool CalendarAvailable
        {
            get
            {
                if (item.CustomerId == null)
                {
                    return item.User.IsMe() || item.User.HaveAccess(item.User.Privacy.CalendarView);
                }
                return true;
            }
        }

        public string UserName
        {
            get
            {
                if(item.CustomerId==null)
                {
                    return item.User.UserName;
                }
                return CustomersReposidory.Instance.GetItem(item.CustomerId.Value).FullName;
            }
        }

        public ExerciseRecordsReportResultItem Item
        {
            get { return item; }
        }

        public string CardioValue
        {
            get
            {
                var time = TimeSpan.FromSeconds((double)item.MaxWeight);
                return time.ToString();
            }
        }

        public bool IsCardio
        {
            get { return item.Exercise.ExerciseType == ExerciseType.Cardio; }
        }

        public int Position
        {
            get
            {
                return (result.PageIndex * pageSize) + result.Items.IndexOf(item) + 1;
            }
        }

        public decimal Weight
        {
            get { return item.MaxWeight.ToDisplayWeight(); }
        }

        public string WeightType
        {
            get
            {
                if (UserContext.Current.ProfileInformation.Settings.WeightType == BodyArchitect.Service.V2.Model.WeightType.Pounds)
                {
                    return Strings.WeightType_Pound;
                }
                else
                {
                    return Strings.WeightType_Kg;
                }
            }
        }
    }

}
