using System;


namespace BodyArchitect.Model.Old
{
    [Serializable]
    public class SizeEntry : EntryObject, IHasWymiar, ICloneable
    {
        public static readonly Guid EntryTypeId = new Guid("023C93B0-69AC-4E65-A39D-F13F8311AB0E");

        public SizeEntry()
        {
            Wymiary = new Wymiary();
        }

        public Wymiary Wymiary
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
            get { return Wymiary == null || Wymiary.IsEmpty; }
        }

        public virtual object Clone()
        {
            SizeEntry sizeEntry = new SizeEntry();
            sizeEntry.Comment = Comment;
            sizeEntry.ReportStatus = ReportStatus;
            sizeEntry.Name = Name;
            sizeEntry.TrainingDay = TrainingDay;
            if (Wymiary != null)
            {
                sizeEntry.Wymiary = Wymiary.Clone(false);
            }
            return sizeEntry;

        }

        #endregion
    }
}
