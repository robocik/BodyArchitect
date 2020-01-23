using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BodyArchitect.Shared;


namespace BodyArchitect.Model
{
    public enum TrainingEnd
    {
        NotEnded,
        Complete
    }

    [Serializable]
    public class A6WTraining:MyTraining
    {
        
    }

    [Serializable]
    public abstract class MyTraining:FMGlobalObject,IHasUser,IHasCustomer
    {
        TrainingEnd traniningEnd = TrainingEnd.NotEnded;

        public MyTraining()
        {
            EntryObjects = new HashSet<EntryObject>();
        }

        #region Persistent properties

        //[Property(NotNull = true, Length = Constants.NameColumnLength)]
        //[ValidateLength(1, Constants.NameColumnLength)]
        //[ValidateNonEmpty]
        virtual public string Name
        {
            get; set;
        }

        public virtual TimeSpan? RemindBefore { get; set; }
        //[HasMany(Inverse=false,Lazy=true,Cascade=ManyRelationCascadeEnum.SaveUpdate)]
        virtual public ICollection<EntryObject> EntryObjects
        {
            get; protected set;
        }
        //[Property(NotNull=true)]
        virtual public TrainingEnd TrainingEnd
        {
            get { return traniningEnd; }
            protected set { traniningEnd = value; }
        }

        //[Property(NotNull = true)]
        virtual public DateTime StartDate
        {
            get; set; }

        //[Property]
        virtual public DateTime? EndDate
        {
            get;
            protected set;
        }

        //[Property(NotNull=true)]
        //[ValidateNonEmpty]
        //virtual public Guid TypeId
        //{
        //    get; set;
        //}

        //[Property(NotNull = true)]
        virtual public Profile Profile
        {
            get; set;
        }

        virtual public Customer Customer
        {
            get;
            set;
        }

        virtual public int PercentageCompleted
        {
            get; set;
        }

        
        #endregion

        #region Methods

        public virtual void SetData(DateTime startDate,DateTime? endDate,TrainingEnd trainingEnd)
        {
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.TrainingEnd = trainingEnd;
        }

        public virtual void Complete(ITimerService timer)
        {
            TrainingEnd = TrainingEnd.Complete;
            EndDate = timer.UtcNow;
        }
        #endregion
      
        //#region Static methods

        //public static IList<MyTraining> GetStartedTrainings(int profileId)
        //{
        //    //return MyTraining.FindAll(Expression.Eq("ProfileId", profileId), Expression.IsNull("EndDate"));
        //    var res = (from td in ActiveRecordLinq.AsQueryable<MyTraining>() where td.ProfileId == profileId && td.EndDate==null select td);
        //    return res.ToList();
        //}

        //public static IList<MyTraining> GetStartedTrainings<T>(int profileId) where T : EntryObject
        //{
        //    //return FindAll(Expression.Eq("ProfileId", profileId),Expression.Eq("EntryType", typeof (T).FullName), Expression.IsNull("EndDate"));
        //    var res = (from td in ActiveRecordLinq.AsQueryable<MyTraining>() where td.ProfileId == profileId && td.EntryType==typeof (T).FullName && td.EndDate==null select td);
        //    return res.ToList();
        //}
           
        

        //public static IList<MyTraining> GetAll(int profileId)
        //{
        //    //return FindAll(Expression.Eq("ProfileId", profileId)).ToList();
        //    var res = (from td in ActiveRecordLinq.AsQueryable<MyTraining>() where td.ProfileId == profileId select td);
        //    return res.ToList();
        //}

        //#endregion
    }
}
