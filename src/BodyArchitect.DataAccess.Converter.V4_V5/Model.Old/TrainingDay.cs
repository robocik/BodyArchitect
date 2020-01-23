using System;
using System.Collections.Generic;
using System.Linq;



namespace BodyArchitect.Model.Old
{
    [Serializable]
    public class TrainingDay : FMObject
    {
        private DateTime date;

        protected TrainingDay()
        {
            Objects = new Iesi.Collections.Generic.HashedSet<EntryObject>();
        }

        public TrainingDay(DateTime trainingDate):this()

        {
            date = trainingDate;
        }
 
        #region Persistent properties

        
        public DateTime TrainingDate
        {
            get { return date.Date; }
            private set { date = value; }
        }

        public virtual Profile Profile { get; set; }

        public virtual string Comment { get; set; }
        
        public virtual ICollection<EntryObject> Objects { get; set; }

        public virtual int Version { get; set; }

        #endregion

        #region Properties

        public virtual bool IsEmpty
        {
            get
            {
                foreach (EntryObject entryObject in Objects)
                {
                    if (!entryObject.IsEmpty)
                    {
                        return false;
                    }
                }
                return true;
            }
        }


        #endregion

        #region Methods

        public virtual List<T> GetSpecifiedEntries<T>() where T : EntryObject
        {
            return Objects.Where(e => e is T).Cast<T>().ToList();
        }

        public virtual bool ContainsSpecifiedEntry(Type entryType)
        {
            foreach (EntryObject entry in Objects)
            {
                if (entry.GetType() == entryType)
                {
                    return true;
                }
            }
            return false;
        }

        public virtual void AddEntry(EntryObject entry)
        {
            entry.TrainingDay = this;
            Objects.Add(entry);
        }

        public virtual EntryObject CreateEntryObject(Type entryType)
        {
            EntryObject entry = (EntryObject)Activator.CreateInstance(entryType);
            return entry;
        }

        public virtual EntryObject AddEntry(Type entryType)
        {
            var entryObject = CreateEntryObject(entryType);
            AddEntry(entryObject);
            return (EntryObject)entryObject;
        }

        public virtual void RemoveEntry(EntryObject entry)
        {
            Objects.Remove(entry);
            //entry.TrainingDay = null;
        }

        #endregion

        #region Static methods

        //public static IList<TrainingDay> GetAll(SessionData sessionData)
        //{
        //    //return FindAll(typeof (TrainingDay), Expression.Eq("ProfileId", sessionData.ProfileId)).Cast<TrainingDay>().ToList();
        //    var res =
        //        (from td in ActiveRecordLinq.AsQueryable<TrainingDay>()
        //         where td.ProfileId == sessionData.ProfileId
        //         select td);
        //    return res.ToList();
        //}

        //public static IList<TrainingDay> GetTrainingDays(SessionData sessionData, int myTrainingId)
        //{
        //    string query = "SELECT td FROM TrainingDay td,EntryObject a WHERE a.TrainingDay=td  AND a.MyTraining.Id=:trainingId";
        //    SimpleQuery<TrainingDay> q = new SimpleQuery<TrainingDay>(typeof(TrainingDay), query);

        //    q.SetParameter("trainingId", myTrainingId);
        //    return q.Execute();
        //}

        //public static TrainingDay GetLastTrainingDay(SessionData sessionData)
        //{
        //    //return (TrainingDay)FindFirst(typeof(TrainingDay),new Order[]{new Order("TrainingDate", false)}, Expression.Eq("ProfileId", sessionData.ProfileId));
        //    var res = (from td in ActiveRecordLinq.AsQueryable<TrainingDay>()
        //               where td.ProfileId == sessionData.ProfileId
        //               orderby td.TrainingDate descending
        //               select td);
        //    return res.FirstOrDefault();
        //}

        //public static TrainingDay GetFirstTrainingDay(SessionData sessionData)
        //{
        //    //return (TrainingDay)FindFirst(typeof(TrainingDay),new Order[]{new Order("TrainingDate", true)}, Expression.Eq("ProfileId", sessionData.ProfileId));
        //    var res = (from td in ActiveRecordLinq.AsQueryable<TrainingDay>()
        //               where td.ProfileId == sessionData.ProfileId
        //               orderby td.TrainingDate ascending
        //               select td);
        //    return res.FirstOrDefault();
        //}

        //public static IList<TrainingDay> GetTrainingDays(SessionData sessionData, DateTime startDate, DateTime endDate)
        //{
        //    var criteria = DetachedCriteria.For(typeof (TrainingDay));
        //    criteria.Add(Expression.Eq("ProfileId", sessionData.ProfileId));
        //    criteria.Add(Expression.Ge("TrainingDate", startDate.Date));
        //    criteria.Add(Expression.Le("TrainingDate", endDate.Date));
        //    //criteria.SetFetchMode("Objects", FetchMode.Join);
        //    criteria.SetResultTransformer(Transformers.DistinctRootEntity);
        //    return FindAll(typeof (TrainingDay), criteria).Cast<TrainingDay>().ToList();
        //}

        //public virtual TrainingDay GetNextTrainingDay(SessionData sessionData)
        //{
        //    //return (TrainingDay)FindFirst(typeof(TrainingDay),new Order[]{new Order("TrainingDate", true)}, Expression.Gt("TrainingDate", TrainingDate), Expression.Eq("ProfileId", ProfileId));
        //    var res = (from td in ActiveRecordLinq.AsQueryable<TrainingDay>()
        //               where td.ProfileId == sessionData.ProfileId && td.TrainingDate > TrainingDate
        //               orderby td.TrainingDate ascending
        //               select td);
        //    return res.FirstOrDefault();
        //}

        //public virtual TrainingDay GetPreviousTrainingDay(SessionData sessionData)
        //{
        //    return (TrainingDay)FindFirst(typeof(TrainingDay),new Order[]{new Order("TrainingDate",false)}, Expression.Lt("TrainingDate", TrainingDate), Expression.Eq("ProfileId", ProfileId));
        //    //var res = (from td in ActiveRecordLinq.AsQueryable<TrainingDay>()
        //    //           where td.ProfileId == sessionData.ProfileId && td.TrainingDate < TrainingDate
        //    //           orderby td.TrainingDate ascending
        //    //           select td);
        //    //return res.FirstOrDefault();
        //}

        //public static TrainingDay GetTrainingDay(SessionData sessionData, DateTime date)
        //{
        //    return GetTrainingDays(sessionData, date, date).SingleOrDefault();
        //}

        #endregion
    }
}