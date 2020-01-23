using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;
using BodyArchitectCustom;

namespace BodyArchitect.WP7.ViewModel
{
    public class StrengthTrainingItemViewModel:ViewModelBase
    {
        private StrengthTrainingItemDTO item;
        private ObservableCollection<SetViewModel> sets;
        private SetViewModel _selectedSet;

        public StrengthTrainingItemViewModel(StrengthTrainingItemDTO item)
        {
            this.item = item;

            sets = new ObservableCollection<SetViewModel>();
            sets.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(sets_CollectionChanged);
            if (item.Series == null)
            {
                item.Series = new List<SerieDTO>();
            }
            foreach (var set in item.Series)
            {
                if (IsCardio)
                {
                    Sets.Add(new CardioSessionViewModel(set));
                }
                else
                {
                    Sets.Add(new SetViewModel(set));    
                }
                
            }
            NotifyPropertyChanged("IsRecord");
        }

        public void RemoveSuperSet()
        {
            Item.SuperSetGroup = null;
            NotifyPropertyChanged("Item");
        }

        void sets_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("SetsCount");
            
        }


        public string WeightType
        {
            get
            {
                if (ApplicationState.Current.ProfileInfo.Settings.WeightType == Service.V2.Model.WeightType.Kg)
                {
                    return ApplicationStrings.Kg;
                }
                else
                {
                    return ApplicationStrings.Pound;
                }
            }
        }

        public StrengthTrainingItemDTO Item
        {
            get { return item; }
        }

        public int Position
        {
            get { return Item.StrengthTrainingEntry.Entries.IndexOf(Item )+ 1; }
        }

        public string DisplayExercise
        {
            get { return Exercise.DisplayExercise; }
        }

        public SetViewModel SelectedSet 
        {
            get { return _selectedSet; } 
            set
            {
                if (_selectedSet != value)
                {
                    _selectedSet = value;
                    NotifyPropertyChanged("SelectedSet");
                }
            }
        }

        //public string SetsString
        //{
        //    get
        //    {
                
        //    }
        //}

        public ExerciseLightDTO Exercise
        {
            get { return Item.Exercise; }
        }

        public string ExerciseType
        {
            get { return EnumLocalizer.Default.Translate(Exercise.ExerciseType); }
        }

        public bool IsRecord
        {
            get
            {
                return Sets.Where(x=>x.IsRecord).Count()>0;
            }
        }

        public string SetsCount
        {
            get { return string.Format(ApplicationStrings.StrengthTrainingItemViewModel_SetsCount,Sets.Count); }
        }

        public ObservableCollection<SetViewModel> Sets
        {
            get { return sets; }
        }

        public void Refresh()
        {
            NotifyPropertyChanged("Position");
            foreach (var setViewModel in Sets)
            {
                setViewModel.Refresh();
            }
        }

        public bool IsCardio
        {
            get { return Exercise != null ? Exercise.ExerciseType == Service.V2.Model.ExerciseType.Cardio : false; }
        }

        SerieDTO addNewSet(StrengthTrainingItemDTO item)
        {
            SerieDTO newSet = new SerieDTO();
            if (Settings.CopyValuesForNewSet && item.Series.Count > 0)
            {//if the settings for copy values is set then when we create next set for the same exercise we should copy reps and weight
                //from the previous set
                var previousSet = item.Series[item.Series.Count - 1];
                newSet.RepetitionNumber = previousSet.RepetitionNumber;
                newSet.Weight = previousSet.Weight;
                //newSet.DropSet = previousSet.DropSet;
                newSet.SetType = previousSet.SetType;
                newSet.IsSuperSlow = previousSet.IsSuperSlow;
            }
            item.AddSerie(newSet);
            return newSet;
        }

        public SetViewModel AddNewSet()
        {

            SerieDTO newSet = addNewSet(item);
            //if (Settings.CopyValuesForNewSet && item.Series.Count>0)
            //{//if the settings for copy values is set then when we create next set for the same exercise we should copy reps and weight
            //    //from the previous set
            //    newSet.RepetitionNumber = item.Series[item.Series.Count - 1].RepetitionNumber;
            //    newSet.Weight = item.Series[item.Series.Count - 1].Weight;
            //}
            //item.AddSerie(newSet);

            if (Settings.TreatSuperSetsAsOne)
            {
                var exercisesInSuperSet = item.GetJoinedItems();
                foreach (var strengthTrainingItemDto in exercisesInSuperSet)
                {

                    addNewSet(strengthTrainingItemDto);
                    //if (Settings.CopyValuesForNewSet && strengthTrainingItemDto.Series.Count > 0)
                    //{//if the settings for copy values is set then when we create next set for the same exercise we should copy reps and weight
                    //    //from the previous set
                    //    joinedSet.RepetitionNumber = strengthTrainingItemDto.Series[item.Series.Count - 1].RepetitionNumber;
                    //    joinedSet.Weight = strengthTrainingItemDto.Series[item.Series.Count - 1].Weight;
                    //}
                    //strengthTrainingItemDto.AddSerie(joinedSet);
                }
            }

            SetViewModel viewModel;
            if (IsCardio)
            {
                viewModel=new CardioSessionViewModel(newSet);
            }
            else
            {
                viewModel=new SetViewModel(newSet);
            }
            
            Sets.Add(viewModel);
            return viewModel;
        }

        public void Delete()
        {
            //entryModel.Delete(this);
            var entry = item.StrengthTrainingEntry;
            
            item.StrengthTrainingEntry.Entries.Remove(item);
            item.StrengthTrainingEntry = null;

            
            StrengthWorkoutViewModel.ResetPositions(entry);
        }

        //public void Delete(SetViewModel set)
        //{
        //    //entryModel.Delete(this);
        //    var index = set.Set.StrengthTrainingItem.Series.IndexOf(set.Set);
        //    set.Set.StrengthTrainingItem.Series.Remove(set.Set);
        //    set.Set.StrengthTrainingItem = null;
        //    sets.Remove(set);

        //    var exercisesInSuperSet = item.GetJoinedItems();
        //    foreach (var strengthTrainingItemDto in exercisesInSuperSet)
        //    {
        //        if (strengthTrainingItemDto.Series.Count > index)
        //        {
        //            var joinedSet = strengthTrainingItemDto.Series[index];
        //            strengthTrainingItemDto.Series.Remove(joinedSet);
        //            joinedSet.StrengthTrainingItem = null;
        //        }

        //    }

            

        //}

        public void JoinExercise(Guid globalId)
        {
            if(string.IsNullOrEmpty(this.item.SuperSetGroup))
            {
                this.item.SuperSetGroup = Guid.NewGuid().ToString();
            }
            var exercise = ApplicationState.Current.Cache.Exercises.GetItem(globalId);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.SuperSetGroup = this.item.SuperSetGroup;
            item.Exercise = exercise;
            this.item.StrengthTrainingEntry.Entries.Add(item);
            item.Position = this.item.StrengthTrainingEntry.Entries.Count;
            item.StrengthTrainingEntry = this.item.StrengthTrainingEntry;
        }
    }
}
