using System;


namespace BodyArchitect.Model.Old
{
    [Serializable]
    public class A6WEntry : EntryObject
    {
        public static readonly Guid EntryTypeId = new Guid("461C9162-E81F-4635-A618-077E6EAAEBB1");

        #region Persistent properties

        //[Property(NotNull = true)]
        //[ValidateRange(1,42)]
        virtual public int DayNumber
        {
            get; set;
        }

        //[Property(NotNull = true)]
        virtual public bool Completed
        {
            get; set;
        }

        //[Property]
        virtual public int? Set1
        {
            get; set;
        }

        //[Property]
        virtual public int? Set2
        {
            get; set;
        }

        //[Property]
        virtual public int? Set3
        {
            get; set;
        }

        //[ValidateNonEmpty]
        //public override MyTraining MyTraining
        //{
        //    get
        //    {
        //        return base.MyTraining;
        //    }
        //    set
        //    {
        //        base.MyTraining = value;
        //    }
        //}

        #endregion

        public override Guid TypeId
        {
            get { return EntryTypeId; }
        }
    }
}
