using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using BodyArchitect.Shared;

namespace BodyArchitect.Service.Model
{
    public enum TrainingEnd
    {
        NotEnded,
        Complete,
        Break
    }

    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class MyTrainingDTO:BAObject
    {
        TrainingEnd traniningEnd = TrainingEnd.NotEnded;

        public MyTrainingDTO()
        {
        }

        #region Persistent properties

        [DataMember]
        [NotNullValidator]
        [StringLengthValidator(1,Constants.NameColumnLength)]
        public string Name
        {
            get;
            set;
        }

  
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
        public int ProfileId
        {
            get;
            set;
        }

        [DataMember]
        public int? PercentageCompleted
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

        public void Abort()
        {
            TrainingEnd = TrainingEnd.Break;
            EndDate = DateTime.Now;
        }

        public void Complete()
        {
            TrainingEnd = TrainingEnd.Complete;
            EndDate = DateTime.Now;
        }
        #endregion

    }
}
