using System;
using System.Runtime.Serialization;
using BodyArchitect.Shared;

namespace BodyArchitect.Service.V2.Model.TrainingPlans
{
    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class TrainingPlanSerie : BAGlobalObject, ICommentable
    {
        private SetType muscleFailure;
        private int? repetitionNumberMin;
        private int? repetitionNumberMax;
        public const string Separator="-";
        private DropSetType dropSet;

        public TrainingPlanSerie()
        {
        }

        public TrainingPlanSerie(int repetitionNumber)
        {
            this.repetitionNumberMin = repetitionNumber;
            this.repetitionNumberMax = repetitionNumber;
        }

        public TrainingPlanSerie(SetType repetitions)
        {
            muscleFailure = repetitions;
        }

        public TrainingPlanSerie(int repetitionNumberMin, int repetitionNumberMax)
        {
            this.repetitionNumberMin = repetitionNumberMin;
            this.repetitionNumberMax = repetitionNumberMax;
        }
        [DataMember]
        public int? RepetitionNumberMin
        {
            get { return repetitionNumberMin; }
            set { repetitionNumberMin = value; }
        }

        [DataMember]
        public bool IsRestPause { get; set; }

        [DataMember]
        public bool IsSuperSlow { get; set; }

        [DataMember]
        public int? RepetitionNumberMax
        {
            get { return repetitionNumberMax; }
            set { repetitionNumberMax = value; }
        }
        [DataMember]
        public string Comment
        {
            get;
            set;
        }
        [DataMember]
        public SetType RepetitionsType
        {
            get { return muscleFailure; }
            set { muscleFailure = value; }
        }
        [DataMember]
        public DropSetType DropSet
        {
            get { return dropSet; }
            set { dropSet = value; }
        }

        public string ToStringRepetitionsRange()
        {
            string format = "{0}-{1}";
            if (repetitionNumberMin == repetitionNumberMax)
            {
                format = "{0}";
            }
            else if (repetitionNumberMin == null)
            {
                format = "-{1}";
            }
            else if (repetitionNumberMax == null)
            {
                format = "{0}-";
            }
            return string.Format(format, repetitionNumberMin, repetitionNumberMax);
        }
        //public override string ToString()
        //{
            //string dropSetString = "";
            //if (dropSet!=DropSetType.None)
            //{
            //    dropSetString = "^" + (int)dropSet;
            //}
            //if(RepetitionsType!=TrainingPlanSerieRepetitions.NotSet)
            //{
            //    var localier = new EnumLocalizer(LocalizedPropertyGridStrings.ResourceManager);
            //    return localier.Translate(RepetitionsType) + dropSetString;
            //}
            //string format = ToStringRepetitionsRange();

            //return format + dropSetString;

        //}

        public void FromString(string text)
        {
            if(string.IsNullOrWhiteSpace(text))
            {
                RepetitionNumberMax = RepetitionNumberMin = null;
                //RepetitionsType = TrainingPlanSerieRepetitions.Normalna;
                return;
            }
            int? repMin, repMax;
            int index = text.IndexOf(Separator);
            if (index > -1)
            {
                int tempMin;
                
                if (int.TryParse(text.Substring(0, index), out tempMin))
                {
                    repMin = tempMin;
                }
                else
                {
                    repMin = null;
                }
                string maxString = text.Substring(index + 1, text.Length - index - 1);
                int tempMax;
                if (int.TryParse(maxString, out tempMax))
                {
                    repMax = tempMax;
                }
                else
                {
                    repMax = null;
                }
                if (repMin != null && repMax != null && repMin.Value > repMax.Value)
                {
                    throw new ArgumentException();
                }
            }
            else
            {
                repMin=repMax=int.Parse(text);
            }
            RepetitionNumberMax = repMax;
            RepetitionNumberMin = repMin;
        }
    }
}
