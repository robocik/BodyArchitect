using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Module.StrengthTraining.Localization;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Views.MyPlace;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Service.V2.Model.Reports;

namespace BodyArchitect.Client.Module.StrengthTraining.Reports
{


    public partial class WeightRepetitionsReportSettings : IWeakEventListener
    {
        ObservableCollection<CheckListItem<ExerciseDTO>> items = new ObservableCollection<CheckListItem<ExerciseDTO>>();
        

        public WeightRepetitionsReportSettings()
        {
            InitializeComponent();

            DataContext = this;
            var setTypes = new List<ListItem<SetType>>();
            foreach (SetType value in Enum.GetValues(typeof(SetType)))
            {
                var item = new ListItem<SetType>(EnumLocalizer.Default.Translate(value), value);
                setTypes.Add(item);
            }
            cmbSetTypes.ItemsSource = setTypes;

            List<ListItem<ExerciseDoneWay>> doneWays = new List<ListItem<ExerciseDoneWay>>();
            foreach (ExerciseDoneWay value in Enum.GetValues(typeof(ExerciseDoneWay)))
            {
                var item = new ListItem<ExerciseDoneWay>(EnumLocalizer.Default.Translate(value), value);
                doneWays.Add(item);
            }
            chkDoneWay.ItemsSource = doneWays;
            chkRestPause.IsChecked = null;
            chkSuperSlow.IsChecked = null;
            listView1.ItemsSource = items;

            PropertyChangedEventManager.AddListener(UserContext.Current, this, string.Empty);
            updateGuiToLicence();
        }

        void updateGuiToLicence()
        {
            chkDoneWay.IsEnabled = chkPlaces.IsEnabled = UserContext.IsPremium;
        }
        private void CheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            OnCanGenerateReportChanged();
        }

        public void Initialize(UserDTO user, CustomerDTO customer)
        {
            Guid? userId = (user != null ? user.GlobalId : (Guid?) null);
            items.Clear();
            foreach (var exercise in ExercisesReposidory.Instance.Items.Values)
            {
                if (exercise.ExerciseType != ExerciseType.Cardio)
                {
                    var item = new CheckListItem<ExerciseDTO>(exercise.GetLocalizedName(), exercise);
                    item.Group = EnumLocalizer.Default.Translate(exercise.ExerciseType);
                    items.Add(item);
                }
            }
            CollectionView myView = (CollectionView)CollectionViewSource.GetDefaultView(listView1.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Group");
            myView.GroupDescriptions.Add(groupDescription);
            myView.SortDescriptions.Add(new SortDescription("Text", ListSortDirection.Ascending));

            ParentWindow.RunAsynchronousOperation((x) =>
            {
                var myPlacesCache = MyPlacesReposidory.GetCache(userId);
                myPlacesCache.EnsureLoaded();
                UIHelper.BeginInvoke(() =>
                {
                    chkPlaces.ItemsSource = myPlacesCache.Items.Values;
                }, Dispatcher);
            });
        }


        public bool CanGenerateReport
        {
            get { return items.Where(x => x.IsChecked).Count() > 0; }
        }

        public event EventHandler CanGenerateReportChanged;

        protected virtual void OnCanGenerateReportChanged()
        {
            if (CanGenerateReportChanged != null)
            {
                CanGenerateReportChanged(this, EventArgs.Empty);
            }
        }

        public List<ExerciseDTO> GetSelectedExercises()
        {
            List<ExerciseDTO> list = new List<ExerciseDTO>();
            foreach (CheckListItem<ExerciseDTO> item in items)
            {
                if (item.IsChecked)
                {
                    list.Add(item.Value);
                }
            }
            return list;
        }

        public ReportWeightRepetitionsParams GetReportParameters()
        {
            var param = new ReportWeightRepetitionsParams();
            UIHelper.Invoke(()=>
            {
                var exercises = GetSelectedExercises();
                param.StartDate = this.usrDateRange1.DateFrom;
                param.EndDate = this.usrDateRange1.DateTo;
                param.Exercises.AddRange(exercises.Select(x => x.GlobalId));
                param.UseAllEntries = usrReportingEntryStatus1.UseAllEntries;
                param.SuperSlow = chkSuperSlow.IsChecked;
                param.RestPause = chkRestPause.IsChecked;
                param.SetTypes.AddRange(getSelectedSetType());
                param.MyPlaces.AddRange(getSelectedPlaces());
                param.DoneWays.AddRange(getSelectedDoneWays());
            },Dispatcher);
            
            return param;
        }

        IEnumerable<SetType> getSelectedSetType()
        {
            ObservableCollection<object> items = (ObservableCollection<object>)cmbSetTypes.SelectedItems;
            return items.Cast<ListItem<SetType>>().Select(x => x.Value);
        }

        IEnumerable<Guid> getSelectedPlaces()
        {
            ObservableCollection<object> items = (ObservableCollection<object>)chkPlaces.SelectedItems;
            return items.Cast<MyPlaceLightDTO>().Select(x => x.GlobalId);
        }

        private IEnumerable<ExerciseDoneWay> getSelectedDoneWays()
        {
            ObservableCollection<object> items = (ObservableCollection<object>)chkDoneWay.SelectedItems;
            return items.Cast<ListItem<ExerciseDoneWay>>().Select(x => x.Value);
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            UIHelper.BeginInvoke(updateGuiToLicence,Dispatcher);
            return true;
        }
    }
}
