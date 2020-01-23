using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.StrengthTraining.Localization;
using BodyArchitect.Client.Module.StrengthTraining.Model;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls
{
    public class SetViewModel : ViewModelBase
    {
        private SerieDTO set;
        private bool _readOnly;
        private StrengthTrainingItemViewModel parent;

        public SetViewModel(StrengthTrainingItemViewModel parent)
        {
            this.parent = parent;
        }

        public SetViewModel(StrengthTrainingItemViewModel parent, SerieDTO set)
            : this(parent)
        {
            this.Set = set;
            ToolTip = createSerieToolTip(Set);
        }

        public bool IsFromPlan
        {
            get { return _isFromPlan; }
            set
            {
                _isFromPlan = value;
                NotifyOfPropertyChange(()=>IsFromPlan);
            }
        }

        private string _toolTip;
        private bool _isFromPlan;

        public string ToolTip
        {
            get { return _toolTip; }
            private set
            {
                _toolTip = value;
                NotifyOfPropertyChange(()=>ToolTip);
            }
        }

        private string createSerieToolTip(SerieDTO serie)
        {
            string tooltip = getDisplayValue(WorkoutPlanOperationHelper.SetDisplayMode.Extended) + Environment.NewLine;
            if (serie.IsCiezarBezSztangi)
            {
                tooltip += StrengthTrainingEntryStrings.SerieCellCiezarBezSztangiToolTip + Environment.NewLine;
            }

            if (serie.SetType == SetType.MuscleFailure)
            {
                tooltip += StrengthTrainingEntryStrings.SerieCellIsLastRepetitionWithHelpToolTip + Environment.NewLine;
            }
            if (serie.StrengthTrainingItem.IsCardio())
            {
                if (serie.RepetitionNumber.HasValue)
                {
                    tooltip += string.Format(StrengthTrainingEntryStrings.SetViewModel_DistanceToolTipFormat, serie.RepetitionNumber.Value.ToDisplayDistance(), UIHelper.DistanceType);
                }
                if (serie.CalculatedValue.HasValue)
                {
                    tooltip += string.Format(StrengthTrainingEntryStrings.SetViewModel_CaloriesToolTipFormat, serie.CalculatedValue.Value);
                }
            }
            if (!string.IsNullOrEmpty(serie.Comment))
            {
                tooltip += Environment.NewLine + serie.Comment;
            }
            
            return tooltip;
        }

        public void RefreshToolTip()
        {
            ToolTip = createSerieToolTip(Set);
        }

        public bool IsReadOnly
        {
            get { return _readOnly; }
            set
            {
                _readOnly = value;
                NotifyOfPropertyChange(() => IsReadOnly);
            }
        }


        public string Comment
        {
            get
            {
                if (Set != null)
                {
                    return Set.Comment;
                }
                return string.Empty;
            }
            set
            {
                EnsureSetExists();
                Set.Comment = value;
                NotifyOfPropertyChange(() => Comment);
            }
        }

        string getDisplayValue(WorkoutPlanOperationHelper.SetDisplayMode extended)
        {
            if (Set != null)
            {
                return Set.GetDisplayText(extended);
            }
            return string.Empty;
        }
        public string DisplayValue
        {
            get
            {
                return getDisplayValue(StrengthTraining.Default.ShowExtendedInfoInSets?WorkoutPlanOperationHelper.SetDisplayMode.Extended:WorkoutPlanOperationHelper.SetDisplayMode.Short);
            }
        }

        public string SetTime
        {
            get
            {
                if(Set==null || Set.StartTime==null || Set.EndTime==null)
                {
                    return "";
                }
                return string.Format("StrengthTrainingDataGrid_ContainsTimeData_ToolTip".TranslateStrength(),Set.EndTime - Set.StartTime);
            }
        }

        public bool ContainsTimeData
        {
            get
            {
                if(Set!=null)
                {
                    return Set.StartTime.HasValue && Set.EndTime.HasValue;
                }
                return false;
            }
        }
        public TimeSpan CardioValue
        {
            get
            {
                if (Set != null)
                {
                    decimal seconds = Set.Weight.HasValue ? Set.Weight.Value : 0;
                    var time = TimeSpan.FromSeconds((double) seconds);
                    return time;
                }
                return TimeSpan.Zero;
            }
            set
            {
                if (Set!=null || value != TimeSpan.Zero)
                {
                    EnsureSetExists();
                    Set.Weight = (int) value.TotalSeconds;
                    ToolTip = createSerieToolTip(Set);
                    Parent.UpdateReadOnly();
                    UpdateDisplay();
                }
            }
        }

        public string Value
        {
            get
            {
                if (Set != null)
                {
                    return Set.GetDisplayText(WorkoutPlanOperationHelper.SetDisplayMode.Short);
                }
                return string.Empty;
            }
            set
            {
                EnsureSetExists();
                Set.SetFromString(value);
                ToolTip = createSerieToolTip(Set);
                Parent.UpdateReadOnly();
                UpdateDisplay();
            }
        }

        internal void EnsureSetExists()
        {
            if (Set == null)
            {
                Set = new SerieDTO();
                parent.Item.AddSerie(Set);
            }
        }

        public StrengthTrainingItemViewModel Parent
        {
            get { return parent; }
        }

        public SerieDTO Set
        {
            get { return set; }
            set
            {
                set = value;
                NotifyOfPropertyChange(() => IsReadOnly);
            }
        }

        public bool Exists
        {
            get { return Set != null && !Set.IsEmpty; }
        }

        public void UpdateDisplay()
        {
            NotifyOfPropertyChange(() => Value);
            NotifyOfPropertyChange(() => DisplayValue);
            NotifyOfPropertyChange(() => ToolTip);
            NotifyOfPropertyChange(() => RestTime);
            NotifyOfPropertyChange(() => IsRestPause);
            NotifyOfPropertyChange(() => ContainsTimeData);
        }

        public bool IsRestPause
        {
            get { return Set!=null && Set.IsRestPause; }
        }

        public string RestTime
        {
            get
            {
                if(Set==null)
                {
                    return "";
                }
                var index = Set.StrengthTrainingItem.Series.IndexOf(Set);
                if (index > 0)
                {
                    var previousSet = Set.StrengthTrainingItem.Series[index - 1];
                    if (previousSet.EndTime.HasValue && Set.StartTime.HasValue)
                    {
                        return TimeSpan.FromSeconds((int)(Set.StartTime.Value - previousSet.EndTime.Value).TotalSeconds).ToString();
                    }
                }
                return "";
            }
        }
    }
}
