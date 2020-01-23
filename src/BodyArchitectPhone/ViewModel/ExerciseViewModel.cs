using System.Windows;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;

namespace BodyArchitect.WP7.ViewModel
{
    public class ExerciseViewModel:ViewModelBase
    {
        private ExerciseDTO exercise;
        private bool isAddMode;

        public ExerciseViewModel(ExerciseDTO exercise)
        {
            this.exercise = exercise;
        }

        public double Rating
        {
            get { return exercise.Rating / Constants.MaxRatingValue; }
        }

        public Visibility ShowGoToWeb
        {
            get
            {
                return string.IsNullOrEmpty(exercise.Url) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public ExerciseDTO Exercise
        {
            get { return exercise; }
            set
            {
                exercise = value;
                NotifyPropertyChanged("Exercise");
            }
        }

        public string Difficult
        {
            get { return EnumLocalizer.Default.Translate(exercise.Difficult); }
        }

        public string ExerciseType
        {
            get { return EnumLocalizer.Default.Translate(exercise.ExerciseType); }
        }

        public string PublicStatusImage
        {
            get
            {
                if (exercise.Profile == null)
                {
                    return "/Images/Global.png";
                }
                if (!exercise.Profile.IsMe)
                {
                    return "/Images/Favorites.png";
                }
                
                return "/Images/private.png";
            }
            set { }
        }
        
        
        public string MechanicsType
        {
            get { return EnumLocalizer.Default.Translate(exercise.MechanicsType); }
        }
        public string ExerciseForceType
        {
            get { return EnumLocalizer.Default.Translate(exercise.ExerciseForceType); }
        }

        public Visibility DescriptionVisibility
        {
            get { return string.IsNullOrEmpty(Exercise.Description) ? Visibility.Collapsed : Visibility.Visible; }
        }
        public bool IsAddMode
        {
            get { return isAddMode; }
            set 
            {
                isAddMode = value;
                NotifyPropertyChanged("IsAddMode");
            }
        }
    }
}
