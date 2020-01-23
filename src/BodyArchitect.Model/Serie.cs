using System;
using System.Runtime.Serialization;
using BodyArchitect.Model;
using BodyArchitect.Shared;

namespace BodyArchitect.Model
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
    public class Serie : FMGlobalObject, ICommentable, ITrainingPlanConnector
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

        //wilks for strength training and calories for cardio
        public virtual decimal? CalculatedValue { get; set; }

        public virtual DateTime? StartTime { get; set; }
        public virtual DateTime? EndTime { get; set; }

        public virtual DropSetType DropSet { get; set; }

        public virtual bool IsRestPause { get; set; }

        public virtual bool IsSuperSlow { get; set; }

        public virtual SetType SetType { get; set; }

        public virtual StrengthTrainingItem StrengthTrainingItem { get; set; }

        public virtual ExerciseProfileData ExerciseProfileData { get; set; }

        public virtual int Position
        {
            get;
            set;
        }

        public virtual decimal? RepetitionNumber
        {
            get; set;
        }

        public virtual decimal? Weight
        {
            get;
            set;
        }

        public virtual bool IsIncorrect { get; set; }

        public override string ToString()
        {
            if (StrengthTrainingItem.Exercise.ExerciseType==ExerciseType.Cardio)
            {
                decimal seconds = Weight.HasValue ? Weight.Value : 0;
                var time = TimeSpan.FromSeconds((double)seconds);
                return time.ToString();
            }
            return string.Format("{0:#}x{1:#.##}", RepetitionNumber, Weight);
        }

        public virtual void SetFromString(string serie)
        {
            if(string.IsNullOrWhiteSpace(serie))
            {
                RepetitionNumber = null;
                Weight = null;
                return;
            }
            int index = serie.IndexOf(SerieSeparator);
            decimal tempRepetition;
            if (decimal.TryParse(serie.Substring(0, index), out tempRepetition))
            {
                RepetitionNumber = tempRepetition;
            }
            else
            {
                RepetitionNumber = null;
            }
            string weightString = serie.Substring(index + 1, serie.Length - index - 1);
            decimal tempWeight;
            if (decimal.TryParse(weightString, out tempWeight))
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
