using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using BodyArchitect.Service.V2.Model.Localization;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;


namespace BodyArchitect.Service.V2.Model
{
    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace,IsReference = true)]
    public class TrainingDayDTO : TrainingDayInfoDTO
    {
        public TrainingDayDTO()
        {
            Objects = new List<EntryObjectDTO>();
        }

        public TrainingDayDTO(DateTime trainingDate)
            : this(trainingDate,Guid.Empty)
        {
        }

        public TrainingDayDTO(DateTime trainingDate, Guid profileId)
            : this()
        {
            TrainingDate = trainingDate;
            ProfileId = profileId;
            Objects = new List<EntryObjectDTO>();
        }

        [DataMember]
        [NotNullValidator]
        [ObjectCollectionValidator(typeof(EntryObjectDTO))]
        public List<EntryObjectDTO> Objects { get; set; }

        

        #region Properties

        

        public virtual bool IsEmpty
        {
            get
            {
                foreach (var entryObject in Objects)
                {
                    if (!entryObject.IsEmpty)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public virtual bool CanMove
        {
            get
            {
                foreach (var entry in Objects)
                {
                    EntryObjectOperationsAttribute entryAttribute = new EntryObjectOperationsAttribute();
                    var attributes = entry.GetType().GetCustomAttributes(typeof(EntryObjectOperationsAttribute), true);
                    if (attributes.Length > 0)
                    {
                        entryAttribute = (EntryObjectOperationsAttribute)attributes[0];
                    }
                    if ((entryAttribute.Operations & EntryObjectOperation.Move) != EntryObjectOperation.Move)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        #endregion

        #region Methods


        public EntryObjectDTO GetEntryObject(Guid id)
        {
            return (from o in Objects where o.GlobalId == id select o).SingleOrDefault();
        }

        public void ChangeDate(DateTime newDate,bool onlyTrainingDate=false)
        {
            this.TrainingDate = newDate;

            if (!onlyTrainingDate)
            {
                foreach (var entry in Objects)
                {
                    IMovable movable = entry as IMovable;
                    if (movable != null)
                    {
                        movable.Move(newDate);
                    }
                }
            }
            
        }

        public TrainingDayDTO Copy()
        {
            TrainingDayDTO newDay = new TrainingDayDTO();
            newDay.Comment = Comment;
            newDay.ProfileId = ProfileId;
            newDay.TrainingDate = TrainingDate;
            newDay.CustomerId = CustomerId;
            newDay.AllowComments = AllowComments;
            foreach (var obj in Objects)
            {
                ICloneable copyable = obj as ICloneable;
                if (copyable != null)
                {
                    newDay.AddEntry((SpecificEntryObjectDTO)copyable.Clone());
                }
            }

            return newDay;
        }

        //public List<T> GetSpecifiedEntries<T>() where T : EntryObjectDTO
        //{
        //    return Objects.Where(e => e is T).Cast<T>().ToList();
        //}

        //public EntryObjectDTO GetSpecifiedEntries(Type type)
        //{
        //    return Objects.Where(e => e.GetType() == type).SingleOrDefault();
        //}

        public bool ContainsSpecifiedEntry(Type entryType)
        {
            foreach (var entry in Objects)
            {
                if (entry.GetType() == entryType)
                {
                    return true;
                }
            }
            return false;
        }

        public void AddEntry(EntryObjectDTO entry)
        {
            IMovable movable = entry as IMovable;
            if (movable != null)
            {
                //if the entry requires a date then set the date from TrainingDay object
                movable.Move(this.TrainingDate);
            }
            entry.TrainingDay = this;
            Objects.Add(entry);
        }

        public SpecificEntryObjectDTO CreateEntryObject(Type entryType)
        {
            var entry = (SpecificEntryObjectDTO)Activator.CreateInstance(entryType);
            IMovable movable = entry as IMovable;
            if (movable != null)
            {
                //if the entry requires a date then set the date from TrainingDay object
                movable.Move(this.TrainingDate);
            }
            //AddEntry(entry);
            return entry;
        }

        public SpecificEntryObjectDTO AddEntry(Type entryType)
        {
            var entryObject = CreateEntryObject(entryType);
            AddEntry(entryObject);
            return (SpecificEntryObjectDTO)entryObject;
        }

        public void RemoveEntry(EntryObjectDTO entry)
        {
            Objects.Remove(entry);
            //entry.TrainingDay = null;
        }

        //protected override bool BeforeSave(System.Collections.IDictionary state)
        //{
        //    for (int i = Objects.Count - 1; i >= 0; i--)
        //    {
        //        if (Objects[i].IsEmpty)
        //        {
        //            RemoveEntry(Objects[i]);
        //            continue;
        //        }
        //    }
        //    return base.BeforeSave(state);
        //}


        //protected override void OnSave()
        //{
        //    for (int i = Objects.Count - 1; i >= 0; i--)
        //    {
        //        IHasMyTraining myTraining = Objects[i] as IHasMyTraining;
        //        if (myTraining != null && myTraining.MyTraining!=null)
        //        {
        //            myTraining.MyTraining.Save();
        //        }
        //    }

        //    base.OnSave();
        //}

        //protected override void PostFlush()
        //{
        //    if (IsEmpty && Id > Constants.UnsavedObjectId)
        //    {
        //        Delete();
        //        return;
        //    }
        //    base.PostFlush();
        //}
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
    //[DataContract(Namespace = Const.ServiceNamespace)]
    //public class TrainingDayDTO:BAObject
    //{
    //    protected TrainingDayDTO()
    //    {
    //        Objects = new List<EntryObjectDTO>();
    //    }

    //    public TrainingDayDTO(DateTime trainingDate)
    //        : this(trainingDate,null)
    //    {
    //    }
    //    public TrainingDayDTO(DateTime trainingDate, ProfileDTO profile)
    //        : this()
    //    {
    //        TrainingDate = trainingDate;
    //        if (profile != null)
    //        {
    //            ProfileId = profile.GlobalId;
    //        }
    //    }

    //    #region Persistent properties

    //    [DataMember]
    //    public DateTime TrainingDate { get; private set; }

    //    [DataMember]
    //    public Guid ProfileId { get; set; }

    //    [DataMember]
    //    public string Comment { get; set; }


    //    [DataMember]
    //    public List<EntryObjectDTO> Objects { get; private set; }

    //    #endregion

    //    #region Properties

    //    public virtual bool IsEmpty
    //    {
    //        get
    //        {
    //            foreach (EntryObjectDTO entryObject in Objects)
    //            {
    //                if (!entryObject.IsEmpty)
    //                {
    //                    return false;
    //                }
    //            }
    //            return true;
    //        }
    //    }

    //    public virtual SpecificEntryObjectDTO CreateEntryObject(Type entryType)
    //    {
    //        var entry = (SpecificEntryObjectDTO)Activator.CreateInstance(entryType);
    //        IMovable movable = entry as IMovable;
    //        if (movable != null)
    //        {
    //            //if the entry requires a date then set the date from TrainingDay object
    //            movable.Move(this.TrainingDate);
    //        }
    //        return entry;
    //    }

    //    public virtual bool ContainsSpecifiedEntry(Type entryType)
    //    {
    //        foreach (var entry in Objects)
    //        {
    //            if (entry.GetType() == entryType)
    //            {
    //                return true;
    //            }
    //        }
    //        return false;
    //    }

    //    public virtual bool CanMove
    //    {
    //        get
    //        {
    //            foreach (var entry in Objects)
    //            {
    //                EntryObjectOperationsAttribute entryAttribute = new EntryObjectOperationsAttribute();
    //                var attributes = entry.GetType().GetCustomAttributes(typeof(EntryObjectOperationsAttribute), true);
    //                if (attributes.Length > 0)
    //                {
    //                    entryAttribute = (EntryObjectOperationsAttribute)attributes[0];
    //                }
    //                if ((entryAttribute.Operations & EntryObjectOperation.Move) != EntryObjectOperation.Move)
    //                {
    //                    return false;
    //                }
    //            }
    //            return true;
    //        }
    //    }

    //    #endregion

    //    public void SplitByType(Dictionary<Type, List<EntryObjectDTO>> entriesGroup, bool loadedOnly)
    //    {
    //        foreach (var entry in Objects)
    //        {
    //            if (!loadedOnly || entry.IsLoaded)
    //            {
    //                Type type = entry.GetType();
    //                if (!entriesGroup.ContainsKey(type))
    //                {
    //                    entriesGroup.Add(type, new List<EntryObjectDTO>());
    //                }
    //                entriesGroup[type].Add(entry);
    //            }
    //        }
    //    }

    //    public Dictionary<Type, List<EntryObjectDTO>> SplitByType(bool loadedOnly)
    //    {
    //        Dictionary<Type, List<EntryObjectDTO>> dict = new Dictionary<Type, List<EntryObjectDTO>>();
    //        SplitByType(dict, loadedOnly);
    //        return dict;
    //    }
    //}
}
