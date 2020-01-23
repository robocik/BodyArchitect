using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;
using BodyArchitectCustom;

namespace BodyArchitect.WP7.ViewModel
{
    public class StrengthWorkoutViewModel:ViewModelBase
    {
        private StrengthTrainingEntryDTO entry;
        private ObservableCollection<StrengthTrainingItemViewModel> _exercises;
        private ObservableCollection<StrengthTrainingItemViewModel> _oldExercises=new ObservableCollection<StrengthTrainingItemViewModel>();

        public StrengthWorkoutViewModel(StrengthTrainingEntryDTO entry)
        {
            this.entry = entry;
            _exercises = new ObservableCollection<StrengthTrainingItemViewModel>();

            if (entry.Entries==null)
            {
                entry.Entries=new List<StrengthTrainingItemDTO>();
            }
            foreach (var item in entry.Entries)
            {
                _exercises.Add(new StrengthTrainingItemViewModel(item));   
            }
        }


        public ObservableCollection<StrengthTrainingItemViewModel> Exercises
        {
            get { return _exercises; }
        }

        public bool EditMode
        {
            get { return Entry.TrainingDay.IsMine && Entry.Status!=EntryObjectStatus.System; }
        }

        

        public string TrainingDate
        {
            get { return Entry.TrainingDay.TrainingDate.ToLongDateString(); }
        }

        public StrengthTrainingItemViewModel SelectedItem { get; set; }

        public StrengthTrainingEntryDTO Entry
        {
            get { return entry; }
        }

        public ObservableCollection<StrengthTrainingItemViewModel> OldExercises
        {
            get { return _oldExercises; }
        }

        public StrengthTrainingItemViewModel AddExercise(ExerciseLightDTO exercise)
        {
            StrengthTrainingItemDTO item= new StrengthTrainingItemDTO();
            item.Exercise = exercise;
            Entry.Entries.Add(item);
            item.Position = Entry.Entries.Count;
            item.StrengthTrainingEntry = Entry;
            StrengthTrainingItemViewModel itemViewModel = new StrengthTrainingItemViewModel(item);
            _exercises.Add(itemViewModel);
            return itemViewModel;
        }


        public void Delete(StrengthTrainingItemViewModel strengthTrainingItemViewModel)
        {
            Entry.Entries.Remove(strengthTrainingItemViewModel.Item);
            _exercises.Remove(strengthTrainingItemViewModel);
            //now reset Position
            ResetPositions(Entry);
        }

        public static void ResetPositions(StrengthTrainingEntryDTO entry)
        {
            for (int index = 0; index < entry.Entries.Count; index++)
            {
                var itemDto = entry.Entries[index];
                itemDto.Position = index;
            }
        }

        public void RemoveSuperSet(StrengthTrainingItemViewModel item)
        {
            string supersetGroup = item.Item.SuperSetGroup;
            item.RemoveSuperSet();
            List<StrengthTrainingItemViewModel> itemsInGroup = new List<StrengthTrainingItemViewModel>();
            foreach (var itemViewModel in _exercises)
            {
                if(itemViewModel.Item.SuperSetGroup==supersetGroup)
                {
                    itemsInGroup.Add(itemViewModel);
                }
            }
            if (itemsInGroup.Count==1)
            {//there is only one exercise in the group so we can remove the group
                itemsInGroup[0].RemoveSuperSet();
            }
        }

        public void ShowOldTraining(EntryObjectDTO entry)
        {
            OldExercises.Clear();
            //if(ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays.ContainsKey(oldDate))
            //{
            //    var oldDay = ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays[oldDate];
            //    if (oldDay.TrainingDay.StrengthWorkout != null)
            //    {
            //        foreach (var item in oldDay.TrainingDay.StrengthWorkout.Entries)
            //        {
            //            _oldExercises.Add(new StrengthTrainingItemViewModel(item));
            //        }
            //    }
            //}
            var strength = (StrengthTrainingEntryDTO) entry;
            if (strength != null)
            {
                foreach (var item in strength.Entries)
                {
                    _oldExercises.Add(new StrengthTrainingItemViewModel(item));
                }
            }
        }

        public void Refresh()
        {
            foreach (var strengthTrainingItemViewModel in Exercises)
            {
                strengthTrainingItemViewModel.Refresh();
            }
        }
    }
}
