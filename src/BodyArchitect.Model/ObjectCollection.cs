using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace BodyArchitect.Model
{
    [Serializable]
    public class ObjectCollection<T> : CollectionBase, IList<T> 
    {
        public ObjectCollection()
        {
        }

        public T this [int index]
        {
            get { return (T)List[index]; }
            set { }
        }

        public int IndexOf(T item)
        {
            return List.IndexOf(item);
        }

        void IList<T>.Insert(int index, T item)
        {
            List.Insert(index,item);
        }

        void IList<T>.RemoveAt(int index)
        {
            List.RemoveAt(index);
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<T>.Remove(T item)
        {
            if (InnerList.Contains(item))
            {
                InnerList.Remove(item);
                return true;
            }
            return false;
        }

        void ICollection<T>.Add(T item)
        {
            List.Add(item);
        }

        public bool Contains(T item)
        {
            return false;
        }


        void ICollection<T>.CopyTo(T[] array, int index)
        {
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new CollectionEnumerator(InnerList.GetEnumerator());
        }

        private class CollectionEnumerator : IEnumerator<T>
        {
            private IEnumerator _Enumerator;

            public CollectionEnumerator(IEnumerator enumerator)
            {
                _Enumerator = enumerator;
            }

            public T Current
            {
                get { return (T)_Enumerator.Current; }
            }

            object IEnumerator.Current
            {
                get { return _Enumerator.Current; }
            }

            public bool MoveNext()
            {
                return _Enumerator.MoveNext();
            }

            public void Reset()
            {
                _Enumerator.Reset();
            }

            public void Dispose()
            {
            }
        }
    }
}

