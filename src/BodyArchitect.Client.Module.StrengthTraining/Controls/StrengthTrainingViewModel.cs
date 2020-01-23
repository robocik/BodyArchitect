using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.StrengthTraining.Localization;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls
{
    public class StrengthTrainingViewModel : ViewModelBase
    {
        private ObservableCollection<StrengthTrainingItemViewModel> items = new ObservableCollection<StrengthTrainingItemViewModel>();
        private StrengthTrainingEntryDTO entry;
        private bool isReadOnly;
        private TrainingPlanDay _trainingPlanDay;
        private bool _showPlanItems;
        private bool canStartTimer;
        private int _setNumber;
        private bool showRestColumns=true;

        public bool ShowRestColumns
        {
            get { return showRestColumns; }
            set
            {
                showRestColumns = value;
                NotifyOfPropertyChange(()=>ShowRestColumns);
            }
        }

        public ObservableCollection<StrengthTrainingItemViewModel> Items
        {
            get { return items; }
        }


        public StrengthTrainingViewModel(int setNumberFromSettings,bool isReadOnly)
        {
            this.IsReadOnly = isReadOnly;
            

            for (int i = 1; i < 16; i++)
            {
                SetNumbers.Add(new ListItem<int>(i.ToString(), i - 1));
            }

            //SelectedSet = 5;
            gridGroupModes.Add(new ListItem<GridGroupMode>(StrengthTrainingEntryStrings.usrStrengthTraining_GroupByNone, GridGroupMode.None));
            gridGroupModes.Add(new ListItem<GridGroupMode>(StrengthTrainingEntryStrings.usrStrengthTraining_GroupByExerciseType, GridGroupMode.ByExerciseType));
            gridGroupModes.Add(new ListItem<GridGroupMode>(StrengthTrainingEntryStrings.usrStrengthTraining_GroupBySupersets, GridGroupMode.BySuperSets));
            SetNumber = setNumberFromSettings;
            items.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(items_CollectionChanged);
            
        }

        public void Fill(StrengthTrainingEntryDTO entry)
        {
            this.entry = entry;
            RebuildViewModel();
        }

        private void RebuildViewModel()
        {
            SetNumber = Math.Max(this.Entry.GetMaximumSeriesCount(), _setNumber);
            items.Clear();
            //_setNumber = Math.Max(this.entry.GetMaximumSeriesCount(), _setNumber);
            foreach (var itemDto in Entry.Entries.OrderBy(x=>x.Position).Select(x => new StrengthTrainingItemViewModel(x)))
            {
                items.Add(itemDto);
            }
            EnsureNewRowAdded();
        }

        void items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.Action==NotifyCollectionChangedAction.Move)
            {
                return;
            }
            if (e.NewItems != null)
            {
                foreach (StrengthTrainingItemViewModel newItem in e.NewItems)
                {
                    newItem.Parent = this;
                }
            }
            if (e.OldItems != null)
            {
                foreach (StrengthTrainingItemViewModel newItem in e.OldItems)
                {
                    Entry.RemoveEntry(newItem.Item);
                }
            }
        }

        #region Ribbon

        private bool canDeleteEntry;
        private bool canMoveUp;
        private bool canMoveDown;
        private bool canShowPlan;
        private bool canJoinSets;
        private bool canSplitSets;
        private bool showSetNumbers;
        private bool showDeleteEntry;
        private bool showMoveUp;
        private bool showMoveDown;
        private bool showSuperSet;
        //private int selectedSetIndex;
        private bool isPlanShowed;
        private GridGroupMode selectedGroup;
        ObservableCollection<Common.ListItem<int>> setNumbers = new ObservableCollection<ListItem<int>>();
        ObservableCollection<Common.ListItem<GridGroupMode>> gridGroupModes = new ObservableCollection<ListItem<GridGroupMode>>();
        private bool _showExerciseTypeColumn;
        private bool _isTimerVisible;

        public ObservableCollection<Common.ListItem<GridGroupMode>> GridGroupModes
        {
            get { return gridGroupModes; }
        }

        public ObservableCollection<Common.ListItem<int>> SetNumbers
        {
            get { return setNumbers; }
        }

        public GridGroupMode SelectedGroup
        {
            get { return selectedGroup; }
            set
            {

                if (selectedGroup != value)
                {
                    selectedGroup = value;

                }
                NotifyOfPropertyChange(() => SelectedGroup);
            }
        }

        public bool IsPlanShowed
        {
            get { return isPlanShowed; }
            set
            {
                isPlanShowed = value;
                NotifyOfPropertyChange(() => IsPlanShowed);
            }
        }

        //public int SelectedSet
        //{
        //    get { return selectedSetIndex; }
        //    set
        //    {

        //        if(selectedSetIndex!=value)
        //        {
        //            selectedSetIndex = value;
        //            changeSetNumbers();
        //        }
        //        NotifyOfPropertyChange(() => SelectedSet);
        //    }
        //}

        public bool ShowSuperSet
        {
            get { return showSuperSet; }
            set
            {
                showSuperSet = value;
                NotifyOfPropertyChange(() => ShowSuperSet);
            }
        }

        public bool ShowSetNumbers
        {
            get { return showSetNumbers; }
            set
            {
                showSetNumbers = value;
                NotifyOfPropertyChange(() => ShowSetNumbers);
            }
        }

        public bool ShowDeleteEntry
        {
            get { return showDeleteEntry; }
            set
            {
                showDeleteEntry = value;
                NotifyOfPropertyChange(() => ShowDeleteEntry);
            }
        }

        public bool ShowMoveUp
        {
            get { return showMoveUp; }
            set
            {
                showMoveUp = value;
                NotifyOfPropertyChange(() => ShowMoveUp);
            }
        }

        public bool ShowMoveDown
        {
            get { return showMoveDown; }
            set
            {
                showMoveDown = value;
                NotifyOfPropertyChange(() => ShowMoveDown);
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

        public bool CanMoveUp
        {
            get { return canMoveUp; }
            set
            {
                canMoveUp = value;
                NotifyOfPropertyChange(() => CanMoveUp);
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

        public bool CanShowPlan
        {
            get { return canShowPlan; }
            set
            {
                canShowPlan = value;
                NotifyOfPropertyChange(() => CanShowPlan);
            }
        }

        public bool CanJoinSets
        {
            get { return canJoinSets; }
            set
            {
                canJoinSets = value;
                NotifyOfPropertyChange(() => CanJoinSets);
            }
        }

        public bool CanStartTimer
        {
            get { return canStartTimer; }
            set
            {
                canStartTimer = value;
                NotifyOfPropertyChange(()=>CanStartTimer);
            }
        }


        public bool CanSplitSets
        {
            get { return canSplitSets; }
            set
            {
                canSplitSets = value;
                NotifyOfPropertyChange(() => CanSplitSets);
            }
        }
        #endregion

       

        public int SetNumber
        {
            get { return _setNumber; }
            set
            {
                _setNumber = value;
                UpdateReadOnly();
                NotifyOfPropertyChange(() => SetNumber);
            }
        }
        

        public TrainingPlanDay TrainingPlanDay
        {
            get { return _trainingPlanDay; }
            set
            {
                _trainingPlanDay = value;
                UpdateTrainingPlan(true);
                NotifyOfPropertyChange(() => TrainingPlanDay);
            }
        }

        public bool ShowPlanItems
        {
            get {
                return _showPlanItems;
            }
            set {
                _showPlanItems = value;
                UpdateTrainingPlan(false);
            }
        }

        public bool IsReadOnly
        {
            get { return isReadOnly; }
            set
            {
                isReadOnly=value;
                UpdateReadOnly();
                NotifyOfPropertyChange(()=>IsReadOnly);
            }
        }

        public StrengthTrainingEntryDTO Entry
        {
            get { return entry; }
        }

        public bool ShowExerciseTypeColumn
        {
            get { return _showExerciseTypeColumn; }
            set
            {
                _showExerciseTypeColumn = value;
                NotifyOfPropertyChange(() => ShowExerciseTypeColumn);
            }
        }

        public bool IsTimerVisible
        {
            get { return _isTimerVisible; }
            set
            {
                _isTimerVisible = value;
                NotifyOfPropertyChange(() => IsTimerVisible);
            }
        }


        public void UpdateReadOnly()
        {
            foreach (var itemViewModel in Items)
            {
                itemViewModel.UpdateReadOnly();
            }
            NotifyOfPropertyChange(()=>IsReadOnly);
        }

        public void UpdateTrainingPlan(bool rebuild)
        {
            if (TrainingPlanDay==null)
            {
                return;
            }
            if(rebuild)
            {
                RebuildViewModel();
            }
            foreach (var itemViewModel in Items)
            {
                if (!itemViewModel.IsNew && itemViewModel.Item.TrainingPlanItemId.HasValue)
                {
                    var entry = TrainingPlanDay.GetEntry(itemViewModel.Item.TrainingPlanItemId.Value);
                    if (entry != null)
                    {
                        itemViewModel.ProsessTrainingPlan(entry);
                    }
                }
                
            }
        }

        public void LicenceChanged()
        {
            foreach (var item in Items)
            {
                item.LicenceChanged();
            }
        }

        public void EnsureNewRowAdded()
        {
            if(IsReadOnly)
            {//in readonly mode we dont need an empty row
                return;
            }
            foreach (var model in Items)
            {
                if (model.IsNew)
                {
                    return;
                }
            }
            var i = new StrengthTrainingItemDTO();
            //entry.AddEntry(i);
            Items.Add(new StrengthTrainingItemViewModel(i));
            UpdateReadOnly();
        }

        public StrengthTrainingEntryDTO GetStrengthTrainingEntry()
        {
            Entry.Entries.Clear();
            for (int index = 0; index < Items.Count; index++)
            {
                var itemViewModel = Items[index];
                if (!itemViewModel.IsNew)
                {
                    var item = itemViewModel.GetStrengthTrainingItem();
                    item.Position = index;
                    Entry.AddEntry(item);
                }
            }
            return Entry;
        }
    }
}
