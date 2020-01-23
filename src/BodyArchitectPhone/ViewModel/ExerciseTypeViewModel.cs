using System;
using System.Collections.ObjectModel;
using System.Linq;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;
using BodyArchitectCustom;

namespace BodyArchitect.WP7.ViewModel
{
    public class ExerciseTypeViewModel:ViewModelBase
    {
        private ObservableCollection<GroupingLayer<string, ExerciseViewModel>> _exercises;
        private ExerciseViewModel _selectedExercise;
        public event EventHandler ExercisesLoaded;
        private bool selectionMode;

        public bool SelectionMode
        {
            get
            {
                return selectionMode;
            }
            set
            {
                selectionMode = value;
                NotifyPropertyChanged("SelectionMode");
            }
        }
        public ExerciseViewModel SelectedExercise
        {
            get { return _selectedExercise; }
            set
            {
                _selectedExercise = value;
                NotifyPropertyChanged("SelectedExercise");
            }
        }

        

        public ObservableCollection<GroupingLayer<string, ExerciseViewModel>> GroupedExercises
        {
            get
            {
                return _exercises;
            }

            private set
            {
                _exercises = value;
                NotifyPropertyChanged("GroupedExercises");
            }
        }

        /// <summary>
        /// Is used to filter exercise list to specific type. Null means show all exercises
        /// </summary>
        public ExerciseType? ExerciseType { get; set; }

        public ExerciseViewModel GetExerciseView(Guid globalId)
        {
            return GroupedExercises.SelectMany(groupedExercise => groupedExercise).FirstOrDefault(exerciseViewModel => exerciseViewModel.Exercise.GlobalId == globalId);
        }

        public void LoadExercises()
        {
            ApplicationState.Current.Cache.Exercises.Loaded += new EventHandler(ObjectsReposidory_ExercisesLoaded);
            ApplicationState.Current.Cache.Exercises.Load();
        }

        void ObjectsReposidory_ExercisesLoaded(object sender, EventArgs e)
        {
            ApplicationState.Current.Cache.Exercises.Loaded -= new EventHandler(ObjectsReposidory_ExercisesLoaded);
            if (ApplicationState.Current.Cache.Exercises == null || !ApplicationState.Current.Cache.Exercises.IsLoaded)
            {
                onExercisesLoaded();
                BAMessageBox.ShowError(ApplicationStrings.ExerciseTypeViewModel_ErrRetrieveExercises);
                return;
            }

            if (GroupedExercises == null)
            {
                GroupedExercises = new ObservableCollection<GroupingLayer<string, ExerciseViewModel>>();
            }
            else
            {
                GroupedExercises.Clear();
            }


            var res = ApplicationState.Current.Cache.Exercises.Items.Values.Where(x=>ExerciseType==null || x.ExerciseType==ExerciseType.Value).OrderBy(x=>EnumLocalizer.Default.Translate(x.ExerciseType)).ThenBy(x => Settings.ExercisesSortBy == ExerciseSortBy.Name ? x.Name : x.Shortcut).Select(x => new ExerciseViewModel(x) { IsAddMode = SelectionMode }).GroupBy(
                x => EnumLocalizer.Default.Translate(x.Exercise.ExerciseType)).Select(n => new GroupingLayer<string, ExerciseViewModel>(n));
            
            foreach (var exercise in res)
            {
                GroupedExercises.Add(exercise);
            }

            onExercisesLoaded();
            
        }

        private void onExercisesLoaded()
        {
            if(ExercisesLoaded!=null)
            {
                ExercisesLoaded(this,EventArgs.Empty);
            }
        }

        public void Refresh()
        {
            ApplicationState.Current.Cache.Exercises.Loaded += new EventHandler(ObjectsReposidory_ExercisesLoaded);
            ApplicationState.Current.Cache.Exercises.Refresh();
        }
    }
}
