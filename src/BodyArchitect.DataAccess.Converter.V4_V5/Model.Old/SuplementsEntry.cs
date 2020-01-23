using System;
using System.Collections.Generic;
using BodyArchitect.Model.Old;



namespace BodyArchitect.Model.Old
{
    [Serializable]
    public class SuplementsEntry : EntryObject, ICloneable
    {
        public static readonly Guid EntryTypeId = new Guid("ABEEA6FD-629F-4A0B-994B-1FAE73180A36");

        public SuplementsEntry()
        {
            Items = new Iesi.Collections.Generic.HashedSet<SuplementItem>();
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

        //[HasMany(Inverse = true, Cascade = ManyRelationCascadeEnum.AllDeleteOrphan, BatchSize = Constants.DefaultBatchSize)]
        public virtual ICollection<SuplementItem> Items { get;set; }

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

        #region ICloneable Members

        public virtual object Clone()
        {
            SuplementsEntry entry = new SuplementsEntry();
            entry.ReportStatus = ReportStatus;
            entry.Comment = Comment;
            entry.Name = Name;
            foreach (var suplementItem in Items)
            {
                
                entry.AddItem((SuplementItem)suplementItem.Clone());
            }
            return entry;
        }

        #endregion
    }
}
