using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Service.V2.Model.Localization;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.V2.Model
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
    [DataContract(Namespace = Const.ServiceNamespace,IsReference = true)]
    public class StrengthTrainingItemDTO : BAGlobalObject, ICommentable, ITrainingPlanConnector
    {
        public StrengthTrainingItemDTO()
        {
            Series = new List<SerieDTO>();
        }

        #region Persistent properties


        [DataMember]
        [NotNullValidator]
        [ObjectCollectionValidator(typeof (SerieDTO))]
        public List<SerieDTO> Series
        {
            get; set;
        }

        [DataMember]
        public ExerciseDoneWay DoneWay
        {
            get;
            set;
        }

        [DataMember]
        public int Position
        {
            get;
            set;
        }

        [DataMember]
        public ExerciseLightDTO Exercise
        {
            get;
            set;
        }

        [DataMember]
        public Guid? TrainingPlanItemId
        {
            get;
            set;
        }
        [DataMember]
        [ValidatorComposition(CompositionType.Or)]
        [StringLengthValidator(0, 30, MessageTemplateResourceName = "StrengthTrainingItemDTO_SuperSetGroup_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        [NotNullValidator(Negated = true)]
        public string SuperSetGroup
        {
            get;
            set;
        }

        [DataMember]
        [ValidatorComposition(CompositionType.Or)]
        [StringLengthValidator(0, Constants.CommentColumnLength, MessageTemplateResourceName = "StrengthTrainingItemDTO_Comment_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        [NotNullValidator(Negated = true)]
        public string Comment
        {
            get;
            set;
        }

        [DataMember]
        [NotNullValidator]
        [ObjectValidator]
        public StrengthTrainingEntryDTO StrengthTrainingEntry
        {
            get;
            set;
        }

        #endregion

        #region Methods

        public virtual void AddSerie(SerieDTO serie)
        {
            serie.StrengthTrainingItem = this;
            Series.Add(serie);
        }

        public virtual void RemoveSerie(SerieDTO serie)
        {
            Series.Remove(serie);
            serie.StrengthTrainingItem = null;
        }

        public virtual StrengthTrainingItemDTO Copy()
        {
            StrengthTrainingItemDTO entry = new StrengthTrainingItemDTO();
            entry.Comment = Comment;
            entry.Exercise = Exercise;
            entry.DoneWay = DoneWay;
            entry.Position = Position;
            entry.DoneWay = DoneWay;
            entry.SuperSetGroup = SuperSetGroup;
            entry.TrainingPlanItemId = TrainingPlanItemId;
            foreach (var serie in Series)
            {
                entry.AddSerie(serie.Copy());
            }
            return entry;
        }
        #endregion

    }
}
