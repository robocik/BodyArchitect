using System;
using System.Net;
using System.Windows;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.Controls.Model;
using BodyArchitect.WP7.UserControls;


namespace BodyArchitect.WP7.ViewModel
{
    public class SetViewModel:ViewModelBase
    {
        public SetViewModel(SerieDTO set)
        {
            Set = set;
        }

        public bool EditMode
        {
            get { return ApplicationState.Current.TrainingDay.TrainingDay.IsMine && Set.StrengthTrainingItem.StrengthTrainingEntry.Status != EntryObjectStatus.System; }
        }

        public decimal? PreviewRepetitionNumber
        {
            get
            {
                var oldSet = Set.Tag as SerieDTO;
                if(oldSet!=null)
                {
                    return oldSet.RepetitionNumber;
                }
                return null;
            }
        }

        public decimal? PreviewWeight
        {
            get
            {
                var oldSet = Set.Tag as SerieDTO;
                if (oldSet != null)
                {
                    return oldSet.Weight;
                }
                return null;
            }
        }

        public bool IsRestPause
        {
            get { return Set.IsRestPause; }
            set
            {
                if (!UpgradeAccountControl.EnsureAccountType(ApplicationStrings.Feature_Premium_AdvancedStrengthTraining, null))
                {
                    NotifyPropertyChanged("IsRestPause");
                    return;
                }
                Set.IsRestPause = value;
                NotifyPropertyChanged("IsRestPause");
            }
        }

        public bool IsSuperSlow
        {
            get { return Set.IsSuperSlow; }
            set
            {
                if (!UpgradeAccountControl.EnsureAccountType(ApplicationStrings.Feature_Premium_AdvancedStrengthTraining, null))
                {
                    NotifyPropertyChanged("IsSuperSlow");
                    return;
                }
                Set.IsSuperSlow = value;
                NotifyPropertyChanged("IsSuperSlow");
            }
        }


        public bool CanSetRestPause
        {
            get { return ApplicationState.Current.IsPremium; }
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

        public void Refresh()
        {
            NotifyPropertyChanged("SetIndexTitle");
            NotifyPropertyChanged("SetIndex");
        }
        public bool IsRecord
        {
            get { return Set.IsRecord; }
        }

        public SerieDTO Set { get; private set; }

        public virtual string SetIndexTitle
        {
            get { return string.Format(ApplicationStrings.SetViewModel_SetIndexTitle,Set.StrengthTrainingItem.Series.IndexOf(Set)+1); }
        }

        

        public string SetIndex
        {
            get { return string.Format("{0} ", SetIndexTitle); }
        }

        public string RestTime
        {
            get
            {
                var index=Set.StrengthTrainingItem.Series.IndexOf(Set);
                if(index>0)
                {
                    var previousSet=Set.StrengthTrainingItem.Series[index - 1];
                    if(previousSet.EndTime.HasValue && Set.StartTime.HasValue)
                    {
                        var seconds = (int) (Set.StartTime.Value - previousSet.EndTime.Value).TotalSeconds;
                        if (seconds>0)
                        {
                            string restPause = Set.IsRestPause ? ApplicationStrings.SetViewModel_RestPauseTime : ApplicationStrings.SetViewModel_RestTime;

                            return string.Format(restPause, TimeSpan.FromSeconds(seconds), restPause);
                        }
                    }
                    return Set.IsRestPause ? string.Format(ApplicationStrings.SetViewModel_RestPauseTime,"") : null;
                }
                return "";
            }
        }
        
        
        public Visibility WeightKgVisible
        {
            get { return Set.Weight.HasValue?Visibility.Visible:Visibility.Collapsed; }
        }

        public string SetTypeInfo
        {
            get
            {
                if (Set.SetType != SetType.Normalna)
                {
                    return  EnumLocalizer.Default.Translate(Set.SetType);
                }
                return string.Empty;
            }
        }

        public bool IsTimerExpanded
        {
            get { return Settings.TimerExpanded; }
            set
            {
                Settings.TimerExpanded = value;
                NotifyPropertyChanged("IsTimerExpanded");
            }
        }
        public string DropSetInfo
        {
            get
            {
                if (Set.DropSet != DropSetType.None)
                {
                    return string.Format(ApplicationStrings.DropSetInfo,(int)Set.DropSet);
                }
                return string.Empty;
            }
        }

        public string SuperSlowInfo
        {
            get
            {
                if (Set.IsSuperSlow)
                {
                    return "SS";
                }
                return string.Empty;
            }
        }


        public void Delete()
        {
            //Set.StrengthTrainingItem.Series.Remove(Set);

            if (Settings.TreatSuperSetsAsOne)
            {
                var index = Set.StrengthTrainingItem.Series.IndexOf(Set);

                var exercisesInSuperSet = Set.StrengthTrainingItem.GetJoinedItems();
                foreach (var strengthTrainingItemDto in exercisesInSuperSet)
                {
                    if (strengthTrainingItemDto.Series.Count > index)
                    {
                        var joinedSet = strengthTrainingItemDto.Series[index];
                        strengthTrainingItemDto.Series.Remove(joinedSet);
                        joinedSet.StrengthTrainingItem = null;
                    }
                }
            }

            Set.StrengthTrainingItem.Series.Remove(Set);
            Set.StrengthTrainingItem = null;

            
        }

        public override string ToString()
        {
            return Set.GetDisplayText();
        }

        public TimeSpan CardioSessionTime
        {
            get
            {
                if (Set.StartTime.HasValue && Set.EndTime.HasValue)
                {
                    return TimeSpan.FromSeconds(Math.Round((Set.EndTime.Value - Set.StartTime.Value).TotalSeconds));
                }
                return TimeSpan.Zero;
            }
            set
            {
                if(Set.StartTime==null)
                {
                    Set.StartTime = DateTime.Now;
                }
                Set.EndTime = Set.StartTime.Value + value;
                Set.Weight = (decimal)(Set.EndTime.Value - Set.StartTime.Value).TotalSeconds;
            }
        }

        public void SetStartTime()
        {
            Set.StartTime = DateTime.Now;

            if (Settings.TreatSuperSetsAsOne)
            {
                var index = Set.StrengthTrainingItem.Series.IndexOf(Set);
                var joinedExercises = Set.StrengthTrainingItem.GetJoinedItems();
                foreach (var strengthTrainingItemDto in joinedExercises)
                {
                    if (strengthTrainingItemDto.Series.Count > index)
                    {
                        strengthTrainingItemDto.Series[index].StartTime = Set.StartTime;
                    }
                }
            }
        }

        protected virtual void TimerCalculation()
        {
            
        }

        public void SetEndTime()
        {
            Set.EndTime = DateTime.Now;

            if (Settings.TreatSuperSetsAsOne)
            {
                var index = Set.StrengthTrainingItem.Series.IndexOf(Set);
                var joinedExercises = Set.StrengthTrainingItem.GetJoinedItems();
                foreach (var strengthTrainingItemDto in joinedExercises)
                {
                    if (strengthTrainingItemDto.Series.Count > index)
                    {
                        strengthTrainingItemDto.Series[index].EndTime = Set.EndTime;
                    }
                }
            }
            TimerCalculation();
            NotifyPropertyChanged("CardioSessionTime");
        }
    }
}
