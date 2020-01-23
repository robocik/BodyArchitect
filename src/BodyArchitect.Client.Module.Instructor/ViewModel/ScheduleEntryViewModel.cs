using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.Instructor.Controls;
using BodyArchitect.Client.UI.Controls.Calendar.Common;
using BodyArchitect.Client.UI.Views.MyPlace;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Shared;

namespace BodyArchitect.Client.Module.Instructor.ViewModel
{
    public class ScheduleEntryViewModel : ScheduleEntryBaseViewModel
    {
        public ScheduleEntryViewModel(ScheduleEntryBaseDTO entry, ScheduleEntriesCalendar calendar) : base(entry, calendar)
        {
        }

        public ScheduleEntryDTO Entry
        {
            get { return (ScheduleEntryDTO) Item; }
        }

        public override string Persons
        {
            get
            {
                string format = "{0}/{1}";
                return string.Format(format, Entry.Reservations.Count(), Entry.MaxPersons);
            }
        }

        public override string ErrorToolTip
        {
            get
            {
                if (Entry.ActivityId == null)
                {
                    return "ErrorToolTip_ScheduleEntryViewModel".TranslateInstructor();
                }
                return null;
            }
        }

        public override bool HasError
        {
            get { return !Entry.ActivityId.HasValue; }
        }

        public override Brush Background
        {
            get
            {
                Color color = Colors.Transparent;
                if (calendar.ColorMode == ScheduleColorMode.Activities)
                {
                    if (Entry.ActivityId.HasValue)
                    {
                        var activity = ActivitiesReposidory.Instance.GetItem(Entry.ActivityId.Value);
                        color = activity.Color.FromColorString().ToMediaColor();
                    }
                }
                else if (calendar.ColorMode == ScheduleColorMode.CustomerGroups)
                {
                    if (Entry.CustomerGroupId.HasValue)
                    {
                        var group = CustomerGroupsReposidory.Instance.GetItem(Entry.CustomerGroupId.Value);
                        color = group.Color.FromColorString().ToMediaColor();
                    }
                }
                return GetBackgroundBrush(color);
            }
        }

        public override string Subject
        {
            get
            {
                string subject = "ScheduleEntryViewModel_Subject_Choose".TranslateInstructor();
                if (Entry.ActivityId.HasValue)
                {
                    var activity = ActivitiesReposidory.Instance.GetItem(Entry.ActivityId.Value);
                    subject = activity.Name;
                }
                if (calendar.ColorMode == ScheduleColorMode.CustomerGroups)
                {
                    if (Entry.CustomerGroupId.HasValue)
                    {
                        var group = CustomerGroupsReposidory.Instance.GetItem(Entry.CustomerGroupId.Value);
                        subject = group.Name;
                    }
                }
                return subject;
            }
        }
    }


    public class ScheduleChampionshipViewModel: ScheduleEntryBaseViewModel
    {
        public ScheduleChampionshipViewModel(ScheduleEntryBaseDTO entry, ScheduleEntriesCalendar calendar)
            : base(entry, calendar)
        {
        }

        public ScheduleChampionshipDTO Entry
        {
            get { return (ScheduleChampionshipDTO)Item; }
        }

        public override string Subject
        {
            get { return Entry.Name; }
        }

        public override bool HasError
        {
            get
            {
                return Entry.Categories.Count==0;
            }
        }

        public override string ErrorToolTip
        {
            get { return "ScheduleChampionshipViewModel_ErrChampionshipWithoutCategory".TranslateInstructor(); }
        }

        public override Brush Background
        {
            get
            {
                Color color = Colors.Transparent;
                if (calendar.ColorMode == ScheduleColorMode.Activities)
                {
                    color = Colors.Thistle;
                }
                return GetBackgroundBrush(color);
            }
        }
    }
    public class ScheduleEntryBaseViewModel :ViewModelBase, IAppointment
    {
        private ScheduleEntryBaseDTO entry;
        protected ScheduleEntriesCalendar calendar;
        private bool lastForNextDay;
        private bool isRunning;
        private CancellationTokenSource cancellationToken;
        private MyPlacesReposidory myPlacesCache;

        public ScheduleEntryBaseViewModel(ScheduleEntryBaseDTO entry, ScheduleEntriesCalendar calendar)
        {
            this.entry = entry;
            this.calendar = calendar;
            myPlacesCache = MyPlacesReposidory.GetCache(null);
        }

        public virtual bool HasError
        {
            get { return false; }
        }

        public bool HasReminder
        {
            get { return entry.RemindBefore != null; }
        }

        public virtual string Persons
        {
            get
            {
                string format = "{0}";
                return string.Format(format, entry.Reservations.Count());
            }
        }

        public virtual string ErrorToolTip
        {
            get
            {
                return null;
            }
        }

        protected Brush GetBackgroundBrush(Color color)
        {
            if (calendar.ColorMode == ScheduleColorMode.MyPlaces)
            {
                if (entry.MyPlaceId.HasValue)
                {
                    var group = myPlacesCache.GetItem(entry.MyPlaceId.Value);
                    color = group.Color.FromColorString().ToMediaColor();
                }
            }
            var brush = new SolidColorBrush(color);

            brush.Opacity = entry.IsLocked ? 0.4 : 1;
            return brush;
        }

        public virtual Brush Background
        {
            get { return GetBackgroundBrush(Colors.Transparent); }
        }

        public virtual string Subject
        {
            get
            {
                string subject = "ScheduleEntryViewModel_Subject_Choose".TranslateInstructor();
                return subject;
            }
        }

       

        public ScheduleEntryBaseDTO Item
        {
            get { return entry; }
            set
            {
                entry = value;
                NotifyOfPropertyChange(()=>Item);
                NotifyOfPropertyChange(() => Persons);
                NotifyOfPropertyChange(() => Subject);
                NotifyOfPropertyChange(() => Background);
                NotifyOfPropertyChange(() => HasError);
                NotifyOfPropertyChange(() => ErrorToolTip);
                NotifyOfPropertyChange(() => HasReminder);
            }
        }

        public DateTime StartTime
        {
            get { return entry.StartTime.ToLocalTime(); }
        }

        public DateTime EndTime
        {
            get
            {
                var localStart= entry.StartTime.ToLocalTime();
                var localEnd= entry.EndTime.ToLocalTime();
                LastForNextDay = false;
                if(localEnd.Date>localStart.Date)
                {
                    if (localEnd.Date < localEnd)
                    {//gdy zajecia trwają od 23:00 do 00:00 to w teori konczą sie następnego dnia ale w praktyce to całosc jest zamknieta w jednym dniu takze nie chcemy wyswietlac 
                        //ikony
                        LastForNextDay = true;    
                    }
                    localEnd = localStart.Date.AddDays(1).AddSeconds(-1);
                    
                }
                return localEnd;
            }
        }

        public bool ReadOnly
        {
            get { return entry.IsLocked; }
        }

        /// <summary>
        /// Difference between EndTime and RealEndTime: EndTime is used in calendar control and when the endtime is for the next day then we shrink. But in the tooltip we
        /// wants to show a real time (not shrinked).
        /// </summary>
        public DateTime RealEndTime
        {
            get { return entry.EndTime.ToLocalTime(); }
        }

        public bool LastForNextDay
        {
            get { return lastForNextDay; }
            set
            {
                lastForNextDay = value;
                NotifyOfPropertyChange(()=>LastForNextDay);
            }
        }

        public bool IsRunning
        {
            get { return isRunning; }
            set
            {
                isRunning = value;
                NotifyOfPropertyChange(()=>IsRunning);
            }
        }

        public CancellationTokenSource CancellationToken
        {
            get { return cancellationToken; }
            set { cancellationToken = value; }
        }
    }
}
