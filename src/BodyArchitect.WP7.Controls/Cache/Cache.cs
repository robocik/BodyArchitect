using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using BodyArchitect.Service.Client.WP7;

namespace BodyArchitect.WP7.Controls.Cache
{
    /// <summary>
    /// Class supports in memory cache for Silverlight applications. Silverlight 2 and 3 are supported.
    /// Each minute the cache items are iterated for validility, invalid cache items are removed.
    /// </summary>
    public class Cache : IDisposable
    {
        public static Cache Current = new Cache();
        private IDictionary<string, CacheItem> _cacheItems = new Dictionary<string, CacheItem>();

        private TimeSpan _period = TimeSpan.FromMinutes(1);
        private TimeSpan _startTimeSpan = TimeSpan.Zero;
        private TimeSpan _stopTimeSpan = TimeSpan.FromMilliseconds(-1);
        private Timer _timer;
        private TimerState _state = TimerState.Stopped;

        public Cache()
        {
            _timer = new Timer(CleanUpItems, this, _stopTimeSpan, _period);
        }


        public TimeSpan Period
        {
            get { return _period; }
            set { _period = value; }
        }

        public TimeSpan StartTimeSpan
        {
            get { return _startTimeSpan; }
            set { _startTimeSpan = value; }
        }

        public TimeSpan StopTimeSpan
        {
            get { return _stopTimeSpan; }
            set { _stopTimeSpan = value; }
        }


        public TimerState State
        {
            get { return _state; }
            set { _state = value; }
        }

        /// <summary>
        /// All the CacheItems are in a Dictionary
        /// </summary>
        public IDictionary<string, CacheItem> CacheItems
        {
            get { return _cacheItems; }
            set { _cacheItems = value; }
        }



        /// <summary>
        /// Get full CacheItem based on key.
        /// </summary>
        /// <param name="key">The key for which a CacheItem is stored.</param>
        /// <returns>The CacheItem stored for the given key, if it is still valid. Otherwise null.</returns>
        public CacheItem this[string key]
        {
            get
            {
                if (CacheItems.ContainsKey(key))
                {
                    CacheItem ci = CacheItems[key];
                    if (ci.IsValid())
                        return CacheItems[key];
                }
                return null;
            }
            set { CacheItems[key] = value; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            _timer.Dispose();
        }

        #endregion

        /// <summary>
        /// Get data typed directly for given key.
        /// </summary>
        /// <typeparam name="T">The Type for which the data is set. If the type is wrong null will be returned.</typeparam>
        /// <param name="key">The key for which a CacheItem is stored.</param>
        /// <returns>The data typed for the given key, if it is still valid. Otherwise null.</returns>
        public T Get<T>(string key) where T : class
        {
            CacheItem item = this[key];
            if (item != null)
                return item.GetData<T>();
            return null;
        }

        /// <summary>
        /// Get data untyped directly for given key.
        /// </summary>
        /// <param name="key">The key for which a CacheItem is stored.</param>
        /// <returns>The data untyped for the given key, if it is still valid. Otherwise null.</returns>
        public object Get(string key)
        {
            CacheItem item = this[key];
            if (item != null)
                return item.GetData();
            return null;
        }

        private void StartTimer()
        {
            if (_state == TimerState.Stopped)
            {
                _timer.Change(_startTimeSpan, _period);
                _state = TimerState.Started;
            }
        }

        private void StopTimer()
        {
            if (_state == TimerState.Started)
            {
                _timer.Change(_stopTimeSpan, _period);
                _state = TimerState.Stopped;
            }
        }

        /// <summary>
        /// Clean up items that are not longer valid.
        /// </summary>
        /// <param name="state">Expect state to be the cache object.</param>
        private static void CleanUpItems(object state)
        {
            try
            {
                var cache = state as Cache;
                if (cache != null)
                {
                    List<KeyValuePair<string, CacheItem>> itemsToRemove =
                        cache.CacheItems.Where(i => !i.Value.IsValid()).ToList();
                    foreach (var item in itemsToRemove)
                    {
                        cache.CacheItems.Remove(item.Key);
                    }
                    if (cache.CacheItems.Count == 0)
                        cache.StopTimer();
                }
            }
            catch (Exception)
            {
            }
            
        }

        /// <summary>
        /// Add a new item to the cache. If the key is already used it will be overwritten. 
        /// </summary>
        /// <param name="key">The key for which a CacheItem is stored.</param>
        /// <param name="value"></param>
        public void Add(string key, CacheItem value)
        {
            if (_cacheItems.ContainsKey(key))
                _cacheItems.Remove(key);
            _cacheItems.Add(key, value);
            StartTimer();
        }

        /// <summary>
        /// Add a new item to the cache. If the key is already used it will be overwritten. 
        /// </summary>
        /// <param name="key">The key for which a CacheItem is stored.</param>
        /// <param name="data">The data to cache.</param>
        /// <param name="validDuration">The duration of the caching of the data.</param>
        public void Add(string key, object data, TimeSpan validDuration)
        {
            Add(key, new CacheItem(data, validDuration));
        }

        /// <summary>
        /// Removes the item for the given key from the cache.
        /// </summary>
        /// <param name="key">The key for which a CacheItem is stored.</param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            bool remove = _cacheItems.Remove(key);
            if (_cacheItems.Count == 0)
                StopTimer();
            return remove;
        }

        #region Nested type: TimerState

        /// <summary>
        /// Used for determing TimerState
        /// </summary>
        public enum TimerState
        {
            Stopped,
            Started
        }

        #endregion
    }

}