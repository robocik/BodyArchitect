using System;
using System.Collections.Generic;
using BodyArchitect.Shared;

namespace BodyArchitect.Model
{
    public enum Intensity
    {
        NotSet,
        Low,
        Medium,
        Hight
    }
    

    [Serializable]
    [EntryObjectInstance(EntryObjectInstance.Single)]
    [EntryObjectOperationsAttribute(EntryObjectOperation.Copy | EntryObjectOperation.Move)]
    public class StrengthTrainingEntry : EntryObject,  ITrainingPlanConnector
    {
        public static readonly Guid EntryTypeId = new Guid("001B7C05-29DC-475E-8B69-7F3B44729742");

        public StrengthTrainingEntry()
        {
            Entries = new HashSet<StrengthTrainingItem>();
        }

        #region Persistent properties

        public virtual ICollection<StrengthTrainingItem> Entries { get; set; }

        public virtual Mood Mood { get; set; }

        public virtual DateTime? EndTime
        {
            get; set;
        }

        public virtual MyPlace MyPlace { get; set; }

        /// <summary>
        /// Workout plan day id
        /// </summary>
        public virtual Guid? TrainingPlanItemId
        {
            get;
            set;
        }
        /// <summary>
        /// Workout plan id
        /// </summary>
        public virtual Guid? TrainingPlanId
        {
            get;
            set;
        }

        public virtual DateTime? StartTime
        {
            get; set;
        }

        public virtual Intensity Intensity { get; set; }

        #endregion

        #region Methods

        public virtual void AddEntry(StrengthTrainingItem entry)
        {
            entry.StrengthTrainingEntry = this;
            Entries.Add(entry);
        }

        public virtual void RemoveEntry(StrengthTrainingItem entry)
        {
            Entries.Remove(entry);
            entry.StrengthTrainingEntry = null;
        }

        public override Guid TypeId
        {
            get { return EntryTypeId; }
        }

        public override bool IsEmpty
        {
            get { return Status==EntryObjectStatus.Done && Entries.Count == 0; }
        }
        public virtual int GetMaximumSeriesCount()
        {
            int max = 0;
            foreach (StrengthTrainingItem entry in Entries)
            {
                if (max < entry.Series.Count)
                {
                    max = entry.Series.Count;
                }
            }
            return max;
        }

        
        #endregion

        
    }
}
