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
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.WP7.ViewModel
{
    public class VotingViewModel
    {
        private IRatingable exercise;

        public VotingViewModel(IRatingable exercise)
        {
            this.exercise = exercise;
        }

        public string Name
        {
            get { return exercise.Name.ToUpper(); }
        }

        public double Rating
        {
            get
            {
                if (exercise.UserRating.HasValue)
                {
                    return exercise.UserRating.Value/Constants.MaxRatingValue;
                }
                return 0;
            }
            set { exercise.UserRating = (float)(value * Constants.MaxRatingValue); }
        }

        public string Comment
        {
            get { return exercise.UserShortComment; }
            set { exercise.UserShortComment = value; }
        }
    }
}
