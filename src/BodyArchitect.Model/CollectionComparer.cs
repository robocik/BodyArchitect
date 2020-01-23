using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Shared;

namespace BodyArchitect.Model
{
    public class CollectionComparer<T> where T : FMGlobalObject
    {
        private List<T> addedItems;
        private List<T> notChangedItems;
        private List<T> removedItems;
        private ItemComparer itemComparer;
        public delegate bool ItemComparer(T item1, T item2);

        public CollectionComparer(IEnumerable<T> newList, IEnumerable<T> oldList, bool addRemoveWithZeroID, ItemComparer itemComparer)
        {
            this.itemComparer = itemComparer;
            addedItems = getAddedReferences(newList, oldList, addRemoveWithZeroID);
            removedItems = getRemovedReferences(newList, oldList, addRemoveWithZeroID);
            notChangedItems = getUnchangedReferences(newList, oldList, addRemoveWithZeroID);
        }

        public CollectionComparer(IEnumerable<T> newList, IEnumerable<T> oldList, bool addRemoveWithZeroID)
            : this(newList, oldList, addRemoveWithZeroID, delegate(T item1, T item2)
            {
                return item2.GlobalId == item1.GlobalId;
            })
        {
        }

        public CollectionComparer(IEnumerable<T> newList, IEnumerable<T> oldList)
            : this(newList, oldList, true)
        {
        }

        public CollectionComparer(IEnumerable<T> newList, IEnumerable<T> oldList, ItemComparer itemComparer)
            : this(newList, oldList, true, itemComparer)
        {
        }

        public List<T> GetForSaveItems()
        {
            List<T> items = new List<T>(AddedItems);
            items.AddRange(NotChangedItems);
            return items;
        }

        public List<T> AddedItems
        {
            get
            {
                return addedItems;
            }
        }

        public List<T> RemovedItems
        {
            get
            {
                return removedItems;
            }
        }

        public bool AreEquals
        {
            get
            {
                return addedItems.Count == 0 && removedItems.Count == 0;
            }
        }

        public List<T> NotChangedItems
        {
            get { return notChangedItems; }
        }

        private List<T> getRemovedReferences(IEnumerable<T> newList, IEnumerable<T> oldList, bool addRemoveWithZeroID)
        {
            List<T> removedReferences = new List<T>();
            foreach (T reference in oldList)
            {
                if (!(addRemoveWithZeroID && reference.GlobalId ==Constants.UnsavedGlobalId) && findReference(newList, reference) == null)
                {
                    removedReferences.Add(reference);
                }
            }
            return removedReferences;
        }

        private List<T> getAddedReferences(IEnumerable<T> newList, IEnumerable<T> oldList, bool addRemoveWithZeroID)
        {
            List<T> addedReferences = new List<T>();
            foreach (T reference in newList)
            {
                if ((addRemoveWithZeroID && reference.GlobalId == Constants.UnsavedGlobalId) || findReference(oldList, reference) == null)
                {
                    addedReferences.Add(reference);
                }
            }
            return addedReferences;
        }

        private List<T> getUnchangedReferences(IEnumerable<T> newList, IEnumerable<T> oldList, bool addRemoveWithZeroID)
        {
            List<T> notChangedItems = new List<T>();
            foreach (T reference in newList)
            {
                if (reference.GlobalId != Constants.UnsavedGlobalId || findReference(oldList, reference) != null)
                {
                    notChangedItems.Add(reference);
                }
            }
            return notChangedItems;
        }

        FMGlobalObject findReference(IEnumerable<T> list, T refD)
        {
            foreach (T objRef in list)
            {
                if (itemComparer(objRef, refD))
                {
                    return objRef;
                }
            }
            return null;
        }
    }
}
