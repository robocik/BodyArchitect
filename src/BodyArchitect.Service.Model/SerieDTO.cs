using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Service.Model.Localization;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.Model
{
    public enum DropSetType
    {
        None,
        IDropSet,
        IIDropSet,
        IIIDropSet,
        IVDropSet,
    }

    public enum SetType
    {
        Normalna,
        Max,
        MuscleFailure,
        Rozgrzewkowa,
        PrawieMax
       
    }

    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class SerieDTO : BAObject
    {

        public const char SerieSeparator = 'x';

        public SerieDTO()
        {
        }

        //for testing only
        public SerieDTO(string serie)
        {
            TimeSpan span;

            if (TimeSpan.TryParse(serie, out span))
            {//for cardio
                RepetitionNumber = (int)span.TotalSeconds;
                return;
            }
            int index = serie.IndexOf(SerieDTO.SerieSeparator);
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

        public virtual bool IsEmpty
        {
            get { return !RepetitionNumber.HasValue && !Weight.HasValue; }
        }


        [DataMember]
        public DropSetType DropSet { get; set; }

        [DataMember]
        [NotNullValidator]
        [ObjectValidator]
        public StrengthTrainingItemDTO StrengthTrainingItem { get; set; }

        [DataMember]
        public int? RepetitionNumber
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? StartTime { get; set; }

        [DataMember]
        public DateTime? EndTime { get; set; }

        [DataMember]
        public float? Weight
        {
            get;
            set;
        }


        public override string ToString()
        {
            return string.Format("{0}x{1}", RepetitionNumber, Weight);
        }

        

        [DataMember]
        [ValidatorComposition(CompositionType.Or)]
        [StringLengthValidator(0, Constants.CommentColumnLength, MessageTemplateResourceName = "SerieDTO_Comment_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        [NotNullValidator(Negated = true)]
        public string Comment
        {
            get;
            set;
        }

        [DataMember]
        public bool IsCiezarBezSztangi
        {
            get;
            set;
        }

        [DataMember]
        public SetType SetType
        {
            get;
            set;
        }

        public SerieDTO Copy()
        {
            SerieDTO serie = new SerieDTO();
            serie.Comment = Comment;
            serie.IsCiezarBezSztangi = IsCiezarBezSztangi;
            serie.SetType = SetType;
            serie.RepetitionNumber = RepetitionNumber;
            serie.Weight = Weight;
            serie.EndTime = EndTime;
            serie.StartTime = StartTime;
            serie.DropSet = DropSet;
            serie.TrainingPlanItemId = TrainingPlanItemId;
            return serie;
        }

        [DataMember]
        public Guid? TrainingPlanItemId
        {
            get;
            set;
        }
    }
}
