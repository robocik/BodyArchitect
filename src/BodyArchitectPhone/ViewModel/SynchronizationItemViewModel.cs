using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.Client.WP7.ModelExtensions;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;

namespace BodyArchitect.WP7.ViewModel
{
    public enum MergeAction
    {
        InConflictSkip,
        UseServer,
        UseLocal,
    }

    public enum MergeState
    {
        None,
        Processing,
        Finished,
        Error
    }

    public enum ItemType{TrainingDay,GPSCoordinates}

    public class SynchronizationItemViewModel:ViewModelBase
    {
        private MergeState state;
        private SynchronizationViewModel parent;

        public SynchronizationItemViewModel(TrainingDayInfo dayInfo, SynchronizationViewModel parent)
        {
            this.parent = parent;
            DayInfo = dayInfo;
            ItemType = ItemType.TrainingDay;
        }

        public SynchronizationItemViewModel(TrainingDayInfo dayInfo,GPSTrackerEntryDTO entry,GPSPointsBag bag, SynchronizationViewModel parent)
        {
            this.parent = parent;
            DayInfo = dayInfo;
            ItemType = ItemType.GPSCoordinates;
            GPSBag = bag;
            GPSEntry = entry;

        }

        public GPSTrackerEntryDTO GPSEntry { get; set; }
        public ItemType ItemType { get; private set; }

        public int Index
        {
            get { return parent.Items.IndexOf(this)+1; }
        }

        public GPSPointsBag GPSBag { get;private set; }

        public string Date
        {
            get { return DayInfo.TrainingDay.TrainingDate.ToShortDateString(); }
        }


        public TrainingDayInfo DayInfo { get; set; }

        public string Status
        {
            get
            {
                if(state==MergeState.Processing)
                {
                    return ApplicationStrings.SynchronizationItemViewModel_Processing;
                }
                else if(state==MergeState.Error)
                {
                    return ApplicationStrings.SynchronizationItemViewModel_Error;
                }
                return string.Empty;
            }
        }
        public MergeState State
        {
            get { return state; }
            set
            {
                if(state!=value)
                {
                    state = value;
                    Deployment.Current.Dispatcher.BeginInvoke(delegate
                                                                  {
                                                                      NotifyPropertyChanged("State");
                                                                      NotifyPropertyChanged("Status");
                                                                  });
                    
                }
            }
        }

        string getImage(EntryObjectDTO entry)
        {
            Type type = entry.GetType();
            if (type == typeof(StrengthTrainingEntryDTO))
            {
                return "/Images/strengthTrainingTile.jpg";
            }
            else if (type == typeof(SuplementsEntryDTO))
            {
                return "/Images/suppleTile.jpg";
            }
            if (type == typeof(BlogEntryDTO))
            {
                return "/Images/blogTile.jpg";
            }
            if (type == typeof(GPSTrackerEntryDTO))
            {
                return "/Images/gpsTile.jpg";
            }
            if (type == typeof(SizeEntryDTO))
            {
                return "/Images/sizesTile.jpg";
            }
            return null;
        }

        public IEnumerable<string> Images
        {
            get { return DayInfo.TrainingDay.Objects.Select(x=>getImage(x)); }
        }

        public Visibility IsTrainingDay
        {
            get { return ItemType == ItemType.TrainingDay ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility IsGpsCoordinates
        {
            get { return ItemType == ItemType.GPSCoordinates ? Visibility.Visible : Visibility.Collapsed; }
        }
        
        public Visibility GpsCoordinatesVisiblitity
        {
            get { return ItemType == ItemType.GPSCoordinates ? Visibility.Visible : Visibility.Collapsed; }
        }
    }
}
