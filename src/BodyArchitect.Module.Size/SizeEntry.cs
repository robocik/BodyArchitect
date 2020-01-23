//using System;
//using BodyArchitect.Model;
//using BodyArchitect.Model.Attributes;

//namespace BodyArchitect.Module.Size.Model
//{
//    [Serializable]
//    [EntryObjectInstanceAttribute(EntryObjectInstance.Single)]
//    [EntryObjectOperationsAttribute(EntryObjectOperation.Copy | EntryObjectOperation.Move)]
//    public class SizeEntry : SpecificEntryObject, IHasWymiar, ICloneable, IMovable
//    {
//        public SizeEntry()
//        {
//            Wymiary = new Wymiary();
//        }

//        //[BelongsTo(Cascade=CascadeEnum.All,Fetch=FetchEnum.Join)]
//        public virtual Wymiary Wymiary
//        {
//            get;
//            set;
//        }

//        #region Methods

//        public override bool IsEmpty
//        {
//            get { return Wymiary == null || Wymiary.IsEmpty; }
//        }

//        public virtual object Clone()
//        {
//            SizeEntry sizeEntry = new SizeEntry();
//            sizeEntry.Comment = Comment;
//            sizeEntry.ReportStatus = ReportStatus;
//            sizeEntry.Name = Name;
//            sizeEntry.TrainingDay = TrainingDay;
//            if (Wymiary != null)
//            {
//                sizeEntry.Wymiary = Wymiary.Clone(false);
//            }
//            return sizeEntry;

//        }

//        public virtual void Move(DateTime newDateTime)
//        {
//            if (Wymiary != null)
//            {
//                Wymiary.DateTime = DateHelper.MoveToNewDate(Wymiary.DateTime, newDateTime);
//            }
//        }
//        #endregion
//    }
//}
