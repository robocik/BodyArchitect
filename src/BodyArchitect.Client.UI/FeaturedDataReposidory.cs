using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.UI
{
    public abstract class SingleObjectCache<T> : INotifyPropertyChanged where T:class 
    {
        private ManualResetEvent exercisesEvent = new ManualResetEvent(false);
        private T featuredData = null;

        public SingleObjectCache()
        {
            UserContext.Current.LoginStatusChanged += new EventHandler<LoginStatusEventArgs>(UserContext_LoginStatusChanged);
            loadExercises();
        }

        void UserContext_LoginStatusChanged(object sender, LoginStatusEventArgs e)
        {
            if (e.Status == LoginStatus.Logged)
            {
                ClearCache();
            }
        }

        public bool IsLoaded
        {
            get { return featuredData != null; }
        }


        private Task loadExercises()
        {
            exercisesEvent.Reset();
            featuredData = null;

            var task = Task.Factory.StartNew(delegate
            {
                Helper.EnsureThreadLocalized();
                try
                {
                    featuredData= GetItemsMethod();
                }
                catch (Exception)
                {
                    exercisesEvent.Set();
                    throw;
                }
                

                exercisesEvent.Set();
                OnPropertyChanged();
            }, exercisesEvent);
            OnPropertyChanged();
            task = LogUnhandledExceptions(task);
            return task;
        }

        protected abstract T GetItemsMethod();

        public void Reset()
        {
            featuredData = null;
            OnPropertyChanged();
        }

        public void ClearCache()
        {
            if (featuredData != null)
            {
                loadExercises();
            }
        }

        public void EnsureLoaded()
        {
            BeginEnsure().WaitOne();
        }

        public WaitHandle BeginEnsure()
        {
            return exercisesEvent;
        }

        public T Item
        {
            get
            {
                EnsureLoaded();
                return featuredData;
            }
        }

        private Task LogUnhandledExceptions(Task task)
        {
            task.ContinueWith(delegate(Task t)
            {
                ExceptionHandler.Default.Process(t.Exception);
            }, TaskContinuationOptions.OnlyOnFaulted);
            return task;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged()
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs("Item"));
        }
    }

    public class FeaturedDataReposidory : SingleObjectCache<FeaturedData>
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


        protected override FeaturedData GetItemsMethod()
        {
            return ServiceManager.GetFeaturedData();
        }
    }
}
