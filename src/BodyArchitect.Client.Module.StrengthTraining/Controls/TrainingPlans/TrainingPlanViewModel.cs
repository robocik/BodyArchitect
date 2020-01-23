using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.StrengthTraining.Localization;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls.TrainingPlans
{
    public class TrainingPlanViewModel : ViewModelBase, IRepositionableParent
    {
        private TrainingPlan plan = new TrainingPlan();//temporary to prevent null reference exception
        private TrainingPlanTreeItemViewModel selectedItem;
        private bool isModified;
        private TrainingPlan originalPlan;

        #region Ribbon

        private bool canMoveUp;
        private bool canMoveDown;
        private bool canAddDay;
        private bool canEditDay;
        private bool canDeleteDay;
        private bool canAddEntry;
        private bool canDeleteEntry;
        private bool canAddSet;
        private bool canDeleteSet;


        public bool CanMoveUp
        {
            get { return canMoveUp; }
            set
            {
                canMoveUp = value;
                NotifyOfPropertyChange(()=>CanMoveUp);
            }
        }

        public bool CanMoveDown
        {
            get { return canMoveDown; }
            set
            {
                canMoveDown = value;
                NotifyOfPropertyChange(() => CanMoveDown);
            }
        }

        public bool CanAddDay
        {
            get { return canAddDay; }
            set
            {
                canAddDay = value;
                NotifyOfPropertyChange(() => CanAddDay);
            }
        }

        public bool CanEditDay
        {
            get { return canEditDay; }
            set
            {
                canEditDay = value;
                NotifyOfPropertyChange(() => CanEditDay);
            }
        }

        public bool CanDeleteDay
        {
            get { return canDeleteDay; }
            set
            {
                canDeleteDay = value;
                NotifyOfPropertyChange(() => CanDeleteDay);
            }
        }

        public bool CanAddEntry
        {
            get { return canAddEntry; }
            set
            {
                canAddEntry = value;
                NotifyOfPropertyChange(() => CanAddEntry);
            }
        }

        public bool CanDeleteEntry
        {
            get { return canDeleteEntry; }
            set
            {
                canDeleteEntry = value;
                NotifyOfPropertyChange(() => CanDeleteEntry);
            }
        }

        public bool CanAddSet
        {
            get { return canAddSet; }
            set
            {
                canAddSet = value;
                NotifyOfPropertyChange(() => CanAddSet);
            }
        }

        public bool CanDeleteSet
        {
            get { return canDeleteSet; }
            set
            {
                canDeleteSet = value;
                NotifyOfPropertyChange(() => CanDeleteSet);
            }
        }

        #endregion

        public string Header
        {
            get { return "TrainingPlanViewModel_Header_EditPlan".TranslateStrength() + plan.Name; }
        }

        public bool IsModified
        {
            get { return isModified; }
        }

        public TrainingPlanViewModel()
        {
            Days = new ObservableCollection<TrainingPlanDayViewModel>();
        }

        public void Fill(TrainingPlan trainingPlan)
        {
            this.plan = trainingPlan;
            
            originalPlan = plan.StandardClone();
            Days.Clear();
            foreach (var day in plan.Days)
            {
                Days.Add(new TrainingPlanDayViewModel(this, day));
            }
            UpdateToolbar();
            SetModifiedFlag();
            NotifyOfPropertyChange(null);
        }

        public void SetModifiedFlag()
        {
            var val = plan.IsModified(originalPlan);
            isModified = val || plan.IsNew;
            NotifyOfPropertyChange(() => Header);
            NotifyOfPropertyChange(() => IsModified);
        }

        public void UpdateToolbar()
        {
            var selectedDay = SelectedItem as TrainingPlanDayViewModel;
            var selectedEntry = SelectedItem as TrainingPlanEntryViewModel;
            var selectedSet = SelectedItem as TrainingPlanSetViewModel;
            var selectedItem = SelectedItem as TrainingPlanTreeItemViewModel;

            CanAddDay = true;
            CanMoveUp = selectedItem != null && selectedItem.CanMoveUp;
            CanMoveDown = selectedItem != null && selectedItem.CanMoveDown;
            CanAddEntry = selectedDay != null;
            CanDeleteEntry = selectedEntry != null;
            CanEditDay = selectedDay != null;
            CanDeleteDay = selectedDay != null;
            CanDeleteSet = selectedSet != null;
            CanAddSet = selectedEntry != null;
        }

        public ObservableCollection<TrainingPlanDayViewModel> Days { get; private set; }

        public void AddDays(string name)
        {
            var day = new TrainingPlanDay();
            day.Name = name;
            var viewModel = new TrainingPlanDayViewModel(this,day);
            Days.Add(viewModel);
            plan.AddDay(day);
            viewModel.IsSelected = true;
            SetModifiedFlag();
        }

        public void AddEntry(TrainingPlanDayViewModel day)
        {
            TrainingPlanEntry entry = new TrainingPlanEntry();
            var viewModel = new TrainingPlanEntryViewModel(day,entry);
            viewModel.Header = StrengthTrainingEntryStrings.SelectExercise;
            day.Entries.Add(viewModel);
            day.Day.AddEntry(entry);
            day.IsExpanded = true;
            viewModel.IsSelected = true;
            SetModifiedFlag();
        }

        public TrainingPlanTreeItemViewModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                if (selectedItem!=null)
                {
                    selectedItem.IsSelected = true;
                }
                NotifyOfPropertyChange(() => SelectedItem);
            }
        }

        public TrainingPlan Plan
        {
            get { return plan; }
        }

        public void DeleteDay(TrainingPlanDayViewModel dayViewModel)
        {
            Days.Remove(dayViewModel);
            Plan.RemoveDay(dayViewModel.Day);
            SetModifiedFlag();
        }

        public void DeleteEntry(TrainingPlanEntryViewModel entry)
        {
            entry.ParentDay.Entries.Remove(entry);
            entry.ParentDay.Day.RemoveEntry(entry.Entry);
            entry.ParentDay.IsSelected = true;
            SetModifiedFlag();
        }

        public void AddSet(TrainingPlanEntryViewModel entry)
        {
            TrainingPlanSerie set = new TrainingPlanSerie(10);
            var viewModel = new TrainingPlanSetViewModel(entry, set);
            entry.Entry.Sets.Add(set);
            entry.Sets.Add(viewModel);
            entry.IsExpanded = true;
            viewModel.IsSelected = true;
            SetModifiedFlag();
        }

        public void DeleteSet(TrainingPlanSetViewModel set)
        {
            set.ParentEntry.Sets.Remove(set);
            set.ParentEntry.Entry.Sets.Remove(set.Set);
            set.ParentEntry.IsSelected = true;
        }


        #region Implementation of IRepositionableParent

        public void RepositionEntry(int index1, int index2)
        {
            var item = Days[index1];
            Days.Remove(item);
            Days.Insert(index2, item);
            Plan.RepositionEntry(index1, index2);
        }

        #endregion

        
    }

    public abstract class TrainingPlanTreeItemViewModel : TreeItemViewModel
    {

        public abstract BAGlobalObject Item { get; }

        
        public virtual bool CanMoveUp
        {
            get { return false; }
        }

        public virtual bool CanMoveDown
        {
            get { return false; }
        }

        
        private string _header;
        public virtual string Header
        {
            get { return _header; }
            set
            {
                _header = value;
                NotifyOfPropertyChange(()=>Header);
            }
        }

        private string _image;
        public virtual string Image
        {
            get { return _image; }
            set
            {
                _image = value;
                NotifyOfPropertyChange(() => Image);
            }
        }

        public virtual void Update()
        {
        }
    }

    public class TrainingPlanDayViewModel : TrainingPlanTreeItemViewModel, IRepositionableChild, IRepositionableParent
    {
        private TrainingPlanDay day;
        private TrainingPlanViewModel parent;

        public TrainingPlanDayViewModel(TrainingPlanViewModel parent,TrainingPlanDay day)
        {
            this.day = day;
            this.parent = parent;
            Entries = new ObservableCollection<TrainingPlanEntryViewModel>();
            Image ="pack://application:,,,/BodyArchitect.Client.Module.StrengthTraining;component/Images/TrainingDay.gif";
            foreach (var entry in day.Entries)
            {
                Entries.Add(new TrainingPlanEntryViewModel(this,entry));
            }
        }

        public override BAGlobalObject Item
        {
            get { return day; }
        }

 
        public override string Header
        {
            get { return Day.Name; }
            set
            {
                Day.Name = value;
                NotifyOfPropertyChange(()=>Header);
            }
        }

        public ObservableCollection<TrainingPlanEntryViewModel> Entries { get; private set; }

        public TrainingPlanDay Day
        {
            get { return day; }
        }

        public override bool CanMoveUp
        {
            get { return Position > 0; }
        }

        public override bool CanMoveDown
        {
            get { return parent.Days.IndexOf(this)<parent.Days.Count-1; }
        }

        #region Implementation of IRepositionableChild

        public int Position
        {
            get { return Day.Position; }
        }

        public IRepositionableParent RepositionableParent
        {
            get { return parent; }
        }

        #endregion

        #region Implementation of IRepositionableParent

        public void RepositionEntry(int index1, int index2)
        {
            var item = Entries[index1];
            Entries.Remove(item);
            Entries.Insert(index2, item);
            Day.RepositionEntry(index1,index2);
        }

        #endregion
    }

    public class TrainingPlanEntryViewModel : TrainingPlanTreeItemViewModel, IRepositionableChild
    {
        private TrainingPlanEntry entry;
        private TrainingPlanDayViewModel parentDay;

        public TrainingPlanEntryViewModel(TrainingPlanDayViewModel parentDay, TrainingPlanEntry entry)
        {
            this.parentDay = parentDay;
            this.entry = entry;
            Sets = new ObservableCollection<TrainingPlanSetViewModel>();
            Image = "pack://application:,,,/BodyArchitect.Client.Module.StrengthTraining;component/Images/TrainingPlanEntry.png";
            foreach (var set in entry.Sets)
            {
                Sets.Add(new TrainingPlanSetViewModel(this,set));
            }
            Update();
        }

        public override bool CanMoveUp
        {
            get { return Position > 0; }
        }

        public override bool CanMoveDown
        {
            get { return parentDay.Entries.IndexOf(this) < parentDay.Entries.Count - 1; }
        }

        public override BAGlobalObject Item
        {
            get { return entry; }
        }

        public override void Update()
        {
            
            var exercise = Entry.Exercise;

            Header = exercise != null ? exercise.GetLocalizedName() : "TrainingPlanViewModel_Update_SelectExercise".TranslateStrength();
            if (exercise != ExerciseDTO.Removed)
            {
                Image = "pack://application:,,,/BodyArchitect.Client.Module.StrengthTraining;component/Images/TrainingPlanEntry.png";
            }
            else
            {
                Image = "pack://application:,,,/BodyArchitect.Client.Module.StrengthTraining;component/Images/TrainingPlanEntry_Error.png";
            }
        }

        public ObservableCollection<TrainingPlanSetViewModel> Sets { get; private set; }

        public TrainingPlanEntry Entry
        {
            get { return entry; }
        }

        public TrainingPlanDayViewModel ParentDay
        {
            get { return parentDay; }
        }

        #region Implementation of IRepositionableChild

        public int Position
        {
            get { return Entry.Position; }
        }

        public IRepositionableParent RepositionableParent
        {
            get { return ParentDay; }
        }

        #endregion
    }

    public class TrainingPlanSetViewModel : TrainingPlanTreeItemViewModel
    {
        private TrainingPlanSerie set;
        private TrainingPlanEntryViewModel parentEntry;

        public TrainingPlanSetViewModel(TrainingPlanEntryViewModel parentEntry,TrainingPlanSerie set)
        {
            this.parentEntry = parentEntry;
            this.set = set;
            Image = "pack://application:,,,/BodyArchitect.Client.Module.StrengthTraining;component/Images/Set.png";
            Update();
        }

        public override BAGlobalObject Item
        {
            get { return set; }
        }

        public string RepetitionRange
        {
            get { return set.GetDisplayText(false); }
            set
            {
                set.FromString(value);
                Update();
            }
        }


        public TimeSpan CardioValue
        {
            get
            {
                decimal seconds = set.RepetitionNumberMin.HasValue ? set.RepetitionNumberMin.Value : 0;
                var time = TimeSpan.FromSeconds((double)seconds);
                return time;
            }
            set
            {
                set.RepetitionNumberMin = (int)value.TotalSeconds;
                Update();

            }
        }

        public bool IsCardio
        {
            get { return parentEntry.Entry.Exercise!=null && parentEntry.Entry.Exercise.ExerciseType == ExerciseType.Cardio; }
        }

        public string Comment
        {
            get
            {
                if (set.Comment==null)
                {
                    return string.Empty;
                }
                return set.Comment.Replace("\n", "\r\n");
            }
            set { set.Comment = value; }
        }

        public TrainingPlanSerie Set
        {
            get { return set; }
        }

        public TrainingPlanEntryViewModel ParentEntry
        {
            get { return parentEntry; }
        }

        public override void Update()
        {
            Header = set.GetDisplayText(IsCardio);
        }
    }
}
