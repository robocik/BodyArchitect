using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Portable;
using BodyArchitect.Service.Client.WP7.ModelExtensions;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Service.Client.WP7
{
    public class TrainingDaysHolder
    {
        Dictionary<DateTime, TrainingDayInfo> days = new Dictionary<DateTime, TrainingDayInfo>();
        private List<DateTime> retrievedMonths = new List<DateTime>();


        public TrainingDaysHolder()
        {
        }

        public TrainingDaysHolder(UserDTO SelectedUser)
        {
            this.User = SelectedUser;
        }

        public TrainingDaysHolder(Guid? customerId)
        {
            this.CustomerId = customerId;
            User = ApplicationState.Current.SessionData.Profile;
        }

        public UserDTO User { get; set; }

        public Guid? CustomerId { get; set; }

        public bool IsMine
        {
            get { return User.GlobalId == ApplicationState.Current.SessionData.Profile.GlobalId; }
            private set { }
        }
        public Dictionary<DateTime, TrainingDayInfo> TrainingDays
        {
            get { return days; }
            set { days = value; }
        }

        

        

        public TrainingDayDTO GetFirstLoadedEntry()
        {
            var newDay = (from day in TrainingDays.Values
                          orderby day.TrainingDay.TrainingDate ascending
                          select day).FirstOrDefault();
            if (newDay != null)
            {
                return newDay.TrainingDay;
            }
            return null;
        }

        public TrainingDayDTO GetLastLoadedEntry()
        {
            var newDay = (from day in TrainingDays.Values
                          orderby day.TrainingDay.TrainingDate descending
                          select day).FirstOrDefault();
            if (newDay!=null)
            {
                return newDay.TrainingDay;    
            }
            return null;
        }

        public TrainingDayInfo SetTrainingDay(TrainingDayDTO day)
        {
            TrainingDayInfo info;
            if (TrainingDays.ContainsKey(day.TrainingDate))
            {
                info = TrainingDays[day.TrainingDate];
                info.TrainingDay = day;
            }
            else
            {
                info = new TrainingDayInfo(day);
                TrainingDays.Add(day.TrainingDate, info);
            }
            return info;
        }

        public List<DateTime> RetrievedMonths
        {
            get { return retrievedMonths; }
            set { retrievedMonths = value; }
        }

        public bool IsMonthLoaded(DateTime month)
        {
            return RetrievedMonths.Contains(month);
        }

        public IList<TrainingDayInfo> GetLocalModifiedEntries()
        {
            var res = from d in TrainingDays where d.Value.IsModified select d.Value;
            return res.ToList();
        }

        //checks if this entry is already saved (to the server or local only)
        public bool IsSaved(EntryObjectDTO entry)
        {
            bool isAlreadySavedInLocal=false;
            if (entry.IsNew)
            {
                isAlreadySavedInLocal = ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays.ContainsKey(entry.TrainingDay.TrainingDate);
                if (isAlreadySavedInLocal)
                {
                    TrainingDayInfo dayInfo = ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays[entry.TrainingDay.TrainingDate];
                    var entrySavedInLocal = dayInfo.TrainingDay.Objects.Where(x => x.InstanceId == entry.InstanceId).SingleOrDefault();
                    isAlreadySavedInLocal = entrySavedInLocal != null;
                }
            }
            bool emptyEntry = entry.IsNew && !isAlreadySavedInLocal;
            return !emptyEntry;
        }
    }
}
