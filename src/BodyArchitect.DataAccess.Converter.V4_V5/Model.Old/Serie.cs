using System;
using System.Runtime.Serialization;
using BodyArchitect.Model.Old;


namespace BodyArchitect.Model.Old
{
    public enum SetType
    {
        Normalna,
        Rozgrzewkowa,
        PrawieMax,
        Max,
        MuscleFailure
    }

    [Serializable]
    public class Serie : FMObject
    {

        public const char SerieSeparator = 'x';

        public Serie()
        {
        }

        public virtual bool IsEmpty
        {
            get { return !RepetitionNumber.HasValue && !Weight.HasValue; }
        }

        public Serie(string serie)
        {
            SetFromString(serie);
        }

        public virtual DateTime? StartTime { get; set; }
        public virtual DateTime? EndTime { get; set; }

        public virtual DropSetType DropSet { get; set; }

        public virtual SetType SetType { get; set; }

        public virtual StrengthTrainingItem StrengthTrainingItem { get; set; }

        public virtual int? RepetitionNumber
        {
            get; set;
        }

        public virtual float? Weight
        {
            get;
            set;
        }


        public override string ToString()
        {
            return string.Format("{0}x{1}", RepetitionNumber, Weight);
        }

        public virtual void SetFromString(string serie)
        {
            int index = serie.IndexOf(SerieSeparator);
            int tempRepetition;
            if (int.TryParse(serie.Substring(0, index), out tempRepetition))
            {
                RepetitionNumber = tempRepetition;
            }
            else
            {
                RepetitionNumber = null;
            }
            string weightString = serie.Substring(index + 1, serie.Length - index - 1);
            float tempWeight;
            if (float.TryParse(weightString, out tempWeight))
            {
                Weight = tempWeight;
            }
            else
            {
                Weight = null;
            }
        }

        //[Property(SqlType = "ntext", ColumnType = "StringClob")]
        public virtual string Comment
        {
            get; set;
        }

        //[Property(NotNull = true)]
        public virtual bool IsCiezarBezSztangi
        {
            get; set;
        }


        //[Property]
        public virtual Guid? TrainingPlanItemId
        {
            get; set;
        }
    }
}
