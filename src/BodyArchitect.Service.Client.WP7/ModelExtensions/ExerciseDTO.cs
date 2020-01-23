using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;

namespace BodyArchitect.Service.V2.Model
{
    public interface IRatingable
    {
        Guid GlobalId { get; }
        float Rating { get; set; }
        string Name { get;  }
        float? UserRating { get; set; }
        string UserShortComment { get; set; }
        UserDTO Profile { get; set; }
    }

    public partial class ExerciseDTO : IRatingable
    {
    }

    public partial class ExerciseLightDTO
    {
        public static readonly ExerciseDTO Deleted = new ExerciseDTO(){Name = ApplicationStrings.ExerciseDTO_DeletedExerciseName};

        public string DisplayExercise
        {
            get { return Settings.ExercisesSortBy == ExerciseSortBy.Name ? Name : Shortcut; }
        }

        public bool IsMine
        {
            get
            {
                if (ProfileId == null || ApplicationState.Current.SessionData.Profile.GlobalId != ProfileId)
                {
                    return false;
                }
                return true;
            }

        }
    }
}
