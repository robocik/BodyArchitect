using System;
using System.Collections.Generic;
using BodyArchitect.Shared;

namespace BodyArchitect.Model
{
    public enum ExerciseDoneWay
    {
        Default,
        Barbell,
        Dumbbell,
        Cable,
        Machine
    }

    [Serializable]
    public class StrengthTrainingItem : FMGlobalObject, ICommentable, ITrainingPlanConnector
    {
        public StrengthTrainingItem()
        {
            Series = new HashSet<Serie>();
        }

        #region Persistent properties

        public virtual ICollection<Serie> Series { get; set; }

        public virtual ExerciseDoneWay DoneWay { get; set; }

        public virtual int Position
        {
            get; set;
        }

        public virtual Exercise Exercise
        {
            get; set;
        }

        public virtual Guid? TrainingPlanItemId
        {
            get;
            set;
        }
        public virtual string SuperSetGroup
        {
            get;
            set;
        }

        public virtual string Comment
        {
            get; set;
        }

        public virtual StrengthTrainingEntry StrengthTrainingEntry
        {
            get; set; 
        }

        #endregion

        #region Methods

        public virtual void AddSerie(Serie serie)
        {
            serie.StrengthTrainingItem = this;
            Series.Add(serie);
        }

        public virtual void RemoveSerie(Serie serie)
        {
            Series.Remove(serie);
            serie.StrengthTrainingItem = null;
        }

 
        #endregion

    }
}
