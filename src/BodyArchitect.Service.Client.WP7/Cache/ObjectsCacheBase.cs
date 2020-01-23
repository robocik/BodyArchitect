using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using BodyArchitect.Service.V2.Model;
using System.ComponentModel;
using Microsoft.Phone.Scheduler;

namespace BodyArchitect.Service.Client.WP7.Cache
{
    public class FeaturedDataReposidory : SingleObjectCacheBase<FeaturedData, GetFeaturedDataCompletedEventArgs>
    {
        private static FeaturedDataReposidory instance;
        public static FeaturedDataReposidory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FeaturedDataReposidory();
                }
                return instance;
            }
        }

        //protected override Guid GetCurrentHash()
        //{
        //    return 
        //}


        protected override void RetrieveObjects(BodyArchitectAccessServiceClient client1, EventHandler<GetFeaturedDataCompletedEventArgs> operationCompleted)
        {
            var criteria = new GetFeaturedDataParam();
            client1.GetFeaturedDataAsync(ApplicationState.Current.SessionData.Token, criteria);
            client1.GetFeaturedDataCompleted -= operationCompleted;
            client1.GetFeaturedDataCompleted += operationCompleted;
        }
    }

    public class RemindersReposidory : ObjectsCacheBase<ReminderItemDTO, GetRemindersCompletedEventArgs>
    {
        protected override void RetrieveObjects(BodyArchitectAccessServiceClient client1, PartialRetrievingInfo pageInfo, EventHandler<GetRemindersCompletedEventArgs> operationCompleted)
        {
            var criteria = new GetRemindersParam();
            client1.GetRemindersAsync(ApplicationState.Current.SessionData.Token, criteria, pageInfo);
            client1.GetRemindersCompleted -= operationCompleted;
            client1.GetRemindersCompleted += operationCompleted;
        }

        protected override Guid GetCurrentHash()
        {
            return ApplicationState.Current.ProfileInfo != null ? ApplicationState.Current.ProfileInfo.DataInfo.ReminderHash : Guid.Empty;
        }

        public void EnsureReminders()
        {
            if (ApplicationState.Current.ProfileInfo.Licence.CurrentAccountType == AccountType.User)
            {//for free account remove all reminders
                RemoveAllReminders();
                return;
            }
            if (!IsLoaded)
            {
                this.Loaded += RemindersReposidory_Loaded;
                this.Load();
            }
            else
            {
                buildReminders();
            }
        }

        public void RemoveAllReminders()
        {
            var existingReminders = ScheduledActionService.GetActions<Reminder>();
            foreach (var existingReminder in existingReminders)
            {
                ScheduledActionService.Remove(existingReminder.Name);
            }
        }

        void RemindersReposidory_Loaded(object sender, EventArgs e)
        {
            this.Loaded -= RemindersReposidory_Loaded;
            if (IsLoaded)
            {
                buildReminders();
            }
        }

        private RecurrenceInterval fromReminderRepetitions(ReminderRepetitions reminderRepetitions)
        {
            if (reminderRepetitions == ReminderRepetitions.EveryDay)
            {
                return RecurrenceInterval.Daily;
            }
            if (reminderRepetitions == ReminderRepetitions.EveryWeek)
            {
                return RecurrenceInterval.Weekly;
            }
            if (reminderRepetitions == ReminderRepetitions.EveryMonth)
            {
                return RecurrenceInterval.Monthly;
            }
            if (reminderRepetitions == ReminderRepetitions.EveryYear)
            {
                return RecurrenceInterval.Yearly;
            }
            return RecurrenceInterval.None;
        }

        void buildReminders()
        {
            var existingReminders = ScheduledActionService.GetActions<Reminder>();

            //check removed reminders
            foreach (var existingReminder in existingReminders)
            {
                try
                {
                    Guid reminderId = new Guid(existingReminder.Name);
                    var reminderDto = GetItem(reminderId);
                    if (reminderDto == null)
                    {//this reminder has been removed so remove it from the phone
                        ScheduledActionService.Remove(existingReminder.Name);
                    }
                }
                catch (Exception)
                {
                }

            }

            //now check added or updated reminders
            foreach (var item in Items.Values)
            {
                try
                {
                    var phoneReminder = existingReminders.Where(x => x.Name == item.GlobalId.ToString()).SingleOrDefault();
                    var reminder = phoneReminder;
                    if (reminder == null)
                    {
                        reminder = new Reminder(item.GlobalId.ToString());
                    }
                    reminder.Title = item.Name;
                    //reminder.Content = contentTextBox.Text;
                    reminder.BeginTime = item.DateTime.ToLocalTime();
                    if (item.RemindBefore != null)
                    {
                        var newDate = item.DateTime.ToLocalTime() - item.RemindBefore.Value;
                        reminder.BeginTime = newDate;

                    }
                    if (item.Repetitions == ReminderRepetitions.Once)
                    {
                        if (reminder.BeginTime < DateTime.Now)
                        {
                            reminder.BeginTime = DateTime.Now.AddSeconds(5);
                        }
                        if (reminder.BeginTime < DateTime.Now - TimeSpan.FromDays(2))
                        {//old reminder. skip it!
                            continue;
                        }
                        reminder.ExpirationTime = reminder.BeginTime.AddDays(1);
                    }
                    //reminder.ExpirationTime = expirationTime;
                    reminder.RecurrenceType = fromReminderRepetitions(item.Repetitions);
                    //reminder.NavigationUri = navigationUri;
                    // Register the reminder with the system.

                    if (phoneReminder == null)
                    {
                        ScheduledActionService.Add(reminder);
                    }
                    else if (phoneReminder.IsScheduled)
                    {
                        ScheduledActionService.Replace(reminder);
                    }
                }
                catch (Exception)
                {
                }
            }



        }
    }

    public class MessagesReposidory : ObjectsCacheBase<MessageDTO, GetMessagesCompletedEventArgs>
    {
        protected override void RetrieveObjects(BodyArchitectAccessServiceClient client1, PartialRetrievingInfo pageInfo, EventHandler<GetMessagesCompletedEventArgs> operationCompleted)
        {
            GetMessagesCriteria criteria = new GetMessagesCriteria();
            criteria.SortAscending = false;
            client1.GetMessagesAsync(ApplicationState.Current.SessionData.Token, criteria, pageInfo);
            client1.GetMessagesCompleted -= operationCompleted;
            client1.GetMessagesCompleted += operationCompleted;
        }

        protected override Guid GetCurrentHash()
        {
            return ApplicationState.Current.ProfileInfo != null ? ApplicationState.Current.ProfileInfo.DataInfo.MessageHash : Guid.Empty;
        }
    }

    public class TrainingPlansReposidory : ObjectsCacheBase<TrainingPlan, GetWorkoutPlansCompletedEventArgs>
    {
        protected override void RetrieveObjects(BodyArchitectAccessServiceClient client1, PartialRetrievingInfo pageInfo, EventHandler<GetWorkoutPlansCompletedEventArgs> operationCompleted)
        {
            WorkoutPlanSearchCriteria criteria = new WorkoutPlanSearchCriteria();
            criteria.SearchGroups.Add(WorkoutPlanSearchCriteriaGroup.Mine);
            criteria.SearchGroups.Add(WorkoutPlanSearchCriteriaGroup.Favorites);
            client1.GetWorkoutPlansAsync(ApplicationState.Current.SessionData.Token, criteria, pageInfo);
            client1.GetWorkoutPlansCompleted -= operationCompleted;
            client1.GetWorkoutPlansCompleted += operationCompleted;
        }

        protected override Guid GetCurrentHash()
        {
            return ApplicationState.Current.ProfileInfo.DataInfo.WorkoutPlanHash;
        }
    }

    public class CustomersReposidory : ObjectsCacheBase<CustomerDTO, GetCustomersCompletedEventArgs>
    {
        protected override void RetrieveObjects(BodyArchitectAccessServiceClient client1, PartialRetrievingInfo pageInfo, EventHandler<GetCustomersCompletedEventArgs> operationCompleted)
        {
            var param = new CustomerSearchCriteria();
            pageInfo.PageSize = -1;
            client1.GetCustomersAsync(ApplicationState.Current.SessionData.Token, param, pageInfo);
            client1.GetCustomersCompleted -= operationCompleted;
            client1.GetCustomersCompleted += operationCompleted;
        }

        protected override Guid GetCurrentHash()
        {
            return ApplicationState.Current.ProfileInfo.DataInfo.CustomerHash;
        }
    }

    public class SupplementsReposidory : ObjectsCacheBase<SuplementDTO, GetSuplementsCompletedEventArgs>
    {
        protected override void RetrieveObjects(BodyArchitectAccessServiceClient client1, PartialRetrievingInfo pageInfo, EventHandler<GetSuplementsCompletedEventArgs> operationCompleted)
        {
            var param = new GetSupplementsParam();
            client1.GetSuplementsAsync(ApplicationState.Current.SessionData.Token, param, pageInfo);
            client1.GetSuplementsCompleted -= operationCompleted;
            client1.GetSuplementsCompleted += operationCompleted;
        }
    }

    public class MyPlacesReposidory : ObjectsCacheBase<MyPlaceDTO, GetMyPlacesCompletedEventArgs>
    {
        protected override void RetrieveObjects(BodyArchitectAccessServiceClient client1, PartialRetrievingInfo pageInfo, EventHandler<GetMyPlacesCompletedEventArgs> operationCompleted)
        {
            var criteria = new MyPlaceSearchCriteria();
            client1.GetMyPlacesAsync(ApplicationState.Current.SessionData.Token, criteria, pageInfo);
            client1.GetMyPlacesCompleted -= operationCompleted;
            client1.GetMyPlacesCompleted += operationCompleted;
        }

        protected override Guid GetCurrentHash()
        {
            return ApplicationState.Current.ProfileInfo.DataInfo.MyPlaceHash;
        }
    }

    public class ExercisesReposidory : ObjectsCacheBase<ExerciseDTO, GetExercisesCompletedEventArgs>
    {
        protected override void RetrieveObjects(BodyArchitectAccessServiceClient client1, PartialRetrievingInfo pageInfo, EventHandler<GetExercisesCompletedEventArgs> operationCompleted)
        {
            ExerciseSearchCriteria criteria = new ExerciseSearchCriteria();
            criteria.SearchGroups = new List<ExerciseSearchCriteriaGroup>();
            criteria.SearchGroups.Add(ExerciseSearchCriteriaGroup.Mine);
            criteria.SearchGroups.Add(ExerciseSearchCriteriaGroup.Global);
            criteria.ExerciseTypes = new List<ExerciseType>();

            client1.GetExercisesAsync(ApplicationState.Current.SessionData.Token, criteria, pageInfo);
            client1.GetExercisesCompleted -= operationCompleted;
            client1.GetExercisesCompleted += operationCompleted;
        }

        protected override Guid GetCurrentHash()
        {
            return ApplicationState.Current.ProfileInfo.DataInfo.ExerciseHash;
        }
    }

    public abstract class ObjectsCacheBase<T, ARG> : INotifyCollectionChanged
        where T : BAGlobalObject
        where ARG : AsyncCompletedEventArgs
    {
        private IDictionary<Guid, T> dictExercises;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event EventHandler Loaded;

        public DateTime? Date { get; set; }

        public void Refresh()
        {
            Date = null;
            dictExercises = null;
            Load();
        }

        public Guid Hash { get; set; }

        public bool IsLoaded
        {
            get { return dictExercises != null && !ShouldRefresh; }
            private set { }
        }

        private bool ShouldRefresh
        {
            get { return shouldRefreshImplementation(Date, dictExercises); }
            set { }
        }

        protected virtual Guid GetCurrentHash()
        {
            return Guid.Empty;
        }

        bool shouldRefreshImplementation(DateTime? date, object dict)
        {
            if (ApplicationState.Current.IsOffline)
            {
                return false;
            }

            if (date == null || dict == null)// || ApplicationState.IsFree)
            {
                return true;
            }

            if (Hash != GetCurrentHash())
            {
                return true;
            }

            //if (Settings.RefreshFrequencyDays == -1)
            //{//setting never

            //    return false;
            //}
            //int refreshDays = Settings.RefreshFrequencyDays;
            //if (refreshDays == 0)
            //{//for refresh Every login we assume for this calculation that the period is 1 day but during login basically we delete the cache
            //    refreshDays = 1;
            //}


            //TimeSpan time = DateTime.UtcNow - date.Value;
            //return time.TotalDays > refreshDays;
            return false;
        }

        protected abstract void RetrieveObjects(BodyArchitectAccessServiceClient service, PartialRetrievingInfo pageInfo, EventHandler<ARG> operationCompleted);

        public void Load()
        {
            if (dictExercises == null || ShouldRefresh)
            {
                PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
                pageInfo.PageSize = 0;

                var m = new ServiceManager<ARG>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<ARG> operationCompleted)
                {
                    RetrieveObjects(client1, pageInfo, operationCompleted);

                });
                m.OperationCompleted += OnOperationCompleted;
                if (!m.Run())
                {
                    if (ApplicationState.Current.IsOffline)
                    {
                        BAMessageBox.ShowError(ApplicationStrings.ErrOfflineMode);
                    }
                    else
                    {
                        BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);
                    }
                    onLoaded();
                }
            }
            else
            {
                onLoaded();
            }
        }

        private void OnOperationCompleted(object s, ServiceManager<ARG>.ServiceManagerOperationResult a)
        {
            var m = (ServiceManager<ARG>)s;
            m.OperationCompleted -= OnOperationCompleted;
            if (a.Error != null)
            {
                onLoaded();
                return;
            }
            Date = DateTime.UtcNow;
            Hash = GetCurrentHash();
            var res = (IServicePagedResult<T>)a.Result;
            dictExercises = res.MyResult.Items.ToDictionary(t => t.GlobalId);
            onLoaded();
        }

        private void onLoaded()
        {
            if (Loaded != null)
            {
                Loaded(null, EventArgs.Empty);
            }
        }

        public IDictionary<Guid, T> Items
        {
            get
            {
                return dictExercises;
            }
            set { dictExercises = value; }
        }

        public T GetItem(Guid id)
        {
            if (dictExercises.ContainsKey(id))
            {
                return dictExercises[id];
            }
            return null;
        }

        public void Remove(Guid globalId)
        {
            Items.Remove(globalId);
        }
    }


    public abstract class SingleObjectCacheBase<T, ARG> : INotifyCollectionChanged
        where T : class
        where ARG : AsyncCompletedEventArgs
    {
        private T item;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event EventHandler Loaded;

        public DateTime? Date { get; set; }

        public void Refresh()
        {
            Date = null;
            item = null;
            Load();
        }

        public Guid Hash { get; set; }

        public bool IsLoaded
        {
            get { return item != null && !ShouldRefresh; }
            private set { }
        }

        private bool ShouldRefresh
        {
            get { return shouldRefreshImplementation(Date, item); }
            set { }
        }

        protected virtual Guid GetCurrentHash()
        {
            return Guid.Empty;
        }

        bool shouldRefreshImplementation(DateTime? date, object dict)
        {
            if (ApplicationState.Current.IsOffline)
            {
                return false;
            }

            if (date == null || dict == null)// || ApplicationState.IsFree)
            {
                return true;
            }

            if (Hash != GetCurrentHash())
            {
                return true;
            }

            //if (Settings.RefreshFrequencyDays == -1)
            //{//setting never

            //    return false;
            //}
            //int refreshDays = Settings.RefreshFrequencyDays;
            //if (refreshDays == 0)
            //{//for refresh Every login we assume for this calculation that the period is 1 day but during login basically we delete the cache
            //    refreshDays = 1;
            //}


            //TimeSpan time = DateTime.UtcNow - date.Value;
            //return time.TotalDays > refreshDays;
            return false;
        }

        protected abstract void RetrieveObjects(BodyArchitectAccessServiceClient service, EventHandler<ARG> operationCompleted);

        public void Load()
        {
            if (item == null || ShouldRefresh)
            {
                var m = new ServiceManager<ARG>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<ARG> operationCompleted)
                {
                    RetrieveObjects(client1, operationCompleted);

                });
                m.OperationCompleted += OnOperationCompleted;
                if (!m.Run())
                {
                    if (ApplicationState.Current.IsOffline)
                    {
                        BAMessageBox.ShowError(ApplicationStrings.ErrOfflineMode);
                    }
                    else
                    {
                        BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);
                    }
                    onLoaded();
                }
            }
            else
            {
                onLoaded();
            }
        }

        private void OnOperationCompleted(object s, ServiceManager<ARG>.ServiceManagerOperationResult a)
        {
            var m = (ServiceManager<ARG>)s;
            m.OperationCompleted -= OnOperationCompleted;
            if (a.Error != null)
            {
                onLoaded();
                return;
            }
            Date = DateTime.UtcNow;
            Hash = GetCurrentHash();
            var res = ((IServiceResult<T>)a.Result);
            item = res.MyResult;
            onLoaded();
        }

        private void onLoaded()
        {
            if (Loaded != null)
            {
                Loaded(null, EventArgs.Empty);
            }
        }

        public T Item
        {
            get
            {
                return item;
            }
            set { item = value; }
        }

    }

    public class CacheKey
    {
        public Guid? ProfileId { get; set; }

        public Guid? CustomerId { get; set; }

        public override bool Equals(object obj)
        {
            var ex = obj as CacheKey;
            if (((object)ex) == null)
            {
                return false;
            }
            return ex.ProfileId == ProfileId && ex.CustomerId == CustomerId;
        }

        public override int GetHashCode()
        {
            return ProfileId.GetHashCode() + CustomerId.GetHashCode();
        }
    }
}
