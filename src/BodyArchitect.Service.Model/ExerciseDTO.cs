using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Service.Model.Localization;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.Model
{
    public enum ExerciseDifficult
    {
        NotSet=0,
        One=1,
        Two=2,
        Three=3,
        Four=4,
        
    }

    public enum MechanicsType
    {
        NotSet,
        Compound,
        Isolation
    }

    public enum ExerciseForceType
    {
        NotSet,
        Push,
        Pull,
        Static
    }

    public enum ExerciseType
    {
        NotSet = 0,
        Biceps = 1,
        Klatka = 2,
        Plecy = 3,
        Triceps = 4,
        Barki = 5,
        Nogi = 6,
        Lydki = 7,
        Przedramie = 8,
        Brzuch = 9,
        Czworoboczny = 10,
        Cardio=11
    }
    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace)]
    [DebuggerDisplay("Shortcut={Shortcut},Name = {Name}")]
    public class ExerciseDTO : IRatingable, IBelongToUser,IVersionable
    {
        private ExerciseDifficult difficult = ExerciseDifficult.One;
        public readonly static ExerciseDTO Removed;

        static ExerciseDTO()
        {
            Removed = new ExerciseDTO(Guid.Empty);
            Removed.Name = "";
        }

        public ExerciseDTO()
        {
        }

        public ExerciseDTO(Guid globalId)
        {
            this.GlobalId = globalId;
        }


        #region Persistent properties

        [DataMember]
        public float Rating
        {
            get;
            set;
        }

        [DataMember]
        public string UserShortComment { get; set; }

        [DataMember]
        public float? UserRating
        {
            get;
            set;
        }

        [DataMember]
        [NotNullValidator(MessageTemplateResourceName = "ExerciseDTO_Name_NotNull", MessageTemplateResourceType = typeof(ValidationStrings))]
        [StringLengthValidator(1, Constants.NameColumnLength, MessageTemplateResourceName = "ExerciseDTO_Name_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        public string Name
        {
            get;
            set;
        }

        [DataMember]
        public UserDTO Profile{ get; set; }

        [DataMember]
        [ValidatorComposition(CompositionType.Or)]
        [StringLengthValidator(0, Constants.CommentColumnLength, MessageTemplateResourceName = "ExerciseDTO_Description_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        [NotNullValidator(Negated = true)]
        public string Description
        {
            get;
            set;
        }

        [DataMember]
        [ValidatorComposition(CompositionType.Or)]
        [StringLengthValidator(0, Constants.UrlLength, MessageTemplateResourceName = "ExerciseDTO_Url_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        [NotNullValidator(Negated = true)]
        public string Url
        {
            get;
            set;
        }

        [DataMember]
        public int Version { get; private set; }

        [DataMember]
        public MechanicsType MechanicsType
        {
            get;
            set;
        }

        [DataMember]
        public ExerciseForceType ExerciseForceType
        {
            get;
            set;
        }

        [DataMember]
        public ExerciseType ExerciseType
        {
            get;
            set;
        }


        [DataMember]
        public Guid GlobalId
        {
            get;
            set;
        }

        [DataMember]
        [NotNullValidator(MessageTemplateResourceName = "ExerciseDTO_Shortcut_NotNull", MessageTemplateResourceType = typeof(ValidationStrings))]
        [StringLengthValidator(1,20, MessageTemplateResourceName = "ExerciseDTO_Shortcut_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        public string Shortcut
        {
            get;
            set;
        }

        [DataMember]
        public PublishStatus Status { get; set; }

        [DataMember]
        public ExerciseDifficult Difficult
        {
            get { return difficult; }
            set { difficult = value; }
        }
        #endregion

        
    }
}
