using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.V2.Model
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
    [EntryObjectInstance(EntryObjectInstance.Multiple)]
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
        public Mood Mood
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? EndTime
        {
            get;
            set;
        }

        /// <summary>
        ///Workout plan day id
        /// </summary>
        [DataMember]
        public Guid? TrainingPlanItemId
        {
            get;
            set;
        }

        /// <summary>
        /// Workout plan id
        /// </summary>
        [DataMember]
        public Guid? TrainingPlanId
        {
            get;
            set;
        }

        [DataMember]
        public MyPlaceLightDTO MyPlace { get; set; }

        [DataMember]
        public DateTime? StartTime
        {
            get;
            set;
        }

        [DataMember]
        public Intensity Intensity { get; set; }

        #endregion

        #region Methods

        public void AddEntry(StrengthTrainingItemDTO entry)
        {
            entry.StrengthTrainingEntry = this;
            Entries.Add(entry);
        }

        public void RemoveEntry(StrengthTrainingItemDTO entry)
        {
            Entries.Remove(entry);
            entry.StrengthTrainingEntry = null;
        }

        public void RepositionEntry(int index1, int index2)
        {
            var item = Entries[index1];
            Entries.Remove(item);
            Entries.Insert(index2, item);
            item.Position = index2;
            Entries[index1].Position = index2;
        }

        public override bool IsEmpty
        {
            get { return Status==EntryObjectStatus.Done && Entries.Count == 0; }
        }
        public int GetMaximumSeriesCount()
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

        public object Clone()
        {
            StrengthTrainingEntryDTO strengthEntry = new StrengthTrainingEntryDTO();
            strengthEntry.StartTime = StartTime;
            strengthEntry.Comment = Comment;
            strengthEntry.Name = Name;
            strengthEntry.Intensity = Intensity;
            strengthEntry.Mood = Mood;
            strengthEntry.Status = Status;
            strengthEntry.Intensity = Intensity;

            strengthEntry.ReportStatus = ReportStatus;
            strengthEntry.TrainingPlanId = TrainingPlanId;
            strengthEntry.TrainingDay = TrainingDay;
            strengthEntry.MyPlace = MyPlace;
            strengthEntry.EndTime = EndTime;
            strengthEntry.TrainingPlanItemId = TrainingPlanItemId;
            strengthEntry.RemindBefore = RemindBefore;
            foreach (StrengthTrainingItemDTO item in Entries)
            {
                strengthEntry.AddEntry(item.Copy());
            }
            return strengthEntry;

        }

        #endregion
    }
}
