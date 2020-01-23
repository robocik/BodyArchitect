using System;
using System.Collections.Generic;
using BodyArchitect.Model;
using BodyArchitect.Shared;


namespace BodyArchitect.Model
{
    [Serializable]
    [EntryObjectInstance(EntryObjectInstance.Single)]
    [EntryObjectOperationsAttribute(EntryObjectOperation.Copy | EntryObjectOperation.Move)]
    public class SuplementsEntry : EntryObject
    {
        public static readonly Guid EntryTypeId = new Guid("ABEEA6FD-629F-4A0B-994B-1FAE73180A36");

        public SuplementsEntry()
        {
            Items = new HashSet<SuplementItem>();
        }

        public override Guid TypeId
        {
            get
            {
                return EntryTypeId;
            }
        }

        public override bool IsEmpty
        {
            get { return Items.Count == 0; }
        }

        public virtual ICollection<SuplementItem> Items { get;protected set; }

        public virtual void AddItem(SuplementItem item)
        {
            item.SuplementsEntry = this;
            Items.Add(item);
        }

        public virtual void RemoveItem(SuplementItem item)
        {
            Items.Remove(item);
            item.SuplementsEntry = null;
        }


    }
}
