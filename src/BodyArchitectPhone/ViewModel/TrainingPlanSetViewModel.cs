using System;
using System.Net;
using System.Windows;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;

namespace BodyArchitect.WP7.ViewModel
{
    public class TrainingPlanSetViewModel:ViewModelBase
    {
        private TrainingPlanSerie set;
        private TrainingPlanEntry entry;

        public TrainingPlanSetViewModel(TrainingPlanSerie set, TrainingPlanEntry entry)
        {
            this.set = set;
            this.entry = entry;
        }

        public bool IsSuperSlow
        {
            get { return set.IsSuperSlow; }
        }

        public bool IsRestPause
        {
            get { return set.IsRestPause; }
        }

        public string Repetitions
        {
            get
            {
                if (set.RepetitionNumberMax.HasValue || set.RepetitionNumberMin.HasValue)
                {
                    return set.ToStringRepetitionsRange();
                }
                //TODO:Translation
                return "(Not set);";
            }
        }

        public string DropSet
        {
            get
            {
                if(set.DropSet!=DropSetType.None)
                {
                    return string.Format(ApplicationStrings.TrainingPlanSetViewModel_DropSet,(int)set.DropSet);
                }
                return string.Empty;
            }
        }

        public string RepetitionsType
        {
            get
            {
                if (set.RepetitionsType != SetType.Normalna)
                {
                    return EnumLocalizer.Default.Translate(set.RepetitionsType);
                }
                return string.Empty;
            }
        }
        public string Comment
        {
            get { return set.Comment; }
        }

        public Visibility CommentVisibility
        {
            get { return string.IsNullOrEmpty(Comment) ? Visibility.Collapsed : Visibility.Visible; }
        }

        public string SetIndex
        {
            get { return (entry.Sets.IndexOf(set) + 1).ToString(); }
        }
    }
}
