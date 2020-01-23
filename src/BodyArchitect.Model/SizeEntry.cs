using System;
using BodyArchitect.Shared;

namespace BodyArchitect.Model
{
    [Serializable]
    [EntryObjectOperationsAttribute(EntryObjectOperation.Copy | EntryObjectOperation.Move)]
    public class SizeEntry : EntryObject, IHasWymiar
    {
        public static readonly Guid EntryTypeId = new Guid("023C93B0-69AC-4E65-A39D-F13F8311AB0E");

        public SizeEntry()
        {
            Wymiary = new Wymiary();
        }

        public virtual Wymiary Wymiary
        {
            get;
            set;
        }

        #region Methods

        public override Guid TypeId
        {
            get { return EntryTypeId; }
        }

        public override bool IsEmpty
        {
            get { return Status==EntryObjectStatus.Done && (Wymiary == null || Wymiary.IsEmpty); }
        }


        #endregion
    }
}
