using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Common
{
    public abstract class ObjectsCacheBase<T> : INotifyCollectionChanged where T : BAGlobalObject
    {
        private ConcurrentDictionary<Guid, T> dictExercises;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        private ManualResetEvent exercisesEvent = new ManualResetEvent(false);
        private bool isLoading = false;

        public ObjectsCacheBase()
        {
            UserContext.Current.LoginStatusChanged += new EventHandler<LoginStatusEventArgs>(UserContext_LoginStatusChanged);
            loadExercises();
        }

        public void EnsureLoaded()
        {
            BeginEnsure().WaitOne();
        }

        public WaitHandle BeginEnsure()
        {
            //if (dictExercises == null)
            //{
            //    dictExercises = new ConcurrentDictionary<Guid, T>();
            //    if (Helper.IsDesignMode)
            //    {
            //        return null;
            //    }

            //}
            if(dictExercises==null && !isLoading)
            {
                loadExercises();
            }
           return exercisesEvent;
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }
        void UserContext_LoginStatusChanged(object sender, LoginStatusEventArgs e)
        {
            if (e.Status == LoginStatus.Logged)
            {
                ClearCache();
            }
        }

        private Task loadExercises()
        {
            exercisesEvent.Reset();
            isLoading = true;
            dictExercises = null;
            
            var task = Task.Factory.StartNew(delegate
            {
                Helper.EnsureThreadLocalized();
                PagedResultRetriever retriever = new PagedResultRetriever();
                var res = retriever.GetAll(delegate(PartialRetrievingInfo pageInfo)
                {
                    try
                    {
                        return GetItemsMethod(pageInfo);
                    }
                    catch (Exception)
                    {
                        exercisesEvent.Set();
                        throw;
                    }

                });
                dictExercises = new ConcurrentDictionary<Guid, T>(res.ToDictionary(t => t.GlobalId, n => n));
                isLoading = false;
                exercisesEvent.Set();
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, dictExercises.Values.ToList()));
            }, exercisesEvent);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            task=LogUnhandledExceptions(task);
            return task;
        }

        protected abstract PagedResult<T> GetItemsMethod(PartialRetrievingInfo pageInfo);

        private Task LogUnhandledExceptions(Task task)
        {
            task.ContinueWith(delegate(Task t)
            {
                ExceptionHandler.Default.Process(t.Exception);
            }, TaskContinuationOptions.OnlyOnFaulted);
            return task;
        }

        public void Replace(T selected, T saved)
        {
            if (dictExercises.TryUpdate(selected.GlobalId, saved, selected))
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, saved,selected));
            }
        }

        public void Add(T newItem)
        {
            EnsureLoaded();
            dictExercises.TryAdd(newItem.GlobalId, newItem);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem));
        }

        public void AddOrUpdate(T newItem)
        {
            EnsureLoaded();
            dictExercises.AddOrUpdate(newItem.GlobalId, newItem, (key, old) => newItem);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem));
        }

        public void Update(T item)
        {
            EnsureLoaded();
            dictExercises[item.GlobalId] = item;
            dictExercises.TryUpdate(item.GlobalId, item, item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, null));
        }

        public void Remove(Guid globalId)
        {
            EnsureLoaded();
            T old;
            dictExercises.TryRemove(globalId, out old);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, old));
        }

        public bool IsLoaded
        {
            get { return dictExercises != null; }
        }

        public IDictionary<Guid, T> Items
        {
            get
            {
                EnsureLoaded();
                return dictExercises;
            }
        }

        public virtual T GetItem(Guid id)
        {
            EnsureLoaded();
            if (dictExercises.ContainsKey(id))
            {
                return dictExercises[id];
            }
            return null;
        }

        public void Reset()
        {
            dictExercises = null;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void ClearCache()
        {
            if (dictExercises != null)
            {
                loadExercises();
            }
        }
    }

    public struct CacheKey
    {
        public Guid? ProfileId { get; set; }

        public Guid? CustomerId { get; set; }
    }


}
