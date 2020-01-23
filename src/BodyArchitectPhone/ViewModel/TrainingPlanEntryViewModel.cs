using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.Controls.Model;

namespace BodyArchitect.WP7.ViewModel
{
    public class TrainingPlanEntryViewModel:ViewModelBase
    {
        private TrainingPlanEntry entry;
        private IList<TrainingPlanSetViewModel> sets;

        public TrainingPlanEntryViewModel(TrainingPlanEntry entry)
        {
            this.entry = entry;
        }

        public string DisplayExercise
        {
            get
            {
                //ensureExercise();
                return entry.Exercise.DisplayExercise;
            }
            set { }
        }

        public Visibility NoSetsVisibility
        {
            get { return Sets.Count == 0 ? Visibility.Visible : Visibility.Collapsed; }
        }
        public IList<TrainingPlanSetViewModel> Sets
        {
            get
            {
                if(sets==null)
                {
                    sets=new List<TrainingPlanSetViewModel>();
                    foreach (var set in entry.Sets)
                    {
                        sets.Add(new TrainingPlanSetViewModel(set,entry));
                    }
                }
                return sets;
            }
            set { sets = value; }
        }

        public ExerciseLightDTO Exercise
        {
            get
            {
                //ensureExercise();
                return entry.Exercise;
            }
            set { entry.Exercise = value; }
        }
        public TrainingPlanEntry Entry
        {
            get { return entry; }
            set { entry = value; }
        }

        public int Position
        {
            get { return entry.Day.Entries.IndexOf(entry) + 1; }
            set { }
        }

        public string SetsInfo
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                foreach (var set in entry.Sets)
                {
                    builder.AppendFormat("{0}, ", set.GetDisplayText());
                }
                return builder.ToString().TrimEnd(' ',',');
            }
            set { }
        }

        public string Comment
        {
            get
            {
                if (!string.IsNullOrEmpty(entry.Comment))
                {
                    return entry.Comment;
                }
                return ApplicationStrings.TrainingPlanEntryViewModel_Comment_Empty;
            }
            set { }
        }

        public int? RestTime
        {
            get { return entry.RestSeconds; }
            set { }
        }

        public string ExerciseType
        {
            get { return EnumLocalizer.Default.Translate(entry.Exercise.ExerciseType); }
            set { }
        }


        //void ensureExercise()
        //{
        //    if(exercise==null)
        //    {
        //        exercise = ApplicationState.Current.ExercisesReposidory.GetExercise(entry.ExerciseId);
        //    }
        //}
    }
}
