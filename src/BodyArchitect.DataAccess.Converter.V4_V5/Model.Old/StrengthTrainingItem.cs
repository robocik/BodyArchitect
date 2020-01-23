using System;
using System.Collections.Generic;


namespace BodyArchitect.Model.Old
{
    [Serializable]
    public class StrengthTrainingItem : FMObject
    {
        public StrengthTrainingItem()
        {
            Series = new Iesi.Collections.Generic.HashedSet<Serie>();
        }

        #region Persistent properties

        public virtual ICollection<Serie> Series { get; set; }

        public virtual int Position
        {
            get; set;
        }

        public virtual Guid ExerciseId
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
