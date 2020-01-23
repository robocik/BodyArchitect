using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.WCF;
using BodyArchitect.Portable;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;

namespace BodyArchitect.Client.UI.Cache
{
    public class TrainingDaysReposidory : INotifyCollectionChanged
    {
        private static ConcurrentDictionary<CacheKey, TrainingDaysReposidory> dictExercises = new ConcurrentDictionary<CacheKey, TrainingDaysReposidory>();
        ConcurrentDictionary<DateTime, TrainingDayDTO> days = new ConcurrentDictionary<DateTime, TrainingDayDTO>();
        private List<DateTime> retrievedMonths = new List<DateTime>();
        private CacheKey key;

        private TrainingDaysReposidory(CacheKey key)
        {
            this.key = key;
            UserContext.Current.LoginStatusChanged += new EventHandler<LoginStatusEventArgs>(UserContext_LoginStatusChanged);
        }

        void UserContext_LoginStatusChanged(object sender, LoginStatusEventArgs e)
        {
            if (e.Status == LoginStatus.Logged)
            {
                Clear();
            }
        }

        public bool IsMonthLoaded(DateTime monthDate)
        {
            return retrievedMonths.Contains(monthDate);
        }

        public IDictionary<DateTime,TrainingDayDTO> Days
        {
            get { return days; }
        }

        public void RetrieveDays(DateTime startDate,DateTime endDate)
        {
            var param = new WorkoutDaysSearchCriteria();
            DateTime firstDay = DateHelper.GetFirstDayOfMonth(startDate);
            DateTime endDay = DateHelper.GetLastDayOfMonth(endDate);

            param.StartDate = firstDay;
            param.EndDate = endDay;
            param.CustomerId = key.CustomerId;
            param.UserId = key.ProfileId;

            var pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = PartialRetrievingInfo.AllElementsPageSize;
            var res = ServiceManager.GetTrainingDays(param, pageInfo);
            foreach (var dto in res.Items)
            {
                days.AddOrUpdate(dto.TrainingDate, dto, (x, y)=>dto);
            }
            for (int i = 0; i < endDate.MonthDifference(startDate)+1; i++)
            {
                DateTime tempDate = startDate.AddMonths(i);
                if (!retrievedMonths.Contains(tempDate))
                {
                    retrievedMonths.Add(tempDate);
                }
            }
        }

        public static TrainingDaysReposidory GetCache(Guid? customerId, Guid? userId)
        {
            if (userId.HasValue && userId == UserContext.Current.CurrentProfile.GlobalId)
            {
                userId = null;
            }
            var key = new CacheKey(){CustomerId = customerId,ProfileId = userId};
            return dictExercises.GetOrAdd(key, (v) => new TrainingDaysReposidory(v));
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }

        public void Add(TrainingDayDTO newItem)
        {
            days.AddOrUpdate(newItem.TrainingDate, newItem, (key, old) => newItem);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem));
        }

        public void Remove(DateTime trainingDayDate)
        {
            TrainingDayDTO old;
            days.TryRemove(trainingDayDate, out old);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, old));
        }

        public void Clear()
        {
            days.Clear();
            retrievedMonths.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public TrainingDayDTO GetByDate(DateTime date)
        {
            TrainingDayDTO day;
            days.TryGetValue(date.Date,out day);
            return day;
        }
    }
}
