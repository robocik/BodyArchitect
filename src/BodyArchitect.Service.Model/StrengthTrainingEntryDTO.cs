using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.Model
{
    public enum Intensity
    {
        NotSet,
        Low,
        Medium,
        Hight
    }
    [DataContract(Namespace = Const.ServiceNamespace,IsReference = true)]
    [Serializable]
    [EntryObjectInstance(EntryObjectInstance.Single)]
    [EntryObjectOperationsAttribute(EntryObjectOperation.Copy | EntryObjectOperation.Move)]
    public class StrengthTrainingEntryDTO : SpecificEntryObjectDTO, ICloneable, ITrainingPlanConnector, IRepositionableParent
    {
        public StrengthTrainingEntryDTO()
        {
            Entries = new List<StrengthTrainingItemDTO>();
        }

        #region Persistent properties

        [DataMember]
        [NotNullValidator]
        [ObjectCollectionValidator(typeof(StrengthTrainingItemDTO))]
        public virtual List<StrengthTrainingItemDTO> Entries { get; set; }

        [DataMember]
        public virtual DateTime? EndTime
        {
            get;
            set;
        }

        /// <summary>
        ///Workout plan day id
        /// </summary>
        [DataMember]
        public virtual Guid? TrainingPlanItemId
        {
            get;
            set;
        }

        /// <summary>
        /// Workout plan id
        /// </summary>
        [DataMember]
        public virtual Guid? TrainingPlanId
        {
            get;
            set;
        }

        [DataMember]
        public virtual DateTime? StartTime
        {
            get;
            set;
        }

        [DataMember]
        public virtual Intensity Intensity { get; set; }

        #endregion

        #region Methods

        public virtual void AddEntry(StrengthTrainingItemDTO entry)
        {
            entry.StrengthTrainingEntry = this;
            Entries.Add(entry);
        }

        public virtual void RemoveEntry(StrengthTrainingItemDTO entry)
        {
            Entries.Remove(entry);
            entry.StrengthTrainingEntry = null;
        }

        public virtual void RepositionEntry(int index1, int index2)
        {
            var item = Entries[index1];
            Entries.Remove(item);
            Entries.Insert(index2, item);
            item.Position = index2;
            Entries[index1].Position = index2;
        }

        public override bool IsEmpty
        {
            get { return Entries.Count == 0; }
        }
        public virtual int GetMaximumSeriesCount()
        {
            int max = 0;
            foreach (StrengthTrainingItemDTO entry in Entries)
            {
                if (max < entry.Series.Count)
                {
                    max = entry.Series.Count;
                }
            }
            return max;
        }

        public virtual object Clone()
        {
            StrengthTrainingEntryDTO strengthEntry = new StrengthTrainingEntryDTO();
            strengthEntry.StartTime = StartTime;
            strengthEntry.Comment = Comment;
            strengthEntry.Name = Name;
            strengthEntry.Intensity = Intensity;
            strengthEntry.ReportStatus = ReportStatus;
            strengthEntry.TrainingPlanId = TrainingPlanId;
            strengthEntry.TrainingDay = TrainingDay;
            strengthEntry.EndTime = EndTime;
            strengthEntry.TrainingPlanItemId = TrainingPlanItemId;
            foreach (StrengthTrainingItemDTO item in Entries)
            {
                strengthEntry.AddEntry(item.Copy());
            }
            return strengthEntry;

        }

        #endregion
    }
}
