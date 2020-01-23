using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.StrengthTraining.Localization;
using BodyArchitect.Client.Module.StrengthTraining.Model;
using BodyArchitect.Client.UI;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls
{
    public class StrengthTrainingItemViewModel : DynamicObject, INotifyPropertyChanged
    {
        private StrengthTrainingItemDTO item;
        private StrengthTrainingViewModel parent;


        public StrengthTrainingItemViewModel(StrengthTrainingItemDTO item)
        {
            this.item = item;
            for (int index = 0; index < item.Series.Count; index++)
            {
                var set = item.Series[index];
                sets.Add(new SetViewModel(this, set));
            }
            //ObjectsReposidory.WorkoutPlans

        }

        internal void ProsessTrainingPlan(TrainingPlanEntry planEntry)
        {
            for (int j = 0; j < planEntry.Sets.Count; j++)
            {
                var setViewModel = GetSetViewModel(j);
                if (setViewModel != null)
                {
                    setViewModel.IsFromPlan =parent.ShowPlanItems && setViewModel.Set != null;
                }
            }
        }

        public void EnsureSets()
        {
            if(parent.SetNumber+1>=sets.Count)
            {
                for (int i = sets.Count; i <= parent.SetNumber ; i++)
                {
                    sets.Add(new SetViewModel(this));
                }
            }
            else
            {
                sets.RemoveRange(parent.SetNumber+1, sets.Count - (parent.SetNumber+1));
            }
            UpdateReadOnly();
        }

        public void UpdateReadOnly()
        {
            bool previousExists = !IsNew;
            foreach (var itemViewModel in sets)
            {
                itemViewModel.IsReadOnly = true;
                if (!itemViewModel.Exists && previousExists)
                {
                    itemViewModel.IsReadOnly = false;
                    previousExists = false;
                }
                else
                {
                    previousExists = itemViewModel.Exists;
                    itemViewModel.IsReadOnly = !previousExists;
                }
            }
            NotifyOfPropertyChange(()=>IsNew);
            NotifyOfPropertyChange(() => CanBeDeleted);
            NotifyOfPropertyChange(() => IsReadOnly);
        }

        public virtual void NotifyOfPropertyChange<TProperty>(Expression<Func<TProperty>> property)
        {
            NotifyOfPropertyChange(property.GetMemberInfo().Name);
        }

        public virtual void NotifyOfPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool IsNew
        {
            get { return item.Exercise.IsEmpty(); }
        }

        public bool CanBeDeleted
        {
            get { return !IsNew && !IsReadOnly; }
        }

        public bool IsReadOnly
        {
            get { return parent!=null?parent.IsReadOnly:true; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IList<ListItem<ExerciseDoneWay>> DoneWays
        {
            get
            {
                List<ListItem<ExerciseDoneWay>> list = new List<ListItem<ExerciseDoneWay>>();
                foreach (ExerciseDoneWay doneWay in Enum.GetValues(typeof(ExerciseDoneWay)))
                {
                    list.Add(new ListItem<ExerciseDoneWay>(EnumLocalizer.Default.Translate(doneWay),doneWay));
                }
                return list;
            }
        }

        public bool HasAdvancedOptions
        {
            get { return !string.IsNullOrEmpty(item.Comment) || DoneWay != ExerciseDoneWay.Default; }
        }

        public bool IsPremium
        {
            get { return !IsReadOnly && UserContext.IsPremium; }
        }

        public ExerciseDoneWay DoneWay
        {
            get { return item.DoneWay; }
            set
            {
                item.DoneWay = value;

                NotifyOfPropertyChange(() => DoneWay);
                NotifyOfPropertyChange(() => HasAdvancedOptions);
            }
        }

        public string Comment
        {
            get { return item.Comment; }
            set
            {
                item.Comment = value;

                NotifyOfPropertyChange(() => Comment);
                NotifyOfPropertyChange(() => HasAdvancedOptions);
            }
        }

        public ExerciseLightDTO Exercise
        {
            get { return item.Exercise; }
            set
            {
                bool updateReadOnly = item.Exercise.IsEmpty();
                item.Exercise = value;
                if (updateReadOnly)
                {
                    EnsureSets();
                }
                UpdateSetsDisplay();
                NotifyOfPropertyChange(() => Exercise);
                NotifyOfPropertyChange(() => ExerciseType);
                NotifyOfPropertyChange(() => IsCardio);
            }
        }

        public void UpdateSetsDisplay()
        {
            foreach (var setViewModel in sets)
            {
                
                setViewModel.UpdateDisplay();
            }
        }

        public bool IsCardio
        {
            get
            {
                if (IsNew)
                {
                    return false;
                }
                return item.IsCardio();
            }
        }

        public string ExerciseType
        {
            get
            {
                if (IsNew)
                {
                    return "StrengthTrainingItemViewModel_NewExercise".TranslateStrength();
                }

                if (item.Exercise != null)
                {
                    return EnumLocalizer.Default.Translate(item.Exercise.ExerciseType);
                }
                return string.Empty;
            }
        }

        public string SuperSetGroup
        {
            get { return item.SuperSetGroup; }
            set
            {
                item.SuperSetGroup = value;
                NotifyOfPropertyChange(() => SuperSetGroup);
                NotifyOfPropertyChange(() => SuperSetName);
            }
        }

        public string SuperSetName
        {
            get
            {
                if (IsNew)
                {
                    return "StrengthTrainingItemViewModel_NewExercise".TranslateStrength();
                }
                if(string.IsNullOrEmpty(item.SuperSetGroup))
                {
                    return "StrengthTrainingItemViewModel_WithoutSupersetGroupName".TranslateStrength();
                }
                return item.SuperSetGroup;
            }
        }

        List<SetViewModel> sets = new List<SetViewModel>();

        public SetViewModel GetSetViewModel(int index)
        {
            return sets[index];
        }

        public StrengthTrainingViewModel Parent
        {
            get { return parent; }
            set
            {
                parent = value;
                EnsureSets();
            }
        }

        public StrengthTrainingItemDTO Item
        {
            get { return item; }
        }

        // If you try to get a value of a property 
        // not defined in the class, this method is called.
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            string name = binder.Name;
            int setNumber = int.Parse(name);
            // If the property name is found in a dictionary,
            // set the result parameter to the property value and return true.
            // Otherwise, return false.
            //if (!dictionary.ContainsKey(name))
            //{
            //    dictionary.Add(name, new SetViewModel(this));
            //}
            result = sets[setNumber];
            return true;
        }

        // If you try to set a value of a property that is
        // not defined in the class, this method is called.
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            int setNumber = int.Parse(binder.Name);
            var iteViewModel = sets[setNumber];
            iteViewModel.Value = (string)value;

            // You can always add a value to a dictionary,
            // so this method always returns true.
            return true;
        }

        public void RemoveSet(int setNumber)
        {
            sets.RemoveAt(setNumber);
            sets.Add(new SetViewModel(this));
            //refresh the grid
            for (int i = 0; i < sets.Count; i++)
            {
                NotifyOfPropertyChange(i.ToString());
            }
            UpdateReadOnly();
        }

        public StrengthTrainingItemDTO GetStrengthTrainingItem()
        {
            item.Series.Clear();
            foreach (var setViewModel in sets)
            {
                if (setViewModel.Set != null && !setViewModel.Set.IsEmpty)
                {
                    item.AddSerie(setViewModel.Set);
                }
            }
            return item;
        }

        public void LicenceChanged()
        {
            NotifyOfPropertyChange(()=>IsPremium);
        }
    }
}
