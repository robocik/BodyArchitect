using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using BodyArchitect.Shared;

namespace BodyArchitect.Service.V2.Model
{
    public enum TrainingEnd
    {
        NotEnded,
        Complete
    }

    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace, IsReference = true)]
    public class A6WTrainingDTO : MyTrainingDTO
    {

    }

    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace, IsReference = true)]
    public class MyTrainingLightDTO : BAGlobalObject
    {
        TrainingEnd traniningEnd = TrainingEnd.NotEnded;


        #region Persistent properties

        [DataMember]
        [NotNullValidator]
        [StringLengthValidator(1, Constants.NameColumnLength)]
        public string Name
        {
            get;
            set;
        }

        [DataMember]
        public TimeSpan? RemindBefore { get; set; }

        [DataMember]
        public TrainingEnd TrainingEnd
        {
            get { return traniningEnd; }
            private set { traniningEnd = value; }
        }

        [DataMember]
        public DateTime StartDate
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? EndDate
        {
            get;
            private set;
        }

        [DataMember]
        public Guid TypeId
        {
            get;
            set;
        }

        [DataMember]
        public Guid ProfileId
        {
            get;
            set;
        }

        [DataMember]
        public int PercentageCompleted
        {
            get;
            set;
        }

        [DataMember]
        public Guid? CustomerId
        {
            get;
            set;
        }

        #endregion

        #region Methods

        public void SetData(DateTime startDate, DateTime? endDate, TrainingEnd trainingEnd)
        {
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.TrainingEnd = trainingEnd;
        }

        public void Complete()
        {
            TrainingEnd = TrainingEnd.Complete;
            EndDate = DateTime.Now;
        }
        #endregion

    }

    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace, IsReference = true)]
    [KnownType(typeof(SupplementsCycleDTO))]
    [KnownType(typeof(A6WTrainingDTO))]
    public abstract class MyTrainingDTO : MyTrainingLightDTO
    {
        protected MyTrainingDTO()
        {
            EntryObjects = new  List<EntryObjectDTO>();

        }

        private IList<EntryObjectDTO> _entryObjects;

        [DataMember]
        public IList<EntryObjectDTO> EntryObjects
        {
            get { return _entryObjects; }
            set { _entryObjects = value; }
        }
    }
}
