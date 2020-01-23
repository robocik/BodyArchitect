using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.StrengthTraining.Localization;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using BodyArchitect.Shared;
using BodyArchitect.Client.UI.Controls.WatermarkExtension;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls
{
    /// <summary>
    /// Interaction logic for usrWorkoutPlansChooser.xaml
    /// </summary>
    public partial class usrWorkoutPlansChooser
    {
        private StrengthTrainingEntryDTO strengthEntry;
        private bool suppressEventForTrainingPlan;

        public event EventHandler SelectedPlanDayChanged;
        private bool loaded;

        public usrWorkoutPlansChooser()
        {
            InitializeComponent();
        }

        private void btnShowTrainingPlan_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPlan == null)
            {
                return;
            }
            UIHelper.RunWithExceptionHandling(delegate
            {
                SelectedPlan.Open();
                setTrainingPlans(FillWorkoutPlans(false));
            });
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow.RunAsynchronousOperation(delegate
            {
                WorkoutPlansReposidory.Instance.ClearCache();
                WorkoutPlansReposidory.Instance.EnsureLoaded();
                loaded = false;
                var plans = FillWorkoutPlans(true);
                Dispatcher.BeginInvoke(new Action(() => setTrainingPlans(plans)));
            }, updateGui);
        }

        public void Fill(StrengthTrainingEntryDTO strengthEntry)
        {
            this.strengthEntry = strengthEntry;
            updateGui(false);
            if (!WorkoutPlansReposidory.Instance.IsLoaded)
            {
                return;
            }

            setTrainingPlans(FillWorkoutPlans(false));
            
        }
        private bool queryingPopup = false;

        public TrainingPlanDay SelectedPlanDay
        {
            get
            {
                if (cmbTrainingPlans.SelectedValue != null)
                {
                    var plans = (List<TrainingPlanItem>)cmbTrainingPlans.ItemsSource;
                    foreach (var trainingPlanItem in plans)
                    {
                        if (trainingPlanItem.GlobalId == (Guid)cmbTrainingPlans.SelectedValue)
                        {
                            return trainingPlanItem.TrainingPlanDay;
                        }

                    }
                }
                return null;
            }
        }

        public TrainingPlan SelectedPlan
        {
            get
            {

                if (cmbTrainingPlans.SelectedValue != null)
                {
                    var plans = (List<TrainingPlanItem>)cmbTrainingPlans.ItemsSource;
                    foreach (var trainingPlanItem in plans)
                    {
                        if (trainingPlanItem.GlobalId == (Guid)cmbTrainingPlans.SelectedValue)
                        {
                            Guid id = trainingPlanItem.TrainingPlanDay != null
                                          ? trainingPlanItem.TrainingPlanDay.TrainingPlan.GlobalId
                                          : trainingPlanItem.GlobalId;
                            return WorkoutPlansReposidory.Instance.GetItem(id);
                        }

                    }
                }
                return null;
            }
        }

        private void setTrainingPlans(IList<TrainingPlanItem> plans)
        {
            cmbTrainingPlans.SetCurrentValue(ComboBox.SelectedItemProperty,null);
            cmbTrainingPlans.SelectedValue = null;
            cmbTrainingPlans.ItemsSource = plans;
            if (strengthEntry!=null && strengthEntry.TrainingPlanItemId.HasValue)
            {
                suppressEventForTrainingPlan = true;
                if (plans.Where(x => x.GlobalId == strengthEntry.TrainingPlanItemId.Value).SingleOrDefault() != null)
                {
                    cmbTrainingPlans.SelectedValue = strengthEntry.TrainingPlanItemId.Value;
                }
                else
                {
                    cmbTrainingPlans.SelectedValue = strengthEntry.TrainingPlanId.Value;
                }

                suppressEventForTrainingPlan = false;
            }
            else
            {
                //cmbTrainingPlans.SelectedIndex = -1;
            }
            onSelectedPlanDayChanged();
            updateGui(false);
        }

        internal List<TrainingPlanItem> FillWorkoutPlans(bool forceFill)
        {
            List<TrainingPlanItem> plans = new List<TrainingPlanItem>();
            var trainingPlans = WorkoutPlansReposidory.Instance.Items.Values;
            foreach (var trainingPlanDto in trainingPlans)
            {
                if (trainingPlanDto.Tag!=null || forceFill)
                {
                    var trainingPlan = trainingPlanDto;

                    foreach (var planDay in trainingPlan.Days)
                    {
                        plans.Add(new TrainingPlanItem(trainingPlan.Name, trainingPlanDto.Profile.UserName, trainingPlan.TrainingType, planDay.Name,
                                                       planDay.GlobalId, planDay));
                    }
                }
                else
                {
                    plans.Add(new TrainingPlanItem(trainingPlanDto.Name, trainingPlanDto.Profile.UserName, trainingPlanDto.TrainingType, StrengthTrainingEntryStrings.usrStrengthTraining_fillWorkoutPlans_NotLoaded,
                                                       trainingPlanDto.GlobalId, null));
                }
            }
            return plans;
        }

        void updateGui(OperationContext context)
        {
            bool start = context.State == OperationState.Started;
            Cursor = start ? Cursors.Wait : Cursors.Arrow;
            updateGui(start);
        }

        private void updateGui(bool start)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action<bool>(updateGui), start);
            }
            else
            {
                cmbTrainingPlans.IsEnabled = strengthEntry != null && strengthEntry.IsNew && !start;
                btnShowTrainingPlan.SetVisible( cmbTrainingPlans.SelectedItem != null && !start && (SelectedPlanDay != null || SelectedPlan != null));
                btnRefresh.IsEnabled = !start;
                progressIndicator1.IsRunning = start;
                progressViewBox.SetVisible(start);
            }

        }

        private void cmbTrainingPlans_DropDownOpened(object sender, EventArgs e)
        {
            if (queryingPopup || loaded)
            {
                queryingPopup = false;
                return;
            }
            queryingPopup = true;

            ParentWindow.RunAsynchronousOperation(delegate
            {
                
                List<TrainingPlanItem> plans = FillWorkoutPlans(true);
                Dispatcher.BeginInvoke(new Action(delegate
                                                 {
                                                     var selectedId=cmbTrainingPlans.SelectedValue;
                                                     cmbTrainingPlans.ItemsSource = plans;
                                                     cmbTrainingPlans.SelectedValue = selectedId;
                                                     loaded = true;
                                                     cmbTrainingPlans.IsEnabled = true;
                                                     cmbTrainingPlans.IsDropDownOpen = true;
                                                 }));
            }, updateGui);
        }


        private void cmbTrainingPlans_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbTrainingPlans.IsDropDownOpen = false;
            if (suppressEventForTrainingPlan || strengthEntry==null)
            {
                return;
            }
            onSelectedPlanDayChanged();
           
            updateGui(false);
        }

        private void onSelectedPlanDayChanged()
        {
            if (SelectedPlanDayChanged != null)
            {
                SelectedPlanDayChanged(this, EventArgs.Empty);
            }
        }
    }

    class TrainingPlanItem
    {
        public TrainingPlanItem(string name, string author, TrainingType trainingType, string dayName, Guid globalId, TrainingPlanDay trainingPlan)
        {
            Name = name;
            TrainingType = trainingType;
            DayName = dayName;
            GlobalId = globalId;
            TrainingPlanDay = trainingPlan;
            Author = author;
        }

        public string Name { get; set; }
        public TrainingType TrainingType { get; set; }
        public string DayName { get; set; }
        public Guid GlobalId { get; set; }
        public TrainingPlanDay TrainingPlanDay { get; set; }
        public string Author { get; set; }

        public override string ToString()
        {
            return string.Format("{0}-{1}", Name, DayName);
        }
    }
}
