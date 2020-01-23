using System;

namespace BodyArchitect.WP7.Controls.Cache
{
    /// <summary>
/// Defines an item that's stored in the Cache.
/// </summary>
    public class CacheItem
    {
        private readonly DateTime _creationDate;

        private readonly TimeSpan _validDuration;

        /// <summary>
        /// Constructs a cache item with for the data with a validity of validDuration.
        /// </summary>
        /// <param name="data">The data for the cache.</param>
        /// <param name="validDuration">The duration for the data being valid in the cache.</param>
        public CacheItem(object data, TimeSpan validDuration):this(data,validDuration,DateTime.Now)
        {

        }

        public CacheItem(object data, TimeSpan validDuration,DateTime creationDate)
        {
            _validDuration = validDuration;
            Data = data;
            _creationDate = creationDate;
        }

        /// <summary>
        /// The data in the Cache.
        /// </summary>
        public object Data { set; get; }

        public TimeSpan ValidDuration
        {
            get { return _validDuration; }
        }

        public DateTime CreationDate
        {
            get { return _creationDate; }
        }

        /// <summary>
        /// Gets the Data typed.
        /// </summary>
        /// <typeparam name="T">The Type for which the data is set. If the type is wrong null will be returned.</typeparam>
        /// <returns>The data typed.</returns>
        public T GetData<T>() where T : class
        {
            return Data as T;
        }

        /// <summary>
        /// Gets the Data untyped.
        /// </summary>
        /// <returns>The data untyped.</returns>
        public object GetData()
        {
            return Data;
        }

        /// <summary>
        /// Check if the Data is still valid.
        /// </summary>
        /// <returns>Valid if the validDuration hasn't passed.</returns>
        public bool IsValid()
        {
            return CreationDate.Add(ValidDuration) > DateTime.Now;
        }

    }
}