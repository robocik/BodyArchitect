using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using BodyArchitect.Service.Client.WP7.ModelExtensions;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.Pages;
using BodyArchitect.WP7.ViewModel;
using Coding4Fun.Phone.Controls;
using Microsoft.Phone.Controls;

namespace BodyArchitect.WP7.UserControls
{
    public partial class WorkoutPlanViewControl : UserControl
    {
        private ObservableCollection<InGroup<TrainingPlanEntryViewModel>> _entries;

        public WorkoutPlanViewControl()
        {
            InitializeComponent();
        }

        public TrainingPlan TrainingPlan
        {
            get
            {
                return (TrainingPlan)GetValue(TrainingPlanProperty);
            }
            set { SetValue(TrainingPlanProperty, value); }
        }

        public TrainingPlanEntryViewModel Entry { get; set; }

        public static readonly DependencyProperty TrainingPlanProperty =
                DependencyProperty.Register("TrainingPlan",
                typeof(TrainingPlan), typeof(WorkoutPlanViewControl),
                new PropertyMetadata(null, OnTrainingPlanChanged));

        static void OnTrainingPlanChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            WorkoutPlanViewControl img = (WorkoutPlanViewControl)obj;
            img.Fill((TrainingPlan)args.NewValue);
        }

        public void Fill(TrainingPlan plan)
        {
            var groups = new Dictionary<string, InGroup<TrainingPlanEntryViewModel>>();
            _entries=new ObservableCollection<InGroup<TrainingPlanEntryViewModel>>();
            foreach (var day in plan.Days)
            {
                var group = new InGroup<TrainingPlanEntryViewModel>(day.Name);
                group.Tag = day;
                groups[day.Name] = group;
                _entries.Add(group);
                foreach (var entry in day.Entries)
                {
                    groups[day.Name].Add(new TrainingPlanEntryViewModel(entry));
                }
            }
            LongList.ItemsSource = _entries;
        }

        private void ShowSet_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = (HyperlinkButton)sender;
            this.GetParent<PhoneApplicationPage>().Navigate("/Pages/TrainingPlanEntryPage.xaml?GlobalId=" + ((TrainingPlanEntryViewModel)btn.Tag).Entry.GlobalId.ToString());
        }

        private bool buttonPressed;
        private void btnUseInToday_Click(object sender, RoutedEventArgs e)
        {
            buttonPressed = true;
            var button = (RoundButton)sender;
            InGroup<TrainingPlanEntryViewModel> group = (InGroup<TrainingPlanEntryViewModel>)button.Tag;
            TrainingPlanDay planDay = (TrainingPlanDay)group.Tag;
            var workoutPlan = planDay.TrainingPlan;
            if (!workoutPlan.IsFavorite && !workoutPlan.IsMine)
            {
                BAMessageBox.ShowInfo(ApplicationStrings.WorkoutPlanViewControl_ErrMustAddPlanToFavorites);
                return;
            }

            if (BAMessageBox.Ask(ApplicationStrings.WorkoutPlanViewControl_btnUseInToday_QUsePlanInCalendar) == MessageBoxResult.Cancel)
            {
                return;
            }

            fillStrengthTrainingEntryWithPlan(planDay);
        }

        private void fillStrengthTrainingEntryWithPlan(TrainingPlanDay planDay)
        {
            if (ApplicationState.Current.CurrentBrowsingTrainingDays.IsMine)
            {
                if(!EntryObjectPageBase.EnsureRemoveEntryTypeFromToday(typeof(StrengthTrainingEntryDTO)))
                {//cancel overwrite
                    return;
                }
                var strengthTraining = new StrengthTrainingEntryDTO();
                strengthTraining.StartTime = DateTime.Now;
                ApplicationState.Current.TrainingDay.TrainingDay.Objects.Add(strengthTraining);
                strengthTraining.TrainingDay = ApplicationState.Current.TrainingDay.TrainingDay;
                ApplicationState.Current.CurrentEntryId=new LocalObjectKey(strengthTraining);
                TrainingBuilder builder = new TrainingBuilder();
                builder.FillTrainingFromPlan(planDay, strengthTraining);
                //apply setting related with copy sets data
                builder.PrepareCopiedStrengthTraining(strengthTraining, Settings.CopyStrengthTrainingMode);
                this.GetParent<PhoneApplicationPage>().Navigate("/Pages/StrengthWorkoutPage.xaml");
            }
            
        }

        void LongList_GroupViewOpened(object sender, GroupViewOpenedEventArgs e)
        {
            if(buttonPressed)
            {
                LongList.CloseGroupView();
            }
            buttonPressed = false;
        }
    }
}
